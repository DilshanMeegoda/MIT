(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('userService', userService);

    userService.$inject = ['$http', 'app.config'];

    function userService($http, config) {
        var self = this;
        var service = {};
        var response = {};
        response.filename = "";
        response.success = false;
        self.url = config.basePath + "User/";

        service.GetUserList = function (companyId, callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetAllUsers/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { "companyId": companyId }
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

        service.CreateUser = function (user, callback) {
            $http({
                cache: false,
                url: self.url + "SaveUser/",
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: user
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        service.GetUserListToDropDown = function (callback) {
            response.result = [];
            $http({
                cache: false,
                url: self.url + 'GetAllActiveUsersToDropDown/',
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

        service.GetUser = function (userId, callback) {
            response.result = {};
            $http({
                cache: false,
                url: self.url + 'GetUserById/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { userId: userId }
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

        service.UpdateUser = function (user, callback) {
            $http({
                cache: false,
                url: self.url + 'UpdateUser/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: user
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
            $http.post(self.url + 'UploadProfilePicture/', fd, {
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

        return service;
    }
})();