(function () {
    'use strict';
    angular.module('app.config', [])
        .value('app.config', {
            basePath: 'http://192.168.8.179:81/api/'
        });
})();