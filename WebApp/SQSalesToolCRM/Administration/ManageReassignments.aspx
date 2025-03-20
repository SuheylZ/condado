<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageReassignments.aspx.cs" Inherits="Admin_ManageReassignments" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/Schedule.ascx" TagName="Schedule" TagPrefix="uc4" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
    <script language="javascript" type="text/javascript">
        function fnProcessConsent(arg) {
            // 666_1: true  666_0: false
            if (arg === true)
                <%=ClientScript.GetPostBackEventReference(new PostBackOptions(btnApply, "666_1"))%>;
            else if (arg === false)
                <%=ClientScript.GetPostBackEventReference(new PostBackOptions(btnApply, "666_0"))%>;


        }
        function IsStatusChanged() {
            $("#<%=rm_change_status.ClientID%>").toggle();
        }
        function isIncludeSubStatusChange() {
            $('#<%=rm_sub_status.ClientID%>').toggle();
        }
        function ddlrmType_selectedIndexChanged(s, e) {
            var index = e.get_index();

            switch (index) {
                case 1:
                    $('#<%=rm_type_user.ClientID%>').show();
                    $('#<%=rm_type_skillgroup.ClientID%>').hide();
                    break;
                case 2:
                    $('#<%=rm_type_user.ClientID%>').hide();
                    $('#<%=rm_type_skillgroup.ClientID%>').show();
                    break;
                default:
                    $('#<%=rm_type_user.ClientID%>').hide();
                    $('#<%=rm_type_skillgroup.ClientID%>').hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="hdnProcessState" runat="server" Value="0" />
            <asp:HiddenField ID="hdnProcessInitiatorId" runat="server" Value="" />
            <asp:HiddenField ID="hdnProcessInitiatorOldState" runat="server" Value="" />

            <div id="divGrid" runat="server">
                <fieldset class="condado">
                    <legend>Manage Reassignment Rules</legend>
                    <div id="divToolbar" class="Toolbar">
                        <table>
                            <tr>
                                <td style="width: 30%;">
                                    <asp:Button ID="btnAddNewQueue" runat="server" Text="Add New Queue" CausesValidation="False"
                                        OnClick="btnAddNewQueue_Click" />
                                    <asp:Button ID="btnPVRefresh" runat="server" Text="Refresh" CausesValidation="False"
                                        OnClick="btnPVRefresh_Click" />
                                </td>
                                <td style="width: 25%;">Reassignment Management Type&nbsp;
                                      <asp:DropDownList ID="rm_type" runat="server" DataValueField="id" DataTextField="name" Width="127px" AutoPostBack="true">
                                          <asp:ListItem Text="--All--" Value="-1" />
                                          <asp:ListItem Text="User" Value="1" />
                                          <asp:ListItem Text="Skill Group" Value="2" />
                                      </asp:DropDownList>
                                    <uc3:StatusLabel ID="StatusLabel1" runat="server" />
                                </td>
                                <td style="width: 70%;">User Type&nbsp;
                                      <asp:DropDownList ID="ddlFilterByUserType" runat="server" DataValueField="id" DataTextField="name" Width="127px" AutoPostBack="true">
                                          <asp:ListItem Text="--All--" Value="-1" />
                                          <asp:ListItem Text="Assigned User" Value="1" />
                                          <asp:ListItem Text="CSR User" Value="2" />
                                          <asp:ListItem Text="Transfer Agent User" Value="3" />
                                          <asp:ListItem Text="Alternate Product User" Value="4" />
                                          <asp:ListItem Text="OnBoard User" Value="5" />
                                      </asp:DropDownList>
                                    <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <telerik:RadGrid ID="grdRassignment" runat="server" Skin="" CssClass="mGrid" Width="100%"
                        CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                        AutoGenerateColumns="False" OnItemDataBound="grdRassignment_ItemDataBound"
                        OnRowDrop="grdRassignment_RowDrop" OnItemCommand="grdRassignment_ItemCommand"
                        SelectedItemStyle-CssClass="gridHeader">
                        <AlternatingItemStyle CssClass="alt" />
                        <ClientSettings AllowRowsDragDrop="True">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>

                        </ClientSettings>
                        <MasterTableView DataKeyNames="Id,Priority">
                            <NoRecordsTemplate>
                                There is no reassignment record.
                            </NoRecordsTemplate>
                            <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                                ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                                ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                                ExportToPdfText="Export to PDF" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                            <RowIndicatorColumn Visible="True" FilterControlAltText="Priority Rule RowIndicator column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </RowIndicatorColumn>
                            <ExpandCollapseColumn Visible="True" FilterControlAltText="Reassignment Rule ExpandColumn column">
                                <HeaderStyle Width="20px"></HeaderStyle>
                            </ExpandCollapseColumn>
                            <Columns>
                                <telerik:GridTemplateColumn FilterControlAltText="Reassignment record up/down" UniqueName=""
                                    HeaderText="Priority">
                                    <ItemTemplate>

                                        <asp:ImageButton ID="imgBtnUpOrder" runat="server" CommandName="UpOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/UpArrow.png" />
                                        <asp:ImageButton ID="imgBtnDownOrder" runat="server" CommandName="DownOrder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            ImageUrl="~/Images/DownArrow.png" />
                                    </ItemTemplate>
                                    <ItemStyle Width="6%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>

                                 <telerik:GridBoundColumn DataField="RM_Type_Name" 
                                    HeaderText="Reassignment Type" UniqueName="RM_Type_Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn> 
                                
                                <telerik:GridBoundColumn DataField="SkillName" 
                                    HeaderText="Skill Group Name" UniqueName="SkillName">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn>

                                <%--SZ: Apr 21, 2014: added after the client request on call the same day--%>
                                <telerik:GridTemplateColumn HeaderText="User Type" UniqueName="type" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTmpType" runat="server" Text='<%# GetUserType(DataBinder.Eval(Container.DataItem, "UserType"))%>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                
                               
                                <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="Reassignment Rule User column"
                                    HeaderText="Reassignment User" UniqueName="UserName">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn>


                                <telerik:GridBoundColumn DataField="Title" FilterControlAltText="Reassignment Rule Title column"
                                    HeaderText="Title" UniqueName="Title">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="LastRun"
                                    HeaderText="Last Run" UniqueName="LastRun">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                </telerik:GridBoundColumn>

                                <telerik:GridTemplateColumn FilterControlAltText="Reassignment record enabled" UniqueName="Enabled"
                                    HeaderText="Enabled">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEnabled" runat="server" CausesValidation="False" CommandName="EnabledX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsActive"))? "Yes": "No" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn FilterControlAltText="Reassignment options column" UniqueName="options"
                                    HeaderText="Options">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Edit"></asp:LinkButton>
                                        |
                                        <asp:LinkButton ID="lnkCopy" runat="server" CausesValidation="False" Text="Copy"
                                            CommandName="CopyX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                                        <asp:Label runat="server" ID="lblSepDel" Text="&nbsp;|&nbsp;" />
                                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                            Text="Delete" OnClientClick="if(confirm('Are you sure want to delete record?')== true) true; else return false;"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="15%" HorizontalAlign="Center" />
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <EditFormSettings>
                                <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                </EditColumn>
                            </EditFormSettings>
                        </MasterTableView>
                        <HeaderStyle CssClass="gridHeader" />
                        <FilterMenu EnableImageSprites="False">
                        </FilterMenu>
                    </telerik:RadGrid>
                </fieldset>
            </div>
            <div id="divForm" runat="server">
                <div class="buttons" style="text-align: right" id="returnButton">
                    <asp:Button runat="server" Text="Return to Queues" ID="btnReturn"
                        OnClick="btnCancelOnForm_Click" CausesValidation="False" class="returnButton"></asp:Button>
                </div>
                <telerik:RadTabStrip ID="tlPrioritizationStrip" runat="server" SelectedIndex="1"
                    Skin="WebBlue" MultiPageID="tlMultiPage"
                    OnTabClick="tlPrioritizationStrip_TabClick">
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Queue Details" PageViewID="tlPageQueueDetails" Owner="tlPrioritizationStrip">
                        </telerik:RadTab>
                        <%--Commented on John's Request
                            <telerik:RadTab runat="server" Text="Schedule: What does it run?" PageViewID="tlPageSchedule"
                            Owner="tlPrioritizationStrip" Selected="True">
                        </telerik:RadTab>--%>
                        <telerik:RadTab runat="server" Text="Lead Filters" PageViewID="tlPageFilters" Owner="tlPrioritizationStrip">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tlMultiPage" runat="server" SelectedIndex="1">
                    <telerik:RadPageView ID="tlPageQueueDetails" runat="server">
                        <fieldset id="fldSetForm" class="condado">
                            <legend>Add/Edit Reassignment</legend>
                            <ul>

                                <li>
                                    <asp:Label runat="server" AssociatedControlID="ddlrmType" Text="Reassignment Management Type" />
                                    <telerik:RadDropDownList ID="ddlrmType" runat="server" Width="230px" OnClientSelectedIndexChanged="ddlrmType_selectedIndexChanged">
                                        <Items>
                                            <telerik:DropDownListItem Text="--Unassigned--" Value="" />
                                            <telerik:DropDownListItem Text="User" Value="1" />
                                            <telerik:DropDownListItem Text="Skill Group" Value="2" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </li>
                                <li runat="server" id="rm_type_user">
                                    <asp:Label ID="lblrmTypeUser" runat="server" AssociatedControlID="ddlUsers" Text="Reassignment User" />
                                    <%--<telerik:RadDropDownList ID="ddlUsers" runat="server" DataTextField="FullName" DataValueField="Key" Width="230px" />--%>
                                    <telerik:RadComboBox ID="ddlUsers" DropDownAutoWidth="Enabled" EnableTextSelection="True"  DataTextField="Name" DataValueField="Value" runat="server" Width="205px" Filter="Contains" AllowCustomText="True"  MarkFirstMatch="true"  >
                                    </telerik:RadComboBox>
                                </li>

                                <li runat="server" id="rm_type_skillgroup">
                                    <asp:Label ID="lblrmTypeSkillGroup" runat="server" AssociatedControlID="ddlSkillGroup" Text="Skill Group" />
                                    <telerik:RadComboBox ID="ddlSkillGroup" runat="server" Width="205px" Filter="Contains"  />
                                </li>
                                <li>
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlUserType" Text="User Type:" />
                                    <telerik:RadDropDownList ID="ddlUserType" runat="server" DataValueField="id" DataTextField="name" Width="230px">
                                        <Items>
                                            <telerik:DropDownListItem Text="--Unassigned--" Value="-1" />
                                            <telerik:DropDownListItem Text="Assigned User" Value="1" />
                                            <telerik:DropDownListItem Text="CSR User" Value="2" />
                                            <telerik:DropDownListItem Text="Transfer Agent User" Value="3" />
                                            <telerik:DropDownListItem Text="Alternate Product User" Value="4" />
                                            <telerik:DropDownListItem Text="OnBoard User" Value="5" />
                                        </Items>

                                    </telerik:RadDropDownList>
                                </li>
                                <li>
                                    <asp:Label ID="lblSchedule" runat="server" AssociatedControlID="ddlSchedule" Text="Schedule:" />
                                    <telerik:RadDropDownList ID="ddlSchedule" runat="server" Width="230px">
                                        <Items>
                                            <telerik:DropDownListItem Text="--Select Schedule--" Value="1" />
                                            <telerik:DropDownListItem Text="Every 30 Minutes" Value="2" />
                                            <telerik:DropDownListItem Text="Every Hour" Value="3" />
                                            <telerik:DropDownListItem Text="12:00 AM" Value="4" />
                                            <telerik:DropDownListItem Text="1:00 AM" Value="5" />
                                            <telerik:DropDownListItem Text="2:00 AM" Value="6" />
                                            <telerik:DropDownListItem Text="3:00 AM" Value="7" />
                                            <telerik:DropDownListItem Text="4:00 AM" Value="8" />
                                            <telerik:DropDownListItem Text="5:00 AM" Value="9" />
                                            <telerik:DropDownListItem Text="6:00 AM" Value="10" />
                                            <telerik:DropDownListItem Text="7:00 AM" Value="11" />
                                            <telerik:DropDownListItem Text="8:00 AM" Value="12" />
                                            <telerik:DropDownListItem Text="9:00 AM" Value="13" />
                                            <telerik:DropDownListItem Text="10:00 AM" Value="14" />
                                            <telerik:DropDownListItem Text="11:00 AM" Value="15" />
                                            <telerik:DropDownListItem Text="12:00 PM" Value="16" />
                                            <telerik:DropDownListItem Text="1:00 PM" Value="17" />
                                            <telerik:DropDownListItem Text="2:00 PM" Value="18" />
                                            <telerik:DropDownListItem Text="3:00 PM" Value="19" />
                                            <telerik:DropDownListItem Text="4:00 PM" Value="20" />
                                            <telerik:DropDownListItem Text="5:00 PM" Value="21" />
                                            <telerik:DropDownListItem Text="6:00 PM" Value="22" />
                                            <telerik:DropDownListItem Text="7:00 PM" Value="23" />
                                            <telerik:DropDownListItem Text="8:00 PM" Value="24" />
                                            <telerik:DropDownListItem Text="9:00 PM" Value="25" />
                                            <telerik:DropDownListItem Text="10:00 PM" Value="26" />
                                            <telerik:DropDownListItem Text="11:00 PM" Value="27" />
                                        </Items>

                                    </telerik:RadDropDownList>
                                </li>
                                <li>
                                    <asp:Label  runat="server" Text="WEB GAL Caps" AssociatedControlID="chk_webGalCap"></asp:Label>
                                    <asp:CheckBox runat="server" ID="chk_webGalCap"  />
                                </li>
                                <li>
                                    <asp:Label  runat="server" Text="State Licensing" AssociatedControlID="chk_StateLicense"></asp:Label>
                                    <asp:CheckBox runat="server" ID="chk_StateLicense"  />
                                </li>
                                <li>
                                    <asp:Label runat="server" Text="Change Status" AssociatedControlID="chk_ChangeStatus"></asp:Label>
                                    <asp:CheckBox runat="server" ID="chk_ChangeStatus" onchange="IsStatusChanged()" />
                                </li>
                                <div runat="server" id="rm_change_status">
                                    <li>
                                        <asp:Label runat="server" Text="Status" ID="lbl_leadStatus" AssociatedControlID="ddlLeadStatus"></asp:Label>
                                        <telerik:RadComboBox runat="server" ID="ddlLeadStatus" Width="205px" Filter="Contains">
                                        </telerik:RadComboBox>
                                    </li>

                                    <li style="margin-top: 2px">
                                        <asp:Label ID="lbl_inl_subStatus" runat="server" Text="Include Sub Status?" AssociatedControlID="chk_inl_subStatus"></asp:Label>
                                        <asp:CheckBox runat="server" ID="chk_inl_subStatus" onchange="isIncludeSubStatusChange()" />
                                    </li>
                                    <li id="rm_sub_status" runat="server">
                                        <asp:Label runat="server" Text="Sub Status" ID="lbl_subStatus" AssociatedControlID="ddlSubStatus"></asp:Label>
                                        <telerik:RadComboBox runat="server" ID="ddlSubStatus" Width="205px" Filter="Contains">
                                        </telerik:RadComboBox>
                                    </li>
                                </div>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Title:"></asp:Label><telerik:RadTextBox
                                        ID="txtTitle" runat="server" Width="230px">
                                    </telerik:RadTextBox><asp:RequiredFieldValidator
                                        ID="reqFldVldTitle" runat="server" ControlToValidate="txtTitle" Display="None"
                                        ErrorMessage="Enter title."></asp:RequiredFieldValidator></li>
                                <li>
                                    <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription"
                                        Text="Description:"></asp:Label><asp:TextBox ID="txtDescription" runat="server" Height="100px"
                                            TextMode="MultiLine" Width="400px"></asp:TextBox></li>
                                <li>
                                    <asp:ValidatorCalloutExtender ID="reqFldVldTitle_ValidatorCalloutExtender" runat="server"
                                        Enabled="True" TargetControlID="reqFldVldTitle">
                                    </asp:ValidatorCalloutExtender>
                                    <asp:Label ID="lblEnabled" runat="server" AssociatedControlID="chkEnabled" Text="Enabled:"></asp:Label><asp:CheckBox
                                        ID="chkEnabled" runat="server" /></li>
                            </ul>
                        </fieldset>
                    </telerik:RadPageView>
                    <%--Commented on John's Request
                        <telerik:RadPageView ID="tlPageSchedule" runat="server">
                        <uc4:Schedule ID="ctrlShiftSchedule" runat="server" />
                    </telerik:RadPageView>--%>
                    <telerik:RadPageView ID="tlPageFilters" runat="server">
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
                                <asp:Button ID="btnApply" runat="server" Text="Save" OnClick="btnApply_Click" OnClientClick="validate();" />
                                &nbsp;
                                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validate();"
                                    Text="Save &amp; Close" />
                                &nbsp;
                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                    Text="Close" />
                                <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldIsCopyMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
