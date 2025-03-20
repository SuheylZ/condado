<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AutoHomePolicy.ascx.cs" Inherits="Leads_UserControls_Policy" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="IndividualBox.ascx" TagName="IndividualBox" TagPrefix="uc3" %>

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
    <div runat="server" id="divGrid" visible="true" class="condado">
        <asp:Button ID="btnAddNew" runat="server" Text="Add New Auto/Home Policy" CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();"
            class="resetChangeFlag" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Policy Type&nbsp;
        <asp:DropDownList ID="ddlPolicyTypeMain" Width="200px" runat="server" AutoPostBack="True">
           <%-- <asp:ListItem Value="-1">All Filter</asp:ListItem>
            <asp:ListItem Value="0">Auto</asp:ListItem>
            <asp:ListItem Value="1">Home</asp:ListItem>
            <asp:ListItem Value="2">Renter</asp:ListItem>
            <asp:ListItem Value="3">Umbrella</asp:ListItem>
            <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
            <asp:ListItem Value="5">Secondary Home</asp:ListItem>
            <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
            <asp:ListItem Value="7">Wind</asp:ListItem>
            <asp:ListItem Value="8">Flood</asp:ListItem>
            <asp:ListItem Value="9">Other</asp:ListItem>--%>
        </asp:DropDownList>
        <br />
        <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
        <br />
        <br />
        <asp:GridView ID="grdHome" runat="server" Width="100%" DataKeyNames="ID"
            ShowHeaderWhenEmpty="True"
            AutoGenerateColumns="False" AllowSorting="True" GridLines="None"
            AlternatingRowStyle-CssClass="alt" OnRowDataBound="grdHome_RowDataBound">
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <Columns>
                <asp:BoundField HeaderText="Policy Type" SortExpression="PolicyType" DataField="PolicyType" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Policy Holder" SortExpression="PolicyHolder" DataField="PolicyHolder" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Policy Number" SortExpression="PolicyNumber" DataField="PolicyNumber" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Policy Status" SortExpression="PolicyStatus" DataField="PolicyStatus" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Carrier" SortExpression="Carrier" DataField="Carrier" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Current Carrier" SortExpression="CurrentCarrier" DataField="CurrentCarrier" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Premium" SortExpression="MonthlyPremium" DataField="MonthlyPremium" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Term" SortExpression="Term" DataField="Term" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Umbrella Policy" SortExpression="UmbrellaPolicy" DataField="UmbrellaPolicy" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="Bound Date" SortExpression="BoundDate" DataField="BoundDate" DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-HorizontalAlign="Left">
                    <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                </asp:BoundField>
                <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false"
                            CommandName="EditX" Text="Edit"></asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                            Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;"></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="false" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" CommandArgument="" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                There are no Policies to display.
            </EmptyDataTemplate>
            <HeaderStyle CssClass="gridHeader" />
            <PagerSettings Position="Top" />
            <PagerStyle VerticalAlign="Bottom" />
        </asp:GridView>
    </div>
    <telerik:RadWindow ID="dlgAutoHomePolicy" runat="server" Width="900" Height="500"
         Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
        IconUrl="../Images/alert1.ico" Title="Add New Auto Home Policy">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel10" runat="server">
                <ContentTemplate>
                    <fieldset style="margin: 10px">
                        <div runat="server" id="divForm" visible="true">
                            <uc2:StatusLabel ID="ctlStatus" runat="server" />
                            <div id="fldSetForm" class="condado">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button runat="server" Visible="false"  Text="Return to Auto/Home Policies" ID="btnReturn"
                                        CausesValidation="False" class="returnButton"></asp:Button>
                                </div>
                                <table runat="server" id="tblForm">
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblIndividualName" runat="server" CssClass="uclabel" Text="Individual:"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddIndividualName" runat="server" Width="200px" DataTextField="FullName" DataValueField="Id" />

                                            <asp:Button ID="btnAddNewIndividual" runat="server" CausesValidation="false" Text="Add" OnClick="btnAddNewIndividual_Click" />
                                            <!--On Client Click="return showDlg('Individual');" -->
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtCompanyName" Text="Company Name :" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCompanyName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblPolicyType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlPolicyType" Text="Policy Type:" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlPolicyType" Width="200px" runat="server" AutoPostBack="True">
                                              <%--  <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="0">Auto</asp:ListItem>
                                                <asp:ListItem Value="1">Home</asp:ListItem>
                                                <asp:ListItem Value="2">Renter</asp:ListItem>
                                                <asp:ListItem Value="3">Umbrella</asp:ListItem>
                                                <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
                                                <asp:ListItem Value="5">Secondary Home</asp:ListItem>
                                                <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
                                                <asp:ListItem Value="7">Wind</asp:ListItem>
                                                <asp:ListItem Value="8">Flood</asp:ListItem>
                                                <asp:ListItem Value="9">Other</asp:ListItem>--%>
                                            </asp:DropDownList>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblPolicyNumber" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtPolicyNumber" Text="Policy Number:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtPolicyNumber" runat="server" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="vldPolicyNumber" runat="server" 
                                                Display="None" ErrorMessage="Policy Number" ControlToValidate="txtPolicyNumber"></asp:RequiredFieldValidator>
                                            <%-- SR 26.3.2014 <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                Enabled="True" TargetControlID="vldPolicyNumber" Width="250px" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="lblEffectiveDate" runat="server" CssClass="uclabel"
                                                AssociatedControlID="rdpEffective" Text="Effective Date:" /></td>
                                        <td>
                                            <telerik:RadDatePicker ID="rdpEffective" runat="server">
                                            </telerik:RadDatePicker>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblBoundDate" runat="server" CssClass="uclabel"
                                                AssociatedControlID="rdpBound" Text="Bound Date:" />
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="rdpBound" runat="server" Enabled="false" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblLapseDate" runat="server" CssClass="uclabel"
                                                AssociatedControlID="rdpBound" Text="Lapse Date:" /></td>
                                        <td>
                                            <telerik:RadDatePicker ID="rdpLapse" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblUmbrellaPolicy" runat="server" Visible="false" CssClass="uclabel"
                                                AssociatedControlID="ddlUmbrellaPolicy" Text="Umbrella Policy:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlUmbrellaPolicy" Width="200px" runat="server" Visible="false">
                                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                                <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblCarrier" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlCarrier" Text="Carrier:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlCarrier" Width="200px" runat="server" DataTextField="Name" DataValueField="Key">
                                            </asp:DropDownList>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblCurrentCarrier" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlCurrentCarrier" Text="Current Carrier:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlCurrentCarrier" Width="200px" runat="server" DataTextField="Name" DataValueField="Key" Visible="false">
                                            </asp:DropDownList>
                                            <asp:TextBox ID="txtCurrentCarrierPolicy" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblMonthlyPremium" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtMonthlyPremium" Text="Premium:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtMonthlyPremium" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblCurrentMonthlyPremium" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtCurrentMonthlyPremium" Text="Current Premium:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtCurrentMonthlyPremium" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblTerm" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlTerm" Text="Term:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTerm" Width="200px" runat="server">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="6">6 Months</asp:ListItem>
                                                <asp:ListItem Value="12">12 Months</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblDidWeIncreaseCoverage" runat="server" CssClass="uclabel"
                                                AssociatedControlID="cbxDidWeIncreaseCoverage" Text="Did We Increase Coverage?:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:CheckBox ID="cbxDidWeIncreaseCoverage" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblWritingAgent" runat="server" CssClass="uclabel" AssociatedControlID="ddlWritingAgent" Text="Writing Agent:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlWritingAgent" Width="200px" runat="server" DataTextField="FullName" DataValueField="Key">
                                            </asp:DropDownList>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblPolicyStatuses" runat="server" AssociatedControlID="cbxDidWeIncreaseCoverage" CssClass="uclabel"
                                                Text="Policy Status:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlPolicyStatus" Width="200px" runat="server" DataTextField="Name" DataValueField="Key"></asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <asp:HiddenField ID="hdnRecordId" runat="server" />
                        <asp:Button ID="btnSaveOnForm" runat="server" OnClientClick="validateGroup('AutoHomePolicyControl');" ValidationGroup="AutoHomePolicyControl" Text="Save" />
                        <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('AutoHomePolicyControl');" Text="Save and Close" ValidationGroup="AutoHomePolicyControl" />
                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" class="returnButton" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>

