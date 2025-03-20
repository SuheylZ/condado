<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" EnableEventValidation="false"
    EnableViewState="true" CodeFile="CustomReports.aspx.cs" Inherits="Reports_CustomReports" %>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/ColumnSelectionLists.ascx" TagName="ColumnSelectionLists"
    TagPrefix="uc4" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc5" %>
<%@ Register src="../UserControls/EmailEditor.ascx" tagname="EmailEditor" tagprefix="uc6" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
            <fieldset class="condado">
                <legend>Custom Reports</legend>
                <div id="mainDv">
                    <uc2:StatusLabel ID="ctlStatus" runat="server" />
                    <div runat="server" id="divGrid" visible="true" class="Toolbar">
                        <div class="buttons" style="text-align: left;height:20px;">
                            <div style="float: left;">
                                <asp:Button runat="server" Text="Add New Report" ID="btnNewReport" CausesValidation="False"
                                    OnClick="btnNewReport_Click"></asp:Button>
                            </div>
                            <div style="float: right; padding-left: 10px; margin-left: 10px;">
                                <asp:TextBox ID="txtSerachReport" runat="server"></asp:TextBox>
                                <asp:Button ID="btnSerachGo" runat="server" OnClick="btnSerachGo_Click" Text="GO"
                                    CausesValidation="false" />
                            </div>
                            <div style="float: left; padding-left: 10px; margin-left: 10px;">
                                <asp:DropDownList ID="ddlBaseDataGrid" runat="server" Width="150px" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlBaseDataGrid_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Value="0">---All Base Data---</asp:ListItem>
                                    <asp:ListItem Value="1">Accounts</asp:ListItem>
                                    <asp:ListItem Value="2">Account History</asp:ListItem>
                                    <asp:ListItem Value="3">Lead History</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div id="fldSetGrid" class="condado" style="clear: both;">
                            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                            <br />
                            <telerik:radgrid id="grdReports" runat="server" skin="" cssclass="mGrid" width="100%"
                                cellspacing="0" gridlines="None" enabletheming="False" onfocus="this.blur();"
                                allowsorting="true" autogeneratecolumns="False" onitemcommand="grdReports_RowCommand"
                                onsortcommand="grdReports_SortGrid" selecteditemstyle-cssclass="gridHeader">
                        <AlternatingItemStyle CssClass="alt" />
                        <MasterTableView DataKeyNames="ReportID">
                            <NoRecordsTemplate>
                                No record found.
                            </NoRecordsTemplate>
                            <Columns>
                                <telerik:GridBoundColumn DataField="ReportID" HeaderText="ID" SortExpression="ReportID"
                                    UniqueName="ReportID">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ReportTitle" FilterControlAltText="ReportTitle"
                                    SortExpression="ReportTitle" HeaderText="Title" UniqueName="ReportTitle">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="40%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="Owner" FilterControlAltText="Owner" SortExpression="Owner"
                                    HeaderText="Owner" UniqueName="Owner">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="10%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn UniqueName="BaseData" HeaderText="BaseData" SortExpression="BaseData">
                                    <ItemTemplate>
                                        <asp:Label ID="lnkBaseData" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "BaseData") %>'
                                            Text='<%# GetBaseDataString(DataBinder.Eval(Container.DataItem, "BaseData")) %>'></asp:Label>

                                            <%--Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "BaseData"))? "Leads": "None"--%>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="10%" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="options" HeaderText="Options">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkRun" runat="server" CausesValidation="false" CommandName="RunX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Run"
                                            class="resetChangeFlag"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Edit"
                                            class="resetChangeFlag"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkCopy" runat="server" CausesValidation="false" CommandName="CopyX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Copy"
                                            class="resetChangeFlag"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkEmail" runat="server" CausesValidation="false" CommandName="EmailX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Email"
                                            class="resetChangeFlag"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkUsers" runat="server" CausesValidation="false" CommandName="UsersX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Users"
                                            class="resetChangeFlag"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ReportID") %>' Text="Delete"
                                            OnClientClick="if(confirm('Are you sure want to delete report?')== true){ return true;} else { return false;}"></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="20%" HorizontalAlign="Center" />
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
                    </telerik:radgrid>
                        </div>
                    </div>
                    <div runat="server" id="divForm" visible="true">
                        <div id="fldSetForm" class="condado">
                            <div class="buttons" style="width: 60%">
                                <table cellpadding="2" bgcolor="Silver">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStep1Static" runat="server" Text="Step 1: Base Data &amp; Title"></asp:Label>
                                            <asp:Label ID="lblStep1Dynamic" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Button runat="server" Text="&lt;&lt; Step 1" ID="btnStep1Backward" CausesValidation="False"
                                                OnClick="btnStep1Backward_Click" Visible="False"></asp:Button>
                                        </td>
                                        <td>
                                            <asp:Button runat="server" Text="Step 2 &gt;&gt;" ID="btnStep2Forward" CausesValidation="False"
                                                OnClick="btnStep2Forward_Click" Visible="False"></asp:Button>
                                            <asp:Button runat="server" Text="&lt;&lt; Step 2" ID="btnStep2Backward" CausesValidation="False"
                                                Visible="False" OnClick="btnStep2Backward_Click"></asp:Button>
                                        </td>
                                        <td>
                                            <asp:Button runat="server" Text="Step 3 &gt;&gt;" ID="btnStep3Forward" CausesValidation="False"
                                                Visible="False" OnClick="btnStep3Forward_Click"></asp:Button>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSaveAndClose" runat="server" ValidationGroup="customreport" Text="Save and Close"
                                                OnClick="btnSaveAndCloseOnForm_Click" Visible="False" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSaveAndRun" runat="server" ValidationGroup="customreport" Text="Save and Run &gt;&gt;"
                                                OnClick="btnSaveAndRun_Click" Visible="False" />
                                            <asp:Button ID="btnSaveUsersAndClose" runat="server" 
                                                OnClick="btnSaveAndClose_Click" Text="Save and Close" 
                                                ValidationGroup="customreport" Visible="False" />
                                            <asp:Button ID="btnSaveEmailAndClose" runat="server" 
                                                OnClick="btnSaveEmailAndClose_Click" Text="Save and Close" 
                                                ValidationGroup="customreport" Visible="False" />
                                        </td>
                                        <td>
                                            <asp:Button runat="server" Text="Return to reports" ID="btnReturn" CausesValidation="False"
                                                class="returnButton" OnClick="btnReturn_Click"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div runat="server" id="divStep1">
                                <table>
                                    <tr>
                                        <td nowrap="nowrap" bgcolor="Silver" colspan="2">
                                            Report Information
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblBaseData" runat="server" Text="Base Data:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlBaseData" runat="server" Width="150px" AutoPostBack="True"
                                                OnSelectedIndexChanged="ddlBaseData_SelectedIndexChanged">                                                
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblReportTitle" runat="server" AssociatedControlID="txtReportTitle"
                                                Text="Report Title:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtReportTitle" runat="server" Width="423px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap" colspan="2">
                                            <asp:HiddenField ID="hdnFieldReportID" runat="server" />
                                            <asp:HiddenField ID="hdnFieldIsGridMode" runat="server" />
                                            <asp:HiddenField ID="hdnFieldIsTemporarySave" runat="server" />
                                            <asp:HiddenField ID="hdnFieldBaseData" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div runat="server" id="divStep2" >
                                <uc4:ColumnSelectionLists ID="ctlColumnSelection" runat="server" Title="Select Columns"
                                    TitleAvailable="Available Columns" TitleSelected="Columns in Report" />
                            </div>
                            <div runat="server" id="divStep3">
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
                                                <uc2:StatusLabel ID="ctrlStatusCustomFilter" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                <uc3:ManageFilters ID="ManageFiltersControl" runat="server" />
                            </div>
                            <div id="divAssignUsers" runat="server">
                                <uc5:SelectionLists ID="ctlSelection" runat="server" Title="Assign Users to Report"
                                    TitleAvailable="Users Not Assigned " TitleSelected="Assigned Users" />
                            </div>
                            <div id="divEmailEditor" runat="server">
                               
                                <uc6:EmailEditor ID="EmailEditorCustomReport" runat="server" />
                               
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
       <asp:PostBackTrigger ControlID="grdReports" />
       <asp:PostBackTrigger ControlID="btnSaveEmailAndClose" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
