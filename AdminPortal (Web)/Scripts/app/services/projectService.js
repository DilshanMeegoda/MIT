(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('projectService', projectService);

    projectService.$inject = ['$http', 'app.config'];

    function projectService($http, config) {
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

        service.GetProject = function (projectId, callback) {
            response.result = {};
            $http({
                cache: false,
                url: self.url + 'GetProjectById/',
                method: "GET",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                params: { projectId: projectId }
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

        service.CreateProject = function (project, callback) {
            $http({
                cache: false,
                url: self.url + 'PostProject/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: project
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        service.UpdateProject = function (project, callback) {
            $http({
                cache: false,
                url: self.url + 'UpdateProject/',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                data: project
            }).success(function (data) {
                if (data != null) {
                    response.success = true;
                }
                callback(response);
            }).error(function () {
                callback(response);
            });
        };

        return service;
    }
})();