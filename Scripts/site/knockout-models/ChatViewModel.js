(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;
        self.newUsername = ko.observable();
        self.showWelcomeMessage = ko.observable(false);
        self.user = ko.observable(object.user);
        self.signalR = $.connection.chatHub;

        self.setUsername = function () {
            if (self.user() != self.newUsername()) {
                self.showWelcomeMessage(true);
                $.post(object.setUsername + "?username=" + self.newUsername())
                    .done(function (partialViewResult) {
                        ko.cleanNode($("#navigationbar")[0]);
                        $("#navigationbar").replaceWith(partialViewResult);
                    });

                self.user(self.newUsername());
                self.signalR.server.addUser(self.user());
                self.activeUsersView.setUsername(self.user());
                self.chatBoxView.setUsername(self.user());
                setTimeout(function () {
                    self.showWelcomeMessage(false);
                }, 5000);
            }
        }

        self.activeUsersView = new IW.All.ActiveUsersView({
            signalRClient: self.signalR.client,
            user: self.user()
        });

        self.chatBoxView = new IW.All.ChatBoxView({
            signalRClient: self.signalR.client,
            signalRServer: self.signalR.server,
            chatMessages: object.chatMessages,
            storeMessageUrl: object.storeMessageUrl,
            user: self.user()
        });

        self.guestUser = function () {
            return !self.user() || self.user() == "";
        }

        $.connection.hub.start({ jsonp: true });
    };

    namespace("IW.All").ActiveUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([]);

        self.setUsername = function (username) {
            self.activeUsers.remove($.t('Chat.YouAsGuest'));
            self.activeUsers.unshift(username);
        }

        self.signalRClient.usersInChat = function (users) {
            if (!object.user || object.user == "") {
                self.activeUsers.push($.t('Chat.YouAsGuest'));
            }

            ko.utils.arrayPushAll(self.activeUsers, users);

            var userIndex = self.activeUsers.indexOf(object.user);
            if (userIndex == -1) {
                self.activeUsers.unshift(object.user);
            }
        }

        self.signalRClient.userJoinedChat = function (username) {
            self.activeUsers.push(username);
        }

        self.signalRClient.userLeftChat = function (username) {
            self.activeUsers.remove(username);
        }
    };
}());