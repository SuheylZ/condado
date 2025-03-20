<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="CampaignGroupModify.aspx.cs" Inherits="SQS_Dialer.CampaignGroupModify" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagPrefix="uc1" TagName="PagingBar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2><asp:Label ID="lblTitle" runat="server" Text="Campaign Group Management"/></h2>
<br />



    <uc1:PagingBar runat="server" ID="pager" NewButtonTitle="Add Campaign Group" OnNewRecord="btnAddCampaignGroup_Click" />
    
            <asp:GridView ID="GridView1" runat="server" 
                CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                AutoGenerateColumns="False" DataKeyNames="campaign_group_id" 
                DataSourceID="CampaignGroupDataSource" Width="100%" AllowSorting="True" 
                HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" AllowPaging="true" 
                OnRowCommand="GridView1_RowCommand" OnRowUpdating="GridView1_RowUpdating" OnRowUpdated="GridView1_RowUpdated" ShowHeaderWhenEmpty="true"
                >
                <PagerSettings Visible="false" />
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="campaign_group_id" HeaderText="campaign_group_id" ReadOnly="True" 
                        SortExpression="agent_id" Visible="False" />
                    <asp:BoundField DataField="campaign_group_name" 
                        HeaderText="Campaign Group" SortExpression="campaign_group_name" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_default_max" 
                        HeaderText="Max Quota" SortExpression="campaign_group_default_max" 
                        HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle" Visible="false">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_priority" 
                        HeaderText="Priority" SortExpression="campaign_group_priority" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_level1" 
                        HeaderText="Level 1" SortExpression="campaign_group_level1" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_level2" 
                        HeaderText="Level 2" SortExpression="campaign_group_level2" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_level3" 
                        HeaderText="Level 3" SortExpression="campaign_group_level3" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_level4" 
                        HeaderText="Level 4" SortExpression="campaign_group_level4" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>

                    <asp:CheckBoxField DataField="campaign_group_inactive" HeaderText="Inactive?" 
                        SortExpression="campaign_group_inactive" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="campaign_group_add_date" HeaderText="Add Date" 
                        ReadOnly="True" SortExpression="campaign_group_add_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_group_modify_date" HeaderText="Modified Date" 
                        ReadOnly="True" SortExpression="campaign_group_modify_date" 
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
                    <asp:ButtonField ButtonType="Image" CommandName="CampaignEdit" 
                        Text="Campaigns" ImageUrl="~/Images/gal/skill.png"   />
                </Columns>
                <HeaderStyle 
                    HorizontalAlign="Left" VerticalAlign="Middle" />
            </asp:GridView>
            <asp:SqlDataSource ID="CampaignGroupDataSource" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT campaign_group_id, campaign_group_name, campaign_group_default_max, campaign_group_priority,campaign_group_level1,campaign_group_level2,campaign_group_level3,campaign_group_level4,campaign_group_inactive,campaign_group_add_date,campaign_group_modify_date,campaign_group_delete_flag FROM [gal_CampaignGroups] WHERE [campaign_group_delete_flag] = 0 and IsNULL([campaign_group_acd_flag],0) = @bGALType   ORDER BY campaign_group_name"
                DeleteCommand="UPDATE [gal_CampaignGroups] SET [campaign_group_delete_flag] = 1 WHERE [campaign_group_id] = @original_campaign_group_id" 
                OldValuesParameterFormatString="original_{0}" 
                UpdateCommand="UPDATE [gal_CampaignGroups] SET campaign_group_name=@campaign_group_name, campaign_group_default_max=@campaign_group_default_max, campaign_group_priority=@campaign_group_priority,campaign_group_level1=@campaign_group_level1,campaign_group_level2=@campaign_group_level2,campaign_group_level3=@campaign_group_level3,campaign_group_level4=@campaign_group_level4,campaign_group_inactive=@campaign_group_inactive,campaign_group_modify_date=@campaign_group_modify_date WHERE [campaign_group_id] = @original_campaign_group_id">
                <DeleteParameters>
                    <asp:Parameter Name="original_campaign_group_id" Type="Object" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="campaign_group_default_max" Type="Int32" />
                    <asp:Parameter Name="campaign_group_priority" Type="Int32" />
                    <asp:Parameter Name="campaign_group_level1" Type="Int32" />
                    <asp:Parameter Name="campaign_group_level2" Type="Int32" />
                    <asp:Parameter Name="campaign_group_level3" Type="Int32" />
                    <asp:Parameter Name="campaign_group_level4" Type="Int32" />
                    <asp:Parameter Name="campaign_group_inactive" Type="Boolean" />
                    <asp:Parameter Name="campaign_group_add_date" Type="DateTime" />
                    <asp:Parameter DefaultValue="6/1/2011 12:00 PM" Name="campaign_group_modify_date" 
                        Type="DateTime" />
                    <asp:Parameter Name="original_campaign_group_id" Type="Object" />
                </UpdateParameters>
                 <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ShowMessageBox="True" ShowSummary="False" />
</asp:Content>
