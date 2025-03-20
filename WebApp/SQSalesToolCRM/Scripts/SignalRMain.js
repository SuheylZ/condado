var signalrMainObj = new SignalMain();
//signalrMainObj.initSignalR();

function SignalMain() {

    var self = this;

    this.initSignalR = function() {
        try {
            
            //http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-javascript-client#crossdomain
            //SignalR handles the use of CORS. Setting jQuery.support.cors to true disables JSONP because it causes SignalR to assume the browser supports CORS.
            jQuery.support.cors = true;
            
            //Assign the URL of SignalR hub
            $.connection.hub.url = (localStorage["SignalRurl"] !== null && localStorage["SignalRurl"] !== undefined) ? localStorage["SignalRurl"] : "";

            localStorage["reAttemptCount"] = 0;

            // Declare a proxy to reference the hub.
            if ($.connection !== undefined) {
                self.SignalRHub = $.connection.selectCareHub;
            }

            //Send Request to HUB for getting registered for notifications from SignalR
            self.SignalRHub.client.SendRegisterationRequest = function () {
                try {

                    var userKey = "";

                    if (localStorage["icUserKeyGUID"] != undefined && localStorage["icUserKeyGUID"] != null && localStorage["icUserKeyGUID"] != "" ) {
                        userKey = localStorage["icUserKeyGUID"];
                    }
                    else {
                        promptForReload("SendRegisterationRequest");
                        return;
                    }
                    //self.SignalRHub.server.keepAlive();
                    console.info("##SR# SignalR self.SignalRHub.client.SendRegisterationRequest localStorage[\"icUserKeyGUID\"] = " + localStorage["icUserKeyGUID"]);
                    self.SignalRHub.server.registerNewUser(userKey);
                    console.info("##SR# SignalR SignalRHub.server.RegisterNewUser(localStorage[\"icUserKeyGUID\"]) = " + localStorage["icUserKeyGUID"]);

                } catch (e) {
                    console.info("##SR# SignalR  SendRegisterationRequest ## Exception: " + e.message);
                }

            };

            //Server gives token to the client using this method after registeration request is received by server hub
            self.SignalRHub.client.KeepToken = function (token) {
                try {
                    //self.SignalRHub.server.keepAlive();
                    if (token == undefined || token == null || token == "") {
                        console.info("##SR# SignalR No token Received from server!");
                        promptForReload("KeepToken");
                        return;
                    }

                    localStorage["SignalRToken"] = token;
                    console.info("##SR# SignalR SignalRHub.client.KeepToken " + token);
                } catch (e) {
                    console.info("##SR# SignalR  KeepToken ## Exception: " + e.message);
                }
            };

            //self.SignalRHub.client.sendKeepAlive = function () {
            //    debugger;
            //    setInterval(self.SignalRHub.server.keepAlive(), 5000);
            //};

            self.SignalRHub.client.KeepAlive = function (message) {
                //debugger;
                console.info("##SR# " + message);
                //setTimeout(function () { self.SignalRHub.server.keepAlive(); }, 120000);
            };

            //// Server asks client to send token to the server using this method,
            ////if the client does not have the token it asks server to register the client,
            ////if the client has the token already, the client asks server to update the client's binding
            self.SignalRHub.client.SendToken = function () {
                try {

                    var userKey = "";
                    var token = "";

                    if (localStorage["SignalRToken"] != undefined && localStorage["SignalRToken"] != null) {
                        var token = localStorage["SignalRToken"];
                    }

                    if (localStorage["icUserKeyGUID"] != undefined && localStorage["icUserKeyGUID"] != null) {
                        userKey = localStorage["icUserKeyGUID"];
                    }
                    else {
                        promptForReload("SendToken , reason icUserKeyGUID");
                        return;
                    }

                    if (userKey != "" && token != "") {
                        console.info("##SR#  SignalR ,  SignalRHub.server.UpdateUserWithKey(token, localStorage[\"icUserKeyGUID\"])" + token + ', ' + localStorage["icUserKeyGUID"]);
                        self.SignalRHub.server.updateUserWithKey(token, userKey);
                    }
                    else if (token == "" && userKey != "") {
                        console.info("##SR#  SignalR ,  SignalRHub.server.RegisterNewUser(localStorage[\"icUserKeyGUID\"]); " + localStorage["icUserKeyGUID"]);
                        self.SignalRHub.server.registerNewUser(userKey);
                    }
                    else if (userKey == "" && token != "") {
                        console.info("##SR#  SignalR ,  SignalRHub.server.updateUser(token)" + token);
                        self.SignalRHub.server.updateUser(token);
                    }
                    else {
                        promptForReload("SendToken , reason no scenario fits.");
                    }

                } catch (e) {
                    console.info("##SR#  SignalR ,   SendToken ## Exception: " + e.message + " " + e.number);
                }
            };


            //Server uses this method to signal client that it has got an update,
            //TYhe client updates the counts using Knock Out viewmodel
            signalrMainObj.SignalRHub.client.UpdateQueueCounts = function (message) {
                try {
                    //If the phonebar is off then return
                    if (localStorage["isSessionStarted"] === undefined || localStorage["isSessionStarted"] === null)
                        return;
                    else if (localStorage["isSessionStarted"] != '1') {
                        return;
                    }

                    console.info("##Q# SignalR " + message);

                    window.qpVM = new queuePersonalViewModel(window.mainVM, null);
                    //window.qpVM.FillPersonalCallsList('pending');
                    window.qpVM.PersonalCountByStatus('pending');

                    //Call Ajax and update the UI as per the method
                    TempAPI.getPersonalCounterByStatus('pending', getCountsSuccess, getCounterError);
                    function getCountsSuccess(response) {
                        try {
                            var PersonalButton = document.getElementById('PersonalQueueButton');

                            PersonalButton.innerHTML = response.Count;
                            if (response.Count > 0) {
                                PersonalButton.className = "btnGALQueue";
                                PersonalButton.disabled = false;
                            } else {
                                PersonalButton.className = "btnGALDisabled";
                                PersonalButton.disabled = true;
                                $('#PersonalQueueBackground').hide();
                                $('#PersonalQueueDisplay').hide();
                            }

                            console.info("##Q# SR PersonalCountByStatus - onPersonalCountSuccess, count is: " + response.Count);
                        } catch (e) {
                            console.info("##Q# SR EXCEPTION PersonalCountByStatus - onPersonalCountSuccess  " + e.message);
                        }
                    }
                    function getCounterError(response) {
                        console.info("##Q# SR PersonalCountByStatus - getCounterError");
                    }
                } catch (e) {
                    console.info("##Q# SignalR  UpdateQueueCounts ## Exception: " + e.message);
                }
            };

            signalrMainObj.SignalRHub.client.updateAcdCounts = function (msg) {
                try {

                    //If the phonebar is off then return
                    if (localStorage["isSessionStarted"] === undefined || localStorage["isSessionStarted"] === null)
                        return;
                    else if (localStorage["isSessionStarted"] != '1') {
                        return;
                    }

                    console.info("##A# SignalR " + msg);

                    window.acdVM = new queueAcdViewModel();
                    window.acdVM.GetAcdPendingCalls();

                    TempAPI.getAcdPendingCalls(getCountsSuccess, getCounterError);
                    function getCountsSuccess(response) {

                        try {
                            if (response != null && response != undefined) {
                                var acdQButton = document.getElementById('btnAcdQueue');
                                var acdTakenButton = document.getElementById('btnAcdTaken');

                                acdQButton.innerHTML = response.AcdCount;
                                acdTakenButton.innerHTML = response.AcdCallTaken;

                                if (response.AcdCount > 0 && response.IsEnabled) {
                                    acdQButton.className = "btnACDQueue";
                                    acdQButton.disabled = false;
                                    acdQButton.title = response.Reason;
                                } else {
                                    acdQButton.className = "btnACDDisabled";
                                    acdQButton.title = response.Reason;
                                    acdQButton.disabled = true;
                                }


                                if (parseInt(response.AcdCount) < 100)
                                    acdQButton.style.fontSize = '40';
                                else if (parseInt(response.AcdCount) > 99 && parseInt(response.AcdCount) < 1000)
                                    acdQButton.style.fontSize = '25px';
                                else
                                    acdQButton.style.fontSize = '20px';

                                if (parseInt(response.AcdCallTaken) < 100)
                                    acdTakenButton.style.fontSize = '40px';
                                else if (parseInt(response.AcdCallTaken) > 99 && parseInt(response.AcdCallTaken) < 1000)
                                    acdTakenButton.style.fontSize = '25px';
                                else
                                    acdTakenButton.style.fontSize = '20px';



                                console.info("##A# SR getAcdPendingCalls - getCountsSuccess, count is: " + response.AcdCount);
                            }
                        } catch (e) {
                            console.info("##A# SR EXCEPTION getAcdPendingCalls - getCountsSuccess  " + e.message);
                        }
                    }
                    function getCounterError(response) {
                        console.info("##A# SR getAcdPendingCalls - getCounterError");
                    }

                    console.info("##A#  SignalR ACD,   getAcdPendingCalls " + msg);
                } catch (e) {
                    console.info("##A#  SignalR ACD,   getAcdPendingCalls ## Exception: " + e.message);
                }
            };


            signalrMainObj.SignalRHub.client.updateDialerCounts = function (msg) {
                try {
                    console.info("##G# SignalR " + msg);

                    //gwbVM = new mainDialerViewModel();
                    //gwbVM.GALDialerQueuesVM.updateCounts();
                    ////ko.applyBindings(gwbVM);
                    //return;


                    TempAPI.getCounts(getCountsSuccess, getCounterError);

                    function getCountsSuccess(response) {
                        try {
                            if (response != null && response != undefined) {
                                var AvailableButton = document.getElementById('btnWebGalAvailable');
                                var DialedButton = document.getElementById('btnWebGalDialed');

                                //AvailableButton.innerHTML = response.total_assignable_leads;
                                DialedButton.innerHTML = response.total_assigned_leads;

                                if (localStorage["WebGALDialedALead"] == "true") {
                                    AvailableButton.innerHTML = "No Lead";
                                    AvailableButton.className = "";
                                    AvailableButton.className = "AvailableButtonNoLead";
                                    AvailableButton.title = "You Recently Dialed a Lead";
                                    AvailableButton.disabled = true;
                                } else if (localStorage["WebGALClickedDial"] == "true") {
                                    AvailableButton.innerHTML = "Queued";
                                    AvailableButton.title = "Request is Queued";
                                    AvailableButton.disabled = true;
                                    AvailableButton.className = "";
                                    AvailableButton.className = "ButtonClicked";
                                }
                                else if (response.total_assignable_leads > 0 && response.IsEnabled) {
                                    AvailableButton.innerHTML = "Take Lead";
                                    AvailableButton.className = "";
                                    AvailableButton.className = "AvailableButton";
                                    AvailableButton.disabled = false;
                                    AvailableButton.title = "Take a Lead";

                                } else if (response.total_assignable_leads == 0) {
                                    AvailableButton.innerHTML = "No Lead";
                                    AvailableButton.className = "";
                                    AvailableButton.className = "AvailableButtonNoLead";
                                    AvailableButton.title = "No Leads Available";
                                    AvailableButton.disabled = true;
                                } else {
                                    AvailableButton.innerHTML = "No Lead";                                    
                                    AvailableButton.className = "";
                                    AvailableButton.className = "AvailableButtonNoLead";
                                    AvailableButton.disabled = true;
                                    //reason
                                    if (response.Reason != undefined && response.Reason != null && response.Reason.length > 2) {
                                        AvailableButton.title = response.Reason;
                                    }
                                    else {
                                        AvailableButton.title = "No Leads Available";
                                    }
                                }

                                if (parseInt(response.total_assigned_leads) < 100)
                                    DialedButton.style.fontSize = '28px';
                                else if (parseInt(response.total_assigned_leads) > 99 && parseInt(response.total_assigned_leads) < 1000)
                                    DialedButton.style.fontSize = '20px';
                                else
                                    DialedButton.style.fontSize = '14px';

                                console.info("##G# SR updateDialerCounts - getCountsSuccess, count is: " + response.total_assignable_leads);
                            }

                            window.gwbVM = new GALDialerQueuesViewModel();
                            window.gwbVM.updateCounts();

                        } catch (e) {
                            console.info("##G# SR EXCEPTION updateDialerCounts - getCountsSuccess  " + e.message);
                        }
                    }
                    function getCounterError(response) {
                        console.info("##G# SR updateDialerCounts - getCounterError");
                    }

                    console.info("##G#  SignalR GWB,   updateDialerCounts " + msg);
                } catch (e) {
                    console.info("##G#  SignalR GWB,   updateDialerCounts ## Exception: " + e.message);
                }
            };

            self.SignalRHub.client.LogServerError = function (message) {
                console.info("##SR# SignalR self.SignalRHub.client.LogServerError" + message);
            };



            self.SignalRHub.client.ping = function () { return (new Date()).getTime(); };
            self.SignalRHub.client.messageReceived = function (msg, args) {
                console.info("self.SignalRHub.client.messageReceived msg: " + msg + "args: " + args);
            };

       
            //$.connection.hub.connectionSlow(function () {
            //    console.error("##SR# SignalR Connection is Slow. Message by $.connection.hub.connectionSlow");
            //});

            //var tryingToReconnect = false;

            //$.connection.hub.reconnecting(function () {
            //    console.info("##SR# SignalR $.connection.hub.reconnecting");
            //    tryingToReconnect = true;
            //});

            //$.connection.hub.reconnected(function () {
            //    console.info("##SR# SignalR $.connection.hub.reconnected");
            //    tryingToReconnect = false;
            //});

            //$.connection.hub.disconnected(function () {
            //    console.info("##SR# SignalR $.connection.hub.disconnected called, tryingtoreconnect is : " + (tryingToReconnect ? "true" : "false"));
            //    if (tryingToReconnect) {
            //        console.info("##SR# SignalR $.connection.hub.disconnected");
            //        //promptForReload();
            //    }
            //});

            $.connection.hub.logging = true;

            //SignalR's method that starts the Hub
            $.connection.hub.start({ jsonp: true })
            //$.connection.hub.start()
                .done(function () {
                    console.info("##SR# SignalR   Started SignalR Hub connection successfully! " + $.connection.hub);
                    localStorage["reAttemptCount"] = 0;
                })

                .fail(function (reason) {                
                    $.connection.hub.connection;
                    console.info("##SR# SignalR   Starting SignalR Hub connection #Failed# " + reason);
                    promptForReload("hub Start failed.");
                });

            $.connection.hub.error(function (error) {
                console.info("##SR# SignalR   SignalR error: " + error);                
                //self.parent.personalSignalRInit();
            });
        }
        catch (ex) {
            //debugger; alert("## Exception: " + ex.message);
            console.info("##SR# SignalR   ## Exception: " + ex.message);
            //self.parent.personalSignalRInit();
        }
    };

    function promptForReload(method) {

        //if (confirm("One of the servers could not be connected, please press 'OK' to reload the page. \n Press 'Cancel' to avoid reload; the GAL counts and Inbound call queues may not update in a timely manner.")) {
        //    window.location.href = window.location.href;
        //}

        console.info("##SR The reattempt to connect to SignalR exhausted by count. There was an issue with signalR connectivity or data. Method name: " + method);

        //try {
        //    var count = 0;
        //    count = parseInt(localStorage["reAttemptCount"]) || 0;

        //    if (count >= 5) {
        //        console.error("##SR The reattempt to connect to SignalR exhausted by count. SignalR could not be started.")
        //    }
        //    signalrMainObj.initSignalR();
        //    console.info("##SR reattempting signalR init, attempt count: " + count);

        //    count++;
        //    localStorage["reAttemptCount"] = count;

        //} catch (e) {
        //    console.info("##SR Exception, promptForReload called from method: " + method + "Exception message: " + e.message);
        //}
    }

    function sendRequest(url, requestType, dataType, onSuccess, onError) {
        $.ajax({
            url: url,
            dataType: dataType,
            type: requestType,
            success: onSuccess,
            error: onError
        });
    }

    var TempAPI = {
        getPersonalCounterByStatus: function getPersonalCount(status, onSuccess, onError) {
            sendRequest('/Services/v2.0/PhoneQueuePersonal.ashx?method=GetCountByStatus&status=' + status + '&inboundSkillId=' + auth.InboundSkillId, 'POST', 'json', onSuccess, onError)
        },
        getAcdPendingCalls: function (onSuccess, onError) {
            sendRequest('/Services/v2.0/PhoneQueueAcd.ashx?method=GetPendingCalls', 'POST', 'json', onSuccess, onError)
        },
        getCounts: function (onSuccess, onError) {
            sendRequest('/Services/GALDialer.ashx?method=GetCounts&agentId=' + localStorage["icUserKeyGUID"], 'POST', 'json', onSuccess, onError)
        }
    }

}