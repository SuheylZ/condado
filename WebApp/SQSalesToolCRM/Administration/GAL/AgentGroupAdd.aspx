<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgentGroupAdd.aspx.cs" Inherits="SQS_Dialer.AgentGroupAdd" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
     <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" />
    <br />
    <asp:FormView ID="FormView1" runat="server" DataKeyNames="agent_id" 
        DataSourceID="AgentGroupDataSource" DefaultMode="Insert" OnItemCommand="FormView1_ItemCommand" OnItemInserted="FormView1_ItemInserted" OnItemInserting="FormView1_ItemInserting">
        <InsertItemTemplate>
            <fieldset class="condado">
                <legend runat="server" id="FormTitle">Agent Group Add</legend>
            <ul>
                <li>
                    <asp:Label ID="l1" runat="server" AssociatedControlID="txtAgentGroupName" Text="Agent Group Name" />
                    <asp:TextBox ID="txtAgentGroupName" runat="server" Text='<%# Bind("agent_group_name") %>'/>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ErrorMessage="Agent group name is required" ControlToValidate="txtAgentGroupName">*</asp:RequiredFieldValidator>
                </li>
                <li>
                    <asp:Label ID="Label1" runat="server" AssociatedControlID="txtAgentGroupQuota" Text="Agent Group Max Daily Quota" />
                     <asp:TextBox ID="txtAgentGroupQuota" runat="server" Text='<%# Bind("agent_group_max_daily") %>' />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" 
                            ControlToValidate="txtAgentGroupQuota" 
                            ErrorMessage="Daily quota must be an integer" Operator="DataTypeCheck" 
                            Type="Integer">*</asp:CompareValidator>

                </li>
                <li>
                    <asp:Label ID="Label3" runat="server" AssociatedControlID="agent_inactiveCheckBox" Text="Inactive" />
                     <asp:CheckBox ID="agent_inactiveCheckBox" runat="server" 
                            Checked='<%# Bind("agent_group_inactive") %>' />
                </li>

            </ul>
            
                <div class="buttons">
            <asp:Button ID="InsertButton" runat="server" CausesValidation="true" Text="Save" CommandName="Insert" OnClientClick='return confirm("Are you sure you want to create this agent group?");' />
            &nbsp;
            <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" Text="Cancel" CommandName="Cancel" />
                    </div>
            </fieldset>

        </InsertItemTemplate>
    </asp:FormView>
    <br />
   
    <asp:SqlDataSource ID="AgentGroupDataSource" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
        SelectCommand="SELECT * FROM [gal_AgentGroups]" 
        InsertCommand="INSERT INTO [gal_AgentGroups] ([agent_group_name], [agent_group_max_daily], [agent_group_add_date], [agent_group_inactive],[agent_group_acd_flag]) VALUES (@agent_group_name, @agent_group_max_daily, @agent_group_add_date, @agent_group_inactive,@agent_group_acd_flag)">
        <InsertParameters>
            <asp:Parameter Name="agent_group_name" Type="String" />
            <asp:Parameter Name="agent_group_max_daily" Type="Int32" />
            <asp:Parameter Name="agent_group_add_date" Type="DateTime" />
            <asp:Parameter Name="agent_group_inactive" Type="Boolean" />
            <asp:Parameter Name="agent_group_acd_flag" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
    </asp:Content>
