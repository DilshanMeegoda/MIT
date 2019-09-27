
(function () {
    'use strict';

    angular.module('angularjsFormBuilderApp')
           .factory('authentication', authentication);

    authentication.$inject = ['sessionService'];

    function authentication(sessionService) {
        return {
            isAuthenticated: function () {
                if (sessionService.get('user'))
                    return true;
                return false;
            },
            user: null
        };
    }
})();



//app.factory('authentication', function () {
//    return {
//        isAuthenticated: false,
//        user: null
//    }
//});