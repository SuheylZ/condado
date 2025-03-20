<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GetALead.ascx.cs" Inherits="MasterPages_GetALead" %>

<link href="/WebGAL/Styles/GetALeadDialer.css" rel="stylesheet" />
<!--[if IE]>
	<link href="/WebGAL/Styles/GetALeadDialerIE.css" rel="stylesheet" />
<![endif]-->

<%--<script src="/PhoneBars/CoreScripts/json2.js" type="text/javascript"></script>
<script src="/PhoneBars/CoreScripts/knockout-3.1.0.js" type="text/javascript"></script>
<script src="/PhoneBars/CoreScripts/jquery.base64.js" type="text/javascript"></script>
<script src="/Scripts/jquery.signalR-2.0.0.js" type="text/javascript"></script>
<script src="/Scripts/jquery.signalR-2.0.3.js" type="text/javascript"></script>
<script src="/Scripts/jquery.signalR-2.1.1.js" type="text/javascript"></script>
<script src="/PhoneBars/CoreScripts/signalRServer.js" type="text/javascript"></script>--%>



<script src="/WebGAL/ViewModels/DialerSignalRViewModel.js"></script>
<script src="/WebGAL/ViewModels/GALDialerAPI.js"></script>
<script src="/WebGAL/ViewModels/GALDialerQueuesViewModel.js"></script>
<script src="/WebGAL/ViewModels/MainDialerViewModel.js"></script>


<script src="/WebGAL/Scripts/displayGALDialer.js"></script>

<script type="text/javascript">
    $(document).keypress(function (e) {
        var keyCode = e.keyCode || e.which;

        var controlId = (e.target || window.event.srcElement).id;

        if (keyCode == 13 && controlId == 'btnWebGalAvailable') {
            //alert('You pressed enter!');
            return false;
        }

        return true;
    });

</script>
<div id="divGetALeadDialer" class="dialerBody">

    <div class="innerDiv">
        <div class="available">
            <div class="label">
                Available
            </div>
            <div class="dialerButtons" data-bind ="with:GALDialerQueuesVM"> 
                <button id="btnWebGalAvailable" title="LeadsAvailable" data-bind="css: txtLeadsAvailableCount() == '0' ? '' : 
    DialerButtonCss, text: txtLeadsAvailableCount, click: btnLeadsAvailableClick,
    attr: { title: reasonText }, enable: btnLeadsAvailableEnabled, style: { fontSize: txtLeadsAvailableCountFS }"></button>
                <%--data-bind="css: txtPersonalQueueCount() == '0' ? 'btnGALDisabled' : personalButtonCss, text: txtPersonalQueueCount, click: btnPersonalQueueClick, enable: btnPersonalQueueEnabled, style: { color: txtPersonalQueueCount() == '0' ? '#696969' : 'white' }"--%>
            </div>
        </div>
        <div class="dialed">
            <div class="label">
                Dialed
            </div>
            <div class="dialerButtons" data-bind ="with: GALDialerQueuesVM">
                <button id="btnWebGalDialed" title="LeadsDialed" data-bind="text: txtLeadsDialedCount, click: btnLeadsDialedClick,
                        enable: btnLeadsDialedEnabled, style: { fontSize: txtLeadsDialedCountFS }"></button>
            </div>
        </div>
    </div>

</div>
