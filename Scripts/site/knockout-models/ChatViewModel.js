(function() {
    namespace("IW.All").ChatViewModel = function(object) {
        var self = this;

        self.user = object.user;
        self.signalR = $.connection.chatHub;
        self.activeUsersView = new IW.All.ActiveUsersView({
            signalRClient: self.signalR.client,
            user: self.user
        });

        self.chatBoxView = new IW.All.ChatBoxView({
            signalRClient: self.signalR.client,
            signalRServer: self.signalR.server,
            chatMessages: object.chatMessages,
            storeMessageUrl: object.storeMessageUrl,
            user: self.user
        });

        $.connection.hub.start({ jsonp: true });
    };

    namespace("IW.All").ActiveUsersView = function (object) {
        var self = this;
        self.signalRClient = object.signalRClient;
        self.activeUsers = ko.observableArray([]);

        self.signalRClient.usersInChat = function (users) {
            ko.utils.arrayPushAll(self.activeUsers, users);
        }

        self.signalRClient.userJoinedChat = function (username) {
            self.activeUsers.push(username);
        }

        self.signalRClient.userLeftChat = function (username) {
            self.activeUsers.remove(username);
        }
    };
}());