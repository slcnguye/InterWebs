namespace("IW.Project.Tournament").GameTypeViewModel = function(app) {
    var self = this;

    self.tournamentName = ko.observable("");

    self.createTournament = function() {
        app.dataModel.tournamentName = self.tournamentName();
        app.navigateToEnterPlayers();
    }
};
