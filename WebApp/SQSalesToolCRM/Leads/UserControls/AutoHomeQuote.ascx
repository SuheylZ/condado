<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AutoHomeQuote.ascx.cs"
    Inherits="UserControls_AutoHomeQuote" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc2" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>


<asp:HiddenField ID="hdnRecordId" runat="server" />
<asp:HiddenField ID="hdnIsInitialLoaded" runat="server" Value="false" />
<script type="text/javascript">
    function confirmDeleteRecord(button) {
        function aspButtonCallbackFn(arg) {
            if (arg) {
                __doPostBack(res, "");
            }
        }
        var str = button.pathname;
        var res = '';
        if (str != '')
            res = str.replace("__doPostBack('", "").replace("','')", "");
        else {
            str = button.href;
            res = str.replace("javascript:__doPostBack('", "").replace("','')", "");
        }
        radconfirm("Are you sure you want to delete the record?", aspButtonCallbackFn, 330, 120, null, "Confirm");
    }

</script>
<div id="mainDv" class="condado">
    <div id="divGrid" runat="server">
        <br />
        <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Quote" CausesValidation="False" OnClientClick="setChangeFlag('0');" />

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    Quote Type:
    <asp:DropDownList ID="ddlQuoteTypeMain" runat="server" Width="200px" AutoPostBack="True">
        <asp:ListItem Value="-1">All Filter</asp:ListItem>
        <asp:ListItem Value="0">Auto</asp:ListItem>
        <asp:ListItem Value="1">Home</asp:ListItem>
        <asp:ListItem Value="2">Renter</asp:ListItem>
        <asp:ListItem Value="3">Umbrella</asp:ListItem>
        <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
        <asp:ListItem Value="5">Secondary Home</asp:ListItem>
        <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
        <asp:ListItem Value="7">Wind</asp:ListItem>
        <asp:ListItem Value="8">Flood</asp:ListItem>
        <asp:ListItem Value="9">Other</asp:ListItem>
    </asp:DropDownList>
        <br />
        <uc2:PagingBar ID="ctlPaging" runat="server" NewButtonTitle="" />
        <br />
        <br />
        <telerik:RadGrid ID="grdAutoHomeQuotes" runat="server" Skin="" CssClass="mGrid" Width="100%"
            CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
            AutoGenerateColumns="False" SelectedItemStyle-CssClass="gridHeader" OnItemDataBound="grdAutoHomeQuotes_ItemDataBound">
            <MasterTableView DataKeyNames="Id">
                <NoRecordsTemplate>
                    There is no quote record.
                </NoRecordsTemplate>
                <CommandItemSettings ExportToPdfText="Export to PDF" ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2013.1.417.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2013.1.417.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2013.1.417.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif" ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2013.1.417.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"></CommandItemSettings>
                <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column"></RowIndicatorColumn>
                <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column"></ExpandCollapseColumn>
                <Columns>
                    <telerik:GridBoundColumn DataField="Carrier" HeaderText="Carrier" UniqueName="Carrier">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="QuoteType" HeaderText="QuoteType"
                        UniqueName="QuoteType" FilterControlAltText="Filter QuoteType column">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="QuotedDate" HeaderText="Date Quoted" DataFormatString="{0: MM/dd/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="QuotedPremium" HeaderText="Amount Quoted" DataFormatString="{0:f2}">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CurrentPremium" HeaderText="Current Premium" DataFormatString="{0:f2}">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="colEdit" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Edit"></asp:LinkButton>
                            |
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                            Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="20%" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colView" Visible="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="View" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings>
                    <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                    </EditColumn>
                </EditFormSettings>
                <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
            </MasterTableView>
            <HeaderStyle CssClass="gridHeader" />
            <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
            <SelectedItemStyle CssClass="gridHeader"></SelectedItemStyle>
            <FilterMenu EnableImageSprites="False">
            </FilterMenu>
        </telerik:RadGrid>
    </div>
    <telerik:RadWindow ID="dlgAutoHomeQuote" runat="server" Width="900" Height="300"
         Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
        IconUrl="../Images/alert1.ico" Title="Add New Quote">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel10" runat="server">
                <ContentTemplate>
                    <fieldset style="margin: 10px">
                        <div id="divForm" runat="server">
                            <uc1:StatusLabel ID="ctlStatus" runat="server" />
                            <uc1:StatusLabel ID="StatusLabel1" runat="server" />
                            <span style="float: right; width: 100%; text-align: right;">
                                <asp:Button ID="btnReturn" Visible="false" runat="server" CausesValidation="False" Text="Return to Quotes" />
                            </span>
                            <div id="divControls" runat="server">
                                <table width="100%">

                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtCompanyName" Text="Company Name :" />
                                        </td>
                                        <td colspan="3">
                                            <asp:TextBox ID="txtCompanyName" runat="server" Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblQuoteType" runat="server" CssClass="uclabel" Text="Quote Type:" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlQuoteType" runat="server" Width="200px"
                                                AutoPostBack="True">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="0">Auto</asp:ListItem>
                                                <asp:ListItem Value="1">Home</asp:ListItem>
                                                <asp:ListItem Value="2">Renter</asp:ListItem>
                                                <asp:ListItem Value="3">Umbrella</asp:ListItem>
                                                <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
                                                <asp:ListItem Value="5">Secondary Home</asp:ListItem>
                                                <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
                                                <asp:ListItem Value="7">Wind</asp:ListItem>
                                                <asp:ListItem Value="8">Flood</asp:ListItem>
                                                <asp:ListItem Value="9">Other</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="vlddlQuoteType" runat="server"
                                                Display="None" ErrorMessage="Quote type required." ControlToValidate="ddlQuoteType"></asp:RequiredFieldValidator>
                                            <%--  <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                Enabled="True" TargetControlID="vlddlQuoteType" Width="250px" />--%>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblSaving" runat="server" CssClass="uclabel" Text="Did We Show Savings?:" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSavings" runat="server" Width="200px">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="3">Yes</asp:ListItem>
                                                <asp:ListItem Value="1">No</asp:ListItem>
                                                <asp:ListItem Value="2">N/A</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQuotedCarrier" runat="server" CssClass="uclabel" Text="Quoted Carrier:" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlQuotedCarrier" runat="server" Width="200px" DataTextField="Name"
                                                DataValueField="Key">
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblCurrentCarrier" runat="server" CssClass="uclabel" Text="Current Carrier:" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlCurrentCarrier" runat="server" Width="200px" DataTextField="Name"
                                                DataValueField="Key" Visible="false">
                                            </asp:DropDownList>
                                            <asp:TextBox ID="txtCurrentCarrierQuote" runat="server"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQuotedPremium" runat="server" CssClass="uclabel" Text="Quoted Premium:" />
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtQuotedPremium" runat="server" Width="100px" MinValue="0"
                                                MaxValue="999999999">
                                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblCurrentPremium" runat="server" CssClass="uclabel" Text="Current Premium:" />
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCurrentPremium" runat="server" Width="100px" MinValue="0"
                                                MaxValue="999999999">
                                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQuoteDate" runat="server" CssClass="uclabel" Text="Quote Date:" />
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="tlQuoteDate" runat="server" Enabled="false">
                                                <Calendar ID="tlDateOnlyCalendar" runat="server">
                                                </Calendar>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td></td>
                                        <td id="tdUmbrellalabel" runat="server">
                                            <asp:Label ID="lblUmbrellaQuoted" runat="server" CssClass="uclabel" Text="Umbrella Quoted:" Visible="false" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlUmbrellaQuoted" runat="server" Width="200px" Visible="false">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="0">Yes</asp:ListItem>
                                                <asp:ListItem Value="1">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnApply" runat="server" Text="Save" OnClientClick="validateGroup('autohomequote');" ValidationGroup="autohomequote" />
                        <asp:Button ID="btnSubmit" runat="server" OnClientClick="validateGroup('autohomequote');" ValidationGroup="autohomequote" Text="Save &amp; Close" />
                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>
