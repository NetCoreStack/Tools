define(['jquery',
    'jqueryui',
    'bootstrap',
    'ace/ace',
    'config',
    'jstree'],
    function ($, ui, bootstrap, ace, config) {
        console.log("jQuery is: ", $.fn.jquery);
        console.log("jQueryUI is: ", $.ui.version);
        console.log("ace is: ", ace.version); // shim config resolver
        console.log("config is: ", config);
        console.log("jstree is: ", $.jstree.version);
    });