<%@ Page Title="" Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="SQS_Dialer._Default1" %>

<!DOCTYPE HTML>
<html>
<head>
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 511px;
        }
    </style>
</head>
<body>
    <form action="Default.aspx" method="post" runat="server">
        <div>
    
        <table class="style1">
            <tr>
                <td colspan="3">
                    Enter your administrator credentials below to manage agents and campaigns for 
                    the Get A Lead application.</td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Label ID="lblLoginStatus" runat="server" ForeColor="Red" Visible="False">Incorrect credentials, please try again.</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblUsername" runat="server" Text="Username:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="username" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="username" ErrorMessage="Username Required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="password" runat="server" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="password" ErrorMessage="Password Required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td>
                    <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                </td>
                <td>
                    &nbsp;</td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>