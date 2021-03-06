﻿(function() {
    namespace("IW.All").GameViewModel = function (object) {
        var self = this;
        self.gameSignalR = $.connection.gameHub;
        self.chatSignalR = $.connection.chatHub;
        self.newUsername = ko.observable();
        self.showWelcomeMessage = ko.observable(false);
        self.user = ko.observable(object.user);

        self.setUsername = function() {
            if (self.user() != self.newUsername()) {
                self.showWelcomeMessage(true);
                $.post(object.setUsername + "?username=" + self.newUsername())
                    .done(function (partialViewResult) {
                        ko.cleanNode($("#navigationbar")[0]);
                        $("#navigationbar").replaceWith(partialViewResult);
                    });

                self.user(self.newUsername());
                self.gameSignalR.server.addUser(self.user());
                self.activeGameUsersView.setUsername(self.user());
                self.gameBoxView.setUsername(self.user());
                self.chatBoxView.setUsername(self.user());
                setTimeout(function () {
                    self.showWelcomeMessage(false);
                }, 5000);
            }
        }

        self.activeGameUsersView = new IW.All.ActiveGameUsersView({
            signalRClient: self.gameSignalR.client,
            user: object.user
        });

        self.gameBoxView = new IW.All.GameBoxView({
            signalRClient: self.gameSignalR.client,
            signalRServer: self.gameSignalR.server,
            cards: object.cards,
            cardPath: object.cardPath,
            backCardPath: object.backCardPath,
            blankCardPath: object.blankCardPath,
            user: object.user
        });

        self.chatBoxView = new IW.All.ChatBoxView({
            signalRClient: self.chatSignalR.client,
            signalRServer: self.chatSignalR.server,
            chatMessages: object.chatMessages,
            storeMessageUrl: object.storeMessageUrl,
            user: object.user
        });

        self.guestUser = function () {
            return !self.user() || self.user() == "";
        }

        $.connection.hub.start({ jsonp: true }).done(
            function() {
                self.gameBoxView.getAllPlayerNames();
            }
        );
    };

    namespace("IW.All").GameBoxView = function(object) {
        var self = this;
        var disableGameInteractions = false;
        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.cards = object.cards;
        self.cardPath = object.cardPath;
        self.backCardPath = object.backCardPath;
        self.blankCardPath = object.blankCardPath;
        self.playing = ko.observable(false);
        self.loadedGame = ko.observable(false);
        self.roundMessage = ko.observable("");
        self.players = [
            ko.observable(new IW.All.Player({ playerId: 0, name: "" })),
            ko.observable(new IW.All.Player({ playerId: 1, name: "" }))
        ];

        self.setUsername = function(username) {
            self.user = username;
        }

        self.cardClicked = function (cardInfo, event) {
            if (disableGameInteractions || !self.user || self.user == "") {
                return;
            }

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

        self.getCard = function (card) {
            if (card == -2) {
                return self.blankCardPath;
            }

            if (card == -1) {
                return self.backCardPath;
            }

            return self.cardPath + self.cards[card];
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
            disableGameInteractions = true;
            var playerInfo = self.players[playerId]();
            playerInfo.cards()[cardIndex]({ src: self.getCard(newCard), index: cardIndex });

            setTimeout(function() {
                playerInfo.selectedCard(-1);
                if (!self.playing() || playerInfo.name() != self.user) {
                    self.signalRServer.isBlankCard(cardIndex, playerId).done(function (valid) {
                        var card = valid ? -2 : -1;
                        playerInfo.cards()[cardIndex]({ src: self.getCard(card), index: cardIndex });
                    });
                }
                else {
                    self.signalRServer.getCard(cardIndex, playerId).done(function(card) {
                        playerInfo.cards()[cardIndex]({ src: self.getCard(card), index: cardIndex });
                    });
                }
                self.roundMessage("");
                disableGameInteractions = false;
            }, 2000);
        };

        self.signalRClient.newGame = function () {
            self.players.forEach(function (player) {
                player().selectedCard(-1);
            });
        }

        self.signalRClient.roundOutcome = function (outcomeMessage) {
            self.roundMessage(outcomeMessage);
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

        self.setUsername = function (username) {
            self.activeUsers.remove($.t('Chat.YouAsGuest'));
            self.activeUsers.unshift(username);
        }

        self.signalRClient.usersInGame = function (users) {
            if (!object.user || object.user == "") {
                self.activeUsers.push($.t('Chat.YouAsGuest'));
            }

            ko.utils.arrayPushAll(self.activeUsers, users);

            var userIndex = self.activeUsers.indexOf(object.user);
            if (userIndex == -1) {
                self.activeUsers.unshift(object.user);
            }
        }

        self.signalRClient.userJoinedGame = function (username) {
            self.activeUsers.push(username);
        }

        self.signalRClient.userLeftGame = function (username) {
            self.activeUsers.remove(username);
        }
    };
}());