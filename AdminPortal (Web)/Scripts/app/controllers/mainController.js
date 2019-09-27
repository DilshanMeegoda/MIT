(function () {
    'use strict';
    var angularApp = angular.module('angularjsFormBuilderApp');
    angularApp.controller('mainController', mainController);

    mainController.$inject = ['$scope'];

    function mainController($scope) {
        $scope.template = 'views/Login/login.html';
    };

})();
