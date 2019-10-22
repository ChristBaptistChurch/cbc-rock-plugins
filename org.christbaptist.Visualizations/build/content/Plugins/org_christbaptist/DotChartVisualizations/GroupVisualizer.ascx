<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupVisualizer.ascx.cs" Inherits="RockWeb.Plugins.org_christbaptist.Visualizers.GroupVisualizer" %>

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

        var chart = new window.org_christbaptist.ColumnCircleChart("", "<%= GetAttributeValue("Title") %>");

        chart.setLavaSummary(`<%= GetAttributeValue("SummaryLava") %>`)
            .setStyle(`<%= GetAttributeValue("Style") %>`)
            .showFilterKey(<%= GetAttributeValue("ShowFilterKey").ToLower() %>)
            .setEntityType(15)
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
                        <asp:ListItem Value="bucket">Columns</asp:ListItem>
                        <asp:ListItem Value="circle">Circles</asp:ListItem>
                    </Rock:RockDropDownList>

                    <Rock:RockTextBox Text="/person/{{Id}}" ID="tbEntityUrl" runat="server" Label="Entity URL" Help="The url to open when an Entity is clicked. Use the <span class='tip'>Id</span> merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />
                    <Rock:RockTextBox Text="/page/113?GroupId={{Id}}" ID="tbGroupViewUrl" runat="server" Label="Group URL" Help="The url to open when a group is clicked. Use the <span class='tip'>Id</span> merge field" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />

                    <Rock:CodeEditor ID="tbSummaryLava" Visible="True" runat="server" Label="Summary Lava Template" EditorMode="Lava" EditorHeight="200"
                        Help="This will display in a floating box when you hover over a group member. Use the <span class='tip'>Row</span> merge field. <span class='tip tip-lava'></span>" />

                    <asp:UpdatePanel ID="upnlBuckets" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <Rock:PanelWidget ID="pwBuckets" runat="server" Title="Buckets" Expanded="true">
                                <Rock:GroupPicker ID="dvGroup" runat="server" Label="Groups" Help="Select the groups you want to appear in this visualization" Required="true" AllowMultiSelect="true" AutoPostBack="True" OnSelectItem="dvGroup_ValueChanged" />

                                <cbc:BucketDetailsControl ID="BucketDetailsControl" runat="server" />
                            </Rock:PanelWidget>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    
                    <cbc:FilterPickerControl ID="FilterControl" runat="server" />
                </Content>
            </Rock:ModalDialog>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
