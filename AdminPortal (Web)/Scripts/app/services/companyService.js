(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('companyService', companyService);

    companyService.$inject = ['$http', 'app.config'];

    function companyService($http, config) {
        var self = this;
        var service = {};
        var response = {};
        response.success = false;
        self.url = config.basePath + "Company/";

        service.getComapanyList = function (callback) {

            response.result = [];

            $http({
                cache: false,
                url: self.url + 'GetCompanyList/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
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

        service.CreateCompany = function (company, callback) {
            $http({
                cache: false,
                url: self.url + 'CreateCompany/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: company
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        service.GetCompany = function (companyId, callback) {
            response.result = {};
            $http({
                cache: false,
                url: self.url + 'GetCompanyById/',
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

        service.UpdateCompany = function (company, callback) {
            $http({
                cache: false,
                url: self.url + 'UpdateCompany/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: company
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        service.UploadFileToUrl = function (file, callback) {
            var fd = new FormData();
            fd.append('profile', file);
            $http.post(self.url + 'UploadComapanyLogo/', fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).success(function (data) {
                if (data != null) {
                    response.fileName = data;
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        service.DownloadImage = function (fileName, callback) {
            response.result = {};
            $http({
                cache: false,
                url: self.url + 'DownloadFile/',
                method: "GET",
                params: { fileName: fileName }
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

        service.CompaniesByTerm = function (term, callback) {
            var response = {};
            response.success = false;
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetCompaniesByTerm/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { term: term }
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