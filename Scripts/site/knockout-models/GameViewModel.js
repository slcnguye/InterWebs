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
                self.gameBoxView.getAllPlayerNames();
            }
        );
    };

    namespace("IW.All").GameBoxView = function(object) {
        var self = this;
        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.cards = object.cards;
        self.cardPath = object.cardPath;
        self.backCardPath = object.backCardPath;
        self.playing = ko.observable(false);
        self.loadedGame = ko.observable(false);
        self.roundMessage = ko.observable("");
        self.players = [
            ko.observable(new IW.All.Player({ playerId: 0, name: "" })),
            ko.observable(new IW.All.Player({ playerId: 1, name: "" }))
        ];

        self.cardClicked = function (cardInfo, event) {
            var player = ko.contextFor(event.target).$parent;
            if (player.name() != self.user) {
                return;
            }

            if (player.selectedCard() == cardInfo.index) {
                // deselect card
                player.selectedCard(null);
                self.signalRServer.unplayCard(cardInfo.index);
            } else {
                // select card
                player.selectedCard(cardInfo.index);
                self.signalRServer.playCard(cardInfo.index);
            }
        };

        self.shuffleDeck = function() {
            self.signalRServer.shuffleDeck();
        };

        self.gameFull = function() {
            return self.players[0]().name() != "" &&
                self.players[1]().name() != "";
        };

        self.joinGame = function() {
            self.playing(true);
            var position = self.players[0]().name() == "" ? 0 : 1;
            self.signalRServer.joinGameTable(position).done(function() {
                self.getPlayerHand(position);
            });
            self.players[position]().name(self.user);
        };

        self.leaveGame = function() {
            self.playing(false);
            var name = self.players[0]().name();
            var position = name == self.user ? 0 : 1;
            self.signalRServer.leaveGameTable(position);
            self.players[position]().name("");

            ko.utils.arrayForEach(self.players, function(player) {
                player().cards()[0]({ src: self.getCard(-1), index: 0 });
                player().cards()[1]({ src: self.getCard(-1), index: 1 });
            });
        };

        self.getAllPlayerNames = function () {
            self.signalRServer.getAllPlayerNames().done(function (playersInfo) {
                playersInfo.forEach(function (playerInfo) {
                    var player = self.players[playerInfo.Id]();
                    player.name(playerInfo.Name);
                    player.cards.push(ko.observable({ src: self.getCard(-1), index: 0 }));
                    player.cards.push(ko.observable({ src: self.getCard(-1), index: 1 }));
                    player.selectedCard(playerInfo.PlayedCard);
                });
            });
            self.loadedGame(true);
        };

        self.getPlayerHand = function (position) {
            self.signalRServer.getPlayer(position).done(function (playerInfo) {
                var player = self.players[position]();
                player.cards()[0]({ src: self.getCard(playerInfo.Cards[0].Value), index: 0 });
                player.cards()[1]({ src: self.getCard(playerInfo.Cards[1].Value), index: 1 });
                player.selectedCard(playerInfo.PlayedCard);
            });
            self.loadedGame(true);
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
            playerInfo.selectedCard(-1);
            playerInfo.cards()[cardIndex]({src: self.getCard(newCard), index: cardIndex});
        };

        self.signalRClient.showCard = function (playerId, cardIndex, newCard) {
            var playerInfo = self.players[playerId]();
            playerInfo.cards()[cardIndex]({ src: self.getCard(newCard), index: cardIndex });

            setTimeout(function() {
                playerInfo.selectedCard(-1);
                if (!self.playing() || playerInfo.name() != self.user) {
                    playerInfo.cards()[cardIndex]({ src: self.getCard(-1), index: cardIndex });
                }
                else {
                    self.signalRServer.getCard(cardIndex, playerId).done(function(card) {
                        playerInfo.cards()[cardIndex]({ src: self.getCard(card), index: cardIndex });
                    });
                }
                self.roundMessage("");
            }, 2000);
        };

        self.signalRClient.roundWinner = function (playerId) {
            self.roundMessage(self.players[playerId]().name() + "won round");
        };

        self.signalRClient.cardPlayed = function (playerId, cardIndex) {
            var playerInfo = self.players[playerId]();
            playerInfo.selectedCard(cardIndex);
        }

        self.signalRClient.cardUnplayed = function (playerId) {
            var playerInfo = self.players[playerId]();
            playerInfo.selectedCard(-1);
        }
    };

    namespace("IW.All").ActiveGameUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([]);

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