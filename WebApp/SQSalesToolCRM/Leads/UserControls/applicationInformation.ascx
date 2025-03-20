<%@ Control Language="C#" AutoEventWireup="true" CodeFile="applicationInformation.ascx.cs" Inherits="Leads_UserControls_applicationInformation" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>


<asp:HiddenField ID="hdnRecordId" runat="server" />

<asp:HiddenField ID="hdnHideGrid" runat="server" />
<asp:HiddenField ID="hdnIsOnActionWizard" runat="server" />
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
<asp:UpdatePanel runat="server" ID="allField">
    <ContentTemplate>
        <div id="divGrid" runat="server" class="condado">

            <asp:Button ID="btnAddnewApplicationTracking" runat="server" Text="New Application Tracking"
                CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();"
                class="resetChangeFlag" />
            <br />

            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" />
            <br />
            <br />
            <telerik:RadGrid ID="gridAppTrackingInfo" runat="server"
                AllowSorting="True" AutoGenerateColumns="False" CellSpacing="0"
                CssClass="mGrid" EnableTheming="True" GridLines="None" onfocus="this.blur();"
                Skin="" OnItemDataBound="gridAppTrackingInfo_ItemDataBound"
                Width="100%">
                <MasterTableView AllowNaturalSort="false">
                    <CommandItemSettings ExportToPdfText="Export to PDF" />
                    <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column"
                        Visible="true">
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column"
                        Visible="true">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="FullName"
                            FilterControlAltText="Filter column column" HeaderText="Delivered To"
                            SortExpression="FullName" UniqueName="uFullName">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="Key"
                            FilterControlAltText="Filter column column" HeaderText="Tracking ID"
                            SortExpression="Key" UniqueName="uKey">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="MedSubID"
                            FilterControlAltText="Filter column column" HeaderText="Policy"
                            SortExpression="MedSubID" UniqueName="uMedSubID">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="ApplSentDate"
                            FilterControlAltText="Filter column column" HeaderText="Application Send Date"
                            SortExpression="ApplSentDate" UniqueName="uApplSentDate">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="ActualAppSentDate"
                            FilterControlAltText="Filter column column" HeaderText="Actual Send Date and Time"
                            SortExpression="ActualAppSentDate" UniqueName="uActualAppSentDate">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn DataField="DeliveryMethod"
                            FilterControlAltText="Filter column column" HeaderText="Delivery Method"
                            SortExpression="DeliveryMethod" UniqueName="uDeliveryMethod">
                            <HeaderStyle HorizontalAlign="Left" />
                        </telerik:GridBoundColumn>

                        <telerik:GridTemplateColumn HeaderText="Options" UniqueName="colEdit">

                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false"
                                    CommandName="EditX" Text="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>
                                </asp:LinkButton>
                                <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                            Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;">
                        </asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="colView" Visible="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false"
                                    CommandName="ViewX" Text="View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <NoRecordsTemplate>
                        There is no Tracking Information
                    </NoRecordsTemplate>

                    <EditFormSettings>
                        <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                    </EditFormSettings>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                </MasterTableView>
                <HeaderStyle CssClass="gridHeader" />

                <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                <FilterMenu EnableImageSprites="False"></FilterMenu>
            </telerik:RadGrid>
        </div>
        <telerik:RadWindow ID="dlgApplicationInformation" runat="server" Width="900" Height="500"
             Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
            IconUrl="../Images/alert1.ico" Title="Application Tracking">
            <ContentTemplate>
                <asp:UpdatePanel ID="Updatepanel10" runat="server">
                    <ContentTemplate>
                        <fieldset style="margin: 10px">
                            <uc4:StatusLabel ID="ctlStatus" runat="server" />
                            <div id="divForm" runat="server" class="condado">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button Visible="false" runat="server" Text="Return to Application Tracking" ID="btnReturn" CausesValidation="False" class="returnButton" />
                                </div>
                                <table width="100%" runat="server" id="tblControls">
                                    <tr>
                                        <td>

                                            <asp:Label ID="lblToPerson" runat="server" Font-Bold="True" Text="To Person:"></asp:Label>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="200px" align="left">
                                            <asp:Label ID="lblIndividualassociatedWith" runat="server" CssClass="uclabel"
                                                Text="Person Sent To:" />
                                        </td>
                                        <td width="200px">
                                            <asp:DropDownList ID="ddFullName" runat="server" Width="99%" ValidationGroup="AppControl" DataTextField="FullName" DataValueField="Id">
                                            </asp:DropDownList>

                                        </td>
                                        <td></td>
                                        <td width="200px">

                                            <asp:Label ID="lblPolicyID" runat="server" CssClass="uclabel" Text="Policy"></asp:Label>

                                        </td>
                                        <td width="200px">

                                            <asp:DropDownList ID="ddPolicies" runat="server" Width="99%"
                                                DataTextField="Policy" DataValueField="key">
                                            </asp:DropDownList>
                                            <%--//TM [14 06 2014] Make policy optional--%>
                                            <%--<asp:RequiredFieldValidator ID="vldPolicy" runat="server" InitialValue=""
                                                Display="None" ErrorMessage="Policies" ControlToValidate="ddPolicies"></asp:RequiredFieldValidator>--%>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td width="300px">
                                            <asp:Label ID="lblRequestedApplicationSentDate" runat="server" CssClass="uclabel"
                                                Text="Requested Send Date:" />
                                        </td>
                                        <td width="200px">
                                            <telerik:RadDatePicker ID="rdpRequestedApplicationSentDate" runat="server"
                                                Width="180px">
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td></td>
                                        <td width="300px">
                                            <asp:Label ID="lblApplicationSenttoCustomerMethod" runat="server" CssClass="uclabel"
                                                Text="Application Sent to Customer Method:" />
                                        </td>
                                        <td width="200px">
                                            <asp:DropDownList ID="ddApplicationSenttoCustomerMethod" runat="server"
                                                Width="99%">
                                                <asp:ListItem Value="">-- None --</asp:ListItem>
                                                <asp:ListItem>Online</asp:ListItem>
                                                <asp:ListItem>Fax</asp:ListItem>
                                                <asp:ListItem>Email</asp:ListItem>
                                                <asp:ListItem>Next Day</asp:ListItem>
                                                <asp:ListItem>Two Day</asp:ListItem>
                                                <asp:ListItem>Regular Mail</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td width="300px">
                                            <asp:Label ID="lblExpectedReturnApplicationMethod" runat="server" CssClass="uclabel"
                                                Text="Expected Return Application Method:" />
                                        </td>
                                        <td width="200px">
                                            <asp:DropDownList ID="ddExpectedReturnApplicationMethod" runat="server" Width="99%">
                                                <asp:ListItem Value="">-- None --</asp:ListItem>
                                                <asp:ListItem>Online</asp:ListItem>
                                                <asp:ListItem>Fax</asp:ListItem>
                                                <asp:ListItem>Email</asp:ListItem>
                                                <asp:ListItem>Next Day</asp:ListItem>
                                                <asp:ListItem>Two Day</asp:ListItem>
                                                <asp:ListItem>Regular Mail</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td width="300px">
                                            <asp:Label ID="lblActualApplicationSentDate" runat="server" A="" CssClass="uclabel"
                                                Text="Actual Send Date and Time:" />
                                        </td>
                                        <td width="200px">
                                            <%--<telerik:RadDatePicker ID="rdpActualApplicationSentDate" Runat="server" 
                                       Width="180px">
                </telerik:RadDatePicker>--%>
                                            <telerik:RadDateTimePicker ID="rdpActualApplicationSentDate" runat="server" Width="180px" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td width="300px">
                                            <asp:Label ID="lblFillFormCaseSpecialist" runat="server" CssClass="uclabel"
                                                Text="Fill Form Case Specialist:"></asp:Label>
                                        </td>
                                        <td width="200px">
                                            <asp:DropDownList ID="ddFillFormCaseSpecialist" runat="server" Width="99%"
                                                DataTextField="FullName" DataValueField="key">
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td width="300px">

                                            <asp:Label ID="lblReturnLabelNumber" runat="server" CssClass="uclabel"
                                                Text="Return Label Number:" />
                                        </td>
                                        <td width="200px">
                                            <asp:TextBox ID="tbReturnLabelNumber" runat="server" Width="99%"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td width="300px">
                                            <asp:Label ID="lblFillFormStatusNote" runat="server" CssClass="uclabel"
                                                Text="Fill Form Status Note:"></asp:Label>
                                        </td>
                                        <td colspan="4">
                                            <asp:TextBox ID="tbfillFormStatusNote" runat="server" TextMode="MultiLine"
                                                Width="99%"></asp:TextBox>
                                        </td>
                                        <td></td>

                                    </tr>
                                    <tr>
                                        <td width="300px">&nbsp;
                                        </td>
                                        <td width="200px">&nbsp;
                                        </td>
                                        <td></td>
                                        <td width="300px">&nbsp;
                                        </td>
                                        <td width="200px">&nbsp;
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblToCarrier" runat="server" Font-Bold="True" Text="To Carrier:"></asp:Label>
                                        </td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSubmittoCarrierCaseSpecialist" runat="server" Text="Submit to Carrier Case Specialist:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddSubmittoCarrierCaseSpecialist" runat="server"
                                                Width="99%" DataTextField="FullName" DataValueField="key">
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblSubmitToCarrierStatusNote" runat="server" Text="Status Note:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbSubmitToCarrierStatusNote" runat="server" Width="99%"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPolicySubmitToCarrierDate" runat="server" CssClass="uclabel"
                                                Text="Policy Submit To Date:"></asp:Label>
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="rdpPolicySubmitToCarrierDate" runat="server"
                                                Width="99%">
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblSentToCarrierMethod" runat="server" CssClass="uclabel"
                                                Text="Application Delivery Method:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddSentToCarrierMethod" runat="server" Width="99%">
                                                <asp:ListItem Value="">-- None --</asp:ListItem>
                                                <asp:ListItem>Online</asp:ListItem>
                                                <asp:ListItem>Fax</asp:ListItem>
                                                <asp:ListItem>Email</asp:ListItem>
                                                <asp:ListItem>Next Day</asp:ListItem>
                                                <asp:ListItem>Two Day</asp:ListItem>
                                                <asp:ListItem>Regular Mail</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSentToCarrierTrackingNumber" runat="server" CssClass="uclabel" Text="Sent To Carrier Tracking Number:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tbSentToCarrierTrackingNumber" runat="server" Width="99%"></asp:TextBox>
                                        </td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNotes" runat="server" Font-Bold="True" Text="Notes:"></asp:Label>
                                        </td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSalesAgentNotes" runat="server" CssClass="uclabel"
                                                Text="Sales Agent Notes:" />
                                        </td>
                                        <td colspan="4">
                                            <asp:TextBox ID="tbSalesAgentNotes" runat="server" Width="99%" Rows="4"
                                                TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCaseSpecialistNotes" runat="server" CssClass="uclabel"
                                                A Text="Case Specialist Notes:" />
                                        </td>
                                        <td colspan="4">
                                            <asp:TextBox ID="tbCaseSpecialistNotes" runat="server" Width="99%" Rows="4"
                                                TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCaseSpecialistNotes2" runat="server" CssClass="uclabel"
                                                Text="Case Specialist Notes 2:" />
                                        </td>
                                        <td colspan="4">
                                            <asp:TextBox ID="tbCaseSpecialistNotes2" runat="server" Width="99%" Rows="4"
                                                TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCaseSpecialistNotes3" runat="server" CssClass="uclabel"
                                                Text="Case Specialist Notes 3:" />
                                        </td>
                                        <td colspan="4">
                                            <asp:TextBox ID="tbCaseSpecialistNotes3" runat="server" Width="99%" Rows="4"
                                                TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                            </div>
                        </fieldset>
                        <div class="buttons" style="text-align: right">
                            <asp:Button ID="btnSaveOnForm" runat="server" OnClientClick="validateGroup('AppControl');" ValidationGroup="AppControl" Text="Save" />
                            <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('AppControl');" Text="Save and Close"  ValidationGroup="AppControl" />
                            <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" class="returnButton" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSaveOnForm" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveAndCloseOnForm" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelOnForm" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
</asp:UpdatePanel>
