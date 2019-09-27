(function () {
    'use strict';

    var angularApp = angular.module('angularjsFormBuilderApp', ['ui.bootstrap', 'ngRoute', 'mgcrea.ngStrap', 'flash', 'ui.grid', 'app.config', 'dndLists', 'ngImgCrop', 'ngTagsInput', 'ui.grid.expandable', 'ui.grid.selection', 'ui.grid.pinning']);

    angularApp.config(function ($routeProvider, $locationProvider, $httpProvider) {

        $httpProvider.defaults.headers.common['Token'] = sessionStorage.getItem('Token');

        $routeProvider
            .when('/', {
                templateUrl: 'views/Home/home.html',
                controller: 'HeaderCtrl',
            })
             .when('/Statistics', {
                 templateUrl: 'views/Statistics/index.html',
                 controller: 'statisticsController',
                 controllerAs: 'statistics'
             })
            .when('/Statistics/Details/:companyId', {
                templateUrl: 'views/Statistics/statisticsdetails.html',
                controller: 'statisticsDetailsController',
                controllerAs: 'statisticsDetails'
            })
            .when('/Login', {
                templateUrl: 'views/Login/login.html',
                controller: 'loginController',
            })
             .when('/Report', {
                 templateUrl: 'views/Report/index.html',
                 controller: 'reportListController',
                 controllerAs: 'report'
             })
            .when('/Report/Create', {
                templateUrl: 'views/Report/create.html',
                controller: 'reportCreateController',
                controllerAs: 'report'
            })
            .when('/Report/Edit/:reportId', {
                templateUrl: 'views/Report/edit.html',
                controller: 'reportEditController',
                controllerAs: 'report'
            })
            .when('/Company', {
                templateUrl: 'views/Company/index.html',
                controller: 'companyListController',
                controllerAs: 'company'
            })
            .when('/Company/Create', {
                templateUrl: 'views/Company/create.html',
                controller: 'companyCreateController',
                controllerAs: 'company'
            })
            .when('/Company/Edit/:companyId', {
                templateUrl: 'views/Company/edit.html',
                controller: 'companyEditController',
                controllerAs: 'company'
            })
            .when('/Project', {
                templateUrl: 'views/Project/index.html',
                controller: 'projectController',
                controllerAs: 'project'
            })
            .when('/Rejects', {
                templateUrl: 'views/Rejects/index.html',
                controller: 'rejectsController',
                controllerAs: 'rejects'
            })
            .when('/Project/Create', {
                templateUrl: 'views/Project/create.html',
                controller: 'projectController',
                controllerAs: 'project'
            })
            .when('/Project/Edit/:projectId', {
                templateUrl: 'views/Project/edit.html',
                controller: 'projectEditController',
                controllerAs: 'project'
            })
            .when('/User', {
                templateUrl: 'views/User/index.html',
                controller: 'userListController',
                controllerAs: 'user'
            })
            .when('/User/Create', {
                templateUrl: 'views/User/create.html',
                controller: 'userCreateController',
                controllerAs: 'user'
            })
            .when('/User/Edit/:userId', {
                templateUrl: 'views/User/edit.html',
                controller: 'userEditController',
                controllerAs: 'user'
            })

            .otherwise({
                redirectTo: '/',
            });
        $locationProvider.html5Mode(true);
    });

    angularApp.run(function (authentication, $rootScope, $location) {
        $rootScope.$on('$routeChangeStart', function (evt) {
            if (!authentication.isAuthenticated) {
                $location.url("/");
            }
            ////event.preventDefault();
        });
    });
})();


