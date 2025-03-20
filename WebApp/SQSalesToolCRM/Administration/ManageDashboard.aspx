<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="ManageDashboard.aspx.cs" Inherits="Administration_ManageDashboard" ValidateRequest="false" %>

<%@ Register src="../UserControls/StatusLabel.ascx" tagname="StatusLabel" tagprefix="uc1" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register src="../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <uc1:StatusLabel ID="ctlStatus" runat="server" />

    <div id="dvGrid" runat="server" visible="true" class="condado">
        <fieldset class="condado">
        <br />
        Section
        <telerik:RadDropDownList ID="ddlSections" runat="server" DataTextField="Title" DataValueField="Id" Width="200px" AutoPostBack="true" />
        <br /><br />

            <uc2:PagingBar ID="ctlPaging" runat="server" />
        <br />
        <telerik:RadGrid ID="grdAnnouncements" runat="server" AutoGenerateColumns="False" 
                         CssClass="mGrid" Skin="" Width="100%" CellSpacing="0" GridLines="None" 
                         EnableTheming="False" onfocus="this.blur();" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt" OnItemDataBound="grdAnnouncements_ItemDataBound" >
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id">
                <NoRecordsTemplate>
                    No data is available to display
                </NoRecordsTemplate>
                <Columns>
                    <telerik:GridCheckBoxColumn DataField="Enabled" DataType="System.Boolean" HeaderText="" UniqueName="Enabled" ItemStyle-Width="40px">
                        <ItemStyle Width="40px"></ItemStyle>
                    </telerik:GridCheckBoxColumn>
                    <telerik:GridBoundColumn DataField="Title" HeaderText="Title" SortExpression="Title" UniqueName="Title"/>
                    <telerik:GridBoundColumn DataField="Body" HeaderText="Body" SortExpression="ann_title" UniqueName="Body" Visible="false"/>
                    <telerik:GridBoundColumn DataField="DateAdded" HeaderText="Added On" SortExpression="DateAdded" UniqueName="DateAdded" ItemStyle-Width="50%" DataType="System.DateTime" DataFormatString="{0: MMM dd, yyyy}" />
                    <telerik:GridTemplateColumn ItemStyle-Width="75px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="xedit"/>
                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="xdelete"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
            </fieldset>
    </div>
    <div id="dvForm" runat="server" visible="false">
        <fieldset class="condado">
            <legend>Edit Announcement</legend>
            <ul>
                <li>
                    <label for='<%=ddlSectionForm.ClientID %>'>Section</label>
                    <telerik:RadDropDownList ID="ddlSectionForm" runat="server" DataTextField="Title" DataValueField="Id" Width="200px" AutoPostBack="true" />
                </li>
                <li>
                    <label for='<%=txtName.ClientID %>'>Title</label>
                    <telerik:RadTextBox ID="txtName" Runat="server">
                    </telerik:RadTextBox>
                </li>
                <li>
                    <asp:Label ID="lbl1" runat="server" Text="Body" AssociatedControlID="txtBody" />
                    <span id="input">
                        <%--  ContentAreaMode="Iframe" --%>
                        <telerik:RadEditor ID="txtBody" runat="server" Skin="WebBlue" Height="180px" Width="400px"
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
                </li>
                <li>
                    <label for='<%=chkEnabled.ClientID %>'>Enabled</label>
                    <telerik:RadButton ID="chkEnabled" runat="server" ButtonType="ToggleButton" ToggleType="CheckBox" />
                </li>
            </ul>
        </fieldset>
        <div class="buttons">
            <asp:Button ID="btnApply" runat="server" Text="Save" CausesValidation="true" />
            &nbsp;
            <asp:Button ID="btnOK" runat="server" CausesValidation="true" Text="Save &amp; Close" />
            &nbsp;
            <asp:Button ID="btnClose" runat="server" CausesValidation="false" Text="Close"/>
        </div>
    </div>

</asp:Content>

