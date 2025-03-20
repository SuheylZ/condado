<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="StateGroupAdd.aspx.cs" Inherits="SQS_Dialer.StateGroupAdd" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
      <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" />
    <br />
    <asp:FormView ID="FormView1" runat="server" DataKeyNames="state_group_id" Width="400px"
        DataSourceID="StateGroupDataSource" DefaultMode="Insert" 
        OnItemCommand="FormView1_ItemCommand" OnItemInserted="FormView1_ItemInserted" OnItemInserting="FormView1_ItemInserting">
        <InsertItemTemplate>
            <fieldset class="condado">
                <legend> State Group Add</legend>
                <ul>
                    <li>
                        <asp:Label ID="lbl1" runat="server" AssociatedControlID="state_group_name" Text="Name" />
                        <asp:TextBox ID="state_group_name" runat="server" Text='<%# Bind("state_group_name") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="State Group Name Required" ControlToValidate='state_group_name' Text="*" />
                    </li>
                    <li>
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="DropDownList2" Text="Priority" />
                        <asp:DropDownList ID="DropDownList2" runat="server" SelectedValue='<%# Bind("state_group_priority") %>' OnLoad="priority_build" Width="200px" />
                    </li>
                    <li>
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="state_group_inactive" Text="Inactive" />
                        <asp:CheckBox ID="state_group_inactive" runat="server" Checked='<%# Bind("state_group_inactive") %>' />
                    </li>
                </ul>
            </fieldset>

            <div class="buttons">
            <asp:Button ID="InsertButton" runat="server" CausesValidation="true"
                CommandName="Insert" Text="Save"
                OnClientClick='return confirm("Are you sure you want to insert this state group?");' />
            <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False"
                CommandName="Cancel" Text="Cancel" />
            </div>
        </InsertItemTemplate>
        
    </asp:FormView>
    
    <asp:SqlDataSource ID="StateGroupDataSource" runat="server" 
        InsertCommand="INSERT INTO [gal_StateGroups] ([state_group_name], [state_group_priority], [state_group_add_date], [state_group_inactive],[state_group_acd_flag]) VALUES (@state_group_name, @state_group_priority, @state_group_add_date, @state_group_inactive,@state_group_acd_flag)">
        <InsertParameters>
            <asp:Parameter Name="state_group_id" Type="Object" />
            <asp:Parameter Name="state_group_name" Type="String" />
            <asp:Parameter Name="state_group_priority" Type="Int32" />
            <asp:Parameter Name="state_group_inactive" Type="Boolean" />
            <asp:Parameter Name="state_group_add_date" Type="DateTime" ConvertEmptyStringToNull="false" />
             <asp:Parameter Name="state_group_acd_flag" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
    </asp:Content>
