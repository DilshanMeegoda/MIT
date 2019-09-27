(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('loginService', loginService);

    loginService.$inject = ['$http', 'app.config'];

    function loginService($http, config) {
        var self = this;
        var service = {};

        self.url = config.basePath + "Authenticate/";

        //$http.defaults.headers.common['Token'] = self.authtoken;

        service.ValidateUser = function (user, callback) {
            var response = {};
            response.success = false;
            response.result = {};
            $http({
                cache: false,
                url: self.url + 'AuthenticateUser/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: user
            }).success(function (data, status, headers) {
                if (data != null) {
                    response.success = true;
                    response.result = data;
                    sessionStorage.removeItem('Token');
                    sessionStorage.setItem('Token', headers('Token'));
                    $http.defaults.headers.common['Token'] = headers('Token');
                }
                callback(response);
            }).error(function (data, status, headers) {
                console.log('error');
            });
        };
        return service;
    }
})();