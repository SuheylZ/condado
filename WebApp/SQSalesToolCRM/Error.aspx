<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   <h2>There was an error in processing your request. Please contact administrator.</h2>

   <br />
   <br />
   <br />

    <asp:Label runat="server" ID="lblError"/>
   
</asp:Content>
