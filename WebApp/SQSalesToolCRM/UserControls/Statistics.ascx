<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Statistics.ascx.cs" Inherits="SalesTool.Web.UserControls.StatisticsControl" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

 <div class="scFilter">
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




<telerik:RadDropDownList ID="ddlAgents" runat="server" DataTextField="FullName" DataValueField="Key" DropDownHeight="400" AutoPostBack="true"/>
&nbsp;&nbsp;
<telerik:RadDropDownList ID="ddlCampaign" runat="server" DataTextField="Title" DataValueField="Id" DropDownHeight="400" AutoPostBack="true" />
&nbsp;&nbsp;
<telerik:RadDropDownList ID="ddlAgentSkillGroups" runat="server" DataTextField="Name" DataValueField="Id" AutoPostBack="true" />
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
            <telerik:RadButton ID="tlkGetResults" runat="server" Text="Get Results" /> 
                <telerik:RadButton ID="tlkClose" runat="server" Text="Close" />
            </div>
            
        </div>
    </ContentTemplate>
</telerik:RadWindow>
</div>
           <telerik:RadtabStrip ID="tlkTabs" runat="server" Width="100%" Skin="WebBlue" MultiPageID="tlkPages">
            <Tabs>
                <telerik:Radtab Text="Sales Metrics" PageViewID="tlkSales" Value="1" Selected="true"/>
                <telerik:RadTab Text="Lead Metrics" PageViewID="tlkLead" Value="2" />
                <telerik:RadTab Text="Call Center Metrics" PageViewID="tlkCallCenter" Value="3" Visible="false"/>
            </Tabs>
        </telerik:RadtabStrip>

        <telerik:RadMultiPage ID="tlkPages" runat="server" SelectedIndex="0">

            <telerik:RadPageView ID="tlkSales" runat="server">
                <div class="scIndicators">
                 <fieldset>
                <ul>
                    <li><label for="lbl20">Talk Time</label><asp:Label ID="lblTalkTime" runat="server" Text="0h 0m" /></li>
                    <li><label for="Label21">Total Calls</label><asp:Label ID="lblTotalCalls" runat="server" Text="0"/></li>
                    <li><label for="Label22">Valid Leads</label><asp:Label ID="lblValidLeads" runat="server" Text=""/></li>
                    <li><label for="Label23"># of Contacts</label><asp:Label ID="lblNoOfContacts" runat="server" Text="0"/></li>
                    <li><label for="Label24">Closes</label><asp:Label ID="lblCloses" runat="server" Text="0"/></li>
                    <li><label for="Label25"># Important Actions</label><asp:Label ID="lblImprtantActions" runat="server" Text="0"/></li>
                    <li><label for="Label26"># Quoted</label><asp:Label ID="lblNoQuoted" runat="server" Text="0"/></li>
                    
                    </ul>
                    </fieldset>
                </div>
            </telerik:RadPageView>
            
            <telerik:radpageview ID="tlkLead" runat="server">
               <div class="scIndicators">
                 <fieldset>
                <ul>
                    <li><label for="lbl10">New Leads</label><asp:Label ID="lblLeadMetricNewLeads" runat="server" Text="20" /></li>
                    <li><label for="Label11">Valid Leads</label><asp:Label ID="lblLeadMetricValidLeads" runat="server" Text="115"/></li>
                    <li><label for="Label12">% Valid</label><asp:Label ID="lblLeadMetricPercentValid" runat="server" Text="79%"/></li>
                    <li><label for="Label13">Contacted</label><asp:Label ID="lblLeadMetricContacted" runat="server" Text="60"/></li>
                    <li><label for="Label14">% Contacted</label><asp:Label ID="lblLeadMetricPercentContacted" runat="server" Text="64%"/></li>
                    <li><label for="Label15"># Quoted</label><asp:Label ID="lblLeadMetricNoQuoted" runat="server" Text="5"/></li>
                    <li><label for="Label16">% Quoted</label><asp:Label ID="lblLeadMetricPercentQuoted" runat="server" Text="10%"/></li>
                    <li><label for="Label15"># Closed</label><asp:Label ID="lblLeadMetricClosed" runat="server" Text=""/></li>
                    <li><label for="Label17">% Closed</label><asp:Label ID="lblLeadMetricPercentClosed" runat="server" Text="8%"/></li>
                    <%--<li><label for="Label18">% Pending Status</label><asp:Label ID="lblLeadMetricPercentPendingStatus" runat="server" Text="25%"/></li>--%>
                    <%--<li><label for="Label19">Lead Score</label><asp:Label ID="lblLeadMetricLeadScore" runat="server" Text="68" Visible="false"/></li>--%>
                </ul>
                    </fieldset>
                </div>
            </telerik:RadPageView>

            <telerik:radpageview ID="tlkCallCenter" runat="server"/>

        </telerik:RadMultiPage>



