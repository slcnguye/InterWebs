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
            user: self.user
        });

        $.connection.hub.start().done(
            function() {
                self.signalR.server.joinGame("All", self.user);
                self.gameBoxView.getPlayerHands();
            }
        );

        $(window).bind('beforeunload', function() {
            self.signalR.server.leaveGame("All", self.user);
        });
    };

    namespace("IW.All").GameBoxView = function(object) {
        var self = this;
        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.cards = object.cards;
        self.cardPath = object.cardPath;
        self.player1Cards = ko.observableArray([]);
        self.player2Cards = ko.observableArray([]);

        self.random = function(max) {
            return Math.floor(Math.random() * max);
        };

        self.changeCard = function () {
            var cardInfo = this;
            self.signalRServer.drawCard("All", cardInfo.player, cardInfo.card);
        };

        self.getPlayerHands = function() {
            self.signalRServer.getCardsForPlayer("All", 1);
            self.signalRServer.getCardsForPlayer("All", 2);
        };

        self.getCard = function(cardIndex) {
            return self.cardPath + self.cards[cardIndex];
        };

        self.signalRClient.drawCard = function (gameName, player, cardIndex, newCard) {
            if ("All" == gameName) {
                var playerCards = player == 1 ? self.player1Cards() : self.player2Cards();
                playerCards[cardIndex].cardSrc(self.getCard(newCard));
            }
        };

        self.signalRClient.playersHand = function(gameName, player, playersHand) {
            if ("All" == gameName) {
                var playerInfo = player == 1 ? self.player1Cards : self.player2Cards;
                playerInfo.push({
                    player: player,
                    card: 0,
                    cardSrc: ko.observable(self.getCard(playersHand[0]))
                });
                playerInfo.push({
                    player: player,
                    card: 1,
                    cardSrc: ko.observable(self.getCard(playersHand[1]))
                });
            }
        };
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