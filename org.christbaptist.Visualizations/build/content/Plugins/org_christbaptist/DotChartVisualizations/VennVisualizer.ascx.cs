// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Security;
using System.Data.Entity;
using org.christbaptist.Visualizations.Model;

namespace RockWeb.Plugins.org_christbaptist.Visualizers
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Venn Visualizer")]
    [Category("Christ Baptist > Visualizations")]
    [Description("View up to three dataviews in a Venn-style visual")]

    [TextField("EntityTypeId", "The entity type Id of the entities represented in this visualization", false, "15", "CustomSetting")]

    [TextField("EntityUrl", "The URL to visit when an entity is clicked", false, "/person/{{Id}}", "CustomSetting")]
    [TextField("DataViewUrl", "The URL to visit when a dataview is clicked", false, "/page/145?DataViewId={{Id}}", "CustomSetting")]

    [TextField("Bucket1", "Data view 1", false, "", "CustomSetting")]
    [TextField("Bucket2", "Data views 2", false, "", "CustomSetting")]
    [TextField("Bucket3", "Data views 3", false, "", "CustomSetting")]

    [TextField("Filters", "Data Views to highlight on the visualization", false, "[]", "CustomSetting")]
    [TextField("ShowFilterKey", "ShowFilterKey", false, "true", "CustomSetting")]
    [TextField("Title", "The title to use on the visualization", false, "", "CustomSetting")]
    [TextField("SummaryLava", "What to display when you hover over a person's circle", false, @"<div class='text-center'>
            <img src =""{{Row.PhotoUrl}}"" style = ""max-width: 200px;"" />
            {%for filter in Filters %}  
                <div style=""width: 100 %; background-color: pink; padding: 10px;margin-top: 10px;"">
                    <i class=""fa fa-exclamation-circle"" style=""color: red;""></i> {{ filter.DisplayAs }}
                </div>
            {% endfor %}
            <h2>{{ Row.NickName }} {{ Row.LastName }}</h2>
            <p>{{ Row.ConnectionStatusValue.Value }}</p>
        </div>", "CustomSetting")]
    public partial class VennVisualizer : Rock.Web.UI.RockBlockCustomSettings
    {
        class EntityEntry
        {
            public int Id;
        }

        #region Fields

        // used for private variables
        Dictionary<string, Filter> filters
        {
            get
            {
                return (Dictionary<string, Filter>)ViewState["filters"] ?? new Dictionary<string, Filter>();
            }

            set
            {
                ViewState["filters"] = value;
            }
        }

        #endregion

        #region Properties

        public string bucketsAsJson
        {
            get
            {
                List<Bucket> buckets = new List<Bucket>();

                // Check PageParameter first
                string BucketParameter = PageParameter("buckets");
                if (BucketParameter.IsNullOrWhiteSpace())
                {
                    buckets.Add(GetAttributeValue("Bucket1").FromJsonOrNull<Bucket>());
                    buckets.Add(GetAttributeValue("Bucket2").FromJsonOrNull<Bucket>());
                    buckets.Add(GetAttributeValue("Bucket3").FromJsonOrNull<Bucket>());
                } else
                {
                    string[] bucketIds = BucketParameter.Split(',').ToArray();

                    foreach (string bucketId in bucketIds)
                    {
                        buckets.Add(new Bucket { Id = bucketId.AsInteger() });
                    }
                }


                var rockContext = new RockContext();
                var dataViewService = new DataViewService(rockContext);
                SortProperty sortBy = new SortProperty();
                sortBy.Property = "Id";
                sortBy.Direction = SortDirection.Ascending;

                var errorMessages = new List<string>();

                foreach (Bucket bucket in buckets)
                {
                    if (bucket != null && bucket.Id != 0)
                    {

                        DataView dataView = dataViewService.Get(bucket.Id);

                        if (bucket.Name.IsNullOrWhiteSpace())
                        {
                            bucket.Name = dataView.Name;
                        }

                        if (dataView.EntityTypeId.HasValue && dataView.IsAuthorized(Authorization.VIEW, CurrentPerson))
                        {
                            bucket.data = dataView.GetQuery(sortBy, 180, out errorMessages).Select(o => new { Id = o.Id }).ToArray();
                        }
                    }
                }

                return buckets.Where((item) => item.IsNotNull() && item.Id != 0).ToJson();
            }
        }

        public string filtersAsJson
        {
            get
            {
                Dictionary<string, Filter> filters = GetAttributeValue("Filters").FromJsonOrNull<Dictionary<string, Filter>>();

                if (filters == null)
                {
                    return "[]";
                }

                List<Filter> filterList = filters.Values.OrderBy((filter) => filter.Order).ToList();

                var rockContext = new RockContext();
                var dataViewService = new DataViewService(rockContext);
                SortProperty sortBy = new SortProperty();
                sortBy.Property = "Id";
                sortBy.Direction = SortDirection.Ascending;

                var errorMessages = new List<string>();

                foreach (Filter filter in filterList)
                {
                    DataView dataView = dataViewService.Get(filter.Id.AsInteger()); 

                    if (dataView.EntityTypeId.HasValue && dataView.IsAuthorized(Authorization.VIEW, CurrentPerson))
                    {
                        filter.data = dataView.GetQuery(sortBy, 180, out errorMessages).Select(o => new { Id = o.Id }).ToArray();
                    }
                }

                return filterList.ToJson();
            }
        }

        public string entityUrl
        {
            get
            {
                return GetAttributeValue("EntityUrl").ToJson();
            }
        }

        public string bucketUrl
        {
            get
            {
                return GetAttributeValue("DataViewUrl").ToJson();
            }
        }

        // used for public / protected properties

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger(upnlContent);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                filters = GetAttributeValue("Filters").FromJsonOrNull<Dictionary<string, Filter>>() ?? new Dictionary<string, Filter>();
            }

            if (!Page.IsPostBack)
            {
                // added for your convenience

                // to show the created/modified by date time details in the PanelDrawer do something like this:
                // pdAuditDetails.SetEntity( <YOUROBJECT>, ResolveRockUrl( "~" ) );
            }
        }

        protected override void ShowSettings()
        {
            pnlConfigure.Visible = true;

            txtVisualizationTitle.Text = GetAttributeValue("Title");

            //var allowedEntities = new List<int>
            //{
            //    15, // Person
            //    16, // Group
            //    113, // Workflow
            //    240, // Connection Request
            //    258, // Registration
            //    313 //Registration Registrant
            //};

            etpEntityType.EntityTypes = new EntityTypeService(new RockContext())
                .Queryable()
                .OrderBy(t => t.FriendlyName).ToList();
            etpEntityType.SelectedEntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();

            tbEntityUrl.Text = GetAttributeValue("EntityUrl");
            tbDataViewUrl.Text = GetAttributeValue("DataViewUrl");
            tbSummaryLava.Text = GetAttributeValue("SummaryLava");

            dvBucket1.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();
            dvBucket2.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();
            dvBucket3.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();

            // Load Data Views
            Bucket bucket1 = GetAttributeValue("Bucket1").FromJsonOrNull<Bucket>();
            Bucket bucket2 = GetAttributeValue("Bucket2").FromJsonOrNull<Bucket>();
            Bucket bucket3 = GetAttributeValue("Bucket3").FromJsonOrNull<Bucket>();

            if (bucket1 != null)
            {
                dvBucket1.SetValue(bucket1.Id);
                tbBucket1Color.Text = bucket1.Color;
            }
            if (bucket2 != null)
            {
                dvBucket2.SetValue(bucket2.Id);
                tbBucket2Color.Text = bucket2.Color;
            }
            if (bucket3 != null)
            {
                dvBucket3.SetValue(bucket3.Id);
                tbBucket3Color.Text = bucket3.Color;
            }

            mdConfigure.Show();

            FilterControl.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();
            FilterControl.ShowFilterKey = GetAttributeValue("ShowFilterKey").AsBoolean();
            FilterControl.filters = filters;
            FilterControl.Refresh();

            upnlContent.Update();
        }

        protected void persistForm()
        {
            Bucket bucket1 = new Bucket();
            bucket1.Id = dvBucket1.SelectedValue.AsInteger();
            bucket1.Name = dvBucket1.ItemName;
            bucket1.Color = tbBucket1Color.Text;
            SetAttributeValue("Bucket1", bucket1.ToJson());

            Bucket bucket2 = new Bucket();
            bucket2.Id = dvBucket2.SelectedValue.AsInteger();
            bucket2.Name = dvBucket2.ItemName;
            bucket2.Color = tbBucket2Color.Text;
            SetAttributeValue("Bucket2", bucket2.ToJson());

            Bucket bucket3 = new Bucket();
            bucket3.Id = dvBucket3.SelectedValue.AsInteger();
            bucket3.Name = dvBucket3.ItemName;
            bucket3.Color = tbBucket3Color.Text;
            SetAttributeValue("Bucket3", bucket3.ToJson());

            filters = FilterControl.filters;
            SetAttributeValue("Filters", filters.ToJson());

            SetAttributeValue("EntityTypeId", etpEntityType.SelectedValue);
            SetAttributeValue("Filters", filters.ToJson());
            SetAttributeValue("Title", txtVisualizationTitle.Text);
            SetAttributeValue("SummaryLava", tbSummaryLava.Text);
            SetAttributeValue("EntityUrl", tbEntityUrl.Text);
            SetAttributeValue("DataViewUrl", tbDataViewUrl.Text);
            SetAttributeValue("ShowFilterKey", FilterControl.ShowFilterKey.ToString());

            SaveAttributeValues();
        }

        protected void mdConfigure_SaveClick(object sender, EventArgs e)
        {

            mdConfigure.Hide();
            pnlConfigure.Visible = false;

            persistForm();

            this.Block_BlockUpdated(sender, e);
        }

        protected void rptDataFilters_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Filter filter = e.Item.DataItem as Filter;
            if (filter != null)
            {
                Literal lFilterName = e.Item.FindControl("lFilterName") as Literal;
                HiddenField FilterId = e.Item.FindControl("hfFilterId") as HiddenField;
                HiddenField FilterOrder = e.Item.FindControl("hfSortOrder") as HiddenField;
                RockTextBox tbDisplayAs = e.Item.FindControl("tbDisplayAs") as RockTextBox;
                RockTextBox tbCSS = e.Item.FindControl("tbCSS") as RockTextBox;

                RockDropDownList ddlDisplayStyle = e.Item.FindControl("ddlDisplayStyle") as RockDropDownList;
                RockCheckBox cbActiveByDefault = e.Item.FindControl("cbActiveByDefault") as RockCheckBox;

                FilterId.Value = filter.Id;
                FilterOrder.Value = filter.Order.ToString();
                lFilterName.Text = filter.DataViewName;
                tbDisplayAs.Text = filter.DisplayAs;
                tbCSS.Text = filter.CSS;
                cbActiveByDefault.Checked = filter.ActiveByDefault;

                ScriptManager.RegisterStartupScript(this.Page, typeof(System.Web.UI.Page), "UpdateSelections",
               "$(document).ready(function(){initializeOptions(" + ddlDisplayStyle.ClientID + ")});", true);
            }
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated(object sender, EventArgs e)
        {
            // reload the full page since controls are dynamically created based on block settings
            NavigateToPage(this.CurrentPageReference);
        }

        protected void etpEntityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvBucket1.SetValue(null);
            dvBucket1.EntityTypeId = etpEntityType.SelectedEntityTypeId;
            dvBucket2.SetValue(null);
            dvBucket2.EntityTypeId = etpEntityType.SelectedEntityTypeId;
            dvBucket3.SetValue(null);
            dvBucket3.EntityTypeId = etpEntityType.SelectedEntityTypeId;
            
            FilterControl.EntityTypeId = etpEntityType.SelectedEntityTypeId ?? 0;
            FilterControl.filters = filters;
            FilterControl.Refresh();

            persistForm();
        }

        #endregion

        #region Methods

        // helper functional methods (like BindGrid(), etc.)

        #endregion
    }
}