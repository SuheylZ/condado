<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserPassword.ascx.cs" Inherits="UserControls_UserPassword" %>
 <fieldset class="condado">
            <legend>Change User Password</legend>
            <ul>
                <li>
                    <asp:Label ID="lblStatusKey" runat="server" />
                </li>
                <li>
                    <asp:Label ID="lblNewKey" runat="server" AssociatedControlID="txtNewPassword" Text="New Password" />
                    <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" 
                        CausesValidation="True" />
                    <asp:RequiredFieldValidator ID="vldRequiredPassword" runat="server" Display="None"
                        ErrorMessage="Please provide a password" ControlToValidate="txtNewPassword" InitialValue=""></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldRequiredPassword_ValidatorCalloutExtender"
                        runat="server" Enabled="True" TargetControlID="vldRequiredPassword" />
                </li>
                <li>
                    <asp:Label ID="lblConfirmKey" runat="server" AssociatedControlID="txtConfirm" Text="Confirm Password" />
                    <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CausesValidation="True" />
                    <asp:CompareValidator ID="vldCompare" runat="server" ControlToCompare="txtNewPassword"
                        ControlToValidate="txtConfirm" Display="None" ErrorMessage="Passwords are not same"></asp:CompareValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldCompare_ValidatorCalloutExtender" runat="server"
                        Enabled="True" TargetControlID="vldCompare" />

                    <asp:RequiredFieldValidator ID="vldConfirmRequired" runat="server" 
                        ControlToValidate="txtConfirm" Display="None" 
                        ErrorMessage="Confirm your password"></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldConfirmRequired_ValidatorCalloutExtender1" runat="server"
                        Enabled="True" TargetControlID="vldConfirmRequired" />

                </li>
                <li class="buttons">
                    <asp:Button ID="btnKeyChange" runat="server"  OnClientClick="validate();"
                        Text="Change" />
                    &nbsp;<asp:Button ID="btnKeyClose" runat="server" CausesValidation="False" 
                         Text="Close" />
                </li>
            </ul>
        </fieldset>