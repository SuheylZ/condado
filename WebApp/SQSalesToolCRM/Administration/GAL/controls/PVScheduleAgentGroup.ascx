<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PVScheduleAgentGroup.ascx.cs" Inherits="SQS_Dialer.PVScheduleAgentGroup" %>
 
        <asp:Label ID="lblTitle" runat="server" Font-Bold="True" Font-Size="Large" Text=""></asp:Label>        
    <br />
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="#993300" />
       

 <div runat="server" id="divFormMode" style="width:500px;">
        <fieldset class="condado">
            <legend>Agent Group PV Schedule</legend>
            <ul>
                <li><asp:Label ID="lbl1" runat="server" AssociatedControlID="lstAgentGroups" Text="Agent Group" />
                    <asp:DropDownList ID="lstAgentGroups" runat="server" 
                                DataSourceID="AgentGroupList" DataTextField="agent_group_name" 
                                DataValueField="agent_group_id"  Font-Size="Small" 
                                   ValidationGroup="insertion" Enabled="false" Width="200px" />
                </li>
                <li><asp:Label ID="Label2" runat="server" AssociatedControlID="ddlHour" Text="Start Time" />
                     <asp:DropDownList ID="ddlHour" runat="server" ValidationGroup="insertion" Width="50px"/>&nbsp;Hour
                     <asp:DropDownList ID="ddlMinutes" runat="server" ValidationGroup="insertion" Width="50px" />&nbsp;Minutes
                </li>
                <li><asp:Label ID="Label3" runat="server" AssociatedControlID="ddlHour0" Text="End Time" />
                    <asp:DropDownList ID="ddlHour0" runat="server" ValidationGroup="insertion" Width="50px" />&nbsp;Hour
                    <asp:DropDownList ID="ddlMinutes0" runat="server" ValidationGroup="insertion" Width="50px" />&nbsp;Minutes
                </li>
                <li><asp:Label ID="Label4" runat="server" AssociatedControlID="txtPVMax" Text="PV Max" />
                     <asp:TextBox ID="txtPVMax" runat="server" Text="0" ValidationGroup="insertion" 
                                Width="88px"/>                            
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                ControlToValidate="txtPVMax" ErrorMessage="*" ValidationExpression="^\d+$" 
                                ValidationGroup="insertion" />
                </li>
            </ul>
        </fieldset>
     <div class="buttons">
                <asp:Button ID="InsertButton" runat="server" CausesValidation="true" CommandName="Insert" ValidationGroup="insertion" Text="Save"  OnClick="btnAddRecord_Click" />
                <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" ValidationGroup="insertion" OnClick="btnCancelRecord_Click" CommandName="Cancel" Text="Cancel" />
     </div>

                
        <br />
        

        <asp:SqlDataSource ID="AgentGroupList" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT agent_group_id, agent_group_name,o=2 FROM [gal_AgentGroups] UNION SELECT null, null, o=1 ORDER BY o, agent_group_name"></asp:SqlDataSource>


        <asp:SqlDataSource ID="AgentDataSource" runat="server" InsertCommand="INSERT INTO [gal_pvsched2agents] 
        ([pvs2agt_agent_id], [pvs2agt_start_time], [pvs2agt_end_time], [pvs2agt_pv_max]) VALUES 
        (@pvs2agt_agent_id, @pvs2agt_start_time, @pvs2agt_end_time, @pvs2agt_pv_max)">
            <InsertParameters>
                <asp:Parameter Name="pvs2agt_agent_id" DbType="Guid" />
                <asp:Parameter Name="pvs2agt_start_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_end_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_pv_max" Type="Int32" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
    
    <div runat="server" id="divGridMode">
        <asp:GridView ID="grdAgentGroupPVSchedules" runat="server" 
            CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
            OnRowCommand="grdAgentGroupPVSchedules_RowCommand"
            AutoGenerateColumns="False" AllowSorting="True"
            DataSourceID="AgentGroupPVScheduleList" DataKeyNames="pvs2agtgrp_id,pvs2agtgrp_agent_id" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" 
            Width="600px">
            <Columns>
            <asp:BoundField DataField="pvs2agtgrp_id" HeaderText="pvs2agtgrp_id" ReadOnly="True" 
                        SortExpression="pvs2agtgrp_id" Visible="False" />
                        <asp:BoundField DataField="pvs2agtgrp_agent_id" HeaderText="pvs2agtgrp_agent_id" 
                        SortExpression="pvs2agtgrp_agent_id" Visible="false" />
                <asp:BoundField DataField="agent_group_name" HeaderText="Agent Group Name" SortExpression="agent_group_name" />
                <asp:BoundField DataField="pvs2agtgrp_start_time" HeaderText="Start Time" SortExpression="pvs2agtgrp_start_time" />
                <asp:BoundField DataField="pvs2agtgrp_end_time" HeaderText="End Time" SortExpression="pvs2agtgrp_end_time" />
                <asp:BoundField DataField="pvs2agtgrp_pv_max" HeaderText="PV Max." SortExpression="pvs2agtgrp_pv_max" />
                 
                <asp:TemplateField ShowHeader="False">                   
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandArgument="<%#(Container as GridViewRow).RowIndex %>" 
                            CommandName="EditRecord" Text="Edit"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" 
                            CommandName="Delete" Text="Delete" OnClientClick='return confirm("Are you sure you want to delete this record?");'></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
                <HeaderStyle 
                    HorizontalAlign="Left" VerticalAlign="Middle" Font-Size="Small" />
                <RowStyle Font-Size="Small" />
        </asp:GridView>
        <asp:SqlDataSource ID="AgentGroupPVScheduleList" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
            OldValuesParameterFormatString="original_{0}" DeleteCommand="DELETE FROM gal_pvsched2agentgroups WHERE [pvs2agtgrp_id] = @original_pvs2agtgrp_id"
            SelectCommand="SELECT gal_pvsched2agentgroups.pvs2agtgrp_id,gal_pvsched2agentgroups.pvs2agtgrp_agent_id, gal_agentgroups.agent_group_name, 
CONVERT(VARCHAR(5),gal_pvsched2agentgroups.pvs2agtgrp_start_time,108) AS pvs2agtgrp_start_time,
CONVERT(VARCHAR(5),gal_pvsched2agentgroups.pvs2agtgrp_end_time,108) AS pvs2agtgrp_end_time,
gal_pvsched2agentgroups.pvs2agtgrp_pv_max FROM gal_agentgroups INNER JOIN gal_pvsched2agentgroups ON gal_agentgroups.agent_group_id = gal_pvsched2agentgroups.pvs2agtgrp_agent_id
where gal_pvsched2agentgroups.pvs2agtgrp_agent_id = @pvs2agtgrp_agent_id"
            UpdateCommand="UPDATE gal_pvsched2agentgroups SET pvs2agtgrp_agent_id =@pvs2agtgrp_agent_id, pvs2agtgrp_start_time =@pvs2agtgrp_start_time, pvs2agtgrp_end_time =@pvs2agtgrp_end_time, pvs2agtgrp_pv_max =@pvs2agtgrp_pv_max
            WHERE [pvs2agtgrp_id] = @original_pvs2agtgrp_id"
            InsertCommand="INSERT INTO [gal_pvsched2agentgroups] 
        ([pvs2agtgrp_agent_id], [pvs2agtgrp_start_time], [pvs2agtgrp_end_time], [pvs2agtgrp_pv_max]) VALUES 
        (@pvs2agtgrp_agent_id, @pvs2agtgrp_start_time, @pvs2agtgrp_end_time, @pvs2agtgrp_pv_max)">
        <InsertParameters>
                <asp:Parameter Name="pvs2agtgrp_agent_id" DbType="Guid" />
                <asp:Parameter Name="pvs2agtgrp_start_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agtgrp_end_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agtgrp_pv_max" Type="Int32" />
            </InsertParameters>
            <DeleteParameters>
                <asp:Parameter Name="original_pvs2agtgrp_id" Type="Int64"  />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="original_pvs2agtgrp_id" Type="Int64"/>
                <asp:Parameter Name="pvs2agtgrp_agent_id" DbType="Guid"/>
                <asp:Parameter Name="pvs2agtgrp_start_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agtgrp_end_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agtgrp_pv_max" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:Parameter Name="pvs2agtgrp_agent_id" DbType="Guid" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
        <asp:Button ID="btnBackToMain" runat="server" Text="Back to Main" 
            CausesValidation="False" CssClass="ButtonStyle" OnClick="btnBackToMain_Click" Visible="false" style="clear: both;float: left;"/>
        <asp:HiddenField ID="hdnFieldIsAgentGroup" runat="server" />
        <asp:HiddenField ID="hdnFieldRecordKey" runat="server" />
        <asp:HiddenField ID="hdnFieldEditRecordKey" runat="server" />
        <asp:HiddenField ID="hdnFieldIsFormMode" runat="server" />
    </div>