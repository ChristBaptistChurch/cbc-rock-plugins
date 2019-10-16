<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataViewVisualizer.ascx.cs" Inherits="RockWeb.Plugins.org_christbaptist.Visualizers.DataViewVisualizer" %>

<%@ Register TagName="FilterPickerControl" TagPrefix="cbc" Src="Controls/FilterPickerControl.ascx" %>
<%@ Register TagName="BucketDetails" TagPrefix="cbc" Src="Controls/BucketDetailsControl.ascx" %>

<script src="https://d3js.org/d3.v5.min.js"></script>
<script src="/Plugins/org_christbaptist/DataViewVisualizer/Scripts/admin.js"></script>
<script src="/Plugins/org_christbaptist/DataViewVisualizer/Scripts/main.js"></script>
<link rel="stylesheet" href="/Plugins/org_christbaptist/DataViewVisualizer/Scripts/visualizations.css" />

<script>
    (function () {
        var summaryLava = `<%= GetAttributeValue("SummaryLava") %>`;
        var filters = <%= filtersAsJson %>;
        var buckets = <%= bucketsAsJson %>;

        var chart = new window.org_christbaptist.ColumnCircleChart("", "<%= GetAttributeValue("Title") %>");

        chart.setLavaSummary(`<%= GetAttributeValue("SummaryLava") %>`)
            .setStyle(`<%= GetAttributeValue("Style") %>`)
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

                    <Rock:RockDropDownList ID="ddlDefaultStyle" runat="server" Label="Default Style" Help="The default chart style" onchange="displayOptionChosen(this)">
                        <asp:ListItem Value="bucket">Bucket</asp:ListItem>
                        <asp:ListItem Value="circle">Circle</asp:ListItem>
                    </Rock:RockDropDownList>

                    <Rock:EntityTypePicker ID="etpEntityType" runat="server" Label="Applies To" OnSelectedIndexChanged="etpEntityType_SelectedIndexChanged" AutoPostBack="true" EnhanceForLongLists="true" Help="The entity type that will be represented by this visualization" />

                    <Rock:RockTextBox Text="/person/{{Id}}" ID="tbEntityUrl" runat="server" Label="Entity URL" Help="The url to open when an Entity is clicked. Use the {{Id}} merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />
                    <Rock:RockTextBox Text="/page/145?DataViewId={{Id}}" ID="tbDataViewUrl" runat="server" Label="Bucket URL" Help="The url to open when a bucket is clicked. Use the {{Id}} merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />

                    <Rock:CodeEditor ID="tbSummaryLava" Visible="True" runat="server" Label="Summary Lava Template" EditorMode="Lava" EditorHeight="200"
                        Help="This will display in a floating box when you hover over a group member. Use the Row merge field.  <span class='tip tip-lava'></span>" />

                    <asp:UpdatePanel ID="upnlBuckets" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <Rock:PanelWidget ID="pwBuckets" runat="server" Title="Buckets" Expanded="true">
                                <Rock:DataViewsPicker ID="dvDataViewBucketPicker" runat="server" Label="Buckets" Help="Select data views to form the base of this visualization" Required="true" AutoPostBack="True" OnTextChanged="dvDataViewBucketPicker_SelectedItem" />

                                <cbc:BucketDetails ID="BucketDetailsControl" runat="server" />
                            </Rock:PanelWidget>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <cbc:FilterPickerControl ID="FilterControl" runat="server" />
                </Content>
            </Rock:ModalDialog>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
