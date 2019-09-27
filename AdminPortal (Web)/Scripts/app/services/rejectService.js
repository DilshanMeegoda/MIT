(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('rejectService', rejectService);

    rejectService.$inject = ['$http', 'app.config'];

    function rejectService($http, config) {
        var self = this;
        var service = {};
        var response = {};
        response.success = false;

        self.url = config.basePath + "Project/";

        service.GetProjectsList = function (companyId, callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetAllProjectsByCompany/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { companyId: companyId }
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                    response.result = data;
                } else {
                    response.success = false;
                }
                callback(response);
            }).error(function () {
                console.log('error in service');
                callback(response);
            });
        };

        return service;
    }
})();