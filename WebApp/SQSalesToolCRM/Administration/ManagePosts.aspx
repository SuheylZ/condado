<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManagePosts.aspx.cs" Inherits="Admin_ManagePosts" ValidateRequest="false" %>

    <%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
 <!-- start waheed -->
        <script type="text/javascript">            Sys.Application.add_load(bindEvents);
</script>

<!-- end waheed -->
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
       
            <div id="divGrid" runat="server">
                <fieldset class="condado">
                    <legend>Manage Posts</legend>
                    <div id="divToolbar" class="Toolbar">
                   
                        <table class="standardTable">
                            <tr>
                                <td>
                                    <table >
                                        <tr>
                                            <td style="width: 30%;">
                                                <asp:Button ID="btnAddNewPost" runat="server" Text="Add New Post" OnClick="btnAddNewPost_Click"
                                                    CausesValidation="False" class="resetChangeFlag"/>
                                            </td>
                                            <td style="width: 70%;">
                                                <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="tableTDLeftMiddle">
                                    <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:HiddenField ID="hdSortColumn" runat="server" />
                    <asp:HiddenField ID="hdSortDirection" runat="server" />
                    <asp:GridView ID="grdPosts" runat="server" Width="100%" OnPageIndexChanging="grdPostTemplates_PageIndexChanging"
                        OnSorting="grdPostTemplates_Sorting" OnRowCommand="grdPostTemplates_RowCommand"
                        AutoGenerateColumns="False" DataKeyNames="PostKey" AllowSorting="True" GridLines="None"
                        AlternatingRowStyle-CssClass="alt" CssClass="mGrid" OnRowDataBound="Grid_RowDataBound">
                        <AlternatingRowStyle CssClass="alt" />
                        <Columns>
                            <asp:BoundField DataField="PostKey" HeaderText="Key" Visible="false"></asp:BoundField>
                            <asp:BoundField DataField="PostTitle" HeaderText="Title" SortExpression="PostTitle">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="40%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="PostUrl" HeaderText="Url" SortExpression="PostUrl">
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
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                        Text="Edit" class="resetChangeFlag"></asp:LinkButton>
                                    |
                                    <asp:LinkButton ID="lnkSettings" runat="server" CausesValidation="False" CommandName="SettingX"
                                        Text="Settings" OnClientClick="" class="resetChangeFlag"/>
                                    <asp:Label runat="server" id="lblSepDel" Text="&nbsp;|&nbsp;" />
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                        Text="Delete" OnClientClick="if(confirm('Are you sure want to delete post?')== true) true; else return false;"/>
                                </ItemTemplate>
                                <ItemStyle Width="15%" HorizontalAlign="Center" />
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
            </div>
            <div id="divForm" runat="server">
                <telerik:RadTabStrip ID="tlEmailTemplateStrip" runat="server" MultiPageID="tabContPostTemplate"
                    CausesValidation="True" SelectedIndex="0" Skin="WebBlue">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Compose Post" PageViewID="tlPage1" Selected="True">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Delivery/Drip Settings" PageViewID="tlPage2">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tabContPostTemplate" runat="server" SelectedIndex="0">
                    <telerik:RadPageView ID="tlPage1" runat="server" Selected="true">
                        <fieldset id="fldSetForm" class="condado">
                            <legend>Add/Edit Post</legend>
                            <ul>
                                <li style="text-align: right">
                                    <asp:Button runat="server" Text="Return to manage posts" ID="btnReturnToManageEmails"
                                        CausesValidation="False" OnClick="btnCancelOnForm_Click" class="returnButton"></asp:Button>
                                </li>
                                <li style="text-align: right"><a id="popupTags" onclick="javascript:window.open('TagFields.aspx', 'pop', 'width=300,height=600');">
                                    View Field Tags</a> </li>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title:"></asp:Label>
                                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="200" Width="400px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="regFldVldTitle" runat="server" ControlToValidate="txtTitle"
                                        Display="None" ErrorMessage="Enter post title."></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="regFldVldTitle_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="regFldVldTitle">
                                    </asp:ValidatorCalloutExtender>
                                </li>
                                <li>
                                    <asp:Label ID="lblUrl" runat="server" AssociatedControlID="txtUrl" Text="Url:"></asp:Label>
                                    <asp:TextBox ID="txtUrl" runat="server" Width="400px" MaxLength="300"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqFldVldUrl" runat="server" ControlToValidate="txtUrl"
                                        Display="None" ErrorMessage="Enter post url."></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExVld" runat="server" ControlToValidate="txtUrl"
                                        Display="None" ErrorMessage="Enter valid url." ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?"></asp:RegularExpressionValidator>
                                    <asp:ValidatorCalloutExtender ID="regExVld_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="regExVld">
                                    </asp:ValidatorCalloutExtender>
                                </li>
                                <li>
                                    <asp:ValidatorCalloutExtender ID="reqFldVldUrl_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="reqFldVldUrl">
                                    </asp:ValidatorCalloutExtender>
                                    <asp:Label ID="lblType" runat="server" AssociatedControlID="ddlType" Text="Type:"></asp:Label>
                                    <asp:DropDownList ID="ddlType" runat="server">
                                        <asp:ListItem Selected="True" Value="0">XML Webservice POST</asp:ListItem>
                                        <asp:ListItem Value="1">URL-encoded POST</asp:ListItem>
                                        <asp:ListItem Value="2">URL-encoded GET</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
                                <li>
                                    <asp:Label ID="lblHeader" runat="server" AssociatedControlID="txtHeader" Text="Header (Optional):"></asp:Label>
                                    <asp:TextBox ID="txtHeader" runat="server" MaxLength="200" Width="400px"></asp:TextBox>
                                </li>
                                <li>
                                    <asp:Label ID="lblSuccessResponse" runat="server" AssociatedControlID="txtSuccessResponse"
                                        Text="Success Regex Response (Optional):"></asp:Label>
                                    <asp:TextBox ID="txtSuccessResponse" runat="server" MaxLength="200" Width="400px"></asp:TextBox>
                                </li>
                                <li class="header">Body</li>
                                <li>
                                    <asp:RequiredFieldValidator ID="reqFldVldBodyText" runat="server" ControlToValidate="txtTemplateBody"
                                        Display="None" ErrorMessage="Enter post body text."></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="reqFldVldBodyText_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" PopupPosition="BottomLeft" TargetControlID="reqFldVldBodyText">
                                    </asp:ValidatorCalloutExtender>                                   
                                    <asp:TextBox ID="txtTemplateBody" runat="server" Height="250px" 
                                        TextMode="MultiLine" Width="100%"></asp:TextBox>
                                </li>
                            </ul>
                            &nbsp;</fieldset>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPage2" runat="server">
                        <fieldset class="condado">
                            <legend>Delayed Sending (Drip Marketing): </legend>
                            <ul>
                                <li style="text-align: right">
                                    <asp:Button runat="server" Text="Return to manage posts" ID="btnReturnManageEmailSettingTab"
                                        CausesValidation="False" OnClick="btnCancelOnForm_Click" class="returnButton"></asp:Button>
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
                                        &nbsp;&nbsp;<telerik:RadNumericTextBox ID="txtDuration" runat="server" Width="50px"
                                            MinValue="0" MaxValue="999999999" ValidationGroup="SendEmail">
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
                                        <br />
                                        &nbsp;&nbsp;<telerik:RadNumericTextBox ID="txtDurationBeforeAfter" runat="server"
                                            Width="50px" MinValue="0" MaxValue="999999999" ValidationGroup="SendEmail">
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
                    
                    <table >
                        <tr>
                            <td style="width: 40%;">
                                <uc3:StatusLabel ID="lblMessageForm" runat="server" />
                            </td>
                            <td style="width: 60%;" class="tableTDLeftTop">
                                <asp:Button ID="btnApply" runat="server" Text="Apply" OnClick="btnApply_Click" OnClientClick="validate();" />
                                &nbsp;
                                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validate();"
                                    Text="Submit" />
                                &nbsp;
                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                    Text="Cancel" class="returnButton" />
                                <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldEditPostTemplateKey" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
