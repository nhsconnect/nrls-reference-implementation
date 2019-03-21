module.exports = function (callback, template, data) {
    var jsreport = require('jsreport-core')();

    jsreport.init().then(function () {
        return jsreport.render({
            template: {
                content: template,
                engine: 'jsrender',
                recipe: 'phantom-pdf'
            },
            data: data
        }).then(function (resp) {
            //callback(/* error */ null, resp.content.toString('base64'));
            callback(/* error */ null, resp.content.toJSON().data);
        });
    }).catch(function (e) {
        callback(/* error */ e, null);
    })
};