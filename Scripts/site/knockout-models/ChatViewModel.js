(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        self.user = object.user;
        self.signalR = $.connection.chatHub;
        self.activeUsersView = new IW.All.ActiveUsersView({
            signalRClient: self.signalR.client
        });

        self.chatBoxView = new IW.All.ChatBoxView({
            signalRClient: self.signalR.client,
            signalRServer: self.signalR.server,
            chatMessages: object.chatMessages,
            storeMessageUrl: object.storeMessageUrl,
            user: self.user
        });

        $.connection.hub.start().done(
            function() {
                self.activeUsersView.activeUsers.push(self.user);
                self.signalR.server.joinChat("All", self.user);
            }
        );

        $(window).bind('beforeunload', function() {
            self.signalR.server.leaveChat("All", self.user);
        });
    };

    namespace("IW.All").ChatBoxView = function (object) {
        var self = this;
        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.storeMessageUrl = object.storeMessageUrl;

        self.send = "Send";
        self.enterMessage = "Enter Message";
        self.enterMessageFocus = ko.observable(true);
        self.message = ko.observable("");
        self.loadedMessages = ko.observable(false);

        self.addChatHistory = function () {
            var messages = ko.observableArray([]).extend({ scrollFollow: '#ChatMessages' });
            object.chatMessages.forEach(function (message) {
                messages.push({
                    user: message.User,
                    text: message.Message
                });
            });
            self.loadedMessages(true);
            return messages;
        };

        self.chatMessages = self.addChatHistory();

        self.sendMessage = function () {
            if (!self.message()) {
                return;
            }

            var message = self.message();
            self.message("");
            self.enterMessageFocus(true);
            $.post(self.storeMessageUrl + "?message=" + message);
            self.chatMessages.push({
                user: self.user,
                text: message
            });
            self.signalRServer.send("All", self.user, message);
        };

        self.signalRClient.newMessage = function (chatName, username, message) {
            self.chatMessages.push({
                user: username,
                text: message
            });
        };
    };

    namespace("IW.All").ActiveUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([]);

        self.signalRClient.usersInChat = function (chatName, users) {
            if ("All" == chatName) {
                self.activeUsers.push(users);
            }
        }

        self.signalRClient.userJoinedChat = function (chatName, username) {
            if ("All" == chatName) {
                self.activeUsers.push(username);
            }
        }

        self.signalRClient.userLeftChat = function (chatName, username) {
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