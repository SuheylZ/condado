function DialerSignalRViewModel(parent, onError) {
    //debugger;
    var self = this;
    self.parent = parent;

    // Declare a proxy to reference the hub.
    self.GWBSignalRHub = $.connection.dialerHub;


    self.initSignalR = function init() {
        try {
            //Assign the URL of SignalR hub
            jQuery.support.cors = true;
            $.connection.hub.url = (localStorage["SignalRurl"] !== null && localStorage["SignalRurl"] !== undefined) ? localStorage["SignalRurl"] : "";

            //Server gives token to the client using this method
            self.GWBSignalRHub.client.KeepToken = function (token) {
                try {
                    if (token == undefined || token == null) {
                        console.info("##G# SignalR GWB,  No token Received from server!");
                    }
                    localStorage["SignalRToken"] = token;
                    console.info("##G# SignalR GWB,  SignalRHub.client.KeepToken " + token);
                } catch (e) {
                    console.info("##G# SignalR GWB,   KeepToken ## Exception: " + e.message);
                }
            };


            // Server asks client to send token to the server using this method,
            //if the client does not have the token it asks server to register the client,
            //if the client has the token already, the client asks server to update the client's binding
            self.GWBSignalRHub.client.SendToken = function () {
                try {
                    var token = localStorage["SignalRToken"];


                    if (localStorage["SignalRToken"] == undefined) {
                        //debugger; alert(" SignalRHub.server.RegisterNewUser(userName); " + localStorage["username"]);
                        console.info("##G# SignalR GWB,  SignalRHub.server.RegisterNewUser(localStorage[\"icUserKeyGUID\"]); " + localStorage["icUserKeyGUID"]);
                        self.GWBSignalRHub.server.registerNewUser(localStorage["icUserKeyGUID"]);
                    }
                    else {
                        if (localStorage["icUserKeyGUID"] == undefined || localStorage["icUserKeyGUID"] == null || localStorage["icUserKeyGUID"] == "") {
                            console.info("##G# SignalR GWB,  SignalRHub.server.updateUser(token)" + token);
                            self.GWBSignalRHub.server.updateUser(token);
                        } else {
                            console.info("##G# SignalR GWB,  SignalRHub.server.updateUserWithKey(token, localStorage[\"icUserKeyGUID\"])" + token + ', ' + localStorage["icUserKeyGUID"]);
                            self.GWBSignalRHub.server.updateUserWithKey(token, localStorage["icUserKeyGUID"]);
                        }
                    }
                } catch (e) {
                    console.info("##G# SignalR GWB,   SendToken ## Exception: " + e.message);
                }
            };

            self.GWBSignalRHub.client.SendRegisterationRequest = function () {

                try {
                    console.info("##G# SignalR GWB,  self.GWBSignalRHub.client.SendRegisterationRequest");
                    self.GWBSignalRHub.server.registerNewUser(localStorage["icUserKeyGUID"]);
                    console.info("##G# SignalR GWB,  SignalRHub.server.RegisterNewUser(localStorage[\"icUserKeyGUID\"]); " + localStorage["icUserKeyGUID"]);
                } catch (e) {
                    console.info("##G# SignalR GWB,   SendRegisterationRequest ## Exception: " + e.message);
                }

            };

            self.GWBSignalRHub.client.updateDialerCounts = function (msg) {
                //debugger;
                try {
                    parent.GALDialerQueuesVM.updateCounts();
                    console.info("##G# SignalR GWB,   updateDialerCounts " + msg);
                } catch (e) {
                    console.info("##G# SignalR GWB,   updateDialerCounts ## Exception: " + e.message);
                }
            };

            self.GWBSignalRHub.client.updateSettings = function (msg) {
                //debugger;
                try {
                    parent.GALDialerQueuesVM.getUserData();
                    console.info("##G#  SignalR GWB,   updateSettings " + msg);
                } catch (e) {
                    console.info("##G#  SignalR GWB,   updateSettings ## Exception: " + e.message);
                }
            };

            //self.unregisterFromSignalRNotifications = function () {
            //    try {
            //        if (localStorage["SignalRToken"] != undefined || localStorage["SignalRToken"] != "") {
            //            var token = localStorage["SignalRToken"];

            //            self.GWBSignalRHub.server.unregisterUser(token);
            //            console.info("##G# SignalR GWB,   SignalRHub.server.unregisterUser(token); " + token);
            //            localStorage.removeItem("SignalRToken");
            //        }
            //    } catch (e) {
            //        console.info("##G# SignalR GWB,   unregisterFromSignalRNotifications ## Exception: " + e.message);
            //    }
            //}

            //SignalR's method that starts the Hub
            self.GWBSignalRHub.start({ jsonp: true })
            //$.connection.hub.start({ jsonp: true })
                .done(function () {
                    //debugger; alert("#Started SignalR Hub connection successfully!");
                    console.info("##G# SignalR GWB,    Started SignalR Dialer  Hub connection successfully!");
                })

                .fail(function (reason) {
                    //debugger;
                    $.connection.hub.connection;
                    self.parent.initSignalR();
                    console.info("##G# SignalR GWB,    Starting SignalR Dialer Hub connection #Failed# " + reason);
                });

            $.connection.hub.error(function (error) {
                console.info("##G# SignalR GWB,    SignalR error: " + error)
            });

            self.GWBSignalRHub.client.LogServerError = function (message) {
                console.info("##G# SignalR GWB,  self.GWBSignalRHub.client.LogServerError" + message);
            };
        }
        catch (ex) {
            //debugger; alert("## Exception: " + ex.message);
            console.info("##G# SignalR GWB   ## Exception: " + ex.message);
        }
    };
}
