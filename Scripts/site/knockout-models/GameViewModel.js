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
            cards: object.cards,
            cardPath: object.cardPath,
            backCardPath: object.backCardPath,
            user: self.user
        });

        $.connection.hub.start({ jsonp: true }).done(
            function() {
                self.signalR.server.joinGame();
                self.gameBoxView.getPlayerHands();
            }
        );

        $(window).bind('beforeunload', function () {
            self.signalR.server.leavePage();
            if (self.gameBoxView.playing()) {
                self.gameBoxView.leaveGame();
            };
        });
    };

    namespace("IW.All").GameBoxView = function(object) {
        var self = this;
        self.join = "Join";
        self.leave = "Leave";

        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.cards = object.cards;
        self.cardPath = object.cardPath;
        self.backCardPath = object.backCardPath;
        self.playing = ko.observable(false);
        self.players = [
            ko.observable(new IW.All.Player({ playerId: 0, name: "" })),
            ko.observable(new IW.All.Player({ playerId: 1, name: "" }))
        ];

        self.changeCard = function (cardInfo, event) {
            var player = ko.contextFor(event.target).$parent;
            if (player.name() != self.user) {
                return;
            }
            self.signalRServer.drawCard(cardInfo.index);
        };

        self.gameFull = function () {
            return self.players[0]().name() != "" &&
                   self.players[1]().name() != "";
        }

        self.joinGame = function() {
            self.playing(true);
            var position = self.players[0]().name() == "" ? 0 : 1;
            self.signalRServer.joinGameTable(position);
            self.players[position]().name(self.user);
        }

        self.leaveGame = function () {
            self.playing(false);
            var name = self.players[0]().name();
            var position = name == self.user ? 0 : 1;
            self.signalRServer.leaveGameTable(position);
            self.players[position]().name("");
        }

        self.getPlayerHands = function () {
            self.signalRServer.getAllPlayers().done(function (playersInfo) {
                playersInfo.forEach(function (playerInfo) {
                    var player = self.players[playerInfo.Id]();
                    player.name(playerInfo.Name);
                    player.cards.push(ko.observable({ src: self.getCard(playerInfo.Cards[0]), index: 0 }));
                    player.cards.push(ko.observable({ src: self.getCard(playerInfo.Cards[1]), index: 1 }));
                });
            });
        };

        self.getCard = function (cardIndex) {
            if (cardIndex < 0) {
                return self.backCardPath;
            }
            return self.cardPath + self.cards[cardIndex];
        };

        self.signalRClient.userJoinedGameTable = function (username, player) {
            self.players[player]().name(username);
        };

        self.signalRClient.userLeftGameTable = function (player) {
            self.players[player]().name("");
        };

        self.signalRClient.drawCard = function (playerId, cardIndex, newCard) {
            var playerInfo = self.players[playerId]();
            playerInfo.cards()[cardIndex]({src: self.getCard(newCard), index: cardIndex});
        };
    };

    namespace("IW.All").ActiveGameUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([object.user]);

        self.signalRClient.usersInGame = function (users) {
            ko.utils.arrayPushAll(self.activeUsers, users);
        }

        self.signalRClient.userJoinedGame = function (username) {
            self.activeUsers.push(username);
        }

        self.signalRClient.userLeftGame = function (username) {
            self.activeUsers.remove(username);
        }
    };
}());