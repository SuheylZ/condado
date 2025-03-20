<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="ArcTestPage.aspx.cs" Inherits="SelectCare.ARC.Web.ArcTestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     <div>
        <div style="float: left">
            Request
            <div style="clear: both">
            <textarea id="txtRequestObject" runat="server" style="height: 440px; width: 500px"></textarea>
                </div>
        </div>
        <div style="float: left">
            Response<div style="clear: both">
            <textarea id="txtResponse" runat="server" style="height: 440px; width: 500px"></textarea>
                </div>
        </div>
        <div style="float: left">
            <asp:Button ID="Button1" runat="server" Text="Clear" OnClick="Btn_ClearClick"/>
            <asp:Button ID="Button2" runat="server" Text="InsertUpdateLead" OnClick="InserUpdateLead" />
            <asp:Button ID="Button3" runat="server" Text="UpdateCampaign" OnClick="UpdateCampaign"/>
            <asp:Button ID="Button4" runat="server" Text="UpdateStatus" OnClick="UpdateStatus"/>
            <asp:Button ID="Button5" runat="server" Text="StopCommunication" OnClick="UpdateCommunication"/>
            <asp:Button ID="Button6" runat="server" Text="AcdCapUpdate" OnClick="UpdateAcd"/>
            <asp:Button ID="Button7" runat="server" Text="ConsentUpdate" OnClick="UpdateConsent"/>
        </div>
    </div>
    </form>
     <div class="footer" style="clear: both;float: left;text-align: center">
            Version: <%= Version %>
        </div>
</body>
</html>
