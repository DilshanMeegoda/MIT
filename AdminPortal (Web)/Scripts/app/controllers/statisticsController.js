(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('statisticsController', statisticsController);

    statisticsController.$inject = ['$location', 'statisticsService', 'Flash'];

    function statisticsController($location, statisticsService, Flash) {
        var statistics = this;
        statistics.title = 'statisticsController';
        statistics.statisticsVm = {};
        statistics.details = [];
        statistics.templateDetails = [];
        statistics.projectDetails = [];

        statistics.Init = function () {
            statistics.GetStatistics();
            statistics.InitializeMainGrid();
        };

        statistics.InitializeMainGrid = function () {
            statistics.gridOptions = {
                data: 'statistics.details',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'company',
                        displayName: 'Company name',
                    },
                    {
                        field: 'createdAllTime',
                        displayName: 'Created all time',
                    },
                    {
                        field: 'archivedAllTime',
                        displayName: 'Archived all time',
                    },
                    {
                        field: 'lastSeen',
                        displayName: 'Last seen',
                        cellFilter: 'date:\'yyyy-MM-dd H:mm\'',
                    },
                    {
                        field: 'Action',
                        displayName: 'Action',
                        width: 100,
                        cellTemplate: '<button id="editBtn" type="button" class="btn btn-xs btn-info"  ng-click="grid.appScope.statistics.CompanyDetailStatistics(row.entity.companyId)" >Detail</button>'
                    }
                ]
            };
        };

        statistics.GetStatistics = function () {
            statisticsService.GetStatisticsDetails(function (response) {
                if (response.success) {
                    statistics.statisticsVm.created = response.result.created
                    statistics.statisticsVm.archived = response.result.archived
                    statistics.statisticsVm.sentForVerification = response.result.sentForVerification
                    statistics.statisticsVm.acceptedVerfication = response.result.acceptedVerfication
                    statistics.statisticsVm.deniedVerification = response.result.deniedVerification
                    statistics.details = response.result.details;
                }
                else {
                    console.log('error statistics loading');
                }
            });
        };

        statistics.CompanyDetailStatistics = function (companyId) {
            $location.path('/Statistics/Details/' + companyId);
        };

        statistics.Init();
    }
})();