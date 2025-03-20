<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="CampaignManager.aspx.cs" Inherits="SQS_Dialer.CampaignManager" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<%@ Register src="../../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2><asp:Label ID="lblCampaignManagerTitle" runat="server" Text="Campaign Management"/></h2>
    <uc1:PagingBar ID="pager" runat="server" NewButtonTitle="" 
         />
<br />

            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="campaign_id,CampaignID" 
                DataSourceID="CampaignManagerDataSource" AllowSorting="True" 
                HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle"
                CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None" AllowPaging="true" ShowHeaderWhenEmpty="True"
                >
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                  <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="campaign_id" HeaderText="campaign_id" ReadOnly="True" 
                        SortExpression="campaign_id" Visible="False" />
                    <asp:BoundField DataField="CampaignID" HeaderText="Campaign ID" 
                        SortExpression="CampaignID" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" 
                        ReadOnly="True">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="CampaignTitle" 
                        HeaderText="Title" SortExpression="CampaignTitle" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle" ReadOnly="True">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"  Width="300px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Campaign Group" SortExpression="campaign_group_name">
                        <EditItemTemplate>
                            <asp:DropDownList ID="lstAgentGroups" runat="server" Width="250px" 
                                DataSourceID="CampaignGroupList" DataTextField="campaign_group_name" 
                                DataValueField="campaign_group_id" SelectedValue='<%# Bind("campaign_group_id") %>' Font-Size="Small">
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblCampaignGroup" runat="server" Text='<%# Bind("campaign_group_name") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="campaign_default_max" 
                        HeaderText="campaign_default_max" SortExpression="campaign_default_max" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" 
                        Visible="False">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_priority" HeaderText="Priority" SortExpression="campaign_priority" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" 
                        ReadOnly="True" Visible="False">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="campaign_level1" 
                        HeaderText="Level 1" 
                        SortExpression="campaign_level1" ReadOnly="True" Visible="False" />
                    <asp:BoundField DataField="campaign_level2" HeaderText="Level 2" 
                        SortExpression="campaign_level2" ReadOnly="True" Visible="False" />
                    <asp:BoundField DataField="campaign_level3" HeaderText="Level 3" 
                        SortExpression="campaign_level3" ReadOnly="True" Visible="False" />
                    <asp:BoundField DataField="campaign_level4" HeaderText="Level 4" 
                        SortExpression="campaign_level4" ReadOnly="True" Visible="False" />
                    <asp:CheckBoxField DataField="campaign_inactive" HeaderText="Inactive" 
                        SortExpression="campaign_inactive" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="campaign_add_date" HeaderText="Add Date" 
                        ReadOnly="True" SortExpression="campaign_add_date">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                        </asp:BoundField>
                    <asp:BoundField DataField="campaign_modify_date" HeaderText="Modify Date" 
                        ReadOnly="True" SortExpression="campaign_modify_date"><HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                        </asp:BoundField>
                    <asp:CommandField ShowEditButton="True" ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
                        DeleteImageUrl="~/Images/gal/remove.png" EditImageUrl="~/Images/gal/edit.png" 
                        UpdateImageUrl="~/Images/gal/check.png" HeaderStyle-Width="50px" EditText="Edit"  />
                </Columns>
                <HeaderStyle 
                    HorizontalAlign="Left" VerticalAlign="Middle" />
                <PagerSettings Visible="False" />
            </asp:GridView>
            <asp:SqlDataSource ID="CampaignManagerDataSource" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" OldValuesParameterFormatString="original_{0}" 
                SelectCommand="SELECT campaign_id, 
                                CampaignID = cmp_id, 
                                CampaignTitle = cmp_title, 
                                campaign_default_max, 
                                campaign_priority, 
                                campaign_level1, 
                                campaign_level2, 
                                campaign_level3, 
                                campaign_level4, 
                                campaign_inactive, 
                                campaign_add_date, 
                                campaign_modify_date, 
                                campaign_group_name, 
                                campaign_group_id 
                                from [gal_Campaigns] 
                                LEFT JOIN [gal_CampaignGroups] ON campaign_campaign_group_id = campaign_group_id 
                                join Campaigns on campaign_id = cmp_id
                                WHERE cmp_delete_flag = 0 order by cmp_title" 
                DeleteCommand="UPDATE [gal_Campaigns] SET [campaign_delete_flag] = 1 WHERE [campaign_id] = @original_campaign_id" 
                UpdateCommand="UPDATE gal_Campaigns SET campaign_inactive = @campaign_inactive , campaign_modify_date = getdate(), campaign_campaign_group_id = @campaign_group_id WHERE campaign_id = @original_campaign_id">
                <DeleteParameters>
                    <asp:Parameter Name="original_campaign_id" Type="Object" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="campaign_priority" Type="Int32"  />
                    <asp:Parameter Name="campaign_level1" Type="Int32" />
                    <asp:Parameter Name="campaign_level2" Type="Int32" />
                    <asp:Parameter Name="campaign_level3" Type="Int32" />
                    <asp:Parameter Name="campaign_level4" Type="Int32" />
                    <asp:Parameter Name="campaign_inactive" Type="Boolean" ConvertEmptyStringToNull="true" />
                    <asp:Parameter Name="campaign_group_id" DbType="Guid" ConvertEmptyStringToNull="true" />
                    <asp:Parameter Name="original_campaign_id" Type="Object" />
                </UpdateParameters>
                 <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="CampaignGroupList" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT campaign_group_id, campaign_group_name, o=2 FROM [gal_CampaignGroups] where IsNULL([campaign_group_acd_flag],0) = @bGALType  UNION SELECT null, null, o=1 ORDER BY o, campaign_group_name">
                 <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>

            </asp:SqlDataSource>
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ShowMessageBox="True" ShowSummary="False" />


</asp:Content>
