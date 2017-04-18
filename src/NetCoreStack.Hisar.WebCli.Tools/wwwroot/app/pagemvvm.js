define(['domready', 'jquery', 'ace/ace', 'knockout', 'knockout-ace'], function (domready, $, ace, ko) {

    console.log("ko is: ", ko.version);

    function PageViewModel() {
        var self = this;
        self.id = ko.observable();
        self.fullname = ko.observable();
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
                url: '/api/editor/getlayoutpage',
                contentType: 'application/json; charset=utf-8',
                success: function (data, textStatus, jqXHR) {
                    self.fullname(data.fullname);
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
                url: '/api/editor/savepagecontent',
                data: JSON.stringify({ fullname: self.fullname(), content: self.content() }),
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

    $('#tree').jstree({
        'core': {
            'data': {
                "check_callback": true,
                'url': '/api/editor/getdirectories'
            },
            'themes': {
                'responsive': false,
                'variant': 'small',
                'stripes': true
            },
            'multiple': false
        },
        'types': {
            'default': { 'icon': 'folder' },
            'file': { 'valid_children': [], 'icon': 'file' }
        },
        "plugins": ['state', 'types', 'unique', 'themes', 'ui']
    }).on('changed.jstree', function (e, data) {
        var fullname = data.instance.get_selected()[0];
        if (data.node && data.node.type == "file") {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/api/editor/getfilecontent?fullname="+encodeURIComponent(fullname),
                success: function (plaintext) {
                    var editor = ace.edit("editor");
                    var mode = fullname.split('.').pop();
                    console.log("mode", mode);
                    if (mode == "cshtml") {
                        mode = "razor";
                    }
                    if (mode == "js") {
                        mode = "javascript";
                    }
                    if (mode == "cs") {
                        mode = "csharp";
                    }
                    editor.getSession().setMode("ace/mode/" + mode);
                    pagemvvm.content(plaintext);
                    pagemvvm.fullname(fullname);
                }
            });
        }
    });
});