(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('projectEditController', projectEditController);

    projectEditController.$inject = ['$location', 'projectService', 'reportService', 'userService', '$routeParams', 'Flash', '$rootScope'];

    function projectEditController($location, projectService, reportService, userService, $routeParams, flash, $rootScope) {
        var project = this;
        project.title = 'projectEditController';
        project.projectVm = {};
        project.reports = [];
        project.users = [];
        project.projectVm.selectedOwner = {};
        project.projectVm.companyId = $rootScope.companyId;
        
        project.Init = function () {
            project.GetProject();
            project.GetReports();
            project.GetUsers();
        };

        project.GetProject = function () {
            project.projectVm.projectId = $routeParams.projectId;
            projectService.GetProject(project.projectVm.projectId, function (response) {
                if (response.success) {
                    project.projectVm = response.result;
                }
                else {
                    console.log('error Project loading');
                }
            });
        };

        project.GetReports = function () {
            reportService.GetReportListToDropDownByCompany(project.projectVm.companyId, function (response) {
                if (response.success) {
                    project.reports = response.result;
                } else {
                    console.log('error report loading');
                }
            });
        };

        project.GetReportsByTerm = function ($query) {
            return project.reports.filter(function (report) {
                return report.name.toLowerCase().indexOf($query.toLowerCase()) != -1;
            });
        };

        project.GetUsersByTerm = function ($query) {
            return project.users.filter(function (user) {
                return user.email.toLowerCase().indexOf($query.toLowerCase()) != -1;
            });
        };

        project.GetUsers = function () {
            userService.GetUserListToDropDown(function (response) {
                if (response.success) {
                    project.users = response.result;
                } else {
                    console.log('error users loading');
                }
            });
        };

        project.UpdateProject = function () {
            projectService.UpdateProject(project.projectVm, function (response) {
                var message;
                if (response.success) {
                    message = '<strong> Project Update Sucess..! </strong> ';
                    $location.path('/Project');
                    flash.create('success', message, 'custom-class');
                } else {
                    message = '<strong> Project Update Fail..! </strong> .';
                    flash.create('danger', message, 'custom-class');
                }
            });
        };

        project.Init();
    }
})();
