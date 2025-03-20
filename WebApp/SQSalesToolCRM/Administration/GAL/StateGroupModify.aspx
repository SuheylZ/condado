<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="StateGroupModify.aspx.cs" Inherits="SQS_Dialer.StateGroupModify" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagPrefix="uc1" TagName="PagingBar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2><asp:Label ID="lblTitle" runat="server" Text="State Group Management"/></h2>
<br />


    <uc1:PagingBar runat="server" ID="pager" NewButtonTitle="Add State Group"  OnNewRecord="btnAddStateGroup_Click" />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="state_group_id" 
                
                CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"

                DataSourceID="StateGroupManagerDataSource" AllowSorting="True" 
                HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" AllowPaging="true" OnRowCommand="GridView1_RowCommand" ShowHeaderWhenEmpty="true">
                <PagerSettings Visible="false" />
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="state_group_name" 
                        HeaderText="State Group" SortExpression="state_group_name" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" ></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="state_group_priority" 
                        HeaderText="Priority" SortExpression="state_group_priority" 
                        Visible="False" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="150px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:CheckBoxField DataField="state_group_inactive" HeaderText="Inactive?" 
                        SortExpression="state_group_inactive" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="150px"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="state_group_add_date" HeaderText="Add Date" 
                        ReadOnly="True" SortExpression="state_group_add_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" ></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="state_group_modify_date" HeaderText="Modified Date" 
                        ReadOnly="True" SortExpression="state_group_modify_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" ></HeaderStyle>
                    </asp:BoundField>
                    <asp:CommandField ShowEditButton="True" 
                        ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
                        DeleteImageUrl="~/Images/remove.png" EditImageUrl="~/Images/gal/edit.png" 
                        UpdateImageUrl="~/Images/gal/check.png">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px"></HeaderStyle></asp:CommandField>
                    <asp:TemplateField ShowHeader="False" Visible="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                CommandName="Delete" ImageUrl="~/Images/gal/remove.png" Text="Delete" ToolTip="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:ButtonField ButtonType="Image" CommandName="StatesEdit" 
                        Text="States" ImageUrl="~/Images/gal/states.png" />
                </Columns>
                <HeaderStyle 
                    HorizontalAlign="Left" VerticalAlign="Middle" />
            </asp:GridView>
            <asp:SqlDataSource ID="StateGroupManagerDataSource" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT state_group_id, state_group_name, state_group_priority, state_group_inactive, state_group_add_date, state_group_modify_date, state_group_delete_flag FROM [gal_StateGroups] WHERE [state_group_delete_flag] = 0 and IsNULL([state_group_acd_flag],0) = @bGALType ORDER BY state_group_name"
                DeleteCommand="UPDATE [gal_StateGroups] SET [state_group_delete_flag] = 1 WHERE [state_group_id] = @original_state_group_id" 
                OldValuesParameterFormatString="original_{0}" 
                
                UpdateCommand="UPDATE [gal_StateGroups] SET state_group_name=@state_group_name, state_group_inactive=@state_group_inactive,state_group_modify_date=@state_group_modify_date WHERE [state_group_id] = @original_state_group_id">
                <DeleteParameters>
                    <asp:Parameter Name="original_state_group_id" Type="Object" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="state_group_name" Type="String" />
                    <asp:Parameter Name="state_group_inactive" Type="Boolean" />
                    <asp:Parameter Name="state_group_add_date" Type="DateTime" />
                    <asp:Parameter DefaultValue="6/1/2011 12:00 PM" Name="state_group_modify_date" 
                        Type="DateTime" />
                    <asp:Parameter Name="original_state_group_id" Type="Object" />
                </UpdateParameters>
                 <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ShowMessageBox="True" ShowSummary="False" />
      
</asp:Content>
