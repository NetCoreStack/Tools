define(['jquery',
    'jqueryui',
    'bootstrap',
    'ace/ace'],
    function ($, ui, bootstrap, ace) {
        console.log("jQuery is: ", $.fn.jquery);
        console.log("jQueryUI is: ", $.ui.version);
        console.log("ace is: ", ace.version); // shim config resolver
    });