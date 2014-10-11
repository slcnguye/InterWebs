(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        self.addChatHistory = function(historicMessages) {
            var history = ko.observableArray([]).extend({ scrollFollow: '#ChatMessages' });
            historicMessages.forEach(function(message) {
                history.push({
                    user: message.User,
                    text: message.Message
                });
            });
            return history;
        };

        self.chatMessages = self.addChatHistory(object.chatMessages);
        self.send = "Send";
        self.enterMessage = "Enter Message";
        self.enterMessageFocus = ko.observable(true);
        self.activeUsers = ko.observableArray([
            { name: "Sang Nguyen" },
            { name: "Thuy Truong" }
        ]);
        self.message = ko.observable("");

        self.user = object.user;
        self.storeMessageUrl = object.storeMessageUrl;

//        self.test = $.connection.chatHub;
//        self.test.client.connection = function(chatName, message) {
//            debugger;
//        };
//        $.connection.hub().done(function() {
//            self.sendMessage = function () {
//                debugger;
//                if (!self.message()) {
//                    return;
//                }
//
//                var message = self.message();
//                self.chatMessages.push({
//                    user: self.user,
//                    text: message
//                });
//
//                self.message("");
//                self.enterMessageFocus(true);
//                $.post(self.storeMessageUrl + "?message=" + message);
//            };
//        });
    }
}());