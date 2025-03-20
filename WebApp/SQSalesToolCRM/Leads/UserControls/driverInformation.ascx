<%@ Control Language="C#" AutoEventWireup="true" CodeFile="driverInformation.ascx.cs" Inherits="Leads_UserControls_driverInformation" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>

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

<br />

<div id="mainDv" class="condado">

    <div runat="server" id="divGrid" visible="true" class="Toolbar">
        <div id="fldSetGrid" class="condado">

            <asp:Button ID="btnAddNew" runat="server" Text="Add New Driver"
                CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();"
                class="resetChangeFlag" />
            <br />

            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" />
            <br />
            <br />
            <asp:GridView ID="grdHome" runat="server" Width="100%" DataKeyNames="ID"
                AutoGenerateColumns="False" AllowSorting="True" GridLines="None" ShowHeaderWhenEmpty="True"
                AlternatingRowStyle-CssClass="alt" HeaderStyle-CssClass="gridHeader" OnRowDataBound="grdHome_RowDataBound">
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField HeaderText="First Name" SortExpression="FirstName" DataField="FirstName" HeaderStyle-HorizontalAlign="Left">
                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Last Name" SortExpression="LastName" DataField="LastName" HeaderStyle-HorizontalAlign="Left">
                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="License Number" SortExpression="LicenseNumber" DataField="LisenceNumber" HeaderStyle-HorizontalAlign="Left">
                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="License Status" SortExpression="LisenseStatus" DataField="LicenseStatus" HeaderStyle-HorizontalAlign="Left">
                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Age Licensed" SortExpression="AgeLicensed" DataField="AgeLicensed" HeaderStyle-HorizontalAlign="Left">

                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                    </asp:BoundField>

                    <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX" Text="Edit" />
                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX" Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;" />
                        </ItemTemplate>

                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                    </asp:TemplateField>

                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                    There are no Driver to display.
                </EmptyDataTemplate>

                <HeaderStyle CssClass="gridHeader"></HeaderStyle>

                <PagerSettings Position="Top" />
                <PagerStyle VerticalAlign="Bottom" />
            </asp:GridView>

        </div>
    </div>
    <telerik:RadWindow ID="dlgDriverInformation" runat="server" Width="900" Height="500"
         Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
        IconUrl="../Images/alert1.ico" Title="Add New Driver Information">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel10" runat="server">
                <ContentTemplate>
                    <fieldset style="margin: 10px">
                        <div runat="server" id="divForm" visible="true">
                            <uc2:StatusLabel ID="ctlStatus" runat="server" />
                            <div id="fldSetForm" class="condado">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button runat="server" Text="Return to Drivers" ID="btnReturn" Visible="false"
                                        CausesValidation="False" class="returnButton"></asp:Button>
                                </div>
                                <table runat="server" id="tblControls">
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblIndividual" runat="server" CssClass="uclabel" AssociatedControlID="ddlIndividual" Text="Driver's Name:" />
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlIndividual" runat="server" DataTextField="FullName" DataValueField="Id" />
                                            <asp:RequiredFieldValidator ID="rfvddlIndividual" runat="server" ControlToValidate="ddlIndividual" InitialValue="-1" Display="None" ErrorMessage="Please select a driver's name."></asp:RequiredFieldValidator>
                                            <%-- SR 26.3.2014 <asp:ValidatorCalloutExtender ID="vceddlIndividual" TargetControlID="rfvddlIndividual" Enabled="True" runat="server" Width="250px" />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblLicenseNumber" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtLicenseNumber" Text="Driver License Number:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtLicenseNumber" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="vldLicenseNumber" runat="server"
                                                Display="None" ErrorMessage="Driver License Number is required" ControlToValidate="txtLicenseNumber"></asp:RequiredFieldValidator>
                                            <%--SR 26.3.2014 <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" Enabled="True" TargetControlID="vldLicenseNumber" Width="250px" />--%>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblLicenseState" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtLicenseState" Text="License State:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtLicenseState" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblLicenseStatus" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtLicenseStatus" Text="License Status:" />
                                        </td>
                                        <td nowrap="nowrap" colspan="3">
                                            <asp:TextBox ID="txtLicenseStatus" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblMaritalStatus" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtMaritalStatus" Text="Marital Status:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtMaritalStatus" runat="server"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblAgeLicensed" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtAgeLicensed" Text="Age Licensed:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtAgeLicensed" runat="server" Culture="en-US"
                                                DbValueFactor="1" LabelWidth="64px" MaxValue="200"
                                                MinValue="0" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblYearsAtResidence" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtYearsAtResidence" Text="Years At Residence:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtYearsAtResidence" runat="server"
                                                Culture="en-US" DbValueFactor="1" MaxValue="200" MinValue="0" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblOccupation" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtOccupation" Text="Occupation:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtOccupation" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblYearsWithCompany" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtYearsWithCompany" Text="Years With Company:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtYearsWithCompany" runat="server"
                                                Culture="en-US" DbValueFactor="1"
                                                MaxValue="200" MinValue="0" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblYearsInField" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtYearsInField" Text="Years In Field:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtYearsInField" runat="server" Culture="en-US"
                                                DbValueFactor="1" MaxValue="255"
                                                MinValue="0" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblEducation" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtEducation" Text="Education:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtEducation" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblTickets" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtTickets" Text="Tickets/Accidents/Claims?:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtTickets" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblNumberOfIncidents" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtNumberOfIncidents" Text="Number Of Incidents:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtNumberOfIncidents" runat="server"
                                                Culture="en-US" DbValueFactor="1"
                                                MaxValue="255" MinValue="0" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblIncidentType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtIncidentType" Text="Incident Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtIncidentType" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblIncidentDescription" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtIncidentDescription" Text="Incident Description:" />
                                        </td>
                                        <td nowrap="nowrap" colspan="3">
                                            <asp:TextBox ID="txtIncidentDescription" runat="server" TextMode="MultiLine"
                                                Rows="4" Width="99%"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblLastIncidentDate" runat="server" CssClass="uclabel"
                                                AssociatedControlID="rdpLastIncidentDate" Text="Last Incident Date:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadDatePicker ID="rdpLastIncidentDate" runat="server" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblClaimPaidAmount" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtClaimPaidAmount" Text="Claim Paid Amount:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtClaimPaidAmount" runat="server">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblSR22" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtSR22" Text="SR22:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtSR22" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblPolicyYears" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtPolicyYears" Text="Policy Years:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadNumericTextBox ID="txtPolicyYears" runat="server" Culture="en-US"
                                                DbValueFactor="1" MaxValue="200"
                                                MinValue="0" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <asp:HiddenField ID="hdnFieldAccountId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                        <asp:HiddenField ID="hdnFieldEditForm" runat="server" />
                        <asp:Button ID="btnSaveOnForm" runat="server" OnClientClick="validateGroup('DriverControl');" ValidationGroup="DriverControl"
                            Text="Save" />
                        <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('DriverControl');" ValidationGroup="DriverControl"
                            Text="Save and Close" />
                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False"
                            Text="Close" class="returnButton" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>



