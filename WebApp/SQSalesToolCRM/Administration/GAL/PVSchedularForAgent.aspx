<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master"
    CodeFile="PVSchedularForAgent.aspx.cs" Inherits="SQS_Dialer.PVSchedularForAgent" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<%@ Register src="~/Administration/GAL/controls/PVScheduleAgent.ascx" tagname="PVScheduleAgent" tagprefix="uc1" %>

<%@ Register src="~/Administration/GAL/controls/PVScheduleAgentGroup.ascx" tagname="PVScheduleAgentGroup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 193px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <br />
        <asp:HiddenField ID="hdnFieldRecordKey" runat="server" />
        <table style="width: 500px !important; ">
            <tr>
                <td class="auto-style1">
                    <asp:CheckBox ID="chkOverride" runat="server" Text="Override PV Schedule" AutoPostBack="true" OnCheckedChanged="chkOverride_CheckedChanged"
                        ValidationGroup="Override" />
                </td>
                <td>
                    <asp:Label runat="server" ID="lblDefaultAgentGroup" Text ="Default Agent Group"> </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlAgentGroups" runat="server"  DataTextField="agent_group_name" DataValueField="agent_group_id" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlAgentGroups_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btnGO" runat="server" Text="GO" 
                        CssClass="ButtonStyle" OnClick="btnGO_Click" Visible="False" />
                </td>
                <asp:SqlDataSource ID="UpdateAgentPVScheduleOverride" runat="server" 
                    ConnectionString="<%$ConnectionStrings:ApplicationServices%>" 
                    SelectCommand="Select agent_override_pv_schedule, agent_default_agent_group_id, agent_default_agent_group_id_acd  from gal_agents
where agent_id = @agent_id" 
                    UpdateCommand="Update gal_agents set agent_override_pv_schedule = @OverrideValue, agent_default_agent_group_id = @default_agentgroup_id
                    ,agent_modify_date =@agent_modify_date  where agent_id = @agent_id"
DeleteCommand="DELETE FROM gal_pvsched2Agents WHERE [pvs2agt_agent_id] = @agent_id">
                    <SelectParameters>
                        <asp:Parameter Name="agent_id" DbType="Guid" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="OverrideValue" Type="Boolean" />
                        <asp:Parameter Name="agent_id" DbType="Guid" />
                        <asp:Parameter Name="default_agentgroup_id" DbType="Guid" />
                        <asp:Parameter Name="agent_modify_date" DbType="DateTime" />
                    </UpdateParameters>
                     <DeleteParameters>
                <asp:Parameter Name="agent_id" DbType="Guid"  />
            </DeleteParameters>
                </asp:SqlDataSource>

                 <asp:SqlDataSource ID="DSAgentGroups" runat="server" 
                    ConnectionString="<%$ConnectionStrings:ApplicationServices%>" SelectCommand="select DISTINCT gal_agentgroups.agent_group_id, agent_group_name from gal_agents
inner join gal_agent2agentgroups on gal_agent2agentgroups.agent_id = gal_agents.agent_id
inner JOIN gal_agentgroups on gal_agent2agentgroups.agent_group_id = gal_agentgroups.agent_group_id
where gal_agents.agent_id = @agent_id and IsNull(gal_agentgroups.agent_group_acd_flag,0) = @bGALType" >
                    <SelectParameters>
                        <asp:Parameter Name="agent_id" DbType="Guid" />
                        <asp:Parameter Name="bGALType" Type="Boolean"  />
                    </SelectParameters>                    
                </asp:SqlDataSource>
                <td>
                    &nbsp;</td>
                <td>
                    
                </td>
            </tr>
    </table>
    <asp:Panel Enabled= "false" runat="server" ID="pnlAgentGroup">
&nbsp;<uc2:PVScheduleAgentGroup ID="PVSchedularAgentGroup" runat="server"   />
</asp:Panel>
    <uc1:PVScheduleAgent ID="PVSchedularAgent" runat="server" />
    <br />
  <asp:Button ID="btnBacktoMain0" runat="server" Text="Back to Main" style="clear: both;float: left;"
                        ValidationGroup="Override" CssClass="ButtonStyle" OnClick="btnBacktoMain0_Click" />
</asp:Content>
