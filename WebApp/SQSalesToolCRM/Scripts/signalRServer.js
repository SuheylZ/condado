/*!
 * ASP.NET SignalR JavaScript Library v2.1.1
 * http://signalr.net/
 *
 * Copyright Microsoft Open Technologies, Inc. All rights reserved.
 * Licensed under the Apache 2.0
 * https://github.com/SignalR/SignalR/blob/master/LICENSE.md
 *
 */

/// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
/// <reference path="jquery.signalR.js" />
(function ($, window, undefined) {
    /// <param name="$" type="jQuery" />
    "use strict";

    if (typeof ($.signalR) !== "function") {
        throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
    }

    var signalR = $.signalR;

    function makeProxyCallback(hub, callback) {
        return function () {
            // Call the client hub method
            callback.apply(hub, $.makeArray(arguments));
        };
    }

    function registerHubProxies(instance, shouldSubscribe) {
        var key, hub, memberKey, memberValue, subscriptionMethod;

        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!(hub.hubName)) {
                    // Not a client hub
                    continue;
                }

                if (shouldSubscribe) {
                    // We want to subscribe to the hub events
                    subscriptionMethod = hub.on;
                } else {
                    // We want to unsubscribe from the hub events
                    subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                    if (hub.client.hasOwnProperty(memberKey)) {
                        memberValue = hub.client[memberKey];

                        if (!$.isFunction(memberValue)) {
                            // Not a client hub function
                            continue;
                        }

                        subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                    }
                }
            }
        }
    }

    $.hubConnection.prototype.createHubProxies = function () {
        var proxies = {};
        this.starting(function () {
            // Register the hub proxies as subscribed
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, true);

            this._registerSubscribedHubs();
        }).disconnected(function () {
            // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, false);
        });

        proxies['selectCareHub'] = this.createHubProxy('selectCareHub');
        proxies['selectCareHub'].client = {};
        proxies['selectCareHub'].server = {
            isConnected: function () {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["isConnected"], $.makeArray(arguments)));
            },

            keepAlive: function () {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["keepAlive"], $.makeArray(arguments)));
            },

            register: function (type, gid) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["register"], $.makeArray(arguments)));
            },

            registerNewUser: function (userKey) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["RegisterNewUser"], $.makeArray(arguments)));
            },

            send: function (name, message) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["send"], $.makeArray(arguments)));
            },

            sendMessage: function (command, args) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["sendMessage"], $.makeArray(arguments)));
            },

            updateUser: function (Token) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["UpdateUser"], $.makeArray(arguments)));
            },

            updateUserWithKey: function (Token, Key) {
                return proxies['selectCareHub'].invoke.apply(proxies['selectCareHub'], $.merge(["UpdateUserWithKey"], $.makeArray(arguments)));
            }
        };

        return proxies;
    };

    signalR.hub = $.hubConnection("/signalr", { useDefaultPath: false });
    $.extend(signalR, signalR.hub.createHubProxies());

}(window.jQuery, window));