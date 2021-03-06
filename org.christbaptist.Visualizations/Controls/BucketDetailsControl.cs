﻿using org.christbaptist.Visualizations.Model;
using Rock;
using Rock.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace org.christbaptist.Visualizations.Controls
{
    class BucketItemTemplate : ITemplate
    {
        ListItemType _type;

        public BucketItemTemplate(ListItemType type)
        {
            _type = type;
        }

        public void InstantiateIn(Control container)
        {
            switch (_type)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    HiddenField hfBucketId = new HiddenField();
                    hfBucketId.ID = "hfBucketId";

                    HiddenField hfSortOrder = new HiddenField();
                    hfSortOrder.ID = "hfSortOrder";

                    Panel bucketPanel = new Panel();
                    bucketPanel.ID = "pnlBucket";
                    bucketPanel.AddCssClass("sccDvvFilterSection");
                    bucketPanel.Attributes.Add("ondragover", "dragover_handler(event)");
                    bucketPanel.Attributes.Add("ondrop", "drop_handler(event)");

                    bucketPanel.Controls.Add(hfBucketId);
                    bucketPanel.Controls.Add(hfSortOrder);

                    // Title Bar
                    bucketPanel.Controls.Add(new LiteralControl(@"<div class='sccDvvFilterSection_title'>
                        <div class='reorder' draggable='true' ondragstart='dragstart_handler(event)' style='padding-right: 15px;'>
                            <i class='fa fa-bars'></i>
                        </div>"));
                    Literal lBucketName = new Literal();
                    lBucketName.ID = "lBucketName";
                    bucketPanel.Controls.Add(lBucketName);
                    bucketPanel.Controls.Add(new LiteralControl("</div>"));

                    bucketPanel.Controls.Add(new LiteralControl(@"<div class='row js-filterconfig-row'>
                        <div class='col-md-4'>"));

                    // Content
                    RockTextBox tbDisplayName = new RockTextBox();
                    tbDisplayName.ID = "tbDisplayName";
                    tbDisplayName.Label = "DisplayAs";
                    tbDisplayName.CssClass = "js-settings-pre-html";
                    tbDisplayName.Help = "The name to show for the bucket";
                    tbDisplayName.ValidateRequestMode = ValidateRequestMode.Disabled;

                    bucketPanel.Controls.Add(tbDisplayName);
                    bucketPanel.Controls.Add(new LiteralControl("</div></div>"));


                    container.Controls.Add(bucketPanel);
                    break;
            }
        }
    }

    public partial class BucketDetailsControl : System.Web.UI.UserControl
    {
        private Repeater rptBucketRepeater;
        private System.ComponentModel.IContainer components;

        private List<Bucket> _buckets;

        public BucketDetailsControl()
        {
            components = new System.ComponentModel.Container();

            rptBucketRepeater = new Repeater();
            rptBucketRepeater.ItemTemplate = new BucketItemTemplate(ListItemType.Item);
            rptBucketRepeater.ItemDataBound += rptBucketRepeater_ItemDataBound;

            Controls.Add(rptBucketRepeater);
        }

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
