<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardGrid.ascx.cs" Inherits="Administration_GAL_controls_DashboardGrid" %>



<telerik:RadGrid ID="grdCustom" runat="server" Skin=""


    CssClass="mGrid" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-CssClass="alt" GridLines="None" CellSpacing="0">
    <MasterTableView>
        <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif" ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif" ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
    </MasterTableView>
    <HeaderStyle CssClass="gridHeader" />
</telerik:RadGrid>