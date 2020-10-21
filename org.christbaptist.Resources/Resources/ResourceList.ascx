<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ResourceList.ascx.cs" Inherits="RockWeb.Plugins.org_christbaptist.Resources.ResourceList" %>

<div style="display: none;" id="resource-config-json">
    <%= filtersAsJson %>
</div>

<!-- Add angular app markup -->
<!-- Add configurationId="xxx" to angular markup -->

<link rel="stylesheet" href="<%=ResolveUrl("cbc-resources/styles.76de66db82ae036859e3.css") %>">


<%--<script src="https://kit.fontawesome.com/6d42517843.js" crossorigin="anonymous"></script>--%>
<div style="width: 100%; height: 35px;"></div>
<script type="text/javascript">window["Zone"] = undefined;</script>
<app-root configurationId="test1259"></app-root>
<script src="<%=ResolveUrl("cbc-resources/runtime-es2015.1eba213af0b233498d9d.js") %>" type="module"></script>
<script src="<%=ResolveUrl("cbc-resources/runtime-es5.1eba213af0b233498d9d.js") %>" nomodule defer></script>
<script src="<%=ResolveUrl("cbc-resources/polyfills-es5.8762124472dbe27575bb.js") %>" nomodule defer></script>
<script src="<%=ResolveUrl("cbc-resources/polyfills-es2015.f2c5ab749249a66bdf26.js") %>" type="module"></script>
<script src="<%=ResolveUrl("cbc-resources/main-es2015.158a3cd5bbc0dae3e442.js") %>" type="module"></script>
<script src="<%=ResolveUrl("cbc-resources/main-es5.158a3cd5bbc0dae3e442.js") %>" nomodule defer></script>


<asp:UpdatePanel ID="upnlContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <%-- Configuration Panel --%>
        <asp:Panel ID="pnlConfigure" runat="server" Visible="false">
            <Rock:ModalDialog ID="mdConfigure" runat="server" ValidationGroup="vgConfigure" OnSaveClick="mdConfigure_SaveClick">
                <Content>
                    <Rock:DataDropDownList ID="ddlContentChannelType" Label="Content Channel Type" runat="server" SourceTypeName="Rock.Model.ContentChannelType, Rock"
                        PropertyName="Name" AutoPostBack="true" OnSelectedIndexChanged="ddlContentChannelType_SelectedIndexChanged" DataValueField="Id" DataTextField="Name"></Rock:DataDropDownList>

                    <Rock:RockCheckBoxList ID="cblContentChannels" runat="server" Label="Content Channels to pull resources from" DataTextField="Name" DataValueField="Id" Visible="false"></Rock:RockCheckBoxList>
                    <Rock:RockCheckBoxList ID="cblContentChannelAttributes" runat="server" Label="Content Channel attributes to use as filters" DataTextField="Name" DataValueField="Id" Visible="false"></Rock:RockCheckBoxList>

                </Content>
            </Rock:ModalDialog>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>