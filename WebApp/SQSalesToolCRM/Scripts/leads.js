// leads.js
// this ocntains all the fucntions that are called from the leads page.
// SZ [Feb 01, 2014]
// should be included by leads page only

var objArcHub = $.connection.selectCareHub;

function initArcHub() {
    // Pulling User information 
    $.ajax({
        url: '/Services/GALDialer.ashx?method=GetUserData',
        dataType: 'json',
        success: function (response) {
                        
            localStorage["icUserKeyGUID"] = (response.agentKey.length > 30 ?
                response.agentKey :
                ((localStorage["icUserKeyGUID"] == undefined || localStorage["icUserKeyGUID"] == null) ?
                "" : localStorage["icUserKeyGUID"]));

            localStorage["SignalRurl"] = response.SignalRurl.length > 3 ?
                response.SignalRurl
                : ((localStorage["SignalRurl"] != undefined) ?
                    localStorage["SignalRurl"]
                    : "");

            signalrMainObj.initSignalR();

            //this.objArcHub = $.connection.selectCareHub;
        }
    });

    
    //$.connection.hub.url = "http://localhost:8080/signalr";
    //hub = $.connection.arcHub;
    //hub.qs = { 'version': '1.0', 'userid': getGuid() };
    //$.connection.hub.qs = { 'version': '1.0', 'userid': getGuid() };
    //$.connection.hub.start().wait().done(function () { });

    //objArcHub.client.messageReceived = function (msg, args) {
    //    // Do logging
    //};
    //objArcHub.client.ping = function () { return (new Date()).getTime(); };

    //hub.sendMessage = function (cmd, args) { hub.server.sendMessage(cmd, args); };
    //hub.isConnected = function () { return hub.server.isConnected(); };
}

function QuickSave() {
    var bAns = false;
    var aid = 0;
    try {
        $.ajax({
            url: GetSelectCareServiceURL('SaveDataForArc'),
            type: "POST",
            dataType: "json",
            data: GetStringifyDataForArc(),
            contentType: "application/json; charset=utf-8",
            success: function (arg) {
                var ans = JSON.parse(arg.d);
                aid = ans;
                if (aid > 0)
                    SetAccountId(aid);
                bAns = (aid > 0);
            },
            error:
                function (xhr, textStatus, errorThrown) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                    bAns = false;
                },
            async: false
        });
    }
    catch (ex) { }
    return aid;
}

function IsDataValid() {
    var bAns = false;
    try {
        $.ajax({
            url: GetSelectCareServiceURL('IsDataValid'),
            type: "POST",
            dataType: "json",
            data: GetStringifyDataForArc(),
            contentType: "application/json; charset=utf-8",
            success: function (arg) {
                var ans = JSON.parse(arg.d);
                bAns = (ans === true);
            },
            error:
                function (xhr, textStatus, errorThrown) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                },
            async: false
        });
    }
    catch (ex) { }
    return bAns;
}

function IsDataSame() {
    var bExit = false;
    try {
        $.ajax({
            url: GetSelectCareServiceURL('IsDataSame'),
            type: "POST",
            dataType: "json",
            data: GetStringifyDataForArc(),
            contentType: "application/json; charset=utf-8",
            success: function (arg) {
                var ans = JSON.parse(arg.d);
                bExit = (ans === true);
            },
            error:
                function (xhr, textStatus, errorThrown) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                },
            async: false
        });
    }
    catch (ex) { }
    return bExit;
}

function redirectToAccount(accountId) {
    var url = window.location.href;
    url = url.split("?")[0].split("#")[0] + "?accountid=" + accountId;
    window.location.href = url;
}

function newArcCall(accId) {
    var bExit = false;
    try {

        //this.objArcHub.server.register('crm', getGuid());
        //debugger;
        //MH:21 April prevention to save empty record
        if (!Page_ClientValidate("indpArc")) {
            return false;
        }
        //SZ  [Feb 5, 2015] prompt disabled by teh client request. performs direct save
        //if (!IsDataSame()) {
        //    if (confirm("The information present has changed. Do you want to save it first?"))
        //        bExit = !QuickSave();
        //}
        //if (bExit)
        //    return false;
        var accountId = QuickSave();
        //--------------------------------------
        //quick fix
        // SZ [09 May 2014] Yasir/Suheyl, on VSC02 we have had reports of New Arc Call returning the  "Required Information" error.  Muzammil said he fixed it, I wanted to confirm that it was fixed and see if one of you can push the changes to VSC02.


        //if (!IsDataValid()) {
        //    alert("Required information is either missing or invalid. Please provide the correct information");
        //    bExit = true;
        //}
        //if (bExit) {
        //    if (accountId > 0)
        //        redirectToAccount(accountId);
        //    return false;
        //}
        if (!IsArcNewImplementationToUse()) {
            $.ajax({
                url: GetSelectCareServiceURL('GetIndividualForArc'),
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ id: GetAccountId() }),
                contentType: "application/json; charset=utf-8",
                success: function (arg) {
                    //debugger;
                    var ans = JSON.parse(arg.d);
                    if (myArc != null) {
                        alert(ans.SourceCode + ", " +
                            ans.Title + ", " + ans.FirstName + ", " + ans.MiddleName + ", " + ans.LastName + ", " + ans.Suffix + ", " +
                             ans.Gender + ", " + ans.State + ", " + ans.Birthdate + ", " + ans.indv_key);
                        //SZ [Dec 16, 2013] Call Arc Fucntionality
                        myArc.NewCase(ans.SourceCode,
                            ans.Title, ans.FirstName, ans.MiddleName, ans.LastName, ans.Suffix,
                              ans.Gender, ans.State, ans.Birthdate, ans.indv_key
                              );
                        writeAccountLog('NewCase() called successfully');
                    }
                    else {
                        //SZ [Dec 16, 2013] Arc is not present, handle the error.
                        alert('Arc could not be found on your machine. Please install Arc system before using this fucntionality');
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                    //alert("Status: " + textStatus + " " + "Error: " + errorThrown);
                    writeAccountLog(errorThrown);
                }
            });
            if (accountId > 0)
                redirectToAccount(accountId);
        }
        else
        {
            this.objArcHub.server.register('crm', getGuid());
            if (accountId < 0) accountId = -accountId;
            this.objArcHub.server.sendMessage('newcase2', accountId);
            var currentAccountId = GetAccountId();
            if (currentAccountId != accountId)
                redirectToAccount(accountId);
        }
        
    }
    catch (ex) {
        alert(ex);
    }
    return false;
}

function openArc(arcReferenceid, accountId) {
    try {

        if (!IsArcNewImplementationToUse()) {
            if (myArc != null /*&& myArc.OpenCase!=undefined*/)
                //SZ [Dec 16, 2013] Call Arc Fucntionality
                myArc.OpenCase(id);
            else
                alert('Arc could not be found on your machine. Please install Arc system before using this fucntionality');            
        }
        else
        {
           this.objArcHub.server.register('crm', getGuid());
           this.objArcHub.server.sendMessage('opencase2', arcReferenceid);
        }        
        //YA[23 Sep, 2014] Redirect to arc referenced account
        var currentAccountId = GetAccountId();
        if (currentAccountId != accountId)
            redirectToAccount(accountId);
    }
    catch (ex) {
        alert("Error");
        writeAccountLog(ex.description);
    }
}


//function getActionTimer(btnId) {
//    var response;
//    var button = $('#'+btnId);
    
//    try {
//        $.ajax({
//            url: GetSelectCareServiceURL('getActionTimer'),
//            type: "GET",
//            dataType: "json",
//            data: getActionTimerData(),
//            contentType: "application/json; charset=utf-8",
//            success: function (arg) {
//                response = JSON.parse(arg.d);
//            },
//            error:
//                function (xhr, textStatus, errorThrown) {
//                    var err = eval("(" + xhr.responseText + ")");
//                    console.log(err);
//                },
//            async: false
//        });
//        if (response.valid === true && button != null) {
//            if (response.disable === true) {
//                button.attr('disabled', 'disabled');
//                setTimeout(function (){ button.removeAttr('disabled', 'disabled');});
//            }
//            else{
//                button.removeAttr('disabled', 'disabled');
//                setTimeout(function (){ button.attr('disabled', 'disabled');});
//            }
//        }
//        else
//            button.removeAttr('disabled');
//    }
//    catch (ex) {
//        console.log("failed to retrieve the action timer attributes. check the server connection");
//    }
//}

