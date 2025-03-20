<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="AppointmentBuilderEdit.aspx.cs" Inherits="Admin_AppointmentBuilderEdit" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>

<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="../UserControls/UserPermissions.ascx" TagName="UserPermissions" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/UserDetail.ascx" TagName="UserDetail" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>
<%@ Register Src="~/UserControls/SelectionLists.ascx" TagName="SelectionControl" TagPrefix="sc" %>

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

        function ConfirmUpdate() {
            return confirm(" You are going to update already generated schedules. \n\r This will replace previously created schedules. \n\r\n\r Are you sure to replace the records?");
        }
    </script>
    <style type="text/css">
        .appointment-table {
            display: inline;
        }
        .display-inline{
            display: inline;
        }
    </style>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">

    <asp:UpdatePanel runat="server" ID="updPanel">
        <ContentTemplate>

            <uc4:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField runat="server" ID="hdID" />
            <asp:HiddenField runat="server" ID="hiddenApptMonthKey" />

            <asp:Panel ID="pnlUserList" runat="server">
                <fieldset class="condado">
                    <legend>Appointment Builder - Edit</legend>

                    <div id="divToolbar" class="Toolbar FloatLeft">
                        <table style="width: 400px;">
                            <tr>
                                <td>Type: </td>
                                <td>
                                    <asp:Literal runat="server" ID="literalType"></asp:Literal></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Year: </td>
                                <td>
                                    <asp:Literal runat="server" ID="literalYear"></asp:Literal></td>
                                <td style="width: 20px;"></td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>Month</td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlMonth" Width="60px" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                                        <asp:ListItem Text="Jan" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Feb" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Mar" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Apr" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="May" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Jun" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Jul" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Aug" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="Sep" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="Oct" Value="10"></asp:ListItem>
                                        <asp:ListItem Text="Nov" Value="11"></asp:ListItem>
                                        <asp:ListItem Text="Dec" Value="12"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td>
                                    <%--<asp:TextBox runat="server" ID="txtGenerateSch" Width="126px"></asp:TextBox>--%>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <hr style="width: 400px;" />
                                </td>
                            </tr>
                        </table>

                        <br />
                        <br />
                        <table style="display: inline;">
                            <tr>
                                <td>Days</td>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkSun" /> Sun
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkMon" /> Mon
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkTue" /> Tue
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkWed" /> Wed
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkThu" /> Thu
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkFri" /> Fri
                                            </td>
                                            <td>
                                                <asp:CheckBox runat="server" ID="checkSat" /> Sat
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-right: 10px;">Increments</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtIncrements" MaxLength="2" Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="Required_txtIncrements" runat="server" ErrorMessage="*" ControlToValidate="txtIncrements" SetFocusOnError="true" ForeColor="Red" ValidationGroup="saveSch"></asp:RequiredFieldValidator>
                                    Minutes
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="align-right">
                                    <br />
                                    <br />
                                    <asp:Button runat="server" Text="Save Schedule" ID="btnSaveSchedule" OnClick="btnSaveSchedule_Click" ValidationGroup="saveSch" />
                                    <asp:Button runat="server" Text="Update Schedule" ID="btnUpdateSchedule" Visible="false" OnClick="btnUpdateSchedule_Click" />
                                    <asp:Button runat="server" Text="Generate Schedules" ID="btnGenerateSch" Visible="false" OnClick="btnGenerateSch_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>

                    <asp:Panel id="panelScheduleList" runat="server" Visible="false" CssClass="display-inline">

                        <asp:Panel id="panelGridSchedules" runat="server" CssClass="FloatLeft">
                            Schedules 
                            <asp:Button runat="server" Text="Add" ID="btnAddSchedule" OnClick="btnAddSchedule_Click" CssClass="FloatRight" />
                            <br />
                            <asp:GridView ID="grdSchedules" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                                CssClass="mGrid" GridLines="None" ShowHeaderWhenEmpty="true">
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="StartTime" HeaderText="Start" DataFormatString="{0:t}" HeaderStyle-Width="75px" />
                                    <asp:BoundField DataField="EndTime" HeaderText="End" DataFormatString="{0:t}" HeaderStyle-Width="75px" />
                                    <asp:BoundField DataField="Spaces" HeaderText="Appointment Spaces" HeaderStyle-Width="135px" ItemStyle-HorizontalAlign="Center" />

                                    <asp:TemplateField HeaderText="Options" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="btnSchEdit" Text="E" ToolTip="Edit Schedule" CommandArgument='<%# Eval("Key") %>' OnClick="btnSchEdit_Click" ></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="btnSchDelete" Text="D" ToolTip="Delete Schedule" CommandArgument='<%# Eval("Key") %>' OnClientClick="return ConfirmDelete();" OnClick="btnSchDelete_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no data to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                            </asp:GridView>
                        </asp:Panel>

                        <div class="FloatLeft" style="padding: 10px;"></div>
                        
                        <div id="divGridException" class="FloatLeft">
                            
                            Exceptions 
                            <asp:Button runat="server" Text="Add" ID="btnAddException" OnClick="btnAddException_Click" CssClass="FloatRight" />
                            <br />
                            <asp:GridView ID="grdExceptions" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                                CssClass="mGrid" GridLines="None" ShowHeaderWhenEmpty="true">
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="ExceptionDate" HeaderText="Date" DataFormatString="{0:d}" HeaderStyle-Width="90px" />
                                    <asp:BoundField DataField="StartTime" HeaderText="Start" DataFormatString="{0:t}" HeaderStyle-Width="75px" />
                                    <asp:BoundField DataField="EndTime" HeaderText="End" DataFormatString="{0:t}" HeaderStyle-Width="75px" />
                                    <asp:BoundField DataField="Spaces" HeaderText="Appointment Spaces" HeaderStyle-Width="135px" ItemStyle-HorizontalAlign="Center" />

                                    <asp:TemplateField HeaderText="Options" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="btnExEdit" Text="E" ToolTip="Edit Exception" CommandArgument='<%# Eval("Key") %>' OnClick="btnExEdit_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="btnExDelete" Text="D" ToolTip="Delete Exception" CommandArgument='<%# Eval("Key") %>' OnClientClick="return ConfirmDelete();" OnClick="btnExDelete_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no data to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                    
                    <div class="clear"></div>

                    
                    <br />
                    <asp:Label runat="server" ID="lblError" ForeColor="Red"></asp:Label>
                </fieldset>
            </asp:Panel>




            <telerik:RadWindow ID="dlgSchedules" runat="server" Width="235" Height="150"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="../Images/alert1.ico" Title="Schedule" VisibleOnPageLoad="false"
                OnClientClose="OnClientClose">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Updatepanel5" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:HiddenField runat="server" ID="hiddenScheduleKey" />

                                <fieldset class="condado display-inline">
                                <%--<legend>Add Schedule</legend>--%>
                                <table class="appointment-table">
                                    <%--<tr>
                                        <td colspan="2">Add Schedule</td>
                                    </tr>--%>
                                    <tr>
                                        <td style="width: 50px;">Start</td>
                                        <td>
                                            <telerik:RadTimePicker runat="server" ID="txtSchStart" Width="130px" DateInput-ReadOnly="true"></telerik:RadTimePicker>
                                            <asp:RequiredFieldValidator runat="server" ID="Required_txtSchStart" ErrorMessage="*" SetFocusOnError="true" ControlToValidate="txtSchStart" ForeColor="Red" ValidationGroup="addSch"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>End</td>
                                        <td>
                                            <telerik:RadTimePicker runat="server" ID="txtSchEnd" Width="130px" DateInput-ReadOnly="true"></telerik:RadTimePicker>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Spaces</td>
                                        <td>
                                            <telerik:RadMaskedTextBox runat="server" ID="txtSchSpaces" Mask="##" DisplayMask="" DisplayText="" Width="104px" ></telerik:RadMaskedTextBox>
                                            <asp:RequiredFieldValidator runat="server" ID="Required_txtSchSpaces" ErrorMessage="*" SetFocusOnError="true" ControlToValidate="txtSchSpaces" ForeColor="Red" ValidationGroup="addSch"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Button ID="btnSchOkay" runat="server" Text="Ok" OnClick="btnSchOkay_Click" ValidationGroup="addSch" />
                                            <asp:Button ID="btnSchUpdate" runat="server" Text="Update" OnClick="btnSchUpdate_Click" ValidationGroup="addSch" Visible="false"  />
                                            <asp:Button ID="btnSchCancel" runat="server" Text="Cancel" OnClick="btnSchCancel_Click" CausesValidation="false" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>


            <telerik:RadWindow ID="dlgExceptions" runat="server" Width="250" Height="180"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="../Images/alert1.ico" Title="Schedule" VisibleOnPageLoad="false"
                OnClientClose="OnClientClose">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Updatepanel1" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:HiddenField runat="server" ID="hiddenExceptionKey" />

                                <fieldset class="condado display-inline">
                                <%--<legend>Add Schedule</legend>--%>
                                <table class="appointment-table">
                                    <%--<tr>
                                        <td colspan="2">Add Schedule</td>
                                    </tr>--%>
                                    <tr>
                                        <td>Date</td>
                                        <td>
                                            <telerik:RadDatePicker runat="server" ID="txtExDate" Width="130px" DateInput-ReadOnly="true" DateInput-DateFormat="MM/dd/yyyy"></telerik:RadDatePicker>
                                            <asp:RequiredFieldValidator runat="server" ID="Required_txtExDate" ErrorMessage="*" SetFocusOnError="true" ControlToValidate="txtExDate" ForeColor="Red" ValidationGroup="addEx"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 50px;">Start</td>
                                        <td>
                                            <telerik:RadTimePicker runat="server" ID="txtExStart" Width="130px" DateInput-ReadOnly="true"></telerik:RadTimePicker>
                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ErrorMessage="*" SetFocusOnError="true" ControlToValidate="txtExStart" ForeColor="Red" ValidationGroup="addEx"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>End</td>
                                        <td>
                                            <telerik:RadTimePicker runat="server" ID="txtExEnd" Width="130px" DateInput-ReadOnly="true"></telerik:RadTimePicker>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Spaces</td>
                                        <td>
                                            <telerik:RadMaskedTextBox runat="server" ID="txtExSpaces" Mask="##" DisplayMask="" DisplayText="" Width="104px" ></telerik:RadMaskedTextBox>
                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ErrorMessage="*" SetFocusOnError="true" ControlToValidate="txtExSpaces" ForeColor="Red" ValidationGroup="addEx"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Button ID="btnExOkay" runat="server" Text="Ok" OnClick="btnExOkay_Click" ValidationGroup="addEx"  />
                                            <asp:Button ID="btnExUpdate" runat="server" Text="Update" OnClick="btnExUpdate_Click" ValidationGroup="addEx" Visible="false"  />
                                            <asp:Button ID="btnExCancel" runat="server" Text="Cancel" OnClick="btnExCancel_Click" CausesValidation="false"  />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow ID="dlgGeneratedSchedules" runat="server" Width="500px" Height="500px"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="/Images/alert1.ico" Title="Review Schedules" VisibleOnPageLoad="false">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Updatepanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <div style="text-align: center;">
                                    <asp:Button runat="server" ID="btnGeneratedSchedulesImport" Text="Continue" OnClick="btnGeneratedSchedulesImport_Click" />
                                    <asp:Button runat="server" ID="btnGeneratedSchedulesUpdate" Text="Update" OnClick="btnGeneratedSchedulesUpdate_Click" Visible="false" OnClientClick="return ConfirmUpdate();" />
                                    <asp:Button runat="server" ID="btnGeneratedSchedulesCancel" Text="Cancel" OnClick="btnGeneratedSchedulesCancel_Click"  />
                                </div>
                                <div style="height: 435px; overflow: scroll">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadGrid runat="server" ID="grdGeneratedSchedules" CellSpacing="0" GridLines="None" AutoGenerateColumns="False">
                                                <MasterTableView>
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="AppointmentType.Name" HeaderText="Appointment Type" UniqueName="column4" FilterControlAltText="Filter column4 column">
                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="ScheduleDate" HeaderText="Date" UniqueName="column" FilterControlAltText="Filter column column" DataFormatString="{0:d}"></telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="ScheduleTime" HeaderText="Time" UniqueName="column1" FilterControlAltText="Filter column1 column" DataFormatString="{0:t}"></telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Increment" HeaderText="Increment" UniqueName="column2" FilterControlAltText="Filter column2 column">
                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Spaces" HeaderText="Spaces" UniqueName="column3" FilterControlAltText="Filter column3 column">
                                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                        </telerik:GridBoundColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </td>
                                    </tr>
                                </table>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>

        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function OnClientClose(sender, eventArgs) {
            //window.location = "Leads.aspx";
        }

        // Get the instance of PageRequestManager.
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        // Add initializeRequest and endRequest
        prm.add_initializeRequest(function (sender, args) { $('#updProgress').show(); });
        prm.add_endRequest(function (sender, args) { $('#updProgress').hide(); });

        //function ShowLoading(group) {
        function ShowLoading() {
            $('#updProgress').show();

            //var isValid = false;

            //if (group == '') isValid = true;
            //else {
            //    if (typeof (Page_ClientValidate) == 'function') {
            //        isValid = Page_ClientValidate(group);
            //    }
            //}

            //if (isValid == true) {
            //    $('#updProgress').show();
            //}
        }
    </script>
</asp:Content>
