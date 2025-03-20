<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Site.master" CodeFile="AcdStatisticsView.aspx.cs" Inherits="Administration_GAL_AcdStatisticsView" %>


<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div>
        <div style="display: block">
            <asp:DropDownList runat="server" ID="agentDropdownlist" DataSourceID="agents" DataTextField="Name" DataValueField="usr_key" />
            <asp:Button runat="server" Text="Reload / Filter" OnClick="filterResult_Click" />
            <asp:SqlDataSource runat="server" ID="agents"
                DataSourceMode="DataReader"
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
                SelectCommand="  SELECT  A.*
        FROM    ( SELECT    u.usr_key ,
                            u.usr_first_name + ' ' + u.usr_last_name AS Name
                  FROM      dbo.users u
                            JOIN dbo.gal_agents g ON u.usr_key = g.agent_id
                  UNION
                  SELECT    NULL ,
                            ''
                ) A
        ORDER BY A.Name"></asp:SqlDataSource>
        </div>
        <telerik:RadGrid runat="server" ID="statGrid" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt" GridLines="None"
                CssClass="mGrid" Skin="" EnableTheming="False" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt">
             <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn DataField="qas_agent_id" HeaderText="AgentId" />
                <telerik:GridBoundColumn DataField="qas_acd_count" HeaderText="ACD Leads Count" />
                <telerik:GridBoundColumn DataField="qas_max_acd" HeaderText="Max ACD" />
                <telerik:GridBoundColumn DataField="qas_max_quota" HeaderText="Max Quota" />
                <telerik:GridBoundColumn DataField="qas_min_level" HeaderText="Level" />
                <telerik:GridBoundColumn DataField="qas_next_refresh" HeaderText="Next Refresh" />
                <telerik:GridBoundColumn DataField="qas_acd_call_taken" HeaderText="Taken Calls" />
                <telerik:GridBoundColumn DataField="qas_PV_schedule_result" HeaderText="PV Schedule Passed?" />
                <telerik:GridBoundColumn DataField="qas_is_enabled" HeaderText="IsEnalbed" />
                <telerik:GridBoundColumn DataField="qas_reason" HeaderText="Reason" />
            </Columns>
                 </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>
