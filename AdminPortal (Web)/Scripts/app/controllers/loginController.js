(function () {
    'use strict';

    angular.module('angularjsFormBuilderApp')
           .controller('loginController', loginController);

    loginController.$inject = ['$location', 'loginService', '$scope', 'Flash', '$rootScope', 'authentication', 'sessionService'];

    function loginController($location, loginService, $scope, flash, $rootScope, authentication, sessionService) {
        /* jshint validthis:true */
        var user = this;
        user.email = "";
        user.password = "";
        user.apptype = 7;
        user.title = 'loginController';
        user.template = 'views/Login/login.html';
        user.login = function () {
            loginService.ValidateUser(user, function (response) {
                if (response.success) {
                    //debugger;
                    $rootScope.role = response.result.role;
                    if (response.result.companyId === null) {
                        $rootScope.companyId = '';
                    } else {
                        $rootScope.companyId = response.result.companyId;
                    }
                    authentication.isAuthenticated = true;
                    user.template = 'views/Home/home.html';
                } else {
                    var message = '<strong> Login Fail..! </strong>  Please Check Your Credential And Try Again.';
                    flash.create('danger', message, 'custom-class');
                }
            });
        };

        user.logout = function () {
            //sessionService.destroy('Token');
            authentication.isAuthenticated = false;
            user.template = 'views/Login/login.html';
        };
    }
})();
