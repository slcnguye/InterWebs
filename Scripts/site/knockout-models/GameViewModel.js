(function() {
    namespace("IW.All").GameViewModel = function (object) {
        var self = this;

        self.user = object.user;
        self.signalR = $.connection.gameHub;
        self.activeGameUsersView = new IW.All.ActiveGameUsersView({
            signalRClient: self.signalR.client,
            user: self.user
        });

        self.gameBoxView = new IW.All.GameBoxView({
            signalRClient: self.signalR.client,
            signalRServer: self.signalR.server,
            user: self.user
        });

        self.random = function (max) {
            return Math.floor(Math.random() * max);
        }

        self.cards = object.cards;
        self.cardPath = object.cardPath;
        self.changeCard = function (ele, event) {
            var imgSrc = event.target;
            var randomCard = self.random(self.cards.length);
            imgSrc.src = self.cardPath + self.cards[randomCard];
        }

        $.connection.hub.start().done(
            function() {
                self.signalR.server.joinGame("All", self.user);
            }
        );

        $(window).bind('beforeunload', function() {
            self.signalR.server.leaveGame("All", self.user);
        });
    };

    namespace("IW.All").GameBoxView = function (object) {
        var self = this;
        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
    };

    namespace("IW.All").ActiveGameUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([object.user]);

        self.signalRClient.usersInGame = function (gameName, users) {
            if ("All" == gameName) {
                self.activeUsers.push(users);
            }
        }

        self.signalRClient.userJoinedGame = function (gameName, username) {
            if ("All" == gameName) {
                self.activeUsers.push(username);
            }
        }

        self.signalRClient.userLeftGame = function (gameName, username) {
            var activeUsers = [];
            self.activeUsers().forEach(function (user) {
                if (user != username) {
                    activeUsers.push(user);
                }
            });
            self.activeUsers(activeUsers);
        }
    };
}());