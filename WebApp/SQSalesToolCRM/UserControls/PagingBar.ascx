<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PagingBar.ascx.cs" Inherits="UserControls_PagingBar" %>
<div style="height: auto; display: block;">
    <span id="spnButton" runat="server"><asp:Button ID="btnAdd" runat="server" Text="Add New" OnClick="Evt_Add_Clicked" /><br /></span>

    
    <span style="float: left; display: block;" runat="server" id="pageSizeSpan">
    
       <asp:TextBox ID="txtPageSize" runat="server" AutoPostBack="true" MaxLength="3" OnTextChanged="Evt_PageSizeChanged" Width="32px"/>
        <ajaxToolkit:FilteredTextBoxExtender ID="txtPageSize_Extender" runat="server" TargetControlID="txtPageSize"
            FilterType="Numbers" />
        / Page </span>
        
       
        
        <span style="float: right; display: inline-block;">
         <asp:Label ID="lblCount" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnPreviousPage" runat="server" OnClick="Evt_PreviousClicked" Text="Prev" />
            <asp:TextBox ID="txtPageNumber" runat="server" AutoPostBack="true" MaxLength="10"
                OnTextChanged="Evt_PageNumberChanged" Width="32px">1</asp:TextBox>
            <ajaxToolkit:FilteredTextBoxExtender ID="txtPageNumber_FilteredTextBoxExtender" runat="server"
                Enabled="True" FilterType="Numbers" TargetControlID="txtPageNumber">
            </ajaxToolkit:FilteredTextBoxExtender>
            of
            <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
            <asp:Button ID="btnNextPage" runat="server" OnClick="Evt_NextClicked" Text="Next" />
        </span>



    <asp:HiddenField ID="hdRecordCount" runat="server" />
    <asp:HiddenField ID="hdSortColumnX" runat="server" /> 
    <asp:HiddenField ID="hdSortDirectionX" runat="server" />
</div>
