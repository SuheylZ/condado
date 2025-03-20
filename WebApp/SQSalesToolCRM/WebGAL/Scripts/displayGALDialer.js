$(document).ready(function () {
    var mainDialerVM = new mainDialerViewModel();
    ko.applyBindings(mainDialerVM, document.getElementById("divGetALeadDialer"));
    mainDialerVM.init();
});