<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    EnableEventValidation="true" CodeFile="ManageCampaign.aspx.cs" Inherits="Admin_ManageCampaign" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">        Sys.Application.add_load(bindEvents);
    </script>
    <div class="mainFormDiv">
        <asp:UpdatePanel ID="updatePanelMain" runat="server">
            <ContentTemplate>
                <telerik:RadDatePicker ID="RadDatePicker1" runat="server" Visible="false">
                </telerik:RadDatePicker>
                <div runat="server" id="divGrid" visible="true" class="Toolbar">
                    <asp:UpdatePanel ID="UpdatePanelGrid" runat="server">
                        <ContentTemplate>
                            <fieldset id="fldSetGrid" class="condado">
                                <legend>Manage Campaigns</legend>
                                <table>
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td width="30%">
                                                        <asp:Button ID="btnAddNewCampaign" runat="server" Text="Add New Campaign" OnClick="btnAddNewCampaign_Click"
                                                            CausesValidation="False" class="resetChangeFlag" />
                                                    </td>
                                                    <td>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <uc3:StatusLabel ID="lblMessageGrid" runat="server" />
                                                        <%--<asp:Label ID="lblMessageGrid" runat="server" CssClass="LabelMessage"></asp:Label>--%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" OnIndexChanged="Evt_Paging_Event" OnSizeChanged="Evt_Paging_Event" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                         
                                            <telerik:RadGrid ID="grdCampaign" runat="server" Skin="" CssClass="mGrid" Width="100%"
                                                CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();" AllowSorting="true"
                                                AutoGenerateColumns="False" OnItemCommand="grdCampaign_RowCommand" OnSortCommand="grdCampaign_SortGrid"
                                                SelectedItemStyle-CssClass="gridHeader" OnItemDataBound="grdCampaign_ItemDataBound">
                                                <AlternatingItemStyle CssClass="alt" />
                                                <MasterTableView DataKeyNames="CampaignID">
                                                    <NoRecordsTemplate>
                                                        No Campaign found.
                                                    </NoRecordsTemplate>
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="CampaignID" FilterControlAltText="ID" HeaderText="ID" SortExpression="CampaignID"
                                                            UniqueName="CampaignID">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="CampaignTitle" FilterControlAltText="CampaignTitle" SortExpression="CampaignTitle"
                                                            HeaderText="Title" UniqueName="CampaignTitle">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" Width="60%" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridTemplateColumn UniqueName="CampaignActive" HeaderText="Active" SortExpression="CampaignActive">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lnkActive" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CampaignActive") %>'
                                                                    Text='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "CampaignActive"))? "Active": "Inactive" %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="options" HeaderText="Options">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>'
                                                                    Text="Edit" class="resetChangeFlag"></asp:LinkButton>
                                                                <asp:Label runat="server" ID="lblMenuSep" Text="|" />
                                                                <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>'
                                                                    Text="Delete" OnClientClick="if(confirm('Are you sure want to delete campaign?')== true){ return true;} else { return false;}"></asp:LinkButton>
                                                            </ItemTemplate>
                                                             <HeaderStyle HorizontalAlign="Center" />
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
                                            <asp:HiddenField ID="hdSortColumn" runat="server" />
                                            <asp:HiddenField ID="hdSortDirection" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAddNewCampaign" EventName="Click" />
                            <asp:PostBackTrigger ControlID="grdCampaign" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div id="tabDiv" runat="server" visible="false">
                    <telerik:RadTabStrip ID="tlCampaignStrip" runat="server" MultiPageID="tlMultipage"
                        CausesValidation="False" Skin="WebBlue" SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab runat="server" Text="Campaign" PageViewID="pgCampaign" Selected="True" />
                            <telerik:RadTab runat="server" Text="Campaign Cost" PageViewID="pgCampaignCost" Enabled="false" />
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage ID="tlMultipage" runat="server" SelectedIndex="0">
                        <telerik:RadPageView ID="pgCampaign" runat="server" Selected="true">
                            <div runat="server" id="divForm" visible="true">
                                <asp:UpdatePanel ID="updatePanelForm" runat="server">
                                    <ContentTemplate>
                                        <fieldset id="fldSetForm" class="condado">
                                            <div class="buttons" style="text-align: right">
                                                <asp:Button runat="server" Text="Return to Manage Campaigns" ID="btnReturn" OnClick="btnCancelOnForm_Click"
                                                    CausesValidation="False" class="returnButton"></asp:Button>
                                            </div>
                                            <legend>Add/Edit Campaigns</legend>
                                            <ul>
                                                <li class="Header">
                                                    <asp:Label ID="lblAddEditCampaign" runat="server" Text="Add Campaign" CssClass="HeadingText"
                                                        Visible="false"></asp:Label>
                                                </li>
                                                <li id="trCampaignID" runat="server">
                                                    <asp:Label ID="lblCampaignID" runat="server" AssociatedControlID="txtCampaignID"
                                                        Text="Campaign ID"></asp:Label>
                                                    <asp:TextBox ID="txtCampaignID" runat="server" Enabled="False" Width="200px"></asp:TextBox>
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" Text="Campaign Name"></asp:Label>
                                                    <asp:TextBox ID="txtTitle" runat="server" Width="200px" ValidationGroup="gpCampaignValidation"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="reqFldTitle" runat="server" ControlToValidate="txtTitle"
                                                        Display="None" ErrorMessage="Enter Campaign Title" ForeColor="#CC0000" Width="200px"></asp:RequiredFieldValidator>
                                                    <asp:ValidatorCalloutExtender ID="reqFldTitle_ValidatorCalloutExtender" runat="server"
                                                        Enabled="True" TargetControlID="reqFldTitle" Width="250px">
                                                    </asp:ValidatorCalloutExtender>
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblAlternateTitle" runat="server" AssociatedControlID="txtAlternateTitle"
                                                        Text="Short Description"></asp:Label>
                                                    <asp:TextBox ID="txtAlternateTitle" runat="server" MaxLength="200" Width="200px"></asp:TextBox>
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblLongDescrip" runat="server" Text="Long Description" AssociatedControlID="tlEditor" />
                                                    <span id="input">
                                                        <%--  ContentAreaMode="Iframe" --%>
                                                        <telerik:RadEditor ID="tlEditor" runat="server" Skin="WebBlue" Height="180px" Width="400px"
                                                            EditModes="Design" BackColor="#EAEAEA" AutoResizeHeight="false" BorderStyle="None"
                                                            BorderWidth="0" ContentAreaCssFile="~/App_Themes/Default/condado.radeditor.css"
                                                            ContentAreaMode="Iframe">
                                                            <Tools>
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
                                                            </Tools>
                                                            <Content>
                                                            </Content>
                                                            <TrackChangesSettings CanAcceptTrackChanges="True" />
                                                        </telerik:RadEditor>
                                                    </span>
                                                    <asp:RequiredFieldValidator ID="vldLongDescription" runat="server" ValidationGroup="gpCampaignValidation"
                                                        Display="None" ErrorMessage="Long description is required" ControlToValidate="tlEditor"></asp:RequiredFieldValidator>
                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" Enabled="True"
                                                        TargetControlID="vldLongDescription" Width="250px" />
                                                </li>
                                                <li>
                                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="ddlOutpulseType" Text="Outpulse Type:"></asp:Label>
                                                    <asp:DropDownList ID="ddlOutpulseType" runat="server" AppendDataBoundItems="True"
                                                        DataTextField="Text" DataValueField="Key" Height="21px" Width="123px">
                                                        <asp:ListItem Value="0">General</asp:ListItem>
                                                        <asp:ListItem Value="1">Personal</asp:ListItem>
                                                    </asp:DropDownList>
                                           
                                                </li>
                                                <li>
                                                    <asp:Label ID="Label3" runat="server" AssociatedControlID="txtOutpulseId" Text="Outpulse ID:"></asp:Label>
                                                    <asp:TextBox ID="txtOutpulseId" runat="server" Width="200px" ValidationGroup="gpCampaignValidation" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtOutpulseId"
                                                        Display="None" ErrorMessage="Enter Outpulse Id" ForeColor="#CC0000" Width="200px" ValidationGroup="gpCampaignValidation" ></asp:RequiredFieldValidator>
                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server" Enabled="True"
                                                        TargetControlID="RequiredFieldValidator3" Width="250px">
                                                    </asp:ValidatorCalloutExtender>
                                                </li>
                                                <li>
                                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlCompanyKey" Text="Company Association"></asp:Label>
                                                    <asp:DropDownList ID="ddlCompanyKey" runat="server" AppendDataBoundItems="True" DataTextField="Text" ValidationGroup="gpCampaignValidation"
                                                        DataValueField="ID" Height="21px" Width="123px">
                                                        <asp:ListItem Value="-1">-- Select Key --</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlCompanyKey"
                                                        Display="None" ErrorMessage="Select company key." InitialValue="-1" ValidationGroup="gpCampaignValidation"></asp:RequiredFieldValidator>
                                                    <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" Enabled="True"
                                                        TargetControlID="RequiredFieldValidator1">
                                                    </asp:ValidatorCalloutExtender>
                                                </li>
                                                                                              
                                                <li>
                                                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email:"></asp:Label>
                                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="200" Width="200px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="reqexVldEmail" runat="server" ControlToValidate="txtEmail"
                                                        Display="None" ErrorMessage="Enter Email in proper format." ForeColor="#CC0000"
                                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                                    <asp:ValidatorCalloutExtender ID="reqexVldEmail_ValidatorCalloutExtender" runat="server"
                                                        Enabled="True" TargetControlID="reqexVldEmail" Width="250px">
                                                    </asp:ValidatorCalloutExtender>
                                                </li>
                                                 <li>
                                                    <asp:Label ID="lblArcMap" runat="server" AssociatedControlID="txtArcMap" Text="ARC Campaign Map:" ></asp:Label>
                                                    <asp:TextBox ID="txtArcMap" runat="server" MaxLength="25" Width="200px" ValidationGroup="gpCampaignValidation"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ID="arcMapVld" ValidationGroup="gpCampaignValidation" ControlToValidate="txtArcMap" ErrorMessage="ARC Map field is required" Display="None"  ForeColor="#CC0000" ></asp:RequiredFieldValidator>
                                                    <asp:ValidatorCalloutExtender ID="arcMapVld_ValidatorCalloutExtender" runat="server"
                                                        TargetControlID="arcMapVld" Width="250px">
                                                    </asp:ValidatorCalloutExtender>
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblDteType" runat="server" AssociatedControlID="chkDTE" Text ="DTE" />
                                                    <asp:CheckBox ID="chkDTE" runat="server" Checked="false" />
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblNote" runat="server" AssociatedControlID="txtNote" Text="Notes"></asp:Label>
                                                    <asp:TextBox ID="txtNote" runat="server" Height="78px" TextMode="MultiLine" Width="426px"></asp:TextBox>
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblActive" runat="server" AssociatedControlID="chkActive" Text="Active:"></asp:Label>
                                                    <asp:CheckBox ID="chkActive" runat="server" />
                                                </li>
                                                <li class="buttons">
                                                    <asp:Label ID="lblMessageForm" runat="server" CssClass="LabelMessage"></asp:Label>
                                                    <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" OnClick="btnDelete_Click"
                                                        OnClientClick="if(confirm('Are you sure want to delete campaign?')== true) { return true; } else { return false; }"
                                                        Text="Delete" />
                                                    &nbsp;<asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" OnClientClick="validate();"
                                                        Text="Submit" ValidationGroup="gpCampaignValidation" />
                                                    &nbsp;<asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" OnClick="btnCancelOnForm_Click"
                                                        Text="Cancel" class="returnButton" />
                                                    <asp:HiddenField ID="hdnFieldEditCampaignKey" runat="server" />
                                                </li>
                                            </ul>
                                        </fieldset>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnDelete" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancelOnForm" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </telerik:RadPageView>
                        <telerik:RadPageView ID="pgCampaignCost" runat="server" Selected="false" >
                            <asp:Panel ID="divCampaignCostMain" runat="server">
                                <asp:Panel runat="server" ID="divCampaignCost" Visible="false" class="Toolbar">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <fieldset id="Fieldset1" class="condado">
                                                <legend>Manage Campaign Cost</legend>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td width="30%">
                                                                        <asp:Button ID="btnAddNewCampaignCost" runat="server" Text="Add New Campaign Cost"
                                                                            CausesValidation="False" class="resetChangeFlag" OnClick="btnAddNewCampaignCost_Click" />
                                                                    </td>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                        <asp:Label ID="lblMessageCampaignCostGrid" runat="server" CssClass="LabelMessage"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc1:PagingBar ID="pgBarCampaignCost" runat="server" NewButtonTitle="" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdCampaignCost" runat="server" Width="100%" OnPageIndexChanging="grdCampaignCost_PageIndexChanging"
                                                                OnSorting="grdCampaignCost_Sorting" AutoGenerateColumns="False" GridLines="None"
                                                                AlternatingRowStyle-CssClass="alt" CssClass="mGrid" OnRowCommand="grdCampaignCost_RowCommand"
                                                                AllowSorting="true" OnRowDataBound="grdCampaignCost_RowDataBound">
                                                                <AlternatingRowStyle CssClass="alt" />
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="ID" SortExpression="CampaignCostId">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblID" runat="server" Text='<%# Bind("CampaignCostId") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="StartDate" SortExpression="StartDate">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="EndDate" SortExpression="EndDate">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblEndDate" runat="server" Text='<%# Bind("EndDate") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Cost" SortExpression="Cost">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblCost" runat="server" Text='<%# Bind("Cost") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Type" SortExpression="Type">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblType" runat="server" Text='<%# Bind("Type") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle Width="50%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="TypeText" SortExpression="TypeText">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTypeText" runat="server" Text='<%# Bind("Text") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                        <ItemStyle Width="50%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Return" SortExpression="Return">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblReturn" runat="server" Text='<%# Bind("Return") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Timer" SortExpression="Timer">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTimer" runat="server" Text='<%# Bind("Timer") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ShowHeader="False">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkEdit1" runat="server" CausesValidation="false" CommandName="EditX1"
                                                                                Text="Edit" class="resetChangeFlag"></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkDelete1" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                                                Text=" Delete" OnClientClick="if(confirm('Are you sure want to delete campaign cost?')== true){ return true;} else { return false;}"></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No Campaign Cost found.
                                                                </EmptyDataTemplate>
                                                                <HeaderStyle CssClass="gridHeader" />
                                                                <PagerSettings Position="Top" />
                                                                <PagerStyle VerticalAlign="Bottom" />
                                                            </asp:GridView>
                                                            <asp:HiddenField ID="hdSortColCC" runat="server" />
                                                            <asp:HiddenField ID="hdSortDirectionCC" runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnAddNewCampaignCost" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </asp:Panel>
                                <div id="divAddUpdateForm" runat="server" visible="false">
                                    <asp:UpdatePanel ID="UpdatePanelCampaignCostForm" runat="server">
                                        <ContentTemplate>
                                            <fieldset id="Fieldset2" class="condado">
                                                <div class="buttons" style="text-align: right">
                                                    <asp:Button runat="server" Text="Return to Manage Campaigns Cost" ID="btnRetunToCampaignCost"
                                                        CausesValidation="False" class="returnButton" OnClick="btnRetunToCampaignCost_Click">
                                                    </asp:Button>
                                                </div>
                                                <asp:Label ID="lblSuccessMsg" runat="server" CssClass="LabelMessage" Visible="false"
                                                    ></asp:Label>
                                                    <%--<uc3:StatusLabel ID="lblSuccessMsgCampaigns" runat="server" />--%>
                                                <legend>Add/Edit Campaigns Cost</legend>
                                                <ul>
                                                    <li class="ErrorLabel">
                                                        <asp:Label ID="lblErrorMsg" runat="server" Text="A Campaign for chosen date range already exists."
                                                            CssClass="HeadingText" Visible="false"></asp:Label>
                                                    </li>
                                                    <li class="Header">
                                                        <asp:Label ID="Label5" runat="server" Text="Add Campaign Cost" CssClass="HeadingText"
                                                            Visible="false"></asp:Label>
                                                    </li>
                                                    <li id="Li1" runat="server">
                                                        <asp:Label ID="lblStartDate" runat="server" AssociatedControlID="rdStartDatePicker"
                                                            Text="Start Date"></asp:Label>
                                                        <telerik:RadDatePicker ID="rdStartDatePicker" runat="server">
                                                        </telerik:RadDatePicker>
                                                    </li>
                                                    <li id="Li2" runat="server">
                                                        <asp:Label ID="lblEndDate" runat="server" AssociatedControlID="rdEndDatePicker" Text="End Date"></asp:Label>
                                                        <telerik:RadDatePicker ID="rdEndDatePicker" runat="server">
                                                        </telerik:RadDatePicker>
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblCostPerlead" runat="server" AssociatedControlID="txtCostPerLead"
                                                            Text="Cost"></asp:Label>
                                                        <telerik:RadNumericTextBox ID="txtCostPerLead" runat="server" Width="72px" MinValue="0"
                                                            MaxValue="999999999" ValidationGroup="gpCampaignCost">
                                                            <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                        </telerik:RadNumericTextBox>
                                                        <asp:RequiredFieldValidator ID="reqFldVldCostperlead" runat="server" Display="None" ValidationGroup="gpCampaignCost"
                                                            ErrorMessage="Enter cost per lead." ControlToValidate="txtCostPerLead"></asp:RequiredFieldValidator>
                                                        <asp:ValidatorCalloutExtender ID="reqFldVldCostperlead_ValidatorCalloutExtender"
                                                            runat="server" Enabled="True" TargetControlID="reqFldVldCostperlead">
                                                        </asp:ValidatorCalloutExtender>
                                                        <asp:RegularExpressionValidator ID="regexVldCostPerLead" runat="server" ControlToValidate="txtCostPerLead"
                                                            Display="None" ErrorMessage="Enter CPL amount in proper format . &lt;br/&gt; Dollar Amount: ###.##"
                                                            ForeColor="#CC0000" ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9]{1,2})?$"></asp:RegularExpressionValidator>
                                                        <asp:ValidatorCalloutExtender ID="regexVldCostPerLead_ValidatorCalloutExtender" runat="server"
                                                            Enabled="True" TargetControlID="regexVldCostPerLead" Width="250px">
                                                        </asp:ValidatorCalloutExtender>
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblCampaignType" runat="server" AssociatedControlID="ddlCampaignType"
                                                            Text="Campaign Type"></asp:Label>
                                                        <asp:DropDownList ID="ddlCampaignType" runat="server" AppendDataBoundItems="True"
                                                            AutoPostBack="true" DataTextField="Text" DataValueField="Id" Height="21px" Width="123px"
                                                            OnSelectedIndexChanged="ddlCampaignType_SelectedIndexChanged" ValidationGroup="gpCampaignCost">
                                                            <asp:ListItem Value="-1">-- Select Type --</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator ID="reqFldVldCamapignType" runat="server" ForeColor="#CC0000" ControlToValidate="ddlCampaignType"
                                                            Display="None" ErrorMessage="Select campaign type." ValidationGroup="gpCampaignCost" InitialValue="-1"></asp:RequiredFieldValidator>
                                                        <asp:ValidatorCalloutExtender ID="reqFldVldCamapignType_ValidatorCalloutExtender"
                                                            runat="server" Enabled="True" TargetControlID="reqFldVldCamapignType">
                                                        </asp:ValidatorCalloutExtender>
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblReturnCC" runat="server" AssociatedControlID="txtReturnCampCost"
                                                            Text="Return"></asp:Label>
                                                        <telerik:RadNumericTextBox ID="txtReturnCampCost" runat="server" Width="72px" MinValue="0"
                                                            MaxValue="999999999" ValidationGroup="gpCampaignCost">
                                                            <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                        </telerik:RadNumericTextBox>
                                                        <asp:RequiredFieldValidator ID="reqFldVldReturnCC" runat="server" Display="None"
                                                            ErrorMessage="Enter Return value." ControlToValidate="txtCostPerLead"></asp:RequiredFieldValidator>
                                                        <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server" Enabled="True"
                                                            TargetControlID="reqFldVldReturnCC">
                                                        </asp:ValidatorCalloutExtender>
                                                        <asp:RegularExpressionValidator ID="regexVldReturnCC" runat="server" ControlToValidate="txtCostPerLead"
                                                            Display="None" ErrorMessage="Enter CPL amount in proper format . &lt;br/&gt; Dollar Amount: ###.##"
                                                            ForeColor="#CC0000" ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9]{1,2})?$"></asp:RegularExpressionValidator>
                                                        <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" runat="server" Enabled="True"
                                                            TargetControlID="regexVldReturnCC" Width="250px">
                                                        </asp:ValidatorCalloutExtender>
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblTimer" runat="server" AssociatedControlID="txtTimer" Visible="false"
                                                            Text="Timer"></asp:Label>
                                                        <telerik:RadNumericTextBox ID="txtTimer" runat="server" Width="72px" Visible="false"
                                                            MinValue="0" MaxValue="999999999" ValidationGroup="gpCampaignCost">
                                                            <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                        </telerik:RadNumericTextBox>
                                                       
                                                    </li>
                                                    <asp:Panel ID="pnlAddCampCostButtons" runat="server" Visible="true">
                                                        <li class="buttons">
                                                            <asp:Label ID="Label16" runat="server" CssClass="LabelMessage"></asp:Label>
                                                            <asp:HiddenField ID="HiddenField3" runat="server" />
                                                            <asp:Button ID="btnDeleteCampCost" runat="server" CausesValidation="False" OnClientClick="if(confirm('Are you sure want to delete campaign?')== true) { return true; } else { return false; }"
                                                                Text="Delete" OnClick="btnDeleteCampCost_Click" />
                                                            &nbsp;<asp:Button ID="btnSubmitCampCost" runat="server" CausesValidation="true" ValidationGroup="gpCampaignCost"
                                                                Text="Submit" OnClick="btnSubmitCampCost_Click"  OnClientClick="validate();"/>
                                                            &nbsp;<asp:Button ID="btnCancelOnFormCampCost" runat="server" CausesValidation="False"
                                                                Text="Cancel" class="returnButton" OnClick="btnCancelOnFormCampCost_Click" />
                                                            <asp:HiddenField ID="hdnFieldEditCampaignCostKey" runat="server" />
                                                        </li>
                                                    </asp:Panel>
                                                </ul>
                                                <asp:Panel ID="pnlAddBoth" runat="server" Visible="false">
                                                    <ul>
                                                        <li class="buttons">
                                                            <asp:Label ID="Label4" runat="server" CssClass="LabelMessage"></asp:Label>
                                                            <asp:HiddenField ID="HiddenField1" runat="server" />
                                                            <asp:Button ID="btnAddBoth" runat="server" CausesValidation="true" ValidationGroup="gpCampaignCost"
                                                                Text="Submit" OnClick="btnAddBoth_Click"/>
                                                            &nbsp;<asp:Button ID="btnCancelBoth" runat="server" CausesValidation="False" Text="Cancel"
                                                                class="returnButton" OnClick="btnCancelBoth_Click" />
                                                            <asp:HiddenField ID="HiddenField2" runat="server" />
                                                        </li>
                                                    </ul>
                                                </asp:Panel>
                                            </fieldset>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnDeleteCampCost" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnSubmitCampCost" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnCancelOnFormCampCost" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </asp:Panel>
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
