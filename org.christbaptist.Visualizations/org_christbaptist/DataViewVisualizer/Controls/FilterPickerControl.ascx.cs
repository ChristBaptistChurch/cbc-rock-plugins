using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using org.christbaptist.Visualizations.Model;
using Rock.Web.UI.Controls;

namespace org.christbaptist.Visualizations
{
    public partial class FilterPickerControl : System.Web.UI.UserControl
    {
        private Dictionary<string, Filter> _filters;

        public Dictionary<string, Filter> filters
        {
            get
            {
                LoadValuesFromPage();
                return _filters;
            }
            set { _filters = value; }
        }

        private int _entityTypeId = 15;

        public int EntityTypeId
        {
            get
            {
                return _entityTypeId;
            }

            set
            {
                _entityTypeId = value;
                dvDataViewPicker.EntityTypeId = _entityTypeId;
                dvDataViewPicker.ClearSelection();
            }
        }
        
        public bool ShowFilterKey
        {
            get
            {
                return cbShowFilterKey.Checked;
            }

            set
            {
                cbShowFilterKey.Checked = value;
            }
        }

        private void LoadValuesFromPage()
        {
            foreach (var item in rptDataFilters.Items.OfType<RepeaterItem>())
            {
                HiddenField FilterId = item.FindControl("hfFilterId") as HiddenField;
                HiddenField FilterOrder = item.FindControl("hfSortOrder") as HiddenField;
                RockTextBox tbDisplayName = item.FindControl("tbDisplayName") as RockTextBox;
                RockTextBox tbCSS = item.FindControl("tbCSS") as RockTextBox;
                RockCheckBox cbActiveByDefault = item.FindControl("cbActiveByDefault") as RockCheckBox;

                _filters[FilterId.Value].DisplayAs = tbDisplayName.Text;
                _filters[FilterId.Value].CSS = tbCSS.Text;
                _filters[FilterId.Value].Order = Int32.Parse(FilterOrder.Value);
                _filters[FilterId.Value].ActiveByDefault = cbActiveByDefault.Checked;
            }
        }

        protected override object SaveControlState()
        {
            object[] controlState = new object[2];
            controlState[0] = base.SaveControlState();
            controlState[1] = _filters;
            return controlState;
        }

        protected override void LoadControlState(object savedState)
        {
            object[] controlState = (object[])savedState;
            base.LoadControlState(controlState[0]);
            _filters = (Dictionary<string, Filter>)controlState[1];
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                dvDataViewPicker.EntityTypeId = _entityTypeId;
                dvDataViewPicker.SelectedIndex = 0;
            }

        }

        public void Refresh()
        {
            // Load Filters
            //find specific items from listbox
            var items = from ListItem li in dvDataViewPicker.Items
                        where _filters.Keys.Any((filter) => { return filter == li.Value; })
                        select li;

            //programmatically select specific items 
            //by default first time those items are selected.
            foreach (ListItem li in items)
            {
                li.Selected = true;
            }

            rptDataFilters.DataSource = _filters.Values.OrderBy(filter => filter.Order);
            rptDataFilters.DataBind();
            upnlFilters.Update();
        }

        protected void dvDataView_SelectedItem(object sender, EventArgs e)
        {
            // Save any changes to existing fields before we update and rebind
            LoadValuesFromPage();

            // Check selections against current selections to determine if a change has been made
            var changeMade = false;

            // Setup a list of Filters based on dataviews that are selected
            var newFilters = new Dictionary<string, Filter>();
            var query = from ListItem item in dvDataViewPicker.Items where item.Selected select item;
            foreach (ListItem item in query)
            {
                Filter newFilter = new Filter();
                newFilter.Id = item.Value;
                newFilter.DataViewName = item.Text;
                newFilter.DisplayAs = item.Text;
                newFilter.CSS = "fill: rgba(255, 0, 0, 0.60);";
                newFilter.Order = _filters.Count() + 1;

                newFilters[newFilter.Id] = newFilter;
            }

            // If newFilters is longer than our current filters, or vice versa, change has been made
            if (newFilters.Count != _filters.Count || _filters.Except(newFilters).Any())
            {
                changeMade = true;
                var tempFilters = new Dictionary<string, Filter>();
                // Keep the data from our existing filters when updating our filter list
                foreach (Filter filter in newFilters.Values)
                {
                    if (_filters.ContainsKey(filter.Id))
                    {
                        tempFilters[filter.Id] = _filters[filter.Id];
                    }
                    else
                    {
                        tempFilters[filter.Id] = newFilters[filter.Id];
                    }
                }

                _filters = tempFilters;
            }

            if (changeMade == true)
            {
                Refresh();
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

                ScriptManager.RegisterStartupScript(Page, typeof(Page), "UpdateSelections",
               "$(document).ready(function(){initializeOptions(" + ddlDisplayStyle.ClientID + ")});", true);
            }
        }
    }
}