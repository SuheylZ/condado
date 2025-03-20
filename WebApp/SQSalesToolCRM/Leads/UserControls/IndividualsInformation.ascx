<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IndividualsInformation.ascx.cs"
    Inherits="Leads_UserControls_IndividualsInformation" %>
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
<div class="condado">
    <uc2:StatusLabel ID="ctlDeleteMessage" runat="server" />
    <asp:HiddenField ID="hdnCurrentOutPulseID" Value="" runat="server" />
    <br />
    <div id="mainDv">
        <div runat="server" id="divGrid" visible="true" class="Toolbar">
            <div id="fldSetGrid" class="condado">
                <asp:Button ID="btnAddNewIndividual" runat="server" Text="Add New Individual" CausesValidation="False"
                    OnClientClick="setChangeFlag('0');return OpenConfirmationBox();" class="resetChangeFlag" />
                <br />

                <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
                <br />
                <br />
                <asp:GridView ID="grdIndividual" runat="server" Width="100%" DataKeyNames="Id" AutoGenerateColumns="False"
                    AllowSorting="True" GridLines="None" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt"
                    OnRowDataBound="Evt_RowDataBound"
                    HeaderStyle-CssClass="gridHeader">
                    <Columns>
                        <asp:BoundField HeaderText="Account ID" SortExpression="AccountId" DataField="AccountId"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="First Name" SortExpression="FirstName" DataField="FirstName"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Last Name" SortExpression="LastName" DataField="LastName"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Date of Birth" SortExpression="Birthdate" DataField="Birthdate"
                            DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-HorizontalAlign="Left" />
                        <asp:TemplateField HeaderText="Day Phone">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDayPhoneGrid" runat="server" CausesValidation="false" OnClick="ClickedToDial1"
                                    Text='<%# Eval("DayPhone","{0:(###) ###-####}") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Evening Phone">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEveningPhoneGrid" runat="server" CausesValidation="false"
                                    OnClick="ClickedToDial1" Text='<%# Eval("EveningPhone","{0:(###) ###-####}") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cell Phone">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCellPhoneGrid" runat="server" CausesValidation="false" OnClick="ClickedToDial1"
                                    Text='<%# Eval("CellPhone","{0:(###) ###-####}") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Status Indv." SortExpression="StatusIndv" DataField="StatusIndv"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Status" SortExpression="Status" DataField="Status"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Sub Status" SortExpression="SubStatus" DataField="SubStatus"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Next Event Date" SortExpression="NextCalendarEventDate" DataField="NextCalendarEventDate" DataFormatString="{0:MM/dd/yyyy}"
                            HeaderStyle-HorizontalAlign="Left" />
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkQuote" runat="server" CausesValidation="False" CommandName="QuoteX"
                                    Text="Quote" Visible="false"></asp:LinkButton>
                                <asp:Button ID="btnQuoteGrid" runat="server" CausesValidation="false" CommandName="QuoteX"
                                    Text="Quote" Width="75px" CssClass="buttonstyle" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFillForm" runat="server" CausesValidation="False" CommandName="FillFormX"
                                    Text="Fill Form"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                    Text="Edit" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                    Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false" ItemStyle-HorizontalAlign="Right" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        There are no Individuals to display.
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
        <telerik:RadWindow ID="dlgNewIndividual" runat="server" Width="900" Height="730"
            Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
            IconUrl="../Images/alert1.ico" Title="Add New Individual">
            <ContentTemplate>
                <asp:UpdatePanel ID="Updatepanel10" runat="server">
                    <ContentTemplate>
                        <fieldset style="margin: 10px">
                            <div runat="server" id="divForm">
                                <div id="fldSetForm" class="condado">
                                    <asp:HiddenField ID="hdnRecordId" runat="server" />
                                    <uc2:StatusLabel ID="ctlMessage" runat="server" />
                                    <div class="buttons" style="text-align: left;">

                                        <asp:Button runat="server" Text="Return to Individuals" ID="btnReturn" CausesValidation="False" Visible="false"
                                            class="returnButton" />
                                    </div>
                                    <table runat="server" id="tblControls">
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblStatuses" runat="server" AssociatedControlID="ddlIndividualStatuses"
                                                    Text="Individual Status:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlIndividualStatuses" runat="server" DataTextField="Title"
                                                    DataValueField="ID" Width="225px" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlIndividualPDPStatuses"
                                                    Text="Individual PDP Status:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlIndividualPDPStatuses" runat="server" DataTextField="Title"
                                                    DataValueField="ID" Width="225px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblFirstname" runat="server" AssociatedControlID="txtFirstName" Text="First Name:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtFirstName" runat="server" Width="200px" ValidationGroup="IndvControl"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="vldFirstName" runat="server" ControlToValidate="txtFirstName" Enabled="false"
                                                    Display="None" ErrorMessage="First name is required" ValidationGroup="IndvControl" />
                                                <ajaxToolkit:ValidatorCalloutExtender ID="vldc1" runat="server" TargetControlID="vldFirstName" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label runat="server" ID="lblMiddle" Text="Middle Name" />
                                                <br />
                                                <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Text="Last Name:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtMiddle" runat="server" Width="200px" MaxLength="1"></asp:TextBox><br />
                                                <asp:TextBox ID="txtLastName" runat="server" Width="200px" ValidationGroup="IndvControl"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="vldLastName" runat="server" ControlToValidate="txtLastName" Enabled="false"
                                                    Display="None" ErrorMessage="Last name is required" ValidationGroup="IndvControl" />
                                                <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server"
                                                    TargetControlID="vldLastName" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblDOB" runat="server" AssociatedControlID="radDOB" Text="Date of Birth:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="radDOB" runat="server" SkipMinMaxDateValidationOnServer="true" ValidationGroup="IndvControl" />
                                                <asp:RequiredFieldValidator ID="vldReqradDOB" runat="server" ControlToValidate="radDOB" Enabled="false"
                                                    Display="None" ErrorMessage="Date of Birth is required" ValidationGroup="IndvControl" />
                                                <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" runat="server"
                                                    TargetControlID="vldReqradDOB" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblZipCode" runat="server" AssociatedControlID="txtZipCode" Text="Zip Code:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtZipCode" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblGender" runat="server" AssociatedControlID="ddlGender" Text="Gender:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlGender" runat="server" Width="100px" ValidationGroup="IndvControl">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>Male</asp:ListItem>
                                                    <asp:ListItem>Female</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="vldReqGender" Display="None" runat="server" Enabled="false"
                                                    ValidationGroup="IndvControl" ErrorMessage="Please select gender." ControlToValidate="ddlGender"
                                                    InitialValue=""> </asp:RequiredFieldValidator>
                                                <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server"
                                                    TargetControlID="vldReqGender" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblTobacco" runat="server" AssociatedControlID="ddlTobacco" Text="Tobacco:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlTobacco" runat="server" Width="100px">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>No</asp:ListItem>
                                                    <asp:ListItem>Yes</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblDayPhone" runat="server" AssociatedControlID="txtDayPhone" Text="Day Phone:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadMaskedTextBox ID="txtDayPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                                <a id="lnkDayPhone" runat="server">Dial</a>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEveningPhone" runat="server" AssociatedControlID="txtEveningPhone"
                                                    Text="Evening Phone:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadMaskedTextBox ID="txtEveningPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                                <a id="lnkEveningPhone" runat="server">Dial</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCellPhone" runat="server" AssociatedControlID="txtCellPhone" Text="Cell Phone:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadMaskedTextBox ID="txtCellPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                                <a id="lnkCellPhone" runat="server">Dial</a>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblFax" runat="server" AssociatedControlID="txtFax" Text="Fax:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadMaskedTextBox ID="txtFax" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                                <a id="lnkFax" runat="server">Dial</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblAddress1" runat="server" AssociatedControlID="txtAddress1" Text="Address 1:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtAddress1" runat="server" Width="200px" ValidationGroup="IndvControl"></asp:TextBox>

                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblAddress2" runat="server" AssociatedControlID="txtAddress2" Text="Address 2:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtAddress2" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtCity" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblStateIndividual" runat="server" AssociatedControlID="ddlStateIndividual"
                                                    Text="State:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlStateIndividual" runat="server" Width="225px" DataTextField="FullName"
                                                    DataValueField="Id" ValidationGroup="IndvControl" />

                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtEmail" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblExternalReferenceId" runat="server" AssociatedControlID="txtExternalReferenceId"
                                                    Text="External Reference ID:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtExternalReferenceId" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEmailOptOut" runat="server" AssociatedControlID="chkEmailOptOut" Text="Email Opt Out:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chkEmailOptOut" Text="" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblApplicationState" runat="server" AssociatedControlID="ddlAppState"
                                                    Text="Application State:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlAppState" runat="server" Width="225px" DataTextField="FullName"
                                                    DataValueField="Id" ValidationGroup="IndvControl" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblHRASubsidyAmount" runat="server" AssociatedControlID="txtHRASubsidyAmount" Text="HRA Subsidy Amount:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtHRASubsidyAmount" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                            <td>

                                                <asp:Label ID="lblConsent" runat="server" Text="Consent" /></td>
                                            <td>
                                                <asp:DropDownList ID="ddlConsent" runat="server" Enabled="false" Width="225px">
                                                    <asp:ListItem Value="0" Text="" />
                                                    <asp:ListItem Value="1" Text="Yes" />
                                                    <asp:ListItem Value="2" Text="No" />
                                                    <asp:ListItem Value="3" Text="Not Applicable" />
                                                </asp:DropDownList>
                                            </td>

                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblNotes" runat="server" AssociatedControlID="txtNotes" Text="Notes:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblDOB2" runat="server" AssociatedControlID="radDOB2" Text="AP Date:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="radDOB2" runat="server" SkipMinMaxDateValidationOnServer="true" />
                                            </td>

                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_indv_ob_dental" runat="server" AssociatedControlID="chk_indv_ob_dental" Text="Dental" CssClass="uclabel"></asp:Label>
                                                <asp:Label ID="lbl_indv_ob_billing" runat="server" AssociatedControlID="chk_indv_ob_billing" Text="Billing" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_dental" />
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_billing" />
                                            </td>

                                        </tr>
                                        <tr runat="server" id="chk_row_annuty_autohomelife">
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_chk_indv_ob_annuity" runat="server" AssociatedControlID="chk_indv_ob_annuity" Text="Annuity" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_annuity" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_chk_indv_ob_auto_home_life" runat="server" AssociatedControlID="chk_indv_ob_auto_home_life" Text="Auto/Home and Life" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_auto_home_life" />
                                            </td> 
                                            
                                        </tr>
                                        <tr runat="server" id="chk_row_onboard_sql">
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label3" runat="server" AssociatedControlID="chk_indv_ob_cs_prep" Text="CS Prep" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_cs_prep" />
                                            </td>

                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_chk_indv_ob_auto_home" runat="server" AssociatedControlID="chk_indv_ob_auto_home" Text="Auto and Home" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_auto_home" />
                                            </td>
                                        </tr>
                                        <tr runat="server" id="chk_row_cs_prep_autohome">
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_chk_indv_ob_cs_prep" runat="server" AssociatedControlID="chk_indv_ob_app_eSign" Text="Application/eSign" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_app_eSign" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lbl_chk_indv_ob_inspection" runat="server" AssociatedControlID="chk_indv_ob_inspection" Text="Inspection" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox runat="server" ID="chk_indv_ob_inspection" />
                                            </td>
                                        </tr>
                                        <tr runat="server" id="checkBoxesPrimarySecondary">
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPrimary" runat="server" AssociatedControlID="cbxPrimary" Text="Primary Account:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="cbxPrimary" runat="server" AutoPostBack="True" OnCheckedChanged="Evt_Primary_Check_Changed" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblSecondary" runat="server" AssociatedControlID="cbxSecondary" CssClass="uclabel" Text="Secondary Account:"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="cbxSecondary" runat="server" AutoPostBack="True" OnCheckedChanged="Evt_Secondary_Check_Changed" />
                                            </td>
                                        </tr>
                                        <%-- Life insurance --%>
                                        <%-- <tr runat="server" ID="tblRowExistingInsurance">
                         <td nowrap="nowrap">
                            <asp:Label ID="lblHasExistingIssurance" runat="server" AssociatedControlID="chkHasExistingIssurance" Text="Existing Insurance" CssClass="uclabel"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkHasExistingIssurance" runat="server" AutoPostBack="True" OnCheckedChanged="Evt_Primary_Check_Changed" />
                        </td>
                       <td><asp:Label ID="lblExistinginsurance" runat="server" Text="Existing Insurance Amount"></asp:Label></td> 
                        <td>
                        <asp:TextBox ID="txtExistingInsuranceAmount" runat="server" Width="200px" ></asp:TextBox>
                    </td>
                    </tr>
                    <tr runat="server" ID="tblRowExistingInsurance1">
                        <td>
                            <asp:Label runat="server" Text="Replacement"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkExistingInsuranceReplacement"/>
                        </td>
                    </tr>
                    
                    <tr>
                         <td><asp:Label ID="lblDesiredInsuranceAmount" runat="server" Text="Desired Insurance Amount"></asp:Label></td> 
                        <td>
                        <asp:TextBox ID="TxtDesiredInsuranceAmount" runat="server" Width="200px" ></asp:TextBox>
                    </td>
                           <td><asp:Label ID="lblAlternateAmount" runat="server" Text="Alternate Amount"></asp:Label></td> 
                        <td>
                        <asp:TextBox ID="txtDesirAltAmount" runat="server" Width="200px" ></asp:TextBox>
                    </td>
                    </tr>
                    <tr>
                        <td>
                          <asp:Label ID="lblTermDuration" runat="server" Text="Term Duration"></asp:Label> 
                        </td>
                        <td>
                            <asp:DropDownList runat="server" Width="200" ID="ddlDesireInsuranceTerms">
                                 <asp:ListItem Value="10" Text="10" />
                                 <asp:ListItem Value="15" Text="15" />
                                 <asp:ListItem Value="20" Text="20" />
                                 <asp:ListItem Value="30" Text="30" />
                            </asp:DropDownList>
                        </td>
                    </tr>--%>
                                        <tr runat="server" id="tblRowArcLifeInsurance">
                                            <td colspan="2">
                                                <asp:Panel runat="server" ID="pnlExistingInsurance" Enabled="false">


                                                    <fieldset style="height: 110px; margin-right: 1.5px">
                                                        <legend>Existing Insurance
                                                        </legend>
                                                        <table>
                                                            <tr>
                                                                <td nowrap="nowrap">
                                                                    <asp:Label ID="lblHasExistingIssurance" runat="server" AssociatedControlID="chkHasExistingIssurance" Text="Has Insurance" CssClass="uclabel"></asp:Label>
                                                                </td>
                                                                <td nowrap="nowrap">
                                                                    <asp:CheckBox ID="chkHasExistingIssurance" runat="server" AutoPostBack="True" OnCheckedChanged="Evt_Primary_Check_Changed" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblExistinginsurance" runat="server" Text="Insurance Amount"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtExistingInsuranceAmount" runat="server" Width="200px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="vExistingInsuranceAmount" Display="None" ControlToValidate="txtExistingInsuranceAmount" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+\.?\d*"></asp:RegularExpressionValidator>
                                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" Enabled="True"
                                                                        TargetControlID="vExistingInsuranceAmount" Width="250px" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label2" runat="server" Text="Replacement"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox runat="server" ID="chkExistingInsuranceReplacement" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>

                                                </asp:Panel>
                                            </td>
                                            <td colspan="2">
                                                <asp:Panel runat="server" ID="pnlDesireInsurance" Enabled="false">


                                                    <fieldset style="height: 110px">
                                                        <legend>Desired Insurance
                                                        </legend>
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblDesiredInsuranceAmount" runat="server" Text="Desired Insurance Amount"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="TxtDesiredInsuranceAmount" runat="server" Width="200px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="vDesiredInsuranceAmount" Display="None" ControlToValidate="TxtDesiredInsuranceAmount" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+\.?\d*"></asp:RegularExpressionValidator>
                                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" Enabled="True"
                                                                        TargetControlID="vDesiredInsuranceAmount" Width="250px" />
                                                                </td>

                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblAlternateAmount" runat="server" Text="Alternate Amount"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtDesirAltAmount" runat="server" Width="200px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="vtxtDesirAltAmount" Display="None" ControlToValidate="txtDesirAltAmount" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+\.?\d*"></asp:RegularExpressionValidator>
                                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server" Enabled="True"
                                                                        TargetControlID="vtxtDesirAltAmount" Width="250px" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblTermDuration" runat="server" Text="Term Duration"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList runat="server" Width="200" ID="ddlDesireInsuranceTerms">
                                                                        <asp:ListItem Value="-1" Text="Select One..." />
                                                                        <asp:ListItem Value="10" Text="10" />
                                                                        <asp:ListItem Value="15" Text="15" />
                                                                        <asp:ListItem Value="20" Text="20" />
                                                                        <asp:ListItem Value="30" Text="30" />
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>

                                                </asp:Panel>
                                            </td>
                                        </tr>

                                    </table>
                                </div>
                            </div>
                        </fieldset>
                        <div class="buttons" style="text-align: right">
                            <asp:Button ID="btnSaveOnForm" runat="server" CausesValidation="true" OnClientClick="validateGroup('IndvControl');"
                                ValidationGroup="IndvControl" Text="Save" />
                            <asp:Button ID="btnSaveAndCloseOnForm" runat="server" CausesValidation="true" OnClientClick="validateGroup('IndvControl');"
                                ValidationGroup="IndvControl" Text="Save and Close" />
                            <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close"
                                class="returnButton" />
                            <asp:Button ID="btnQuote" runat="server" CausesValidation="False" Text="Quote" Visible="false" />
                            <asp:Button ID="btnFillForm" runat="server" CausesValidation="False" Text="Fill Form" Visible="false" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </div>
</div>
