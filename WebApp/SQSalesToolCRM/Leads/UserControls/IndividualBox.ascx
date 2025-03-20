<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IndividualBox.ascx.cs" Inherits="SalesTool.Web.UserControls.IndividualBox" %>
<%@ Register Src="IndividualsAddEdit.ascx" TagName="IndividualsAddEdit" TagPrefix="uc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadWindow ID="dlgIndividual" runat="server"  Behaviors="Move" Modal="True" Width="900" Height="650" Style="z-index: 9999999"
    VisibleStatusbar="false" Skin="WebBlue" Title="Add Individual" DestroyOnClose="true" RenderMode="Classic">
    <ContentTemplate>
        <uc1:IndividualsAddEdit ID="ctlIndividual" runat="server" />
    </ContentTemplate>
</telerik:RadWindow>



