
function mainDialerViewModel() {
    var self = this;
    self.GALDialerQueuesVM = new GALDialerQueuesViewModel(self, onSystemError);
    //self.DialerSignalRVM = new DialerSignalRViewModel(self, onSystemError);

    self.init = function () {
        localStorage["WebGALClickedDial"] = false;
        self.GALDialerQueuesVM.getUserData();
        if (localStorage["WebGALDialedALead"] == "true") {
            self.GALDialerQueuesVM.TimerLock(true);
            //self.GALDialerQueuesVM.btnLeadsAvailableEnabled(false);
            var SecondsToDisable = localStorage["DisableDialALeadSeconds"];
            var disabledTime = localStorage["DisableDialALeadTime"];

            var timer = setInterval(function () {
                var newtime = new Date().getTime();
                if (newtime - disabledTime > SecondsToDisable * 1000) {
                    self.GALDialerQueuesVM.TimerLock(false);
                    localStorage["WebGALDialedALead"] = false;
                    localStorage["DisableDialALeadTime"] = "";
                    self.GALDialerQueuesVM.updateCounts();
                    clearInterval(timer);
                }
            }, 200);
        }
    }
    self.initSignalR = function () {
        self.GALDialerQueuesVM.getUserData();
    }

    //initializeCounts();
    //function initializeCounts() {        
    //    self.init();
    //}
}

function onSystemError(error) {
    var msg = "Unknown page error."
    if (error != null) {
        //if (error.statusCode == 0) {
        //    msg = "Logging out of session....";
        //    //msg = "Connection failed. If the error continues contact your System Administrator.";
        //} else {
        //    msg = "Error " + error.statusCode + ": " + error.statusDescription;
        //}
    }

    //self.loginVM.setStatusMessage(msg, true);
    //console.log("Error: " + error.statusCode + " Desc:" + error.statusDescription + " Text: " + error.statusText + " Resp: " + error.responseText);
}