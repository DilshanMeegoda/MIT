(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .directive('customOnChange', customOnChange);

    customOnChange.$inject = ['$window'];

    function customOnChange($window) {

        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var onChangeHandler = scope.$eval(attrs.customOnChange);
                element.bind('change', onChangeHandler);
            }
        };
    }

})();