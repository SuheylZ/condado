<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AlertConsent.ascx.cs" Inherits="SalesTool.Web.UserControls.AlertConsentControl" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
 
<script type="text/javascript">
//<![CDATA[

	/*function tlWindow_Show(sender,args)
	{
	    var dlgConsentBoxId = '< % = tlWindow .ClientID %>';
	    var x = $find(dlgConsentBoxId);
	    if (x != null)
	        $(dlgConsentBoxId).show();
	 
	} */
//]]>
</script>



<%--<script type="text/javascript" src="../Scripts/simulateEvents.js"></script>--%>
<script type="text/javascript">




    function shouldPostback() {
        var x = $('#<%=ddlChoice.ClientID%>');
        if (x.val() == -1) {
            $('span#spnErrorDlg').show();
            return false;
        }
        else
            $('span#spnErrorDlg').hide();
        return true;
    }

</script>


<telerik:RadWindow ID="tlWindow" runat="server" Skin="WebBlue" Height="181px" Width="352px" Title="Consent Alert" Modal="true" AutoSize="false" Behaviors="Move, Reload" DestroyOnClose="True" KeepInScreenBounds="True" RenderMode="Lightweight" AutoSizeBehaviors="Height" Style="position: absolute; z-index:999999;">
    <ContentTemplate>

        <asp:Label ID="lblAlert" runat="server" Text="This is a sample message to be used for Phone Consent Alert. It will be replaced."/>
            <br />
        <div style="vertical-align:middle; text-align:center; width:100%;clear:both;">
            <span id="spnErrorDlg" class="exception" style="font-weight:bold;display:none;">*</span>
        <asp:DropDownList ID="ddlChoice" runat="server" Width="130px"> 
            <asp:ListItem Selected="True" Value="-1">Choose an Item</asp:ListItem>
            <asp:ListItem Value="0">No</asp:ListItem>
            <asp:ListItem Value="1">Yes</asp:ListItem>
            <asp:ListItem Value="2">Not Applicable</asp:ListItem>
        </asp:DropDownList> 
        
            <br /><br />
        <asp:Button ID="btnAlertClose" runat="server" Text="OK" CssClass="button" OnClientClick="return shouldPostback();" />
            </div>
  
    </ContentTemplate>
</telerik:RadWindow>

