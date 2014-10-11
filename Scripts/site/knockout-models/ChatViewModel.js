(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        self.chatMessages = ko.observableArray([]).extend({ scrollFollow: '#ChatMessages' });
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

        self.sendMessage = function () {
            if (!self.message()) {
                return;
            }

            var message = self.message();
            self.chatMessages.push({
                user: self.user,
                text: message
            });
            
            self.message("");
            self.enterMessageFocus(true);
//            debugger;
            $.post(self.storeMessageUrl + "?message=" + message);
        };
    }
}());