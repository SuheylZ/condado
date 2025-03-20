<%@ Control Language="C#" AutoEventWireup="true" CodeFile="screenerInformation.ascx.cs" Inherits="Leads_UserControls_screenerInformation" %>

<div class="condado">
<div>
    <table style="width: 100%; height: 100%">
        <tr>
            <td>
                <span>
                    Screener:
                </span>
            </td>
            <td>
                <span>
                    <asp:TextBox
                runat="server" ID="txtScreener" />

                </span>
            </td>
            <td>
                <span>
                    Appointment:
                </span>
            </td>
            <td>
                <span>
                    <asp:TextBox
                runat="server" ID="txtAppointment" />

                </span>
            </td>
        </tr>
        <tr>
            <td>
                <span>
                    Screener Notes:
                </span>
            </td>
            <td colspan="3">
                <span>
                    <asp:TextBox
                runat="server" ID="txtScreenerNotes" Rows="4" TextMode="MultiLine" Width="98%" />

                </span>
            </td>
        </tr>
    </table>
</div>
<div style="text-align: center">
    <p>
        
    </p>
    
</div>

</div>