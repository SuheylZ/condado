<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ColumnSelectionLists.ascx.cs"
    Inherits="UserControls.UserControlsColumnSelectionLists" %>
<fieldset class="condado">
    <legend id="lblTitle" runat="server" />
    <div id="sldOuter">
        <div id="sldLeft">
            <h2>
                <asp:Label ID="lblLeftTitle" runat="server" /></h2>
            <asp:ListBox ID="lbAvailable" runat="server" Width="90%" Height="250px" 
                SelectionMode="Multiple" onprerender="lbAvailable_PreRender" />
        </div>
        <div id="sldCenter" style= "width: 15%;">
            <br />
            <ul>
                <li>
                    <asp:Button ID="btnSelectOne" runat="server" Text="&gt;" ToolTip="Select One" OnClick="Evt_SelectOne_Clicked"
                        Width="70px" />
          
                </li>
                <li>
                    <asp:Button ID="btnSelectAll" runat="server" Text="&gt;&gt;&gt;" ToolTip="Select All"
                        OnClick="Evt_SelectAll_Clicked" Width="70px" /></li>
                <li>
                    <asp:Button ID="btnDeselectAll" runat="server" Text="&lt;&lt;&lt;" ToolTip="Deselect All"
                        OnClick="Evt_DeselectAll_Clicked" Width="70px" /></li>
                <li>
                    <asp:Button ID="btnCount" runat="server" Text="COUNT &gt;&gt;" OnClick="btnCount_Click"
                        Width="70px" /></li>
                <li>
                    <asp:Button ID="btnSum" runat="server" Text="SUM &gt;&gt;" OnClick="btnSum_Click"
                        Width="70px" /></li>
                <li>
                    <asp:Button ID="btnMin" runat="server" Text="MIN &gt;&gt;" OnClick="btnMin_Click"
                        Width="70px" /></li>
                <li>
                    <asp:Button ID="btnMax" runat="server" Text="MAX &gt;&gt;" OnClick="btnMax_Click"
                        Style="height: 26px" Width="70px" /></li>
                <li>
                    <asp:Button ID="btnAverage" runat="server" Text="AVG &gt;&gt;" OnClick="btnAverage_Click"
                        Width="70px" /></li>
                <li>
                    <asp:Button ID="btnDeselectOne" runat="server" Text=" &lt; " OnClick="Evt_DeselectOne_Clicked"
                        Width="70px" /></li>
            </ul>
        </div>
        <div id="sldRight" >   
               
                <h2>
                    <asp:Label ID="lblRightTitle" runat="server" /></h2>
                <asp:ListBox ID="lbChosen" runat="server" Width="90%" Height="250px" SelectionMode="Multiple" />
            
        </div>
         <div style="float:left;" >
                <asp:Button ID="btnUp" runat="server" Text="Up" OnClick="btnUp_Click" Width="70px" />
                <br />
                <br />
                <asp:Button ID="btnDown" runat="server" Text="Down" OnClick="btnDown_Click" Width="70px" />
            </div>   
        <span style="clear: both;" />
    </div>
    
    
    
</fieldset>
