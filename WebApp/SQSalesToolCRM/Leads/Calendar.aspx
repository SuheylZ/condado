<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="Leads_Calendar" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Src="~/Leads/UserControls/uc_calandar.ascx" TagPrefix="uc1" TagName="uc_calandar" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <script type="text/javascript" src="../Scripts/common.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        
    <div>
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server" />
        <uc1:uc_calandar runat="server" ID="uc_calandar1" />
    </div>
    </form>
</body>
</html>