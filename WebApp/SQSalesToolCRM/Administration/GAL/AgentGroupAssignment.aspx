<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AgentGroupAssignment.aspx.cs" MasterPageFile="~/MasterPages/GAL.master" Inherits="Administration_GAL_AgentGroupAssignment" %>

<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style3 {
            width: 45%;
            text-align: center;
            vertical-align: middle;
        }

        .style4 {
            width: 10%;
            text-align: center;
            vertical-align: middle;
        }

        .style5 {
            width: 45%;
            text-align: center;
            vertical-align: middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div style="text-align: center;">
        <asp:Label ID="AgentGroup" runat="server"
            Font-Bold="True" Font-Size="Large" ForeColor="#736038"></asp:Label>
    </div>
    <asp:Label ID="lblTitle" runat="server" Font-Bold="True"
        Font-Size="Large" Text="Agent Group Assignment"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableGroups" runat="server" SelectionMode="Multiple"
                    Width="80%" Height="250px" DataSourceID="Available"
                    DataTextField="agent_group_name" DataValueField="agent_group_id"></asp:ListBox>
                <asp:SqlDataSource ID="Available" runat="server"
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    SelectCommand="  SELECT  g.agent_group_name ,
                                            g.agent_group_id
                                    FROM    gal_AgentGroups g
                                    WHERE   ISNULL(g.agent_group_acd_flag, 0) = @bGALType AND g.agent_group_delete_flag=0
                                            AND g.agent_group_id NOT  IN (
                                            SELECT  ag.agent_group_id
                                            FROM    dbo.gal_agent2agentgroups ag
                                            WHERE   ag.agent_id = @agentId )
                                    ORDER BY g.agent_group_name">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="agentId" QueryStringField="agentId" />
                        <asp:Parameter Name="bGALType" Type="Boolean" />
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
                <asp:ListBox ID="lstAddedGroups" runat="server" SelectionMode="Multiple"
                    Width="80%" DataSourceID="Assigned"
                    DataTextField="agent_group_name" DataValueField="agent_group_id"
                    Height="250px"></asp:ListBox>
                <asp:SqlDataSource ID="Assigned" runat="server"
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    SelectCommand="  SELECT  ( CASE WHEN @bGALType = 0
               THEN CASE WHEN g.agent_group_id = a.agent_default_agent_group_id
                         THEN g.agent_group_name + ' ( Default )'
                         ELSE g.agent_group_name
                    END
               ELSE CASE WHEN g.agent_group_id = a.agent_default_agent_group_id_acd
                         THEN g.agent_group_name + ' ( Default )'
                         ELSE g.agent_group_name
                    END
          END ) AS agent_group_name ,
        g.agent_group_id
FROM    gal_AgentGroups g
        JOIN dbo.gal_agent2agentgroups ag ON ag.agent_group_id = g.agent_group_id
                                             AND ag.agent_id = @agentID AND ISNULL(g.agent_group_acd_flag,0)=@bGALType
        JOIN dbo.gal_agents a ON a.agent_id = ag.agent_id
ORDER BY g.agent_group_name">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="agentId" QueryStringField="agentId"  />
                        <asp:Parameter Name="bGALType" Type="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <asp:Button ID="btnReturn" runat="server" Text="Return To Agent " OnClick="btnReturnToAgentManager_Click" />
                <asp:Button ID="btnDefault" runat="server" Text="Make Default Group " OnClick="btnMakeDefault_Click" />
            </td>
        </tr>
    </table>
    <%--  <asp:SqlDataSource ID="AgentGroupInfoDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        OldValuesParameterFormatString="original_{0}"
        SelectCommand="select agent_group_name
                                    from gal_AgentGroups (nolock)
                                    WHERE ([agent_group_id] = @agent_group_id and IsNULL([agent_group_acd_flag],0) = @bGALType  )">
        <SelectParameters>
            <asp:SessionParameter Name="agent_group_id" SessionField="AgentGroupKey" Type="Object" />
            <asp:Parameter Name="bGALType" Type="Boolean" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <br />
</asp:Content>

