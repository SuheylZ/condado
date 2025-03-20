<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentGroupModify.aspx.cs" Inherits="SQS_Dialer.AgentGroupModify" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>
<%@ Register src="../../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"> </asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2><asp:Label ID="lblAgentManagerTitle" runat="server" Text="Agent Group Management"/></h2>



            <uc1:PagingBar ID="pager" runat="server" NewButtonTitle="Add Agent Group" OnNewRecord="btnAddAgent_Click" />

            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="agent_group_id" 
                DataSourceID="AgentGroupDataSource" Width="100%" AllowSorting="True" 
                 CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" AllowPaging="True" 
                OnRowCommand="GridView1_RowCommand" OnRowUpdating="GridView1_RowUpdating" OnRowUpdated="GridView1_RowUpdated" ShowHeaderWhenEmpty="True">
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField DataField="agent_group_id" HeaderText="agent_group_id" ReadOnly="True" 
                        SortExpression="agent_group_id" Visible="False" />
                    <asp:BoundField DataField="agent_group_name" HeaderText="Agent Group Name" 
                        SortExpression="agent_group_name" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="agent_group_max_daily" 
                        HeaderText="Max Daily Web Leads" SortExpression="agent_group_max_daily" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>   
                    <asp:BoundField DataField="agent_group_max_daily_acd" 
                        HeaderText="Max Daily ACD Leads" SortExpression="agent_group_max_daily_acd" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:CheckBoxField DataField="agent_group_inactive" HeaderText="Inactive?" 
                        SortExpression="agent_group_inactive" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="agent_group_add_date" HeaderText="Add Date" 
                        ReadOnly="True" SortExpression="agent_group_add_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="agent_group_modify_date" HeaderText="Modified Date" 
                        ReadOnly="True" SortExpression="agent_group_modify_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:CommandField ShowEditButton="True" 
                        ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
                        DeleteImageUrl="~/Images/gal/remove.png" EditImageUrl="~/Images/gal/edit.png" 
                        UpdateImageUrl="~/Images/gal/check.png" />
                    <asp:TemplateField ShowHeader="False" Visible="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                CommandName="Delete" ImageUrl="~/Images/gal/remove.png" Text="Delete" ToolTip="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");' />
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:ButtonField ButtonType="Image" CommandName="AgentsEdit" 
                        Text="Agents" ImageUrl="~/Images/gal/skill.png" Visible="true" />
                        <asp:ButtonField ButtonType="Image" CommandName="AgentsPVScheduleEdit" 
                        Text="AgentsPVSchedule" ImageUrl="~/Images/gal/Clock-icon-32.png" Visible="true" />
                </Columns>

                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>

<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="gridHeader"></HeaderStyle>
                <PagerSettings Visible="False" />
            </asp:GridView>

            <asp:SqlDataSource ID="AgentGroupDataSource" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT agent_group_id, agent_group_name, agent_group_max_daily,agent_group_max_daily_acd, agent_group_inactive, agent_group_add_date, agent_group_modify_date, agent_group_delete_flag, agent_group_acd_flag FROM [gal_AgentGroups] WHERE [agent_group_delete_flag] = 0 and IsNULL(agent_group_acd_flag,0)= @bGALType ORDER BY agent_group_name"
                DeleteCommand="UPDATE [gal_AgentGroups] SET [agent_group_delete_flag] = 1 WHERE [agent_group_id] = @original_agent_group_id" 
                OldValuesParameterFormatString="original_{0}"                
                UpdateCommand="UPDATE [gal_AgentGroups] SET [agent_group_name] = @agent_group_name, [agent_group_inactive] = @agent_group_inactive, [agent_group_modify_date] = @agent_group_modify_date, [agent_group_max_daily] = @agent_group_max_daily, [agent_group_max_daily_acd] = @agent_group_max_daily_acd WHERE [agent_group_id] = @original_agent_group_id">
                <DeleteParameters>
                    <asp:Parameter Name="original_agent_group_id" Type="Object" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="agent_group_name" Type="String" />
                    <asp:Parameter Name="agent_group_max_daily" Type="Int32" />
                    <asp:Parameter Name="agent_group_max_daily_acd" Type="Int32" />
                    <asp:Parameter Name="agent_group_inactive" Type="Boolean" />
                    <asp:Parameter Name="agent_group_add_date" Type="DateTime" />
                    <asp:Parameter DefaultValue="6/1/2011 12:00 PM" Name="agent_group_modify_date" 
                        Type="DateTime" />
                    <asp:Parameter Name="original_agent_group_id" Type="Object" />
                </UpdateParameters>
                  <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ShowMessageBox="True" ShowSummary="False" />

</asp:Content>
