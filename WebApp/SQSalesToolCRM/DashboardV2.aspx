<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DashboardV2.aspx.cs" Inherits="DashboardV2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SelectCare Reporting Dashboard</title>
    <link rel="shortcut icon" href="Images/logo.png" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
    <meta http-equiv="EXPIRES" content="0" />

    <!--- SCRIPTS AREA ------>
    <script src="Scripts/jquery-1.8.2.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/jquery.dataTables.js"></script>
    <script src="Scripts/highcharts.js"></script>
    <script src="Scripts/highcharts-more.js"></script>
    <script src="Scripts/exporting.js"></script>
    <script src="Scripts/gray.js"></script>
    <script src="Scripts/odometer.js"></script>

    <!--- CSS AREA ----->

    
    <link rel="Stylesheet" href="Styles/Dashboard.css" type="text/css" />
    <link rel="Stylesheet" href="Styles/NumOdometer.css" type="text/css" />
    <link rel="Stylesheet" href="Styles/jquery-ui-1.8.4.custom.css" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
        <div id="scWrapper">
            <div id="scMetricsWrapper">
                <div id="totalSalesHeader">
                    <span class="scTextTiles">SelectCARE Dashboard</span>
                </div>
                <!--
                <div id="scGauges" style="height: 325px; padding-top: 10px;">
                    <div style="float: left; width: 310px;">
                        <div id="scTotalSalesGauge" style="width: 300px; height: 300px; margin: 0 auto"></div>
                    </div>
                    <div style="float: left; width: 870px;">
                        <img src="Images/PlaceHolder.jpg" width="870" height="300" />
                    </div>
                    <div style="float: right; width: 310px;">
                        <div id="scTotalLeads" style="width: 300px; height: 300px; margin: 0 auto"></div>
                    </div>
                    <div style="clear: both;"></div>
                </div>
                -->
                <div id="scSalesMetricArea">
                    <div>
                        <div class="scTextTiles" style="float: left; width: 425px; text-align: right;">Sales Metrics </div>
                        <div style="float: right; width: 100px; text-align: right; padding-right: 10px;">
                            <img src="Images/Refresh-icon.png" width="25" height="25" />
                        </div>
                        <div style="clear: both;"></div>
                    </div>

                    <div class="scParametersMetrics">

                        <select class="scSelect">
                            <option value="all">All-Agents</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>

                        <select class="scSelect">
                            <option value="all">All-Campaigns</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>

                        <select class="scSelect">
                            <option value="all">All-Skill Groups</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>

                        <select class="scSelect">
                            <option value="all">Date-Today</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>

                    </div>
                    <div class="scSalesDashBoxesLeft">
                        Talk Time<br />
                        <div id="talkHour" class="odometer">00</div>
                        h 
                        <div id="talkMinutes" class="odometer">00</div>
                        m
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        Total Calls<br />
                        <div id="totalCalls" class="odometer">000</div>
                    </div>
                    <div class="scSalesDashBoxesRight">
                        Valid Leads<br />
                        <div id="validLeads" class="odometer">000</div>
                    </div>
                    <div style="clear: both;"></div>
                    <div class="scSalesDashBoxesLeft">
                        # of Contacts<br />
                        <div id="numContacts" class="odometer">000</div>
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        Closes<br />
                        <div id="numCloses" class="odometer">000</div>
                    </div>
                    <div class="scSalesDashBoxesRight">
                        # Important Actions<br />
                        <div id="numImportantActions" class="odometer">000</div>
                        <div style="clear: both;"></div>
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        # Quoted<br />
                        <div id="numQuoted" class="odometer">000</div>

                    </div>
                    <div class="scSalesDashBoxesLeft"></div>
                    <div class="scSalesDashBoxesRight"></div>
                    <div style="clear: both;"></div>
                </div>

                <div id="scLeadMetricArea">
                    <div>
                        <div class="scTextTiles">Lead Metrics </div>
                    </div>
                    <div class="scParametersMetrics">
                        <asp:DropDownList ID="lstlmAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstlmCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstlmSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstlmDateSelected" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="lmRefreshData" src="Images/Refresh-icon.png" width="25" height="25" style="vertical-align: middle; margin-left: 25px;" /><br />
                        <input type="text" id="txtLeadMetricsStartDate" value="Start Date" class="scParmTextbox" style="display: none; margin-right:25px;" />
                        <input type="text" id="txtLeadMetricsEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        New Leads<br />
                        <div id="lmOutPutNewLead" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        Valid Leads<br />
                        <div id="lmOutValidLead" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesRight">
                        % Valid<br />
                        <div id="lmOutPutPercentValid" class="odometer">00</div>
                    </div>
                    <div style="clear: both;"></div>
                    <div class="scSalesDashBoxesLeft">
                        Contacted<br />
                        <div id="lmOutPutContacted" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        % Contacted<br />
                        <div id="lmOutPutPercentContacted" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesRight">
                        # Quoted<br />
                        <div id="lmOutPutNumberQuoted" class="odometer">00</div>
                    </div>
                    <div style="clear: both;"></div>
                    <div class="scSalesDashBoxesLeft">
                        % Quoted<br />
                        <div id="lmOutPutPercentQuoted" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesLeft">
                        # Closed<br />
                        <div id="lmOutPutNumberClosed" class="odometer">00</div>
                    </div>
                    <div class="scSalesDashBoxesRight">
                        % Closed<br />
                        <div id="lmOutPutPercentClosed" class="odometer">00</div>
                    </div>
                    <div style="clear: both;"></div>

                </div>

                <div style="clear: both;"></div>
                <!-- 
                    ----------- REPORTING BEGINGS ---------
                -->
                                 <!-- ScoreCard Report -->
                <div class="reportingHeader">
                    <div id="ScoreCardHeader" style="float: left; width: 400px;">
                        <img id="imgScoreCardCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgScoreCardExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblScoreCardReporting">Score Card Reporting</label>
                    </div>
                    <div id="ScoreCardParms" class="scParametersReports">
                        <asp:DropDownList ID="lstScoreCardCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtScoreCardStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtScoreCardEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="ScoreCardRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="ScoreCardReport">
                    <div style="padding: 10px;">
                        <table id="ScoreCardReportingTable" class="dashboardReportingTables" border="0">
                                <thead>
                                    <tr>
                                        <th colspan="17" style="font-size: 18px;">CPA Reporting</th>
                                    </tr>
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
                    </div>
                </div>
                <!-- Stacked Ranking Chart -->
                <div class="reportingHeader">
                    <div id="StackedRankingHeader" style="float: left; width: 400px;">
                        <img id="imgStackedRankingCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgStackedRankingExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblStackedRankingReporting">Stacked Ranking Reporting</label>
                    </div>
                    <div id="StackedRankingParms" class="scParametersReports">
                        <asp:DropDownList ID="lstStackedRankingCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtStackedRankingStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtStackedRankingEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="StackedRankingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="StackedRankingReport">
                    <div style="padding: 10px;">
                        <div id="StackedRankingChart" style="min-height:1000px; margin: 0 auto"></div>
                    </div>
                </div>
                 <!-- CPA Report -->
                <div class="reportingHeader">
                    <div id="CpaHeader" style="float: left; width: 400px;">
                        <img id="imgCpaCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgCpaExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblCpaReporting">CPA Reporting</label>
                    </div>
                    <div id="CpaParms" class="scParametersReports">
                        <asp:DropDownList ID="lstCpaCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtCpaStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtCpaEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="CpaRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="CpaReport">
                    <div style="padding: 10px;">
                        <table id="CpaReportingTable" class="dashboardReportingTables">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">CPA Reporting</th>
                                </tr>
                                <tr>
                                    <th width="200">Agent Name</th>
                                    <th>Valid Leads</th>
                                    <th>MedSupp Plans Closed</th>
                                    <th>% of Valid (MedSupp)</th>
                                    <th>MA Plans Closed</th>
                                    <th>% of Valid (MA)</th>
                                    <th>Policies Closed</th>
                                    <th>% of Valid (Total)</th>
                                    <th>Projected Close %</th>
                                    <th>Cost Per Acquisition</th>
                                    <th>Projected CPA</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <th>TOTAL</th>
                                    <th id="cpaTotal1">0</th>
                                    <th id="cpaTotal2">0</th>
                                    <th id="cpaTotal3">0</th>
                                    <th id="cpaTotal4">0</th>
                                    <th id="cpaTotal5">0</th>
                                    <th id="cpaTotal6">0</th>
                                    <th id="cpaTotal7">0</th>
                                    <th id="cpaTotal8">0</th>
                                    <th id="cpaTotal9">0</th>
                                    <th id="cpaTotal10">0</th>
                                </tr>
                            </tfoot>
                        </table>

                    </div>
                </div>
                <!-- PIPELINE REPORT -->
                <div class="reportingHeader">
                    <div id="PipelineHeader" style="float: left; width: 400px;">
                        <img id="imgPipelineCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgPipelineExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblPipelineReporting">Pipeline Reporting</label>
                    </div>
                    <div id="PipelineParms" class="scParametersReports">
                        <asp:DropDownList ID="lstPipelineAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="PipelineRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="PiplineReport">
                    <div style="padding: 10px;">
                        <table id="PipelineReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                            <thead>
                                <tr>
                                    <th colspan="3" style="font-size: 18px;">Pipeline Report</th>
                                </tr>
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
                </div>
                <!-- Incentive Tracking REPORT -->
                <div class="reportingHeader">
                    <div id="IncentiveTrackingHeader" style="float: left; width: 400px;">
                        <img id="imgIncentiveTrackingCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgIncentiveTrackingExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblIncentiveTrackingReporting">Incentive Tracking Report</label>
                    </div>
                    <div id="IncentiveTrackingParms" class="scParametersReports">
                        <asp:DropDownList ID="lstIncentiveTrackingAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="IncentiveTrackingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="IncentiveTrackingReport">
                    <div style="padding: 10px;">
                        <table id="IncentiveTrackingReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Incentive Tracking Report</th>
                                </tr>
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
                </div>

                <!-- Quota Tracking REPORT -->
                <div class="reportingHeader">
                    <div id="QuotaTrackingHeader" style="float: left; width: 400px;">
                        <img id="imgQuotaTrackingCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgQuotaTrackingExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblQuotaTrackingReporting">Quota Tracking Report</label>
                    </div>
                    <div id="QuotaTrackingParms" class="scParametersReports">
                        <asp:DropDownList ID="lstQuotaTrackingPlanType" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstQuotaTrackingCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="QuotaTrackingRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="QuotaTrackingReport">
                    <div style="padding: 10px;">
                        <table id="QuotaTrackingReportingTable" class="dashboardReportingTables" border="0" style="width: 100%">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Quota Tracking Report</th>
                                </tr>
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
                </div>

                <!-- Commision Dashboard REPORT -->
                <!-- THIS REPORT IS TURNED OFF FOR NOW
                <div class="reportingHeader">
                    <div id="CommisionDashboardHeader" style="float: left; width: 400px;">
                        <img id="imgCommisionDashboardCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgCommisionDashboardExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblCommisionDashboardReporting">Commission Dashboard</label>
                    </div>
                    <div id="CommisionDashboardParms" class="scParametersReports">
                        <asp:DropDownList ID="lstCommisionDashboardCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="CommisionDashboardRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="CommisionDashboardReport">
                    <div style="padding: 10px;">
                        <table id="CommisionReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Compensation Summary STILL NEEDS DATA</th>
                                </tr>
                                <tr>
                                    <th width="200">Compensation Type</th>
                                    <th>Amount</th>
                                    <th>Submitted Policies in Period</th>
                                    <th>Approved/Enrolled Policies in Period</th>
                                    <th>Percent Approved/Enrolled</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                -->

                <!-- Lead Volume REPORT -->
                <div class="reportingHeader">
                    <div id="LeadVolumeHeader" style="float: left; width: 400px;">
                        <img id="imgLeadVolumeCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgLeadVolumeExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblLeadVolumeReporting">Lead Volume</label>
                    </div>
                    <div id="LeadVolumeParms" class="scParametersReports">
                        <asp:DropDownList ID="lstLeadVolumeAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstLeadVolumeSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstLeadVolumeCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstLeadVolumeCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtLeadVolumeStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtLeadVolumeEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="LeadVolumeRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="LeadVolumeReport">
                    <div style="padding: 10px;">
                        <table id="LeadVolumeReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Lead Volume Report</th>
                                </tr>
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
                </div>

                <!-- Goal REPORT -->
                <div class="reportingHeader">
                    <div id="GoalHeader" style="float: left; width: 400px;">
                        <img id="imgGoalCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgGoalExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblGoalReporting">Goal Report</label>
                    </div>
                    <div id="GoalParms" class="scParametersReports">
                        <asp:DropDownList ID="lstGoalCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="GoalRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="GoalReport">
                    <div style="padding: 10px;">
                        <table id="GoalReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Goal Report</th>
                                </tr>
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
                </div>

                <!-- Case Specialist REPORT -->
                <div class="reportingHeader">
                    <div id="CaseSpecialistHeader" style="float: left; width: 400px;">
                        <img id="imgCaseSpecialistCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgCaseSpecialistExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblCaseSpecialistReporting">Case Specialist Report</label>
                    </div>
                    <div id="CaseSpecialistParms" class="scParametersReports">
                        <asp:DropDownList ID="lstCaseSpecialistSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstCaseSpecialistCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtCaseSpecialistStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtCaseSpecialistEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="CaseSpecialistRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="CaseSpecialistReport">
                    <div style="padding: 10px;">
                        <table id="CaseSpecialistReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Goal Report</th>
                                </tr>
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
                </div>

                <!-- Submission & Enrolled REPORT -->
                <div class="reportingHeader">
                    <div id="SubmissionEnrollHeader" style="float: left; width: 400px;">
                        <img id="imgSubmissionEnrollCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgSubmissionEnrollExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblSubmissionEnrollReporting">Submission & Enrolled Report</label>
                    </div>
                    <div id="SubmissionEnrollParms" class="scParametersReports">
                        <asp:DropDownList ID="lstSubmissionEnrollCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" Width="150px" />
                        <img id="SubmissionEnrollRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="SubmissionEnrollReport">
                    <div style="padding: 10px;">
                        <table id="SubmissionsEnrollmentsReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="13" style="font-size: 18px;">Submission & Enrolled Report</th>
                                </tr>
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
                </div>

                <!-- Premium REPORT -->
                <div class="reportingHeader">
                    <div id="PremiumHeader" style="float: left; width: 400px;">
                        <img id="imgPremiumCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgPremiumExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblPremiumReporting">Premium Report</label>
                    </div>
                    <div id="PremiumParms" class="scParametersReports">
                        <asp:DropDownList ID="lstPremiumAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <img id="PremiumRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="PremiumReport">
                    <div style="padding: 10px;">
                        <table id="Table1" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Premium Report STILL NEEDS DATA</th>
                                </tr>
                                <tr>
                                    <th style="width: 200px;">Agent</th>
                                    <th>---</th>
                                    <th>---</th>
                                    <th>---</th>
                                    <th>---</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>

                <!-- Carrier Mix REPORT -->
                <div class="reportingHeader">
                    <div id="CarrierMixHeader" style="float: left; width: 400px;">
                        <img id="imgCarrierMixCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgCarrierMixExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblCarrierMixReporting">Carrier Mix Report</label>
                    </div>
                    <div id="CarrierMixParms" class="scParametersReports">
                        <asp:DropDownList ID="lstCarrierMixAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstCarrierMixSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstCarrierMixCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstCarrierMixCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtCarrierMixStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtCarrierMixEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="CarrierMixRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="CarrierMixReport">
                    <div style="padding: 10px;">
                        <table id="CarrierMixReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Carrier Mix</th>
                                </tr>
                                <tr>
                                    <th> </th>
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
                </div>

                <!-- Fill Form REPORT -->
                <div class="reportingHeader">
                    <div id="FillFormHeader" style="float: left; width: 400px;">
                        <img id="imgFillFormCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgFillFormExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblFillFormReporting">Fill Form Report</label>
                    </div>
                    <div id="FillFormParms" class="scParametersReports">
                        <asp:DropDownList ID="lstFillFormAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFillFormSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFillFormCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFillFormCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtFillFormStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtFillFormEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="FillFormRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="FillFormReport">
                    <div style="padding: 10px;">
                        <table id="FillFormReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Fill Form</th>
                                </tr>
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
                </div>

                <!-- Fall Off REPORT -->
                <!-- HIDE FOR NOW
                <div class="reportingHeader">
                    <div id="FallOffHeader" style="float: left; width: 400px;">
                        <img id="imgFallOffCollapse" src="Images/Collapse.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <img id="imgFallOffExpand" src="Images/Expand.png" style="width: 25px; height: 25px; vertical-align: middle;" />
                        <label id="lblFallOffReporting">Fall Off Report</label>
                    </div>
                    <div id="FallOffParms" class="scParametersReports">
                        <asp:DropDownList ID="lstFallOffAgents" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFallOffSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFallOffCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <asp:DropDownList ID="lstFallOffCalendar" runat="server" class="scSelect" AppendDataBoundItems="true" />
                        <input type="text" id="txtFallOffStartDate" value="Start Date" class="scParmTextbox" style="display: none;" />
                        <input type="text" id="txtFallOffEndDate" value="End Date" class="scParmTextbox" style="display: none;" />
                        <img id="FallOffRefresh" src="Images/Refresh-icon.png" style="width: 25px; height: 25px; vertical-align: middle; margin-left: 25px; margin-right: 10px;" />
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div id="FallOffReport">
                    <div style="padding: 10px;">
                        <table id="FallOffReportingTable" class="dashboardReportingTables" border="0">
                            <thead>
                                <tr>
                                    <th colspan="11" style="font-size: 18px;">Fall Out</th>
                                </tr>
                                <tr>
                                    <th style="width: 200px;">Agent</th>
                                    <th>Count</th>
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
                </div>
                -->

            </div>
        </div>

    </form>
    <script src="Scripts/jquery-dashboard.js"></script>

</body>
</html>
