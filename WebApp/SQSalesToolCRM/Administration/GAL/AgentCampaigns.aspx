<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentCampaigns.aspx.cs" Inherits="SQS_Dialer.AgentEdit" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style3
        {
            width: 45%;
            text-align: center;
            vertical-align:middle;
        }
        .style4
        {
            width: 10%;
            text-align: center;
            vertical-align:middle;
        }
        .style5
        {
            width: 45%;
            text-align: center;
            vertical-align:middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div style="text-align:center;"><asp:Label ID="AgentName" runat="server" 
            Font-Bold="True" Font-Size="Large" ForeColor="#736038"></asp:Label></div>
    <asp:Label ID="lblAgentCampaignsTitle" runat="server" Font-Bold="True" 
    Font-Size="Large" Text="Agent Campaign Select"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableCampaigns" runat="server" SelectionMode="Multiple" 
                    Width="80%" Height="250px" DataSourceID="AvailableCampaignsDataSource" 
                    DataTextField="campaign_l360_campaign_title" DataValueField="campaign_id">
                </asp:ListBox>
                <asp:SqlDataSource ID="AvailableCampaignsDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" SelectCommand="SELECT     TOP (100) PERCENT campaign_l360_campaign_title = CampaignTitle, campaign_id
FROM         dbo.Campaigns
JOIN dbo.Leads360_CampaignList on CampaignID = campaign_l360_campaign_id
WHERE     (NOT (campaign_id IN
                          (SELECT     cmp2agt_campaign_id
                            FROM          dbo.vwAgent2Campaign
                            WHERE      (cmp2agt_agent_id = @agent_id)
))
)
ORDER BY campaign_l360_campaign_title">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_id" SessionField="AgentCampaignKey" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
            <td class="style4">
                <asp:Button ID="btnAddSelected" runat="server" Text="&gt;" Width="50px" OnClick="btnAddSelected_Click" />
                <br />
                <br />
                <asp:Button ID="btnAddAll" runat="server" Text="&gt;&gt;&gt;" Width="50px" OnClick="btnAddAll_Click" />
                <br />
                <br />
                <asp:Button ID="btnRemoveAll" runat="server" Text="&lt;&lt;&lt;" Width="50px" OnClick="btnRemoveAll_Click" />
                <br />
                <br />
                <asp:Button ID="btnRemoveSelected" runat="server" Text="&lt;" Width="50px" OnClick="btnRemoveSelected_Click" />
            </td>
            <td class="style5">
                <asp:ListBox ID="lstAddedCampaigns" runat="server" SelectionMode="Multiple" 
                    Width="80%" DataSourceID="AddedCampaignsDataSource" 
                    DataTextField="campaign_l360_campaign_title" DataValueField="cmp2agt_id" 
                    Height="250px">
                </asp:ListBox>
                <asp:SqlDataSource ID="AddedCampaignsDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}" 
                    
                    SelectCommand="SELECT * FROM [vwAgent2Campaign] WHERE ([cmp2agt_agent_id] = @cmp2agt_agent_id) ORDER BY [campaign_l360_campaign_title]">
                    <SelectParameters>
                        <asp:SessionParameter Name="cmp2agt_agent_id" SessionField="AgentCampaignKey" 
                            Type="Object" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <asp:Button ID="btnReturnToAgentManager0" runat="server" Text="Return To Agent Manager" OnClick="btnReturnToAgentManager0_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: left">
                <br />
    <asp:Label ID="lblAgentCampaignsTitle0" runat="server" Font-Bold="True" 
    Font-Size="Large" Text="Agent Campaign Edit"></asp:Label>
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <br />
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    CellPadding="4" DataKeyNames="cmp2agt_id" 
                    DataSourceID="CampaignModifyDataSource" ForeColor="#333333" GridLines="None" 
                    Width="100%" OnDataBound="GridView1_DataBound">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="cmp2agt_id" HeaderText="cmp2agt_id" ReadOnly="True" 
                            SortExpression="cmp2agt_id" Visible="False" />
                        <asp:BoundField DataField="campaign_l360_campaign_title" HeaderText="Campaign" 
                            ReadOnly="True" SortExpression="campaign_l360_campaign_title" >
                        <ItemStyle Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="cmp2agt_max" HeaderText="Daily Leads Maximum" 
                            SortExpression="cmp2agt_max" >
                        <ItemStyle Width="30%" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Level" SortExpression="cmp2agt_level">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Bind("cmp2agt_level") %>'>
                                    <asp:ListItem>1</asp:ListItem>
                                    <asp:ListItem>2</asp:ListItem>
                                    <asp:ListItem>3</asp:ListItem>
                                    <asp:ListItem>4</asp:ListItem>
                            </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("cmp2agt_level") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="cmp2agt_inactive" HeaderText="Inactive?" 
                            SortExpression="cmp2agt_inactive" >
                        <ItemStyle Width="30%" />
                        </asp:CheckBoxField>
                        <asp:BoundField DataField="cmp2agt_agent_id" HeaderText="cmp2agt_agent_id" 
                            SortExpression="cmp2agt_agent_id" Visible="False" />
                        <asp:CommandField ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
                            DeleteImageUrl="~/Images/gal/remove.png" EditImageUrl="~/Images/gal/edit.png" 
                            ShowEditButton="True" UpdateImageUrl="~/Images/gal/check.png" />
                    </Columns>
                    <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5A471B" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                    <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    <SortedAscendingCellStyle BackColor="#FDF5AC" />
                    <SortedAscendingHeaderStyle BackColor="#4D0000" />
                    <SortedDescendingCellStyle BackColor="#FCF6C0" />
                    <SortedDescendingHeaderStyle BackColor="#820000" />
                </asp:GridView>
                <asp:SqlDataSource ID="CampaignModifyDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}"                   
                    SelectCommand="SELECT * FROM [vwAgent2Campaign] WHERE ([cmp2agt_agent_id] = @cmp2agt_agent_id) ORDER BY [campaign_l360_campaign_title]" 
                    DeleteCommand="DELETE FROM Campaign2Agent WHERE cmp2agt_id = @cmp2agt_id" 
                    UpdateCommand="UPDATE Campaign2Agent SET cmp2agt_max = @cmp2agt_max, cmp2agt_inactive = @cmp2agt_inactive, cmp2agt_level = @cmp2agt_level where cmp2agt_id = @original_cmp2agt_id">
                
                    <SelectParameters>
                        <asp:SessionParameter Name="cmp2agt_agent_id" SessionField="AgentCampaignKey" 
                            Type="Object" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="cmp2_agt_max" Type="Int32" />
                        <asp:Parameter Name="cmp2agt_inactive" Type="Int32" />
                        <asp:Parameter Name="cmp2agt_level" Type="Int32" />
                        <asp:Parameter Name="original_cmp2agt_id" Type="Object" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="AgentInfoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}"                   
                    SelectCommand="select AgentName, location_name
                                    from agents (nolock)
                                    join Leads360_AgentList (nolock) on AgentId = agent_l360_id
                                    join locations (nolock) on agent_location_id = location_id 
                                    WHERE ([agent_id] = @agent_id)">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_id" SessionField="AgentCampaignKey" 
                            Type="Object" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
    <asp:Button ID="btnReturnToAgentManager" runat="server" 
        Text="Return To Agent Manager" />
            </td>
        </tr>
    </table>
    <br />
    </asp:Content>
