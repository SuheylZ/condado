﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SampleWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <asp:LoginView ID="LoginView1" runat="server">
            <AnonymousTemplate>
                <asp:LoginStatus ID="LoginStatus1" runat="server" LoginText="Sign In" LogoutAction="RedirectToLoginPage" />
            </AnonymousTemplate>
            <LoggedInTemplate>
                <asp:LoginName ID="LoginName1" runat="server" />
            </LoggedInTemplate>
        </asp:LoginView>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
