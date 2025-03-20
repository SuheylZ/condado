<%@ Page Title="Log In" Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>SQ Sales Tool Login</title>
</head>
<body>
    <script type="text/javascript">
        function clearMemory() {
            localStorage.clear();
        }
    </script>
      <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="page">
        <div class="header">
            <div class="loginDisplay">
                &nbsp;
            </div>
            <div class="clear hideSkiplink Toolbar">
                <table>
                    <tr>
                        <td style="width: 200px"><asp:Image ID="Image1" Width="200px" runat="server" ImageUrl="~/Images/select-care-logo.png" AlternateText="SelectCARE" /></td>
                        <td >
                            
                        </td>
                        <td align="right">
                            
                        </td>
                        <td >
                           
                        </td>
                        <td>
                            
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="main"> 
    <h2>
        Log In
    </h2>
    <p>
        <%--Please enter your username and password.
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false">Register</asp:HyperLink> if you don't have an account.--%>
    </p>
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" OnLoggedIn="OnLoggedIn">
        <LayoutTemplate>
            <span class="failureNotification">
                <asp:Literal ID="FailureText" runat="server"></asp:Literal>
            </span>
            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
                 ValidationGroup="LoginUserValidationGroup"/>
            <div class="accountInfo">
                <fieldset class="login">
                    <legend>Account Information</legend>
                    <p>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="passwordEntry" Width="80%"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                             CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" 
                            TextMode="Password" Width="80%"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                             CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:CheckBox ID="RememberMe" runat="server"/>
                        <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                    </p>
                </fieldset>
                <p class="submitButton">
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup" OnClientClick="javascript:return clearMemory();"/>
                </p>
            </div>
        </LayoutTemplate>
    </asp:Login>
 </div>
        <div class="clear">
        </div>
    </div>
    <div class="footer">
          Version: <%=  Application[Konstants.APPLICATION_VERSION].ToString() %>
    </div>
    </form>
</body>
</html>