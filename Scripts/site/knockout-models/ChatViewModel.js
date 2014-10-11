(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        $.connection.hub.start().done(
            function() {
                self.loadedMessages(true);
                var chat = document.querySelector('#ChatMessages');
                chat.scrollTop = chat.scrollHeight - chat.clientHeight;
                self.activeUsers.push(self.user);
                self.signalR.server.joinChat("All", self.user);
            }
        );

        self.signalR = $.connection.chatHub;
        self.signalR.client.newMessage = function (chatName, username, message) {
            self.chatMessages.push({
                user: username,
                text: message
            });
        };

        self.signalR.client.usersInChat = function(chatName, users) {
            if ("All" == chatName) {
                self.activeUsers.push(users);
            }
        }

        self.signalR.client.userJoinedChat= function(chatName, username) {
            if ("All" == chatName) {
                self.activeUsers.push(username);
            }
        }

        self.signalR.client.userLeftChat = function(chatName, username) {
            var activeUsers = [];
            self.activeUsers().forEach(function(user) {
                if (user != username) {
                    activeUsers.push(user);
                }
            });
            self.activeUsers(activeUsers);
        }

        self.signalR.client.getUsers = function(chatName) {
            if (chatName == "All") {
                return self.activeUsers();
            }
        }

        self.addChatHistory = function (historicMessages) {
            var history = ko.observableArray([]).extend({ scrollFollow: '#ChatMessages' });
            historicMessages.forEach(function (message) {
                history.push({
                    user: message.User,
                    text: message.Message
                });
            });
            return history;
        };

        self.user = object.user;
        self.storeMessageUrl = object.storeMessageUrl;
        self.chatMessages = self.addChatHistory(object.chatMessages);

        self.send = "Send";
        self.enterMessage = "Enter Message";

        self.enterMessageFocus = ko.observable(true);
        self.activeUsers = ko.observableArray([]);
        self.message = ko.observable("");
        self.loadedMessages = ko.observable(false);

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
            self.signalR.server.send("All", self.user, message);
        };

        $(window).bind('beforeunload', function () {
            self.signalR.server.leaveChat("All", self.user);
        });
    }
}());