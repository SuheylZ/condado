<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentManager.aspx.cs" Inherits="SQS_Dialer.AgentManager" Culture="en-US" UICulture="en-US" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<%@ Register src="../../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <h2><asp:Label ID="lblAgentManagerTitle" runat="server" Text="Agent Management"></asp:Label></h2>
<uc1:PagingBar ID="pager" runat="server" NewButtonTitle="" />
<br />

			<asp:GridView ID="GridView1" runat="server" 
				CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
				
				AutoGenerateColumns="False" DataKeyNames="agent_id,agent_default_agent_group_id" 
				DataSourceID="AgentManagerDataSource" Width="100%" AllowSorting="True" 
				HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" 
				OnRowCommand="GridView1_RowCommand" OnRowEditing="GridView1_RowEditing" OnRowUpdated="GridView1_RowUpdated" OnRowUpdating="GridView1_RowUpdating" AllowPaging="True" ShowFooter="True" ShowHeaderWhenEmpty="True">
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                 <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
				<Columns>
					<asp:BoundField DataField="agent_id" HeaderText="agent_id" ReadOnly="True" 
						SortExpression="agent_id" Visible="False" />
                    <asp:BoundField DataField="agent_group_id" HeaderText="agent_group_id" ReadOnly="True" 
						SortExpression="agent_group_id" Visible="False" />
					<asp:TemplateField HeaderText="Name" SortExpression="AgentName" 
						HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
						<EditItemTemplate>
							<asp:Label ID="Label2" runat="server" Text='<%# Bind("AgentName") %>'></asp:Label>
						</EditItemTemplate>
						<ItemTemplate>
							<asp:Label ID="Label2" runat="server" Text='<%# Bind("AgentName") %>'></asp:Label>
						</ItemTemplate>
						<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%"/>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Groups" SortExpression="agent_group_name" >
					    <EditItemTemplate>
							<asp:DropDownList ID="lstAgentGroups" runat="server" ToolTip="Default assigned agent group"
								DataSource='<%#GetDataSource(Eval("agent_id"),false) %>' DataTextField="agent_group_name" 
								DataValueField="agent_group_id" SelectedValue='<%# Bind("agent_default_agent_group_id") %>' Font-Size="Small" Width="180px">
							</asp:DropDownList>
                            <br />
						</EditItemTemplate>
						<ItemTemplate>
							<asp:Label ID="lblAgentGroup" runat="server" Text='<%#Server.HtmlDecode(Eval("AgentGroups").ToString())%>'></asp:Label>
                           
						</ItemTemplate>
						<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
					</asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Groups" SortExpression="agent_group_name" >
					    <EditItemTemplate>
							<asp:DropDownList ID="lstAgentGroups_Acd" runat="server" ToolTip="Default assigned agent group"
								DataSource='<%#GetDataSource(Eval("agent_id"),true) %>' DataTextField="agent_group_name" 
								DataValueField="agent_group_id" SelectedValue='<%# Bind("agent_default_agent_group_id_acd") %>' Font-Size="Small" Width="180px">
							</asp:DropDownList>
                            <br />
						</EditItemTemplate>
						<ItemTemplate>
							<asp:Label ID="lblAgentGroup_acd" runat="server" Text='<%#Server.HtmlDecode(Eval("AgentGroupsACD").ToString())%>'></asp:Label>
                           
						</ItemTemplate>
						<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
					</asp:TemplateField>
				  
					<asp:BoundField DataField="agent_max_daily_leads"  
						HeaderText="Max Daily Web Leads" SortExpression="agent_max_daily_leads" 
						Visible="True" HeaderStyle-HorizontalAlign="Left" 
						HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%"></HeaderStyle>
					</asp:BoundField>
                    <asp:BoundField DataField="agent_max_daily_acd_leads"  
						HeaderText="Max Daily ACD Leads" SortExpression="agent_max_daily_acd_leads" 
						Visible="True" HeaderStyle-HorizontalAlign="Left" 
						HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%"></HeaderStyle>
					</asp:BoundField>
					  <asp:TemplateField HeaderText="First Call" SortExpression="agent_first_call" 
						HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
						<EditItemTemplate>
							<asp:TextBox ID="txtAgentFirstCall" runat="server" Text='<%# Bind("agent_first_call") %>' Width="90%"></asp:TextBox>
							<ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtAgentFirstCall" Format="MM/dd/yyyy" >
							</ajaxToolkit:CalendarExtender>
							
							<asp:RegularExpressionValidator ID="regXValidatorDateTime" runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="txtAgentFirstCall" ValidationExpression="^(?=\d)(?:(?:(?:(?:(?:0?[13578]|1[02])(\/|-|\.)31)\1|(?:(?:0?[1,3-9]|1[0-2])(\/|-|\.)(?:29|30)\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})|(?:0?2(\/|-|\.)29\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))|(?:(?:0?[1-9])|(?:1[0-2]))(\/|-|\.)(?:0?[1-9]|1\d|2[0-8])\4(?:(?:1[6-9]|[2-9]\d)?\d{2}))($|\ (?=\d)))?(((0?[1-9]|1[012])(:[0-5]\d){0,2}(\ [AaPp][Mm]))|([01]\d|2[0-3])(:[0-5]\d){1,2})?$" Width="5px"></asp:RegularExpressionValidator>
						</EditItemTemplate>
						<ItemTemplate>
							<asp:Label ID="lblAgentFirstCallEdit" runat="server" Text='<%# Bind("agent_first_call") %>' ></asp:Label>                            
							
						</ItemTemplate>                        
						<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="150px" />
					</asp:TemplateField>
					<asp:CheckBoxField DataField="agent_inactive" HeaderText="Inactive?" 
						SortExpression="agent_inactive" HeaderStyle-HorizontalAlign="Left" 
						HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
					</asp:CheckBoxField>
					<asp:BoundField DataField="agent_modify_date" HeaderText="Modified On" 
						ReadOnly="True" SortExpression="agent_modify_date" 
						HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" DataFormatString="{0: MM/dd/yyyy hh:mm:ss tt}">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
					</asp:BoundField>
					<asp:CommandField ShowEditButton="True" 
						ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
						DeleteImageUrl="~/Images/gal/remove.png" EditImageUrl="~/Images/gal/edit.png" 
						UpdateImageUrl="~/Images/gal/check.png" EditText="Edit Tooltip" />
					<asp:TemplateField ShowHeader="False" Visible="False">
						<ItemTemplate>
							<asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
								CommandName="Delete" ImageUrl="~/Images/gal/remove.png" Text="Delete" ToolTip="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");' />
						</ItemTemplate>
					</asp:TemplateField>
					 <asp:ButtonField ButtonType="Image" CommandName="AgentsPVScheduleEdit" 
						Text="AgentsPVSchedule" ImageUrl="~/Images/gal/Clock-icon-32.png"   />
                  <%--  <asp:TemplateField>
    <ItemTemplate>
       <asp:ImageButton ID="ImageButto213n1" runat="server" AlternateText="My Text"  CommandName="AgentsPVScheduleEdit" CommandArgument='<%# Container.DataItemIndex %>' ImageUrl="~/Images/gal/Clock-icon-32.png"  />
    </ItemTemplate>
</asp:TemplateField>--%>

					<asp:ButtonField ButtonType="Image" CommandName="CampaignEdit" 
						Text="Campaigns" ImageUrl="~/Images/gal/skill.png" Visible="false" />
                    <asp:ButtonField ButtonType="Image" CommandName="AgentsEdit" 
                        Text="Agents" ImageUrl="~/Images/gal/skill.png" Visible="true" />
				</Columns>
				<HeaderStyle 
					HorizontalAlign="Left" VerticalAlign="Middle" Font-Size="Small" />
				<PagerSettings Mode="NextPreviousFirstLast" Position="TopAndBottom" Visible="False" />
				<RowStyle Font-Size="Small" />
			</asp:GridView>

			<asp:SqlDataSource ID="AgentManagerDataSource" runat="server" 
				ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
				SelectCommand=" SELECT [gal_Agents].agent_id , agent_default_agent_group_id,agent_default_agent_group_id_acd,
                                agent_max_daily_leads ,
                                agent_max_daily_acd_leads,
                                agent_inactive ,
                                agent_add_date ,
                                agent_modify_date ,
                                AgentName = usr_last_name + ', ' + usr_first_name ,
                                AgentGroups=dbo.fn_gal_AgentGroupsNames(dbo.gal_agents.agent_id)  ,
                                AgentGroupsACD = dbo.fn_gal_AgentGroupsNames_Acd(dbo.gal_agents.agent_id) ,
                                agent_first_call 
                         FROM   [gal_Agents]
                                JOIN [users] ON agent_id = usr_key                                
                         WHERE  [agent_delete_flag] = 0 AND usr_delete_flag=0 
                         ORDER BY AgentName"
				DeleteCommand="UPDATE [gal_Agents] SET [agent_delete_flag] = 1 WHERE [agent_id] = @original_agent_id" 
				OldValuesParameterFormatString="original_{0}" >
				<DeleteParameters>
					<asp:Parameter Name="original_agent_id" Type="Object" />
				</DeleteParameters>
				<UpdateParameters>
					<asp:Parameter Name="agent_max_daily_leads" Type="Int32" />
					<asp:Parameter Name="agent_max_daily_acd_leads" Type="Int32" />
					<asp:Parameter Name="agent_inactive" Type="Boolean" />
					<asp:Parameter DefaultValue="6/1/2011 12:00 PM" Name="agent_modify_date" Type="DateTime" />
					<asp:Parameter Name="agent_first_call" DbType="DateTime"  />
					<asp:Parameter Name="original_agent_id" Type="Object" />
					<%--<asp:Parameter Name="agent_default_agent_group_id" Type="Object" />
					<asp:Parameter Name="agent_default_agent_group_id_acd" Type="Object" />--%>
				</UpdateParameters>
                <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
			</asp:SqlDataSource>
			<%--<asp:SqlDataSource ID="AgentGroupList" runat="server" 
				ConnectionString="<%$ConnectionStrings:ApplicationServices%>" 
				SelectCommand="SELECT  G.agent_group_id ,G.agent_group_name ,o = 2 FROM    [gal_AgentGroups] G JOIN dbo.gal_agent2agentgroups ag ON ag.agent_group_id = G.agent_group_id AND AG.agent_id = @agentId
                                WHERE   ISNULL([agent_group_acd_flag], 0) = @bGALType
                                UNION SELECT  NULL , NULL , o = 1
                                ORDER BY o ,agent_group_name">
                <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                    <asp:Parameter Name="agentId" Type="Object"  />
                </SelectParameters>
			</asp:SqlDataSource>--%>
			<br />
			<asp:ValidationSummary ID="ValidationSummary1" runat="server" 
				ShowMessageBox="True" ShowSummary="False" />

</asp:Content>
