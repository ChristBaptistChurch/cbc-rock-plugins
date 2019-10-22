<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VennVisualizer.ascx.cs" Inherits="RockWeb.Plugins.org_christbaptist.Visualizers.VennVisualizer" %>

<%--<%@ Register TagName="FilterPickerControl" TagPrefix="cbc" Src="Controls/FilterPickerControl.ascx" %>--%>
<%@ Register TagPrefix="cbc" Namespace="org.christbaptist.Visualizations.Controls" Assembly="org.christbaptist.Visualizations" %>

<script src="https://d3js.org/d3.v5.min.js"></script>

<script src="/Plugins/org_christbaptist/DotChartVisualizations/Scripts/admin.js"></script>
<script src="/Plugins/org_christbaptist/DotChartVisualizations/Scripts/main.js"></script>
<link rel="stylesheet" href="/Plugins/org_christbaptist/DotChartVisualizations/Scripts/visualizations.css" />

<script>
    (function () {
        var summaryLava = `<%= GetAttributeValue("SummaryLava") %>`;
            var filters = <%= filtersAsJson %>;
            var buckets = <%= bucketsAsJson %>;

            var chart = new window.org_christbaptist.CircularVenn("", "<%= GetAttributeValue("Title") %>");

            chart.setLavaSummary(`<%= GetAttributeValue("SummaryLava") %>`)
                .showFilterKey(<%= GetAttributeValue("ShowFilterKey").ToLower() %>)
                .setEntityType(<%= GetAttributeValue("EntityTypeId") %>)
                .setEntityUrl(<%= entityUrl %>)
                .setBucketUrl(<%= bucketUrl %>);


        buckets.forEach((bucket) => chart.addBucket(bucket));
        filters.forEach((bucket) => chart.addFilter(bucket));

        chart.render();
    })();
</script>

<asp:UpdatePanel ID="upnlContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <%-- Configuration Panel --%>
        <asp:Panel ID="pnlConfigure" runat="server" Visible="false">
            <Rock:ModalDialog ID="mdConfigure" runat="server" ValidationGroup="vgConfigure" OnSaveClick="mdConfigure_SaveClick">
                <Content>
                    <Rock:RockTextBox ID="txtVisualizationTitle" runat="server" Label="Visualization Title" />

                    <Rock:EntityTypePicker ID="etpEntityType" runat="server" Label="Applies To" OnSelectedIndexChanged="etpEntityType_SelectedIndexChanged" AutoPostBack="true" EnhanceForLongLists="true" Help="The entity type that will be represented by this visualization" />

                    <Rock:RockTextBox Text="/person/{{Id}}" ID="tbEntityUrl" runat="server" Label="Entity URL" Help="The url to open when an Entity is clicked. Use the {{Id}} merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />
                    <Rock:RockTextBox Text="/page/145?DataViewId={{Id}}" ID="tbDataViewUrl" runat="server" Label="Bucket URL" Help="The url to open when a bucket is clicked. Use the {{Id}} merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />

                    <Rock:CodeEditor ID="tbSummaryLava" Visible="True" runat="server" Label="Summary Lava Template" EditorMode="Lava" EditorHeight="200"
                        Help="This will display in a floating box when you hover over a group member. Use the Row merge field.  <span class='tip tip-lava'></span>" />

                    <asp:UpdatePanel ID="upnlBuckets" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <Rock:PanelWidget ID="pwBuckets" runat="server" Title="Buckets" Expanded="true">
                                <Rock:DataViewItemPicker ID="dvBucket1" runat="server" Label="Bucket 1" Help="Select the base data view to display" Required="true" />

                                <Rock:RockTextBox Text="#97002e" ID="tbBucket1Color" runat="server" Label="Color" Help="The CSS color of this bucket (must be hex or RGB)" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />

                                <Rock:DataViewItemPicker ID="dvBucket2" runat="server" Label="Bucket 2" Help="Select the second data view to display (optional)" Required="false" />

                                <Rock:RockTextBox Text="#005a40" ID="tbBucket2Color" runat="server" Label="Color" Help="The CSS color of this bucket (must be hex or RGB)" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />

                                <Rock:DataViewItemPicker ID="dvBucket3" runat="server" Label="Bucket 3" Help="Select the third data view to display (optional)" Required="false" />

                                <Rock:RockTextBox Text="#003763" ID="tbBucket3Color" runat="server" Label="Color" Help="The CSS color of this bucket (must be hex or RGB)" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />
                            </Rock:PanelWidget>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <cbc:FilterPickerControl ID="FilterControl" runat="server" />

                </Content>
            </Rock:ModalDialog>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
