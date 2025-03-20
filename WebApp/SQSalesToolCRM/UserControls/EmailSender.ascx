<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailSender.ascx.cs" Inherits="UserControls_EmailSender" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Src="StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>
<%@ Register src="../Leads/UserControls/OverrideEmailAttachments.ascx" tagname="OverrideEmailAttachments" tagprefix="uc2" %>
<fieldset id="fldSetForm" class="condado">
    <legend>Send Email</legend>
    <ul runat="server" id="listEmailTemplates" visible="false">
        <li>
            <asp:Label ID="lblEmailTemplate" runat="server"
                AssociatedControlID="txtFromEmail" Text="Email Templates:" CssClass="leftlabel"></asp:Label>
            <asp:DropDownList ID="ddlEmailTemplates" runat="server" Width="200px" AutoPostBack="true"
                OnSelectedIndexChanged="ddlEmailTemplates_SelectedIndexChanged">
            </asp:DropDownList>
        </li>
    </ul>
    <asp:Panel runat="server" ID="pnlEmailForm">
        <ul>

            <li>
                <asp:Label ID="lblFromEmail" runat="server" AssociatedControlID="txtFromEmail" Text="From Email:" CssClass="leftlabel"></asp:Label>
                <asp:TextBox ID="txtFromEmail" runat="server" MaxLength="100" ValidationGroup="emailValidation"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqFldVldFromMail" runat="server" ErrorMessage="Enter from email address."
                    ControlToValidate="txtFromEmail" Display="None" ValidationGroup="emailValidation"></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="reqFldVldFromMail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="reqFldVldFromMail" />
                <asp:RegularExpressionValidator ID="regexVldFromEmail" runat="server" ControlToValidate="txtFromEmail"
                    Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*|{(.*?)}"></asp:RegularExpressionValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="regexVldFromEmail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="regexVldFromEmail" />
            </li>
            <li>
                <asp:Label ID="lblToEmail" runat="server" AssociatedControlID="txtToEmail" Text="To Email(s):" CssClass="leftlabel"></asp:Label>
                <asp:TextBox ID="txtToEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                    ID="lblEmailSepration" runat="server" Text="{Separate emails with commas}"></asp:Label>
                <asp:RequiredFieldValidator ID="reqFldVldToEmail" runat="server" ControlToValidate="txtToEmail"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Atleast one To email is required."></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="reqFldVldToEmail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="reqFldVldToEmail" />
                <asp:RegularExpressionValidator ID="regexVldToEmail" runat="server" ControlToValidate="txtToEmail"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                    ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="regexVldToEmail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="regexVldToEmail" />
            </li>
            <li>
                <asp:Label ID="lblCCEmail" runat="server" AssociatedControlID="txtCCEmail" Text="CC Email(s):" CssClass="leftlabel"></asp:Label>
                <asp:TextBox ID="txtCCEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                    ID="lblEmailSepration1" runat="server" Text="{Separate emails with commas}"></asp:Label>
                <asp:RegularExpressionValidator ID="regexVldCCEmail" runat="server" ControlToValidate="txtCCEmail"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                    ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="regexVldCCEmail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="regexVldCCEmail" />
            </li>
            <li>
                <asp:Label ID="lblBCCEmail2" runat="server" AssociatedControlID="txtBCCEmail" Text="BCC Email(s):" CssClass="leftlabel"></asp:Label>
                <asp:TextBox ID="txtBCCEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                    ID="lblEmailSepration2" runat="server" Text="{Separate emails with commas}"></asp:Label>
                <asp:RegularExpressionValidator ID="regexBCCEmail" runat="server" ControlToValidate="txtBCCEmail"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                    ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="regexBCCEmail_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="regexBCCEmail" />
            </li>
            <li>
                <asp:Label ID="lblSubject" runat="server" AssociatedControlID="txtSubject" Text="Subject:" CssClass="leftlabel"></asp:Label><asp:TextBox
                    ID="txtSubject" runat="server" Width="400px" MaxLength="300" ValidationGroup="emailValidation"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqFldVldSubject" runat="server" ControlToValidate="txtSubject"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Enter email subject."></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="reqFldVldSubject_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="reqFldVldSubject" PopupPosition="BottomRight" />
            </li>
            <li>       
                <asp:UpdatePanel ID="updatePanelFormat" runat="server">

                    <ContentTemplate>
                        <asp:Label ID="lblFormat" runat="server" AssociatedControlID="rdlFormatText" Text="Format:" CssClass="leftlabel"></asp:Label>
                        <asp:RadioButton ID="rdlFormatText" runat="server" GroupName="Format" OnCheckedChanged="FormatText_CheckedChanged" AutoPostBack="True"  />
                        Text
            <asp:RadioButton ID="rdlFormatHtml" runat="server" GroupName="Format" Checked="True" OnCheckedChanged="FormatText_CheckedChanged" AutoPostBack="True" />
                        HTML 
                    </ContentTemplate>

                    <Triggers>

                        <asp:PostBackTrigger ControlID="rdlFormatText" />
                        <asp:PostBackTrigger ControlID="rdlFormatHtml" />
                    </Triggers>
                </asp:UpdatePanel>         
                        
                    
            </li>
            <%--<li class="Whiteheader">Body</li>--%>
            <li>
                <asp:RequiredFieldValidator ID="reqFldVldBodyText" runat="server" ControlToValidate="txtEmailBody"
                    ValidationGroup="emailValidation" Display="None" ErrorMessage="Enter email body text."></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender ID="reqFldVldBodyText_ValidatorCalloutExtender"
                    runat="server" Enabled="True" TargetControlID="reqFldVldBodyText" PopupPosition="TopLeft" />
                <%--<cc1:Editor ID="txtEmailBody" runat="server" CssClass="" BorderStyle="Solid" />--%>
                <telerik:RadEditor ID="txtEmailBody" runat="server" Skin="WebBlue"  
                    Height="350px" Width="100%"
                    BackColor="#EAEAEA" BorderStyle="None"
                    BorderWidth="0px" ContentAreaCssFile="~/App_Themes/Default/condado.radeditor.css"  >
                   
                    <%--    <Tools>
                                                             <telerik:EditorToolGroup Tag="MainToolbar">
                                                                    <telerik:EditorTool Name="Cut" />
                                                                    <telerik:EditorTool Name="Copy" />
                                                                    <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />                                                                    
                                                                </telerik:EditorToolGroup>
                                                                <telerik:EditorToolGroup Tag="Formatting">
                                                                    <telerik:EditorTool Name="Bold" />
                                                                    <telerik:EditorTool Name="Italic" />
                                                                    <telerik:EditorTool Name="Underline" />
                                                                    <telerik:EditorSeparator />
                                                                    <telerik:EditorSplitButton Name="ForeColor">
                                                                    </telerik:EditorSplitButton>
                                                                    <telerik:EditorSplitButton Name="BackColor">
                                                                    </telerik:EditorSplitButton>
                                                                    <telerik:EditorSeparator />
                                                                    <telerik:EditorDropDown Name="FontName">
                                                                    </telerik:EditorDropDown>
                                                                    <telerik:EditorDropDown Name="RealFontSize">
                                                                    </telerik:EditorDropDown>
                                                                </telerik:EditorToolGroup>
                                                                 
                                                            </Tools> --%>
              
                    <Tools>
                        <telerik:EditorToolGroup Tag="MainToolbar">
                            <telerik:EditorTool Name="Print" ShortCut="CTRL+P" />
                            <telerik:EditorTool Name="AjaxSpellCheck" Visible="False" />
                            <telerik:EditorTool Name="FindAndReplace" ShortCut="CTRL+F" />
                            <telerik:EditorTool Name="SelectAll" ShortCut="CTRL+A" />
                            <telerik:EditorTool Name="Cut" />
                            <telerik:EditorTool Name="Copy" ShortCut="CTRL+C" />
                            <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />
                            <telerik:EditorToolStrip Name="PasteStrip">
                            </telerik:EditorToolStrip>
                            <telerik:EditorSeparator />
                            <telerik:EditorSplitButton Name="Undo">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSplitButton Name="Redo">
                            </telerik:EditorSplitButton>
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup Tag="InsertToolbar">
                            <telerik:EditorTool Name="ImageManager" ShortCut="CTRL+G" />
                            <telerik:EditorTool Name="DocumentManager" />
                            <telerik:EditorTool Name="FlashManager" />
                            <telerik:EditorTool Name="MediaManager" />
                            <telerik:EditorTool Name="TemplateManager" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="LinkManager" ShortCut="CTRL+K" />
                            <telerik:EditorTool Name="Unlink" ShortCut="CTRL+SHIFT+K" />
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="Superscript" />
                            <telerik:EditorTool Name="Subscript" />
                            <telerik:EditorTool Name="InsertParagraph" />
                            <telerik:EditorTool Name="InsertGroupbox" />
                            <telerik:EditorTool Name="InsertHorizontalRule" />
                            <telerik:EditorTool Name="InsertDate" />
                            <telerik:EditorTool Name="InsertTime" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="FormatCodeBlock" />
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup>
                            <telerik:EditorDropDown Name="FormatBlock">
                            </telerik:EditorDropDown>
                            <telerik:EditorDropDown Name="FontName">
                            </telerik:EditorDropDown>
                            <telerik:EditorDropDown Name="RealFontSize">
                            </telerik:EditorDropDown>
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="AbsolutePosition" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="Bold" ShortCut="CTRL+B" />
                            <telerik:EditorTool Name="Italic" ShortCut="CTRL+I" />
                            <telerik:EditorTool Name="Underline" ShortCut="CTRL+U" />
                            <telerik:EditorTool Name="StrikeThrough" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="JustifyLeft" />
                            <telerik:EditorTool Name="JustifyCenter" />
                            <telerik:EditorTool Name="JustifyRight" />
                            <telerik:EditorTool Name="JustifyFull" />
                            <telerik:EditorTool Name="JustifyNone" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="Indent" />
                            <telerik:EditorTool Name="Outdent" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="InsertOrderedList" />
                            <telerik:EditorTool Name="InsertUnorderedList" />
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="ToggleTableBorder" />
                            <telerik:EditorTool Name="XhtmlValidator" />
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup>
                            <telerik:EditorSplitButton Name="ForeColor">
                            </telerik:EditorSplitButton>
                            <telerik:EditorSplitButton Name="BackColor">
                            </telerik:EditorSplitButton>
                            <telerik:EditorDropDown Name="ApplyClass">
                            </telerik:EditorDropDown>
                            <telerik:EditorToolStrip Name="FormatStripper">
                            </telerik:EditorToolStrip>
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup Tag="DropdownToolbar">
                            <telerik:EditorSplitButton Name="InsertSymbol">
                            </telerik:EditorSplitButton>
                            <telerik:EditorToolStrip Name="InsertTable">
                            </telerik:EditorToolStrip>
                            <telerik:EditorToolStrip Name="InsertFormElement">
                            </telerik:EditorToolStrip>
                            <telerik:EditorSplitButton Name="InsertSnippet">
                            </telerik:EditorSplitButton>
                            <telerik:EditorTool Name="ImageMapDialog" />
                            <telerik:EditorDropDown Name="InsertCustomLink">
                            </telerik:EditorDropDown>
                            <telerik:EditorSeparator />
                            <telerik:EditorTool Name="ConvertToLower" />
                            <telerik:EditorTool Name="ConvertToUpper" />
                            <telerik:EditorSeparator />
                            <telerik:EditorDropDown Name="Zoom">
                            </telerik:EditorDropDown>
                            <telerik:EditorSplitButton Name="ModuleManager">
                            </telerik:EditorSplitButton>
                            <telerik:EditorTool Name="ToggleScreenMode" ShortCut="F11" Visible="false" />
                            <telerik:EditorTool Name="AboutDialog" />
                        </telerik:EditorToolGroup>
                    </Tools>
                    <Content>
                                                            
                    

</Content>
                    <TrackChangesSettings CanAcceptTrackChanges="True" />
                </telerik:RadEditor>
            </li>
            <li>
                <%--<asp:UpdatePanel runat="server" ID="updatePanelOverrideAttachments" >
                    <ContentTemplate>--%>
                        <uc2:OverrideEmailAttachments ID="ctlOverrideEmailAttachments" runat="server" />
                    <%--</ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="ctlOverrideEmailAttachments"   />
                    </Triggers>
                </asp:UpdatePanel>--%>
                
                
            </li>
        </ul>
    </asp:Panel>

</fieldset>
<asp:HiddenField ID="hdnFieldEmailTemplateKey" runat="server" />
<asp:HiddenField ID="hdnFieldIsGeneralMode" runat="server" />
<asp:HiddenField ID="hdnFieldHasCustomTemplate" runat="server" />
<div class="buttons">
    <table>
        <tr>
            <td style="width: 100%;">
                <uc1:StatusLabel ID="lblMessageForm" runat="server" />
            </td>
        </tr>
    </table>
</div>
