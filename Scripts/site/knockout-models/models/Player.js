(function() {
    namespace("IW.All").Player = function (object) {
        var self = this;
        self.name = ko.observable(object.name);
        self.playerId = object.playerId;
        self.cards = ko.observableArray([]);
    };
}());