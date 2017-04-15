define(['domready', 'ace/ace', 'knockout', 'knockout-ace'], function (domready, ace, ko) {

    console.log("ko is: ", ko.version);

    function PageViewModel() {
        var self = this;
        self.id = ko.observable();
        self.name = ko.observable();
        self.content = ko.observable();
        self.progress = ko.observable();
        self.saving = ko.computed(function () {
            if (self.progress()) {
                var p = "bust=" + (new Date()).getTime();
                return '<span class="span-status">Saving&nbsp;</span><img src="img/auto_saving.gif?' + p + '" />';
            }
            return '<span class="span-status">Saved&nbsp;</span><img src="img/auto_waiting.gif" />';
        });

        self.fetch = function () {
            var jqXHR = $.ajax({
                type: "GET",
                cache: false,
                url: '/page/getpage',
                contentType: 'application/json; charset=utf-8',
                success: function (data, textStatus, jqXHR) {
                    self.id(data.id);
                    self.name(data.name);
                    self.content(data.content);
                },
                error: function (response) {

                }
            }).always = function (data, textStatus, jqXHR) {
            };
        }

        self.save = function () {
            var jqXHR = $.ajax({
                type: "POST",
                cache: false,
                url: '/page/savepage',
                data: JSON.stringify({ id: self.id(), name: self.name(), content: self.content() }),
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    self.progress(true);
                },
                success: function (data, textStatus, jqXHR) {
                    
                },
                error: function (response) {
                    
                }
            }).always = function (data, textStatus, jqXHR) {
                setTimeout(function () {
                    self.progress(false);
                }, 1200);
            };
        }
    }

    var pagemvvm = new PageViewModel();
    ko.applyBindings(pagemvvm);
    pagemvvm.fetch();
});