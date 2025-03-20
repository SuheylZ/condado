<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentStates.aspx.cs" Inherits="SQS_Dialer.AgentStates" %>
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
    <asp:Label ID="lblSectionTitle" runat="server" Font-Bold="True" 
    Font-Size="Large" Text="Agent State Licensures"></asp:Label>
    <br />
    <br />
    <table class="style1">
        <tr>
            <td class="style3">
                <asp:ListBox ID="lstAvailableStates" runat="server" SelectionMode="Multiple" 
                    Width="80%" Height="250px" DataSourceID="AvailableStatesDataSource" 
                    DataTextField="state_name" DataValueField="state_id">
                </asp:ListBox>
                <asp:SqlDataSource ID="AvailableStatesDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    SelectCommand="select	state_name, state_id
                                    from	States
                                    where	state_id not in (select agent_state_licensure_state_id from AgentStateLicensure where agent_state_licensure_agent_id = @agent_id)
                                    order by state_name">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_id" SessionField="AgentStatesKey" />
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
                <asp:ListBox ID="lstAddedStates" runat="server" SelectionMode="Multiple" 
                    Width="80%" DataSourceID="AddedStatesDataSource" 
                    DataTextField="state_name" DataValueField="agent_state_licensure_id" 
                    Height="250px">
                </asp:ListBox>
                <asp:SqlDataSource ID="AddedStatesDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}" 
                    
                    
                    SelectCommand="SELECT * FROM vwAgent2StateLicense WHERE agent_state_licensure_agent_id = @agent_id ORDER BY state_name">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_id" SessionField="AgentStatesKey" 
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
                <asp:SqlDataSource ID="AgentInfoDataSource" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                    OldValuesParameterFormatString="original_{0}"                   
                    SelectCommand="select AgentName, location_name
                                    from agents (nolock)
                                    join Leads360_AgentList (nolock) on AgentId = agent_l360_id
                                    join locations (nolock) on agent_location_id = location_id 
                                    WHERE ([agent_id] = @agent_id)">
                    <SelectParameters>
                        <asp:SessionParameter Name="agent_id" SessionField="AgentStatesKey" 
                            Type="Object" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <br />
            </td>
        </tr>
        </table>
    <br />
    </asp:Content>
