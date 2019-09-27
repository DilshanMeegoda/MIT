(function () {
    'use strict';
    var angularApp = angular.module('angularjsFormBuilderApp');

    angularApp.controller('HeaderCtrl', function ($scope, $location, $rootScope, companyService) {
        $scope.$location = $location;
        $scope.companyList = [];

        $rootScope.companyId = '';
        $scope.broadcast = function () {
            if ($scope.selectedCompany === null) {
                $rootScope.companyId = '';
            } else {
                $rootScope.companyId = $scope.selectedCompany.companyId;
            }
            $location.path('/');
        };

        $scope.GetCompanies = function () {
            companyService.getComapanyList(function (response) {
                if (response.success) {
                    $scope.companyList = response.result;
                    $scope.selectedCompany = $scope.companyList[0];
                    $rootScope.companyId = $scope.selectedCompany.companyId;
                } else {
                    console.log('error company loading');
                }
            });
            $location.path('/Report');
        };

        $scope.GetCompanies();
    });
})();
