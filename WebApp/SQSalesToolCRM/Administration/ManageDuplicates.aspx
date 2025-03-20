<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageDuplicates.aspx.cs" Inherits="Admin_ManageDuplicates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/Schedule.ascx" TagName="Schedule" TagPrefix="uc4" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc5" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
            <uc3:StatusLabel ID="ctlStatus" runat="server" />
            <div id="divGrid" runat="server">
                <fieldset class="condado">
                    <legend>Manage Lead Duplicate(s) Rules</legend>
                    <div id="divToolbar" class="Toolbar">
                        <table>
                            <tr>
                                <td style="width: 30%;">
                                    <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Queue" CausesValidation="False"
                                        OnClick="btnAddNewQueue_Click" />
                                </td>
                                <td style="width: 70%;"></td>
                            </tr>
                        </table>
                    </div>
                    <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                        OnIndexChanged="Evt_PageNumberChanged" />
                    <telerik:RadGrid ID="grdLeadDuplicateRules" runat="server" Skin="" CssClass="mGrid" Width="100%"
                        CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                        AutoGenerateColumns="False" OnItemDataBound="grdLeadDuplicateRules_ItemDataBound"
                        OnRowDrop="grdLeadDuplicateRules_RowDrop" OnItemCommand="grdLeadDuplicateRules_ItemCommand"
                        SelectedItemStyle-CssClass="gridHeader">
                        <AlternatingItemStyle CssClass="alt" />
                        <ClientSettings AllowRowsDragDrop="True">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>

                        </ClientSettings>
                        <MasterTableView DataKeyNames="Id,Priority">
                            <NoRecordsTemplate>
                                No record found.
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
                                    HeaderText="Sort">
                                    <ItemTemplate>

                                        <asp:ImageButton ID="imgBtnUpOrder" runat="server" CommandName="UpOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/UpArrow.png" />
                                        <asp:ImageButton ID="imgBtnDownOrder" runat="server" CommandName="DownOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/DownArrow.png" />
                                    </ItemTemplate>
                                    <ItemStyle Width="6%" HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Title" FilterControlAltText="Rule Title column"
                                    HeaderText="Title" UniqueName="Title">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="FilterText" FilterControlAltText="Rule Filters"
                                    HeaderText="FilterText" UniqueName="FilterText">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="50%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Rule record enabled" UniqueName="Enabled"
                                    HeaderText="Enabled">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive"))? "Yes": "No" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="options column" UniqueName="options"
                                    HeaderText="Options">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Edit"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                            Text="Delete" OnClientClick="if(confirm('Are you sure want to delete record?')== true) true; else return false;"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                                    </ItemTemplate>
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
                <div class="buttons" style="text-align: right" id="returnButton">
                    <asp:Button runat="server" Text="Return to Programs" ID="btnReturn"
                        OnClick="btnCancelOnForm_Click" CausesValidation="False" class="returnButton"></asp:Button>
                </div>
                <telerik:RadTabStrip ID="tlDuplicatesStrip" runat="server" SelectedIndex="0"
                    Skin="WebBlue" MultiPageID="tlMultiPage"
                    OnTabClick="tlDuplicatesStrip_TabClick">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Program Details" PageViewID="tlPageQueueDetails"
                            Selected="True" Owner="tlDuplicatesStrip">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Which incoming leads should be checked?" PageViewID="tlPageIncomingleadCheck"
                            Owner="tlDuplicatesStrip">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="What are the rules for identifying duplicates?" PageViewID="tlPageFilters" Owner="tlDuplicatesStrip">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Auto Reconciliation Rules" Visible="false" PageViewID="tlPageAutoReconciliationRules" Owner="tlDuplicatesStrip" Enabled="false">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Visible="false" Text="Merge Rules" PageViewID="tlPageMergeRules" Owner="tlDuplicatesStrip" Enabled="false">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tlMultiPage" runat="server" SelectedIndex="0">
                    <telerik:RadPageView ID="tlPageQueueDetails" runat="server">
                        <fieldset id="fldSetForm" class="condado">
                            <legend>Add/Edit Duplicate Rule</legend>
                            <ul>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title:"></asp:Label><asp:TextBox
                                        ID="txtTitle" runat="server" Width="200px"></asp:TextBox><asp:RequiredFieldValidator
                                            ID="reqFldVldTitle" runat="server" ControlToValidate="txtTitle" Display="None"
                                            ErrorMessage="Enter title."></asp:RequiredFieldValidator></li>
                                <li>
                                    <asp:Label ID="lblEnabled" runat="server" AssociatedControlID="chkEnabled" Text="Enabled:"></asp:Label>
                                    <asp:CheckBox ID="chkEnabled" runat="server" />

                                </li>
                                <li>
                                    <asp:Label ID="lblReconciliation" runat="server" AssociatedControlID="chkManual" Text="Reconciliation:"></asp:Label>
                                    <asp:CheckBox ID="chkManual" runat="server" Checked="true" />
                                </li>
                            </ul>
                        </fieldset>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageIncomingleadCheck" runat="server">
                        <fieldset class="condado">
                            <legend>Filter Settings</legend>
                            <table>
                                <tr>
                                    <td style="width: 20%">
                                        <asp:RadioButtonList ID="rdBtnlstFilterSelectionForIncoming" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="rdBtnlstFilterSelectionForIncoming_SelectedIndexChanged" RepeatDirection="Horizontal"
                                            Width="200px">
                                            <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                            <asp:ListItem Value="1">Any</asp:ListItem>
                                            <asp:ListItem Value="2">Custom</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td style="width: 20%">
                                        <asp:TextBox ID="txtCustomFilterForIncoming" runat="server" Enabled="False" OnTextChanged="txtCustomFilterForIncoming_TextChanged"
                                            AutoPostBack="True"></asp:TextBox>
                                    </td>
                                    <td>
                                        <uc3:StatusLabel ID="ctrlStatusCustomFilterForIncoming" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <uc2:ManageFilters ID="ctlFiltersForIncomingLeads" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageFilters" runat="server">
                        <uc5:SelectionLists ID="ctlColumnSelection" runat="server" Title="Select which field(s) should be used to determine matches."
                            TitleAvailable="Available Fields" TitleSelected="Identify Duplicates By" />
                        <fieldset class="condado">
                            <legend>Filter Settings</legend>
                            <table>
                                <tr>
                                    <td style="width: 20%">
                                        <asp:RadioButtonList ID="rdBtnlstFilterSelectionForExisting" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="rdBtnlstFilterSelectionForExisting_SelectedIndexChanged" RepeatDirection="Horizontal" Width="200px">
                                            <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                            <asp:ListItem Value="1">Any</asp:ListItem>
                                            <asp:ListItem Value="2">Custom</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td style="width: 20%">
                                        <asp:TextBox ID="txtCustomFilterForExisting" runat="server" Enabled="False" OnTextChanged="txtCustomFilterForExisting_TextChanged" AutoPostBack="True"></asp:TextBox>
                                    </td>
                                    <td>
                                        <uc3:StatusLabel ID="ctrlStatusCustomFilterForExisting" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <uc2:ManageFilters ID="ctlFiltersForExistingLeads" runat="server" />
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="tlPageAutoReconciliationRules" Visible="false" runat="server" Enabled="false">
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageMergeRules" runat="server" Visible="false" Enabled="false">
                        <uc5:SelectionLists ID="ctlColumnSelectionMergeRules" runat="server" Title="Select which field(s) will be merged during the Manual or Automated Reconciliation process."
                            TitleAvailable="Available Fields" TitleSelected="Fields to be merged." />
                    </telerik:RadPageView>

                </telerik:RadMultiPage>
                <div class="buttons">
                    <table>
                        <tr>
                            <td style="width: 40%;"></td>
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
