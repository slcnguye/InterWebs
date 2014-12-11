(function() {
    namespace("IW.All").ProjectsViewModel = function () {
        var self = this;
        
        self.adjustAnchor = function(context, event) {
            event.preventDefault();
            var target = $(event.target);
            var getHref = target.attr("href").split("#")[1];
            var offsetSize = 75;
            $(window).scrollTop($("[id*='" + getHref + "']").offset().top - offsetSize);
        }
    };
}());