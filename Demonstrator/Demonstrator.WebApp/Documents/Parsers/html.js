module.exports = function (callback, template, data) {
    var jsrender = require('jsrender');

    var tmpl = jsrender.templates(template);

    var html = tmpl.render(data); 

    callback(html);
};