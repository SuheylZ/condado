<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" EnableViewState="true"
    CodeFile="ReportDisplay.aspx.cs" Inherits="Reports_ReportDisplay" %>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc3" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript" id="telerikClientEvents2">
        //<![CDATA[
      var myDlg = null;
      function evt_MenuItem_clicked(sender, args) 
      {

          var item = args.get_item();
          var key = item.get_value();

          if (key == "delete") 
          {
              $("span#<%= ctlStatus.ClientID %>").hide();
              item.get_parent().hide();

              if (confirm("Are you sure to delete the record?") == true)
                  args.set_cancel(false);
              else {
                  args.set_cancel(true);
              }
              return;
          }
      }

      function showMenu(e) 
      {
          var contextMenu = $find('<%=tlMenuOptions.ClientID%>');
          $telerik.cancelRawEvent(e);

          if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
              contextMenu.show(e);
          }
      }
      function closeDlg(v) {
          closeDlgAssignAccount();
          return true;
      }
      function closeDlgAssignAccount() {
          if (myDlg != null) {
              myDlg.close();
              myDlg = null;
          }

          return false;
      }
        //]]>
    </script>
    <div runat="server" id="divCustomReportDisplay">
        <asp:HiddenField ID="hdnFieldReportID" runat="server" />
        <uc2:StatusLabel ID="ctlStatus" runat="server" />
        <fieldset class="condado">
            <legend>Report Display : </legend>
            <div class="buttons" style="text-align: right">
            <%--<asp:Button runat="server" Text="Export to doc file" ID="btnExportDoc" CausesValidation="False" CommandName = "doc"
                    OnClick="btnExportReport_Click"></asp:Button>--%>
            <asp:Button runat="server" Text="Export to CSV file" ID="btnExportCSV" CausesValidation="False" CommandName = "csv"
                    OnClick="btnExportReport_Click"></asp:Button>
            <asp:Button runat="server" Text="Export to excel file" ID="btnExportExcel" CausesValidation="False" CommandName = "excel"
                    OnClick="btnExportReport_Click"></asp:Button>
            <asp:Button runat="server" Text="Export to text file" ID="btnExportReport" CausesValidation="False" CommandName = "text"
                    OnClick="btnExportReport_Click"></asp:Button>
                <asp:Button runat="server" Text="Edit this report" ID="btnEditReport" CausesValidation="False"
                    OnClick="btnEditReport_Click"></asp:Button>
                &nbsp;<asp:Button runat="server" Text="Return to reports" ID="btnReturn" CausesValidation="False"
                    OnClick="btnReturn_Click"></asp:Button>
            </div>
            <asp:Label runat="server" ID="lblFilterValues" Text="" Visible="false"></asp:Label>
            <uc3:ManageFilters ID="ManageFiltersControl" runat="server" Visible="false" />
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                CancelSelectOnNullParameter="false"></asp:SqlDataSource>
                 <asp:Label ID="lblMessageForm" runat="server" CssClass="LabelMessage" Visible="false"  Text="No report found."></asp:Label>
            <uc1:PagingBar ID="ctlPaging" runat="server" OnSizeChanged="Evt_PageSizeChanged"
                OnIndexChanged="Evt_PageNumberChanged" NewButtonTitle="" />
                 <telerik:RadContextMenu ID="tlMenuOptions" runat="server" CollapseDelay="250" OnItemClick="Evt_Menu_Router"
                    OnClientItemClicking="evt_MenuItem_clicked" CssClass="menu" EnableTheming="True"
                    ForeColor="White" Skin="" CausesValidation="false">
                    <Targets>
                        <telerik:ContextMenuControlTarget ControlID="lnkOptions" />
                    </Targets>
                    <Items>
                        <telerik:RadMenuItem runat="server" Text="Select All" Value="select" val />
                        <telerik:RadMenuItem runat="server" Text="Deselect All" Value="deselect" />
                        <telerik:RadMenuItem runat="server" Text="Reassign Accounts" Value="reassign" />
                        <telerik:RadMenuItem runat="server" Text="Delete Accounts" Value="delete" Visible='<%# CurrentUser.UserPermissions.FirstOrDefault().Permissions.Account.SoftDelete %>' />
                    </Items>
                    <ExpandAnimation Duration="250" Type="Linear" />
                    <CollapseAnimation Duration="0" Type="None" />
                </telerik:RadContextMenu>
                                            <br/>
            <telerik:radgrid id="grdCustomReportRun" runat="server" autogeneratecolumns="false"
                skin="" cssclass="mGrid" width="100%" cellspacing="15" cellpadding = "20" gridlines="None" enabletheming="false" AllowMultiRowSelection="True"
                onitemcommand="grdCustomReportRun_ItemCommand" onfocus="this.blur();" AllowSorting="true" OnSortCommand="grdCustomReportRun_SortGrid">
                        <AlternatingItemStyle CssClass="alt" />
                        <ClientSettings EnableRowHoverStyle="true">
                                <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                            </ClientSettings>
                        <%--<ExportSettings IgnorePaging="true" OpenInNewWindow="true">
                            <Pdf PageHeight="210mm" PageWidth="297mm" PageTitle="SushiBar menu" DefaultFontFamily="Arial Unicode MS"
                                PageBottomMargin="20mm" PageTopMargin="20mm" PageLeftMargin="20mm" PageRightMargin="20mm">
                            </Pdf>
                        </ExportSettings>--%>
                        <ExportSettings HideStructureColumns="true" FileName="report" OpenInNewWindow="true" IgnorePaging="true" >
                        </ExportSettings>
                        <MasterTableView  CommandItemDisplay="Top">
                            <NoRecordsTemplate>
                                There are no record to display.
                            </NoRecordsTemplate>
                            <CommandItemSettings ShowAddNewRecordButton="false"
                                ShowRefreshButton="false" />
                            <%--<CommandItemSettings ShowExportToPdfButton="true" ShowExportToCsvButton="true" ShowExportToWordButton="true"
                                ShowExportToExcelButton="true" ExportToPdfText="Export to PDF" ShowAddNewRecordButton="false"
                                ShowRefreshButton="false" />--%>
                            <RowIndicatorColumn Visible="True" FilterControlAltText="RowIndicator column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>
                            <ExpandCollapseColumn Visible="True" FilterControlAltText="ExpandColumn column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                            <Columns>
                      
                            </Columns>              
                        </MasterTableView>
                        <HeaderStyle CssClass="gridHeader" />            
        </telerik:radgrid>
        </fieldset>
    </div>
  <telerik:RadWindow ID="dlgAssignAccount" runat="server" Width="450" Height="175"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Assign/ Reassign Account">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel2" runat="server">
                <ContentTemplate>
                    <p>
                    <uc2:StatusLabel ID="lblMessageAssignUsers" runat="server" />                        
                    </p>
                    <table style="width: 100%; height: 100%">
                        <tr style="display: none;">
                            <td>
                                <span>&nbsp;Account ID :&nbsp;</span><asp:Label ID="lbAccountID" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Assign Type :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlAssignType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAssignType_SelectedIndexChanged"
                                    DataTextField="FullName" DataValueField="Key" Width="150px" AppendDataBoundItems="True">
                                    <asp:ListItem>Agent</asp:ListItem>
                                    <asp:ListItem>CSR</asp:ListItem>
                                    <asp:ListItem>TA</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Users :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlAssignUsers" runat="server" DataTextField="FullName" DataValueField="Key"
                                    Width="150px" AppendDataBoundItems="True">
                                    <%--<asp:ListItem Value="-1">-- Unassigned  --</asp:ListItem>--%>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlAssignUsers"
                                    ErrorMessage="Select user." ForeColor="#CC0000" ValidationGroup="vgAssignUser"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="height: 50px;">
                                <div class="buttons" align="center" style="height: 40px">
                                    <%--OnClientClick="return validate('vgAssignUser');"--%>
                                    <asp:Button ID="btnSaveAssignUser" runat="server" Text="Save" ValidationGroup="vgAssignUser"
                                        OnClick="SaveAssignUser_Click" Width="80px" Height="30px" />
                                    &nbsp;
                                    <asp:Button ID="btnCancelAssignUser" runat="server" CausesValidation="false" Text="Cancel" OnClick="btnCancelAssignUser_Click"
                                         Width="80px" Height="30px" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                <asp:PostBackTrigger ControlID="btnCancelAssignUser" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
