<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="AppointmentBuilder.aspx.cs" Inherits="Admin_AppointmentBuilder" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>

<%@ Register src="../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register src="../UserControls/UserPermissions.ascx" tagname="UserPermissions" tagprefix="uc2" %>
<%@ Register src="../UserControls/UserDetail.ascx" tagname="UserDetail" tagprefix="uc3" %>
<%@ Register src="../UserControls/StatusLabel.ascx" tagname="StatusLabel" tagprefix="uc4" %>
<%@ Register src="~/UserControls/SelectionLists.ascx" TagName="SelectionControl" TagPrefix="sc" %>

<asp:Content ID="cntHeader1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">
        function showMenu(e, contextMenu) {
            $telerik.cancelRawEvent(e);

            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }
        }

        function ConfirmDelete() {
            return confirm("Are you sure to delete the record?");
        }
    </script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">

    <asp:UpdatePanel runat="server" ID="updPanel">
        <ContentTemplate>
                
            <uc4:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField runat="server" ID="hdID" />
            <asp:Panel ID="pnlUserList" runat="server">
                <fieldset class="condado">
                    <legend>Appointment Builder</legend>

                    <div id="divAddNew" runat="server" visible="false">
                        <table style="width: 200px;">
                            <tr>
                                <td style="width: 50px;">Type</td>
                                <td><asp:DropDownList runat="server" ID="ddlScheduleTypes" Width="154px"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>Year</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtYear" Width="150px"></asp:TextBox>
                                    <ajaxToolkit:MaskedEditExtender 
                                        ID="txtYear_MaskedEditExtender" 
                                        runat="server" 
                                        TargetControlID="txtYear" 
                                        Mask="9999" 
                                        MaskType="Number"
                                        ClearMaskOnLostFocus="false">
                                    </ajaxToolkit:MaskedEditExtender>

                                    <ajaxToolkit:MaskedEditValidator ID="MaskedEditValidator1" 
                                            runat="server" 
                                            ControlExtender="txtYear_MaskedEditExtender" 
                                            ControlToValidate="txtYear" 
                                            EmptyValueMessage="*" 
                                            InvalidValueMessage="*" 
                                            IsValidEmpty="False"  
                                            ValidationGroup ="newSch" 
                                            Display="Dynamic" 
                                            SetFocusOnError="true"
                                            ForeColor="Red">
                                    </ajaxToolkit:MaskedEditValidator>
                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="align-right">
                                    <br />
                                    <asp:Button runat="server" Text="Cancel" ID="btnCancelSchedule" OnClick="btnCancelSchedule_Click" CausesValidation="false" />
                                    <asp:Button runat="server" Text="Add" ID="btnAddNewSchedule" OnClick="btnAddNewSchedule_Click" ValidationGroup="newSch" CausesValidation="true" />
                                </td>
                            </tr>
                        </table>
                        
                    </div>

                    <div id="divScheduleList" runat="server">
                        <div id="divToolbar" class="Toolbar">
                            <asp:Button runat="server" ID="btnAddAppointment" Text="Add" OnClick="btnAddAppointment_Click" />
                            <br />
                        </div>
                        <div id="divGrid">
            
                            <asp:GridView ID="grdAppointmentBuilder" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                                CssClass="mGrid" GridLines="None" ShowHeaderWhenEmpty="true" Width="300px" >
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="AppointmentType.Name" HeaderText="Type" ItemStyle-Width="100px" />
                                    <asp:BoundField DataField="Year" HeaderText="Year" ItemStyle-Width="100px" />
                                    <asp:TemplateField HeaderText="Options" ItemStyle-Width="60px">
                                        <ItemTemplate>
                                            <a title="Edit schedule" href="AppointmentBuilderEdit.aspx?id=<%#Convert.ToString(Eval("Key"))%>&year=<%#Convert.ToString(Eval("Year"))%>&type=<%#Convert.ToString(Eval("AppointmentType.Key"))%>" >E</a>
                                            <asp:LinkButton runat="server" ID="btnDelete" Text="D" ToolTip="Delete schedule" CommandArgument='<%# Eval("Key") %>' OnClick="btnDelete_Click" OnClientClick="return ConfirmDelete();" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no data to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                            </asp:GridView>
                        </div>
                    </div>

                    <br />
                    <asp:Label runat="server" ID="lblError" ForeColor="Red"></asp:Label>
                </fieldset>
            </asp:Panel>
               
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
