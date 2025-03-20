<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="DashboardV0" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>SelectCARE Reporting Dashboard</title>
    <!--- SCRIPTS AREA ------>
    <script src="Scripts/jquery-1.8.2.js" type="text/jscript"></script>
    <script src="Scripts/jquery-ui.js" type="text/jscript"></script>
    <script src="Scripts/odometer.js" type="text/jscript"></script>
    <script src="Scripts/jquery.dataTables.js" type="text/jscript"></script>
    <script src="Scripts/highcharts.js" type="text/jscript"></script>
    <script src="Scripts/highcharts-more.js" type="text/jscript"></script>
    <script src="Scripts/exporting.js" type="text/jscript"></script>
    <script src="Scripts/jquery.cookie.js" type="text/jscript"></script>

    <link rel="Stylesheet" href="Styles/jquery-ui-1.8.4.custom.css" type="text/css" />
    <link rel="Stylesheet" href="Styles/DashboardCss-1.2.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="metricsWrapper">
        <div class="metricsDashboardHeader" style="float: left; width: 740px;">
            <div style="float: left; width: 700px;">Sales Metrics</div>
            <div style="float: left; width: 30px; height: 20px; margin: 2px;">
                <img id="fltSalesMetrics" src="Images/btnFilter.png" width="20" alt="Filter" title="Filter" /></div>
            <div style="clear: both;"></div>
        </div>
        <div class="metricsDashboardHeader" style="float: right; width: 740px;">
            <div style="float: left; width: 700px;">Leads Metrics</div>
            <div style="float: left; width: 30px; height: 20px; margin: 2px;">
                <img id="fltLeadMetrics" src="Images/btnFilter.png" width="20" alt="Filter" title="Filter" /></div>
            <div style="clear: both;"></div>
        </div>
        <div style="clear: both;"></div>
        <!-- SALES METRCIS -->
        <div class="metricsSalesBox">
            <div id="salesMetricsParms" class="filterDivBox">
                <div style="float: left; font-size: 18px; color: #ffffff; width: 100px;">Filters</div>
                <div style="float: right; width: 50px; text-align: center;">
                    <img id="imgSmRefreshData" src="Images/Refresh-icon.png" width="30" height="30" style="vertical-align: middle; margin-left: 25px;" alt="refreshIcon" />
                </div>
                <div style="clear: both"></div>
                <asp:DropDownList ID="lstSmAgents" runat="server"  AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmCampaign" runat="server" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmSkillGroup" runat="server"  AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmDateSelected" runat="server"  AppendDataBoundItems="true" Width="200" />
                <input type="text" id="txtSalesMetricsStartDate" value="" style="width: 75px; display: none; margin-right: 25px;" />
                <input type="text" id="txtSalesMetricsEndDate" value="" style="width: 75px; display: none;" />
            </div>
            <div id="LoaderSMImageHolder" style="width:700px;text-align:center;display: none;">
                <img id="Img1" src="Images/progress.gif" alt="" style="margin-top:75px;display: none;"/>
            </div>
            <div id="salesMetricReport" class="metricsTable">
                <div class="metricsRow">
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Talk Time</div>
                        <div id="lmOutPutSalesTalkTimeDays" class="odometer">1</div> :
                        <div id="lmOutPutSalesTalkTimeHours" class="odometer">1</div> :
                        <div id="lmOutPutSalesTalkTimeMinutes" class="odometer">1</div> :
                        <div id="lmOutPutSalesTalkTimeSeconds" class="odometer">1</div>
                        <div class="metricsCellInnerText">Day : Hr. : Min. : Sec.</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Total Calls</div>
                        <div id="lmOutSalesTotalCalls" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Valid Leads</div>
                        <div id="lmOutPutSalesValidLeads" class="odometer">0</div>
                    </div>
                </div>
                <div class="metricsRow">
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># of Contacts</div>
                        <div id="lmOutPutSalesNumOfContacts" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Closes</div>
                        <div id="lmOutPutSalesCloses" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># Important Actions</div>
                        <div id="lmOutPutSalesNumImportantActions" class="odometer">0</div>
                    </div>
                </div>
                <div class="metricsRow">
                    <div class="metricsCell" style="border: 0;"></div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># Quoted</div>
                        <div id="lmOutPutSalesNumQuoted"  class="odometer">0</div>
                    </div>
                    <div class="metricsCell" style="border: 0;"></div>
                </div>
            </div>
        </div>
        <!-- LEADS METRCIS -->
        <div class="metricsLeadsBox">
            <div id="leadMetricsParms" class="filterDivBox">
                <div style="float: left; font-size: 18px; color: #ffffff; width: 100px;">Filters</div>
                <div style="float: right; width: 50px; text-align: center;">
                    <img id="imgLmRefreshData" src="Images/Refresh-icon.png" width="30" height="30" style="vertical-align: middle; margin-left: 25px;" alt="refreshIcon" />
                </div>
                <div style="clear: both"></div>
                <asp:DropDownList ID="lstLmAgents" runat="server" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmCampaign" runat="server" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmSkillGroup" runat="server" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmDateSelected" runat="server" AppendDataBoundItems="true" Width="200" />
                <input type="text" id="txtLeadMetricsStartDate" value="" style="width: 75px; display: none; margin-right: 25px;" />
                <input type="text" id="txtLeadMetricsEndDate" value="" style="width: 75px; display: none;" />
            </div>
            <div id="LoaderLMImageHolder" style="width:700px;text-align:center;display: none;">
                <img id="testImage" src="Images/progress.gif" alt="" style="margin-top:75px;display: none;"/>
            </div>
            <div id="leadMetricReport" class="metricsTable">
                <div class="metricsRow">
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Unassigned Leads</div>
                        <div id="lmOutPutNewLead" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">Valid Leads</div>
                        <div id="lmOutValidLead" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">% Valid</div>
                        <div id="lmOutPutPercentValid" class="odometer">0</div>
                    </div>
                </div>
                <div class="metricsRow">
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># Contacted</div>
                        <div id="lmOutPutContacted" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">% Contacted</div>
                        <div id="lmOutPutPercentContacted" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># Quoted</div>
                        <div id="lmOutPutNumberQuoted" class="odometer">0</div>
                    </div>
                </div>
                <div class="metricsRow">
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">% Quoted</div>
                        <div id="lmOutPutPercentQuoted" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText"># Closed</div>
                        <div id="lmOutPutNumberClosed" class="odometer">0</div>
                    </div>
                    <div class="metricsCell">
                        <div class="metricsCellInnerText">% Closed</div>
                        <div id="lmOutPutPercentClosed" class="odometer">0</div>
                    </div>
                </div>
            </div>
            
        </div>
        <div class="divClear"></div>
    </div>
    <!-- SPACER -->
    <div style="height:5px;"></div>

    <!-- ***************** REPORTING ************************************* -->
    <!-- Score Card -->
    <div id="showScoreCard">
        <div id="topScoreCard" class="reportHeaderBarActive">
            <div id="ScoreCardHeader" class="reportHeadingLable">
                <label id="lblScoreCardReporting">Score Card</label>
            </div>
            <div id="ScoreCardParms" class="scParametersReports">
                <asp:DropDownList ID="lstScoreCardCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                <input id="txtScoreCardStartDate" type="text" style="width:100px; display: none;" />
                <input id="txtScoreCardEndDate" type="text" style="width:100px; display: none;" />
                <img id="ScoreCardRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="ScoreCardReport" class="reportBodyBar">
            <div id="IML1" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <table id="ScoreCardReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th width="175">Agent Name</th>
                        <th>Call Time</th>
                        <th>Call Hours</th>
                        <th>Call Time Score</th>
                        <th>Leads</th>
                        <th>Valid</th>
                        <th>Avg Call Time</th>
                        <th>Avg Call Time Score</th>
                        <th>Total Calls</th>
                        <th>Total Call Score</th>
                        <th>Five Minute Calls</th>
                        <th>Five Minute Score</th>
                        <th>Average Between</th>
                        <th>Average Between Score</th>
                        <th>Sales</th>
                        <th>Sales Score</th>
                        <th>Total Score</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Stacked Ranking Chart -->
    <div id="ShowStackRanking">   
        <div id="topStackRanking" class="reportHeaderBar">
            <div id="StackedRankingHeader" class="reportHeadingLable">
                    <label id="lblStackedRankingReporting">Stacked Ranking</label>
                </div>
                <div id="StackedRankingParms" class="scParametersReports">
                    <asp:DropDownList ID="lstStackedRankingCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                    <input type="text" id="txtStackedRankingStartDate" style="width:100px; display: none;" />
                    <input type="text" id="txtStackedRankingEndDate" style="width:100px; display: none;" />
                    <img id="StackedRankingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
                </div>
                <div style="clear: both;"></div>
        </div>
        <div id="StackedRankingReport" class="reportBodyBar">
            <div id="IML2" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="StackedRankingChart" style="min-height: 1000px; margin: 0 auto"></div>
        </div>
    </div>

    <!-- Carrier Mix REPORT -->
    <div id="ShowCarrierMix">
        <div id="topCarrierMix" class="reportHeaderBar">
            <div id="CarrierMixHeader" class="reportHeadingLable">
                    <label id="lblCarrierMixReporting">Carrier Mix</label>
                </div>
                <div id="CarrierMixParms" class="scParametersReports">
                    <asp:DropDownList ID="lstCarrierMixAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                    <asp:DropDownList ID="lstCarrierMixSkillGroup" runat="server" AppendDataBoundItems="true" Width="150" />
                    <asp:DropDownList ID="lstCarrierMixCampaign" runat="server" AppendDataBoundItems="true" Width="150" />
                    <asp:DropDownList ID="lstCarrierMixCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                    <input type="text" id="txtCarrierMixStartDate" style="width:100px; display: none;" />
                    <input type="text" id="txtCarrierMixEndDate" style="width:100px; display: none;" />
                    <img id="CarrierMixRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
                </div>
                <div style="clear: both;"></div>
        </div>
        <div id="CarrierMixReport" class="reportBodyBar">
            <div id="IML3" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;" />
            </div>
            <div id="CarrierMixReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="CarrierMixReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th></th>
                        <th colspan="2">MedSupp</th>
                        <th colspan="2">MA</th>
                        <th colspan="2">PDP</th>
                        <th colspan="2">Dental</th>
                        <th colspan="2">Total</th>
                    </tr>
                    <tr>
                        <th width="200">Title</th>
                        <th>MST</th>
                        <th>MSP</th>
                        <th>MAT</th>
                        <th>MAP</th>
                        <th>PDT</th>
                        <th>PDP</th>
                        <th>DVT</th>
                        <th>DVP</th>
                        <th>Total</th>
                        <th>Per</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <br />
            <!-- <div id="CarrierMixChart" style="min-height: 200px; margin: 0 auto"></div> -->
        </div>
    </div>

    <!-- Case Specialist REPORT -->
    <div id="ShowCaseSpecialist">
        <div id="topCaseSpecialist" class="reportHeaderBar">
            <div id="CaseSpecialistHeader" class="reportHeadingLable">
                <label id="lblCaseSpecialistReporting">Case Specialist</label>
            </div>
            <div id="CaseSpecialistParms" class="scParametersReports">
                <asp:DropDownList ID="lstCaseSpecialistSkillGroup" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstCaseSpecialistCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                <input type="text" id="txtCaseSpecialistStartDate" style="width:100px; display: none;" />
                <input type="text" id="txtCaseSpecialistEndDate" style="width:100px; display: none;" />
                <img id="CaseSpecialistRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="CaseSpecialistReport" class="reportBodyBar">
            <div id="IML4" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="CaseSpecialistReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="CaseSpecialistReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th width="200">Agent Name</th>
                        <th>Submitted</th>
                        <th>Pending</th>
                        <th>Approved</th>
                        <th>Declined</th>
                        <th>Withdrawn</th>
                        <th>NPA</th>
                        <th>Speed</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
             <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Fill Form Speed REPORT -->
    <div id="ShowFillFormSpeed">
        <div id="topFillForm" class="reportHeaderBar">
            <div id="FillFormHeader" class="reportHeadingLable">
                <label id="lblFillFormReporting">Fill Form Speed</label>
            </div>
            <div id="FillFormParms" class="scParametersReports">
                <asp:DropDownList ID="lstFillFormAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstFillFormSkillGroup" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstFillFormCampaign" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstFillFormCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                <input type="text" id="txtFillFormStartDate" style="width:100px; display: none;" />
                <input type="text" id="txtFillFormEndDate" style="width:100px; display: none;" />
                <img id="FillFormRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="FillFormReport" class="reportBodyBar">
            <div id="IML5" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="FillFormReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="FillFormReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th style="width: 200px;">Case Specialist</th>
                        <th>Form Count</th>
                        <th>AvgMinutes</th>
                        <th>Min Minutes</th>
                        <th>Max Minutes</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Goal REPORT -->
    <div id="ShowGoalReport">
        <div id="topGoalReport" class="reportHeaderBar">
            <div id="GoalHeader" class="reportHeadingLable">
                <label id="lblGoalReporting">Goal</label>
            </div>
            <div id="GoalParms" class="scParametersReports">
                <asp:DropDownList ID="lstGoalMonth" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstGoalYear" runat="server" AppendDataBoundItems="true" Width="150" />
                <img id="GoalRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="GoalReport" class="reportBodyBar">
            <div id="IML6" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="GoalReportingParameters_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="GoalReportingParameters" class="dashboardReportingTables" border="0" style="width: 600px">
                <thead>
                    <tr>
                        <th width="200">For the Period Ending</th>
                        <th>Days Worked</th>
                        <th>Total Work Days</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr style="background-color:#E0E6ED; color:#696969;">
                        <th id="GRP1" style="border: 1px solid #465C71;">0</th>
                        <th id="GRP2" style="border: 1px solid #465C71;">0</th>
                        <th id="GRP3" style="border: 1px solid #465C71;">0</th>
                    </tr>
                </tfoot>
            </table>
            <table id="GoalReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th width="200">Agent Name</th>
                        <th>Quota</th>
                        <th>MTD</th>
                        <th>Percent of Quota</th>
                        <th>Daily Average</th>
                        <th>Projected</th>
                        <th>Variance From Projected</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Incentive Tracking REPORT -->
    <div id="ShowIncentiveTracking">
        <div id="topIncentiveTracking" class="reportHeaderBar">
            <div id="IncentiveTrackingHeader" class="reportHeadingLable">
                <label id="lblIncentiveTrackingReporting">Incentive Tracking</label>
            </div>
            <div id="IncentiveTrackingParms" class="scParametersReports">
                <asp:DropDownList ID="lstIncentiveTrackingAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                <img id="IncentiveTrackingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="IncentiveTrackingReport" class="reportBodyBar">
            <div id="IML7" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="IncentiveTrackingReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="IncentiveTrackingReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                <thead>
                    <tr>
                        <th width="200">Agent Name</th>
                        <th>Policy Count</th>
                        <th>Hawaii</th>
                        <th>Cayman Islands</th>
                        <th>SPA Day KC</th>
                        <th>Golf KCCC</th>
                        <th>I-Pad 2</th>
                        <th>Capital Grille</th>
                        <th>48" Flat Screen</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Lead Volume REPORT -->
    <div id="ShowLeadVolume">
        <div id="topLeadVolume" class="reportHeaderBar">
            <div id="LeadVolumeHeader" class="reportHeadingLable">
                <label id="lblLeadVolumeReporting">Lead Volume</label>
            </div>
            <div id="LeadVolumeParms" class="scParametersReports">
                <asp:DropDownList ID="lstLeadVolumeAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstLeadVolumeSkillGroup" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstLeadVolumeCampaign" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstLeadVolumeCalendar" runat="server" AppendDataBoundItems="true" Width="150" />
                <input type="text" id="txtLeadVolumeStartDate" style="width:100px; display: none;" />
                <input type="text" id="txtLeadVolumeEndDate" style="width:100px; display: none;" />
                <img id="LeadVolumeRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="LeadVolumeReport" class="reportBodyBar">
            <div id="IML8" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="LeadVolumeReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="LeadVolumeReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th width="200">Campaign</th>
                        <th>Volume</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>
   
     <!-- PIPELINE REPORT -->
    <div id="ShowPipeline">
        <div id="topPipeline" class="reportHeaderBar">
            <div id="PipelineHeader" class="reportHeadingLable">
                <label id="lblPipelineReporting">Pipeline</label>
            </div>
            <div id="PipelineParms" class="scParametersReports">
                <asp:DropDownList ID="lstPipelineAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                <img id="PipelineRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="PiplineReport" class="reportBodyBar">
            <div id="IML9" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="PipelineReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="PipelineReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                <thead>
                    <tr>
                        <th>Status</th>
                        <th>Sub Status</th>
                        <th>Volume</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!--  Prioritized List REPORT -->
    <div id="PrioritizedList">
        <div id="topPrioritizedList" class="reportHeaderBar">
            <div id="PrioritizedListHeader" class="reportHeadingLable">
                <label id="lblPrioritizedListReporting">Prioritized List</label>
            </div>
            <div id="PrioritizedListParms" class="scParametersReports">
                <asp:DropDownList ID="lstPrioritizedListAgents" runat="server" AppendDataBoundItems="true" Width="150" />
                <img id="PrioritizedListRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="PrioritizedListReport" class="reportBodyBar">
            <div id="IML10" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="PrioritizedListReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="PrioritizedListReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                <thead>
                    <tr>
                        <th>Row Id</th>
                        <th>Account ID</th>
                        <th>Date Created</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Date of Birth</th>
                        <th>Campaign</th>
                        <th>Status</th>
                        <th>Sub Status 1</th>
                        <th>User</th>
                        <th>State</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Quota Tracking REPORT -->
    <div id="ShowQuotaTracking">
        <div id="topQuotaTracking" class="reportHeaderBar">
            <div id="QuotaTrackingHeader" class="reportHeadingLable">
                <label id="lblQuotaTrackingReporting">Quota Tracking</label>
            </div>
            <div id="QuotaTrackingParms" class="scParametersReports">
                <asp:DropDownList ID="lstQuotaTrackingMonth" runat="server" AppendDataBoundItems="true" Width="150" />
                <asp:DropDownList ID="lstQuotaTrackingYear" runat="server" AppendDataBoundItems="true" Width="150" />
                <img id="QuotaTrackingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="QuotaTrackingReport" class="reportBodyBar">
            <div id="IML11" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            
            <table id="QuotaTrackingReportingParameters" class="dashboardReportingTables" border="0" style="width: 600px">
                <thead>
                    <tr>
                        <th width="200">For the Period Ending</th>
                        <th>Days Worked</th>
                        <th>Total Work Days</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr style="background-color:#E0E6ED; color:#696969;">
                        <th id="QTRP1" style="border: 1px solid #465C71;">0</th>
                        <th id="QTRP2" style="border: 1px solid #465C71;">0</th>
                        <th id="QTRP3" style="border: 1px solid #465C71;">0</th>
                    </tr>
                </tfoot>
            </table>
            <div id="QuotaTrackingReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="QuotaTrackingReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                <thead>
                    <tr>
                        <th width="200">Agent Name</th>
                        <th>Quota</th>
                        <th>MTD</th>
                        <th>Percent of Quota</th>
                        <th>Daily Average</th>
                        <th>Projected</th>
                        <th>Variance From Projected</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <!-- Submission & Enrolled REPORT -->
    <div id="ShowSubmissionEnrolled">
        <div id="topSubmissionEnroll" class="reportHeaderBar">
            <div id="SubmissionEnrollHeader" class="reportHeadingLable">
                <label id="lblSubmissionEnrollReporting">Submission & Enrolled</label>
            </div>
            <div id="SubmissionEnrollParms" class="scParametersReports">
                <asp:DropDownList ID="lstSubmissionEnrollCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" Width="150px" />
                <img id="SubmissionEnrollRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" alt="refreshIcon" />
            </div>
            <div style="clear: both;"></div>
        </div>
        <div id="SubmissionEnrollReport" class="reportBodyBar">
            <div id="IML12" style="display: none;width: 1480px;margin: 0px auto 0px auto;padding: 5px;text-align:center;">
                <img src="Images/progress.gif" alt="" style="margin-top:50px;"/>
            </div>
            <div id="SubmissionsEnrollmentsReportingTable_wrapper" class="dataTables_wrapper" role="grid" style="overflow: auto; height: 300px;">
            <table id="SubmissionsEnrollmentsReportingTable" class="dashboardReportingTables" border="0">
                <thead>
                    <tr>
                        <th>Policy</th>
                        <th>Jan</th>
                        <th>Feb</th>
                        <th>Mar</th>
                        <th>Apr</th>
                        <th>May</th>
                        <th>Jun</th>
                        <th>Jul</th>
                        <th>Aug</th>
                        <th>Sep</th>
                        <th>Oct</th>
                        <th>Nov</th>
                        <th>Dec</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
                <tfoot>
                    <tr>
                        <th>TOTAL</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                        <th>0</th>
                    </tr>
                </tfoot>
            </table>
                </div>
            <div style="min-height:50px;"></div>
        </div>
    </div>

    <asp:HiddenField ID="UserInfo" runat="server"/>
    <asp:HiddenField ID="IsAutoHome" runat="server"/>
    <asp:HiddenField ID="IsSenior" runat="server"/>
    <asp:HiddenField ID="IsTermLife" runat="server"/>


    <!-- Jquery For the Dashdoard -->
    <script src="Scripts/jquery-dashboard-1.2.js" type="text/jscript"></script> 
</asp:Content>

