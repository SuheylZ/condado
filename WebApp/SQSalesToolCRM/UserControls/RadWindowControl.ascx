<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RadWindowControl.ascx.cs" Inherits="RadWindowControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock runat="server" ID="rcb1">
<script language="javascript" type="text/javascript">

    function GetRadWindow() {
        var oWindow = null;
        if (window.radWindow) oWindow = window.radWindow;
        else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
        return oWindow;
    }
    function CloseDialog(button) {
        GetRadWindow().close();
    }  

</script>
</telerik:RadCodeBlock>
  <telerik:radwindow id="dlgTimerAlert" runat="server" width="450" height="250"
                behaviors="Close,Move" modal="false" skin="WebBlue" destroyonclose="true" visiblestatusbar="false"
                iconurl="~/Images/alert1.ico" visibleonpageload="false" title="Timer Alert">
                <ContentTemplate>               
               
               <asp:UpdatePanel runat="server" id = "updatePanel2">
               <ContentTemplate>
            
                <div class="buttons">
                 <asp:Label ID="lblTimerAlertCampaignName" runat="server" Text=""/>                             
                 
                                               </div>
                                               <br />
                   <ul>
                                                <li>
                                                    <asp:Label ID="lblTimerAlertName" runat="server" Text="Name:" Font-Bold="True"/> &nbsp;<asp:Label ID="lblInnerTimerAlertAlertName" runat="server" Text=""/>                             
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblTimerAlertMessage" runat="server" Text="Message:" Font-Bold="True"/> &nbsp;<asp:Label ID="lblInnerTimerAlertMessage" runat="server" Text=""/>
                                                </li>
                                                                                                       
                                              
                                            </ul>
                    <%--<div class="buttons" align="center" style="height: 40px">                        
                        <asp:Button ID="Button1" runat="server" CausesValidation="false" Text="Close" OnClientClick="CloseDialog"
                           Width="80px" Height="30px" OnClick="btnCloseTimerAlert_Click" /> 
                    </div>--%>
                    </ContentTemplate>
               <Triggers>
               
               </Triggers>
               </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:radwindow>