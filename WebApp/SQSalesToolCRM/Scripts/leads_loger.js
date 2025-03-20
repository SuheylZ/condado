// leads.js
// this ocntains all the fucntions that are called from the leads page.
// SZ [Feb 01, 2014]
// should be included by leads page only

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
        
        if (loger.isEnabled && $.isFunction(console.group))
            console.group("New Arc Call");
        loger.info("Account Quick Saving");
        
        var accountId = QuickSave();
        loger.success("Account saved with Id " + accountId);
        loger.info("Validating ArcCase data");
        
        if (!IsDataValid()) {
            loger.warn("Required information is either missing or invalid. Please provide the correct information");
            alert("Required information is either missing or invalid. Please provide the correct information");
            bExit = true;
        }
        if (bExit) {
            if (accountId > 0)
                redirectToAccount(accountId);
            return false;
        }

        loger.info("Getting individual information for ArcCase from svc");
        $.ajax({
            url: GetSelectCareServiceURL('GetIndividualForArc'),
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ id: GetAccountId() }),
            contentType: "application/json; charset=utf-8",
            success: function (arg) {
                var ans = JSON.parse(arg.d);
                if (myArc != null) {
                    alert(ans.SourceCode + ", " +
                        ans.Title + ", " + ans.FirstName + ", " + ans.MiddleName + ", " + ans.LastName + ", " + ans.Suffix + ", " +
                        ans.Gender + ", " + ans.State + ", " + ans.Birthdate + ", " + ans.indv_key);
                    //MH:26 March 2014
                    loger.info("Calling NewCase OLE Implementation with data " + JSON.stringify(ans));
                    //SZ [Dec 16, 2013] Call Arc Fucntionality
                    try {
                        myArc.NewCase(ans.SourceCode,
                            ans.Title, ans.FirstName, ans.MiddleName, ans.LastName, ans.Suffix,
                            ans.Gender, ans.State, ans.Birthdate, ans.indv_key
                        );
                        loger.success("NewCase() called successfully");
                        writeAccountLog('NewCase() called successfully');
                    } catch (exc) {
                        //MH:26 March 2014
                        loger.errorInfo("NewCase failed it is either due to Non IE browser or ARC OLE not working, more details are given below");
                        loger.error(exc);
                        throw exc;
                    } if (loger.isEnabled && $.isFunction(console.groupEnd))
                        console.groupEnd("New Arc Call");
                }
                else {
                    //SZ [Dec 16, 2013] Arc is not present, handle the error.
                    alert('Arc could not be found on your machine. Please install Arc system before using this fucntionality');
                    //MH:26 March 2014
                    loger.error("Arc could not be found on your machine. Please install Arc system before using this fucntionality");
                    if (loger.isEnabled && $.isFunction(console.groupEnd))
                        console.groupEnd("New Arc Call");
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                var err = eval("(" + xhr.responseText + ")");
                alert(err.Message);
                //alert("Status: " + textStatus + " " + "Error: " + errorThrown);
                writeAccountLog(errorThrown);
                //MH:26 March 2014
                loger.error(errorThrown);
                if (loger.isEnabled && $.isFunction(console.groupEnd))
                    console.groupEnd("New Arc Call");
            }
        });
        if (accountId > 0)
            redirectToAccount(accountId);
    }
    catch (ex) {
        alert(ex);
        //MH:26 March 2014
        loger.error(ex);
        if (loger.isEnabled && $.isFunction(console.groupEnd))
            console.groupEnd("New Arc Call");
    }
    return false;
}

function openArc(id) {
    //MH:26 March 2014
    if (loger.isEnabled && $.isFunction(console.group))
        console.group("ARC OpenCase " + id);
    try {
        if (myArc != null /*&& myArc.OpenCase!=undefined*/)
            //SZ [Dec 16, 2013] Call Arc Fucntionality
        {
            myArc.OpenCase(id);
            loger.success("ARC OpenCase successfully");
        } else {
            alert('Arc could not be found on your machine. Please install Arc system before using this fucntionality');
            loger.error("Arc could not be found on your machine. Please install Arc system before using this fucntionality");
        }
    }
    catch (ex) {
        
        loger.errorInfo("OpenCase failed");
        loger.error(ex.description);
        writeAccountLog(ex.description);
        
    }
    if (loger.isEnabled && $.isFunction(console.groupEnd))
        console.groupEnd("ARC OpenCase " + id);
}