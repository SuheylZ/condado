<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Individuals.aspx.cs" Inherits="Leads_Individuals" %>
<%@ Register Src="~/Leads/UserControls/IndividualsAddEdit.ascx" TagName="IndividualsAddEdit" TagPrefix="uc" %>
<%@ Register src="../UserControls/AlertConsent.ascx" tagname="AlertConsent" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function CloseRadWindow() {
            var oWindow = null;
            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;
            oWindow.Close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <uc:IndividualsAddEdit ID="IndividualsAddEdit1" runat="server" />
    
    </div>
        <uc1:AlertConsent ID="dlgConsent" runat="server" />
    </form>
</body>
</html>
