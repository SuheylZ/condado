<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgeGroupAdd.aspx.cs" Inherits="SQS_Dialer.AgeGroupAdd" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <br />
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" />
    <br />
    <asp:FormView ID="FormView1" runat="server" DataKeyNames="age_group_id"  Width="400px"
        DataSourceID="AgeGroupDataSource" DefaultMode="Insert" 
        OnItemCommand="FormView1_ItemCommand" OnItemInserted="FormView1_ItemInserted" OnItemInserting="FormView1_ItemInserting">
        <InsertItemTemplate>
            <fieldset class="condado">
                <legend runat="server" id="lgnd">Age Group Add</legend>
                <ul>
                    <li>
                        <asp:Label ID="lbl1" runat="server" AssociatedControlID="age_group_start" Text="Age Group Start" />
                        <asp:TextBox ID="age_group_start" runat="server" Text='<%# Bind("age_group_start") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Age Group Start Required" ControlToValidate='age_group_start' Text="*" Display="Dynamic"/>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="Age Group Start Must Be An Integer" ControlToValidate="age_group_start" Type="Integer" Text="*" Operator="DataTypeCheck" Display="Dynamic"/>
                    </li>
                    <li>
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="age_group_end" Text="Age Group End" />
                        <asp:TextBox ID="age_group_end" runat="server" Text='<%# Bind("age_group_end") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Age Group End Required" ControlToValidate='age_group_end' Text="*" Display="Dynamic" />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Age Group End Must Be An Integer" ControlToValidate="age_group_end" Type="Integer" Text="*" Operator="DataTypeCheck" Display="Dynamic" />
                    </li>
                    <li>
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="age_group_inactive" Text="Inactive" />
                        <asp:CheckBox ID="age_group_inactive" runat="server" Checked='<%# Bind("age_group_inactive") %>' />
                    </li>                
                </ul>
            </fieldset>
            <div class="buttons">
                <asp:Button ID="InsertButton" runat="server" CausesValidation="true" CommandName="Insert" Text="Save" />
                <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
            </div>
        </InsertItemTemplate>
        
    </asp:FormView>
    <br />
    
    <asp:SqlDataSource ID="AgeGroupDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicationServices %>"     
        InsertCommand="INSERT INTO [gal_AgeGroups] ([age_group_start], [age_group_end], [age_group_add_date], [age_group_inactive],[age_group_acd_flag]) VALUES (@age_group_start, @age_group_end, @age_group_add_date, @age_group_inactive,@age_group_acd_flag)">
        <InsertParameters>
            <asp:Parameter Name="age_group_id" Type="Object" />
            <asp:Parameter Name="age_group_start" Type="String" />
            <asp:Parameter Name="age_group_end" Type="Int32" />
            <asp:Parameter Name="age_group_inactive" Type="Boolean" />
            <asp:Parameter Name="age_group_add_date" Type="DateTime" ConvertEmptyStringToNull="false" />
            <asp:Parameter Name="age_group_acd_flag" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
    </asp:Content>
