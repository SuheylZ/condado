//MH:26 March 2014
function Logger() {
    var self = this;
    self.isEnabled = false;
    self.info = function (msg) {
        if (self.isEnabled)
            console.info("%c" + msg, "color:blue");
    };
    self.log = function (msg) {
        if (self.isEnabled)
            console.info("%c" + msg, "color:black");
    };
    self.success = function (msg) {
        if (self.isEnabled)
            console.info("%c" + msg, "color:green");
    };
    self.warn = function (msg) {
        if (self.isEnabled)
            console.warn(msg);
    };
    self.error = function (msg) {
        if (self.isEnabled)
            console.error(msg);
    };
    self.errorInfo = function (msg) {
        if (self.isEnabled)
            console.info("%c" + msg, "color:red");
    };
};
//loger = new Logger();

function confirmDelete() {

    return confirm('Are you sure to delete the record?');
}

function validate() {
    // start wm
    //return Page_ClientValidate();

    var r = Page_ClientValidate();

    if (r) {
        setChangeFlag('0');
    }

    return r;

    // end wm
}
function validateGroup(argGroupName) {
    //return Page_ClientValidate(argGroupName);

    // start wm
    //return Page_ClientValidate(argGroupName);

    var r = Page_ClientValidate(argGroupName);

    if (r) {
        setChangeFlag('0');
    }

    return r;

    // end wm

}

//function SQConfirm(message) {
//        var bResult=false;

//        alert("inside SQConfirm");
//        jConfirm("Do you want to continue?", "SQ Sales Tool", inner);

//        function inner(arg) {
//            if (arg == true){
//                bResult = true;
//             }
//            return bResult;
//        }
//    }

// Start wm

function setChangeFlag(v) {
    $('#dirtyFlag').val(v);
}

//    function confirmBox() {

//        if ($('#dirtyFlag').val() == "0") {
//            return true;
//        }

//        var r = confirm("You have unsaved changes, are you sure to leave without saving?");
//        
//        if (r) {
//            setChangeFlag('0');
//        }

//        return r;
//    }

function onClickYes() {
    setChangeFlag('0');

    return true;
}

function bindEvents() {
    
    $(document).ready(function () {
      
        $(".phone").focus(onFieldFocus);
        $(".phone").blur(onFieldBlur);

        $(".getText").click(function (e) {
            return closeDlg($(this).text());
        });

        $(".returnButton").click(function (e) {
            return confirmBox();
        });

        $(".resetChangeFlag").click(function (e) {
            setChangeFlag('0');
        });

        $("input,select,textarea").change(function (e) {
            if ($(this).attr("id") != undefined) {
                if ($(this).attr("id").search("txtSearch") != -1
                    || $(this).attr("id").search("txtPageSize") != -1
                    || $(this).attr("id").search("txtPageNumber") != -1) {

                    return;
                }

                setChangeFlag('1');
            }
        });

        //            $("input[type=radio]").click(function (e) {
        //                $('input').change();
        //            });

    });
}

//functions used for applying input mask and validation on phone no. and fax fields
function onFieldFocus() {

    // apply mask with all values optional to prevent invalid input being removed by jquery plugin
    $(this).mask("?9999999999", { placeholder: "" });
}

function onFieldBlur() {

    var el = $(this);
    var inputValue = jQuery.trim(el.val());
    el.val(inputValue); // set value with no spaces

    if (inputValue.length == 10) {
        //if input is of valid length then apply mask
        el.mask("(999) 999-9999", { placeholder: "_" });
    }
}

function validatePhone(sender, arguments) {
    //get text box id from controltovalidate property of sender(custom validator)

    var elId = $("#" + sender.id).attr('controltovalidate');
    var element = $("#" + elId);

    if (element) {
        var inputValue = element.val();

        inputValue = jQuery.trim(inputValue); //remove white space before and after
        inputValue = inputValue.replace(/ /g, ''); //remove spaces in between
        inputValue = inputValue.replace("(", "");
        inputValue = inputValue.replace(")", "");
        inputValue = inputValue.replace("-", "");

        // phone number is valid if length is 10 or 0
        if ((inputValue.length == 0) || (inputValue.length == 10)) {
            arguments.IsValid = true;
            element.mask("(999) 999-9999", { placeholder: "_" });
        }
        else {
            arguments.IsValid = false;
            element.val(jQuery.trim(element.val()));
        }
        return;
    }
    // if element not found then let validation pass
    arguments.IsValid = true;
}

// End wm


