<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EventReminder.aspx.cs" Inherits="Leads_EventReminder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hdnEventId" runat="server" />
    <asp:HiddenField ID="hdnAccountId" runat="server" />

        <br />
    <asp:HiddenField ID="hdnUserKey" runat="server" />

    <div>
        &nbsp;&nbsp;<asp:Label ID="lbEventTitle" runat="server" Font-Size="Medium" Font-Bold="true"></asp:Label>

        &nbsp;&nbsp;<asp:Label ID="lbDateTime" runat="server" Font-Bold="true"></asp:Label><span style="font-weight:bold;color:Red">&nbsp;(Past Due)</span><br /><br />
        &nbsp;&nbsp;<asp:Label ID="lbTitle" runat="server" Font-Bold="true"></asp:Label><br /><br />
        &nbsp;&nbsp;<div ID="txtDesc" runat="server"></div>

                           <div class="buttons" style="text-align: left">

    <span>Snooze for:</span>&nbsp;&nbsp;<asp:DropDownList ID="ddlTimeFromNow" Width="100px" runat="server">
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
                    <asp:Button ID="btnSnooze" runat="server" CausesValidation="false" OnClick="Snooze_Click" Text="Snooze" />&nbsp;&nbsp;
                    <asp:Button ID="btnComplete" runat="server" CausesValidation="false" OnClick="Complete_Click" Text="Complete" />&nbsp;&nbsp;
                               <%-- [QN, 02-05-2013] btnDismiss has been hide on clients request. => [[7:56:18 PM] John Dobrotka: Qasim can you make a change on the event pop up?]
                    [7:56:27 PM] John Dobrotka: just hide the dismiss button.--%>
                    <asp:Button ID="btnDismiss" runat="server" CausesValidation="false" OnClick="Dismiss_Click" Text="Dismiss Reminder" Visible="false"/>
                    <asp:Button ID="btnOpenAccount" runat="server" CausesValidation="false" OnClick="OpenAccount_Click" Text="Open Account" />&nbsp;&nbsp;
                    </div>

    </div>
    </form>
</body>
</html>
