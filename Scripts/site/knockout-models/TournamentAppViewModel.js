(function () {
    namespace("IW.Project.Tournament").AppViewModel = function(dataModel) {
        // Private state
        var self = this;

        // Data
        self.Views = {
            // Other views are added dynamically by app.addViewModel(...).
            Loading: {}
        };

        // UI state
        self.dataModel = dataModel;
        self.view = ko.observable(self.Views.Loading);

        self.loading = ko.computed(function() {
            return self.view() === self.Views.Loading;
        });

        // Other navigateToX functions are added dynamically by app.addViewModel(...).

        // Other operations
        self.addViewModel = function(options) {
            var viewItem = {},
                navigator;

            // Add view to AppViewModel.Views enum (for example, app.Views.Home).
            self.Views[options.name] = viewItem;

            // Add binding member to AppViewModel (for example, app.home);
            self[options.bindingMemberName] = ko.computed(function() {
                if (self.view() !== viewItem) {
                    return null;
                }
                return new options.factory(self);
            });

            if (typeof (options.navigatorFactory) !== "undefined") {
                navigator = options.navigatorFactory(self);
            } else {
                navigator = function() {
                    self.view(viewItem);
                };
            }

            // Add navigation member to AppViewModel (for example, app.NavigateToHome());
            self["navigateTo" + options.name] = navigator;
        };

        self.initialize = function () {
            self.addViewModel({
                name: "GameType",
                bindingMemberName: "gametype",
                factory: IW.Project.Tournament.GameTypeViewModel
            });

            self.addViewModel({
                name: "EnterPlayers",
                bindingMemberName: "enterplayers",
                factory: IW.Project.Tournament.EnterPlayersViewModel
            });

//            self.navigateToGameType();
            self.navigateToEnterPlayers();
        };

        self.initialize();
    };
}());