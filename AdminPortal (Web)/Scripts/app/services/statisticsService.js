(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('statisticsService', statisticsService);

    statisticsService.$inject = ['$http', 'app.config'];

    function statisticsService($http, config) {
        var self = this;
        var service = {};
        var response = {};
        response.success = false;

        self.url = config.basePath + "Statistics/";

        service.GetStatisticsDetails = function (callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetStatisticsForAllCompanies/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' }
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
        service.GetStatisticsByCompany = function (companyId, callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetStatisticsByCompany/',
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

        service.GetStatisticsByCompanyAndReport = function (companyId, reportId, callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetStatisticsByCompanyAndReport/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { companyId: companyId, reportId: reportId }
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
    };

})();