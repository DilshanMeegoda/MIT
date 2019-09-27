(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('statisticsDetailsController', statisticsDetailsController);

    statisticsDetailsController.$inject = ['$location', 'statisticsService', 'Flash', '$routeParams', '$scope', '$timeout'];

    function statisticsDetailsController($location, statisticsService, Flash, $routeParams, $scope, $timeout) {
        var statisticsDetails = this;

        statisticsDetails.title = 'statisticsDetailsController';
        statisticsDetails.statisticsDetailsVm = {};
        statisticsDetails.templateDetails = [];
        statisticsDetails.projectDetails = [];
        statisticsDetails.reportDetails = [];

        statisticsDetails.Init = function () {
            statisticsDetails.loading = true;

            $timeout(function () {
                statisticsDetails.GetTemplateAndProjectStatistics();
            }, 500);
            statisticsDetails.InitializeTemplateStatisticsGrid();
            statisticsDetails.InitializeProjectStatisticsGrid();
        };

        statisticsDetails.GetTemplateAndProjectStatistics = function () {
            statisticsDetails.statisticsDetailsVm.companyId = $routeParams.companyId;
            statisticsService.GetStatisticsByCompany(statisticsDetails.statisticsDetailsVm.companyId, function (response) {
                if (response) {
                    statisticsDetails.statisticsDetailsVm.companyName = response.result.companyName;
                    statisticsDetails.statisticsDetailsVm.created = response.result.created;
                    statisticsDetails.statisticsDetailsVm.archived = response.result.archived;
                    statisticsDetails.statisticsDetailsVm.sentForVerification = response.result.sentForVerification;
                    statisticsDetails.statisticsDetailsVm.acceptedVerfication = response.result.acceptedVerfication;
                    statisticsDetails.statisticsDetailsVm.deniedVerification = response.result.deniedVerification;
                    statisticsDetails.templateDetails = response.result.templateStatistics;
                    statisticsDetails.projectDetails = response.result.projectStatistics;
                    statisticsDetails.loading = false;
                }
                else {
                    console.log('error statisticsDetails loading');
                }
            });
        };

        statisticsDetails.GetStatisticsByCompanyAndReport = function () {
            statisticsDetails.statisticsDetailsVm.companyId = $routeParams.companyId;
            statisticsService.GetStatisticsByCompanyAndReport(statisticsDetails.statisticsDetailsVm.companyId, statisticsDetails.reportId, function (response) {
                if (response) {
                    statisticsDetails.reportDetails = response.result;
                }
                else {
                    console.log('error statisticsDetails loading');
                }
            });
        };

        statisticsDetails.InitializeTemplateStatisticsGrid = function () {
            statisticsDetails.templategridOptions = {
                data: 'statisticsDetails.templateDetails',
                enableColumnResize: true,
                expandableRowTemplate: 'views/Statistics/expandableRowTemplate.html',
                expandableRowHeight: 150,
                columnDefs: [
                    {
                        field: 'templateName',
                        displayName: 'Template',
                    },
                    {
                        field: 'created',
                        displayName: 'Created',
                    },
                    {
                        field: 'archived',
                        displayName: 'Archived',
                    },
                    {
                        field: 'lastSeen',
                        displayName: 'Last seen',
                        cellFilter: 'date:\'yyyy-MM-dd H:mm\'',
                    },
                     {
                         field: 'total',
                         displayName: 'Total',
                     }
                ],

                onRegisterApi: function (gridApi) {
                    gridApi.expandable.on.rowExpandedStateChanged($scope, function (row) {
                        if (row.isExpanded) {
                            row.entity.subGridOptions = {
                                columnDefs: [
                                 {
                                     field: 'reportNo',
                                     displayName: 'Report No',
                                     width: 100,
                                 },
                                 {
                                     field: 'projectName',
                                     displayName: 'Project Name',
                                 },
                                 {
                                     field: 'name',
                                     displayName: 'Name',
                                 },
                                 {
                                     field: 'description',
                                     displayName: 'Description',
                                 },
                                 {
                                     field: 'mail',
                                     displayName: 'Mail',
                                 },
                                 {
                                     field: 'status',
                                     displayName: 'Status',
                                     width: 200,
                                 },
                                 {
                                     field: 'lastSeen',
                                     displayName: 'Last seen',
                                     cellFilter: 'date:\'yyyy-MM-dd H:mm\'',
                                 }
                                ]
                            };

                            statisticsDetails.loading = true;

                            $timeout(function () {
                                statisticsService.GetStatisticsByCompanyAndReport($routeParams.companyId, row.entity.reportId, function (response) {
                                    if (response) {
                                        row.entity.subGridOptions.data = response.result;
                                        statisticsDetails.loading = false;
                                    }
                                    else {
                                        console.log('error statisticsDetails loading');
                                    }
                                });
                            }, 500);
                        }
                    });
                }
            };
        };

        statisticsDetails.InitializeProjectStatisticsGrid = function () {
            statisticsDetails.projectgridOptions = {
                data: 'statisticsDetails.projectDetails',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'projectName',
                        displayName: 'Project',
                    },
                    {
                        field: 'noOfTemplates',
                        displayName: 'Templates',
                    },
                    {
                        field: 'created',
                        displayName: 'Created',
                    },
                    {
                        field: 'lastSeen',
                        displayName: 'Last seen',
                        cellFilter: 'date:\'yyyy-MM-dd H:mm\'',
                    },
                    {
                        field: 'total',
                        displayName: 'Total',
                    }
                ]
            };
        };

        statisticsDetails.Init();
    }

})();