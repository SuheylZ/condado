<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="StateGroupStates.aspx.cs" Inherits="SQS_Dialer.StateGroupStates" %>
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
    <div style="text-align:center;">
        <asp:Label ID="AgentGroup" runat="server" 
            Font-Bold="True" Font-Size="Large" ForeColor="#736038"></asp:Label></div>
    <asp:Label ID="lblTitle" runat="server" Font-Bold="True" 
    Font-Size="Large" Text="State Group Campaign Selection"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableCampaigns" runat="server" SelectionMode="Multiple" 
                    Width="80%" Height="250px" DataSourceID="Available" 
                    DataTextField="state_name" DataValueField="state_id">
                </asp:ListBox>
                <asp:SqlDataSource ID="Available" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    SelectCommand=" SELECT   state_name, state_id
                                    FROM         gal_States
                                    WHERE     state_id not IN (SELECT stgrp_state_id FROM gal_StateGroupStates
                                                                JOIN dbo.gal_stategroups ON state_group_id = dbo.gal_stategroupstates.stgrp_state_group_id
                                                                   and IsNULL([state_group_acd_flag],0) = @bGALType         
                                                                )
                                    ORDER BY state_name">
                    <SelectParameters>
                        <asp:SessionParameter Name="state_group_id" SessionField="StateGroupKey" Type="Object" />
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
                    DataTextField="state_name" DataValueField="stgrp_id" 
                    Height="250px">
                </asp:ListBox>
                <asp:SqlDataSource ID="Assigned" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}" 
                    SelectCommand=" SELECT   state_name, stgrp_id
                                    FROM    gal_States
                                    JOIN    gal_StateGroupStates on stgrp_state_id = state_id
                                    WHERE     stgrp_state_group_id = @state_group_id
                                    ORDER BY state_name">
                    <SelectParameters>
                        <asp:SessionParameter Name="state_group_id" SessionField="StateGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <asp:Button ID="btnReturn" runat="server" Text="Return To State Groups" OnClick="btnReturnToAgentManager_Click" />
            </td>
        </tr>
        </table>
                <asp:SqlDataSource ID="StateGroupInfoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}"                   
                    SelectCommand="select state_group_name
                                    from gal_StateGroups (nolock)
                                    WHERE ([state_group_id] = @state_group_id and IsNULL([state_group_acd_flag],0) = @bGALType)">
                    <SelectParameters>
                        <asp:SessionParameter Name="state_group_id" SessionField="StateGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
                    </SelectParameters>
                </asp:SqlDataSource>
    <br />
    </asp:Content>
