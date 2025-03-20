<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventCalendarAddEdit.ascx.cs" EnableViewState="true"
    Inherits="Leads_UserControls_EventCalendarAddEdit" %>
<%--<%@ Reference Control="~/MasterPages/Site.master" %>--%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
<asp:HiddenField ID="hdnFieldEditForm" runat="server" />
<asp:HiddenField ID="hdnHideEventsList" runat="server" />
<asp:HiddenField ID="hdnNewAddedEventIDs" runat="server" />
<asp:HiddenField ID="hdnIsOnActionWizard" runat="server" />
<asp:HiddenField ID="hdnSelectedUsr" runat="server" />
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
<!--
    function OnUserChange(dropdown) {

        var selectedindex = dropdown.selectedIndex;
        var selValue = dropdown.options[selectedindex].value;
        document.getElementById('<%= hdnSelectedUsr.ClientID %>').value = selValue;

        return true;
    }
    //-->
    </script>
</telerik:RadScriptBlock>
<%--TM [may 27, 2014], Added a message when user tries to delete the entry that is being editted--%>

<telerik:RadWindow ID="dlgAlertBox" runat="server" Style="z-index: 999999999999!important; width:auto; " VisibleStatusbar="False" Title="Alert">
    <ContentTemplate>
        <table style="width: 100%; height: 100%">
            <tr>
                <td>
                    <div id="divConfirmMessage" align="center" style="text-align: center; width:auto;">
                        <br />
                        <asp:Label ID="lblAlertMessage" runat="server"></asp:Label>
                        <br />
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 50px; width:100%;">
                    <div class="buttons" style="height: 40px; margin:0 auto 0 auto; width:70%; ">
                        <div style="float:left;">
                            <asp:Button ID="btnOk" runat="server" CausesValidation="false"
                                Text="Delete" OnClientClick="closeDlg();return true;" OnClick="btnOk_Click" Width="80px"
                                Height="30px" />
                        </div>
                        <div style="padding-left:20px; float:left; ">
                            <asp:Button ID="btnCancelDelete" runat="server" CausesValidation="false"
                                Text="Cancel" OnClientClick="closeDlg();return true;" OnClick="btnCancelDelete_Click" Width="80px"
                                Height="30px" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</telerik:RadWindow>

<div class="condado">
    <div id="mainDv">
        <div runat="server" id="divForm">
            <div id="fldSetForm" class="condado">
               <table cellpadding="5">
                   <tr><td style="background-color:white;">
              <table width="550" cellpadding ="5">
                  <tr><td align="center" valign="top" style="background-color:white;border-top-style:solid;border-bottom-style:solid;border-left-style:solid;border-right-style:solid;border-bottom-width:thin;border-left-width:thin;border-right-width:thin;border-top-width:thin">                
                <table cellpadding="3" >
                    <tr>
                        <td valign="top" align="left" style="background-color:white;">
                            &nbsp;</td>
                        <td colspan="2" valign="top" align="left" style="background-color:white;">
                            <asp:Label ID="lblMessageGrid" runat="server" CssClass="LabelMessage"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color:white;">
                            &nbsp;</td>
                        <td style="background-color:white;">
                            <span>Campaign:</span>
                        </td>
                        <td valign="top" align="left" style="background-color:white;">
                            <asp:Label ID="lbCampaign" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;&nbsp;</td>
                        <td>
                            Event Type:</td>
                        <td valign="top" align="left">
                            <table style="width:170px"><tr><td>
                            <input type="radio" id="rdoEventTypeTask" name="rdoEventType" runat="server" />
                                                           </td><td><span>Task</span>s</td><td>
                                                               <input type="radio" id="rdoEventTypeAppointment" name="rdoEventType" runat="server" value="Appointment" /><span>Appointment</span>
                            </td></tr></table>
                            </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;&nbsp;</td>
                        <td>
                            <span>User:</span>
                        </td>
                        <td valign="top" align="left">

                            <asp:DropDownList ID="ddlUser" Width="455px" runat="server" DataTextField="FullName"
                                DataValueField="Key" onchange='OnUserChange(this);'>
                            </asp:DropDownList>


                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            <span>Title:</span>
                        </td>
                        <td valign="top" align="left">
                            <asp:TextBox ID="txtTitle" MaxLength="30" runat="server" BackColor="#EAEAEA"
                                ValidationGroup="EventCalendarVldGroup" Width="450px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="vldTitle" runat="server" ValidationGroup="EventCalendarVldGroup" CssClass="Error"
                                ErrorMessage="* Title required" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
                            <%--<asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                            Enabled="True" TargetControlID="vldTitle" Width="250px"/>--%>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            <span>Description:</span>
                        </td>
                        <td valign="top" align="left">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="70px" Width="450px"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="vldDescription" runat="server" ValidationGroup="EventCalendarVldGroup" CssClass="Error"
                                            ErrorMessage="* Description required" ControlToValidate="txtDescription"></asp:RequiredFieldValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            <span>Date:</span>
                        </td>
                        <td valign="top" align="left">
                        <table style="width:450px">
                            <tr>
                                <td>
                                     
                            <%--<input type="radio" id="rdoTimeFromNow" name="rdoFromNow" runat="server" checked="true" value="V1" />--%>
                                     <input type="radio" id="rdoTimeFromNow" name="rdoFromNow" runat="server" />
                                </td>
                                <td>

                            <asp:DropDownList ID="ddlTimeFromNow" Width="100px" runat="server">
                                <asp:ListItem>5 minutes</asp:ListItem>
                                <asp:ListItem>10 minutes</asp:ListItem>
                                <asp:ListItem>15 minutes</asp:ListItem>
                                <asp:ListItem>30 minutes</asp:ListItem>
                                <asp:ListItem>1 hour</asp:ListItem>
                                <asp:ListItem>2 hours</asp:ListItem>
                                <asp:ListItem>4 hours</asp:ListItem>
                                <asp:ListItem>8 hours</asp:ListItem>
                                <asp:ListItem>24 hours</asp:ListItem>
                                <asp:ListItem>2 days</asp:ListItem>
                                <asp:ListItem>3 days</asp:ListItem>
                                <asp:ListItem>1 week</asp:ListItem>
                            </asp:DropDownList>

                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td  >

                            <asp:Label ID="Label1" runat="server" Text="From now to" Width="80px"></asp:Label>
                                </td>
                                <td>

                            <%--<input type="radio" id="rdoSpecificDateTimeFromNow" name="rdoFromNow" runat="server" checked="true" value="V1" />--%>
                                <input type="radio" id="rdoSpecificDateTimeFromNow" name="rdoFromNow" runat="server" />
                                </td>
                                <td>

                            <telerik:RadDatePicker ID="rdpSpcificDateFromNow" runat="server" Width="100px">
                            </telerik:RadDatePicker>

                                </td>
                                <td>
                                    <telerik:RadTimePicker ID="rtpSpcificTimeFromNow" runat="server" Width="80px">
                            </telerik:RadTimePicker>
                                </td>
                                <td>

                            <asp:DropDownList ID="ddlTimeZone" Width="150px" runat="server">
                                <%--<asp:ListItem Value="0">Select Time Zone</asp:ListItem>--%>
                                <asp:ListItem Value="1">Eastern</asp:ListItem>
                                <asp:ListItem Value="2">Central</asp:ListItem>
                                <asp:ListItem Value="3">Mountain</asp:ListItem>
                                <asp:ListItem Value="4">Pacific</asp:ListItem>
                                <asp:ListItem Value="5">Alaska</asp:ListItem>
                                <asp:ListItem Value="6">Hawaii</asp:ListItem>
                            </asp:DropDownList>

                                </td>
                            </tr>
                        </table>    
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            <span>Alert Me By:</span></td>
                        <td valign="top" align="left">
                            <table style="width:300px">
                                <tr>                                
                                <td>
                                    Popup </td>
                                <td>
                                    <input type="checkbox" id="chkAlertMePopup" runat="server" /><asp:DropDownList ID="ddlAlertTimeBefore" Width="100px" runat="server">
                                <asp:ListItem>0 minutes</asp:ListItem>
                                <asp:ListItem>5 minutes</asp:ListItem>
                                <asp:ListItem>10 minutes</asp:ListItem>
                                <asp:ListItem>15 minutes</asp:ListItem>
                                <asp:ListItem>30 minutes</asp:ListItem>
                                <asp:ListItem>1 hour</asp:ListItem>
                                <asp:ListItem>2 hours</asp:ListItem>
                                <asp:ListItem>4 hours</asp:ListItem>
                                <asp:ListItem>8 hours</asp:ListItem>
                                <asp:ListItem>24 hours</asp:ListItem>
                                <asp:ListItem>2 days</asp:ListItem>
                                <asp:ListItem>3 days</asp:ListItem>
                                <asp:ListItem>1 week</asp:ListItem>
                            </asp:DropDownList>                                    
                                </td>
                                <td>
                                    before start date
                                </td>
                                <td>
                            <input Visible="False" type="checkbox" id="chkAlertMeEmail" runat="server" />
                                </td>
                                <td>
                            <input Visible="False" type="checkbox" id="chkAlertMeText" runat="server" disabled="disabled" />
                                </td>
                                   </tr></table>
                            </td>
                    </tr>
                    <tr>
                        <td valign="top" align="left">
                             &nbsp;</td>
                        <td valign="top" align="left">
                             Other:
                            </td>
                        <td valign="top" align="left">
                        <table style="width:430px"><tr><td>
                            <input type="checkbox" id="chkCreateOutlookCalendarReminder" runat="server" disabled="disabled" /></td><td> <span>Create Outlook Calendar Reminder</span> </td><td>
                            <input type="checkbox" id="chkDismissUponStatusChange" runat="server" /></td><td>Dismiss Upon Action Change</td></tr><tr><td>
                            &nbsp;</td><td> &nbsp;&nbsp;<asp:Button ID="btnAddNew" runat="server" Text="Add New" CausesValidation="False" Visible="False"
                                    OnClientClick="setChangeFlag('0');" class="resetChangeFlag" OnClick="AddNew_Click" />
                                <asp:Button ID="btnSaveAndCloseOnForm" Visible="false" runat="server" ValidationGroup="EventCalendarVldGroup"
                                    CausesValidation="true" OnClick="SaveAndClose_Click"
                                    Text="Save and Close" />                                
                                </td><td>
                            &nbsp;</td><td valign="top" align="right">
                                 <asp:Label ID="lblCloseRadWindow" runat="server" />                                
                            </td></tr></table>    
                        </td>
                    </tr>
                    </table>
                </td></tr>
              </table>
                       </td></tr>
               </table>
            </div>
        </div>
    </div>
</div>
<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%--<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>--%><%--<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>--%><%--    <asp:HiddenField ID="hdSortColumn" runat="server" />
    <asp:HiddenField ID="hdSortDirection" runat="server" />--%>
<%--TM 16 June 2014, To make it hidden when Apply action is called--%>
<div class="condado" runat="server" id="divEventsandButtons">
    <div id="Div1">
        <div runat="server" id="divGrid" visible="true">
            <div id="fldSetGrid" class="condado">
                <table style="background-color:white;">
                    <tr>
                        <td>
                <table style="width: 100%">                   
                    <tr>
                        <td valign="top" style="background-color:white;" >
                            <table>
                                <tr>
                                    <td style="width:580px" style="background-color:white;"></td>
                                    <td >
                                <asp:Button ID="btnSaveOnForm" runat="server" ValidationGroup="EventCalendarVldGroup"
                                    CausesValidation="true" OnClick="Save_Click"
                                    Text="Save" />                                
                                <asp:Button ID="btnCancelChanges" runat="server" Text="Clear" CausesValidation="False"
                                    OnClientClick="setChangeFlag('0');" class="resetChangeFlag" OnClick="CancelChanges_Click" />
                                 <asp:Button ID="btnCloseEventCalendar" runat="server" CausesValidation="false" OnClick="btnCloseEventCalendar_Click"
                                    Text="Close" OnClientClick="closeDlg();return true;" />
                                    </td></tr>
                                </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color:white;" >
                            <table cellpadding="3" style="width:733px">
                                <tr>
                                    <td>
                            <asp:GridView ID="grdEventCalendar" runat="server" Width="100%" DataKeyNames="ID"
                                OnPageIndexChanging="grdEventCalendar_PageIndexChanging" OnSorting="grdEventCalendar_Sorting"
                                OnRowCommand="grdEventCalendar_RowCommand" AutoGenerateColumns="false" AllowSorting="True"
                                GridLines="None" PageSize="10" AlternatingRowStyle-CssClass="alt">
                                <Columns>
                                    <asp:TemplateField HeaderText="Schedule Date" SortExpression="PolicyType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Bind("Date") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Title" SortExpression="Title">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTitle" runat="server" Text='<%# Bind("Title") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User" SortExpression="UserName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" SortExpression="Status">
                                        <ItemTemplate>
                                            <%-- <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'>
                                            </asp:Label>--%>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# GetEventStatus((int)(DataBinder.Eval(Container.DataItem, "Status"))) %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Options">
                                        <ItemTemplate>
                                            <%--Enabled='<%1# Bind("IsCompleted") %>'--%>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                                Text="E"></asp:LinkButton>
                                            <asp:LinkButton ID="lnkComplete" runat="server" CausesValidation="false" CommandName="CompleteX"
                                                Text="C" OnClientClick="if(confirm('Are you sure to change the status to Completed?')== true){ return true;} else { return false;}"></asp:LinkButton>
                                            <asp:LinkButton ID="lnkOutlook" runat="server" CausesValidation="false" CommandName="OutlookX"
                                                Text="O" Enabled="false"></asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                Text="D" OnClientClick="if (!confirmDelete()) return false;"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no events to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" />
                            </asp:GridView>
                                        </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color:white;" >
<asp:Label ID="lblNoRecords" runat="server" Text="There are no events to display."></asp:Label>

                        </td>
                    </tr>
                    <tr>
                        <td style="background-color:white;" valign="top" align="left" >                        
                            <table>
                                <tr><td style="width:733px">
                                    <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                                    </td></tr>
                            </table>
                            
                        </td>
                    </tr>
                     <tr>
                        <td nowrap="nowrap" style="background-color:white;" >
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="background-color:white;" >
                            <asp:HiddenField ID="hdSortColumn" runat="server" />
<asp:HiddenField ID="hdSortDirection" runat="server" />
<asp:HiddenField ID="hdnDisplayPagingBar" runat="server" />
<asp:HiddenField ID="hdnCurrentEventID" runat="server" />
<asp:HiddenField ID="hdnShowOptionButtons" runat="server" />
<asp:HiddenField ID="hdnShowCurrentUserEvents" runat="server" />

                        </td>
                    </tr>
                </table>
                </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>


