(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('rejectsController', rejectsController);

    rejectsController.$inject = ['$location', 'projectService', 'reportService', 'userService', 'Flash', '$rootScope'];

    function rejectsController($location, projectService, reportService, userService, flash, $rootScope) {
        /* jshint validthis:true */
        var rejects = this;
        rejects.title = 'rejectsController';
        rejects.projectVm = {};
        rejects.projects = [];
        rejects.projectVm.companyId = $rootScope.companyId;

        rejects.reports = [];
        rejects.users = [];

        rejects.Init = function () {
            rejects.GetRejects();
        };

        rejects.InitializeGrid = function () {
            rejects.gridOptions = {
                data: 'project.projects',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'reportId',
                        displayName: 'Report Id',
                        width: 100,
                    },
                    {
                        field: 'note',
                        displayName: 'Note',
                        width: 250,
                    }
                ]
            };
        };

        rejects.GetRejects = function () {
            projectService.GetProjectsList(rejects.projectVm.companyId, function (response) {
                if (response.success) {
                    rejects.projects = response.result;
                }
                else {
                    console.log('error Rejects List loading');
                }
            });
        };

        rejects.Init();

    }
})();
