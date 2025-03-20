<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EventCalendar.aspx.cs" Inherits="Leads_EventCalendar" EnableEventValidation="false"
    Title="Event Calendar" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="UserControls/EventCalendarAddEdit.ascx" TagName="EventCalendarAddEdit"
    TagPrefix="ec" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Event Calendar</title>
    <link rel="Stylesheet" href="~/Styles/GeneralTable.css" type="text/css" id="homeCss" runat="server" />
</head>
<script type="text/javascript">

    function CloseRadWindow() {
        var oWindow = null;
        if (window.radWindow)
            oWindow = window.radWindow;
        else if (window.frameElement.radWindow)
            oWindow = window.frameElement.radWindow;
        oWindow.Close();
    }

    function SetPageAndClose(str) {
        top.location.href = str;
    }


</script>
<body style="border: 2px">
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="scmanager" runat="server">
        </telerik:RadScriptManager>

        <asp:MultiView ID="mView" runat="server" ActiveViewIndex="0">
            <asp:View ID="CalendarEventView" runat="server">
                   <%--TM [17 june 2014] changed to false HideEventsList="false"--%>
                   <ec:EventCalendarAddEdit ID="EventCalendarAddEdit1" runat="server" HideEventsList="false"
                            IsOnActionWizard="true"></ec:EventCalendarAddEdit>
                       
                <asp:UpdatePanel ID="updatePanelEventCalendar" runat="server">
                    <ContentTemplate>                    

                            <div class="buttons" style="text-align: right">
                                <!--YA[April 18, 2013] Added close button server side event as using client side closedlg will not work properly  -->
                               <!--KH[May 8, 2014]<asp:Button ID="btnCloseEventCalendar" runat="server" CausesValidation="false" OnClick="btnCloseEventCalendar_Click"
                                    Text="Close" />-->
                            </div>
                        
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnCloseEventCalendar" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:View>
        </asp:MultiView>


        <asp:Label ID="lblCloseRadWindow" runat="server" />
    </form>
</body>
</html>
