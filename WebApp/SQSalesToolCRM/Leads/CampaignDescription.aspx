<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CampaignDescription.aspx.cs"
    Title="Campaign Description" Inherits="Leads_CampaignDescription" %>
    <%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Campaign Description</title>
</head>
<script type="text/javascript">

    function CloseRadWindow() {
        var oWindow = null;
        if (window.radWindow) oWindow = window.radWindow;
        else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
        oWindow.Close();
        
    }
</script>
<body style="border: 2px">
    <form id="form1" runat="server">
     <telerik:radscriptmanager id="RadScriptManager1" runat="server"
            enabletheming="True" asyncpostbacktimeout="600">
            </telerik:radscriptmanager>
    <asp:UpdatePanel runat="server" ID="updatePanelMain">
        <ContentTemplate>
            <div>
                <table>
                    <tr>
                        <td>
                            <div id="div1" align="center" style="text-align: center">
                                <br />
                                <h2>
                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="lblCampaignDescription"
                                        Text="Campaign Description"></asp:Label></h2>
                                <br />
                                <asp:Label ID="lblCampaignDescription" Text="No description found." runat="server"></asp:Label>
                                <br />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 50px;">
                        </td>
                    </tr>
                </table>
            </div>
            <div class="buttons" align="center" style="height: 40px">
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="CloseRadWindow();return false;" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
