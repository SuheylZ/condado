<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DashboardV3.aspx.cs" Inherits="DashboardV3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.8.2.js" type="text/javascript"></script>
    <script src="Scripts/Dashboard/jquery-ui.js" type="text/javascript"></script>
    <script src="Scripts/Dashboard/jquery.tabSlideOut.v1.3.js" type="text/javascript"></script>

    <link rel="stylesheet" href="Styles/Dashboard/jquery-ui.css" />

     <script type="text/javascript">
         $(document).ready(function () {

             $(function () {
                 $('.slide-out-div').tabSlideOut({
                     tabHandle: '.handle',                     //class of the element that will become your tab
                     pathToTabImage: 'images/feedback_top_tab.gif', //path to the image for the tab //Optionally can be set using css
                     imageHeight: '32px',                     //height of tab image           //Optionally can be set using css
                     imageWidth: '167px',                       //width of tab image            //Optionally can be set using css
                     tabLocation: 'top',                      //side of screen where tab lives, top, right, bottom, or left
                     speed: 300,                               //speed of animation
                     action: 'click',                          //options: 'click' or 'hover', action to trigger animation
                     topPos: '100px',                          //position from the top/ use if tabLocation is left or right
                     leftPos: '171px',                          //position from left/ use if tabLocation is bottom or top
                     fixedPosition: false                      //options: true makes it stick(fixed position) on scroll
                 });

             });


             // Show Reports on Click from Slider
             $("#lnkSalesMetrics").click(function () {
                 $("#scSalesMetrics").show();
             });

             $("#lnkLeadMetrics").click(function () {
                 $("#scLeadMetrics").show();
             });

// ---------------  LEAD Metrics SCRIPTS ------------------------------------------------

             $("#parLeadMetrics").click(function () {
                 loadPopupBoxLeadMetrics();
             });

             $("#clLeadMetrics").click(function () {
                 $("#scSalesMetrics").hide();
             });
             // When site loaded, load the Popupbox First


             $('#LeadPopupBoxClose').click(function () {
                 unloadPopupBoxLeadMetrics();
             });


             function unloadPopupBoxLeadMetrics() {    // TO Unload the Popupbox
                 $('#LeadMetricsParm').fadeOut("slow");
             }

             function loadPopupBoxLeadMetrics() {    // To Load the Popupbox
                 $('#LeadMetricsParm').fadeIn("slow");
             }

             $("#lstLmDateSelected").change(function () {
                 var optionSelected = $("#lstLmDateSelected").val();
                 if (optionSelected == "7") {
                     $("#txtLmStartDate").datepicker().show();
                     $("#txtLmEndDate").datepicker().show();
                     $("#LeadMetricsParm").css({ // this is just for style        
                         "height": "275px"
                     });
                 } else {
                     $("#txtLmStartDate").datepicker().hide();
                     $("#txtLmEndDate").datepicker().hide();
                     $("#LeadMetricsParm").css({ // this is just for style        
                         "height": "40px"
                     });
                 }
             });


 // --------------    SALES Metrix SCRIPTS ----------------------------------------------

             $("#parSalesMetrics").click(function () {
                 loadPopupBoxSalesMetrics();
             });

             $("#clSalesMetrics").click(function () {
                 $("#scSalesMetrics").hide();
             });
             // When site loaded, load the Popupbox First


             $('#SalesPopupBoxClose').click(function () {
                 unloadPopupBoxSalesMetrics();
             });


             function unloadPopupBoxSalesMetrics() {    // TO Unload the Popupbox
                 $('#SalesMetricsParm').fadeOut("slow");
             }

             function loadPopupBoxSalesMetrics() {    // To Load the Popupbox
                 $('#SalesMetricsParm').fadeIn("slow");
             }

             $("#lstSmDateSelected").change(function () {
                 var optionSelected = $("#lstSmDateSelected").val();
                 if (optionSelected == "7") {
                     $("#txtSmStartDate").datepicker().show();
                     $("#txtSmEndDate").datepicker().show();
                     $("#SalesMetricsParm").css({ // this is just for style        
                         "height": "275px"
                     });
                 } else {
                     $("#txtSmStartDate").datepicker().hide();
                     $("#txtSmEndDate").datepicker().hide();
                     $("#SalesMetricsParm").css({ // this is just for style        
                         "height": "40px"
                     });
                 }
             });

         });

    </script>
    <style type="text/css">
        body {
            background: #b6b7bc;
            font-size: .80em;
            font-family: "Helvetica Neue", "Lucida Grande", "Segoe UI", Arial, Helvetica, Verdana, sans-serif;
            margin: 0px;
            padding: 0px;
            color: #696969;
        }

        #scWrapper {
            width: 1560px;
            height: auto;
            min-height: 1000px;
            background-color: #fff;
            margin: 0px auto 0px auto;
            border: 1px solid #496077;
        }

      .slide-out-div {
          padding: 20px;
          width: 1519px;
          background: #ccc;
          border: 1px solid #29216d;
      }
      
      /* popup_box DIV-Styles*/
    .popup_box {
        display: none; /* Hide the DIV */
        margin:0 auto;
        /*position:relative;*/
        /*_position: relative;*/ /* hack for internet explorer 6 */
        align-content:center;
        width: 900px;
        height:40px;
        background: #FFFFFF;
        left: 0px;
        top: 5px;
        z-index: 100; /* Layering ( on-top of others), if you have lots of layers: I just maximized, you can change it yourself */
        /*margin-left: 15px;*/
        /* additional features, can be omitted */
        border: 2px solid #b6b7bc;
        padding: 10px;
        -moz-box-shadow: 0 0 5px #b6b7bc;
        -webkit-box-shadow: 0 0 5px #b6b7bc;
        box-shadow: 0 0 5px #b6b7bc;
    }        
    
      </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="scWrapper">
    <div class="slide-out-div">
            <a class="handle" href="#"></a>
            <div>Select the Reports to display</div>
            <div>
                <a id="lnkSalesMetrics" href="#" style="margin-right:10px;">Sales Metrics</a>
                <a id="lnkLeadMetrics" href="#" style="margin-right:10px;">Lead Metrics</a>
            </div>
        </div>
        <!--- SPACER --->
        <div id="scSpacerDiv" style="height:75px;"></div>
        <!--- SALES METRICS INFO --->
        <div id="scSalesMetrics">
            <div style="text-align:center; float:left; width:1450px;">Sales Metrics Summary
            </div>
            <div style="text-align:right; float:right; width:100px;">
                <img id="parSalesMetrics" src="Images/ArrowDownIcon.png" style="width:30px; height:30px;" />
                <img id="clSalesMetrics" src="Images/CloseIcon.png" style="width:30px; height:30px;" />
            </div>
            <div style="clear:both;"></div>
            <!-- OUR PopupBox DIV-->
            <div id="SalesMetricsParm" class="popup_box">    
                <asp:DropDownList ID="lstSmAgents" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstSmDateSelected" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <img id="smRefreshData" src="Images/Refresh-icon.png" width="25" height="25" style="vertical-align: middle; margin-left:25px;" />
                <img id="SalesPopupBoxClose" src="Images/CloseIcon.png" style="width:30px; height:30px; vertical-align: middle;" /> 
                <div style="text-align:center">
                    <input type="text" id="txtSmStartDate" value="Start Date" class="scParmTextbox" style="display:none;" />
                    <input type="text" id="txtSmEndDate" value="End Date" class="scParmTextbox" style="display:none;" />
                </div>  
            </div>
            REPORT GO HERE
        </div>
        <!--- LEAD METRICS INFO --->
        <div id="scLeadMetrics">
            <div style="text-align:center; float:left; width:1450px;">Lead Metrics Summary
            </div>
            <div style="text-align:right; float:right; width:100px;">
                <img id="parLeadMetrics" src="Images/ArrowDownIcon.png" style="width:30px; height:30px;" />
                <img id="clLeadMetrics" src="Images/CloseIcon.png" style="width:30px; height:30px;" />
            </div>
            <div style="clear:both;"></div>
            <!-- PopupBox Div -->
            <div id="LeadMetricsParm" class="popup_box">  
                <asp:DropDownList ID="lstLmAgents" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmCampaign" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmSkillGroup" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <asp:DropDownList ID="lstLmDateSelected" runat="server" class="scSelect" AppendDataBoundItems="true" Width="200" />
                <img id="lmRefreshData" src="Images/Refresh-icon.png" width="25" height="25" style="vertical-align: middle; margin-left:25px;" />
                <img id="LeadPopupBoxClose" src="Images/CloseIcon.png" style="width:30px; height:30px; vertical-align: middle;" /> 
                <div style="text-align:center">
                    <input type="text" id="txtLmStartDate" value="Start Date" class="scParmTextbox" style="display:none;" />
                    <input type="text" id="txtLmEndDate" value="End Date" class="scParmTextbox" style="display:none;" />
                </div>
                   
            </div>
        </div>
    </div>
    </form>
</body>
</html>
