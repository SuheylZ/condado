<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageRetentions.aspx.cs" Inherits="Admin_ManageRetentions" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/Schedule.ascx" TagName="Schedule" TagPrefix="uc4" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updRetention" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
            <asp:HiddenField ID="hdnFieldIsCopyMode" runat="server" />
            <asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
            <asp:Panel ID="pnlGrid" runat="server" Visible="true">
                <fieldset class="condado">
                    <legend>Manage Retention</legend>
                    <div id="divToolbar" class="Toolbar">
                        <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Queue" CausesValidation="False"
                            OnClick="btnAddNewQueue_Click" />
                        &nbsp; &nbsp; &nbsp;
                        <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                    </div>
                    <telerik:RadGrid ID="grdRetention" runat="server" Skin="" CssClass="mGrid" Width="100%"
                        CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                        AutoGenerateColumns="False" OnItemDataBound="grdRetention_ItemDataBound" OnRowDrop="grdRetention_RowDrop"
                        OnItemCommand="grdRetention_ItemCommand" SelectedItemStyle-CssClass="gridHeader">
                        <ClientSettings AllowRowsDragDrop="True">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                            <%--<ClientEvents OnRowDropping="onRowDropping"></ClientEvents>--%>
                        </ClientSettings>
                        <MasterTableView DataKeyNames="Id,Priority">
                            <NoRecordsTemplate>
                                There is no prioritization record..
                            </NoRecordsTemplate>
                            <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                                ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                                ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                                ExportToPdfText="Export to PDF" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                            <RowIndicatorColumn Visible="True" FilterControlAltText="Retention Rule RowIndicator column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>
                            <ExpandCollapseColumn Visible="True" FilterControlAltText="Retention Rule ExpandColumn column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                            <Columns>
                                <telerik:GridTemplateColumn FilterControlAltText="Priority record up/down" UniqueName=""
                                    HeaderText="Priority">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgBtnUpOrder" runat="server" CommandName="UpOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/UpArrow.png" ToolTip="Priority Up" />
                                        <asp:ImageButton ID="imgBtnDownOrder" runat="server" CommandName="DownOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/DownArrow.png" ToolTip="Priority Down" />
                                    </ItemTemplate>
                                    <ItemStyle Width="6%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Title" FilterControlAltText="Retention Rule Title column"
                                    HeaderText="Title" UniqueName="Title">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="70%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Retention record enabled" UniqueName="Enabled"
                                    HeaderText="Enabled">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive"))? "Yes": "No" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Retention options column" UniqueName="options"
                                    HeaderText="Options">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Edit"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkCopy" runat="server" CausesValidation="False" Text="Copy"
                                            CommandName="CopyX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
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
            </asp:Panel>
            <asp:Panel ID="pnlDetail" runat="server" Visible="false">
                <div class="buttons" style="text-align: right">
                    <asp:Button runat="server" Text="Return to Queues" ID="btnReturn" OnClick="btnCancelOnForm_Click"
                        CausesValidation="False" class="returnButton" />
                </div>
                <telerik:RadTabStrip ID="tlRetentionStrip" runat="server" SelectedIndex="0" Skin="WebBlue"
                    MultiPageID="tlPages" OnTabClick="tlRetentionStrip_TabClick">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Queue Details" PageViewID="pgQueueDetail" />
                        <telerik:RadTab runat="server" Text="Schedule: What does it run?" PageViewID="pgSchedule" />
                        <telerik:RadTab runat="server" Text="Lead Filters" PageViewID="pgFilters" />
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tlPages" runat="server" SelectedIndex="0">
                    <telerik:RadPageView ID="pgQueueDetail" runat="server">
                        <fieldset id="fldSetForm" class="condado">
                            <legend>Add/Edit Retention</legend>
                            <ul>
                                <li>
                                    <asp:Label ID="lblUser" runat="server" AssociatedControlID="ddlUsers" Text="Retention User" />
                                    <asp:DropDownList ID="ddlUsers" runat="server" DataTextField="FullName" DataValueField="Key" />
                                </li>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title" />
                                    <asp:TextBox ID="txtTitle" runat="server" />
                                    <asp:RequiredFieldValidator ID="reqFldVldTitle" runat="server" ControlToValidate="txtTitle"
                                        Display="None" ErrorMessage="Enter title." />
                                </li>
                                <li>
                                    <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription"
                                        Text="Description" />
                                    <asp:TextBox ID="txtDescription" runat="server" Height="100px" TextMode="MultiLine" />
                                </li>
                                <li>
                                    <asp:Label ID="lblEnabled" runat="server" AssociatedControlID="chkEnabled" Text="Enabled" />
                                    <asp:CheckBox ID="chkEnabled" runat="server" />
                                    <asp:ValidatorCalloutExtender ID="reqFldVldTitle_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="reqFldVldTitle" />
                                </li>
                            </ul>
                        </fieldset>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="pgSchedule" runat="server">
                        <uc4:Schedule ID="ctrlShiftSchedule" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="pgFilters" runat="server">
                        <fieldset class="condado">
                            <legend>Filter Settings</legend>
                            <table>
                                <tr>
                                    <td style="width: 20%">
                                        <asp:RadioButtonList ID="rdBtnlstFilterSelection" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="rdBtnlstFilterSelection_SelectedIndexChanged" RepeatDirection="Horizontal"
                                            Width="200px">
                                            <asp:ListItem Value="0" Selected="True">All</asp:ListItem>
                                            <asp:ListItem Value="1">Any</asp:ListItem>
                                            <asp:ListItem Value="2">Custom</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td style="width: 20%">
                                        <asp:TextBox ID="txtCustomFilter" runat="server" Enabled="False" OnTextChanged="txtCustomFilter_TextChanged"
                                            AutoPostBack="True" />
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
                                <asp:Button ID="btnApply" runat="server" Text="Save" OnClick="btnApply_Click" OnClientClick="validate();" />
                                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validate();"
                                    Text="Save & Close" />
                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                    Text="Close" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
