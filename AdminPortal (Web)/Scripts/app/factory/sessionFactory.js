(function () {
    'use strict';

    angular.module('angularjsFormBuilderApp')
           .factory('sessionService', sessionService);

    sessionService.$inject = ['$http'];

    function sessionService() {
        return {
            set: function (key, value) {
                return sessionStorage.setItem(key, value);
            },
            get: function (key) {
                return sessionStorage.getItem(key);
            },
            destroy: function (key) {
                return sessionStorage.removeItem(key);
            }
        };
    }
})();