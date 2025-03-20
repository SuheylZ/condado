<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportDisplay.ascx.cs" Inherits="SalesTool.Web.UserControls.ReportDisplay" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=7.1.13.802, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<script type="text/javascript">
    function GetRadWindow() {
        var oWindow = null;
        if (window.radWindow)
            oWindow = window.radWindow;
        else if (window.frameElement && window.frameElement.radWindow)
            oWindow = window.frameElement.radWindow;
        return oWindow;
    }

    function CloseWindow(sender, args) {
        alert('hello');
        var oWindow = GetRadWindow();
        if (oWindow == null)
            alert('no window found');
        else
            oWindow.close();       //closes the window       
    }
</script>

<div class="report">
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" Height="100%" EnableAJAX="true"  >

        <b>Report Type&nbsp;</b>
        <telerik:RadDropDownList ID="ddlReportType" runat="server" DataTextField="Title" DataValueField="Id" AutoPostBack="true"/>
        <!-- Filters for the report -->
        <hr />
        <b>Parameters</b><br />
        <span style="display:inline-block;">
            <asp:Label ID="lblAgents" runat="server" Text="&nbsp;Agents&nbsp;" />
            <telerik:RadDropDownList ID="ddlAgents" runat="server" DataTextField="FullName" DataValueField="key" DropDownHeight="300px"/>
        </span>
        <span style="display:inline-block;">
            <asp:Label ID="lblAgentType" runat="server" Text="&nbsp;Agent Type&nbsp;" />
            <telerik:RadDropDownList ID="ddlAgentGroups" runat="server" DataTextField="Name" DataValueField="Id" />
        </span>
        <span style="display:inline-block;">
            <asp:Label ID="lblCampaign" runat="server" Text="&nbsp;Campaign&nbsp;" />
            <telerik:RadDropDownList ID="ddlCampaign" runat="server" DataTextField="Title" DataValueField="Id" DropDownHeight="400" />
        </span>
        <span style="display:inline-block;">
            <asp:Label ID="lblDate" runat="server" Text="&nbsp;Date&nbsp;" />
            <telerik:RadDropDownList ID="ddlTime" runat="server" AutoPostBack="true">
                <Items>
                    <telerik:DropDownListItem Value="-2" Text="Any Time" />
                    <telerik:DropDownListItem Value="1" Text="Today" Selected="True" />
                    <telerik:DropDownListItem Value="2" Text="Yesterday" />
                    <telerik:DropDownListItem Value="3" Text="Week to Date" />
                    <telerik:DropDownListItem Value="4" Text="Month to Date" />
                    <telerik:DropDownListItem Value="5" Text="Last 7 Days" />
                    <telerik:DropDownListItem Value="6" Text="Last 14 Days" />
                    <telerik:DropDownListItem Value="7" Text="Last 30 Days" />
                    <telerik:DropDownListItem Value="8" Text="Current Year" />
                    <telerik:DropDownListItem Value="9" Text="Custom Date" />
                </Items>
            </telerik:RadDropDownList>
        </span>
        <span style="display:inline-block;">
            <asp:Label ID="lblYear" runat="server" Text="&nbsp;Year&nbsp;" />
            <telerik:RadDropDownList ID="ddlYear" runat="server" >
                <Items>
                    <telerik:DropDownListItem Text="Current Year" Value="1" />
                    <telerik:DropDownListItem Text="Past Year" Value="2" />
                </Items>
            </telerik:RadDropDownList>
        </span>
        <span style="display:inline-block;">
            <asp:Label ID="lblType" runat="server" Text="&nbsp;Type&nbsp;" />
            <telerik:RadDropDownList ID="ddlGoalType" runat="server" >
                <Items>
                    <telerik:DropDownListItem Value="1" Text="Monthly" Selected="True" />
                    <telerik:DropDownListItem Value="2" Text="Yearly" />
                </Items>
            </telerik:RadDropDownList>
        </span>
       <span style="display:inline-block;">
           <asp:Label ID="lblMonthYr" runat="server" Text="&nbsp;Type&nbsp;" />
           <telerik:RadMonthYearPicker ID="tlkMonthYr" runat="server" />
           </span>
        <span style="display:inline-block;">
            <telerik:RadButton ID="btnApply" runat="server" AutoPostBack="true" Text="Apply" />
            <telerik:RadButton ID="btnClear" runat="server" AutoPostBack="true" Text="Clear" />
        </span>
 
        <telerik:RadWindow ID="tlkDateWindow" runat="server" Width="344px" Height="200px" VisibleTitlebar="true" Title="Choose Date Filter" 
                           Behaviors="Resize,  Move" VisibleOnPageLoad="false" AutoSizeBehaviors="Width, Height" 
                           Skin="WebBlue" Modal="true" >
            <ContentTemplate>
                <div style="padding: 8px; align-content: center; text-align: justify; ">
                    <div style="display: block; width: auto; padding: 4px;">
                        Please specify a start date and end date to be used filter the report
                    </div>
                    <div style="display: table-row; padding: 8px;">
                        <div style="display: table-cell; width: 50%;">Choose Start Date</div>
                        <div style="display: table-cell; width: 50%;">
                            <telerik:RadDatePicker ID="tlkStartDate" Runat="server" Culture="en-US">
                                <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" runat="server"/>
                                <DateInput DateFormat="MMM d, yyyy" DisplayDateFormat="MMMM d, yyyy" LabelWidth="40%" runat="server"/>
                                <DatePopupButton runat="server" />
                            </telerik:RadDatePicker>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell; width: 50%;">Choose End Date</div>
                        <div style="display: table-cell; width: 50%;">
                            <telerik:RadDatePicker ID="tlkEndDate" Runat="server" Culture="en-US">
                                <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" runat="server" />
                                <DateInput DateFormat="MMM d, yyyy" DisplayDateFormat="MMMM d, yyyy" LabelWidth="40%" runat="server"/>
                                <DatePopupButton runat="server" />
                            </telerik:RadDatePicker>
                        </div>
                    </div>
                    <div style="float:right;padding: 10px;">
                        <telerik:RadButton ID="tlkClose" runat="server" Text="Close" />
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
        <br />
        <hr />
        <br />
        <telerik:ReportViewer ID="tlkViewer" runat="server" Width="100%" Height="550px" ToolbarVisible="true"  DocumentMapVisible="False" ShowHistoryButtons="False" ParametersAreaVisible="False" ShowDocumentMapButton="False"  ShowParametersButton="False"/>

    </telerik:RadAjaxPanel>
</div>

