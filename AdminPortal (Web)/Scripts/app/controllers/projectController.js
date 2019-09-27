(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('projectController', projectController);

    projectController.$inject = ['$location', 'projectService', 'reportService', 'userService', 'Flash', '$rootScope'];

    function projectController($location, projectService, reportService, userService, flash, $rootScope) {
        /* jshint validthis:true */
        var project = this;
        project.title = 'projectController';
        project.projectVm = {};
        project.projects = [];
        project.projectVm.companyId = $rootScope.companyId;

        project.reports = [];
        project.users = [];

        project.Init = function () {
            project.GetProjects();
            project.GetReports();
            project.GetUsers();
            project.InitializeGrid();
        };

        project.InitializeGrid = function () {
            project.gridOptions = {
                data: 'project.projects',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'projectId',
                        displayName: 'Project Id',
                        width: 100,
                    },
                    {
                        field: 'title',
                        displayName: 'Title',
                        width: 250,
                    },
                    {
                        field: 'description',
                        displayName: 'Description',
                        width: 300,
                    },
                    {
                        field: 'location',
                        displayName: 'Location',
                        width: 100,
                    },
                    {
                        field: 'radius',
                        displayName: 'Radius',
                        width: 100,
                    },
                    {
                        field: 'isActive',
                        displayName: 'Active',
                        width: 100,
                    },
                    {
                        field: 'Action',
                        displayName: 'Action',
                        width: 100,
                        cellTemplate: '<button id="editBtn" type="button" class="btn btn-xs btn-info"  ng-click="grid.appScope.project.EditProject(row.entity.projectId)" >Edit</button>'
                    }
                ]
            };
        };

        project.EditProject = function (projectId) {
            $location.path('/Project/Edit/' + projectId);
        };

        project.GetProjects = function () {
            projectService.GetProjectsList(project.projectVm.companyId, function (response) {
                if (response.success) {
                    project.projects = response.result;
                }
                else {
                    console.log('error Project List loading');
                }
            });
        };

        project.CreateNewProject = function () {
            $location.path('/Project/Create');
        };

        project.CreateProject = function () {
            projectService.CreateProject(project.projectVm, function (response) {
                var message;
                if (response.success) {
                    message = '<strong> Project Creation Sucess..! </strong> ';
                    $location.path('/Project');
                    flash.create('success', message, 'custom-class');
                } else {
                    message = '<strong> Project Creation Fail..! </strong> .';
                    flash.create('danger', message, 'custom-class');
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

        project.GetUsers = function () {
            userService.GetUserListToDropDown(function (response) {
                if (response.success) {
                    project.users = response.result;
                } else {
                    console.log('error users loading');
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

        project.Init();

    }
})();
