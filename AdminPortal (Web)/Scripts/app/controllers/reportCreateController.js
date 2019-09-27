(function () {
    'use strict';
    var angularApp = angular.module('angularjsFormBuilderApp');

    angularApp.controller('reportCreateController', function ($scope, reportService, $location, Flash) {

        var report = this;
        report.title = 'reportCreateController';
        report.reportVm = {};

        report.reportVm.models = {
            selected: null,
            Elements: [],
            Zones: {
                "Header": [],
                "Details": []
            }
        };

        report.reportVm.name = 'Sample Report';
        report.reportVm.lastelementid = 0;
        report.reportVm.headertemplate = report.reportVm.models.Zones.Header;
        report.reportVm.detailtemplate = report.reportVm.models.Zones.Details;
        report.reportVm.models.Elements = reportService.fields;

        report.reportcategories = [
         { id: null, name: '--Select--' },
         { id: 1, name: 'Management' },
         { id: 2, name: 'Overall' },
         { id: 3, name: 'General' },
         { id: 4, name: 'Security' },
         { id: 5, name: 'Fire' },
         { id: 6, name: 'Garden' },
         { id: 7, name: 'Floor' },
         { id: 8, name: 'Unit' },
         { id: 9, name: 'Electricity' },
         { id: 10, name: 'Water' }
        ];

        report.saveForm = function () {
            var message;
            if (report.reportVm.headertemplate == null || report.reportVm.headertemplate.length == 0) {
                message = '<strong> Please add Fields to Header...! </strong> ';
                Flash.create('danger', message, 'custom-class');
            } else if (report.reportVm.detailtemplate == null || report.reportVm.detailtemplate.length == 0) {
                message = '<strong> Please add Fields to Detail...! </strong> ';
                Flash.create('danger', message, 'custom-class');
            }
            else {
                reportService.IsReportNameValid(report.reportVm.name, function (response) {
                    if (response.success && response.result) {
                        reportService.saveForm(report.reportVm, function (response) {
                            if (response.success) {
                                message = '<strong> Report Creation Sucess..! </strong> ';
                                $location.path('/Report');
                                Flash.create('success', message, 'custom-class');
                            } else {
                                message = '<strong> Report Creation Fail..! </strong> .';
                                Flash.create('danger', message, 'custom-class');
                            }
                        });
                    }
                    else {
                        message = '<strong> Report name already exist please use different name..! </strong> .';
                        Flash.create('danger', message, 'custom-class');
                    }
                });
            }
        };

        $scope.$watch('models', function (model) {
            $scope.modelAsJson = angular.toJson(model, true);
        }, true);

        $scope.addNewItem = function (items) {
            items.push({
                name: "",
                value: "false",
                condition: false,
                child: [[]]
            });
        };

        $scope.dropCallback = function (event, index, item) {
            if (item.id == 0 || event.ctrlKey) {
                report.reportVm.lastelementid++;
                item.id = report.reportVm.lastelementid;
            }
            return item;
        };

        $scope.logEvent = function (message) {
            console.log(message + 'Last inserted Id' + report.reportVm.lastelementid);
        };

    });
})();