<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Filter.ascx.cs" Inherits="SalesTool.Web.UserControls.FilterControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


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




<telerik:RadDropDownList ID="ddlLeadType" runat="server" />
&nbsp;&nbsp;
<telerik:RadDropDownList ID="ddlCampaign" runat="server" DataTextField="Title" DataValueField="Id" DropDownHeight="400" AutoPostBack="true" />
&nbsp;&nbsp;
<telerik:RadDropDownList ID="ddlAgentGroups" runat="server" DataTextField="Name" DataValueField="Id" AutoPostBack="true" />
&nbsp;&nbsp;
<telerik:RadDropDownList ID="ddlTime" runat="server" AutoPostBack="true">
    <Items>
        <telerik:DropDownListItem Value="1" Text="Today" Selected="True" />
        <telerik:DropDownListItem Value="2" Text="Yesterday" />
        <telerik:DropDownListItem Value="3" Text="Week to Date" />
        <telerik:DropDownListItem Value="4" Text="Month to Date" />
        <telerik:DropDownListItem Value="5" Text="Last 7 Days" />
        <telerik:DropDownListItem Value="6" Text="Last 14 Days" />
        <telerik:DropDownListItem Value="7" Text="Last 30 Days" />
        <telerik:DropDownListItem Value="8" Text="Custom Date" />
    </Items>
</telerik:RadDropDownList>
<telerik:RadWindow ID="tlkDateWindow" runat="server" Width="344px" Height="200px" VisibleTitlebar="true" Title="Choose Date Filter" 
    Behaviors="Resize,  Move" VisibleOnPageLoad="false" AutoSizeBehaviors="Width, Height" 
    Skin="WebBlue" Modal="true" >
    <ContentTemplate>
        <div style="padding: 8px; align-content: center; text-align: justify; ">

            <div style="display: block; width: auto; padding: 4px;">
                Please specify a start date and end date to be used to apply to the filter
            </div>

            <div style="display: table-row; padding: 8px;">
                <div style="display: table-cell; width: 50%;">Choose Start Date</div>
                <div style="display: table-cell; width: 50%;">
                    <telerik:RadDatePicker ID="tlkStartDate" Runat="server" Culture="en-US">
                        <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
                        </Calendar>
                        <DateInput DateFormat="MMM d, yyyy" DisplayDateFormat="MMMM d, yyyy" LabelWidth="40%"/>
                           
                        
                        <DatePopupButton HoverImageUrl="" ImageUrl="" />
                    </telerik:RadDatePicker>
                </div>
            </div>
            <div style="display: table-row;">
                <div style="display: table-cell; width: 50%;">Choose End Date</div>
                <div style="display: table-cell; width: 50%;">
                    <telerik:RadDatePicker ID="tlkEndDate" Runat="server" Culture="en-US">
                        <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
                        </Calendar>
                        <DateInput DateFormat="MMM d, yyyy" DisplayDateFormat="MMMM d, yyyy" LabelWidth="40%"/>
                    
                        <DatePopupButton HoverImageUrl="" ImageUrl="" />
                    </telerik:RadDatePicker>
                </div>
            </div>
            <div style="float:right;padding: 10px;">
                <telerik:RadButton ID="tlkClose" runat="server" Text="Close" />
            </div>
            
        </div>
    </ContentTemplate>
</telerik:RadWindow>



