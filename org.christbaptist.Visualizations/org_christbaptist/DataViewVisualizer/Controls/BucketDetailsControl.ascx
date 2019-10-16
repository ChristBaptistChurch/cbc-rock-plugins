﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BucketDetailsControl.ascx.cs" Inherits="org.christbaptist.Visualizations.BucketDetailsControl" %>



        <asp:Repeater ID="rptBucketRepeater" runat="server" OnItemDataBound="rptBucketRepeater_ItemDataBound">
            <ItemTemplate>
                <asp:Panel ID="Panel1" runat="server" class="sccDvvFilterSection" ondragover="dragover_handler(event)" ondrop="drop_handler(event)">
                    <asp:HiddenField ID="hfBucketId" runat="server" />
                    <asp:HiddenField ID="hfSortOrder" runat="server" />
                    <div class="sccDvvFilterSection_title">
                        <div class="reorder" draggable="true" ondragstart="dragstart_handler(event)" style="padding-right: 15px;">
                            <i class="fa fa-bars"></i>
                        </div>
                        <asp:Literal ID="lBucketName" runat="server" />
                    </div>
                    <div class="row js-filterconfig-row">
                        <div class="col-md-4">
                            <Rock:RockTextBox ID="tbDisplayName" runat="server" Label="Display As" Help="The name to show for the filter" CssClass="js-settings-pre-html" ValidateRequestMode="Disabled" />
                        </div>
                    </div>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
 
