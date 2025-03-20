<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="CampaignGroupCampaigns.aspx.cs" Inherits="SQS_Dialer.CampaignGroupCampaigns" %>
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
    <div style="text-align:center;" style="width=100%;">
        <asp:Label ID="AgentGroup" runat="server" 
            Font-Bold="True" Font-Size="Large" ForeColor="#736038"></asp:Label></div>
    <asp:Label ID="lblTitle" runat="server" Font-Bold="True" 
    Font-Size="Large" Text="Campaign Group Campaign Selection"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableCampaigns" runat="server" SelectionMode="Multiple" 
                    Width="80%" Height="250px" DataSourceID="Available" 
                    DataTextField="CampaignTitle" DataValueField="campaign_id">
                </asp:ListBox>
                <asp:SqlDataSource ID="Available" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    SelectCommand=" SELECT   CampaignTitle = case when campaign_group_id is null then cmp_title else cmp_title + ' (' + campaign_group_name + ')' end, campaign_id
                                    FROM         gal_Campaigns
                                    JOIN campaigns on Campaign_Id = cmp_id 
                                    LEFT JOIN gal_CampaignGroups on campaign_campaign_group_id = campaign_group_id and IsNULL([campaign_group_acd_flag],0) = @bGALType
                                    WHERE     (NOT (campaign_id IN (SELECT campaign_id
                                                                    FROM gal_Campaigns
                                                                    WHERE campaign_campaign_group_id= @campaign_campaign_group_id)))
                                        and cmp_delete_flag = 0
                                    ORDER BY CampaignTitle">
                    <SelectParameters>
                        <asp:SessionParameter Name="campaign_campaign_group_id" SessionField="CampaignGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
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
                    Width="80%" DataSourceID="Assigned" 
                    DataTextField="CampaignTitle" DataValueField="campaign_id" 
                    Height="250px">
                </asp:ListBox>
                <asp:SqlDataSource ID="Assigned" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}" 
                    SelectCommand=" SELECT   CampaignTitle = cmp_title, campaign_id
                                    FROM         gal_Campaigns
                                    JOIN Campaigns on campaign_id = cmp_id 
                                    WHERE     campaign_campaign_group_id= @campaign_campaign_group_id and cmp_delete_flag = 0
                                    ORDER BY CampaignTitle">
                    <SelectParameters>
                        <asp:SessionParameter Name="campaign_campaign_group_id" SessionField="CampaignGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <asp:Button ID="btnReturn" runat="server" Text="Return To Campaign Groups" OnClick="btnReturnToAgentManager_Click" />
            </td>
        </tr>
        </table>
                <asp:SqlDataSource ID="AgentGroupInfoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}"                   
                    SelectCommand="select campaign_group_name
                                    from gal_CampaignGroups (nolock)
                                    WHERE ([campaign_group_id] = @campaign_group_id and IsNULL([campaign_group_acd_flag],0) = @bGALType)">
                    <SelectParameters>
                        <asp:SessionParameter Name="campaign_group_id" SessionField="CampaignGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
                    </SelectParameters>
                </asp:SqlDataSource>
    <br />
    </asp:Content>
