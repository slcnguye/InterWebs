namespace("IW.Project.Tournament").EnterPlayersViewModel = function(app) {
    var self = this;

    self.tournamentName = ko.observable(app.dataModel.tournamentName);
    self.lastMatch = ko.observable(new IW.Project.Tournament.Match());
    self.matches = ko.observableArray([self.lastMatch()]);

    self.addNewMatch = ko.computed(function () {
        if (self.lastMatch().onePlayerLeft()) {
            var copyMatch = new IW.Project.Tournament.Match();
            copyMatch.player1(self.lastMatch().player1());
            copyMatch.player2(self.lastMatch().player2());
            copyMatch.player3(self.lastMatch().player3());
            copyMatch.player4(self.lastMatch().player4());

            self.lastMatch().player1("");
            self.lastMatch().player2("");
            self.lastMatch().player3("");
            self.lastMatch().player4("");

            self.matches.splice(self.matches().length - 1, 0, copyMatch);
        }
    });
};
