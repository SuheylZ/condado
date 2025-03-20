<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HomeInformation.ascx.cs" Inherits="Leads_UserControls_HomeInformation" %>

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

            <asp:Button ID="btnAddNew" runat="server" Text="Add New Home" CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();" class="resetChangeFlag" />
            <br />
            <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
            <br />
            <br />
            <asp:GridView ID="grdHome" runat="server" Width="100%" DataKeyNames="Id"
                ShowHeaderWhenEmpty="true"
                AutoGenerateColumns="false" AllowSorting="True" GridLines="None" PageSize="10"
                AlternatingRowStyle-CssClass="alt" HeaderStyle-CssClass="gridHeader" OnRowDataBound="grdHome_RowDataBound">
                <Columns>
                    <asp:BoundField HeaderText="Home ID" SortExpression="HomeID" DataField="Id" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Name" SortExpression="FullName" DataField="FullName" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Year Built" SortExpression="YearBuilt" DataField="YearBuilt" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Dwelling Type" SortExpression="DwellingType" DataField="DwellingType" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Design Type" SortExpression="DesignType" DataField="DesignType" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Bedrooms" SortExpression="NumberOfBedrooms" DataField="NumberOfBedrooms" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Bathrooms" SortExpression="NumberOfBathrooms" DataField="NumberOfBathrooms" HeaderStyle-HorizontalAlign="Left" />

                    <asp:BoundField HeaderText="Square Footage" SortExpression="SquareFootage" DataField="SquareFootage" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField HeaderText="Carrier" SortExpression="Carrier" DataField="Carrier" HeaderStyle-HorizontalAlign="Left" />

                    <asp:TemplateField ShowHeader="false" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX" Text="Edit" />
                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX" Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Right" Visible="false">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    There are no Home Insurances to display.
                </EmptyDataTemplate>
                <PagerSettings Position="Top" />
                <PagerStyle VerticalAlign="Bottom" />
            </asp:GridView>
        </div>
    </div>
    <telerik:RadWindow ID="dlgNewHome" runat="server" Width="900" Height="550"
         Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
        IconUrl="../Images/alert1.ico" Title="Add New Home">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel10" runat="server">
                <ContentTemplate>
                    <fieldset style="margin: 10px">
                        <div runat="server" id="divForm" visible="true">
                            <uc2:StatusLabel ID="ctlStatus" runat="server" />
                            <div id="fldSetForm" class="condado">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button Visible="false" runat="server"   Text="Return to Homes List" ID="btnReturn" CausesValidation="False" class="returnButton"></asp:Button>
                                </div>
                                <table runat="server" id="tblControls">
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblIndividual" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlIndividual" Text="Individual:" />
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlIndividual" runat="server" DataTextField="FullName" DataValueField="Id" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap" class="uclabel">Address 1:
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtHomeAddress1" runat="server"></asp:TextBox>

                                        </td>

                                        <td nowrap="nowrap" class="uclabel">Address 2:
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtHomeAddress2" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap" class="uclabel">City:
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtHomeCity" runat="server"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap" class="uclabel">State:
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:DropDownList ID="ddlHomeStates" runat="server" DataTextField="FullName"
                                                DataValueField="Id" Width="129px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap" class="uclabel">Zip Code:
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtHomeZip" runat="server"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>

                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                        <td nowrap="nowrap">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblYear" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtYear" Text="Year Built:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <telerik:RadMaskedTextBox ID="txtYear" runat="server"
                                                Mask="####" Width="130px"  />
                                            <asp:RequiredFieldValidator ID="vldYear" runat="server"
                                                Display="None" ErrorMessage="Year is required" ControlToValidate="txtYear"></asp:RequiredFieldValidator>
                                           <%-- SR [3.26.2014] <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                Enabled="True" TargetControlID="vldYear" Width="250px" />--%>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblSquareFootage" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtSquareFootage" Text="Square Footage:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtSquareFootage" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblDwellingType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtDwellingType" Text="Dwelling Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDwellingType" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblDesignType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtDesignType" Text="Design Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDesignType" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblRoofAge" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtRoofAge" Text="Roof Age:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtRoofAge" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblRoofType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtRoofType" Text="Roof Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtRoofType" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblFoundationType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtFoundationType" Text="Foundation Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtFoundationType" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblHeatingType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtHeatingType" Text="Heating Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtHeatingType" runat="server"></asp:TextBox>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblExteriorWallType" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtExteriorWallType" Text="Exterior Wall Type:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtExteriorWallType" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblNumberOfHomeClaims" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtNumberOfHomeClaims" Text="Number Of Home Claims:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtNumberOfHomeClaims" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblNumberOfBedrooms" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtNumberOfBedrooms" Text="Number Of Bedrooms:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtNumberOfBedrooms" runat="server"></asp:TextBox>
                                        </td>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblNumberOfBathrooms" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtNumberOfBathrooms" Text="Number Of Bathrooms:" />
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtNumberOfBathrooms" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblRequestedCoverage" runat="server" CssClass="uclabel"
                                                AssociatedControlID="txtRequestedCoverage" Text="Requested Coverage:" />
                                        </td>
                                        <td colspan="3">
                                            <asp:TextBox ID="txtRequestedCoverage" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">&nbsp;</td>
                                    </tr>
                                    <tr>

                                        <td nowrap="nowrap">
                                            <asp:Label ID="lblCurrentCarrier" runat="server" CssClass="uclabel"
                                                AssociatedControlID="ddlCurrentCarrier" Text="Current Carrier:" />
                                        </td>
                                        <td nowrap="nowrap" colspan="3">
                                            <asp:DropDownList ID="ddlCurrentCarrier" runat="server" DataTextField="Name"
                                                DataValueField="Key">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <asp:HiddenField ID="hdnRecordId" runat="server" Value="0" />
                        <asp:Button ID="btnSaveOnForm" runat="server" OnClientClick="validateGroup('HomeControl');" ValidationGroup="HomeControl" Text="Save" />
                        <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('HomeControl');" ValidationGroup="HomeControl" Text="Save and Close" />
                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" class="returnButton" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</div>

