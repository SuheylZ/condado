<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardSettings.ascx.cs" Inherits="SalesTool.Web.UserControls.DashboardSettings" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

    <fieldset class="condado">
        <ul>
            <li>
                <label for='<%=tlkDashboardType.ClientID%>'>Dashboard Type </label>
                <telerik:RadDropDownList ID="tlkDashboardType" runat="server" DataTextField="Name" DataValueField="Id"/>
            </li>

        </ul>


    </fieldset>
    
    
