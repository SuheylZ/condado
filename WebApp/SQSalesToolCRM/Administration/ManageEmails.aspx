<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageEmails.aspx.cs" Inherits="Admin_ManageEmails" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript" language="javascript">
        
    </script>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">
            function validationFailed(sender, eventArgs) {
                $("#ErrorHolder").append("<p>Validation failed for '" + eventArgs.get_fileName() + "'.</p>").fadeIn("slow");
            }
            function IsFileSelected() {

                //                var upload = $find("<%= fUploadAsync.ClientID %>");
                //                var fileInputs = upload.getFileInputs();
                //                alert(fileInputs.length);
                //finds all file uploads that are currently in progress
                //                var uploadingRows = $(".RadAsyncUpload").find(".ruUploadProgress");
                //                //iterates and checks is there any file uploads that are successfully completed or failed and if yes - pop-up an alert box and prevent page submitting 
                //                for (var i = 0; i < uploadingRows.length; i++) {
                //                    if (!$(uploadingRows[i]).hasClass("ruUploadCancelled") && !$(uploadingRows[i]).hasClass("ruUploadFailure") && !$(uploadingRows[i]).hasClass("ruUploadSuccess")) {
                //                        alert("you could not submit the page during upload :)");
                //                        return false;
                //                    }
                //                }
                //                for (var i = fileInputs.length - 1; i >= 0; i--) {
                //                    return true;
                //                }

                return false;
            }                                                        
       
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- start waheed -->
    <script type="text/javascript">        Sys.Application.add_load(bindEvents);
    </script>
    <!-- end waheed -->
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
         
            <div id="divGrid" runat="server">
                <asp:UpdatePanel ID="UpdatePanelGrid" runat="server">
                    <ContentTemplate>
                        <fieldset class="condado">
                            <legend>Manage Email Templates</legend>
                            <div id="divToolbar" class="Toolbar">
                                <asp:Button ID="btnAddNewEmail" runat="server" Text="Add New Email" OnClick="btnAddNewEmail_Click"
                                    CausesValidation="False" class="resetChangeFlag" />
                                <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                                <br />
                                <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
                            </div>
                            <br />
                            <asp:HiddenField ID="hdSortColumn" runat="server" />
                            <asp:HiddenField ID="hdSortDirection" runat="server" />
                            <asp:GridView ID="grdEmailTemplates" runat="server" Width="100%" OnPageIndexChanging="grdEmailTemplates_PageIndexChanging"
                                OnSorting="grdEmailTemplates_Sorting" OnRowCommand="grdEmailTemplates_RowCommand"
                                AutoGenerateColumns="False" DataKeyNames="EmailKey" AllowSorting="True" GridLines="None"
                                AlternatingRowStyle-CssClass="alt" CssClass="mGrid" OnRowDataBound="grdEmailTemplates_RowDataBound">
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="EmailKey" HeaderText="Key" Visible="false"></asp:BoundField>
                                    <asp:BoundField DataField="EmailTitle" HeaderText="Title" SortExpression="EmailTitle">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="40%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SubjectEmail" HeaderText="Subject" SortExpression="SubjectEmail">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="40%" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Enabled">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                                Text='<%# Bind("Status") %>'></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Options">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                                Text="Edit" class="resetChangeFlag"></asp:LinkButton>
                                            |
                                            <asp:LinkButton ID="lnkSettings" runat="server" CausesValidation="False" CommandName="SettingX"
                                                Text="Settings" OnClientClick="" class="resetChangeFlag"></asp:LinkButton>
                                            <asp:Label runat="server" id="lblEmailSep" Text="&nbsp;|&nbsp;" />
                                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                Text="Delete" OnClientClick="if(confirm('Are you sure want to delete email template?')== true) true; else return false;"></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" />
                            </asp:GridView>
                        </fieldset>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAddNewEmail" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div id="divForm" runat="server">
                <asp:UpdatePanel ID="UpdatePanelForm" runat="server">
                    <ContentTemplate>                    
                        <telerik:RadTabStrip ID="tlEmailTemplateStrip" runat="server" MultiPageID="tabContEmailTemplate"
                            CausesValidation="True" SelectedIndex="0" Skin="WebBlue" OnTabClick="tlEmailTemplateStrip_TabClick1">
                            <Tabs>
                                <telerik:RadTab runat="server" Text="Compose Email" PageViewID="tlPage1" Selected="True">
                                </telerik:RadTab>
                                <telerik:RadTab runat="server" Text="Delivery/Drip Settings" PageViewID="tlPage2">
                                </telerik:RadTab>
                            </Tabs>
                        </telerik:RadTabStrip>
                        <telerik:RadMultiPage ID="tabContEmailTemplate" runat="server" SelectedIndex="0">
                            <telerik:RadPageView ID="tlPage1" runat="server" Selected="true">
                                <fieldset id="fldSetForm" class="condado">
                                    <legend>Add/Edit Email Templates</legend>
                                    <ul>
                                        <li style="text-align: right">
                                            <asp:Button runat="server" Text="Return to manage emails" ID="btnReturnToManageEmails"
                                                CausesValidation="False" OnClick="btnCancelOnForm_Click" class="returnButton">
                                            </asp:Button>
                                        </li>
                                        <li style="text-align: right"><a id="popupTags" onclick="javascript:window.open('TagFields.aspx', 'pop', 'width=300,height=600');">
                                            View Field Tags</a> </li>
                                        <li>
                                            <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title:"></asp:Label>
                                            <asp:TextBox ID="txtTitle" runat="server" MaxLength="200" ValidationGroup="emailValidation"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="regFldVldTitle" runat="server" ControlToValidate="txtTitle"
                                                Display="None" ErrorMessage="Enter email template title." ValidationGroup="emailValidation"></asp:RequiredFieldValidator>
                                            <asp:ValidatorCalloutExtender ID="regFldVldTitle_ValidatorCalloutExtender" runat="server" 
                                                Enabled="True" TargetControlID="regFldVldTitle" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblFromEmail" runat="server" AssociatedControlID="txtFromEmail" Text="From Email:"></asp:Label>
                                            <asp:TextBox ID="txtFromEmail" runat="server" MaxLength="100" ValidationGroup="emailValidation"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="reqFldVldFromMail" runat="server" ErrorMessage="Enter from email address."
                                                ControlToValidate="txtFromEmail" Display="None" ValidationGroup="emailValidation"></asp:RequiredFieldValidator>
                                            <asp:ValidatorCalloutExtender ID="reqFldVldFromMail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="reqFldVldFromMail" />
                                            <asp:RegularExpressionValidator ID="regexVldFromEmail" runat="server" ControlToValidate="txtFromEmail"
                                                Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*|{(.*?)}"></asp:RegularExpressionValidator>
                                            <asp:ValidatorCalloutExtender ID="regexVldFromEmail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="regexVldFromEmail" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblToEmail" runat="server" AssociatedControlID="txtToEmail" Text="To Email(s):"></asp:Label>
                                            <asp:TextBox ID="txtToEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                                                ID="lblEmailSepration" runat="server" Text="{Separate emails with commas}"></asp:Label>
                                            <asp:RequiredFieldValidator ID="reqFldVldToEmail" runat="server" ControlToValidate="txtToEmail" ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Atleast one To email is required."></asp:RequiredFieldValidator>
                                            <asp:ValidatorCalloutExtender ID="reqFldVldToEmail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="reqFldVldToEmail" />
                                            <asp:RegularExpressionValidator ID="regexVldToEmail" runat="server" ControlToValidate="txtToEmail" ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                                                ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                                            <asp:ValidatorCalloutExtender ID="regexVldToEmail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="regexVldToEmail" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblCCEmail" runat="server" AssociatedControlID="txtCCEmail" Text="CC Email(s):"></asp:Label>
                                            <asp:TextBox ID="txtCCEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                                                ID="lblEmailSepration1" runat="server" Text="{Separate emails with commas}"></asp:Label>
                                            <asp:RegularExpressionValidator ID="regexVldCCEmail" runat="server" ControlToValidate="txtCCEmail" ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                                                ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                                            <asp:ValidatorCalloutExtender ID="regexVldCCEmail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="regexVldCCEmail" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblBCCEmail2" runat="server" AssociatedControlID="txtBCCEmail" Text="BCC Email(s):"></asp:Label>
                                            <asp:TextBox ID="txtBCCEmail" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox><asp:Label
                                                ID="lblEmailSepration2" runat="server" Text="{Separate emails with commas}"></asp:Label>
                                            <asp:RegularExpressionValidator ID="regexBCCEmail" runat="server" ControlToValidate="txtBCCEmail" ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email."
                                                ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}"></asp:RegularExpressionValidator>
                                            <asp:ValidatorCalloutExtender ID="regexBCCEmail_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="regexBCCEmail" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblBCCEmailHidden" runat="server" AssociatedControlID="txtBCCEmailHidden" Text="Hidden BCC Email(s):"></asp:Label>
                                            <asp:TextBox ID="txtBCCEmailHidden" runat="server" MaxLength="800" ValidationGroup="emailValidation"></asp:TextBox>
                                            <asp:Label ID="lblEmailSepration3" runat="server" Text="{Separate emails with commas}"></asp:Label>
                                            <asp:RegularExpressionValidator ID="regexBCCEmailHidden" runat="server" ControlToValidate="txtBCCEmailHidden" Display="None" ErrorMessage="Invalid email address(s). Enter comma separated email." ValidationExpression="(\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}[,;])*\w([-_+.']*\w+)+@(\w(-*\w+)+\.)+[a-zA-Z]{2,4}|{(.*?)}" ValidationGroup="emailValidation"></asp:RegularExpressionValidator>
                                            <asp:ValidatorCalloutExtender ID="regexBCCEmailHidden_ValidatorCalloutExtender" runat="server" Enabled="True" TargetControlID="regexBCCEmailHidden" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblSubject" runat="server" AssociatedControlID="txtSubject" Text="Subject:"></asp:Label><asp:TextBox
                                                ID="txtSubject" runat="server" Width="400px" MaxLength="300"  ValidationGroup="emailValidation"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="reqFldVldSubject" runat="server" ControlToValidate="txtSubject"  ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Enter email subject."></asp:RequiredFieldValidator>
                                            <asp:ValidatorCalloutExtender ID="reqFldVldSubject_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="reqFldVldSubject" PopupPosition="BottomRight" />
                                        </li>
                                        <%--<li>
                                            <asp:Label ID="lblPopNotification" runat="server" AssociatedControlID="chkPopNotifcation"
                                                Text="Screen Pop Notification When Email Opened:"></asp:Label>
                                            <asp:CheckBox ID="chkPopNotifcation" runat="server" /></li>--%>
                                        <li>
                                            <asp:Label ID="lblLockTemplate" runat="server" AssociatedControlID="chkLockTemplate"
                                                Text="Lock Template:"></asp:Label>
                                            <asp:CheckBox ID="chkLockTemplate" runat="server" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblrequireAttachment" runat="server" AssociatedControlID="chkLockTemplate" Text="Require Attachment:"></asp:Label>
                                            <asp:CheckBox ID="chkRequireAttachment" runat="server" />
                                        </li>
                                        <li>
                                            <asp:Label ID="lblFormat" runat="server" AssociatedControlID="rdlFormatText" Text="Format:"></asp:Label>
                                            <asp:RadioButton ID="rdlFormatText" runat="server"  GroupName="Format" />Text
                                            <asp:RadioButton ID="rdlFormatHtml" runat="server" Checked="True" GroupName="Format" />HTML
                                        </li>
                                        <li class="Whiteheader">Body</li>
                                        <li>
                                            <asp:RequiredFieldValidator ID="reqFldVldBodyText" runat="server" ControlToValidate="txtEmailBody" ValidationGroup="emailValidation"
                                                Display="None" ErrorMessage="Enter email body text."></asp:RequiredFieldValidator>
                                            <asp:ValidatorCalloutExtender ID="reqFldVldBodyText_ValidatorCalloutExtender" runat="server"
                                                Enabled="True" TargetControlID="reqFldVldBodyText" PopupPosition="TopLeft" Width="200px" />
                                            <%--<cc1:Editor ID="txtEmailBody" runat="server" CssClass="" BorderStyle="Solid" />--%>
                                            <telerik:RadEditor runat="server" ID="txtEmailBody" Width="100%"></telerik:RadEditor>
                                        </li>
                                    </ul>
                                    <fieldset class="condado">
                                        <legend>Attachments</legend>
                                        <ul>
                                            <li>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <telerik:RadAsyncUpload ID="fUploadAsync" runat="server" UploadedFilesRendering="BelowFileInput"
                                                                MaxFileInputsCount="1" OnClientFileSelected="" OnClientFilesSelected="">
                                                            </telerik:RadAsyncUpload>
                                                        </td>
                                                        <td>
                                                            <div id="ErrorHolder">
                                                            </div>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblDescription" runat="server">Description</asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtDescription" runat="server" MaxLength="200" ValidationGroup="Upload"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="reqFldVldDescription" runat="server" ControlToValidate="txtDescription"
                                                                Display="None" ErrorMessage="Enter attachment description." ValidationGroup="Upload"></asp:RequiredFieldValidator>
                                                            <asp:ValidatorCalloutExtender ID="reqFldVldDescription_ValidatorCalloutExtender"
                                                                runat="server" Enabled="True" TargetControlID="reqFldVldDescription" PopupPosition="BottomRight" />
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload"
                                                                ValidationGroup="Upload" OnClientClick="validateGroup('Upload');" Enabled="False" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </li>
                                            <li>
                                                <asp:GridView ID="grdEmailAttachments" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                                    CssClass="mGrid" DataKeyNames="EmailAttachmentKey" GridLines="None" OnRowCommand="grdEmailAttachments_RowCommand"
                                                    OnSorting="grdEmailTemplates_Sorting" Width="100%">
                                                    <AlternatingRowStyle CssClass="alt" />
                                                    <Columns>
                                                        <asp:BoundField DataField="EmailAttachmentKey" HeaderText="Key" Visible="False" />
                                                        <asp:TemplateField HeaderText="File">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDownloadAttachment" runat="server" CausesValidation="False"
                                                                    CommandName="Download" Text="Download"></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                        </asp:TemplateField>
                                                       <%-- <asp:BoundField DataField="EmailAttachmentFileName" HeaderText="File Name" SortExpression="EmailAttachmentFileName">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" Width="20%" />
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="EmailAttachmentDescription" HeaderText="Description" SortExpression="EmailAttachmentDescription">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" Width="70%" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField ShowHeader="False">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDeleteAttachment" runat="server" CausesValidation="False"
                                                                    CommandName="DeleteAttachment" OnClientClick="if(confirm('Are you sure want to delete email attachment?')== true) true; else return false;"
                                                                    Text="Delete"></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        No record found.
                                                    </EmptyDataTemplate>
                                                    <HeaderStyle CssClass="gridHeader" />
                                                    <PagerSettings Position="Top" />
                                                    <PagerStyle VerticalAlign="Bottom" />
                                                </asp:GridView>
                                            </li>
                                        </ul>
                                    </fieldset>
                                </fieldset>
                            </telerik:RadPageView>
                            <telerik:RadPageView ID="tlPage2" runat="server">
                                <fieldset class="condado">
                                    <legend>Delayed Sending (Drip Marketing): </legend>
                                    <ul>
                                        <li style="text-align: right">
                                            <asp:Button runat="server" Text="Return to manage emails" ID="btnReturnManageEmailSettingTab"
                                                CausesValidation="False" OnClick="btnCancelOnForm_Click" class="returnButton">
                                            </asp:Button>
                                        </li>
                                    </ul>
                                    <table class="standardTable" border="1" cellpadding="3" cellspacing="0">
                                        <tr>
                                            <td class="tableTDRightMiddle" rowspan="3">
                                                When to send an email:
                                            </td>
                                            <td>
                                                <asp:RadioButton ID="rdSendImmediately" runat="server" AutoPostBack="True" GroupName="SendEmail"
                                                    OnCheckedChanged="rdSendImmediately_CheckedChanged" Checked="true" />Send immediately:
                                                The email will be sent as soon as the trigger occurs
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButton ID="rdSendAfterTrigger" runat="server" AutoPostBack="True" GroupName="SendEmail"
                                                    OnCheckedChanged="rdSendAfterTrigger_CheckedChanged" />Send after a trigger
                                                occurs: This determines the delay in sending the email after the trigger occurs
                                                (triggers such as status, action, assignment etc)
                                                <br />
                                                &nbsp;&nbsp;<%--<asp:RequiredFieldValidator ID="reqFldVldEmailTrigger" 
                                                    runat="server" ControlToValidate="txtDuration" Display="None" 
                                                    ErrorMessage="Enter duration." ValidationGroup="SendEmail"></asp:RequiredFieldValidator>
                                                <asp:ValidatorCalloutExtender ID="reqFldVldEmailTrigger_ValidatorCalloutExtender" 
                                                    runat="server" Enabled="True" TargetControlID="reqFldVldEmailTrigger">
                                                </asp:ValidatorCalloutExtender>--%><telerik:RadNumericTextBox ID="txtDuration" runat="server"
                                                    Width="50px" MinValue="0" MaxValue="999999999" ValidationGroup="SendEmail">
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                </telerik:RadNumericTextBox>
                                                <asp:DropDownList ID="ddlSpan" runat="server" Width="70px">
                                                    <asp:ListItem Value="0">Minutes</asp:ListItem>
                                                    <asp:ListItem Value="1">Hour</asp:ListItem>
                                                    <asp:ListItem Value="2">Days</asp:ListItem>
                                                    <asp:ListItem Value="3">Weeks</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButton ID="rdSendBeforeAfter" runat="server" AutoPostBack="True" GroupName="SendEmail"
                                                    OnCheckedChanged="rdSendBeforeAfter_CheckedChanged" />Send before or after a
                                                specific date: This determines the delay in sending the email from a selected date
                                                field.
                                                <%-- <asp:XmlDataSource ID="XmlDataSource1" runat="server" 
                                                    datafile="packages.config"></asp:XmlDataSource>
                                                <asp:TreeView ID="TreeView1" runat="server" datasourceid="XmlDataSource1">
                                                </asp:TreeView>--%>
                                                <br />
                                                &nbsp;&nbsp;<%--<asp:RequiredFieldValidator ID="reqFldVldEmailSendBeforeAfter" 
                                                    runat="server" ControlToValidate="txtDurationBeforeAfter" Display="None" 
                                                    ErrorMessage="Enter duration." ValidationGroup="SendEmail"></asp:RequiredFieldValidator>
                                                <asp:ValidatorCalloutExtender ID="reqFldVldEmailSendBeforeAfter_ValidatorCalloutExtender" 
                                                    runat="server" Enabled="True" TargetControlID="reqFldVldEmailSendBeforeAfter">
                                                </asp:ValidatorCalloutExtender>--%><telerik:RadNumericTextBox ID="txtDurationBeforeAfter"
                                                    runat="server" Width="50px" MinValue="0" MaxValue="999999999" ValidationGroup="SendEmail">
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                </telerik:RadNumericTextBox>
                                                <asp:DropDownList ID="ddlSpanBeforeAfter" runat="server" Width="70px">
                                                    <asp:ListItem Value="0">Minutes</asp:ListItem>
                                                    <asp:ListItem Value="1">Hour</asp:ListItem>
                                                    <asp:ListItem Value="2">Days</asp:ListItem>
                                                    <asp:ListItem Value="3">Weeks</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlTypeBeforeAfter" runat="server" Width="70px">
                                                    <asp:ListItem Value="0">Before</asp:ListItem>
                                                    <asp:ListItem Value="1">After</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlFieldsBeforeAfter" runat="server" DataTextField="Name" DataValueField="Key"
                                                    AppendDataBoundItems="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tableTDRightMiddle" style="width: 40%;">
                                                Cancel upon status change:
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkCancel" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                <fieldset class="condado">
                                    <legend>Filter Settings</legend>
                                    <table>
                                        <tr>
                                            <td style="width: 20%">
                                                <asp:RadioButtonList ID="rdBtnlstFilterSelection" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="rdBtnlstFilterSelection_SelectedIndexChanged" RepeatDirection="Horizontal"
                                                    Width="200px">
                                                    <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                                    <asp:ListItem Value="1">Any</asp:ListItem>
                                                    <asp:ListItem Value="2">Custom</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td style="width: 20%">
                                                <asp:TextBox ID="txtCustomFilter" runat="server" Enabled="False" OnTextChanged="txtCustomFilter_TextChanged"
                                                    AutoPostBack="True"></asp:TextBox>
                                            </td>
                                            <td>
                                                <uc3:StatusLabel ID="ctrlStatusCustomFilter" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                <uc2:ManageFilters ID="ManageFiltersControl" runat="server" />
                            </telerik:RadPageView>
                        </telerik:RadMultiPage>
                        <div class="buttons">
                            <table>
                                <tr>
                                    <td style="width: 40%;">
                                        <uc3:StatusLabel ID="lblMessageForm" runat="server" />
                                    </td>
                                    <td style="width: 60%;" class="tableTDLeftTop">
                                        <asp:Button ID="btnApply" runat="server" Text="Apply" OnClick="btnApply_Click" OnClientClick="validate();" ValidationGroup="emailValidation" />
                                        &nbsp;
                                        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"  ValidationGroup="emailValidation"
                                            Text="Submit" />
                                        &nbsp;
                                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                            Text="Cancel" class="returnButton" />
                                        <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                        <asp:HiddenField ID="hdnFieldEditEmailTemplateKey" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:PostBackTrigger ControlID="btnApply" />
                        <asp:PostBackTrigger ControlID="btnSubmit" />
                        <asp:PostBackTrigger ControlID="btnCancelOnForm" />
                        <asp:PostBackTrigger ControlID="rdSendImmediately" />
                        <asp:PostBackTrigger ControlID="rdSendAfterTrigger" />                        
                        <asp:PostBackTrigger ControlID="rdSendBeforeAfter" />--%>
                        <asp:PostBackTrigger ControlID="grdEmailAttachments" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
