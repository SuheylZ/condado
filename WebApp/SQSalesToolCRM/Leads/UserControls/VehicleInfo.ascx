<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VehicleInfo.ascx.cs" Inherits="UserControls_VehicleInfo" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>

<asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
<asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
<asp:HiddenField ID="hdnFieldAccountID" runat="server" />
<asp:HiddenField ID="hdnFieldVehicleId" runat="server" />

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

<asp:Panel ID="pnlGrid" runat="server" Visible="true">
    <div class="condado">

        <div id="divToolbar" class="Toolbar">
            <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Vehicle" CausesValidation="False" OnClientClick="setChangeFlag('0');"
                OnClick="btnAddNewQueue_Click" />
            &nbsp; &nbsp; &nbsp;
            
        </div>
        <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" OnSizeChanged="PagingBar_Event" OnIndexChanged="PagingBar_Event" />
        <br />
        <br />
        <telerik:RadGrid ID="grdVehicles" runat="server" Skin="" CssClass="mGrid" Width="100%"
            CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
            AutoGenerateColumns="False" OnItemCommand="grdVehicles_ItemCommand" OnItemDataBound="grdVehicles_ItemDataBound">
            <MasterTableView DataKeyNames="key">
                <NoRecordsTemplate>
                    There is no vehicle record.
                </NoRecordsTemplate>
                <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                    ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                    ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                    ExportToPdfText="Export to PDF" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                <Columns>
                    <telerik:GridBoundColumn DataField="FullName" FilterControlAltText="Individual Name" HeaderText="Name"
                        UniqueName="FullName">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="15%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Key" FilterControlAltText="Vehicle ID" HeaderText="VehicleID"
                        UniqueName="Key">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Make" FilterControlAltText="Make" HeaderText="Make"
                        UniqueName="Make">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="5%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Model" FilterControlAltText="Model" HeaderText="Model"
                        UniqueName="Model">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="5%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Year" FilterControlAltText="Year" HeaderText="Year"
                        UniqueName="Year">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="AnnualMileage" FilterControlAltText="Annual Mileage"
                        HeaderText="Annual Mileage" UniqueName="AnnualMileage">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ComprehensiveDeductable" FilterControlAltText="Comprehensive Deductible"
                        HeaderText="Comprehensive Deductible" UniqueName="ComprehensiveDeductable">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CollisionDeductable" FilterControlAltText="Collision Deductible"
                        HeaderText="Collision Deductible" UniqueName="CollisionDeductable">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn FilterControlAltText="Vehicle record enabled" UniqueName="Enabled"
                        HeaderText="Enabled">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive"))? "Yes": "No" %>'></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colEdit"
                        HeaderText="" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' Text="Edit"></asp:LinkButton>|
                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="10%" />
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="colView" Visible="false"
                        HeaderText="" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' Text="View" />
                        </ItemTemplate>
                        <ItemStyle Width="10%" />
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <HeaderStyle CssClass="gridHeader" />
            <FilterMenu EnableImageSprites="False">
            </FilterMenu>
        </telerik:RadGrid>
    </div>
</asp:Panel>
<telerik:RadWindow ID="dlgVehicleInfo" runat="server" Width="900" Height="370"
     Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
    IconUrl="../Images/alert1.ico" Title="Add New Vehicle Information">
    <ContentTemplate>
        <asp:UpdatePanel ID="Updatepanel10" runat="server">
            <ContentTemplate>
                <fieldset style="margin: 10px">
                    <asp:Panel ID="pnlDetail" runat="server" Visible="false">
                        <uc3:StatusLabel ID="ctlStatus" runat="server" />
                        <div class="buttons" style="text-align: right">
                            <asp:Button runat="server" Text="Return to Vehicles" ID="btnReturn" Visible="false" OnClick="btnCancelOnForm_Click"
                                CausesValidation="False" class="returnButton" />
                        </div>
                        <div id="fldSetForm" class="condado">

                            <table runat="server" id="tblControls">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblIndividual" runat="server" Text="Individual" />
                                    </td>
                                    <td colspan="3">
                                        <asp:DropDownList ID="ddlIndividuals" runat="server" DataTextField="FullName" 
                                            DataValueField="Id" />
                                        <asp:RequiredFieldValidator ID="rfvddlIndividuals" runat="server" ControlToValidate="ddlIndividuals" InitialValue="-1" Display="None" ErrorMessage="Individual is required."></asp:RequiredFieldValidator>
                                        <%--SR <ajaxToolkit:ValidatorCalloutExtender ID="vceExtddlIndividual" TargetControlID="rfvddlIndividuals" Enabled="True" runat="server" Width="250px" />--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblYear" runat="server" Text="Year" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtYear" runat="server" Width="100px" MinValue="0"
                                            MaxValue="999999999" ValidationGroup="VehicleControl">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                        <asp:RequiredFieldValidator ID="vldYear" runat="server"
                                            Display="None" ErrorMessage="Year is required" ControlToValidate="txtYear"></asp:RequiredFieldValidator>
                                        <%--<asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                Enabled="True" TargetControlID="vldYear" Width="250px"/>--%>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblMake" runat="server" Text="Make" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMake" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblModel" runat="server" Text="Model" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtModel" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblSubModel" runat="server" Text="Submodel" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSubModel" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblAnnualMileage" runat="server" Text="Annual Mileage" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtAnnualMileage" runat="server" Width="100px" MinValue="0"
                                            MaxValue="999999999">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTitle3" runat="server" Text="Weekly Commute Days" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtWeeklyCommuteDays" runat="server" Width="100px"
                                            MinValue="0" MaxValue="999999999">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPrimaryUse" runat="server" Text="Primary Use" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrimaryUse" runat="server" />
                                    </td>
                                    <td>&nbsp;
                                    </td>
                                    <td>&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblComprehensiveDedeuctible" runat="server" Text="Comprehensive Deductible" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtComprehensiveDeductible" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblCollisionDeductible" runat="server" Text="Collision Deductible" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCollisionDeductible" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblSecuritySystem" runat="server" Text="Security System" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSecuritySystem" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblParkingLocation" runat="server" Text="Parking Location" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtParkingLocation" runat="server" />
                                    </td>
                                </tr>

                            </table>
                        </div>
                    </asp:Panel>
                </fieldset>
                <div class="buttons" style="text-align: right">
                    <asp:Button ID="btnApply" runat="server" Text="Save" OnClick="btnApply_Click" OnClientClick="validateGroup('VehicleControl');" ValidationGroup="VehicleControl" />
                    <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validateGroup('VehicleControl');" ValidationGroup="VehicleControl" Text="Save & Close" />
                    <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click" Text="Close" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
