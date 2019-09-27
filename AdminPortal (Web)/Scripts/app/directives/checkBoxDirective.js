(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .directive('checkBox', checkBoxDirective);

    checkBoxDirective.$inject = ['$window', '$compile'];

    function checkBoxDirective($window, $compile) {
        // Usage:
        //     <checkBoxDirective></checkBoxDirective>
        // Creates:
        // 
        var directive = {
            scope: { multiItem: "=" },
            link: link,
            restrict: 'EA',
            template: ' <input type="text" ng-model="multiItem.name" class="form-control input-sm" placeholder="Item Name"> <div class="fa fa-times delete" ng-click=""> </div>'
        };
        return directive;

        function link(scope, element, attrs) {

            //scope.multiItem.addNewItem = function () {
            //    //scope.multiItem.push({
            //    //    name: "",
            //    //    value: ""
            //    //});
            //    var x = scope;
            //    //element.after($compile('<check-box name="itemName"></check-box>')(scope));
            //};
        }
    }

})();