function GALDialerQueuesViewModel(parent, onError) {

    var self = this;
    self.LeadsAvailableCount = ko.observable("1");
    self.txtLeadsAvailableCount = ko.observable("");
    self.txtLeadsDialedCount = ko.observable("0");

    self.TakeLeadClicked = ko.observable(false);
    self.OldText = ko.observable("");
    self.Oldreason = ko.observable("");

    self.TimerLock = ko.observable(false);

    self.btnLeadsDialedEnabled = ko.observable(false);
    //self.btnLeadsAvailableEnabled = ko.observable(true);
    self.reasonText = ko.observable("");
    self.isEnabled = ko.observable(true);
    self.txtLeadsAvailableCountFS = ko.computed(function () {
        return '13px';
        //if (parseInt(self.txtLeadsAvailableCount()) < 100)
        //    return '28px';
        //else if (parseInt(self.txtLeadsAvailableCount()) > 99 && parseInt(self.txtLeadsAvailableCount()) < 1000)
        //    return '20px';
        //else
        //    return '14px';
    });
    self.txtLeadsDialedCountFS = ko.computed(function () {
        if (parseInt(self.txtLeadsDialedCount()) < 100)
            return '28px';
        else if (parseInt(self.txtLeadsDialedCount()) > 99 && parseInt(self.txtLeadsDialedCount()) < 1000)
            return '20px';
        else
            return '14px';
    });
    self.btnLeadsAvailableEnabled = ko.computed(function () {
        if (!self.isEnabled())
            return false;
        if (self.LeadsAvailableCount() > 0 && !self.TimerLock())
            return true;
        else
            return false;
    });

    self.btnLeadsAvailableClick = function () {
        self.DialALead();
    };


    self.btnLeadsDialedClick = function () {
        self.updateCounts();
        //return false;
    };


    self.DialerButtonCss = ko.computed(function () {

        var btn = document.getElementById('btnWebGalAvailable');
        btn.className = "";

        if (self.TakeLeadClicked()) {
            btn.className = "ButtonClicked"
            //return "ButtonClicked";
            return "";
        }
        if (self.btnLeadsAvailableEnabled()) {
            btn.className = "AvailableButton";
            //return "AvailableButton";
            return "";
        }
        btn.className = "AvailableButtonNoLead";
        return "";
        //return "AvailableButtonNoLead";
    });
     

    self.checkUserData = function () {
        if (localStorage["SignalRurl"] == null || localStorage["SignalRurl"] == undefined || localStorage["SignalRurl"] == ""
            //|| localStorage["icUserKeyGUID"] == null || localStorage["icUserKeyGUID"] == undefined || localStorage["icUserKeyGUID"] == ""
            /*|| parseInt(localStorage["maxDailyLeadsGal"]) || 0 == 0*/) {
            return self.getUserData();
        } else {
            return true;
        }
    }

    self.getUserData = function () {
        console.info("#@G# getUserData ");
        galDialerAPI.getUserData(onGetDataSuccess, onGetDataError);
        function onGetDataSuccess(response) {

            localStorage["icUserKeyGUID"] = (response.agentKey.length > 30 ?
                response.agentKey :
                ((localStorage["icUserKeyGUID"] == undefined || localStorage["icUserKeyGUID"] == null) ?
                "" : localStorage["icUserKeyGUID"]));


            localStorage["SignalRurl"] = response.SignalRurl.length > 3 ?
                response.SignalRurl
                : ((localStorage["SignalRurl"] != undefined) ?
                    localStorage["SignalRurl"]
                    : "");

            localStorage["DisableDialALeadSeconds"] = ((parseInt(response.DisableSeconds) || 0) > 0 ? parseInt(response.DisableSeconds) : 15);
            //localStorage["maxDailyLeadsGal"] = parseInt(response.maxDailyLead) || 0;
            self.updateCounts();
            //parent.DialerSignalRVM.initSignalR();
            console.info("#@G# getUserData - onGetDataSuccess, response: " + response);
            signalrMainObj.initSignalR();
            return true;
        }
        function onGetDataError(response) {
            console.info("#@G# getUserData - onGetDataError ");
            return false;
        }
    }


    self.updateCounts = function () {
        
        console.info("#@G# GALDialer - updateCounts ");
        galDialerAPI.getCounts(onGetCountSuccess, onGetCountError);
        
        function onGetCountSuccess(response) {
            if (response != null && response !== undefined) {
                //self.isEnabled(response.IsEnabled);
                //self.reasonText(response.Reason);

                var lastCount = 0;
                if (localStorage["lastWebGALCount"] != undefined || localStorage["lastWebGALCount"] != null) {
                    var lastCount = localStorage["lastWebGALCount"];
                }
                var available = parseInt(response.total_assignable_leads) || 0;
                var dialed = parseInt(response.total_assigned_leads) || 0;
                if (lastCount < available) {
                    //TL if Agent is on phone turn off tone
                    if (localStorage["currentState"] === "Available" || localStorage["currentState"] === "Unavailable") {
                        self.playTone();
                    }
                }

                //Commented out not required
                //var maxCap = parseInt(localStorage["maxDailyLeadsGal"]) || 0;
                //if (maxCap > 0 && dialed >= maxCap) {
                //    self.isEnabled(response.IsEnabled);
                //    self.reasonText("Max Daily Limit Reached");
                //}


                self.isEnabled(response.IsEnabled);
                self.LeadsAvailableCount(available);
                
                if (self.TakeLeadClicked() == true) {
                    self.txtLeadsAvailableCount("Queued");
                    self.reasonText("Request is Queued");
                    self.isEnabled(false);
                }
                else if (self.TimerLock()) {
                    self.txtLeadsAvailableCount("No Lead");
                    self.reasonText("You Recently Dialed a Lead");
                    self.isEnabled(false);
                }
                else {
                    if ((parseInt(available) || 0) > 0 && response.IsEnabled) {
                        self.txtLeadsAvailableCount("Take Lead");
                        self.reasonText("Take a Lead");
                        self.isEnabled(true);
                    }
                    else {
                        self.txtLeadsAvailableCount("No Lead");
                        self.isEnabled(false);
                        //reason
                        if(response.Reason != undefined && response.Reason != null && response.Reason.length > 2){
                            self.reasonText(response.Reason);
                        }
                        else{
                            self.reasonText( "No Leads Available");
                        }                        
                    }
                    //Dialed button count and actual count received (not shown)
                    self.LeadsAvailableCount(available);
                    self.txtLeadsDialedCount(dialed);
                    localStorage["lastWebGALCount"] = available;
                }
            }
            else {
                self.txtLeadsAvailableCount("No Lead");
                self.reasonText('No Leads Available');
                self.isEnabled(false);
                localStorage["lastWebGALCount"] = 0;
            }
            console.info("#@G# GALDialerCount - onGetCountSuccess:  response " + response);
        }
        function onGetCountError(response) {
            console.info("#@G# GALDialerCount - onGetCountError:  response " + response);
        }
    }


    self.DialALead = function () {
        console.info("#@G# DialALead Clicked");
        self.TimerLock(true);
        self.TakeLeadClicked(true);
        localStorage["WebGALClickedDial"] = true;
        self.OldText(self.txtLeadsAvailableCount());
        self.Oldreason(self.reasonText());
        self.txtLeadsAvailableCount("Queued");
        self.reasonText("Request is Queued");
        
               
        galDialerAPI.dialAlead(onDialALeadSuccess, onDialALeadError);
       
        function onDialALeadSuccess(response) {
            
            if (response != null && response !== undefined && response != "") {
                localStorage["DisableDialALeadTime"] = new Date().getTime();
                localStorage["WebGALDialedALead"] = true;
                self.TimerLock(true);
                window.location.href = response;
            }
            else {
                localStorage["WebGALClickedDial"] = false;
                self.TimerLock(false);
                self.TakeLeadClicked(false);
                localStorage["DisableDialALeadTime"] = "";
                self.txtLeadsAvailableCount(self.OldText());
                self.reasonText(self.Oldreason());
                self.Oldreason("");
                self.OldText("");
                self.updateCounts();
            }
            console.info("#@G# DialALead - onDialALeadSuccess:  response " + response);
        }
        function onDialALeadError(response) {
            console.info("#@G# DialALead - onDialALeadError:  response " + response);
            self.TimerLock(false);
            self.TakeLeadClicked(false);
            localStorage["DisableDialALeadTime"] = "";
            localStorage["WebGALClickedDial"] = false;
            self.txtLeadsAvailableCount(self.OldText());
            self.reasonText(self.Oldreason());
            self.Oldreason("");
            self.OldText("");
            self.updateCounts();
        }
    }

    self.playTone = function () {
        try {

            //If the variable is not initialized or has an invalied value then assign it false i.e. not in call
            if (inCall == undefined || inCall == null || inCall == ""
                || (inCall != true && inCall != false)) {
                inCall = true;
            }

            if (!inCall) {
                var div = document.createElement('div');
                div.innerHTML = ' <audio controls> <source src="/WebGAL/Sounds/ElectronicChime.mp3" type="audio/mpeg"><source src="/PhoneBars/Sounds/ElectronicChime.WAV" type="audio/wav"></audio>';
                var sound = div.getElementsByTagName("audio");
                sound[0].volume = 0.6;
                sound[0].play();
                console.info("#@G# playTone - TonePlayed");
            }
        } catch (e) {
            console.info("#@G# playTone - ##EXCEPTION:  " + e.message());
        }
    }
}