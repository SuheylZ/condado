<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LeadsMarketing.ascx.cs" Inherits="Leads_UserControls_LeadsMarketing" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>

<div>
    <div id="mainDv">
        <uc2:StatusLabel ID="ctlStatus" runat="server" />
        <div runat="server" id="divGrid" class="Toolbar">
            <div id="fldSetGrid" class="condado">
                <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
                <br />
                <br />
                <asp:GridView ID="grdHome" runat="server" Width="100%" DataKeyNames="ID"
                    ShowHeaderWhenEmpty="true"
                    AutoGenerateColumns="false" AllowSorting="True" GridLines="None" PageSize="10"
                    AlternatingRowStyle-CssClass="alt">
                    <Columns>
                        <asp:TemplateField HeaderText="Campaign" SortExpression="Campaign">
                            <ItemTemplate>
                                <asp:Label ID="lblCampaign" runat="server" Text='<%# Bind("Campaign") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sub Status" SortExpression="SubStatus">
                            <ItemTemplate>
                                <asp:Label ID="lblSubStatus" runat="server" Text='<%# Bind("SubStatus") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Create Date" SortExpression="DateCreated">
                            <ItemTemplate>
                                <asp:Label ID="lblCreateDate" runat="server" Text='<%# Bind("DateCreated") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tracking Code" SortExpression="TrackingCode">
                            <ItemTemplate>
                                <asp:Label ID="lblTrackingCode" runat="server" Text='<%# Bind("TrackingCode") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Source Code" SortExpression="SourceCode">
                            <ItemTemplate>
                                <asp:Label ID="lblSourceCode" runat="server" Text='<%# Bind("SourceCode") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false"
                                    CommandName="EditX" Text="Edit"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        There are no Leads & Marketing Info to display.
                    </EmptyDataTemplate>
                    <HeaderStyle CssClass="gridHeader" />
                    <PagerSettings Position="Top" />
                    <PagerStyle VerticalAlign="Bottom" />
                </asp:GridView>
            </div>
        </div>
        <telerik:RadWindow ID="dlgLeadsMarketing" runat="server" Width="900" Height="630"
             Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
            IconUrl="../Images/alert1.ico" Title="Leads Marketing">
            <ContentTemplate>
                <asp:UpdatePanel ID="Updatepanel10" runat="server">
                    <ContentTemplate>
                        <fieldset style="margin: 10px">
                            <div runat="server" id="divForm" visible="true">
                                <div id="fldSetForm">
                                    <div class="buttons" style="text-align: right">
                                        <asp:Button runat="server" Text="Return to Leads & Marketing" ID="btnReturn" Visible="false"
                                            CausesValidation="False" class="returnButton"></asp:Button>
                                    </div>
                                    <table runat="server" id="tblControls">
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblStatus" runat="server" Text="Status:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbStatus" runat="server" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblSubStatus" runat="server" Text="Sub Status:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbSubStatus" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblLastAction" runat="server" Text="Last Action:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbLastAction" runat="server" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblLastActionDate" runat="server" Text="Last Action Date:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbLastActionDate" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCampaign" runat="server" Text="Campaign:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbCampaign" runat="server" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPublisherSubId" runat="server"
                                                    AssociatedControlID="txtPublisherSubId" Text="Publisher Sub ID:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtPublisherSubId" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblSourceCode" runat="server"
                                                    AssociatedControlID="txtSourceCode" Text="Source Code:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtSourceCode" runat="server" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                                <%--<asp:RequiredFieldValidator ID="vldSourceCode" runat="server" InitialValue=""
                                                        Display="None" ErrorMessage="Source Code is required" ControlToValidate="txtSourceCode"></asp:RequiredFieldValidator>
                            <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                          Enabled="True" TargetControlID="vldSourceCode" Width="250px"/>--%>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEmailTrackingCode" runat="server"
                                                    AssociatedControlID="txtEmailTrackingCode" Text="Email Tracking Code:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtEmailTrackingCode" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label1" runat="server"
                                                    AssociatedControlID="txtPublisherId" Text="Publisher ID:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtPublisherId" runat="server" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                            </td>

                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label2" runat="server"
                                                    AssociatedControlID="txtAdVariation" Text="Ad. Variation:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtAdVariation" runat="server" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label3" runat="server"
                                                    AssociatedControlID="txtIPAddress" Text="IP Address:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtIPAddress" runat="server" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                            </td>

                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label6" runat="server"
                                                    AssociatedControlID="radFirstContactDateTime" Text="First Contact Appointment" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">

                                                <telerik:RadDateTimePicker ID="radFirstContactDateTime" runat="server"
                                                    Width="200px">
                                                    <Calendar ID="tlCalendar" runat="server">
                                                    </Calendar>
                                                </telerik:RadDateTimePicker>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label4" runat="server"
                                                    AssociatedControlID="txtCompany" Text="Company:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap" colspan="3">
                                                <asp:TextBox ID="txtCompany" runat="server" Rows="4" TextMode="MultiLine" Width="99%" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label5" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtGroup" Text="Group:" />
                                            </td>
                                            <td nowrap="nowrap" colspan="3">
                                                <asp:TextBox ID="txtGroup" runat="server" Rows="4" TextMode="MultiLine" Width="99%" ValidationGroup="LeadsMarketing"></asp:TextBox>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblTrackingCode" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtTrackingCode" Text="Tracking Code:" />
                                            </td>
                                            <td nowrap="nowrap" colspan="3">
                                                <asp:TextBox ID="txtTrackingCode" runat="server" Rows="4" TextMode="MultiLine"
                                                    Width="99%"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblTrackingInfo" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtTrackingInfo" Text="Tracking Information:" />
                                            </td>
                                            <td nowrap="nowrap" colspan="3">
                                                <asp:TextBox ID="txtTrackingInfo" runat="server" Rows="4" TextMode="MultiLine"
                                                    Width="99%"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" nowrap="nowrap">
                                                <asp:HiddenField ID="hdnRecordId" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </fieldset>
                        <div class="buttons" style="text-align: right">
                            <asp:Button ID="btnSaveOnForm" runat="server"
                                OnClientClick="validateGroup('LeadsMarketing');" Text="Save" ValidationGroup="LeadsMarketing" />
                            <asp:Button ID="btnSaveAndCloseOnForm" runat="server"
                                OnClientClick="validateGroup('LeadsMarketing');" ValidationGroup="LeadsMarketing"
                                Text="Save and Close" />
                            <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False"
                                class="returnButton" Text="Close" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </div>
</div>

