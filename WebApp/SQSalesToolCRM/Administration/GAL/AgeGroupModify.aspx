<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" CodeFile="AgeGroupModify.aspx.cs" Inherits="SQS_Dialer.AgeGroupModify" %>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagPrefix="uc1" TagName="PagingBar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>
    <asp:Label ID="lblTitle" runat="server" Text="Age Group Management"/></h2>


 

    <uc1:PagingBar runat="server" ID="pager" NewButtonTitle="Add Age Group" OnNewRecord="btnAdd_Click" />
    <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="age_group_id" 
                CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None"
                DataSourceID="AgeGroupManagerDataSource" AllowSorting="True" 
                HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle" AllowPaging="true" ShowHeaderWhenEmpty="true">
                <PagerSettings Visible="false" />
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="age_group_start" 
                        HeaderText="Age Range Start" SortExpression="age_group_start" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="age_group_end" 
                        HeaderText="Age Range End" SortExpression="age_group_end" 
                        Visible="True" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:CheckBoxField DataField="age_group_inactive" HeaderText="Inactive?" 
                        SortExpression="age_group_inactive" HeaderStyle-HorizontalAlign="Left" 
                        HeaderStyle-VerticalAlign="Middle">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="150px"></HeaderStyle>
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="age_group_add_date" HeaderText="Add Date" 
                        ReadOnly="True" SortExpression="age_group_add_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="age_group_modify_date" HeaderText="Modified Date" 
                        ReadOnly="True" SortExpression="age_group_modify_date" 
                        HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
<HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:CommandField ShowEditButton="True" 
                        ButtonType="Image" CancelImageUrl="~/Images/gal/cancel.png" 
                        DeleteImageUrl="~/Images/gal/remove.png" EditImageUrl="~/Images/gal/edit.png" 
                        UpdateImageUrl="~/Images/gal/check.png">
                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px"></HeaderStyle></asp:CommandField>
                    <asp:TemplateField ShowHeader="False" Visible="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                CommandName="Delete" ImageUrl="~/Images/gal/remove.png" Text="Delete" ToolTip="Delete" OnClientClick='return confirm("Are you sure you want to delete this entry?");' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle 
                    HorizontalAlign="Left" VerticalAlign="Middle" />
            </asp:GridView>
            <asp:SqlDataSource ID="AgeGroupManagerDataSource" runat="server" 
                ConnectionString="<%$ ConnectionStrings:ApplicationServices %>" 
                SelectCommand="SELECT age_group_id, age_group_start, age_group_end, age_group_inactive, age_group_add_date, age_group_modify_date, age_group_delete_flag FROM [gal_AgeGroups] WHERE [age_group_delete_flag] = 0 and IsNULL([age_group_acd_flag],0) = @bGALType ORDER BY age_group_start, age_group_end"
                DeleteCommand="UPDATE [gal_AgeGroups] SET [age_group_delete_flag] = 1 WHERE [age_group_id] = @original_age_group_id" 
                OldValuesParameterFormatString="original_{0}" 
                UpdateCommand="UPDATE [gal_AgeGroups] SET age_group_start=@age_group_start, age_group_end=@age_group_end, age_group_inactive=@age_group_inactive,age_group_modify_date=@age_group_modify_date WHERE [age_group_id] = @original_age_group_id">
                <DeleteParameters>
                    <asp:Parameter Name="original_age_group_id" Type="Object" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="age_group_start" Type="String" />
                    <asp:Parameter Name="age_group_end" Type="Int32" />
                    <asp:Parameter Name="age_group_inactive" Type="Boolean" />
                    <asp:Parameter Name="age_group_add_date" Type="DateTime" />
                    <asp:Parameter DefaultValue="6/1/2011 12:00 PM" Name="age_group_modify_date" 
                        Type="DateTime" />
                    <asp:Parameter Name="original_age_group_id" Type="Object" />
                </UpdateParameters>
                 <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                ShowMessageBox="True" ShowSummary="False" />
     
</asp:Content>
