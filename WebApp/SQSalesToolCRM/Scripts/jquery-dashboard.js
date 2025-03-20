
$(document).ready(function () {

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
    g_ShowHide[13] = true; // Score Card Report
    g_ShowHide[14] = true; // Prioritized List Report

    window.g_StartDate = new Array();
    g_StartDate[0] = "";
    g_StartDate[1] = "";

    //Setup the Start and End date for begin load. 
    CalendarTodayStartEndDates();
    // Setup DATE PICKER TEXT BOXES
    SetupDatePickerTextBoxes();
    // LOADS DEFAULT REPORTS DATA
    loadPrioritizedListReportData("0");
    loadLeadMetricsData("0", "0", "0", g_StartDate[0], g_StartDate[1]);
    loadScoreCardReportData(g_StartDate[0], g_StartDate[1]);
    loadStackedRankingChart(g_StartDate[0], g_StartDate[1]);
    loadCarrierMixReportData("0", "0", "0", g_StartDate[0], g_StartDate[1]);
    loadCaseSpecialistReportData("0", g_StartDate[0], g_StartDate[1]);
    loadCpaReportData(g_StartDate[0], g_StartDate[1]);
    loadFillFormReportData("0", "0", "0", g_StartDate[0], g_StartDate[1]);
    loadGoalReportData("0", "0", "0", "0");
    loadIncentiveTrackingReportData(g_StartDate[0]);
    loadLeadVolumeReportData("0", "0", "0", g_StartDate[0], g_StartDate[1]);
    // Premium Report Goes here
    loadPipeLineReportData("0");
    
    loadQuotaTrackingReportData(g_StartDate[0]);
    loadSubmissionsEnrollmentsReportData(g_StartDate[0]);
    

    // ---- GET THE LEAD METRICS DATA

    function loadLeadMetricsData(strAgents, strCampaigns, strSkillGroup, strStartDate, strEndDate) {
        $('#scLeadMetricArea').fadeTo(500, 0.6);
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
                        }
        });
        $('#scLeadMetricArea').fadeTo(100, 1.0);
    }

    // LOADS LEAD DATA FIRST TIME
    

    //Hide Show Textbox for Calendar

    $('#MainContent_lstlmDateSelected').click(function () {
        var calId = $('#MainContent_lstlmDateSelected').val();

        if (calId == 7) {
            $('#txtLeadMetricsStartDate').show();
            $('#txtLeadMetricsEndDate').show();
        } else {
            $('#txtLeadMetricsStartDate').hide();
            $('#txtLeadMetricsEndDate').hide();
        }
    });

    // LEAD Metrics Button
    $("#lmRefreshData").click(function () {
        var strAgents = $("#MainContent_lstlmAgents").val();
        var strCampaign = $("#MainContent_lstlmCampaign").val();
        var strSkillGroup = $("#MainContent_lstlmSkillGroup").val();
        var strDateSelected = $("#MainContent_lstlmDateSelected").val();
        
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
    });



    //  ----- CHARTS ----
    // DATATABLES USE WEB SERVICES TO PULL IN DATA
    //  ----- Stack Ranking Chart ----

    function onStartShowHideStackedRankingReport(g_ShowHide) {
        if (g_ShowHide[4] == true) {
            window.g_ShowHide[4] = false;
            hideshowStackedRanking();

        }
    }

    $('#StackedRankingHeader').click(function () {
        hideshowStackedRanking();
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
            $('#lblStackedRankingReporting').css('color', '#E96F00');
            $('#StackedRankingParms').show();
            $('#StackedRankingReport').slideDown("slow");
        }
        else {
            $('#lblStackedRankingReporting').css('color', '#FFFFFF');
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
        onStartShowHideStackedRankingReport(g_ShowHide)
    }


    // ----- ALL REPORTS ---- 
    // DATATABLES USE WEB SERVICES TO PULL IN DATA
    //  ----- ScoreCard REPORTING

    function onStartShowHideScoreCardReporting(g_ShowHide) {
        if (g_ShowHide[13] == true) {
            window.g_ShowHide[13] = false;
            hideshowScoreCardReport();
        }
    }

    $('#ScoreCardHeader').click(function () {
        hideshowScoreCardReport();
    });

    $('#ScoreCardRefresh').click(function () {
        var strDateSelected = $("#MainContent_lstScoreCardCalendar").val();
        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadScoreCardReportData(strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtScoreCardStartDate").val())
            var isEndDateGood = isDate($("#txtScoreCardEndDate").val())
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
            $('#lblScoreCardReporting').css('color', '#E96F00');
            $('#ScoreCardParms').show();
            $('#ScoreCardReport').slideDown("slow");
        }
        else {
            $('#lblScoreCardReporting').css('color', '#FFFFFF');
            $('#ScoreCardReport').slideUp("slow");
            $('#ScoreCardParms').hide();
        }
    }

    function loadScoreCardReportData(strStartDate, strEndDate) {
        $('#ScoreCardReport').fadeTo(500, 0.2);
        var intCounter = 0;
        $("#ScoreCardReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripeClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetScoreCardReport",
            "bDeferRender": false,
            "sScrollY": 150,
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
                $('#ScoreCardReport').fadeTo(100, 1.0);
                onStartShowHideScoreCardReporting(g_ShowHide);
            }
        });
        
    }

    // ----- Carrier Mix REPORTING

    function onStartShowHideCarrierMixReport(g_ShowHide) {
        if (g_ShowHide[10] == true) {
            window.g_ShowHide[10] = false;
            hideshowCarrierMixReport();
        }
    }

    $('#CarrierMixHeader').click(function () {
        hideshowCarrierMixReport();
    });

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
            $('#lblCarrierMixReporting').css('color', '#E96F00');
            $('#CarrierMixParms').show();
            $('#CarrierMixReport').slideDown("slow");
        }
        else {
            $('#lblCarrierMixReporting').css('color', '#FFFFFF');
            $('#CarrierMixReport').slideUp("slow");
            $('#CarrierMixParms').hide();
        }
    }

    function loadCarrierMixReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#CarrierMixReport').fadeTo(500, 0.2);
        $("#CarrierMixReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetCarrierMix",
            "bDeferRender": true,
            "sScrollY": "300px",
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

                $('#CarrierMixReport').fadeTo(100, 1.0);
                onStartShowHideCarrierMixReport(g_ShowHide)
            }
        });
        
    }

    // ----- Case Specialist REPORTING

    function onStartShowHideCaseSpecialistReport(g_ShowHide) {
        if (g_ShowHide[7] == true) {
            window.g_ShowHide[7] = false;
            hideshowCaseSpecialistReport();
        }
    }

    $('#CaseSpecialistHeader').click(function () {
        hideshowCaseSpecialistReport();
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
            $('#lblCaseSpecialistReporting').css('color', '#E96F00');
            $('#CaseSpecialistParms').show();
            $('#CaseSpecialistReport').slideDown("slow");
        }
        else {
            $('#lblCaseSpecialistReporting').css('color', '#FFFFFF');
            $('#CaseSpecialistReport').slideUp("slow");
            $('#CaseSpecialistParms').hide();
        }
    }

    function loadCaseSpecialistReportData(strSkillGroup, strStartDate, strEndDate) {
        $('#CaseSpecialistReport').fadeTo(500, 0.2);
        $("#CaseSpecialistReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetCaseSpecialist",
            "bDeferRender": true,
            "sScrollY": "175px",
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

                $('#CaseSpecialistReport').fadeTo(500, 1.0);
                onStartShowHideCaseSpecialistReport(g_ShowHide);
            }
        });
        
    }

    //  ----- CPA REPORTING 

    // Create According look for reports
    function onStartShowHideCpaReporting(g_ShowHide) {
        if (g_ShowHide[1] == true) {
            window.g_ShowHide[1] = false;
            hideshowCpaReport();
            
        }
    }

    $('#CpaHeader').click(function () {
        hideshowCpaReport();
    });

    $('#CpaRefresh').click(function () {
        var strDateSelected = $("#MainContent_lstCpaCalendar").val();
        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadCpaReportData(strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtCpaStartDate").val())
            var isEndDateGood = isDate($("#txtCpaEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadCpaReportData($("#txtCpaStartDate").val(), $("#txtCpaEndDate").val());
            }
        }
    });

    $('#MainContent_lstCpaCalendar').click(function () {
        var calId = $('#MainContent_lstCpaCalendar').val();

        if (calId == 7) {
            $('#txtCpaStartDate').show();
            $('#txtCpaEndDate').show();
        } else {
            $('#txtCpaStartDate').hide();
            $('#txtCpaEndDate').hide();
        }
    });

    function hideshowCpaReport() {
        if ($('#CpaReport').is(':hidden')) {
            $('#lblCpaReporting').css('color', '#E96F00');
            $('#CpaParms').show();
            $('#CpaReport').slideDown("slow");
        }
        else {
            $('#lblCpaReporting').css('color', '#FFFFFF');
            $('#CpaReport').slideUp("slow");
            $('#CpaParms').hide();
        }
    }


    function loadCpaReportData(strStartDate, strEndDate) {
        $('#CpaReport').fadeTo(500, 0.2);
        $("#CpaReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetCpaReport",
            "bDeferRender": false,
            "sScrollY": 150,
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
                        $("#CpaReportingTable").show();
                    }
                });

            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iValidLeads = 0;
                var iMedSuppPlansClosed = 0;
                var iPercentOfValidMedSupp = 0;
                var iMAPlansClosed = 0;
                var iPercentOfValidMA = 0;
                var iPoliciesClosed = 0;
                var iPercentOfValidTotal = 0;
                var iProjectedClosePercent = 0;
                var iCostPerAcquisition = 0;
                var iProjectedCPA = 0;

                for (var i = 0 ; i < aoData.length ; i++) {

                    iValidLeads += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                    iMedSuppPlansClosed += parseInt(Number(parseInt(aoData[i][2].replace(/[^0-9\.]+/g, ""))))
                    iPercentOfValidMedSupp += parseFloat(Number(parseInt(aoData[i][3].replace(/[^0-9\.]+/g, ""))))
                    iMAPlansClosed += parseInt(Number(parseInt(aoData[i][4].replace(/[^0-9\.]+/g, ""))))
                    iPercentOfValidMA += parseFloat(Number(parseInt(aoData[i][5].replace(/[^0-9\.]+/g, ""))))
                    iPoliciesClosed += parseInt(Number(parseInt(aoData[i][6].replace(/[^0-9\.]+/g, ""))))
                    iPercentOfValidTotal += parseFloat(Number(parseInt(aoData[i][7].replace(/[^0-9\.]+/g, ""))))
                    iProjectedClosePercent += parseFloat(Number(parseInt(aoData[i][8].replace(/[^0-9\.]+/g, ""))))
                    iCostPerAcquisition += parseInt(Number(parseInt(aoData[i][9].replace(/[^0-9\.]+/g, ""))))
                    iProjectedCPA += parseInt(Number(parseInt(aoData[i][10].replace(/[^0-9\.]+/g, ""))))
                }

                nCells[1].innerHTML = FormatNumber(iValidLeads);
                nCells[2].innerHTML = FormatNumber(iMedSuppPlansClosed);
                nCells[3].innerHTML = FormatNumber(iPercentOfValidMedSupp);
                nCells[4].innerHTML = FormatNumber(iMAPlansClosed);
                nCells[5].innerHTML = FormatNumber(iPercentOfValidMA);
                nCells[6].innerHTML = FormatNumber(iPoliciesClosed);
                nCells[7].innerHTML = FormatNumber(iPercentOfValidTotal);
                nCells[8].innerHTML = FormatNumber(iProjectedClosePercent);
                nCells[9].innerHTML = FormatNumber(iCostPerAcquisition);
                nCells[10].innerHTML = FormatNumber(iProjectedCPA);

                $('#CpaReport').fadeTo(500, 1.0);
                onStartShowHideCpaReporting(g_ShowHide);
            }

        });
        
    }


    // ----- Fall Off Form REPORTING

    function onStartShowHideFallOffReport(g_ShowHide) {
        if (g_ShowHide[12] == true) {
            window.g_ShowHide[12] = false;
            hideshowFallOffReport();
        }
    }

    $('#FallOffHeader').click(function () {
        hideshowFallOffReport();
    });

    $('#FallOffRefresh').click(function () {
        //var strPlanType = $("#MainContent_lstCarrierMixCalendar").val();
        var strAgent = $("#MainContent_lstFallOffAgents").val();
        var strSkillGroup = $("#MainContent_lstFallOffSkillGroup").val();
        var strCampaign = $("#MainContent_lstFallOffCampaign").val();
        var strDateSelected = $("#MainContent_lstFallOffCalendar").val();

        if (strDateSelected <= 6) {
            var strReturnedDates = GetDateSelected(strDateSelected);
            loadFallOffReportData(strAgent, strSkillGroup, strCampaign, strReturnedDates[0], strReturnedDates[1]);
        } else {
            var isStartDateGood = isDate($("#txtFallOffStartDate").val())
            var isEndDateGood = isDate($("#txtFallOffEndDate").val())
            if (isStartDateGood && isEndDateGood) {
                loadFallOffReportData(strAgent, strSkillGroup, strCampaign, $("#txtFallOffStartDate").val(), $("#txtFallOffEndDate").val());
            }
        }
    });

    $('#MainContent_lstFallOffCalendar').click(function () {
        var calId = $('#MainContent_lstFallOffCalendar').val();

        if (calId == 7) {
            $('#txtFallOffStartDate').show();
            $('#txtFallOffEndDate').show();
        } else {
            $('#txtFallOffStartDate').hide();
            $('#txtFallOffEndDate').hide();
        }
    });

    function hideshowFallOffReport() {
        if ($('#FallOffReport').is(':hidden')) {
            $('#lblFallOffReporting').css('color', '#E96F00');
            $('#FallOffParms').show();
            $('#FallOffReport').slideDown("slow");
        }
        else {
            $('#lblFallOffReporting').css('color', '#FFFFFF');
            $('#FallOffReport').slideUp("slow");
            $('#FallOffParms').hide();
        }
    }

    function loadFallOffReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#FallOffReport').fadeTo(500, 0.2);
        $("#FallOffReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetFallOff",
            "bDeferRender": true,
            "sScrollY": "75px",
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
                                    $("#FallOffReportingTable").show();
                                }
                });
            },
            "fnFooterCallback": function (nRow, aoData) {
                var nCells = nRow.getElementsByTagName('th');
                var iFallOutCount = 0;

                for (var i = 0 ; i < aoData.length ; i++) {
                    iFallOutCount += parseInt(Number(parseInt(aoData[i][1].replace(/[^0-9\.]+/g, ""))));
                }

                nCells[1].innerHTML = FormatNumber(iFallOutCount);
                $('#FallOffReport').fadeTo(500, 1.0);
                onStartShowHideFallOffReport(g_ShowHide);
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

    $('#FillFormHeader').click(function () {
        hideshowFillFormReport();
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
            $('#lblFillFormReporting').css('color', '#E96F00');
            $('#FillFormParms').show();
            $('#FillFormReport').slideDown("slow");
        }
        else {
            $('#lblFillFormReporting').css('color', '#FFFFFF');
            $('#FillFormReport').slideUp("slow");
            $('#FillFormParms').hide();
        }
    }

    function loadFillFormReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#FillFormReport').fadeTo(500, 0.2);
        $("#FillFormReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetFillForm",
            "bDeferRender": true,
            "sScrollY": "200px",
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

                $('#FillFormReport').fadeTo(500, 1.0);
                onStartShowHideFillFormReport(g_ShowHide)
            }
        });
        
    }

    // ----- Goal REPORTING
    
    function onStartShowHideGoalReport(g_ShowHide) {
        if (g_ShowHide[6] == true) {
            window.g_ShowHide[6] = false;
            hideshowGoalReport();
        }
    }

    $('#GoalHeader').click(function () {
        hideshowGoalReport();
    });

    $('#GoalRefresh').click(function () {
        //var strPlanType = $("#MainContent_lstQuotaTrackingPlanType").val();
        var strMonth = $("#MainContent_lstGoalCalendar").val();
        var strYear = "";
        var strTotalWorkDays = "";
        var strTotalDaysWorked = "";
        loadGoalReportData(strMonth, strYear, strTotalWorkDays, strTotalDaysWorked);
    });

    function hideshowGoalReport() {
        if ($('#GoalReport').is(':hidden')) {
            $('#lblGoalReporting').css('color', '#E96F00');
            $('#GoalParms').show();
            $('#GoalReport').slideDown("slow");
        }
        else {
            $('#lblGoalReporting').css('color', '#FFFFFF');
            $('#GoalReport').slideUp("slow");
            $('#GoalParms').hide();
        }
    }

    function loadGoalReportData(strMonth, strYear, strTotalWorkDays, strTotalDaysWorked) {
        $('#GoalReport').fadeTo(500, 0.2);
        $("#GoalReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetGoalReport",
            "bDeferRender": true,
            "sScrollY": "300px",
            "sDom": "rtS",
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "strMonth", "value": strMonth });
                aoData.push({ "name": "strYear", "value": strYear });
                aoData.push({ "name": "strTotalWorkDays", "value": strTotalWorkDays });
                aoData.push({ "name": "strTotalDaysWorked", "value": strTotalDaysWorked });
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

                $('#GoalReport').fadeTo(500, 1.0);
                onStartShowHideGoalReport(g_ShowHide)
            }
        });
        
    }
    
    // ----- Incentive Tracking REPORTING
    
    function onStartShowHideIncentiveTrackingReport(g_ShowHide) {
        if (g_ShowHide[3] == true) {
            window.g_ShowHide[3] = false;
            hideshowIncentiveTracking()

        }
    }

    $('#IncentiveTrackingHeader').click(function () {
        hideshowIncentiveTracking();
    });

    $('#IncentiveTrackingRefresh').click(function () {
        var strAgents = $("#MainContent_lstIncentiveTrackingAgents").val();
        loadIncentiveTrackingReportData(strAgents);
    });

    function hideshowIncentiveTracking() {
        if ($('#IncentiveTrackingReport').is(':hidden')) {
            $('#lblIncentiveTrackingReporting').css('color', '#E96F00');
            $('#IncentiveTrackingParms').show();
            $('#IncentiveTrackingReport').slideDown("slow");
        }
        else {
            $('#lblIncentiveTrackingReporting').css('color', '#FFFFFF');
            $('#IncentiveTrackingReport').slideUp("slow");
            $('#IncentiveTrackingParms').hide();
        }
    }



    function loadIncentiveTrackingReportData(strStartDate) {
        $('#IncentiveTrackingReport').fadeTo(500, 0.2);
        $("#IncentiveTrackingReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetIncentiveTracking",
            "bDeferRender": true,
            "sScrollY": "300px",
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
                $('#IncentiveTrackingReport').fadeTo(500, 1.0);
                onStartShowHideIncentiveTrackingReport(g_ShowHide);
            }
        });
        
    }

    // ----- Lead Volume REPORTING

    function onStartShowHideLeadVolumeReport(g_ShowHide) {
        if (g_ShowHide[5] == true) {
            window.g_ShowHide[5] = false;
            hideshowLeadVolume();
        }
    }

    $('#LeadVolumeHeader').click(function () {
        hideshowLeadVolume();
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
            $('#lblLeadVolumeReporting').css('color', '#E96F00');
            $('#LeadVolumeParms').show();
            $('#LeadVolumeReport').slideDown("slow");
        }
        else {
            $('#lblLeadVolumeReporting').css('color', '#FFFFFF');
            $('#LeadVolumeReport').slideUp("slow");
            $('#LeadVolumeParms').hide();
        }
    }

    function loadLeadVolumeReportData(strAgent, strSkillGroup, strCampaign, strStartDate, strEndDate) {
        $('#LeadVolumeReport').fadeTo(500, 0.2);
        $("#LeadVolumeReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetLeadVolume",
            "bDeferRender": true,
            "sScrollY": "300px",
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
                $('#LeadVolumeReport').fadeTo(500, 1.0);
                onStartShowHideLeadVolumeReport(g_ShowHide);
            }
        });
        
    }

    // ----- Premium REPORTING

    $('#PremiumHeader').click(function () {
        hideshowPremiumReport();
    });

    $('#PremiumRefresh').click(function () {
        //var strPlanType = $("#MainContent_lstSubmissionEnrollCalendar").val();
        var strCalendar = $("#MainContent_lstPremiumAgents").val();
        //loadCaseSpecialistReportData(strAgents);
    });

    function hideshowPremiumReport() {
        if ($('#PremiumReport').is(':hidden')) {
            $('#lblPremiumReporting').css('color', '#E96F00');
            $('#PremiumParms').show();
            $('#PremiumReport').slideDown("slow");
        }
        else {
            $('#lblPremiumReporting').css('color', '#FFFFFF');
            $('#PremiumReport').slideUp("slow");
            $('#PremiumParms').hide();
        }
    }

    // ----- PIPELINE REPORTING --------------

    function onStartShowHidePipelineReport(g_ShowHide) {
        if (g_ShowHide[2] == true) {
            window.g_ShowHide[2] = false;
            hideshowPipeline();

        }
    }

    // Create According look for reports
    $('#PipelineHeader').click(function () {
        hideshowPipeline();
    });

    $('#PipelineRefresh').click(function () {
        var strAgents = $("#MainContent_lstPipelineAgents").val();
        loadPipeLineReportData(strAgents);
    });

    function hideshowPipeline() {
        if ($('#PiplineReport').is(':hidden')) {
            $('#lblPipelineReporting').css('color', '#E96F00');
            $('#PipelineParms').show();
            $('#PiplineReport').slideDown("slow");
        }
        else {
            $('#lblPipelineReporting').css('color', '#FFFFFF');
            $('#PiplineReport').slideUp("slow");
            $('#PipelineParms').hide();
        }
    }


    function loadPipeLineReportData(strAgentID) {
        $('#PiplineReport').fadeTo(500, 0.2);
        $("#PipelineReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": true,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetPipelineReport",
            "bDeferRender": true,
            "sScrollY": "300px",
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
                $('#PiplineReport').fadeTo(500, 1.0);
                onStartShowHidePipelineReport(g_ShowHide);
            }
        });
    }

    // ----- Prioritized List Report

    function onStartShowHidePrioritizedList(g_ShowHide) {
        if (g_ShowHide[14] == true) {
            window.g_ShowHide[14] = false;
            hideshowPrioritizedList();
        }
    }

    $('#PrioritizedListHeader').click(function () {
        hideshowPrioritizedList();
    });

    $('#PrioritizedListRefresh').click(function () {
        var strAgent = $("#MainContent_lstPrioritizedListAgents").val();
        loadPrioritizedListReportData(strAgent);
    });

    function hideshowPrioritizedList() {
        if ($('#PrioritizedListReport').is(':hidden')) {
            $('#lblPrioritizedListReporting').css('color', '#E96F00');
            $('#PrioritizedListParms').show();
            $('#PrioritizedListReport').slideDown("slow");
        }
        else {
            $('#lblPrioritizedListReporting').css('color', '#FFFFFF');
            $('#PrioritizedListReport').slideUp("slow");
            $('#PrioritizedListParms').hide();
        }
    }

    function loadPrioritizedListReportData(strAgent) {
        $('#PrioritizedListReport').fadeTo(500, 0.2);
        $("#PrioritizedListReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetPrioritizedLeads",
            "bDeferRender": true,
            "sScrollY": "300px",
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
                $('#PrioritizedListReport').fadeTo(500, 1.0);
                onStartShowHidePrioritizedList(g_ShowHide);
            }
        });
        
    }

    // ----- Quota Tracking REPORTING

    function onStartShowHideQuotaTracking(g_ShowHide) {
        if (g_ShowHide[0] == true) {
            window.g_ShowHide[0] = false;
            hideshowQuotaTracking();
        }
    }

    $('#QuotaTrackingHeader').click(function () {
        hideshowQuotaTracking();
    });

    $('#QuotaTrackingRefresh').click(function () {
        var strPlanType = $("#MainContent_lstQuotaTrackingPlanType").val();
        var strCalendar = $("#MainContent_lstQuotaTrackingCalendar").val();
        if (strCalendar == "0") {
            loadQuotaTrackingReportData(g_StartDate[0]);
        }
        
    });

    function hideshowQuotaTracking() {
        if ($('#QuotaTrackingReport').is(':hidden')) {
            $('#lblQuotaTrackingReporting').css('color', '#E96F00');
            $('#QuotaTrackingParms').show();
            $('#QuotaTrackingReport').slideDown("slow");
        }
        else {
            $('#lblQuotaTrackingReporting').css('color', '#FFFFFF');
            $('#QuotaTrackingReport').slideUp("slow");
            $('#QuotaTrackingParms').hide();
        }
    }

    function loadQuotaTrackingReportData(strStartDate) {
        $('#QuotaTrackingReport').fadeTo(500, 0.2);
        $("#QuotaTrackingReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetQuotaTracking",
            "bDeferRender": true,
            "sScrollY": "300px",
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

                $('#QuotaTrackingReport').fadeTo(500, 1.0);
                onStartShowHideQuotaTracking(g_ShowHide);
            }
        });
        
    }


    // ----- Submissions & Enrollments REPORTING

    function onStartShowHideSubmissionEnrollReport(g_ShowHide) {
        if (g_ShowHide[8] == true) {
            window.g_ShowHide[8] = false;
            hideshowSubmissionEnrollReport();
        }
    }

    $('#SubmissionEnrollHeader').click(function () {
        hideshowSubmissionEnrollReport();
    });

    $('#SubmissionEnrollRefresh').click(function () {
        var strCalendar = $("#MainContent_lstSubmissionEnrollCalendar").val();
        
        if (strCalendar == 0) {
            var date = new Date();
            varStartDate = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
            loadSubmissionsEnrollmentsReportData(varStartDate);
        } else {
            var date = new Date();
            date.setFullYear(date.getFullYear() - 1);
            varStartDate = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
            loadSubmissionsEnrollmentsReportData(varStartDate);
        }
        
    });

    function hideshowSubmissionEnrollReport() {
        if ($('#SubmissionEnrollReport').is(':hidden')) {
            $('#lblSubmissionEnrollReporting').css('color', '#E96F00');
            $('#SubmissionEnrollParms').show();
            $('#SubmissionEnrollReport').slideDown("slow");
        }
        else {
            $('#lblSubmissionEnrollReporting').css('color', '#FFFFFF');
            $('#SubmissionEnrollReport').slideUp("slow");
            $('#SubmissionEnrollParms').hide();
        }
    }

    function loadSubmissionsEnrollmentsReportData(strStartDate) {
        $('#SubmissionEnrollReport').fadeTo(500, 0.2);
        $("#SubmissionsEnrollmentsReportingTable").dataTable({
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": false,
            "bInfo": false,
            "bAutoWidth": false,
            "asStripClasses": null,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "Services/DashboardReporting.asmx/GetSubmissionsEnrollments",
            "bDeferRender": true,
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

                $('#SubmissionEnrollReport').fadeTo(500, 1.0);
                onStartShowHideSubmissionEnrollReport(g_ShowHide)
            }
        });
        
    }

    // ----- Compensation Summary NOT ACTIVE CURRENTLY

    //NOT SHOWING THIS REPORT AT THIS TIME
    /*
    $('#CommisionDashboardHeader').click(function () {
        hideshowCommisionDashboard();
    });

    $('#CommisionDashboardRefresh').click(function () {
        //var strPlanType = $("lstQuotaTrackingPlanType").val();
        var strCalendar = $("#MainContent_lstCommisionDashboardCalendar").val();
        //loadQuotaTrackingReportData(strAgents);
    });

    function hideshowCommisionDashboard() {
        if ($('#CommisionDashboardReport').is(':hidden')) {
            $('#lblCommisionDashboardReporting').css('color', '#E96F00');
            $('#CommisionDashboardParms').show();
            $('#CommisionDashboardReport').slideDown("slow");
        }
        else {
            $('#lblCommisionDashboardReporting').css('color', '#FFFFFF');
            $('#CommisionDashboardReport').slideUp("slow");
            $('#CommisionDashboardParms').hide();
        }
    }
    */



    // Numbr Formatter
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
                var date = new Date();
                varStartDate = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
                varEndDate = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
                break;
            case '1':
                var startDate = new Date();
                startDate.setDate(startDate.getDate() - 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                break;
            case '2':
                var startDate = new Date();
                var EndDate = new Date();
                var startDay = new Date().getDay();  //0=Sun, 1=Mon, ..., 6=Sat
                startDate.setDate(startDate.getDate() - startDay);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear()
                break;
            case '3':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate((startDate.getDate() - startDate.getDate()) + 1);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear()
                break;
            case '4':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 7);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear()
                break;
            case '5':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 14);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear()
                break;
            case '6':
                var startDate = new Date();
                var EndDate = new Date();
                startDate.setDate(startDate.getDate() - 30);
                varStartDate = (startDate.getMonth() + 1) + '/' + startDate.getDate() + '/' + startDate.getFullYear()
                varEndDate = (EndDate.getMonth() + 1) + '/' + EndDate.getDate() + '/' + EndDate.getFullYear()
                break;
        }

        return [varStartDate, varEndDate];
    }

    // Set Date Picker TextBoxes
    function SetupDatePickerTextBoxes() {
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

    // ----- SET First HIDE SHOW 

    hideshowPremiumReport();
    
    
});