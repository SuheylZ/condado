

$(document).ready(function () {
    
    window.userkey = $("#MainContent_UserInfo").val();
    //$.removeCookie("incentiveTrackingInputs");
    //alert(userkey);
    window.g_ReportInfo = new Array();

    $.ajax({
        "dataType": 'json',
        "contentType": "application/json; charset=utf-8",
        "type": "POST",
        "url": "Services/DashboardReporting.asmx/LoadAllReports4Display",
        "data": JSON.stringify({
            BusinessUnit: "1",
        }),
        "success": function (msg) {
            var json = jQuery.parseJSON(msg.d);
            $.each(json.aaReports, function (i, item) {
                var x = (item.DT_RowId - 1);
                g_ReportInfo[x] = item.DT_RowId + "," + item.ReportTitle + "," + item.StroedProc + "," + item.DataWarehouse + "," + item.AgentType + "," + item.Active;
            });
            
            LoadAllReports();
        }
    });

    //GLOBAL ARRAYS
    //This ARRAY allows the reports to be hidden after they have been loaded
    window.g_ShowHide = new Array();
    g_ShowHide[0] = true; // QuotaTracking
    g_ShowHide[1] = true; // CPA Report
    g_ShowHide[2] = true; // Pipeline Report
    g_ShowHide[3] = true; // Incentive Tracking Report
    g_ShowHide[4] = true; // Stacked Ranking Report
    g_ShowHide[5] = true; // Lead Volume Report
    g_ShowHide[6] = true; // Goal Report
    g_ShowHide[7] = true; //Case Specialist Report
    g_ShowHide[8] = true; // Submission Enroll Report
    g_ShowHide[9] = true; // Premium Report
    g_ShowHide[10] = true; // Carrier Mix Report
    g_ShowHide[11] = true; // Fill Form Report
    g_ShowHide[12] = true; // Fall Off Report
    g_ShowHide[13] = true; // Score Card Report FALSE because it stays open to begin
    g_ShowHide[14] = true; // Prioritized List Report

    window.g_StartDate = new Array();
    g_StartDate[0] = "";
    g_StartDate[1] = "";

    window.g_TableScrollHeight = new Array();
    g_TableScrollHeight[0] = 0;

    window.g_ParmShowHide = new Array();
    g_ParmShowHide[0] = false; // Carrier Mix
    g_ParmShowHide[1] = true; // Score Card Report TRUE because it stays open to begin
    g_ParmShowHide[2] = false; // Stacked Ranking 
    g_ParmShowHide[3] = false; // Case Specialist
    g_ParmShowHide[4] = false; // Fill Form
    g_ParmShowHide[5] = false; // Goal
    g_ParmShowHide[6] = false; // Incentive Tracking
    g_ParmShowHide[7] = false; // Lead Volume
    g_ParmShowHide[8] = false; // Pipeline
    g_ParmShowHide[9] = false; // Prioritized List
    g_ParmShowHide[10] = false; // QuotaTracking
    g_ParmShowHide[11] = false; // Submission Enroll

    //window.g_ReportShowHide = false;
    // GLOBAL CALLS
    CalendarTodayStartEndDates();
    SetupDatePickerTextBoxes();
    CookieCreator();
    HideAllReports();

    // Splits all the Data for all the Reports...
    function LoadAllReports() {
        var showScoreCard = g_ReportInfo[0].split(',');
        var ShowStackRanking = g_ReportInfo[1].split(',');//CPA
        var ShowCPA = g_ReportInfo[2].split(',');
        var ShowPipeline = g_ReportInfo[3].split(',');
        var ShowIncentiveTracking = g_ReportInfo[4].split(',');
        var ShowQuotaTracking = g_ReportInfo[5].split(',');
        var ShowCommision = g_ReportInfo[6].split(',');
        var ShowLeadVolume = g_ReportInfo[7].split(',');
        var ShowGoal = g_ReportInfo[8].split(',');
        var ShowCaseSpecialist = g_ReportInfo[9].split(',');
        var ShowSubmissionEnrolled = g_ReportInfo[10].split(',');
        var ShowPremium = g_ReportInfo[11].split(',');
        var ShowCarrierMix = g_ReportInfo[12].split(',');
        var ShowFillFormSpeed = g_ReportInfo[13].split(',');
        var ShowFallOff = g_ReportInfo[14].split(',');
        var ShowPrioritized = g_ReportInfo[15].split(',');

        // Show/Hide reports
       if (showScoreCard[5] == 1) {
          $('#showScoreCard').show();
       }
       if (ShowStackRanking[5] == 1) {
           $('#ShowStackRanking').show();
           hideshowStackedRanking();
       }
       if (ShowCPA[5] == 1) {
           //$('#ShowCPA').show();
           //hideshowStackedRanking();
       }
       if (ShowPipeline[5] == 1) {
           $('#ShowPipeline').show();
           hideshowPipeline();
       }
       if (ShowIncentiveTracking[5] == 1) {
           $('#ShowIncentiveTracking').show();
           hideshowIncentiveTracking();
       }
       if (ShowQuotaTracking[5] == 1) {
           $('#ShowQuotaTracking').show();
           hideshowQuotaTracking();
       }
       if (ShowCommision[5] == 1) {
           //$('#ShowCommision').show();
       }
       if (ShowLeadVolume[5] == 1) {
           $('#ShowLeadVolume').show();
           hideshowLeadVolume();
       }
       if (ShowGoal[5] == 1) {
           $('#ShowGoalReport').show();
           hideshowGoalReport();
       }
       if (ShowCaseSpecialist[5] == 1) {
           $('#ShowCaseSpecialist').show();
           hideshowCaseSpecialistReport();
       }
       if (ShowSubmissionEnrolled[5] == 1) {
           $('#ShowSubmissionEnrolled').show();
           hideshowSubmissionEnrollReport();
       }
       if (ShowPremium[5] == 1) {
           //$('#ShowPremium').show();
           //hideshowShowPremiumReport();
       }
       if (ShowCarrierMix[5] == 1) {
           $('#ShowCarrierMix').show();
           hideshowCarrierMixReport();
       }
       if (ShowFillFormSpeed[5] == 1) {
           $('#ShowFillFormSpeed').show();
           hideshowFillFormReport();
       } 

       if (ShowFallOff[5] == 1) {
           //$('#ShowFallOff').show();
           //hideshowFallOffReport();
       }
       
       if (ShowPrioritized[5] == 1) {
           $('#PrioritizedList').show();
           hideshowPrioritizedList();
       }
       
    }

    function HideAllReports() {
        $('#showScoreCard').hide();
        $('#ShowStackRanking').hide();
        $('#ShowCarrierMix').hide();
        $('#ShowCaseSpecialist').hide();
        $('#ShowFillFormSpeed').hide();
        $('#ShowFillForm').hide();
        $('#ShowGoalReport').hide();
        $('#ShowIncentiveTracking').hide();
        $('#ShowLeadVolume').hide();
        $('#ShowPipeline').hide();
        $('#PrioritizedList').hide();
        $('#ShowQuotaTracking').hide();
        $('#ShowSubmissionEnroll').hide();
    }

    
    // LOAD REPORTS FROM WEB SERVICE Using Cookie

    loadLeadMetricsFromCookie();
    loadSalesMetricsFromCookie();
    
    loadScoreCardFromCookie();

    
    loadStackedRankingChart(g_StartDate[0], g_StartDate[1]);
  

    // SALES METRICS


    function loadSalesMetricsFromCookie() {
        var dataLead = JSON.parse($.cookie("salesMetricsInputs"));
        var strAgent, strCampaigns, strSkillGroup, strDateOptionSelected, strStartDate, strEndDate
        $.each(dataLead, function () {
            strAgent = this['Agent'];
            strCampaigns = this['Campaigns'];
            strSkillGroup = this['SkillGroup'];
            strDateOptionSelected = this['DateOptionSelected'];
            strStartDate = this['StartDate'];
            strEndDate = this['EndDate'];
        });
        $('#MainContent_lstSmAgents').val(strAgent).attr("selected", "selected");
        $('#MainContent_lstSmCampaign').val(strCampaigns).attr("selected", "selected");
        $('#MainContent_lstSmSkillGroup').val(strSkillGroup).attr("selected", "selected");
        $('#MainContent_lstSmDateSelected').val(strDateOptionSelected).attr("selected", "selected");

        if (strDateOptionSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateOptionSelected);
            loadSalesMetricsData(strAgent, strCampaigns, strSkillGroup, strReturnedDates[0], strReturnedDates[1]);
        } else {
            // == 7 
            $("#txtSalesMetricsStartDate").val(strStartDate);
            $("#txtSalesMetricsEndDate").val(strEndDate);
            $('#txtSalesMetricsStartDate').show();
            $('#txtSalesMetricsEndDate').show();
            if (strStartDate && strEndDate) {
                loadSalesMetricsData(strAgent, strCampaigns, strSkillGroup, strStartDate, strEndDate);
            }
        }
    }

    $('#fltSalesMetrics').click(function () {
        
        if ($('#salesMetricsParms').is(':hidden')) {
            $('#salesMetricReport').hide();
            $('#salesMetricsParms').slideDown("slow");
        } else {
            $('#salesMetricsParms').slideUp("slow", function () {
                $('#salesMetricReport').fadeIn("slow");
            });
            
        }
        
        
    });


    $("#imgSmRefreshData").click(function () {
        var strAgents = $("#MainContent_lstSmAgents").val();
        var strCampaign = $("#MainContent_lstSmCampaign").val();
        var strSkillGroup = $("#MainContent_lstSmSkillGroup").val();
        var strDateSelected = $("#MainContent_lstSmDateSelected").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadSalesMetricsData(strAgents, strCampaign, strSkillGroup, strReturnedDates[0], strReturnedDates[1]);
        } else {
            // == 7 
            var isStartDateGood = isDate($("#txtSalesMetricsStartDate").val())
            var isEndDateGood = isDate($("#txtSalesMetricsEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadSalesMetricsData(strAgents, strCampaign, strSkillGroup, $("#txtSalesMetricsStartDate").val(), $("#txtSalesMetricsEndDate").val());
            }
        }
        $('#salesMetricsParms').hide();
    });

    $('#MainContent_lstSmDateSelected').click(function () {
        var calId = $('#MainContent_lstSmDateSelected').val();

        if (calId == 7) {
            $('#txtSalesMetricsStartDate').show();
            $('#txtSalesMetricsEndDate').show();
        } else {
            $('#txtSalesMetricsStartDate').hide();
            $('#txtSalesMetricsEndDate').hide();
        }
    });

    function loadSalesMetricsData(strAgents, strCampaigns, strSkillGroup, strStartDate, strEndDate) {

        var strDateSelected = $("#MainContent_lstSmDateSelected").val();
        var salesMetricsInputs = [
            { 'Agent': strAgents, 'Campaigns': strCampaigns, 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': strDateSelected }
        ];
        $.cookie("salesMetricsInputs", JSON.stringify(salesMetricsInputs), { expires: 30 });
   
        lmOutPutSalesTalkTimeDays.innerHTML = '0';
        lmOutPutSalesTalkTimeHours.innerHTML = '0';
        lmOutPutSalesTalkTimeMinutes.innerHTML = '0';
        lmOutPutSalesTalkTimeSeconds.innerHTML = '0';
        lmOutSalesTotalCalls.innerHTML = '0';
        lmOutPutSalesValidLeads.innerHTML = '0';
        lmOutPutSalesNumOfContacts.innerHTML = '0';
        lmOutPutSalesCloses.innerHTML = '0';
        lmOutPutSalesNumImportantActions.innerHTML = '0';
        lmOutPutSalesNumQuoted.innerHTML = '0';
        $.ajax({
            "dataType": 'json',
            "contentType": "application/json; charset=utf-8",
            "type": "POST",
            "url": "Services/DashboardReporting.asmx/GetSalesMetricsData",
            "data": JSON.stringify({
                varAgents: strAgents,
                varCampaigns: strCampaigns,
                varSkillGroup: strSkillGroup,
                varStartDate: strStartDate,
                varEndDate: strEndDate
            }),
            "success": function (msg) {
                var json = jQuery.parseJSON(msg.d);
                $.each(json, function () {
                    lmOutPutSalesTalkTimeDays.innerHTML = this['TalkTimeDay'];
                    lmOutPutSalesTalkTimeHours.innerHTML = this['TalkTimeHours'];
                    lmOutPutSalesTalkTimeMinutes.innerHTML = this['TalkTimeMinutes'];
                    lmOutPutSalesTalkTimeSeconds.innerHTML = this['TalkTimeSeconds'];
                    lmOutSalesTotalCalls.innerHTML = this['TotalCalls'];
                    lmOutPutSalesValidLeads.innerHTML = this['ValidLeads'];
                    lmOutPutSalesNumOfContacts.innerHTML = this['NumOfContacts'];
                    lmOutPutSalesCloses.innerHTML = this['Closes'];
                    lmOutPutSalesNumImportantActions.innerHTML = this['NumImportantActions'];
                    lmOutPutSalesNumQuoted.innerHTML = this['NumQuoted'];
                });
            },
            beforeSend: function () {
                $('#salesMetricReport').hide();
                $('#LoaderSMImageHolder').show();
                $('#Img1').show();
            },
            complete: function () {
                $('#Img1').hide();
                $('#LoaderSMImageHolder').delay(2000).hide();
                $('#salesMetricReport').fadeIn("slow");
            }
        });
        
    }


    //----------------------------------------------------------------------------
    // LEAD METRICS 


    function loadLeadMetricsFromCookie() {
        var dataLead = JSON.parse($.cookie("leadMetricsInputs"));
        var strLMAgent, strLMCampaigns, strLMSkillGroup, strLMDateOptionSelected, strLMStartDate, strLMEndDate
        $.each(dataLead, function () {
            strLMAgent = this['Agent'];
            strLMCampaigns = this['Campaigns'];
            strLMSkillGroup = this['SkillGroup'];
            strLMDateOptionSelected = this['DateOptionSelected'];
            strLMStartDate = this['StartDate'];
            strLMEndDate = this['EndDate'];
        });
        $('#MainContent_lstLmAgents').val(strLMAgent).attr("selected", "selected");
        $('#MainContent_lstLmCampaign').val(strLMCampaigns).attr("selected", "selected");
        $('#MainContent_lstLmSkillGroup').val(strLMSkillGroup).attr("selected", "selected");
        $('#MainContent_lstLmDateSelected').val(strLMDateOptionSelected).attr("selected", "selected");

        if (strLMDateOptionSelected <= 6) {
            var strReturnedDates = GetDateSelected(strLMDateOptionSelected);
            loadLeadMetricsData(strLMAgent, strLMCampaigns, strLMSkillGroup, strReturnedDates[0], strReturnedDates[1]);
        } else {
            // == 7 
            $("#txtLeadMetricsStartDate").val(strLMStartDate);
            $("#txtLeadMetricsEndDate").val(strLMEndDate);
            $('#txtLeadMetricsStartDate').show();
            $('#txtLeadMetricsEndDate').show();
            if (strLMStartDate && strLMEndDate) {
                loadLeadMetricsData(strLMAgent, strLMCampaigns, strLMSkillGroup, strLMStartDate, strLMEndDate);
            }
        }
    }

    $('#fltLeadMetrics').click(function () {
        if ($('#leadMetricsParms').is(':hidden')) {
            $('#leadMetricReport').hide();
            $('#leadMetricsParms').slideDown("slow");
        } else {
            $('#leadMetricsParms').slideUp("slow", function () {
                $('#leadMetricReport').fadeIn("slow");
            });

        }
    });


    function loadLeadMetricsData(strAgents, strCampaigns, strSkillGroup, strStartDate, strEndDate) {
        var strDateSelected = $("#MainContent_lstLmDateSelected").val();
        var leadMetricsInputs = [
            { 'Agent': strAgents, 'Campaigns': strCampaigns, 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': strDateSelected }
        ];
        $.cookie("leadMetricsInputs", JSON.stringify(leadMetricsInputs), { expires: 30 });

        //$('#scLeadMetricArea').fadeTo(500, 0.6);
        lmOutPutNewLead.innerHTML = "0";
        lmOutValidLead.innerHTML = "0";
        lmOutPutPercentValid.innerHTML = "0";
        lmOutPutContacted.innerHTML = "0";
        lmOutPutPercentContacted.innerHTML = "0";
        lmOutPutNumberQuoted.innerHTML = "0";
        lmOutPutPercentQuoted.innerHTML = "0";
        lmOutPutNumberClosed.innerHTML = "0";
        lmOutPutPercentClosed.innerHTML = "0";
        $.ajax({
            "dataType": 'json',
            "contentType": "application/json; charset=utf-8",
            "type": "POST",
            "url": "Services/DashboardReporting.asmx/GetLeadMetricsData",
            "data": JSON.stringify({
                varAgents: strAgents,
                varCampaigns: strCampaigns,
                varSkillGroup: strSkillGroup,
                varStartDate: strStartDate,
                varEndDate: strEndDate
            }),
            "success": function (msg) {
                var json = jQuery.parseJSON(msg.d);
                $.each(json, function () {
                    lmOutPutNewLead.innerHTML = this['NewLeads'];
                    lmOutValidLead.innerHTML = this['ValidLeads'];
                    lmOutPutPercentValid.innerHTML = this['ValidPercent'];
                    lmOutPutContacted.innerHTML = this['Contacted'];
                    lmOutPutPercentContacted.innerHTML = this['ContactedPercent'];
                    lmOutPutNumberQuoted.innerHTML = this['Quoted'];
                    lmOutPutPercentQuoted.innerHTML = this['QuotedPercent'];
                    lmOutPutNumberClosed.innerHTML = this['Closed'];
                    lmOutPutPercentClosed.innerHTML = this['ClosedPercent'];
                });
            },
            beforeSend: function () {
                $('#leadMetricReport').hide();
                $('#LoaderLMImageHolder').show();
                $('#testImage').show();
            },
            complete: function () {
                $('#testImage').hide();
                $('#LoaderLMImageHolder').delay(2000).hide();
                $('#leadMetricReport').fadeIn("slow");
            }
        });
        
    }

    //Hide Show Textbox for Calendar

    $('#MainContent_lstLmDateSelected').click(function () {
        var calId = $('#MainContent_lstLmDateSelected').val();

        if (calId == 7) {
            $('#txtLeadMetricsStartDate').show();
            $('#txtLeadMetricsEndDate').show();
        } else {
            $('#txtLeadMetricsStartDate').hide();
            $('#txtLeadMetricsEndDate').hide();
        }
    });

    // LEAD Metrics Button
    $("#imgLmRefreshData").click(function () {
        var strAgents = $("#MainContent_lstLmAgents").val();
        var strCampaign = $("#MainContent_lstLmCampaign").val();
        var strSkillGroup = $("#MainContent_lstLmSkillGroup").val();
        var strDateSelected = $("#MainContent_lstLmDateSelected").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadLeadMetricsData(strAgents, strCampaign, strSkillGroup, strReturnedDates[0], strReturnedDates[1]);
        } else {
            // == 7 
            var isStartDateGood = isDate($("#txtLeadMetricsStartDate").val())
            var isEndDateGood = isDate($("#txtLeadMetricsEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadLeadMetricsData(strAgents, strCampaign, strSkillGroup, $("#txtLeadMetricsStartDate").val(), $("#txtLeadMetricsEndDate").val());
            }
        }
        $('#leadMetricsParms').hide();
        //$('testImage').show();
        //$('#leadMetricsParms').slideUp("slow", function () {
            //$('#LoaderLMImageHolder').show();
        //});
    });

    // ------------  CHARTS ----------------------------
    //  ----- Stack Ranking Chart ----


    $('#topStackRanking').click(function () {
        $('#MainContent_lstStackedRankingCalendar').on('click', function () {
            g_ParmShowHide[2] = true;
        });
        $('#txtStackedRankingStartDate').on('click', function () {
            g_ParmShowHide[2] = true;
        });
        $('#txtStackedRankingEndDate').on('click', function () {
            g_ParmShowHide[2] = true;
        });
        $('#StackedRankingRefresh').on('click', function () {
            g_ParmShowHide[2] = true;
        });

        if (g_ParmShowHide[2] == false) {
            hideshowStackedRanking();
            if (g_ShowHide[4] == true) {
                window.g_ShowHide[4] = false;
                loadStackedRankingChart(g_StartDate[0], g_StartDate[1]);
            }
        } else {
            g_ParmShowHide[2] = false;
        }
        
    });

    $('#MainContent_lstStackedRankingCalendar').click(function () {
        var calId = $('#MainContent_lstStackedRankingCalendar').val();

        if (calId == 7) {
            $('#txtStackedRankingStartDate').show();
            $('#txtStackedRankingEndDate').show();
        } else {
            $('#txtStackedRankingStartDate').hide();
            $('#txtStackedRankingEndDate').hide();
        }
    });
    $('#StackedRankingRefresh').click(function () {
        var strDateSelected = $("#MainContent_lstStackedRankingCalendar").val();
        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadStackedRankingChart(strReturnedDates[0], strReturnedDates[1]);
        }
        if (strDateSelected == 7) {
            var isStartDateGood = isDate($("#txtStackedRankingStartDate").val())
            var isEndDateGood = isDate($("#txtStackedRankingEndDate").val())
            if (isStartDateGood && isEndDateGood) {

                loadStackedRankingChart($("#txtStackedRankingStartDate").val(), $("#txtStackedRankingEndDate").val());
            }
        }
    });

    function hideshowStackedRanking() {
        if ($('#StackedRankingReport').is(':hidden')) {
            $('#topStackRanking').removeClass("reportHeaderBar");
            $('#topStackRanking').addClass("reportHeaderBarActive");
            $('#StackedRankingParms').show();
            $('#StackedRankingReport').slideDown("slow");
        }
        else {
            $('#topStackRanking').removeClass("reportHeaderBarActive");
            $('#topStackRanking').addClass("reportHeaderBar");
            $('#StackedRankingReport').slideUp("slow");
            $('#StackedRankingParms').hide();
        }
    }

    //Build Chart
    function loadStackedRankingChart(startDate, endDate) {
        $('#StackedRankingChart').highcharts({
            chart: {
                type: 'bar',
                events: { load: GetStackedRankingChartAgents(startDate, endDate) }
            },
            title: {
                text: 'Stacked Ranking Chart'
            },
            yAxis: {
                min: 0
            },
            legend: {
                backgroundColor: '#000000',
                reversed: false
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                },
                series: {
                    stacking: 'normal'

                }
            },
            loading: {
                hideDuration: 1000,
                showDuration: 1000
            },
            scrollbar: {
                enabled: true
            },
            series: [{
                name: 'Temp',
                data: [1]
            }, {
                name: 'Temp',
                data: [1]
            }, {
                name: 'Temp',
                data: [1]
            }]
        });
    };
    // Get Json Data for Chart
    function GetStackedRankingChartAgents(startDate, endDate) {
        var agentNames = "";
        var totalRecordCount = "";
        $.ajax({
            "dataType": 'json',
            "contentType": "application/json; charset=utf-8",
            "type": "POST",
            "url": "Services/DashboardReporting.asmx/GetStackedRankingReport",
            "data": JSON.stringify({ varStartDate: startDate, varEndDate: endDate }),
            "success": function (msg) {
                var json = jQuery.parseJSON(msg.d);
                $.each(json.aaRecordCount, function (i, item) {
                    totalRecordCount = item.Total;
                });

                var Agents = [];
                var MedSup = [];
                var MedAdv = [];
                var Pdp = [];
                $.each(json.aaAgents, function (i, item) {
                    Agents.push(item.AgentName);
                    MedSup.push(item.Ms);
                    MedAdv.push(item.Ma);
                    Pdp.push(item.Dpd);
                });
                addToChart(Agents, MedSup, MedAdv, Pdp, totalRecordCount)
            }
        });
    }
    // Load Json Data into chart
    function addToChart(Agents, MedSup, MedAdv, Pdp, totalRecordCount) {

        var ChartHeight = totalRecordCount * 20;
        $("#StackedRankingReport").css("{height:" + ChartHeight + "}");
        var chart = $('#StackedRankingChart').highcharts();

        chart.xAxis[0].setCategories(Agents);
        chart.series[0].update({ name: 'Medicare Sup' });
        chart.series[1].update({ name: 'Medicare Adv' });
        chart.series[2].update({ name: 'PDP' });
        chart.series[0].setData(MedSup);
        chart.series[1].setData(MedAdv);
        chart.series[2].setData(Pdp);

        chart.setSize(1480, ChartHeight);
    }

    // ---- Carrier Mix Chart --------

    function loadCarrierMixChart(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        var DataSeries = [];
        var TempSeries = [];
        var Title = [];
        var Total = [];
        // GetCarrierMixChart(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate)
        $.ajax({
            "dataType": 'json',
            "contentType": "application/json; charset=utf-8",
            "type": "POST",
            "url": "Services/DashboardReporting.asmx/GetCarrierMixChartReport",
            "data": JSON.stringify({ varAgent: strAgent, varSkillGroup: strSkillGroup, varCampaign: strCampaign, varStartDate: strStartDate, varEndDate: strEndDate }),
            "success": function (msg) {
                var json = jQuery.parseJSON(msg.d);

                $.each(json.aaData, function (i, item) {
                    Title.push(item.Title);
                    Total.push(item.Total);
                });
            }
        });

        var mytest = "name";
        $('#CarrierMixChart').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: ''
            },
            xAxis: {
                categories: ['1/31/2014']
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Policies Sold'
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: mytest
        });

    }

    // -------------REPORTING -------------------------

    //  ----- ScoreCard REPORTING

    function loadScoreCardFromCookie() {
        var data = JSON.parse($.cookie("scoreCardInputs"));
        var strDateOptionSelected, strStartDate, strEndDate
        $.each(data, function () {
            strDateOptionSelected = this['DateOptionSelected'];
            strStartDate = this['StartDate'];
            strEndDate = this['EndDate'];
        });
        $('#MainContent_lstScoreCardCalendar').val(strDateOptionSelected).attr("selected", "selected");

        if (strDateOptionSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateOptionSelected);
            loadScoreCardReportData(strReturnedDates[0], strReturnedDates[1]);
        } else {
            $('#txtScoreCardStartDate').show();
            $('#txtScoreCardEndDate').show();
            $("#txtScoreCardStartDate").val(strStartDate);
            $("#txtScoreCardEndDate").val(strEndDate);
            if (strStartDate && strEndDate) {
                loadScoreCardReportData(strStartDate, strEndDate);
            }
        }

    }


    $('#topScoreCard').click(function () {
        $('#MainContent_lstScoreCardCalendar').on('click', function () {
            g_ParmShowHide[1] = true;
        });
        $('#txtScoreCardStartDate').on('click', function () {
            g_ParmShowHide[1] = true;
        });
        $('#txtScoreCardEndDate').on('click', function () {
            g_ParmShowHide[1] = true;
        });
        $('#ScoreCardRefresh').on('click', function () {
            g_ParmShowHide[1] = true;
        });

        if (g_ParmShowHide[1] == false) {
            hideshowScoreCardReport();
            if (g_ShowHide[13] == true) {
                window.g_ShowHide[13] = false;
                
            }
        } else {
            g_ParmShowHide[1] = false;
        }
    });

    $('#ScoreCardRefresh').click(function () {
        var strDateSelected = $("#MainContent_lstScoreCardCalendar").val();
        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadScoreCardReportData(strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtScoreCardStartDate").val());
            var isEndDateGood = isDate($("#txtScoreCardEndDate").val());
            if (isStartDateGood && isEndDateGood) {
                loadScoreCardReportData($("#txtScoreCardStartDate").val(), $("#txtScoreCardEndDate").val());
            }
        }
    });

    $('#MainContent_lstScoreCardCalendar').click(function () {
        var calId = $('#MainContent_lstScoreCardCalendar').val();

        if (calId == 7) {
            $('#txtScoreCardStartDate').show();
            $('#txtScoreCardEndDate').show();
        } else {
            $('#txtScoreCardStartDate').hide();
            $('#txtScoreCardEndDate').hide();
        }
    });

    function hideshowScoreCardReport() {
        if ($('#ScoreCardReport').is(':hidden')) {
            
            $('#topScoreCard').removeClass("reportHeaderBar");
            $('#topScoreCard').addClass("reportHeaderBarActive");
            $('#ScoreCardParms').show();
            $('#ScoreCardReport').slideDown("slow");
        }
        else {
            $('#topScoreCard').removeClass("reportHeaderBarActive");
            $('#topScoreCard').addClass("reportHeaderBar");
            $('#ScoreCardReport').slideUp("slow");
            $('#ScoreCardParms').hide();
        }
    }

    function loadScoreCardReportData(strStartDate, strEndDate) {
        $('#ScoreCardReportingTable').hide();
        $('#IML1').show();

        // Store Parameters in Cookie
        var DatePickerID = $('#MainContent_lstScoreCardCalendar').val();
        var scoreCardInputs = [
            { 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': DatePickerID }
        ];
        $.cookie("scoreCardInputs", JSON.stringify(scoreCardInputs), { expires: 30 });

        var intCounter = 0;
        $("#ScoreCardReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripeClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetScoreCardReport",
            "bDeferRender": false,
            "bScrollCollapse": true,
            "sScrollY": 300,
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strStartDate", "value": strStartDate });
                aoData.push({ "name": "strEndDate", "value": strEndDate });

            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success": function (msg) {

                        var json = jQuery.parseJSON(msg.d);
                        fnCallback(json);
                        $("#ScoreCardReportingTable").show();
                    }
                });

            },
            "fnFooterCallback": function (nRow, aoData) {
                $('#IML1').hide();
                $('#ScoreCardReportingTable').show();
            }
        });

    }

    // ----- Carrier Mix REPORTING

    $('#topCarrierMix').click(function () {
        
        $('#MainContent_lstCarrierMixAgents').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#MainContent_lstCarrierMixSkillGroup').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#MainContent_lstCarrierMixCampaign').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#MainContent_lstCarrierMixCalendar').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#txtCarrierMixStartDate').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#txtCarrierMixEndDate').on('click', function () {
            g_ParmShowHide[0] = true;
        });
        $('#CarrierMixRefresh').on('click', function () {
            g_ParmShowHide[0] = true;
        });

        if (g_ParmShowHide[0] == false) {
            hideshowCarrierMixReport();
            // If it's the first time through load report. 
            if (g_ShowHide[10] == true) {
                g_ShowHide[10] = false;
                var data = JSON.parse($.cookie("carrierMixInputs"));
                var strLMAgent, strLMCampaigns, strLMSkillGroup, strLMDateOptionSelected, strLMStartDate, strLMEndDate
                $.each(data, function () {
                    strLMAgent = this['Agent'];
                    strLMCampaigns = this['Campaigns'];
                    strLMSkillGroup = this['SkillGroup'];
                    strLMDateOptionSelected = this['DateOptionSelected'];
                    strLMStartDate = this['StartDate'];
                    strLMEndDate = this['EndDate'];
                });
                $('#MainContent_lstCarrierMixAgents').val(strLMAgent).attr("selected", "selected");
                $('#MainContent_lstCarrierMixCampaign').val(strLMCampaigns).attr("selected", "selected");
                $('#MainContent_lstCarrierMixSkillGroup').val(strLMSkillGroup).attr("selected", "selected");
                $('#MainContent_lstCarrierMixCalendar').val(strLMDateOptionSelected).attr("selected", "selected");

                if (strLMDateOptionSelected <= 6) {
                    var strReturnedDates = GetDateSelected(strLMDateOptionSelected);
                    loadCarrierMixReportData(strLMAgent, strLMCampaigns, strLMSkillGroup, strReturnedDates[0], strReturnedDates[1]);
                } else {
                    // == 7 
                    $("#txtCarrierMixStartDate").val(strLMStartDate);
                    $("#txtCarrierMixEndDate").val(strLMEndDate);
                    $('#txtCarrierMixStartDate').show();
                    $('#txtCarrierMixEndDate').show();
                    if (strLMStartDate && strLMEndDate) {
                        loadCarrierMixReportData(strLMAgent, strLMCampaigns, strLMSkillGroup, strLMStartDate, strLMEndDate);
                    }
                }
            }
        } else {
            g_ParmShowHide[0] = false;
        }
    });

    function myFunction() {

    }

    $('#CarrierMixRefresh').click(function () {
        var strAgent = $("#MainContent_lstCarrierMixAgents").val();
        var strSkillGroup = $("#MainContent_lstCarrierMixSkillGroup").val();
        var strCampaign = $("#MainContent_lstCarrierMixCampaign").val();
        var strDateSelected = $("#MainContent_lstCarrierMixCalendar").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadCarrierMixReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtCarrierMixStartDate").val())
            var isEndDateGood = isDate($("#txtCarrierMixEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadCarrierMixReportData(strAgent, strSkillGroup, strCampaign, $("#txtCarrierMixStartDate").val(), $("#txtCarrierMixEndDate").val());
            }
        }

    });

    $('#MainContent_lstCarrierMixCalendar').click(function () {
        var calId = $('#MainContent_lstCarrierMixCalendar').val();

        if (calId == 7) {
            $('#txtCarrierMixStartDate').show();
            $('#txtCarrierMixEndDate').show();
        } else {
            $('#txtCarrierMixStartDate').hide();
            $('#txtCarrierMixEndDate').hide();
        }
    });

    function hideshowCarrierMixReport() {

        if ($('#CarrierMixReport').is(':hidden')) {
            $('#topCarrierMix').removeClass("reportHeaderBar");
            $('#topCarrierMix').addClass("reportHeaderBarActive");
            $('#CarrierMixParms').show();
            $('#CarrierMixReport').slideDown("slow");
        }
        else {
            $('#topCarrierMix').removeClass("reportHeaderBarActive");
            $('#topCarrierMix').addClass("reportHeaderBar");
            $('#CarrierMixReport').slideUp("slow");
            $('#CarrierMixParms').hide();
        }
    }

    function loadCarrierMixReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        var localData = "";
        //$('#CarrierMixReport').fadeTo(500, 0.2);
        $('#CarrierMixReportingTable').hide();
        $('#IML3').show();
        // Store Parameters in Cookie
        var DatePickerID = $('#MainContent_lstCarrierMixCalendar').val();
        var carrierMixInputs = [
            { 'Agent': strAgent, 'Campaigns': strCampaign, 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': DatePickerID }
        ];
        $.cookie("carrierMixInputs", JSON.stringify(carrierMixInputs), { expires: 30 });

        $("#CarrierMixReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetCarrierMix",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgent", "value": strAgent });
                aoData.push({ "name": "strSkillGroup", "value": strSkillGroup });
                aoData.push({ "name": "strCampaign", "value": strCampaign });
                aoData.push({ "name": "strStartDate", "value": strStartDate });
                aoData.push({ "name": "strEndDate", "value": strEndDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    //"data": JSON.stringify({ UserID: "1" }),
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#CarrierMixReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iMST = 0;
                var iMTP = 0;
                var iMAT = 0;
                var iMAP = 0;
                var iPDT = 0;
                var iPDP = 0;
                var iDVT = 0;
                var iDVP = 0;
                var iTotal = 0;
                var iPercent = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iMST += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iMTP += parseFloat(Number(parseFloat(aoData[i][2].replace(/[^0-9\.]+/g, ""))));
                    iMAT += parseInt(Number(parseInt(aoData[i][3].replace(/[^0-9\.]+/g, ""))));
                    iMAP += parseFloat(Number(parseFloat(aoData[i][4].replace(/[^0-9\.]+/g, ""))));
                    iPDT += parseInt(Number(parseInt(aoData[i][5].replace(/[^0-9\.]+/g, ""))));
                    iPDP += parseFloat(Number(parseFloat(aoData[i][6].replace(/[^0-9\.]+/g, ""))));
                    iDVT += parseInt(Number(parseInt(aoData[i][7].replace(/[^0-9\.]+/g, ""))));
                    iDVP += parseFloat(Number(parseFloat(aoData[i][8].replace(/[^0-9\.]+/g, ""))));
                    iTotal += parseInt(Number(parseInt(aoData[i][9].replace(/[^0-9\.]+/g, ""))));
                    iPercent += parseFloat(Number(parseFloat(aoData[i][10].replace(/[^0-9\.]+/g, ""))));
                    
                }

                nCells[1].innerHTML = FormatNumber(iMST);
                nCells[2].innerHTML = iMTP.toFixed(2) + " %";
                nCells[3].innerHTML = FormatNumber(iMAT);
                nCells[4].innerHTML = iMAP.toFixed(2) + " %";
                nCells[5].innerHTML = FormatNumber(iPDT);
                nCells[6].innerHTML = iPDP.toFixed(2) + " %";
                nCells[7].innerHTML = FormatNumber(iDVT);
                nCells[8].innerHTML = iDVP.toFixed(2) + " %";
                nCells[9].innerHTML = FormatNumber(iTotal);
                nCells[10].innerHTML = iPercent.toFixed(2) + " %";

                //$('#CarrierMixReport').fadeTo(100, 1.0);
                $('#IML3').hide();
                $('#CarrierMixReportingTable').show();
            }
        });
    }

    // ----- Case Specialist REPORTING


    $('#topCaseSpecialist').click(function () {
        $('#MainContent_lstCaseSpecialistSkillGroup').on('click', function () {
            g_ParmShowHide[3] = true;
        });
        $('#MainContent_lstCaseSpecialistCalendar').on('click', function () {
            g_ParmShowHide[3] = true;
        });
        $('#txtCaseSpecialistStartDate').on('click', function () {
            g_ParmShowHide[3] = true;
        });
        $('#txtCaseSpecialistEndDate').on('click', function () {
            g_ParmShowHide[3] = true;
        });
        $('#CaseSpecialistRefresh').on('click', function () {
            g_ParmShowHide[3] = true;
        });

        if (g_ParmShowHide[3] == false) {
            hideshowCaseSpecialistReport();
            if (g_ShowHide[7] == true) {
                window.g_ShowHide[7] = false;
                var dataLead = JSON.parse($.cookie("caseSpecialistInputs"));
                var strSkillGroup, strDateOptionSelected, strStartDate, strEndDate
                $.each(dataLead, function () {
                    strSkillGroup = this['SkillGroup'];
                    strDateOptionSelected = this['DateOptionSelected'];
                    strStartDate = this['StartDate'];
                    strEndDate = this['EndDate'];
                });
                $('#MainContent_lstCaseSpecialistSkillGroup').val(strSkillGroup).attr("selected", "selected");
                $('#MainContent_lstCaseSpecialistCalendar').val(strDateOptionSelected).attr("selected", "selected");

                if (strDateOptionSelected <= 6) {
                    var strReturnedDates = GetDateSelected(strDateOptionSelected);
                    loadCaseSpecialistReportData(strSkillGroup, strReturnedDates[0], strReturnedDates[1]);
                } else {
                    // == 7 
                    $("#txtCaseSpecialistStartDate").val(strStartDate);
                    $("#txtCaseSpecialistEndDate").val(strEndDate);
                    $('#txtCaseSpecialistStartDate').show();
                    $('#txtCaseSpecialistEndDate').show();
                    if (strStartDate && strEndDate) {
                        loadCaseSpecialistReportData(strSkillGroup, strStartDate, strEndDate);
                    }
                }
            }
        } else {
            g_ParmShowHide[3] = false;
        }
    });

    $('#CaseSpecialistRefresh').click(function () {
        var strSkillGroup = $("#MainContent_lstCaseSpecialistSkillGroup").val();
        var strDateSelected = $("#MainContent_lstCaseSpecialistCalendar").val();
        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadCaseSpecialistReportData(strSkillGroup, strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtCaseSpecialistStartDate").val())
            var isEndDateGood = isDate($("#txtCaseSpecialistEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadCaseSpecialistReportData(strSkillGroup, $("#txtCaseSpecialistStartDate").val(), $("#txtCaseSpecialistEndDate").val());
            }
        }
    });

    $('#MainContent_lstCaseSpecialistCalendar').click(function () {
        var calId = $('#MainContent_lstCaseSpecialistCalendar').val();

        if (calId == 7) {
            $('#txtCaseSpecialistStartDate').show();
            $('#txtCaseSpecialistEndDate').show();
        } else {
            $('#txtCaseSpecialistStartDate').hide();
            $('#txtCaseSpecialistEndDate').hide();
        }
    });

    function hideshowCaseSpecialistReport() {
        if ($('#CaseSpecialistReport').is(':hidden')) {
            $('#topCaseSpecialist').removeClass("reportHeaderBar");
            $('#topCaseSpecialist').addClass("reportHeaderBarActive");
            $('#CaseSpecialistParms').show();
            $('#CaseSpecialistReport').slideDown("slow");
        }
        else {
            $('#topCaseSpecialist').removeClass("reportHeaderBarActive");
            $('#topCaseSpecialist').addClass("reportHeaderBar");
            $('#CaseSpecialistReport').slideUp("slow");
            $('#CaseSpecialistParms').hide();
        }
    }

    function loadCaseSpecialistReportData(strSkillGroup, strStartDate, strEndDate) {
        $('#CaseSpecialistReportingTable').hide();
        $('#IML4').show();

        // Store Parameters in Cookie
        var DatePickerID = $('#MainContent_lstCaseSpecialistCalendar').val();
        var caseSpecialistInputs = [
            { 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': DatePickerID }
        ];
        $.cookie("caseSpecialistInputs", JSON.stringify(caseSpecialistInputs), { expires: 30 });
        $("#CaseSpecialistReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetCaseSpecialist",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strSkillGroup", "value": strSkillGroup });
                aoData.push({ "name": "strStartDate", "value": strStartDate });
                aoData.push({ "name": "strEndDate", "value": strEndDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    //"data": JSON.stringify({ UserID: "1" }),
                    "data": aoData,
                    "success": function (msg) {
                        var json = jQuery.parseJSON(msg.d);
                        fnCallback(json);
                        $("#CaseSpecialistReportingTable").show();
                    }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iSubmittied = 0;
                var iPending = 0;
                var iApproved = 0;
                var iDeclined = 0;
                var iWithdrawn = 0;
                var iNPA = 0;
                var iSpeed = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iSubmittied += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iPending += parseInt(Number(parseInt(aoData[i][2].replace(/[^0-9\.]+/g, ""))));
                    iApproved += parseFloat(Number(parseFloat(aoData[i][3].replace(/[^0-9\.]+/g, ""))));
                    iDeclined += parseFloat(Number(parseFloat(aoData[i][4].replace(/[^0-9\.]+/g, ""))));
                    iWithdrawn += parseInt(Number(parseInt(aoData[i][5].replace(/[^0-9\.]+/g, ""))));
                    iNPA += parseInt(Number(parseInt(aoData[i][6].replace(/[^0-9\.]+/g, ""))));
                    iSpeed += parseInt(Number(parseInt(aoData[i][7].replace(/[^0-9\.]+/g, ""))));
                }

                nCells[1].innerHTML = FormatNumber(iSubmittied);
                nCells[2].innerHTML = FormatNumber(iPending);
                nCells[3].innerHTML = FormatNumber(iApproved);
                nCells[4].innerHTML = FormatNumber(iDeclined);
                nCells[5].innerHTML = FormatNumber(iWithdrawn);
                nCells[6].innerHTML = FormatNumber(iNPA);
                nCells[7].innerHTML = FormatNumber(iSpeed);

                $('#IML4').hide();
                $('#CaseSpecialistReportingTable').show();
            }
        });

    }

    // ----- Fill Form REPORTING


    function onStartShowHideFillFormReport(g_ShowHide) {
        if (g_ShowHide[11] == true) {
            window.g_ShowHide[11] = false;
            hideshowFillFormReport();
        }
    }

    $('#topFillForm').click(function () {
        $('#MainContent_lstFillFormAgents').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#MainContent_lstFillFormSkillGroup').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#MainContent_lstFillFormCampaign').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#MainContent_lstFillFormCalendar').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#txtFillFormStartDate').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#txtFillFormEndDate').on('click', function () {
            g_ParmShowHide[4] = true;
        });
        $('#FillFormRefresh').on('click', function () {
            g_ParmShowHide[4] = true;
        });

        if (g_ParmShowHide[4] == false) {
            hideshowFillFormReport();
            if (g_ShowHide[11] == true) {
                g_ShowHide[11] = false;
                var dataLead = JSON.parse($.cookie("fillFormSpeedInputs"));
                var strAgent, strCampaign, strSkillGroup, strDateOptionSelected, strStartDate, strEndDate
                $.each(dataLead, function () {
                    strAgent = this['Agent'];
                    strCampaign = this['Campaigns'];
                    strSkillGroup = this['SkillGroup'];
                    strDateOptionSelected = this['DateOptionSelected'];
                    strStartDate = this['StartDate'];
                    strEndDate = this['EndDate'];
                });
                $('#MainContent_lstFillFormAgents').val(strAgent).attr("selected", "selected");
                $('#MainContent_lstFillFormCampaign').val(strCampaign).attr("selected", "selected");
                $('#MainContent_lstFillFormSkillGroup').val(strSkillGroup).attr("selected", "selected");
                $('#MainContent_lstFillFormCalendar').val(strDateOptionSelected).attr("selected", "selected");

                if (strDateOptionSelected <= 6) {
                    var strReturnedDates = GetDateSelected(strDateOptionSelected);
                    loadFillFormReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
                } else {
                    // == 7 
                    $("#txtFillFormStartDate").val(strStartDate);
                    $("#txtFillFormEndDate").val(strEndDate);
                    $('#txtFillFormStartDate').show();
                    $('#txtFillFormEndDate').show();
                    if (strStartDate && strEndDate) {
                        loadFillFormReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate)
                    }
                }
            }
        } else {
            g_ParmShowHide[4] = false;
        }
    });

    $('#FillFormRefresh').click(function () {
        var strAgent = $("#MainContent_lstFillFormAgents").val();
        var strSkillGroup = $("#MainContent_lstFillFormSkillGroup").val();
        var strCampaign = $("#MainContent_lstFillFormCampaign").val();
        var strDateSelected = $("#MainContent_lstFillFormCalendar").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadFillFormReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtFillFormStartDate").val())
            var isEndDateGood = isDate($("#txtFillFormEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadFillFormReportData(strAgent, strSkillGroup, strCampaign, $("#txtFillFormStartDate").val(), $("#txtFillFormEndDate").val());
            }
        }
    });

    $('#MainContent_lstFillFormCalendar').click(function () {
        var calId = $('#MainContent_lstFillFormCalendar').val();

        if (calId == 7) {
            $('#txtFillFormStartDate').show();
            $('#txtFillFormEndDate').show();
        } else {
            $('#txtFillFormStartDate').hide();
            $('#txtFillFormEndDate').hide();
        }
    });

    function hideshowFillFormReport() {
        if ($('#FillFormReport').is(':hidden')) {
            $('#topFillForm').removeClass("reportHeaderBar");
            $('#topFillForm').addClass("reportHeaderBarActive");
            $('#FillFormParms').show();
            $('#FillFormReport').slideDown("slow");
        }
        else {
            $('#topFillForm').removeClass("reportHeaderBarActive");
            $('#topFillForm').addClass("reportHeaderBar");
            $('#FillFormReport').slideUp("slow");
            $('#FillFormParms').hide();
        }
    }

    function loadFillFormReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#FillFormReportingTable').hide();
        $('#IML5').show();

        // Store Parameters in Cookie
        var DatePickerID = $('#MainContent_lstFillFormCalendar').val();
        var fillFormSpeedInputs = [
            { 'Agent': strAgent, 'Campaigns': strCampaign, 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': DatePickerID }
        ];
        $.cookie("fillFormSpeedInputs", JSON.stringify(fillFormSpeedInputs), { expires: 30 });

        $("#FillFormReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetFillForm",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgent", "value": strAgent });
                aoData.push({ "name": "strSkillGroup", "value": strSkillGroup });
                aoData.push({ "name": "strCampaign", "value": strCampaign });
                aoData.push({ "name": "strStartDate", "value": strStartDate });
                aoData.push({ "name": "strEndDate", "value": strEndDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    //"data": JSON.stringify({ UserID: "1" }),
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#FillFormReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iFormCount = 0;
                var iMaxMinutes = 0;
                var iMinMinutes = 0;
                var iAvgMinutes = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iFormCount += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iAvgMinutes += parseFloat(Number(parseFloat(aoData[i][2].replace(/[^0-9\.]+/g, ""))));
                    iMinMinutes += parseInt(Number(parseInt(aoData[i][3].replace(/[^0-9\.]+/g, ""))));
                    iMaxMinutes += parseFloat(Number(parseFloat(aoData[i][4].replace(/[^0-9\.]+/g, ""))));
                }

                nCells[1].innerHTML = FormatNumber(iFormCount);
                nCells[2].innerHTML = FormatNumber(iAvgMinutes);
                nCells[3].innerHTML = FormatNumber(iMinMinutes);
                nCells[4].innerHTML = FormatNumber(iMaxMinutes);

                $('#IML5').hide();
                $('#FillFormReportingTable').show();
            }
        });

    }

    // ----- Goal REPORTING


    $('#topGoalReport').click(function () {
        $('#MainContent_lstGoalMonth').on('click', function () {
            g_ParmShowHide[5] = true;
        });
        $('#MainContent_lstGoalYear').on('click', function () {
            g_ParmShowHide[5] = true;
        });
        $('#GoalRefresh').on('click', function () {
            g_ParmShowHide[5] = true;
        });

        if (g_ParmShowHide[5] == false) {
            hideshowGoalReport();
            if (g_ShowHide[6] == true) {
                window.g_ShowHide[6] = false;
                var dataLead = JSON.parse($.cookie("goalInputs"));
                var strMonth, strYear
                $.each(dataLead, function () {
                    strMonth = this['Month'];
                    strYear = this['Year'];
                });
                $('#MainContent_lstGoalMonth').val(strMonth).attr("selected", "selected");
                $('#MainContent_lstGoalYear').val(strYear).attr("selected", "selected");

                var strdays = Math.round(((new Date(strYear, strMonth)) - (new Date(strYear, strMonth - 1))) / 86400000);

                var startDate = new Date();
                if (strMonth == (startDate.getMonth() + 1) && strYear == startDate.getFullYear()) {
                    //alert("1:" + (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
                    loadGoalReportData((startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
                } else {
                    //alert("2:" + strMonth + "/" + strdays + "/" + strYear);
                    loadGoalReportData(strMonth + "/" + strdays + "/" + strYear);
                }
            }
        } else {
            g_ParmShowHide[5] = false;
        }
        
    });

    $('#GoalRefresh').click(function () {
        var strMonth = $("#MainContent_lstGoalMonth").val();
        var strYear = $("#MainContent_lstGoalYear").val();
        var strdays = Math.round(((new Date(strYear, strMonth)) - (new Date(strYear, strMonth - 1))) / 86400000);
        
        var startDate = new Date();
        if (strMonth == (startDate.getMonth() + 1) && strYear == startDate.getFullYear()) {

            loadGoalReportData((startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
        } else {
            loadGoalReportData(strMonth + "/" + strdays + "/" + strYear);
        }
    });

    function hideshowGoalReport() {
        if ($('#GoalReport').is(':hidden')) {
            $('#topGoalReport').removeClass("reportHeaderBar");
            $('#topGoalReport').addClass("reportHeaderBarActive");
            $('#GoalParms').show();
            $('#GoalReport').slideDown("slow");
        }
        else {
            $('#topGoalReport').removeClass("reportHeaderBarActive");
            $('#topGoalReport').addClass("reportHeaderBar");
            $('#GoalReport').slideUp("slow");
            $('#GoalParms').hide();
        }
    }

    function loadGoalReportData(strStartDate) {
        $('GoalReportingParameters').hide();
        $('#IML6').show();

        var MonthID = $('#MainContent_lstGoalMonth').val();
        var YearID = $('#MainContent_lstGoalYear').val();

       // alert(MonthID);
        var goalInputs = [
            { 'Month': MonthID, 'Year': YearID }
        ];
        $.cookie("goalInputs", JSON.stringify(goalInputs), { expires: 30 });

        $("#GoalReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetGoalReport",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strStartDate", "value": strStartDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    //"data": JSON.stringify({ UserID: "1" }),
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    $.each(json.aaRecordCount, function (i, item) {
                                        $('tfoot th#GRP1').html(item.CurrentDate); //Pull in Json Data
                                        $('tfoot th#GRP2').html(item.DaysWorked); //Pull in Json Data
                                        $('tfoot th#GRP3').html(item.TotalWorkDays); //Pull in Json Data
                                    });
                                    fnCallback(json);
                                    $("#GoalReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iQuota = 0;
                var iMTD = 0;
                var iPercentOfQuota = 0;
                var iDailyAverage = 0;
                var iProjected = 0;
                var iVarianceFromProjected = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iQuota += parseInt(aoData[i][1] || 0);
                    iMTD += parseInt(aoData[i][2] || 0);
                    iPercentOfQuota += parseFloat(aoData[i][3] || 0);
                    iDailyAverage += parseFloat(aoData[i][4]);
                    iProjected += parseInt(aoData[i][5] || 0);
                    iVarianceFromProjected += parseInt(aoData[i][6] || 0);
                }

                nCells[1].innerHTML = iQuota;
                nCells[2].innerHTML = iMTD;
                nCells[3].innerHTML = iPercentOfQuota;
                nCells[4].innerHTML = iDailyAverage.toFixed(2);
                nCells[5].innerHTML = iProjected;
                nCells[6].innerHTML = iVarianceFromProjected;

                $('#IML6').hide();
                $('GoalReportingParameters').show();
            }
        });

    }

    // ----- Incentive Tracking REPORTING


    $('#topIncentiveTracking').click(function () {
        $('#MainContent_lstIncentiveTrackingAgents').on('click', function () {
            g_ParmShowHide[6] = true;
        });
        $('#IncentiveTrackingRefresh').on('click', function () {
            g_ParmShowHide[6] = true;
        });

        
        if (g_ParmShowHide[6] == false) {
            hideshowIncentiveTracking();
            if (g_ShowHide[3] == true) {
                window.g_ShowHide[3] = false;
                var dataLead = JSON.parse($.cookie("incentiveTrackingInputs"));
                var strAgent
                $.each(dataLead, function () {
                    strAgent = this['Agent'];
                });
                $('#MainContent_lstIncentiveTrackingAgents').val(strAgent).attr("selected", "selected");
                loadIncentiveTrackingReportData(strAgent);
            }
        } else {
            g_ParmShowHide[6] = false;
        }
    });

    $('#IncentiveTrackingRefresh').click(function () {
        var strAgent = $("#MainContent_lstIncentiveTrackingAgents").val();
        loadIncentiveTrackingReportData(strAgent);
    });

    function hideshowIncentiveTracking() {
        if ($('#IncentiveTrackingReport').is(':hidden')) {
            $('#topIncentiveTracking').removeClass("reportHeaderBar");
            $('#topIncentiveTracking').addClass("reportHeaderBarActive");
            $('#IncentiveTrackingParms').show();
            $('#IncentiveTrackingReport').slideDown("slow");
        }
        else {
            $('#topIncentiveTracking').removeClass("reportHeaderBarActive");
            $('#topIncentiveTracking').addClass("reportHeaderBar");
            $('#IncentiveTrackingReport').slideUp("slow");
            $('#IncentiveTrackingParms').hide();
        }
    }



    function loadIncentiveTrackingReportData(strAgent) {
        $('#IncentiveTrackingReportingTable').hide();
        $('#IML7').show();

        var incentiveTrackingInputs = [
            { 'Agent': strAgent }
        ];
        $.cookie("incentiveTrackingInputs", JSON.stringify(incentiveTrackingInputs), { expires: 30 });

        $("#IncentiveTrackingReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetIncentiveTracking",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgentID", "value": strAgent });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#IncentiveTrackingReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                $('#IML7').hide();
                $('#IncentiveTrackingReportingTable').show();
            }
        });

    }

    // ----- Lead Volume REPORTING


    $('#topLeadVolume').click(function () {
        $('#MainContent_lstLeadVolumeAgents').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#MainContent_lstLeadVolumeSkillGroup').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#MainContent_lstLeadVolumeCampaign').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#MainContent_lstLeadVolumeCalendar').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#txtLeadVolumeStartDate').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#txtLeadVolumeEndDate').on('click', function () {
            g_ParmShowHide[7] = true;
        });
        $('#LeadVolumeRefresh').on('click', function () {
            g_ParmShowHide[7] = true;
        });

        if (g_ParmShowHide[7] == false) {
            hideshowLeadVolume();
            if (g_ShowHide[5] == true) {
                window.g_ShowHide[5] = false;
                var dataLead = JSON.parse($.cookie("leadVolumeInputs"));
                var strAgent, strCampaign, strSkillGroup, strDateOptionSelected, strStartDate, strEndDate
                $.each(dataLead, function () {
                    strAgent = this['Agent'];
                    strCampaign = this['Campaigns'];
                    strSkillGroup = this['SkillGroup'];
                    strDateOptionSelected = this['DateOptionSelected'];
                    strStartDate = this['StartDate'];
                    strEndDate = this['EndDate'];
                });
                $('#MainContent_lstLeadVolumeAgents').val(strAgent).attr("selected", "selected");
                $('#MainContent_lstLeadVolumeCampaign').val(strCampaign).attr("selected", "selected");
                $('#MainContent_lstLeadVolumeSkillGroup').val(strSkillGroup).attr("selected", "selected");
                $('#MainContent_lstLeadVolumeCalendar').val(strDateOptionSelected).attr("selected", "selected");

                if (strDateOptionSelected <= 6) {
                    var strReturnedDates = GetDateSelected(strDateOptionSelected);
                    loadCaseSpecialistReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
                } else {
                    // == 7 
                    $("#txtLeadVolumeStartDate").val(strStartDate);
                    $("#txtLeadVolumeEndDate").val(strEndDate);
                    $('#txtLeadVolumeStartDate').show();
                    $('#txtLeadVolumeEndDate').show();
                    if (strStartDate && strEndDate) {
                        loadCaseSpecialistReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate)
                    }
                }
            }
        } else {
            g_ParmShowHide[7] = false;
        }
    });

    $('#LeadVolumeRefresh').click(function () {
        var strAgent = $("#MainContent_lstLeadVolumeAgents").val();
        var strSkillGroup = $("#MainContent_lstLeadVolumeSkillGroup").val();
        var strCampaign = $("#MainContent_lstLeadVolumeCampaign").val();
        var strDateSelected = $("#MainContent_lstLeadVolumeCalendar").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadLeadVolumeReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtLeadVolumeStartDate").val())
            var isEndDateGood = isDate($("#txtLeadVolumeEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadLeadVolumeReportData(strAgent, strSkillGroup, strCampaign, $("#txtLeadVolumeStartDate").val(), $("#txtLeadVolumeEndDate").val());
            }
        }
    });

    $('#MainContent_lstLeadVolumeCalendar').click(function () {
        var calId = $('#MainContent_lstLeadVolumeCalendar').val();

        if (calId == 7) {
            $('#txtLeadVolumeStartDate').show();
            $('#txtLeadVolumeEndDate').show();
        } else {
            $('#txtLeadVolumeStartDate').hide();
            $('#txtLeadVolumeEndDate').hide();
        }
    });

    function hideshowLeadVolume() {
        if ($('#LeadVolumeReport').is(':hidden')) {
            $('#topLeadVolume').removeClass("reportHeaderBar");
            $('#topLeadVolume').addClass("reportHeaderBarActive");
            $('#LeadVolumeParms').show();
            $('#LeadVolumeReport').slideDown("slow");
        }
        else {
            $('#topLeadVolume').removeClass("reportHeaderBarActive");
            $('#topLeadVolume').addClass("reportHeaderBar");
            $('#LeadVolumeReport').slideUp("slow");
            $('#LeadVolumeParms').hide();
        }
    }

    function loadLeadVolumeReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#LeadVolumeReportingTable').hide();
        $('#IML8').show();
        var DatePickerID = $('#MainContent_lstLeadVolumeCalendar').val();
        var leadVolumeInputs = [
            { 'Agent': strAgent, 'Campaigns': strCampaign, 'SkillGroup': strSkillGroup, 'StartDate': strStartDate, 'EndDate': strEndDate, 'DateOptionSelected': DatePickerID }
        ];
        $.cookie("leadVolumeInputs", JSON.stringify(leadVolumeInputs), { expires: 30 });

        var myTable;
        
        myTable = $("#LeadVolumeReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetLeadVolume",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": 300,
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgent", "value": strAgent });
                aoData.push({ "name": "strSkillGroup", "value": strSkillGroup });
                aoData.push({ "name": "strCampaign", "value": strCampaign });
                aoData.push({ "name": "strStartDate", "value": strStartDate });
                aoData.push({ "name": "strEndDate", "value": strEndDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    
                                    fnCallback(json);
                                    

                                    $("#LeadVolumeReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                
                var nCells = nRow.getElementsByTagName('th');
                var iVolumeTotal = 0;
                

                for (var i = 0 ; i < aoData.length ; i++) {
                    iVolumeTotal += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                }
                
                nCells[1].innerHTML = FormatNumber(iVolumeTotal);
                $('#IML8').hide();
                $('#LeadVolumeReportingTable').show();
            }            
        });

    }


    // ----- PIPELINE REPORTING 


    $('#topPipeline').click(function () {
        $('#MainContent_lstPipelineAgents').on('click', function () {
            g_ParmShowHide[8] = true;
        });
        $('#PipelineRefresh').on('click', function () {
            g_ParmShowHide[8] = true;
        });

        if (g_ParmShowHide[8] == false) {
            hideshowPipeline();
            if (g_ShowHide[2] == true) {
                window.g_ShowHide[2] = false;
                var dataLead = JSON.parse($.cookie("pipelineInputs"));
                var strAgent
                $.each(dataLead, function () {
                    strAgent = this['Agent'];
                });
                $('#MainContent_lstPipelineAgents').val(strAgent).attr("selected", "selected");
                loadPipeLineReportData(strAgent);
            }
        } else {
            g_ParmShowHide[8] = false;
        }
    });

    $('#PipelineRefresh').click(function () {
        var strAgents = $("#MainContent_lstPipelineAgents").val();
        loadPipeLineReportData(strAgents);
    });

    function hideshowPipeline() {
        if ($('#PiplineReport').is(':hidden')) {
            $('#topPipeline').removeClass("reportHeaderBar");
            $('#topPipeline').addClass("reportHeaderBarActive");
            $('#PipelineParms').show();
            $('#PiplineReport').slideDown("slow");
        }
        else {
            $('#topPipeline').removeClass("reportHeaderBarActive");
            $('#topPipeline').addClass("reportHeaderBar");
            $('#PiplineReport').slideUp("slow");
            $('#PipelineParms').hide();
        }
    }


    function loadPipeLineReportData(strAgentID) {
        $('#PipelineReportingTable').hide();
        $('#IML9').show();

        var pipelineInputs = [
            { 'Agent': strAgentID }
        ];
        $.cookie("pipelineInputs", JSON.stringify(pipelineInputs), { expires: 30 });
        $("#PipelineReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetPipelineReport",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgentID", "value": strAgentID });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    //"data": JSON.stringify({ UserID: "1" }),
                    "data": aoData,
                    "success": function (msg) {
                        var json = jQuery.parseJSON(msg.d);
                        fnCallback(json);
                        $("#PipelineReportingTable").show();
                    }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                $('#IML9').hide();
                $('#PipelineReportingTable').show();
            }
        });
    }

    // ----- Prioritized List Report


    $('#topPrioritizedList').click(function () {
        $('#MainContent_lstPrioritizedListAgents').on('click', function () {
            g_ParmShowHide[9] = true;
        });
        $('#PrioritizedListRefresh').on('click', function () {
            g_ParmShowHide[9] = true;
        });

        if (g_ParmShowHide[9] == false) {
            hideshowPrioritizedList();
            if (g_ShowHide[14] == true) {
                window.g_ShowHide[14] = false;
                var dataLead = JSON.parse($.cookie("prioritizedListInputs"));
                var strAgent
                $.each(dataLead, function () {
                    strAgent = this['Agent'];
                });
                $('#MainContent_lstPrioritizedListAgents').val(strAgent).attr("selected", "selected");


                loadPrioritizedListReportData(strAgent);
            }
        } else {
            g_ParmShowHide[9] = false;
        }
    });

    $('#PrioritizedListRefresh').click(function () {
        var strAgent = $("#MainContent_lstPrioritizedListAgents").val();
        loadPrioritizedListReportData(strAgent);
    });

    function hideshowPrioritizedList() {
        if ($('#PrioritizedListReport').is(':hidden')) {
            $('#topPrioritizedList').removeClass("reportHeaderBar");
            $('#topPrioritizedList').addClass("reportHeaderBarActive");
            $('#PrioritizedListParms').show();
            $('#PrioritizedListReport').slideDown("slow");
        }
        else {
            $('#topPrioritizedList').removeClass("reportHeaderBarActive");
            $('#topPrioritizedList').addClass("reportHeaderBar");
            $('#PrioritizedListReport').slideUp("slow");
            $('#PrioritizedListParms').hide();
        }
    }

    function loadPrioritizedListReportData(strAgent) {
        $('#PrioritizedListReportingTable').hide();
        $('#IML10').show();

        var prioritizedListInputs = [
           { 'Agent': strAgent }
        ];
        $.cookie("prioritizedListInputs", JSON.stringify(prioritizedListInputs), { expires: 30 });

        $("#PrioritizedListReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetPrioritizedLeads",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strAgent", "value": strAgent });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#PrioritizedListReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                $('#IML10').hide();
                $('#PrioritizedListReportingTable').show();
            }
        });

    }

    // ----- Quota Tracking REPORTING


    $('#topQuotaTracking').click(function () {
        $('#MainContent_lstQuotaTrackingMonth').on('click', function () {
            g_ParmShowHide[10] = true;
        });
        $('#MainContent_lstQuotaTrackingYear').on('click', function () {
            g_ParmShowHide[10] = true;
        });
        $('#QuotaTrackingRefresh').on('click', function () {
            g_ParmShowHide[10] = true;
        });

        if (g_ParmShowHide[10] == false) {
            hideshowQuotaTracking();
            if (g_ShowHide[0] == true) {
                window.g_ShowHide[0] = false;
                var dataLead = JSON.parse($.cookie("quotaTrackingInputs"));
                var strMonth, strYear
                $.each(dataLead, function () {
                    strMonth = this['Month'];
                    strYear = this['Year'];
                });
                $('#MainContent_lstQuotaTrackingMonth').val(strMonth).attr("selected", "selected");
                $('#MainContent_lstQuotaTrackingYear').val(strYear).attr("selected", "selected");

                var strdays = Math.round(((new Date(strYear, strMonth)) - (new Date(strYear, strMonth - 1))) / 86400000);

                var startDate = new Date();
                if (strMonth == (startDate.getMonth() + 1) && strYear == startDate.getFullYear()) {
                    //alert("1:" + (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
                    loadQuotaTrackingReportData((startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
                } else {
                    //alert("2:" + strMonth + "/" + strdays + "/" + strYear);
                    loadQuotaTrackingReportData(strMonth + "/" + strdays + "/" + strYear);
                }
            }
        } else {
            g_ParmShowHide[10] = false;
        } 
    });

    $('#QuotaTrackingRefresh').click(function () {
        var strMonth = $("#MainContent_lstQuotaTrackingMonth").val();
        var strYear = $("#MainContent_lstQuotaTrackingYear").val();
        var strdays = Math.round(((new Date(strYear, strMonth)) - (new Date(strYear, strMonth - 1))) / 86400000);

        var startDate = new Date();
        if (strMonth == (startDate.getMonth() + 1) && strYear == startDate.getFullYear()) {

            loadQuotaTrackingReportData((startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
        } else {
            loadQuotaTrackingReportData(strMonth + "/" + strdays + "/" + strYear);
        }

    });

    function hideshowQuotaTracking() {
        if ($('#QuotaTrackingReport').is(':hidden')) {
            $('#topQuotaTracking').removeClass("reportHeaderBar");
            $('#topQuotaTracking').addClass("reportHeaderBarActive");
            $('#QuotaTrackingParms').show();
            $('#QuotaTrackingReport').slideDown("slow");
        }
        else {
            $('#topQuotaTracking').removeClass("reportHeaderBarActive");
            $('#topQuotaTracking').addClass("reportHeaderBar");
            $('#QuotaTrackingReport').slideUp("slow");
            $('#QuotaTrackingParms').hide();
        }
    }

    function loadQuotaTrackingReportData(strStartDate) {
        $('#QuotaTrackingReportingParameters').hide();
        $('#IML11').show();

        var strMonth = $("#MainContent_lstQuotaTrackingMonth").val();
        var strYear = $("#MainContent_lstQuotaTrackingYear").val();

        var quotaTrackingInputs = [
            { 'Month': strMonth, 'Year': strYear }
        ];
        $.cookie("quotaTrackingInputs", JSON.stringify(quotaTrackingInputs), { expires: 30 });

        $("#QuotaTrackingReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetQuotaTracking",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strStartDate", "value": strStartDate });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    $.each(json.aaRecordCount, function (i, item) {
                                        $('tfoot th#QTRP1').html(item.CurrentDate); //Pull in Json Data
                                        $('tfoot th#QTRP2').html(item.DaysWorked); //Pull in Json Data
                                        $('tfoot th#QTRP3').html(item.TotalWorkDays); //Pull in Json Data
                                    });
                                    fnCallback(json);
                                    $("#QuotaTrackingReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iQuota = 0;
                var iMTD = 0;
                var iPercentOfQuota = 0;
                var iDailyAverage = 0;
                var iProjected = 0;
                var iVarianceFromProjected = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iQuota += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iMTD += parseInt(Number(parseInt(aoData[i][2].replace(/[^0-9\.]+/g, ""))));
                    iPercentOfQuota += parseFloat(Number(parseFloat(aoData[i][3].replace(/[^0-9\.]+/g, ""))));
                    iDailyAverage += parseFloat(Number(parseFloat(aoData[i][4].replace(/[^0-9\.]+/g, ""))));
                    iProjected += parseInt(Number(parseInt(aoData[i][5].replace(/[^0-9\.]+/g, ""))));
                    iVarianceFromProjected += parseInt(Number(parseInt(aoData[i][6].replace(/[^0-9\.]+/g, ""))));
                }

                nCells[1].innerHTML = iQuota;
                nCells[2].innerHTML = iMTD;
                nCells[3].innerHTML = iPercentOfQuota.toFixed(2);
                nCells[4].innerHTML = iDailyAverage.toFixed(2);
                nCells[5].innerHTML = iProjected;
                nCells[6].innerHTML = iVarianceFromProjected;

                $('#IML11').hide();
                $('#QuotaTrackingReportingParameters').show();
            }
        });

    }

    // ----- Submissions & Enrollments REPORTING


    $('#topSubmissionEnroll').click(function () {
        $('#MainContent_lstSubmissionEnrollCalendar').on('click', function () {
            g_ParmShowHide[11] = true;
        });
        $('#SubmissionEnrollRefresh').on('click', function () {
            g_ParmShowHide[11] = true;
        });

        if (g_ParmShowHide[11] == false) {
            hideshowSubmissionEnrollReport();
            if (g_ShowHide[8] == true) {
                window.g_ShowHide[8] = false;
                var dataLead = JSON.parse($.cookie("submissionEnrolledInputs"));
                var strYear
                $.each(dataLead, function () {
                    strYear = this['Year'];
                });
                $('#MainContent_lstSubmissionEnrollCalendar').val(strYear).attr("selected", "selected");
                loadSubmissionsEnrollmentsReportData(strYear);
            }
        } else {
            g_ParmShowHide[11] = false;
        }
    });

    $('#SubmissionEnrollRefresh').click(function () {
        var strYear = $("#MainContent_lstSubmissionEnrollCalendar").val();
        loadSubmissionsEnrollmentsReportData(strYear);
    });

    function hideshowSubmissionEnrollReport() {
        if ($('#SubmissionEnrollReport').is(':hidden')) {
            $('#topSubmissionEnroll').removeClass("reportHeaderBar");
            $('#topSubmissionEnroll').addClass("reportHeaderBarActive");
            $('#SubmissionEnrollParms').show();
            $('#SubmissionEnrollReport').slideDown("slow");
        }
        else {
            $('#topSubmissionEnroll').addClass("reportHeaderBar");
            $('#topSubmissionEnroll').removeClass("reportHeaderBarActive");
            $('#SubmissionEnrollReport').slideUp("slow");
            $('#SubmissionEnrollParms').hide();
        }
    }

    function loadSubmissionsEnrollmentsReportData(strYear) {
        $('#SubmissionsEnrollmentsReportingTable').hide();
        $('#IML12').show();

        var submissionEnrolledInputs = [
            { 'Year': strYear }
        ];
        $.cookie("submissionEnrolledInputs", JSON.stringify(submissionEnrolledInputs), { expires: 30 });
        $("#SubmissionsEnrollmentsReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": false,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetSubmissionsEnrollments",
            "bDeferRender": true,
            "bScrollCollapse": true,
            //"sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strYear", "value": strYear });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#SubmissionsEnrollmentsReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iJan = 0;
                var iFeb = 0;
                var iMar = 0;
                var iApr = 0;
                var iMay = 0;
                var iJun = 0;
                var iJul = 0;
                var iAug = 0;
                var iSep = 0;
                var iOct = 0;
                var iNov = 0;
                var iDec = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iJan += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iFeb += parseInt(Number(parseInt(aoData[i][2].replace(/[^0-9\.]+/g, ""))));
                    iMar += parseInt(Number(parseInt(aoData[i][3].replace(/[^0-9\.]+/g, ""))));
                    iApr += parseInt(Number(parseInt(aoData[i][4].replace(/[^0-9\.]+/g, ""))));
                    iMay += parseInt(Number(parseInt(aoData[i][5].replace(/[^0-9\.]+/g, ""))));
                    iJun += parseInt(Number(parseInt(aoData[i][6].replace(/[^0-9\.]+/g, ""))));
                    iJul += parseInt(Number(parseInt(aoData[i][7].replace(/[^0-9\.]+/g, ""))));
                    iAug += parseInt(Number(parseInt(aoData[i][8].replace(/[^0-9\.]+/g, ""))));
                    iSep += parseInt(Number(parseInt(aoData[i][9].replace(/[^0-9\.]+/g, ""))));
                    iOct += parseInt(Number(parseInt(aoData[i][10].replace(/[^0-9\.]+/g, ""))));
                    iNov += parseInt(Number(parseInt(aoData[i][11].replace(/[^0-9\.]+/g, ""))));
                    iDec += parseInt(Number(parseInt(aoData[i][12].replace(/[^0-9\.]+/g, ""))));
                }

                nCells[1].innerHTML = FormatNumber(iJan);
                nCells[2].innerHTML = FormatNumber(iFeb);
                nCells[3].innerHTML = FormatNumber(iMar);
                nCells[4].innerHTML = FormatNumber(iApr);
                nCells[5].innerHTML = FormatNumber(iMay);
                nCells[6].innerHTML = FormatNumber(iJun);
                nCells[7].innerHTML = FormatNumber(iJul);
                nCells[8].innerHTML = FormatNumber(iAug);
                nCells[9].innerHTML = FormatNumber(iSep);
                nCells[10].innerHTML = FormatNumber(iOct);
                nCells[11].innerHTML = FormatNumber(iNov);
                nCells[12].innerHTML = FormatNumber(iDec);

                $('#IML12').hide();
                $('#SubmissionsEnrollmentsReportingTable').show();
            }
        });

    }
    

    // FUNCTIONS USED WITHIN THIS PAGE
    // Number Formatter
    function FormatNumber(yourNumber) {
        var n = yourNumber.toString().split(".");
        n[0] = n[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return n.join(".");
    }

    // Date Checker
    function isDate(txtDate) {
        var currVal = txtDate;
        if (currVal == '')
            return false;

        //Declare Regex  
        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
        var dtArray = currVal.match(rxDatePattern); // is format OK?

        if (dtArray == null)
            return false;

        //Checks for mm/dd/yyyy format.
        dtMonth = dtArray[1];
        dtDay = dtArray[3];
        dtYear = dtArray[5];

        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    }

    // Function to get the correct Date Selected
    function GetDateSelected(SelectedDate) {
        var varStartDate = "";
        var varEndDate = "";
        switch (SelectedDate) {
            case '0':
                varStartDate = g_StartDate[0];
                varEndDate = g_StartDate[1];
                break;
            case '1':
                var startDate = new Date();
                startDate.setDate(startDate.getDate() - 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                break;
            case '2':
                var startDate = new Date();
                var EndDate = new Date();
                var startDay = new Date().getDay();  //0=Sun, 1=Mon, ..., 6=Sat
                startDate.setDate(startDate.getDate() - startDay);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
                break;
            case '3':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate((startDate.getDate() - startDate.getDate()) + 1);
                EndDate.setDate(EndDate.getDate() + 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
                break;
            case '4':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 7);
                EndDate.setDate(EndDate.getDate() + 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
                break;
            case '5':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 14);
                EndDate.setDate(EndDate.getDate() + 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
                break;
            case '6':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 30);
                EndDate.setDate(EndDate.getDate() + 1); 
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear();
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
                break;
        }

        return [varStartDate, varEndDate];
    }

    // Set Date Picker TextBoxes
    function SetupDatePickerTextBoxes() {
        $('#txtSalesMetricsStartDate').val(g_StartDate[0]);
        $('#txtSalesMetricsEndDate').val(g_StartDate[1]);
        $('#txtSalesMetricsStartDate').datepicker();
        $('#txtSalesMetricsEndDate').datepicker();
        
        $('#txtLeadMetricsStartDate').val(g_StartDate[0]);
        $('#txtLeadMetricsEndDate').val(g_StartDate[1]);
        $('#txtLeadMetricsStartDate').datepicker();
        $('#txtLeadMetricsEndDate').datepicker();
        //score card datepickers
        $('#txtScoreCardStartDate').val(g_StartDate[0]);
        $('#txtScoreCardEndDate').val(g_StartDate[1]);
        $('#txtScoreCardStartDate').datepicker();
        $('#txtScoreCardEndDate').datepicker();
        //CPA datepickers
        $('#txtCpaStartDate').val(g_StartDate[0]);
        $('#txtCpaEndDate').val(g_StartDate[1]);
        $('#txtCpaStartDate').datepicker();
        $('#txtCpaEndDate').datepicker();

        //Lead Volume datepickers
        $('#txtLeadVolumeStartDate').val(g_StartDate[0]);
        $('#txtLeadVolumeEndDate').val(g_StartDate[1]);
        $('#txtLeadVolumeStartDate').datepicker();
        $('#txtLeadVolumeEndDate').datepicker();

        //Lead Volume datepickers
        $('#txtCaseSpecialistStartDate').val(g_StartDate[0]);
        $('#txtCaseSpecialistEndDate').val(g_StartDate[1]);
        $('#txtCaseSpecialistStartDate').datepicker();
        $('#txtCaseSpecialistEndDate').datepicker();

        //Carrier Mix datepickers
        $('#txtCarrierMixStartDate').val(g_StartDate[0]);
        $('#txtCarrierMixEndDate').val(g_StartDate[1]);
        $('#txtCarrierMixStartDate').datepicker();
        $('#txtCarrierMixEndDate').datepicker();

        //Fill Form datepickers
        $('#txtFillFormStartDate').val(g_StartDate[0]);
        $('#txtFillFormEndDate').val(g_StartDate[1]);
        $('#txtFillFormStartDate').datepicker();
        $('#txtFillFormEndDate').datepicker();

        //Fall Out datepickers
        $('#txtFillFormStartDate').val(g_StartDate[0]);
        $('#txtFillFormEndDate').val(g_StartDate[1]);
        $('#txtFillFormStartDate').datepicker();
        $('#txtFillFormEndDate').datepicker();

        //Stack Ranking datepickers
        $('#txtStackedRankingStartDate').val(g_StartDate[0]);
        $('#txtStackedRankingEndDate').val(g_StartDate[1]);
        $('#txtStackedRankingStartDate').datepicker();
        $('#txtStackedRankingEndDate').datepicker();
    }

    // Start Calendar Dates 
    function CalendarTodayStartEndDates() {
        var StartDate = new Date();
        var EndDate = new Date();
        EndDate.setDate(EndDate.getDate() + 1);
        g_StartDate[0] = (StartDate.getMonth() + 1) + '/' + StartDate.getDate() + '/' + StartDate.getFullYear();
        g_StartDate[1] = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear();
    }

    //Creates the Cookies for the Page
    function CookieCreator() {
        // LeadsMetrics
        if ($.cookie('leadMetricsInputs') === null ||
            $.cookie('leadMetricsInputs') === "" ||
            $.cookie('leadMetricsInputs') === "null" ||
            $.cookie('leadMetricsInputs') === undefined)
        {
            var leadMetricsInputs = [
            { 'Agent': "0", 'Campaigns': "0", 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected':"0" }
            ];
            $.cookie("leadMetricsInputs", JSON.stringify(leadMetricsInputs), { expires: 30 });
            
        }

        // SalesMetrics
        if ($.cookie('salesMetricsInputs') === null ||
            $.cookie('salesMetricsInputs') === "" ||
            $.cookie('salesMetricsInputs') === "null" ||
            $.cookie('salesMetricsInputs') === undefined) {
            var salesMetricsInputs = [
            { 'Agent': "0", 'Campaigns': "0", 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("salesMetricsInputs", JSON.stringify(salesMetricsInputs), { expires: 30 });

        }

        // Score Card
        if ($.cookie('scoreCardInputs') === null ||
            $.cookie('scoreCardInputs') === "" ||
            $.cookie('scoreCardInputs') === "null" ||
            $.cookie('scoreCardInputs') === undefined) {
            var scoreCardInputs = [
            { 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("scoreCardInputs", JSON.stringify(scoreCardInputs), { expires: 30 });

        }

        // Carrier Mix
        if ($.cookie('carrierMixInputs') === null ||
            $.cookie('carrierMixInputs') === "" ||
            $.cookie('carrierMixInputs') === "null" ||
            $.cookie('carrierMixInputs') === undefined) {
            var carrierMixInputs = [
            { 'Agent': "0", 'Campaigns': "0", 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("carrierMixInputs", JSON.stringify(carrierMixInputs), { expires: 30 });

        }

        // Case Specialist
        if ($.cookie('caseSpecialistInputs') === null ||
            $.cookie('caseSpecialistInputs') === "" ||
            $.cookie('caseSpecialistInputs') === "null" ||
            $.cookie('caseSpecialistInputs') === undefined) {
            var caseSpecialistInputs = [
            { 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("caseSpecialistInputs", JSON.stringify(caseSpecialistInputs), { expires: 30 });

        }

        // Fill Form Speed
        if ($.cookie('fillFormSpeedInputs') === null ||
            $.cookie('fillFormSpeedInputs') === "" ||
            $.cookie('fillFormSpeedInputs') === "null" ||
            $.cookie('fillFormSpeedInputs') === undefined) {
            var fillFormSpeedInputs = [
            { 'Agent': "0", 'Campaigns': "0", 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("fillFormSpeedInputs", JSON.stringify(fillFormSpeedInputs), { expires: 30 });

        }

        // Goal
        if ($.cookie('goalInputs') === null ||
            $.cookie('goalInputs') === "" ||
            $.cookie('goalInputs') === "null" ||
            $.cookie('goalInputs') === undefined) {
            var StartDate = new Date();
            var goalInputs = [
            { 'Month': (StartDate.getMonth() + 1), 'Year': StartDate.getFullYear() }
            ];
            $.cookie("goalInputs", JSON.stringify(goalInputs), { expires: 30 });

        }

        // Incentive Tracking
        if ($.cookie('incentiveTrackingInputs') === null ||
            $.cookie('incentiveTrackingInputs') === "" ||
            $.cookie('incentiveTrackingInputs') === "null" ||
            $.cookie('incentiveTrackingInputs') === undefined) {
            var incentiveTrackingInputs = [
            { 'Agent': "0"}
            ];
            $.cookie("incentiveTrackingInputs", JSON.stringify(incentiveTrackingInputs), { expires: 30 });
        }

        // Lead Volume
        if ($.cookie('leadVolumeInputs') === null ||
            $.cookie('leadVolumeInputs') === "" ||
            $.cookie('leadVolumeInputs') === "null" ||
            $.cookie('leadVolumeInputs') === undefined) {
            var leadVolumeInputs = [
            { 'Agent': "0", 'Campaigns': "0", 'SkillGroup': "0", 'StartDate': g_StartDate[0], 'EndDate': g_StartDate[1], 'DateOptionSelected': "0" }
            ];
            $.cookie("leadVolumeInputs", JSON.stringify(leadVolumeInputs), { expires: 30 });

        }

        // Pipeline
        if ($.cookie('pipelineInputs') === null ||
            $.cookie('pipelineInputs') === "" ||
            $.cookie('pipelineInputs') === "null" ||
            $.cookie('pipelineInputs') === undefined) {
            var pipelineInputs = [
            { 'Agent': "0" }
            ];
            $.cookie("pipelineInputs", JSON.stringify(pipelineInputs), { expires: 30 });
        }

        // Prioritized List
        
        if ($.cookie('prioritizedListInputs') === null ||
            $.cookie('prioritizedListInputs') === "" ||
            $.cookie('prioritizedListInputs') === "null" ||
            $.cookie('prioritizedListInputs') === undefined) {
            var prioritizedListInputs = [
            { 'Agent': userkey }
            ];
            $.cookie("prioritizedListInputs", JSON.stringify(prioritizedListInputs), { expires: 30 });
        }

        // Quota Tracking
        if ($.cookie('quotaTrackingInputs') === null ||
            $.cookie('quotaTrackingInputs') === "" ||
            $.cookie('quotaTrackingInputs') === "null" ||
            $.cookie('quotaTrackingInputs') === undefined) {
            var StartDate = new Date();
            var quotaTrackingInputs = [
            { 'Month': (StartDate.getMonth() + 1), 'Year': StartDate.getFullYear() }
            ];
            $.cookie("quotaTrackingInputs", JSON.stringify(quotaTrackingInputs), { expires: 30 });

        }

        // Submission & Enrolled
        if ($.cookie('submissionEnrolledInputs') === null ||
            $.cookie('submissionEnrolledInputs') === "" ||
            $.cookie('submissionEnrolledInputs') === "null" ||
            $.cookie('submissionEnrolledInputs') === undefined) {
            var StartDate = new Date();
            var submissionEnrolledInputs = [
            { 'Year': StartDate.getFullYear() }
            ];
            $.cookie("submissionEnrolledInputs", JSON.stringify(submissionEnrolledInputs), { expires: 30 });

        }
        
    }
});