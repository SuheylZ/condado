<%@ Control Language="C#" AutoEventWireup="true" CodeFile="uc_calandar.ascx.cs" Inherits="Leads.UserControls.LeadsUserControlsUcCalandar" %>
<%@ Register TagPrefix="ec" TagName="eventcalendaraddedit" Src="~/Leads/UserControls/EventCalendarAddEdit.ascx" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>




<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        
        function GetSelectedAccountId(sender, args) {

            var selectedAccountId = sender._cssClass;
            document.getElementById('<%= hdnAccountId.ClientID %>').value = selectedAccountId;




        }
        function CloseAccountWindow(sender, args) {
            var selectedAccountId = sender._cssClass;
            if (window.opener == null) {
                var parentWindow = window.parent;

                parentWindow.location.target = "_blank";
                parentWindow.location = "Leads.aspx?accountid=" + selectedAccountId;
            } else {


                var qrStr = window.opener.location.search;
                if (qrStr != null && qrStr != '') {
                    var currentAccountId = qrStr.split("?")[1].split("=")[1];
                    if (currentAccountId != null && currentAccountId == selectedAccountId)
                        return false;
                }
                window.opener.location = "Leads.aspx?accountid=" + selectedAccountId;
                // window.opener.location.reload(true);
                //window.close();
            }


            return false;
        }

        //]]>
    </script>
    <style>
        .RowHeightRadSchedular {
            height: 300px;
        }
        
    </style>
</telerik:RadScriptBlock>
<div>

    <table width="100%" style="background-color: White;">
        <tr>
            <td>
                <div style="font-weight: bold; text-align: center">
                    <span>Appointments </span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="height: 100%">
                <table width="100%;">
                    <tr>
                        <td style="vertical-align: top">
                            
                            <asp:Label runat="server" ID="lblMessage"></asp:Label>
                            <asp:HiddenField runat="server" ID="hdnAccountId" />
                            <telerik:RadCalendar ID="RadCalendar1" runat="server"  OnSelectionChanged="RadCalendar1_SelectionChanged" AutoPostBack="True" EnableMultiSelect="False" SelectedDate="" Skin="Simple" OnDataBinding="RadCalendar1_DataBinding">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Date="" Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                                <WeekendDayStyle CssClass="rcWeekend"></WeekendDayStyle>

                                <CalendarTableStyle CssClass="rcMainTable"></CalendarTableStyle>

                                <OtherMonthDayStyle CssClass="rcOtherMonth"></OtherMonthDayStyle>

                                <OutOfRangeDayStyle CssClass="rcOutOfRange"></OutOfRangeDayStyle>

                                <DisabledDayStyle CssClass="rcDisabled"></DisabledDayStyle>

                                <SelectedDayStyle CssClass="rcSelected"></SelectedDayStyle>

                                <DayOverStyle CssClass="rcHover"></DayOverStyle>

                                <FastNavigationStyle CssClass="RadCalendarMonthView RadCalendarMonthView_Simple"></FastNavigationStyle>

                                <ViewSelectorStyle CssClass="rcViewSel"></ViewSelectorStyle>
                            </telerik:RadCalendar>
                            <br />


                            <telerik:RadButton ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" OnClick="btnRefresh_Click" Skin="Simple" />

                       
                         <%--    <asp:EntityDataSource ID="EntityDataSource2" runat="server" ConnectionString="name=LeadModel" DefaultContainerName="LeadEntities" EntitySetName="EventCalendars1" EntityTypeFilter="EventCalendar"   Select="it.[ID], it.[UserID], it.[Title], it.[Description], it.[AlertPopup], it.[AlertEmail], it.[AlertTextSMS], it.[DismissUponStatusChange], it.[CreateOutlookReminder], it.[EventType], it.[EventStatus], it.[SnoozeAfter], it.[Completed], it.[TimeFromNow], it.[Dismissed], it.[IsActive], it.[IsDeleted], it.[Added], it.[Changed], it.[AccountID], it.[IsTimeFromNow], it.[IsSpecificDateTimeFromNow], it.[SpecificDateTimeFromNow], it.[AlertTimeBefore], it.[IsOpened]">
                            </asp:EntityDataSource>--%>
                        </td>
                        <td style="overflow: scroll; overflow-x: hidden; overflow-y: scroll">
                            <telerik:RadScheduler ID="RadScheduler1" runat="server" DataDescriptionField="Description" DataEndField="SpecificDateTimeFromNow" DataKeyField="ID" DataStartField="SpecificDateTimeFromNow" DataSubjectField="Title" AllowDelete="False" AllowEdit="False" AllowInsert="False" EnableTheming="True" HoursPanelTimeFormat="t" ReadOnly="True" Skin="Simple" TimeLabelRowSpan="4" FirstDayOfWeek="Monday" LastDayOfWeek="Sunday" OverflowBehavior="Expand" ShowAllDayRow="False" AppointmentStyleMode="Default"
                                AdvancedForm-EnableCustomAttributeEditing="true"  RowHeight="100px"  ForeColor="Black"
                                CustomAttributeNames="Title,Description,AccountID,PrimaryName,UserID" DayStartTime="06:00:00" WorkDayEndTime="19:00:00" OnNavigationCommand="RadScheduler1_NavigationCommand" Height="">
                                <AdvancedForm Modal="True" />

                                <DayView UserSelectable="True" ShowResourceHeaders="True"></DayView>
                                <MonthView UserSelectable="False"></MonthView>
                                <WeekView UserSelectable="False"></WeekView>
                                <TimelineView UserSelectable="False"></TimelineView>

                                <AppointmentTemplate>
                                    <h3 style="color:black;">
                                                   Account ID:
                                                    <%# Eval("AccountID") %> 
                                                  Primary Name:
                                                    <%# Eval("PrimaryName") %> 
                                                     
                                                </h3>
                                                <br />
                                                 <h3 style="color:black;">
                                                   Title:
                                                    <%# Eval("Title") %> 
                                                     Description:   <%#Eval("Description") %>
                                                </h3>
                                    <table>
                                        <tr>
                                            <td style="width:75%;" >

                                             
                                            </td>
                                            <td style="width:25%;text-align: right;">
                                                <telerik:RadButton ID="btnOpenAccount" runat="server" Text="Open Account" CssClass='<%#  Eval("AccountID") %>' CausesValidation="false" OnClientClicked="CloseAccountWindow" Skin="Simple"  AutoPostBack="False" />
                                                <telerik:RadButton ID="btnEvent" runat="server" Text="Open Event" Skin="Simple"  CssClass='<%#  Eval("AccountID") %>' AutoPostBack="True" OnClientClicked="GetSelectedAccountId" OnClick="btnAddEvent1_Click" />

                                            </td>

                                        </tr>

                                    </table>
                                </AppointmentTemplate>
                                <TimeSlotContextMenuSettings EnableDefault="true"></TimeSlotContextMenuSettings>
                                <AppointmentContextMenuSettings EnableDefault="False"></AppointmentContextMenuSettings>

                            </telerik:RadScheduler>

                            <telerik:RadWindow ID="dlgEventCalendar" runat="server" Width="750" Height="650" EnableViewState="false"
                                 Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                                IconUrl="../Images/alert1.ico" Title="Calendar Event - Lead Occurrence" VisibleOnPageLoad="False"
                                OnClientClose="OnClientClose">
                                <ContentTemplate>
                                    <asp:UpdatePanel ID="Updatepanel5" runat="server">
                                        <ContentTemplate>
                                            <ec:eventcalendaraddedit ID="EventCalendarAddEdit1" runat="server" />

                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="EventCalendarAddEdit1" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <div class="buttons" style="text-align: right">
                                        <asp:Button ID="btnCloseEventCalendar" runat="server" CausesValidation="false" OnClick="btnCloseEventCalendar_Click"
                                            Text="Close" />
                                    </div>


                                </ContentTemplate>
                            </telerik:RadWindow>
                            <%--  </Windows>
                            </telerik:RadWindowManager>--%>
                            <%--<ec:RadWindowControl runat="server" ID="RadWindowControl" />--%>
                            <asp:SqlDataSource ID="AppointmentsDataSource" runat="server"
                                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT ID, Subject, Description, Start, [End], RecurrenceRule, RecurrenceParentID, Reminder, Annotations FROM Appointments"></asp:SqlDataSource>
                            <%--<asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                                SelectCommand="SELECT [evc_id], [act_account_id], [usr_user_id], [evc_title], [evc_description], [evc_is_time_from_now], [evc_time_from_now], [evc_dismiss_upon_status_change], [evc_is_specific_date_time_from_now], [evc_event_type], [evc_event_status], [evc_snooze_after], [evc_completed], [evc_dismissed], [evc_create_outlook_reminder], [evc_specific_date_time_from_now], [evc_alert_popup], [evc_alert_email], [evc_alert_text_sms], [evc_alert_time_before], [evc_add_user], [evc_add_date], [evc_modified_user], [evc_modified_date], [evc_active_flag], [evc_delete_flag], [evc_isopened_flag] FROM [eventcalendar]"></asp:SqlDataSource>--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
<telerik:RadScriptBlock ID="RadScriptBlock2" runat="server">
    <script type="text/javascript">
        
        // Sys.Application.add_load(BindEvents);

        var myDlg = null;

        function showDlg(v) {
            
            switch (v) {

                case "EventCalendar":
                    myDlg = window.$find('<%=dlgEventCalendar.ClientID %>');
                    break;


            }

            if (myDlg != null) {
                myDlg.show();
                myDlg.center();

                return false;
            }
            return false;
        }

        function closeDlg1() {

            $('#dirtyFlag').val('0');

            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
            }

            return false;
        }



        function closeDlg() {
            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
            }
        }

        function closeDlg2() {
            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
                window.__doPostBack("btnYes", "valYes");
            }
        }




        function OnClientClose(sender, eventArgs) {
            window.location = "viewCalander.aspx";

        }
    </script>


</telerik:RadScriptBlock>

