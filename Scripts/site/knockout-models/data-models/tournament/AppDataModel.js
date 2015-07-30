(function () {
    namespace("IW.Project.Tournament").AppDataModel = function() {
        var self = this,
            // Routes
            submitLaborMetricsUrl = "/webclock/SubmitLaborMetrics";

        self.tournamentName = "";

        self.submitLaborMetrics = function(lmExternalRefs) {
            return $.ajax(submitLaborMetricsUrl, {
                type: "POST",
                data: { lmExternalRefs: lmExternalRefs }
            });
        };
    };
}());
