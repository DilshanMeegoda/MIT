(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('companyListController', companyListController);

    companyListController.$inject = ['$location', 'companyService', 'Flash'];

    function companyListController($location, companyService, flash) {
        /* jshint validthis:true */
        var company = this;
        company.title = 'companyListController';

        company.companies = [];
        company.companyVm = {};

        company.Init = function () {
            company.GetCompanyList();
            company.InitializeGrid();
        };

        company.InitializeGrid = function () {
            company.gridOptions = {
                data: 'company.companies',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'companyId',
                        displayName: 'Company Id',
                        width: 150,
                    },
                    {
                        field: 'name',
                        displayName: 'Name',
                        width: 200,
                    },
                    {
                        field: 'address',
                        displayName: 'Address',
                        width: 200,
                    },
                    {
                        field: 'email',
                        displayName: 'E-Mail',
                        width: 200,
                    },
                    {
                        field: 'phone',
                        displayName: 'Phone',
                        width: 100,
                    },
                    {
                        field: 'isPaid',
                        displayName: 'Paid',
                        width: 80,
                    },
                    {
                        field: 'isActive',
                        displayName: 'Active',
                        width: 80,
                    },
                    {
                        field: 'Action',
                        displayName: 'Action',
                        width: 100,
                        cellTemplate: '<button id="editBtn" type="button" class="btn btn-xs btn-info"  ng-click="grid.appScope.company.EditCompany(row.entity.companyId)" >Edit</button>'
                    }
                ]
            };
        };

        company.GetCompanyList = function () {
            companyService.getComapanyList(function (response) {
                if (response.success) {
                    company.companies = response.result;
                } else {
                    console.log('error company loading');
                }
            });
        };

        company.CreateNewCompany = function () {
            $location.path('/Company/Create');
        };

        company.EditCompany = function (companyId) {
            $location.path('/Company/Edit/' + companyId);
        };

        company.GetCompaniesByTerm = function () {
            var searchterm = company.companyVm.term;
            companyService.CompaniesByTerm(searchterm, function (response) {
                if (response.success) {
                    company.companies = response.result;
                }
                else {
                    console.log('error user loading');
                }
            });
        };

        company.Init();
    }
})();
