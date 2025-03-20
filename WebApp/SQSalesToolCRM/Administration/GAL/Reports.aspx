<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master"
    CodeFile="Reports.aspx.cs" Inherits="SQS_Dialer.Reports" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <telerik:RadTabStrip ID="ReportsTabContainer" runat="server" Skin="WebBlue" MultiPageID="tlMultipage">
        <Tabs>
            <telerik:RadTab runat="server" Text="Group Matrix" PageViewID="RadPageView1"/>
            <telerik:RadTab runat="server" Text="Agent Totals" PageViewID="RadPageView2" />
            <telerik:RadTab runat="server" Text="Campaign Totals" PageViewID="RadPageView3" />
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="tlMultipage" runat="server" SelectedIndex="0" >
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <asp:GridView ID="grdGroupMatrix" runat="server"
                    CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                    DataSourceID="dsGroupMatrix" ShowHeaderWhenEmpty="true" OnRowDataBound="grdGroupMatrix_RowDataBound" >
                    <HeaderStyle HorizontalAlign="Center"
                        VerticalAlign="Middle" Font-Size="Small" />
                    <RowStyle Font-Size="Small" HorizontalAlign="Center" VerticalAlign="Middle" />
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                </asp:GridView>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <asp:GridView ID="grdAgentTotals" runat="server"
                      CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                    DataSourceID="dsAgentTotals" ShowHeaderWhenEmpty="true" OnRowDataBound="grdAgentTotals_RowDataBound">
                    <HeaderStyle HorizontalAlign="Center"
                        VerticalAlign="Middle" Font-Size="Small" />
                    <RowStyle Font-Size="Small" HorizontalAlign="Center" VerticalAlign="Middle" />
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                </asp:GridView>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView3" runat="server">
                 <asp:GridView ID="grdCampaignTotals" runat="server" AutoGenerateColumns="False"
                    CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                    DataSourceID="dsCampaignTotals"
                    Width="600px" ShowHeaderWhenEmpty="true" OnRowDataBound="grdCampaignTotals_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="cmp_title" HeaderText="Campaign" SortExpression="cmp_title" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:#,###}" >
                        <HeaderStyle CssClass="GridItem" HorizontalAlign="Right" />
                        <ItemStyle CssClass="GridItem" HorizontalAlign="Right" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle HorizontalAlign="Left"
                        VerticalAlign="Middle" Font-Size="Small" />
                    <RowStyle Font-Size="Small" />
                     <EmptyDataTemplate>
                         No record found.
                     </EmptyDataTemplate>
                </asp:GridView>
        </telerik:RadPageView>
        </telerik:RadMultiPage>

        <asp:SqlDataSource ID="dsCampaignTotals" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="proj_GetCampaignTotals" SelectCommandType="StoredProcedure">
            <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
        </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsGroupMatrix" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="proj_GetGroupMatrix" SelectCommandType="StoredProcedure">
          <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
    </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsAgentTotals" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        SelectCommand="proj_GetAgentMatrix" SelectCommandType="StoredProcedure">
              <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
