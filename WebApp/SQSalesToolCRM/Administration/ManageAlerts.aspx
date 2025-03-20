<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageAlerts.aspx.cs" Inherits="Administration.ManageAlertsPage" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/RequiredFields.ascx" TagName="RequiredFields" TagPrefix="uc4" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc5" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset class="condado">
                <legend runat="server" id="FormTitle">Manage Alerts </legend>
                <asp:HiddenField ID="hdnPageState" runat="server" />
                <asp:HiddenField ID="hdnRecordId" runat="server" />                
                <asp:Panel ID="pnlList" runat="server">
                    <div style="float: left; width: 130px;">
                        <telerik:radmenu id="rmSideMenu" runat="server" cssclass="menu" skin="" style="z-index: 1"
                            flow="Vertical">
                            <Items>
                                <telerik:RadMenuItem runat="server" Text="Timer" Value="timer" />
                                <telerik:RadMenuItem runat="server" Text="Campaign" Value="campaign" />                                
                            </Items>
                        </telerik:radmenu>
                    </div>
                    <div style="float: right; width: 90%;">
                    <uc2:StatusLabel ID="lblMessage" runat="server" />
                        <div runat="server" id="divGridList">
                            <uc1:PagingBar ID="ctlPaging" runat="server" OnNewRecord="btnAddNew_Click" />
                            <br />
                            <telerik:radgrid id="grdAlerts" runat="server" width="100%" allowsorting="True" gridlines="None"
                                alternatingrowstyle-cssclass="alt" onfocus="this.blur();" cssclass="mGrid" skin=""
                                enabletheming="False" cellspacing="0" headerstyle-cssclass="gridHeader" alternatingitemstyle-cssclass="alt"
                                autogeneratecolumns="False" onitemcommand="Evt_Action_ItemCommand" OnSortCommand="grdAlerts_SortGrid" OnItemDataBound="grdAlerts_ItemDataBound">
                            <AlternatingItemStyle CssClass="alt" />
                            <MasterTableView>
                                <NoRecordsTemplate>
                                    There are no records to display at the moment.
                                </NoRecordsTemplate>
                     
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="colID" Visible="False"/>
                                    <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="colName" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="20%" />

                                    <telerik:GridBoundColumn HeaderText="Message" UniqueName="colMessage" DataField="Message" DataFormatString="{0}" ItemStyle-Width="20px" HeaderStyle-HorizontalAlign="Left" />
                                    <telerik:GridBoundColumn HeaderText="Detail Message" UniqueName="colDetailMessage" DataField="DetailMessage" ItemStyle-Width="40%" HeaderStyle-HorizontalAlign="Left" />
                                    <telerik:GridTemplateColumn FilterControlAltText="Record enabled" UniqueName="Enabled" ItemStyle-Width="40px"
                                    HeaderText="Enabled">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Enabled"))? "Yes": "No" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="colOptions" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>                                            
                                            <asp:LinkButton ID="lnkAcEdit" runat="server" Text="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="cmdEdit" /> 
                                            <asp:Label runat="server" id="lblSepDel" Text="&nbsp;|&nbsp;" />
                                            <asp:LinkButton ID="lnkAcDelete" runat="server" Text="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="cmdDelete" 
                                                            OnClientClick="if (confirm('Are you sure want to delete record?') == true)
                                                                true;
                                                            else
                                                                return false;"/>
                                        </ItemTemplate>
                                        <ItemStyle Width="40px" />
                                    </telerik:GridTemplateColumn>
                                </Columns>

                                <EditFormSettings>
                                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                </EditFormSettings>
                            </MasterTableView>

                            <HeaderStyle CssClass="gridHeader" />

                            <FilterMenu EnableImageSprites="False"></FilterMenu>
                        </telerik:radgrid>
                        </div>
                        <div runat="server" id="divForm">
                            <fieldset id="fldSetForm" class="condado">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button runat="server" Text="Return to List" ID="btnReturn" OnClick="btnCancelOnForm_Click"
                                        CausesValidation="False" class="returnButton"></asp:Button>
                                </div>
                                <ul>                                    
                                    <li>
                                        <asp:Label ID="lblAlertName" runat="server" AssociatedControlID="txtName" Text="Name"></asp:Label>
                                        <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox>
                                    </li>
                                    <li>
                                        <asp:Label ID="lblAlertMessage" runat="server" AssociatedControlID="txtMessage" Text="Message"></asp:Label>
                                        <asp:TextBox ID="txtMessage" runat="server" Width="200px"></asp:TextBox>
                                    </li>
                                    <li runat="server" id="liDetailMesssage">
                                        <asp:Label ID="lblDetailMessage" runat="server" 
                                            AssociatedControlID="txtDetailMessage" Text="Detail Message"></asp:Label>
                                        <asp:TextBox ID="txtDetailMessage" runat="server" Height="100px" 
                                            TextMode="MultiLine" Width="200px"></asp:TextBox>
                                    </li>
                                    <li>
                                        <asp:Label ID="lblDetailMessage0" runat="server" 
                                            AssociatedControlID="txtMessage" Text="Alert Type"></asp:Label>
                                        <asp:DropDownList ID="ddlAlertTypes" runat="server" Width="190px">
                                        </asp:DropDownList>
                                    </li>
                                    <li runat="server" id="liCampaign">
                                        <asp:Label ID="lblAttachedCampaign" runat="server" 
                                            AssociatedControlID="txtMessage" Text="Attached Campaign"></asp:Label>
                                        <asp:DropDownList ID="ddlAttachedCampaign" runat="server" Width="190px" ValidationGroup="campaignAlert">
                                        </asp:DropDownList>
                                    </li>
                                    <li runat="server" id="liStatuses">
                                        <asp:Label ID="lblStatuses" runat="server" 
                                            AssociatedControlID="ddlStatus" Text="Status"></asp:Label>
                                        <asp:DropDownList ID="ddlStatus" runat="server" Width="190px" ValidationGroup="campaignAlert">
                                        </asp:DropDownList>
                                    </li>
                                   
                                    <li runat="server" id="liTimeLapse">
                                        <asp:Label ID="lblTimeLapsed" runat="server" AssociatedControlID="txtTimeLapse" 
                                            Text="Time Lapse Period"></asp:Label>
                                        <telerik:RadNumericTextBox ID="txtTimeLapse" runat="server" MaxValue="999999999" ValidationGroup="timerAlert"
                                            MinValue="0" Width="50px">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                        <asp:DropDownList ID="ddlTimeLapseType" runat="server" Width="100px">
                                            <asp:ListItem Value="1">Seconds</asp:ListItem>
                                            <asp:ListItem Value="2">Minutes</asp:ListItem>
                                            <asp:ListItem Value="3">Hours</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="vldTimeLapse" runat="server"  Display="None" ValidationGroup="timerAlert"
                                            ControlToValidate="txtTimeLapse" ErrorMessage="Enter time lapse period."></asp:RequiredFieldValidator>
                                            <ajaxToolkit:ValidatorCalloutExtender ID="vldTimeLapse_Callout" runat="server"  Enabled="True" TargetControlID="vldTimeLapse" />
                                    </li>
                                    <li>
                                    <asp:Label ID="lblEnabled" runat="server" AssociatedControlID="chkEnabled" 
                                            Text="Enabled"></asp:Label>
                                            <asp:CheckBox runat="server" ID="chkEnabled" Checked="true" />
                                    </li>
                                   
                                </ul>
                                <div class="buttons">
                                    <table>
                                        <tr>                                           
                                            <td style="width: 60%;" class="tableTDCenterMiddle">
                                                
                                    <asp:Button ID="btnApply" runat="server" Text="Apply" OnClick="btnApply_Click" ValidationGroup="timerAlert" />
                                                &nbsp;
                                                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ValidationGroup="timerAlert" 
                                                    Text="Submit" />
                                                &nbsp;
                                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                                    Text="Cancel" class="returnButton" />            
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </asp:Panel>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
