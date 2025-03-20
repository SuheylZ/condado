<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailEditor.ascx.cs" Inherits="UserControls_EmailEditor" %>
<script type="text/javascript" src='<%= Page.ResolveClientUrl("~/Scripts/jQuery.dualListBox-1.3.js") %>'></script>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initControl)

        $(document).ready(function (){
            initializeDuallistBoxes();
        });

        function initControl(sender, args){
            initializeDuallistBoxes();
        }
     
        function initializeDuallistBoxes() {
            $.configureBoxes({
                box1View: '<%=lbAvailble.ClientID %>',
                box2View: '<%=lbChosen.ClientID %>',
                box1Storage: '<%=lbAvailableStore.ClientID%>',
                box2Storage: '<%=lbChosenStore.ClientID%>',
                to1: 'btnDeselectOne',
                to2: 'btnSelectOne',
                allTo1: 'btnDeselectAll',
                allTo2: 'btnSelectAll',
                useSorting: true,
                selectOnSubmit: true
            });
        }

    </script>
    <asp:ListBox SelectionMode="Multiple" id="lbAvailableStore" runat="server" style="display:none;" />                
    <asp:ListBox SelectionMode="Multiple" id="lbChosenStore" runat="server"  style="display:none;" />
                
  
<fieldset class="condado" style="min-width:900px;">
    <legend> Send Report by Email </legend>
    <ul>
        <li>
            <asp:Label ID="lbl1" runat="server" AssociatedControlID="rbExcel" Text="Report Format" />
            
            <asp:RadioButton ID="rbExcel" runat="server" GroupName="format" />Excel &nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbText" runat="server" GroupName="format"  />Text
            
        </li>
        <li>
            <label for="dvEmail">Recipients</label> 
            <div id="dvEmail" style="vertical-align:central;">
                    <asp:ListBox ID="lbAvailble" runat="server" SelectionMode="Multiple" Width="250" Height="110"/>
                    
                    <ul style="list-style:none; display:inline-block;">
                        <li> <button ID="btnSelectOne" type="button" style="width:25px"> &gt; </button></li>
                        <li> <button ID="btnSelectAll" type="button" style="width:25px"> &gt;&gt; </button></li>
                        <li> <button ID="btnDeselectOne" type="button" style="width:25px"> &lt; </button></li>
                        <li> <button ID="btnDeselectAll" type="button" style="width:25px">&lt;&lt;</button> </li>
                    </ul>

                    <asp:ListBox ID="lbChosen" runat="server" SelectionMode="Multiple" Width="250" Height="110" />
            </div>
        </li>
        <li>
            <asp:Label ID="lbl3" runat="server" AssociatedControlID="txtSubject" Text="Subject" />
            <asp:TextBox ID="txtSubject" runat="server" Width="540px"/>
        </li>
        <li>
            <asp:Label ID="lbl4" runat="server" AssociatedControlID="txtMessage" Text="Message" />
            <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Width="540px" Height="250"/>
       </li>
        <li>
            <asp:Label ID="lbl5" runat="server" AssociatedControlID="chkFilter" Text="Filter Emailed Results by Role" />
            <asp:CheckBox ID="chkFilter" runat="server" />
            <i>(If reports are blank for a particular user, no email will be sent to that user)</i>
        </li>
        <li>
            <asp:Label ID="lbl6" runat="server" AssociatedControlID="rbSendNow" Text="Schedule" />
            <asp:RadioButton ID="rbSendNow" runat="server" GroupName="frequency" />Immediately&nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbSendOnce" runat="server" GroupName="frequency" />Once&nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbSendDaily" runat="server" GroupName="frequency" />Everyday&nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbSendWeekly" runat="server" GroupName="frequency" />Every Week&nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbSendMonthly" runat="server" GroupName="frequency" />Every Month
        </li>
    </ul>
</fieldset>
