<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="OLD_Dashboard.aspx.cs" Inherits="Dashboard" %>
<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>


<%@ Register src="UserControls/Statistics.ascx" tagname="Statistics" tagprefix="uc1" %>
<%@ Register src="UserControls/Filter.ascx" tagname="Filter" tagprefix="uc2" %>

<%@ Register src="UserControls/ReportDisplay.ascx" tagname="ReportDisplay" tagprefix="uc3" %>

<%@ Register src="UserControls/Announcement.ascx" tagname="Announcement" tagprefix="uc4" %>




<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=7.1.13.802, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="Stylesheet" href="Styles/HomeLayout.css" type="text/css" />
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <div style="display:table-row; height:150px;width:100%;">


    </div>

     <div  class="scAnnouncement">

        <div class="scContests">

            <div class="scMonthlyContest">
                <uc4:Announcement ID="ctlMonthly" runat="server" SectionId="1" />
            </div>

            <div class="scDailyContest">
                <uc4:Announcement ID="ctlDaily" runat="server" SectionId="2" />
            </div>

        </div>

        <div class="scMainAnnouncement">
             <uc4:Announcement ID="ctlMain" runat="server" SectionId="3" />
        </div>

        <div class="scLeadAnnouncement">
             <uc4:Announcement ID="ctlLeads" runat="server" SectionId="4" />
        </div>
    </div>

    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" height="100%">
    

    <div class="scTabs">
        <uc1:Statistics ID="ctlStatistics" runat="server" />
    </div>
    </telerik:RadAjaxPanel>
    


    <div class="scReports">

        <telerik:RadSplitter ID="tlkSplitter" Runat="server" Width="100%" Height="100%" LiveResize="true" 
        Orientation="Vertical" ResizeMode="Proportional" ResizeWithBrowserWindow="false" Skin="WebBlue" BorderSize="0">
        
        <telerik:RadPane ID="tlkLeftPane" runat="server" MinWidth="100" Scrolling="None" RenderMode="Lightweight" RegisterWithScriptManager="true" Height="100%" Width="100%" >
            <uc3:ReportDisplay ID="ctlReportLeft" runat="server" />
        </telerik:RadPane> 

        <telerik:RadSplitBar ID="tlkSplitBar" runat="server" RenderMode="Native" CollapseMode="Both" />

        <telerik:RadPane ID="tlkRightPane" runat="server" MinWidth="100" Scrolling="None">
            <uc3:ReportDisplay ID="ctlReportRight" runat="server" />
        </telerik:RadPane>
    </telerik:RadSplitter>

    </div>

    <div class="scFooter">




    </div>
</asp:Content>

