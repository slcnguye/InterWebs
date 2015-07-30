(function () {
    namespace("IW.Project.Tournament").Match = function () {
        var self = this;

        self.player1 = ko.observable();
        self.player2 = ko.observable();
        self.player3 = ko.observable();
        self.player4 = ko.observable();

        self.matchReady = function () {
            return !(!self.player1() || !self.player2() || !self.player3() || !self.player4());
        };

        self.onePlayerLeft = function() {
            var playersReady = 0;

            if (self.player1()) {
                playersReady++;
            }

            if (self.player2()) {
                playersReady++;
            }

            if (self.player3()) {
                playersReady++;
            }

            if (self.player4()) {
                playersReady++;
            }

            return playersReady === 3;
        }
    };
}());