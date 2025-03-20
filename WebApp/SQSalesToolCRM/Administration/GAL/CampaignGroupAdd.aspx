<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="CampaignGroupAdd.aspx.cs" Inherits="SQS_Dialer.CampaignGroupAdd" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    
    <br />
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowMessageBox="True" />
    <br/>
    <asp:FormView ID="FormView1" runat="server" DataKeyNames="agent_id" 
        DataSourceID="CampaignGroupDataSource" DefaultMode="Insert" Width="557px" 
        OnItemCommand="FormView1_ItemCommand" OnItemInserted="FormView1_ItemInserted" OnItemInserting="FormView1_ItemInserting">
        <EditItemTemplate>
            <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update" Text="Update" />
            &nbsp;
            <asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
        </EditItemTemplate>
        
        
        <InsertItemTemplate>
            <fieldset class="condado">
                <legend> Add Campaign Group</legend>
                <ul>
                    <li>
                        <asp:Label ID="l1" runat="server" AssociatedControlID="campaign_group_name_text_box" Text="Campaign Group Name" />
                         <asp:TextBox ID="campaign_group_name_text_box" runat="server" Text='<%# Bind("campaign_group_name") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Campaign Group Name Required" ControlToValidate='campaign_group_name_text_box' Text="*"></asp:RequiredFieldValidator>
                    </li>
                    <li>
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="priority" Text="Priority" />
                        <asp:DropDownList ID="priority" runat="server" OnLoad="priority_build" Width="50px" SelectedValue='<%# Bind("campaign_group_priority") %>'/>
                    </li>
                    <li>
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="TextBox1" Text="Level 1" />
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("campaign_group_level1") %>' />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Level 1 Required" ControlToValidate='TextBox1' Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="CompareValidator0" runat="server" ErrorMessage="Level 1 must be a number between 0 and 9999" ControlToValidate="TextBox1" Type="Integer" Text="*" Operator="DataTypeCheck" Display="Dynamic"></asp:CompareValidator>
                    </li>
                    <li>
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="TextBox2" Text="Level 2" />
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("campaign_group_level2") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Level 2 Required" ControlToValidate='TextBox2' Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Level 2 must be a number between 0 and 9999" ControlToValidate="TextBox2" Type="Integer" Text="*" Operator="DataTypeCheck" Display="Dynamic"></asp:CompareValidator>
                    </li>
                    <li>
                        <asp:Label ID="Label4" runat="server" AssociatedControlID="TextBox3" Text="Level 3" />
                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("campaign_group_level3") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Level 3 Required" ControlToValidate='TextBox3' Text="*" Display="Dynamic"/>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="Level 3 must be a number between 0 and 9999" ControlToValidate="TextBox3" Type="Integer" Text="*" Operator="DataTypeCheck" Display="Dynamic"/>
                    </li>
                    <li>
                        <asp:Label ID="Label5" runat="server" AssociatedControlID="TextBox4" Text="Level 4" />
                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("campaign_group_level4") %>' />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Level 4 Required" ControlToValidate='TextBox4' Text="*" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="CompareValidator3" runat="server" 
                            ErrorMessage="Level 4 must be a number between 0 and 9999" 
                            ControlToValidate="TextBox4" Type="Integer" Text="*" Operator="DataTypeCheck" 
                            Display="Dynamic"/>
                    </li>
                    <li>
                        <asp:Label ID="Label6" runat="server" AssociatedControlID="CheckBox1" Text="Inactive" />
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("campaign_group_inactive") %>' />

                    </li>
                </ul>
             </fieldset>
            
            <div class="buttons">
                <asp:Button ID="InsertButton" runat="server" CausesValidation="true" CommandName="Insert" Text="Save" OnClientClick='return confirm("Are you sure you want to insert this campaign group?");' />
                <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
            </div>

        </InsertItemTemplate>
        
    </asp:FormView>
    <br />
    
    <asp:SqlDataSource ID="CampaignGroupDataSource" runat="server" SelectCommand="SELECT [campaign_group_name], [campaign_group_priority], [campaign_group_level1], [campaign_group_level2], [campaign_group_level3], [campaign_group_level4], [campaign_group_inactive], [campaign_group_add_date] FROM gal_CampaignGroups"
        InsertCommand="INSERT INTO [gal_CampaignGroups] ([campaign_group_name], [campaign_group_priority], [campaign_group_level1], [campaign_group_level2], [campaign_group_level3], [campaign_group_level4], [campaign_group_inactive], [campaign_group_add_date],[campaign_group_acd_flag]) VALUES (@campaign_group_name, @campaign_group_priority, @campaign_group_level1, @campaign_group_level2, @campaign_group_level3, @campaign_group_level4, @campaign_group_inactive, @campaign_group_add_date, @campaign_group_acd_flag)" ConnectionString="<%$ConnectionStrings:ApplicationServices%>">
        <SelectParameters>
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="campaign_group_name" Type="String" />
            <asp:Parameter Name="campaign_group_priority" Type="Int32" />
            <asp:Parameter Name="campaign_group_level1" Type="Int32" />
            <asp:Parameter Name="campaign_group_level2" Type="Int32" />
            <asp:Parameter Name="campaign_group_level3" Type="Int32" />
            <asp:Parameter Name="campaign_group_level4" Type="Int32" />
            <asp:Parameter Name="campaign_group_inactive" Type="Boolean" />
            <asp:Parameter Name="campaign_group_add_date" Type="DateTime" ConvertEmptyStringToNull="false" />
            <asp:Parameter Name="campaign_group_acd_flag" Type="Boolean" />
        </InsertParameters>
    </asp:SqlDataSource>
    </asp:Content>
