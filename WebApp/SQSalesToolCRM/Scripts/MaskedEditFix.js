// Fixes issue with delete key not working on Ipad browsers.
// Source: http://ajaxcontroltoolkit.codeplex.com/workitem/26978

(function () {
    
    try {
        var p = Sys.Extended.UI.MaskedEditBehavior.prototype,
            en = p._ExecuteNav;

        p._ExecuteNav = function (e) {
            var type = e.type;
            if (type == 'keydown') { e.type = 'keypress' };
            en.apply(this, arguments);
            e.type = type;
        }

    } catch (e) {
        return;
    }

})();