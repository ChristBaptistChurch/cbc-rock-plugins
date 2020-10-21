using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Attribute = Rock.Model.Attribute;

namespace RockWeb.Plugins.org_christbaptist.Resources
{
    [DisplayName("Resource List")]
    [Category("Christ Baptist > Resources")]
    [Description("View resources in an interactive filter block")]

    [LinkedPage("Detail Page")]
    [TextField("ContentChannelType", "The content channel type to use for this resource block", true)]
    [TextField("ContentChannels", "Comma separated list of Content Channel Ids to show in this block", true)]
    [TextField("ContentChannelAttributes", "Comma separated list of ContentChannelType Attribute Ids to show as filters in this block", true)]
    public partial class ResourceList : Rock.Web.UI.RockBlockCustomSettings
    {
        private class MinimalAttributeValue
        {
            public string Value;
            public string Display;
        }

        private class AttributeWithPossibleValues
        {
            public int Id;
            public string Name;
            public List<MinimalAttributeValue> possibleAttributeValues;
        }

        public string filtersAsJson
        {
            get
            {
                List <AttributeWithPossibleValues> attributesWithValues = new List<AttributeWithPossibleValues>();

                List<int> filters = GetAttributeValue("ContentChannelAttributes").Split(',').AsIntegerList();
                List<int> contentChannelIds = GetAttributeValue("ContentChannels").Split(',').AsIntegerList();

                var rockContext = new RockContext();
                var attributeService = new AttributeService(rockContext);
                var attributeValueService = new AttributeValueService(rockContext);

                // Only create pool from content channel items from the selected content channels
                var populatedContentChannelItems = new ContentChannelItemService(rockContext).Queryable()
                    .Where(cci => contentChannelIds.Any((id) => id == cci.ContentChannelId));

                var contentChannels = new ContentChannelService(rockContext).Queryable();

                // Add type filter
                var typeValues = populatedContentChannelItems
                    .Select((cci) => cci.ContentChannelId)
                    .GroupBy((id) => id)
                    .Select(groupedByIds => groupedByIds.Key)
                    .Join(contentChannels, ccid => ccid, contentChannel => contentChannel.Id, (ccid, contentChannel) => new MinimalAttributeValue
                    {
                        Value = ccid.ToString(),
                        Display = contentChannel.Name
                    })
                    .OrderBy(mav => mav.Display)
                    .ToList();

                typeValues.Insert(0, new MinimalAttributeValue { Display = "All Types", Value = null });
                attributesWithValues.Add(new AttributeWithPossibleValues
                {
                    Id = -1,
                    Name = "Type",
                    possibleAttributeValues = typeValues
                });

                // Add date filter
                var dateValues = populatedContentChannelItems
                    .Select((cci) => cci.StartDateTime.Year)
                    .GroupBy((Year) => Year)
                    .Select(groupedByYears => new MinimalAttributeValue
                    {
                        Value = groupedByYears.Key.ToString(),
                        Display = groupedByYears.Key.ToString()
                    })
                    .OrderByDescending(mav => mav.Display)
                    .ToList();

                dateValues.Insert(0, new MinimalAttributeValue { Display = "All Dates", Value = null });
                attributesWithValues.Add(new AttributeWithPossibleValues
                {
                    Id = -2,
                    Name = "Date",
                    possibleAttributeValues = dateValues
                });

                foreach (int filterAttributeId in filters)
                {
                    Attribute attribute = attributeService.Get(filterAttributeId);
                    List<MinimalAttributeValue> attributeValues;

                    

                    // If this attribute represents a Defined Value
                    if (attribute.FieldTypeId == 16)
                    {
                        int definedTypeId = attribute.AttributeQualifiers.Where(aq => aq.Key == "definedtype").Single().Value.AsInteger();
                        var definedValues = new DefinedValueService(rockContext).Queryable().Where(dv => dv.DefinedTypeId == definedTypeId);
                        
                        attributeValues = attributeValueService.Queryable()
                            // Join with populatedContentChannelItems to limit our possible values to content channel items in scope
                            .Join(populatedContentChannelItems, av => av.EntityId, pcci => pcci.Id, (av, pcci) => av)
                            // Only valid values for our present attribute
                            .Where(av => av.AttributeId == filterAttributeId && av.Value != null && av.Value != "")
                            // Only interested in unique values
                            .GroupBy(av => av.Value)
                            .Select(groupedByValue => groupedByValue.Key)
                            // Because one DV Attribute can be a multi-select, we have to account for comma separated values in attribute values
                            .AsEnumerable().SelectMany(av => av.Split(',')).Select((dvGuid) => dvGuid.Trim(new char[] { '{', '}'}).ToLower())
                            // ...we might have picked up some duplicates with that last split because co-authors would have become their own authors
                            .Distinct()
                            // Lookup the actual Value for the DefinedValue Guid that the attribute stores
                            .Join(definedValues, dvGuid => Guid.Parse(dvGuid), definedValue => definedValue.Guid, (dvGuid, dv) => new
                                {
                                    Value = dvGuid,
                                    Display = dv.Value,
                                    Order = dv.Order
                                })
                            // Sort based on DefinedValue order...allows for custom sorting
                            .OrderBy(dv => dv.Order)
                            // Convert it to the format we need to consume
                            .Select(dv => new MinimalAttributeValue { Value = dv.Value, Display = dv.Display })
                            .ToList();
                    } else if (attribute.FieldTypeId == 2)
                    {
                        // Multi-select should sort attribute values by the values set in the attribute
                        List<string> possibleValues = attribute.AttributeQualifiers.Where(aq => aq.Key == "values").Single().Value.Split(',').ToList();

                        attributeValues = attributeValueService.Queryable()
                            // Join with populatedContentChannelItems to limit our possible values to content channel items in scope
                            .Join(populatedContentChannelItems, av => av.EntityId, pcci => pcci.Id, (av, pcci) => av)
                            .Where(av => av.AttributeId == filterAttributeId && av.Value != null)
                            .GroupBy(av => av.Value)
                            .AsEnumerable()
                            .SelectMany(avg => avg.Key.Split(','))
                            .Distinct()
                            .Select(g => new { Display = g, Value = g, Order = possibleValues.IndexOf(g) })
                            .OrderBy(g => g.Order)
                            .Select(g => new MinimalAttributeValue { Display = g.Value, Value = g.Value })
                            .ToList();
                    }
                    else // Text field
                    {
                        attributeValues = attributeValueService.Queryable()
                            // Join with populatedContentChannelItems to limit our possible values to content channel items in scope
                            .Where(av => av.AttributeId == filterAttributeId && av.Value != null)
                            .Join(populatedContentChannelItems, av => av.EntityId, pcci => pcci.Id, (av, pcci) => av)
                            .GroupBy(av => av.Value)
                            .Select(g => new MinimalAttributeValue { Display = g.Key, Value = g.Key })
                            .OrderBy(d => d.Display)
                            .ToList();
                    }

                    //System.Diagnostics.Debug.WriteLine(attributeValues.ToJson());

                    attributeValues.Insert(0, new MinimalAttributeValue { Display = "All " + attribute.Name.Pluralize(), Value = null });
                    attributesWithValues.Add(new AttributeWithPossibleValues
                    {
                        Id = attribute.Id,
                        Name = attribute.Name,
                        possibleAttributeValues = attributeValues
                    });
                }

                return attributesWithValues.ToJson();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            bool canEdit = IsUserAuthorized(Rock.Security.Authorization.EDIT);
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

            }
        }

        protected override void ShowSettings()
        {
            pnlConfigure.Visible = true;

            var rockContext = new RockContext();
            var contentChannelTypeService = new ContentChannelTypeService(rockContext);

            ddlContentChannelType.DataSource = contentChannelTypeService.Queryable().ToList();
            ddlContentChannelType.DataBind();

            int contentChannelType = GetAttributeValue("ContentChannelType").AsInteger();
            List<int> contentChannels = GetAttributeValue("ContentChannels").Split(',').AsIntegerList();
            List<int> attributes = GetAttributeValue("ContentChannelAttributes").Split(',').AsIntegerList();

            // If no saved selection
            ddlContentChannelType.Items.Insert(0, new ListItem(string.Empty, string.Empty));

            if (contentChannelType != 0)
            {
                ddlContentChannelType.SelectedValue = contentChannelType.ToString();
                updateChannelsAndAttributes();

                var contentChannelOptions = from ListItem li in cblContentChannels.Items
                                       where contentChannels.Any((channelId) => { return channelId == li.Value.AsInteger(); })
                                       select li;
                foreach (ListItem li in contentChannelOptions)
                {
                    li.Selected = true;
                }

                var contentChannelAttributeOptions = from ListItem li in cblContentChannelAttributes.Items
                                            where attributes.Any((attributeId) => { return attributeId == li.Value.AsInteger(); })
                                            select li;
                foreach (ListItem li in contentChannelAttributeOptions)
                {
                    li.Selected = true;
                }
            }
            else
            {
                ddlContentChannelType.SelectedIndex = 0;
            }

            mdConfigure.Show();
            upnlContent.Update();
        }

        protected void persistForm()
        {
            SetAttributeValue("ContentChannels", cblContentChannels.SelectedValues.AsDelimited(","));
            SetAttributeValue("ContentChannelAttributes", cblContentChannelAttributes.SelectedValues.AsDelimited(","));
            SetAttributeValue("ContentChannelType", ddlContentChannelType.SelectedValue);

            SaveAttributeValues();
        }

        protected void Block_BlockUpdated(object sender, EventArgs e)
        {
            // reload the full page since controls are dynamically created based on block settings
            NavigateToPage(this.CurrentPageReference);
        }

        protected void mdConfigure_SaveClick(object sender, EventArgs e)
        {

            mdConfigure.Hide();
            pnlConfigure.Visible = false;

            persistForm();

            this.Block_BlockUpdated(sender, e);
        }

        protected void updateChannelsAndAttributes()
        {
            var rockContext = new RockContext();
            var contentChannels = new ContentChannelService(rockContext).Queryable().Where(q => q.ContentChannelTypeId.ToString() == ddlContentChannelType.SelectedValue);
            var contentChannelAttributes = new AttributeService(rockContext)
                   .Queryable()
                   .Where(q => q.EntityTypeQualifierColumn == "ContentChannelTypeId" && q.EntityTypeQualifierValue == ddlContentChannelType.SelectedValue);

            cblContentChannels.DataSource = contentChannels.ToList();
            cblContentChannels.DataBind();
            cblContentChannels.Visible = true;

            cblContentChannelAttributes.DataSource = contentChannelAttributes.ToList();
            cblContentChannelAttributes.DataBind();
            cblContentChannelAttributes.Visible = true;
        }

        protected void ddlContentChannelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(ddlContentChannelType.SelectedValue);

            updateChannelsAndAttributes();

            upnlContent.Update();
        }
    }
}

