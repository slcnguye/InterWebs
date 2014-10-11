(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        $.connection.hub.start();
        self.signalR = $.connection.chatHub;
        self.signalR.client.newMessage = function (chatName, username, message) {
            self.chatMessages.push({
                user: username,
                text: message
            });
        };

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
        self.activeUsers = ko.observableArray([
            { name: "Sang Nguyen" },
            { name: "Thuy Truong" }
        ]);
        self.message = ko.observable("");

        self.sendMessage = function () {
            if (!self.message()) {
                return;
            }

            var message = self.message();
            self.message("");
            self.enterMessageFocus(true);
            $.post(self.storeMessageUrl + "?message=" + message);
            self.signalR.server.send("All", self.user, message);
        };
    }
}());