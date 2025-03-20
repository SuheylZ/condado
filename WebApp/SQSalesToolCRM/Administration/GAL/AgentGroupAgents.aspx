<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentGroupAgents.aspx.cs" Inherits="SQS_Dialer.AgentGroupAgents" %>

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
        Font-Size="Large" Text="Agent Group Agent Selection"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableCampaigns" runat="server" SelectionMode="Multiple"
                    Width="80%" Height="250px" DataSourceID="Available"
                    DataTextField="AgentName" DataValueField="agent_id"></asp:ListBox>
                <asp:SqlDataSource ID="Available" runat="server"
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    SelectCommand="  SELECT AgentName = usr_last_name + ', ' + usr_first_name ,
                                            a.agent_id
                                     FROM   gal_Agents a
                                            JOIN users ON a.agent_id = usr_key
                                     WHERE  ( NOT ( a.agent_id IN (
                                                    SELECT DISTINCT b.agent_id
                                                    FROM    gal_Agents b
                                                            INNER JOIN dbo.gal_agent2agentgroups aag ON aag.agent_id = b.agent_id
                                                                                                  AND aag.agent_group_id = @agent_agent_group_id ) )
                                            )
                                            AND usr_delete_flag = 0
                                     ORDER BY AgentName">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_agent_group_id" SessionField="AgentGroupKey" Type="Object" />
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
                <asp:ListBox ID="lstAddedCampaigns" runat="server" SelectionMode="Multiple"
                    Width="80%" DataSourceID="Assigned"
                    DataTextField="AgentName" DataValueField="agent_id"
                    Height="250px"></asp:ListBox>
                <asp:SqlDataSource ID="Assigned" runat="server"
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                    OldValuesParameterFormatString="original_{0}"
                    SelectCommand="  SELECT AgentName = usr_last_name + ', ' + usr_first_name ,
                                            a.agent_id
                                     FROM   gal_Agents a
                                            JOIN users ON agent_id = usr_key
                                            INNER JOIN dbo.gal_agent2agentgroups aag ON aag.agent_id = a.agent_id
                                                                                        AND aag.agent_group_id = @agent_agent_group_id
                                     ORDER BY AgentName">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_agent_group_id" SessionField="AgentGroupKey" Type="Object" />
                        <asp:Parameter Name="bGALType" Type="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center">
                <asp:Button ID="btnReturn" runat="server" Text="Return To Agent Groups" OnClick="btnReturnToAgentManager_Click" />
            </td>
        </tr>
    </table>
    <asp:SqlDataSource ID="AgentGroupInfoDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
        OldValuesParameterFormatString="original_{0}"
        SelectCommand="select agent_group_name
                                    from gal_AgentGroups (nolock)
                                    WHERE ([agent_group_id] = @agent_group_id and IsNULL([agent_group_acd_flag],0) = @bGALType  )">
        <SelectParameters>
            <asp:SessionParameter Name="agent_group_id" SessionField="AgentGroupKey" Type="Object" />
            <asp:Parameter Name="bGALType" Type="Boolean" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>
