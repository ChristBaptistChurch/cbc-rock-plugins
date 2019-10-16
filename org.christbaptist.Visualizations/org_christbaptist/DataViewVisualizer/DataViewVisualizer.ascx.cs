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
using org.christbaptist.Visualizations.Model;

namespace RockWeb.Plugins.org_christbaptist.Visualizers
{
    public class EntityEntry
    {
        public int Id;
    }

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Data View Visualizer" )]
    [Category("Christ Baptist > Visualizations")]
    [Description( "Visual diagrams to represent data views" )]

    [TextField("EntityTypeId", "The entity type Id of the entities represented in this visualization", false, "15", "CustomSetting")]

    [TextField("EntityUrl", "The URL to visit when an entity is clicked", false, "/person/{{Id}}", "CustomSetting")]
    [TextField("DataViewUrl", "The URL to visit when a dataview is clicked", false, "/page/145?DataViewId={{Id}}", "CustomSetting")]

    [TextField("Style", "The default chart style", false, "circle", "CustomSetting")]

    [TextField("Buckets", "Data views to form the basis of the visualization", false, "[]", "CustomSetting")]
    [TextField("Filters", "Data Views to highlight on the visualization", false, "[]", "CustomSetting")]
    [TextField("ShowFilterKey", "ShowFilterKey", false, "true", "CustomSetting")]
    [TextField("Title", "The title to use on the visualization", false, "", "CustomSetting")]
    [TextField("SummaryLava", "What to display when you hover over a person's circle", false, @"<div class='text-center'>
            <img src =""{{Row.PhotoUrl}}"" style = ""max-width: 200px;"" />
            {%for filter in Filters %}
                <div style=""width: 100 %; background-color: pink; padding: 10px; margin-top: 10px;"">
                    <i class=""fa fa-exclamation-circle"" style=""color: red;""></i> {{ filter.DisplayAs }}
                </div>
            {% endfor %}
            <h2>{{ Row.NickName }} {{ Row.LastName }}</h2>
            <p>{{ Row.ConnectionStatusValue.Value }}</p>
        </div>", "CustomSetting")]
    public partial class DataViewVisualizer : Rock.Web.UI.RockBlockCustomSettings
    {
        #region Fields

        // used for private variables
        Dictionary<string, Filter> filters
        {
            get
            {
                return (Dictionary<string, Filter>) ViewState["filters"] ?? new Dictionary<string, Filter>();
            }

            set
            {
                ViewState["filters"] = value;
            }
        }

        List<Bucket> buckets
        {
            get
            {
                return (List<Bucket>)ViewState["buckets"] ?? new List<Bucket>();
            }

            set
            {
                ViewState["buckets"] = value;
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
                    buckets = GetAttributeValue("Buckets").FromJsonOrNull<List<Bucket>>();
                }
                else
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

                return buckets.Where((item) => item.IsNotNull() && item.Id != 0).OrderBy(b => b.Order).ToJson();
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
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            Page.MaintainScrollPositionOnPostBack = true;

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;    
            this.AddConfigurationUpdateTrigger( upnlContent );
        }
        
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            
            if (!Page.IsPostBack)
            {
                filters = GetAttributeValue("Filters").FromJsonOrNull<Dictionary<string, Filter>>() ?? new Dictionary<string, Filter>();
                buckets = GetAttributeValue("Buckets").FromJsonOrNull<List<Bucket>>() ?? new List<Bucket>();
            }

            if ( !Page.IsPostBack )
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

            etpEntityType.EntityTypes = new EntityTypeService(new RockContext())
                .Queryable()
                .OrderBy(t => t.FriendlyName).ToList();
            etpEntityType.SelectedEntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();

            dvDataViewBucketPicker.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();

            // Load Data Views
            List<Bucket> buckets = GetAttributeValue("Buckets").FromJsonOrNull<List<Bucket>>() ?? new List<Bucket>();
            var dataViewsOptions = from ListItem li in dvDataViewBucketPicker.Items
                        where buckets.Any((bucket) => { return bucket.Id == li.Value.AsInteger(); })
                        select li;
            foreach (ListItem li in dataViewsOptions)
            {
                li.Selected = true;
            }

            tbSummaryLava.Text = GetAttributeValue("SummaryLava");
            ddlDefaultStyle.SelectedValue = GetAttributeValue("Style");            

            mdConfigure.Show();

            BucketDetailsControl.buckets = buckets;
            BucketDetailsControl.Refresh();
            upnlBuckets.Update();

            FilterControl.EntityTypeId = GetAttributeValue("EntityTypeId").AsInteger();
            FilterControl.ShowFilterKey = GetAttributeValue("ShowFilterKey").AsBoolean();
            FilterControl.filters = filters;
            FilterControl.Refresh();

            upnlContent.Update();
        }

        protected void persistForm()
        {
            buckets = BucketDetailsControl.buckets;
            filters = FilterControl.filters;

            SetAttributeValue("Buckets", buckets.ToJson());
            SetAttributeValue("Filters", filters.ToJson());

            SetAttributeValue("Title", txtVisualizationTitle.Text);
            SetAttributeValue("SummaryLava", tbSummaryLava.Text);
            SetAttributeValue("Style", ddlDefaultStyle.SelectedValue);
            SetAttributeValue("EntityUrl", tbEntityUrl.Text);
            SetAttributeValue("DataViewUrl", tbDataViewUrl.Text);
            SetAttributeValue("ShowFilterKey", FilterControl.ShowFilterKey.ToString());
            SetAttributeValue("EntityTypeId", etpEntityType.SelectedValue);

            SaveAttributeValues();
        }

        protected void mdConfigure_SaveClick(object sender, EventArgs e)
        {

            mdConfigure.Hide();
            pnlConfigure.Visible = false;

            persistForm();

            this.Block_BlockUpdated(sender, e);
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

        protected void rptDataFilters_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Filter filter = e.Item.DataItem as Filter;
            if (filter != null)
            {
                Literal lFilterName = e.Item.FindControl("lFilterName") as Literal;
                HiddenField FilterId = e.Item.FindControl("hfFilterId") as HiddenField;
                HiddenField FilterOrder = e.Item.FindControl("hfSortOrder") as HiddenField;
                RockTextBox tbDisplayName = e.Item.FindControl("tbDisplayName") as RockTextBox;
                RockTextBox tbCSS = e.Item.FindControl("tbCSS") as RockTextBox;

                RockDropDownList ddlDisplayStyle = e.Item.FindControl("ddlDisplayStyle") as RockDropDownList;
                RockCheckBox cbActiveByDefault = e.Item.FindControl("cbActiveByDefault") as RockCheckBox;

                FilterId.Value = filter.Id;
                FilterOrder.Value = filter.Order.ToString();
                lFilterName.Text = filter.DataViewName;
                tbDisplayName.Text = filter.DisplayAs;
                tbCSS.Text = filter.CSS;
                cbActiveByDefault.Checked = filter.ActiveByDefault;

                ScriptManager.RegisterStartupScript(this.Page, typeof(System.Web.UI.Page), "UpdateSelections",
               "$(document).ready(function(){initializeOptions(" + ddlDisplayStyle.ClientID+")});", true);
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
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            // reload the full page since controls are dynamically created based on block settings
            NavigateToPage(this.CurrentPageReference);
        }

        protected void etpEntityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvDataViewBucketPicker.ClearSelection();
            dvDataViewBucketPicker.EntityTypeId = etpEntityType.SelectedEntityTypeId;

            buckets = new List<Bucket>();
            filters = new Dictionary<string, Filter>();

            BucketDetailsControl.buckets = buckets;
            BucketDetailsControl.Refresh();
            upnlBuckets.Update();

            FilterControl.EntityTypeId = etpEntityType.SelectedEntityTypeId ?? 0;
            FilterControl.filters = filters;
            FilterControl.Refresh();
        }

        protected void dvDataViewBucketPicker_SelectedItem(object sender, EventArgs e)
        {
            this.buckets = BucketDetailsControl.buckets;

            // Check selections against current selections to determine if a change has been made
            var changeMade = false;

            // Setup a list of Filters based on dataviews that are selected
            var newBuckets = new List<Bucket>();
            var query = from ListItem item in dvDataViewBucketPicker.Items where item.Selected select item;
            foreach (ListItem item in query)
            {
                Bucket newBucket = new Bucket();
                newBucket.Id = item.Value.AsInteger();
                newBucket.Name = item.Text;
                newBucket.DisplayAs = item.Text;
                newBucket.Order = buckets.Count() + 1;

                newBuckets.Add(newBucket);
            }

            // If newFilters is longer than our current filters, or vice versa, change has been made
            if (newBuckets.Count != buckets.Count || buckets.Except(newBuckets).Any())
            {
                changeMade = true;
                var tempBuckets = new List<Bucket>();
                // Keep the data from our existing filters when updating our filter list
                foreach (Bucket bucket in newBuckets)
                {
                    Bucket existingBucket = buckets.Find(b => b.Id == bucket.Id);

                    if (existingBucket.IsNotNull())
                    {
                        tempBuckets.Add(existingBucket);
                    }
                    else
                    {
                        tempBuckets.Add(bucket);
                    }
                }

                buckets = tempBuckets;
            }

            if (changeMade == true)
            {
                BucketDetailsControl.buckets = buckets;
                BucketDetailsControl.Refresh();
            }
        }

        #endregion

        #region Methods

        // helper functional methods (like BindGrid(), etc.)

        #endregion
    }
}