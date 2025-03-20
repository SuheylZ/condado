<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageQuickLinks.aspx.cs" Inherits="Admin_ManageQuickLinks" %>

    <%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register src="../UserControls/SelectionLists.ascx" tagname="SelectionLists" tagprefix="uc4" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
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
                    <legend>Manage Quick Links</legend>
                    <div id="divToolbar" class="Toolbar">
                        <table class="standardTable">
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td style="width: 30%;">
                                                <asp:Button ID="btnAddNewQuickLink" runat="server" Text="Add New Quick Link" OnClick="btnAddNewQuickLink_Click"
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
                    <asp:GridView ID="grdQuickLinks" runat="server" Width="100%" OnPageIndexChanging="grdQuickLinks_PageIndexChanging"
                        OnSorting="grdQuickLinks_Sorting" OnRowCommand="grdQuickLinks_RowCommand" AutoGenerateColumns="False"
                        DataKeyNames="Key" AllowSorting="True" GridLines="None" AlternatingRowStyle-CssClass="alt"
                        CssClass="mGrid" OnRowDataBound="Grid_RowDataBound">
                        <AlternatingRowStyle CssClass="alt" />
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="Key" Visible="false"></asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="20%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Url" HeaderText="Url" SortExpression="Url">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="30%" />
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
                                     <asp:LinkButton ID="lnkAssignSkills" runat="server" CausesValidation="False" CommandName="AssignSkillsX"
                                        Text="Assign Skills" OnClientClick="" class="resetChangeFlag"></asp:LinkButton>
                                     <asp:Label runat="server" id="lblSep" Text="&nbsp;|&nbsp;" />
                                     <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                        Text="Delete" OnClientClick="if(confirm('Are you sure want to delete quick link?')== true) true; else return false;"></asp:LinkButton>
                                   
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
                <telerik:RadTabStrip ID="tlQuickLinksStrip" runat="server" MultiPageID="tabContQuickLinks"
                    CausesValidation="True" SelectedIndex="0" Skin="WebBlue" EnableAjaxSkinRendering="true">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Quick Link" PageViewID="tlPage1" Selected="True">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Assign Skills" PageViewID="tlPage2">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                  <!-- Start Waheed -->

                          <div class="buttons" style="text-align: right">
                         <asp:Button runat="server" Text="Return to Manage Quick Links" ID="btnReturn"
                                        OnClick="btnCancelOnForm_Click" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>

                                        <!-- End Waheed -->
                <telerik:RadMultiPage ID="tabContQuickLinks" runat="server" SelectedIndex="0">

                    <telerik:RadPageView ID="tlPage1" runat="server" Selected="true">
                        <fieldset id="fldSetForm" class="condado">
                           

                            <legend>Add/Edit Quick Link</legend>
                            <ul>
                                <li>
                                    <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" 
                                        Text="Name:"></asp:Label>
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="200" Width="200px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="regFldVldName" runat="server" ControlToValidate="txtName"
                                        Display="None" ErrorMessage="Enter quick link name."></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="regFldVldName_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="regFldVldName">
                                    </asp:ValidatorCalloutExtender>
                                </li>
                                <li>
                                    <asp:Label ID="lblDescription" runat="server" 
                                        AssociatedControlID="txtDescription" Text="Description:"></asp:Label>
                                    <asp:TextBox ID="txtDescription" runat="server" Width="400px"></asp:TextBox>
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
                                    <asp:Label ID="lblTarget" runat="server" AssociatedControlID="ddlTarget" 
                                        Text="Target:"></asp:Label>
                                    <asp:DropDownList ID="ddlTarget" runat="server" Width="200px">
                                        <asp:ListItem Selected="True" Value="0">_blank</asp:ListItem>
                                        <asp:ListItem Value="1">_parent</asp:ListItem>
                                        <asp:ListItem Value="2">_search</asp:ListItem>
                                        <asp:ListItem Value="3">_self</asp:ListItem>
                                        <asp:ListItem Value="4">_top</asp:ListItem>
                                    </asp:DropDownList>
                                </li>
                                <li>
                                    <asp:Label ID="lblAlertBox" runat="server" AssociatedControlID="chkAlertBox" 
                                        Text="Alert Box:"></asp:Label>
                                    <asp:CheckBox ID="chkAlertBox" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblMessage" runat="server" AssociatedControlID="txtMessage" 
                                        Text="Message:"></asp:Label>
                                    <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="200px" Width="400px" 
                                        ></asp:TextBox>
 <asp:RequiredFieldValidator ID="reqFldVldBodyText" runat="server" ControlToValidate="txtMessage"
                                        Display="None" ErrorMessage="Enter message text."></asp:RequiredFieldValidator>
                                    <asp:ValidatorCalloutExtender ID="reqFldVldBodyText_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="reqFldVldBodyText">
                                    </asp:ValidatorCalloutExtender>       
                                </li>                             
                            </ul>
                            &nbsp;</fieldset>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPage2" runat="server">
                        <uc4:SelectionLists ID="ctlSkillGroupsAssignment" runat="server" Title="Assigned Skill Groups" TitleAvailable="Skill Groups Not Assigned " TitleSelected="Assigned Skill Groups" />                                              
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
                                <asp:HiddenField ID="hdnFieldEditRecordKey" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
