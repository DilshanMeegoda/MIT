(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('reportListController', reportListController);

    reportListController.$inject = ['$location', 'reportService'];

    function reportListController($location, reportService) {
        /* jshint validthis:true */
        var report = this;
        report.title = 'reportListController';
        report.reportvm = {};
        report.reports = [];

        report.Init = function () {
            report.GetReports();
            report.InitializeGrid();
        };

        report.InitializeGrid = function () {
            report.gridOptions = {
                data: 'report.reports',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'reportId',
                        displayName: 'Report Id',
                        width: 100,
                    },
                    {
                        field: 'reportName',
                        displayName: 'Report Name',
                        width: 400,
                    },
                    {
                        field: 'reportCategory',
                        displayName: 'Report Category',
                        width: 250,
                    },
                    {
                        field: 'isStandard',
                        displayName: 'Standard',
                        width: 100,
                        cellTemplate: '<input type="checkbox" ng-model="row.entity.isStandard" ng-true-value="true" ng-false-value="false" ng-disabled="true">'
                    },
                    {
                        field: 'isActive',
                        displayName: 'Active',
                        width: 100,
                        cellTemplate: '<input type="checkbox" ng-model="row.entity.isActive" ng-true-value="true" ng-false-value="false" ng-disabled="true">'
                    },
                    {
                        field: 'Action',
                        displayName: 'Action',
                        width: 100,
                        cellTemplate: '<button id="editBtn" type="button" class="btn btn-xs btn-info"  ng-click="grid.appScope.report.EditReport(row.entity.reportId)" >Edit</button>'
                    }
                ]
            };
        };

        report.GetReports = function () {
            reportService.GetReportList(function (response) {
                if (response.success) {
                    report.reports = response.result;
                }
                else {
                    console.log('error user loading');
                }
            });
        };

        report.EditReport = function (reportId) {
            $location.path('/Report/Edit/' + reportId);
        };

        report.CreateNewReport = function () {
            $location.path('/Report/Create');
        };

        report.GetReportByTerm = function () {
            var searchterm = report.reportVm.term;
            reportService.GetReportListByTerm(searchterm, function (response) {
                if (response.success) {
                    report.reports = response.result;
                }
                else {
                    console.log('error user loading');
                }
            });
        };

        report.Init();
    }
})();
