(function() {
    namespace("IW.All").ChatViewModel = function() {
        var self = this;

        self.chatMessages = ko.observableArray([]);
        self.send = "Send";
        self.user = "Sang Nguyen";
        self.enterMessage = "Enter Message";
        self.enterMessageFocus = ko.observable(true);
        self.activeUsers = ko.observableArray([
            { name: "Sang Nguyen" },
            { name: "Thuy Truong" }
        ]);
        self.message = ko.observable("");

        self.sendMessage = function () {
            self.chatMessages.push({
                user: self.user,
                text: self.message()
            });
            self.message("");
            self.enterMessageFocus(true);
        };
    }
}());