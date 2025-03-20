<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PVScheduleAgent.ascx.cs" Inherits="SQS_Dialer.PVScheduleAgent" %>
   
       <asp:Label ID="lblErrorMessage" runat="server" ForeColor="#993300" />
       
  <div runat="server" id="divFormMode" style="width:500px;">

      <fieldset class="condado">
          <legend>Agent PV Schedule</legend>
          <ul>
              <li>
                  <asp:Label ID="lbl1" runat="server" AssociatedControlID="lstAgents" Text="Agent" />
                   <asp:DropDownList ID="lstAgents" runat="server" 
                                DataSourceID="AgentList" DataTextField="AgentName" 
                                DataValueField="agent_id"  Font-Size="Small" 
                                   ValidationGroup="insertion" Enabled="false" Width="200px" />
              </li>
              <li>
                  <asp:Label ID="Label2" runat="server" AssociatedControlID="ddlHour" Text="Start Time" />
                    <asp:DropDownList ID="ddlHour" runat="server" ValidationGroup="insertion" Width="50px"/>&nbsp;&nbsp;Hour &nbsp;
                    <asp:DropDownList ID="ddlMinutes" runat="server" ValidationGroup="insertion" Width="50px" />&nbsp;Minutes
              </li>
              <li>
                  <asp:Label ID="Label3" runat="server" AssociatedControlID="ddlHour0" Text="End Time" />
                   <asp:DropDownList ID="ddlHour0" runat="server" ValidationGroup="insertion" Width="50px"/>&nbsp;&nbsp;Hour &nbsp;
                   <asp:DropDownList ID="ddlMinutes0" runat="server" ValidationGroup="insertion" Width="50px"/>&nbsp;Minutes
              </li>
              <li>
                  <asp:Label ID="Label4" runat="server" AssociatedControlID="txtPVMax" Text="PV Max" />
                <asp:TextBox ID="txtPVMax" runat="server" Text="0" ValidationGroup="insertion" />                            
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                ControlToValidate="txtPVMax" ErrorMessage="*" ValidationExpression="^\d+$" 
                                ValidationGroup="insertion"></asp:RegularExpressionValidator>
              </li>

          </ul>

      </fieldset>

      <div class="buttons">
                <asp:Button ID="btnInsert" runat="server" CausesValidation="true" CommandName="Insert" ValidationGroup="insertion"
                    Text="Save"  OnClick="btnAddRecord_Click" />
                <asp:Button ID="btnCancel" runat="server" CausesValidation="False" ValidationGroup="insertion" OnClick="btnCancelRecord_Click"
                    CommandName="Cancel" Text="Cancel" />
      </div>

        <asp:SqlDataSource ID="AgentList" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"                     
                    SelectCommand=" SELECT   AgentName = usr_last_name + ', ' + usr_first_name, agent_id
                                    FROM         gal_Agents
                                    JOIN users on agent_id = usr_key 
                                    WHERE     usr_delete_flag = 0 
                                    ORDER BY AgentName">
            <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
                </asp:SqlDataSource>
        </div>
    
    <div runat="server" id="divGridMode">
        <asp:GridView ID="grdAgentPVSchedules" runat="server" 
            AutoGenerateColumns="False" AllowSorting="True"
             CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
            DataSourceID="AgentPVScheduleList" DataKeyNames="pvs2agt_id,pvs2agt_agent_id" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" 
            Width="600px" OnRowCommand="grdAgentPVSchedules_RowCommand">
            <Columns>
            <asp:BoundField DataField="pvs2agt_id" HeaderText="pvs2agt_id" ReadOnly="True" 
                        SortExpression="pvs2agt_id" Visible="False" />
                        <asp:BoundField DataField="pvs2agt_agent_id" HeaderText="pvs2agt_agent_id" 
                        SortExpression="pvs2agt_agent_id" Visible="false" />
                <asp:BoundField DataField="AgentName" HeaderText="Agent Name" SortExpression="AgentName" />
                <asp:BoundField DataField="pvs2agt_start_time" HeaderText="Start Time" SortExpression="pvs2agt_start_time" />
                <asp:BoundField DataField="pvs2agt_end_time" HeaderText="End Time" SortExpression="pvs2agt_end_time" />
                <asp:BoundField DataField="pvs2agt_pv_max" HeaderText="PV Max." SortExpression="pvs2agt_pv_max" />
                 
                <asp:TemplateField ShowHeader="False">                   
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandArgument="<%# (Container as GridViewRow).RowIndex %>" 
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
        <asp:SqlDataSource ID="AgentPVScheduleList" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"
            OldValuesParameterFormatString="original_{0}" DeleteCommand="DELETE FROM gal_pvsched2Agents WHERE [pvs2agt_id] = @original_pvs2agt_id"
            SelectCommand="SELECT gal_pvsched2Agents.pvs2agt_id,gal_pvsched2Agents.pvs2agt_agent_id, usr_last_name + ', ' + usr_first_name as AgentName, agent_id , 
CONVERT(VARCHAR(5),gal_pvsched2Agents.pvs2agt_start_time,108) AS pvs2agt_start_time,
CONVERT(VARCHAR(5),gal_pvsched2Agents.pvs2agt_end_time,108) AS pvs2agt_end_time,
gal_pvsched2Agents.pvs2agt_pv_max FROM 
gal_Agents INNER JOIN gal_pvsched2Agents ON gal_Agents.agent_id = gal_pvsched2Agents.pvs2agt_agent_id
JOIN users on gal_Agents.agent_id = users.usr_key
WHERE pvs2agt_agent_id = @pvs2agt_agent_id"
            UpdateCommand="UPDATE gal_pvsched2Agents SET pvs2agt_agent_id =@pvs2agt_agent_id, 
pvs2agt_start_time =@pvs2agt_start_time, pvs2agt_end_time =@pvs2agt_end_time, 
pvs2agt_pv_max =@pvs2agt_pv_max
            WHERE pvs2agt_id = @original_pvs2agt_id"
            InsertCommand="INSERT INTO [gal_pvsched2agents] 
        ([pvs2agt_agent_id], [pvs2agt_start_time], [pvs2agt_end_time], [pvs2agt_pv_max]) VALUES 
        (@pvs2agt_agent_id, @pvs2agt_start_time, @pvs2agt_end_time, @pvs2agt_pv_max)">
        <InsertParameters>
                 <asp:Parameter Name="pvs2agt_agent_id" DbType="Guid" />
                <asp:Parameter Name="pvs2agt_start_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_end_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_pv_max" Type="Int32" />
            </InsertParameters>
            <DeleteParameters>
                <asp:Parameter Name="original_pvs2agt_id" Type="Int64"  />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="original_pvs2agt_id" Type="Int64"/>
                <asp:Parameter Name="pvs2agt_agent_id" DbType="Guid"/>
                <asp:Parameter Name="pvs2agt_start_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_end_time" Type="DateTime" />
                <asp:Parameter Name="pvs2agt_pv_max" Type="Int32" />
            </UpdateParameters>
             <SelectParameters>
                <asp:Parameter Name="pvs2agt_agent_id" DbType="Guid" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
        <asp:Button ID="btnBackToMain" runat="server" Text="Back to Main" 
            CausesValidation="False" CssClass="ButtonStyle" OnClick="btnBackToMain_Click" Visible="false" />
        <asp:HiddenField ID="hdnFieldIsAgent" runat="server" />
        <asp:HiddenField ID="hdnFieldRecordKey" runat="server" />
        <asp:HiddenField ID="hdnFieldEditRecordKey" runat="server" />
        <asp:HiddenField ID="hdnFieldIsFormMode" runat="server" />
    </div>