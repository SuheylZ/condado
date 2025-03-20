<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManagePrioritization.aspx.cs" Inherits="Admin_ManagePrioritizartion" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/Schedule.ascx" TagName="Schedule" TagPrefix="uc4" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
            <div id="divGrid" runat="server">
                <fieldset class="condado">
                    <legend>Manage Prioritization Rules</legend>
                    <div id="divToolbar" class="Toolbar">
                        <table>
                            <tr>
                                <td style="width: 30%;">
                                    <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Queue" CausesValidation="False"
                                        OnClick="btnAddNewQueue_Click" />
                                     <asp:Button ID="btnPVRefresh" runat="server" Text="Refresh" CausesValidation="False"
                                        OnClick="btnPVRefresh_Click" />
                                </td>
                                <td style="width: 70%;">
                                    User Type&nbsp;
                                      <asp:DropDownList ID="ddlFilterByUserType" runat="server" DataValueField="id" DataTextField="name" Width="127px" AutoPostBack="true">
                                        <asp:ListItem Text="--All--" Value="-1" />
                                        <asp:ListItem Text="Assigned User" Value="1" />
                                        <asp:ListItem Text="CSR User" Value="2" />
                                        <asp:ListItem Text="Transfer Agent User" Value="3" />
                                        <asp:ListItem Text="Alternate Product User" Value="4" />
                                        <asp:ListItem Text="OnBoard User" Value="5" />
                                     </asp:DropDownList>
                                    <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <telerik:RadGrid ID="grdPrioritization" runat="server" Skin="" CssClass="mGrid" Width="100%"
                        CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                        AutoGenerateColumns="False" OnItemDataBound="grdPrioritization_ItemDataBound"
                        OnRowDrop="grdPrioritization_RowDrop" OnItemCommand="grdPrioritization_ItemCommand"
                         SelectedItemStyle-CssClass="gridHeader">
                         <AlternatingItemStyle CssClass="alt" />
                        <ClientSettings AllowRowsDragDrop="True">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                            
                        </ClientSettings>
                        <MasterTableView DataKeyNames="Id,Priority">
                            <NoRecordsTemplate>
                                There is no prioritization record.
                            </NoRecordsTemplate>
                            <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                                ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                                ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                                ExportToPdfText="Export to PDF" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                            <RowIndicatorColumn Visible="True" FilterControlAltText="Priority Rule RowIndicator column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>
                            <ExpandCollapseColumn Visible="True" FilterControlAltText="Priority Rule ExpandColumn column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                            <Columns>
                                <telerik:GridTemplateColumn FilterControlAltText="Priority record up/down" UniqueName=""
                                    HeaderText="Priority">
                                    <ItemTemplate>
                                     
                                        <asp:ImageButton ID="imgBtnUpOrder" runat="server" CommandName="UpOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                                        ImageUrl="~/Images/UpArrow.png" />
                                                        <asp:ImageButton ID="imgBtnDownOrder" runat="server" CommandName="DownOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                                        ImageUrl="~/Images/DownArrow.png" />
                                    </ItemTemplate>
                                    <ItemStyle Width="6%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>

                                <%--SZ: Apr 21, 2014: added after the client request on call the same day--%>
                                <telerik:GridTemplateColumn HeaderText="User Type" UniqueName="type" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                         <asp:Label ID="lblTmpType" runat="server" Text='<%# GetUserType(DataBinder.Eval(Container.DataItem, "UserType"))%>' />
                                    </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                <telerik:GridBoundColumn DataField="Title" FilterControlAltText="Priority Rule Title column"
                                    HeaderText="Title" UniqueName="Title">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="70%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Priority record enabled" UniqueName="Enabled"
                                    HeaderText="Enabled">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive"))? "Yes": "No" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Priority options column" UniqueName="options"
                                    HeaderText="Options">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Edit"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkCopy" runat="server" CausesValidation="False" Text="Copy"
                                            CommandName="CopyX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                                        <asp:Label runat="server" id="lblSepDel" Text="&nbsp;|&nbsp;" />
                                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                            Text="Delete" OnClientClick="if(confirm('Are you sure want to delete record?')== true) true; else return false;"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                                    </ItemTemplate>
                                     <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="15%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <EditFormSettings>
                                <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                </EditColumn>
                            </EditFormSettings>
                        </MasterTableView>
                        <HeaderStyle CssClass="gridHeader" />
                        <FilterMenu EnableImageSprites="False">
                        </FilterMenu>
                    </telerik:RadGrid>
                </fieldset>
            </div>
            <div id="divForm" runat="server">
             <div class="buttons" style="text-align: right" id = "returnButton">
                         <asp:Button runat="server" Text="Return to Queues" ID="btnReturn"
                                        OnClick="btnCancelOnForm_Click" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>
                <telerik:RadTabStrip ID="tlPrioritizationStrip" runat="server" SelectedIndex="1"
                    Skin="WebBlue" MultiPageID="tlMultiPage" 
                    ontabclick="tlPrioritizationStrip_TabClick">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Queue Details" PageViewID="tlPageQueueDetails" Owner="tlPrioritizationStrip">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Schedule" PageViewID="tlPageSchedule"
                            Owner="tlPrioritizationStrip" Selected="True">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Lead Filters" PageViewID="tlPageFilters" Owner="tlPrioritizationStrip">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tlMultiPage" runat="server" SelectedIndex="1">
                    <telerik:RadPageView ID="tlPageQueueDetails" runat="server">
                        <fieldset id="fldSetForm" class="condado">
                            <legend>Add/Edit Priority</legend>
                            <ul>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title:"></asp:Label><asp:TextBox
                                        ID="txtTitle" runat="server" Width="200px"></asp:TextBox><asp:RequiredFieldValidator
                                            ID="reqFldVldTitle" runat="server" ControlToValidate="txtTitle" Display="None"
                                            ErrorMessage="Enter title."></asp:RequiredFieldValidator></li>
                                  <li>
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlUserType" Text="User Type:"  />
                                    <asp:DropDownList ID="ddlUserType" runat="server" DataValueField="id" DataTextField="name" Width="127px">
                                        <asp:ListItem Text="--Unassigned--" Value="-1" />
                                        <asp:ListItem Text="Assigned User" Value="1" />
                                        <asp:ListItem Text="CSR User" Value="2" />
                                        <asp:ListItem Text="Transfer Agent User" Value="3" />
                                        <asp:ListItem Text="Alternate Product User" Value="4" />
                                        <asp:ListItem Text="OnBoard User" Value="5" />
                                     </asp:DropDownList>
                                </li>
                                
                                <li>
                                                <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription"
                                                    Text="Description:"></asp:Label><asp:TextBox ID="txtDescription" runat="server" Height="100px"
                                                        TextMode="MultiLine" Width="400px"></asp:TextBox></li>

                                
                                
                                <li>
                                                            <asp:ValidatorCalloutExtender ID="reqFldVldTitle_ValidatorCalloutExtender" runat="server"
                                                                Enabled="True" TargetControlID="reqFldVldTitle">
                                                            </asp:ValidatorCalloutExtender>
                                                            <asp:Label ID="lblEnabled" runat="server" AssociatedControlID="chkEnabled" Text="Enabled:"></asp:Label><asp:CheckBox
                                                                ID="chkEnabled" runat="server" /></li></ul>
                        </fieldset>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageSchedule" runat="server">
                        <uc4:Schedule ID="ctrlShiftSchedule" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageFilters" runat="server">
                        <fieldset class="condado">
                            <legend>Filter Settings</legend>
                            <table>
                                <tr>
                                    <td style="width: 20%">
                                        <asp:RadioButtonList ID="rdBtnlstFilterSelection" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="rdBtnlstFilterSelection_SelectedIndexChanged" RepeatDirection="Horizontal"
                                            Width="200px">
                                            <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                            <asp:ListItem Value="1">Any</asp:ListItem>
                                            <asp:ListItem Value="2">Custom</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td style="width: 20%">
                                        <asp:TextBox ID="txtCustomFilter" runat="server" Enabled="False" OnTextChanged="txtCustomFilter_TextChanged"
                                            AutoPostBack="True"></asp:TextBox>
                                    </td>
                                    <td>
                                        <uc3:StatusLabel ID="ctrlStatusCustomFilter" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <uc2:ManageFilters ID="ManageFiltersControl" runat="server" />
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
                <div class="buttons">
                    <table>
                        <tr>
                            <td style="width: 40%;">
                                <uc3:StatusLabel ID="lblMessageForm" runat="server" />
                            </td>
                            <td style="width: 60%;" class="tableTDLeftTop">
                                <asp:Button ID="btnApply" runat="server" Text="Apply" OnClick="btnApply_Click" OnClientClick="validate();" />
                                &nbsp;
                                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validate();"
                                    Text="Submit" />
                                &nbsp;
                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                    Text="Cancel" />
                                <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldIsCopyMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
