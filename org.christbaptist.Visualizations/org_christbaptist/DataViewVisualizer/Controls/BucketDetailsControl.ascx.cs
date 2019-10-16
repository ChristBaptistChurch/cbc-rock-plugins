using org.christbaptist.Visualizations.Model;
using Rock;
using Rock.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace org.christbaptist.Visualizations
{

    public partial class BucketDetailsControl : System.Web.UI.UserControl
    {
        private List<Bucket> _buckets;

        public List<Bucket> buckets
        {
            get
            {
                LoadValuesFromPage();
                return _buckets;
            }
            set { _buckets = value; }
        }

        protected override object SaveControlState()
        {
            object[] controlState = new object[2];
            controlState[0] = base.SaveControlState();
            controlState[1] = _buckets;
            return controlState;
        }

        protected override void LoadControlState(object savedState)
        {
            object[] controlState = (object[])savedState;
            base.LoadControlState(controlState[0]);
            _buckets = (List<Bucket>)controlState[1];
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void LoadValuesFromPage()
        {
            foreach (var item in rptBucketRepeater.Items.OfType<RepeaterItem>())
            {
                HiddenField BucketId = item.FindControl("hfBucketId") as HiddenField;
                HiddenField BucketOrder = item.FindControl("hfSortOrder") as HiddenField;
                RockTextBox tbDisplayName = item.FindControl("tbDisplayName") as RockTextBox;

                Bucket thisBucket = _buckets.Find((b) => b.Id == BucketId.ValueAsInt());

                thisBucket.DisplayAs = tbDisplayName.Text;
                thisBucket.Order = Int32.Parse(BucketOrder.Value);
            }
        }

        protected void rptBucketRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Bucket bucket = e.Item.DataItem as Bucket;
            if (bucket != null)
            {
                Literal lBucketName = e.Item.FindControl("lBucketName") as Literal;
                HiddenField BucketId = e.Item.FindControl("hfBucketId") as HiddenField;
                HiddenField BucketOrder = e.Item.FindControl("hfSortOrder") as HiddenField;
                RockTextBox tbDisplayName = e.Item.FindControl("tbDisplayName") as RockTextBox;

                BucketId.Value = bucket.Id.ToString();
                BucketOrder.Value = bucket.Order.ToString();
                lBucketName.Text = bucket.Name;
                tbDisplayName.Text = bucket.DisplayAs;
            }
        }

        public void Refresh()
        {
            rptBucketRepeater.DataSource = _buckets;
            rptBucketRepeater.DataBind();
        }
    }

}
