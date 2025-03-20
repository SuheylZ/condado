<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectionLists.ascx.cs" Inherits="UserControls.UserControlsSelectionLists" %>

            <fieldset class="condado">
            <legend id="lblTitle" runat="server"/>
   <asp:HiddenField runat="server" ID="hdnfieldIsGroupedData" Value="0" />
    <div id="sldOuter">
        <div id="sldLeft">
<h2><asp:Label ID="lblLeftTitle" runat="server" /></h2>
<asp:ListBox ID="lbAvailable" runat="server" Width="100%" Height="250px" SelectionMode="Multiple" onprerender="lbAvailable_PreRender" />
</div>

        <div id="sldCenter">
        <br />
<ul>
<li><asp:Button ID="btnSelectOne" runat="server" Text="&gt;" ToolTip="Select One" 
onclick="Evt_SelectOne_Clicked"/></li>
<li><asp:Button ID="btnSelectAll" runat="server" Text="&gt;&gt;&gt;" 
ToolTip="Select All" onclick="Evt_SelectAll_Clicked"/></li>
<li><asp:Button ID="btnDeselectAll" runat="server" Text="&lt;&lt;&lt;" 
ToolTip="Deselect All" onclick="Evt_DeselectAll_Clicked"/></li>
<li><asp:Button ID="btnDeselectOne" runat="server" Text="&lt;" 
ToolTip="Deselect One" onclick="Evt_DeselectOne_Clicked"   /></li>
</ul>
</div>

        <div id="sldRight">
<h2><asp:Label ID="lblRightTitle" runat="server" /></h2>
<asp:ListBox ID="lbChosen" runat="server"  Width="100%"  Height="250px" SelectionMode="Multiple" OnPreRender="lbSelected_PreRender" />
</div>

        <span style="clear:both;" />
    </div>
               
</fieldset>