<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" EnableViewState="false" CodeFile="viewCalander.aspx.cs" Inherits="Leads_viewCalander" EnableEventValidation="false" %>

<%@ Register Src="~/Leads/UserControls/uc_calandar.ascx" TagPrefix="uc1" TagName="uc_calandar" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server" style="background-color: White; overflow-y: visible; width: 100%; height: 100%">

   
    <uc1:uc_calandar runat="server" ID="uc_calandar1" />

</asp:Content>


