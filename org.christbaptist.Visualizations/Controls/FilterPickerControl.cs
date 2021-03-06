﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Rock;
using Rock.Web.UI.Controls;
using org.christbaptist.Visualizations.Model;
using System.Web.UI.WebControls;

namespace org.christbaptist.Visualizations.Controls
{
    class FilterItemTemplate : ITemplate
    {
        ListItemType _type;

        public FilterItemTemplate(ListItemType type)
        {
            _type = type;
        }

        public void InstantiateIn(Control container)
        {
            switch (_type)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    HiddenField hfFilterId = new HiddenField();
                    hfFilterId.ID = "hfFilterId";

                    HiddenField hfSortOrder = new HiddenField();
                    hfSortOrder.ID = "hfSortOrder";

                    System.Web.UI.WebControls.Panel filterPanel = new Panel();
                    filterPanel.ID = "pnlFilter";
                    filterPanel.AddCssClass("sccDvvFilterSection");
                    filterPanel.Attributes.Add("ondragover", "dragover_handler(event)");
                    filterPanel.Attributes.Add("ondrop", "drop_handler(event)");

                    filterPanel.Controls.Add(hfFilterId);
                    filterPanel.Controls.Add(hfSortOrder);

                    // Title Bar
                    filterPanel.Controls.Add(new LiteralControl(@"<div class='sccDvvFilterSection_title'>
                        <div class='reorder' draggable='true' ondragstart='dragstart_handler(event)' style='padding-right: 15px;'>
                            <i class='fa fa-bars'></i>
                        </div>
                        <svg style='width: 30px; height: 30px; margin-right: 5px; '>
                                       <circle r='10' cy='15' cx='15' class='dot red'></circle>
                        </svg>"));
                    Literal lFilterName = new Literal();
                    lFilterName.ID = "lFilterName";
                    filterPanel.Controls.Add(lFilterName);

                    // Enabled by Default Checkbox
                    filterPanel.Controls.Add(new LiteralControl(@"</div>
                            <div class='row enabledByDefault'>
                                <div class='col-xs-12 col-sm-4 col-md-4'>"));
                    RockCheckBox cbActiveByDefault = new RockCheckBox();
                    cbActiveByDefault.ID = "cbActiveByDefault";
                    cbActiveByDefault.Label = "Visible by Default";
                    cbActiveByDefault.Checked = true;
                    filterPanel.Controls.Add(cbActiveByDefault);
                    filterPanel.Controls.Add(new LiteralControl("</div></div>"));

                    // START Filter options
                    filterPanel.Controls.Add(new LiteralControl(@"<div class='row js-filterconfig-row'>
                        <div class='col-md-4'>"));

                    // Display As text field
                    RockTextBox tbDisplayName = new RockTextBox();
                    tbDisplayName.ID = "tbDisplayName";
                    tbDisplayName.Label = "DisplayAs";
                    tbDisplayName.CssClass = "js-settings-pre-html";
                    tbDisplayName.Help = "The name to show for the filter";
                    tbDisplayName.ValidateRequestMode = ValidateRequestMode.Disabled;
                    filterPanel.Controls.Add(tbDisplayName);
                    filterPanel.Controls.Add(new LiteralControl("</div>"));

                    // Color Picker
                    filterPanel.Controls.Add(new LiteralControl("<div class='col-md-3 sccDvvFilterColorPicker'>"));
                    RockDropDownList ddlDisplayStyle = new RockDropDownList();
                    ddlDisplayStyle.Label = "Style";
                    ddlDisplayStyle.ID = "ddlDisplayStyle";
                    ddlDisplayStyle.Help = "The style to use when displaying people who match this filter";
                    ddlDisplayStyle.Attributes.Add("onchange", "displayOptionChosen(this)");
                    ddlDisplayStyle.Items.Add(new ListItem("Red", "red"));
                    ddlDisplayStyle.Items.Add(new ListItem("Orange", "orange"));
                    ddlDisplayStyle.Items.Add(new ListItem("Yellow", "yellow"));
                    ddlDisplayStyle.Items.Add(new ListItem("Green", "green"));
                    ddlDisplayStyle.Items.Add(new ListItem("Blue", "blue"));
                    ddlDisplayStyle.Items.Add(new ListItem("Purple", "purple"));
                    ddlDisplayStyle.Items.Add(new ListItem("Outline", "outline"));
                    ddlDisplayStyle.Items.Add(new ListItem("Custom", "custom"));
                    filterPanel.Controls.Add(ddlDisplayStyle);

                    // Custom CSS
                    filterPanel.Controls.Add(new LiteralControl("</div><div class='col-md-4 custom-css'>"));
                    RockTextBox tbCSS = new RockTextBox();
                    tbCSS.ID = "tbCSS";
                    tbCSS.Label = "CSS";
                    tbCSS.Help = "Custom CSS properties to apply. NOTE: These must be for SVG elements, not HTML elements.";
                    tbCSS.CssClass = "js-settings-pre-html";
                    tbCSS.ValidateRequestMode = ValidateRequestMode.Disabled;
                    tbCSS.TextMode = TextBoxMode.MultiLine;
                    tbCSS.Rows = 5;
                    tbCSS.Attributes.Add("onchange", "updateCustomStyles(this)");
                    filterPanel.Controls.Add(tbCSS);
                    filterPanel.Controls.Add(new LiteralControl("</div></div>"));

                    // END Filter options


                    container.Controls.Add(filterPanel);
                    break;
            }
        }
    }

    public partial class FilterPickerControl : System.Web.UI.UserControl
    {
        DataViewsPicker dvDataViewPicker;
        UpdatePanel upnlFilters;
        RockCheckBox cbShowFilterKey;
        Repeater rptDataFilters;

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

        public FilterPickerControl()
        {
            upnlFilters = new UpdatePanel();
            upnlFilters.UpdateMode = UpdatePanelUpdateMode.Conditional;

            PanelWidget pwFilters = new PanelWidget();
            pwFilters.Title = "Filters";
            pwFilters.ID = "pwFilters";
            pwFilters.Expanded = true;

            // Data View picker
            dvDataViewPicker = new DataViewsPicker();
            dvDataViewPicker.ID = "dvDataViewPicker";
            dvDataViewPicker.Label = "Filters";
            dvDataViewPicker.Help = "Select data views you want to represent in this visulization";
            dvDataViewPicker.AutoPostBack = true;
            dvDataViewPicker.TextChanged += dvDataView_SelectedItem;
            pwFilters.Controls.Add(dvDataViewPicker);

            // Show filter key checkbox
            pwFilters.Controls.Add(new LiteralControl("<div class='row showFilterKey'><div class='col-xs-12'>"));
            cbShowFilterKey = new RockCheckBox();
            cbShowFilterKey.ID = "cbShowFilterKey";
            cbShowFilterKey.Label = "Show Filter Key";
            cbShowFilterKey.Checked = true;
            cbShowFilterKey.Help = "Show the filter key in the top left hand corner of the chart";
            pwFilters.Controls.Add(cbShowFilterKey);
            pwFilters.Controls.Add(new LiteralControl("</div></div>"));


            RockControlWrapper rcwDatafilters = new RockControlWrapper();
            rptDataFilters = new Repeater();
            rptDataFilters.ID = "rptDataFilters";
            rptDataFilters.ItemDataBound += rptDataFilters_ItemDataBound;
            rptDataFilters.ItemTemplate = new FilterItemTemplate(ListItemType.Item);
            rcwDatafilters.Controls.Add(rptDataFilters);
            pwFilters.Controls.Add(rcwDatafilters);

            upnlFilters.ContentTemplateContainer.Controls.Add(pwFilters);
            Controls.Add(upnlFilters);
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
                newFilter.CSS = "fill: #FF595E;";
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