<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master"
    CodeFile="PVSchedular.aspx.cs" Inherits="SQS_Dialer.PVSchedular" %>

<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<%@ Register src="~/Administration/GAL/controls/PVScheduleAgentGroup.ascx" tagname="PVScheduleAgentGroup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <uc1:PVScheduleAgentGroup ID="PVScheduleAgentGroup1" runat="server" />
   
</asp:Content>
