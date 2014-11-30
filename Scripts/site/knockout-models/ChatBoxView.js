(function() {
    namespace("IW.All").ChatBoxView = function (object) {
        var self = this;

        self.user = object.user;
        self.signalRClient = object.signalRClient;
        self.signalRServer = object.signalRServer;
        self.storeMessageUrl = object.storeMessageUrl;

        self.enterMessageFocus = ko.observable(true);
        self.message = ko.observable("");
        self.loadedMessages = ko.observable(false);

        self.setUsername = function (username) {
            self.user = username;
        }

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
            self.signalRServer.send(message);
        };

        self.signalRClient.newMessage = function (username, message) {
            self.chatMessages.push({
                user: username,
                text: message
            });
        };
    };
}());