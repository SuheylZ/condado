<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="viewPrioritizedLeads.aspx.cs" Inherits="Leads_viewPrioritizedLeads" %>
<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script language="javascript" type="text/javascript">
        // <![CDATA[
        function redirect(accountid) {
            location.href = 'Leads.aspx?accountid=accountid';
        }
        // ]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">

    
    </script>
    <fieldset class="condado">
        <legend>Prioritization View: </legend>
        <asp:SqlDataSource ID="SqlDataSource1"  OnSelecting="sqldatasource_selecting" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
            CancelSelectOnNullParameter="false">
            <SelectParameters>
                <asp:Parameter Name="userId" />
                <%--[QN, 22/05/2013] @mode is new parameter added to the  proj_GetPrioritizedList...
        ... its value can be off, top1 or all. on the basis of this parameter data is...
        ... fetched from database.--%>
                <asp:Parameter Name="mode" />
                <asp:Parameter Name="rows" Type="Int32" Direction="ReturnValue" />
            </SelectParameters>
        </asp:SqlDataSource>
        <div id="divToolbar" class="Toolbar">
            <table>
                <tr>
                    <td style="width: 30%;">
                        <asp:Button ID="btnPVRefresh" runat="server" Text="Refresh" CausesValidation="False"
                            OnClick="btnPVRefresh_Click" />
                    </td>
                    <td style="width: 70%; text-align: right;">
                        <asp:Label runat="server" ID="lblTotalRecords"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>

        <br />
        <asp:GridView ID="grdLeads" runat="server" AutoGenerateColumns="False" Skin="" CssClass="mGrid" 
            Width="100%" CellSpacing="0" GridLines="None" EnableTheming="True" onfocus="this.blur();" AllowPaging="True"
            OnRowCommand="Evt_ItemCommand" ShowHeaderWhenEmpty="true" DataSourceID="SqlDataSource1" EnableViewState="False"
            OnRowDataBound="pvGrid_ItemDataBound" DataKeyNames="leadId" HeaderStyle-CssClass="gridHeader">
            <AlternatingRowStyle CssClass="alt" />
            <PagerSettings Visible="True" />
            <PagerStyle HorizontalAlign="Left" CssClass="GridPager"></PagerStyle>
            <Columns>
                <asp:BoundField DataField="leadId" HeaderText="" Visible="false"></asp:BoundField>
                <asp:BoundField DataField="accountId" SortExpression="accountId" HeaderText="Account ID"></asp:BoundField>
                <asp:BoundField DataField="individualId" HeaderText="Individual ID" Visible="false"></asp:BoundField>
                <asp:BoundField DataField="dateCreated" SortExpression="dateCreated" DataFormatString="{0:MM/dd/yyyy hh:mm tt}"
                    HeaderText="Date Created"></asp:BoundField>
                <asp:BoundField DataField="firstName" SortExpression="firstName" HeaderText="First Name"></asp:BoundField>
                <asp:BoundField DataField="lastName" SortExpression="lastName" HeaderText="Last Name"></asp:BoundField>
                <asp:BoundField DataField="dateOfBirth" SortExpression="dateOfBirth" DataFormatString="{0:MM/dd/yyyy}"
                    HeaderText="Date of Birth"></asp:BoundField>
                <asp:TemplateField HeaderText="Day Phone">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDayPhoneGrid" runat="server" CausesValidation="false" CommandName="DayPhoneX"
                            OnClientClick='<%# GetOnClickClientScript(Convert.ToString(Eval("DayPhone")),Convert.ToString(Eval("DayPluseId"))) %>'
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "leadId") +","+ DataBinder.Eval(Container.DataItem, "accountId")+","+ DataBinder.Eval(Container.DataItem, "DayPhone") %>' outpulseId='<%# DataBinder.Eval(Container.DataItem, "DayPluseId") %>'
                            Text='<%# Eval("DayPhone","{0:(###) ###-####}") %>' OnClick="DialPhone"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Evening Phone">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEveningPhoneGrid" runat="server" CausesValidation="false"
                            OnClientClick='<%# GetOnClickClientScript(Convert.ToString(Eval("EveningPhone")),Convert.ToString(Eval("EvenPluseId"))) %>'
                             CommandName="EveningPhoneX" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"leadId") +","+ DataBinder.Eval(Container.DataItem, "accountId")+","+ DataBinder.Eval(Container.DataItem, "EveningPhone") %>' outpulseId='<%# DataBinder.Eval(Container.DataItem, "EvenPluseId") %>'
                            Text='<%# Eval("EveningPhone","{0:(###) ###-####}") %>' OnClick="DialPhone"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="leadCampaign" SortExpression="leadCampaign" HeaderText="Campaign"></asp:BoundField>
                <asp:BoundField DataField="leadStatus" SortExpression="leadStatus" HeaderText="Status"></asp:BoundField>
                <asp:BoundField DataField="SubStatus1" SortExpression="SubStatus1" HeaderText="Sub Status 1"
                    HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="AssignedUserKey" HeaderText="User" Visible="false"></asp:BoundField>
                <asp:BoundField DataField="userAssigned" SortExpression="userAssigned" HeaderText="User"></asp:BoundField>
                <asp:BoundField DataField="CSR" SortExpression="CSR" HeaderText="CSR"></asp:BoundField>
                <asp:BoundField DataField="TA" SortExpression="TA" HeaderText="TA"></asp:BoundField>
                <asp:BoundField DataField="State" SortExpression="State" HeaderText="State"></asp:BoundField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%--[QN, 22/05/2013] Server control asp:LinkButton is used instead of html anchor. Code behind...
                                        ... find this control in row data bound event. and show and hide this control on the
                                        ... basis of value in priority list on user permission.
                        --%>
                        <%--<a id="lnkEdit" href="Leads.aspx?accountid=<%#DataBinder.Eval(Container.DataItem, "accountId")%>" title="Edit Account">Edit</a>--%>
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit" OnClientClick="javascript:disableUI();" PostBackUrl='<%#"Leads.aspx?accountid="+Eval("accountId")+"&ruleid="+DataBinder.Eval(Container.DataItem, "ruleid") %>'></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                There are no records to display.
            </EmptyDataTemplate>
        </asp:GridView>
    </fieldset>
</asp:Content>
