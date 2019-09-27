(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('reportEditController', reportEditController);

    reportEditController.$inject = ['$location', 'reportService', '$routeParams', 'Flash', '$scope'];

    function reportEditController($location, reportService, $routeParams, flash, $scope) {
        /* jshint validthis:true */
        var report = this;
        report.title = 'reportEditController';
        report.reportVm = {};

        report.reportVm.models = {
            selected: null,
            Elements: [],
            Zones: {
                "Header": [],
                "Details": []
            }
        };

        report.reportVm.models.Elements = reportService.fields;
        report.reportVm.models.Zones.Header = [];
        report.reportVm.models.Zones.Details = [];

        report.reportcategories = [
        { id: null, name: '--Select--' },
        { id: 1, name: 'Standard' },
        { id: 2, name: 'HMS' },
        { id: 3, name: 'General' },
        { id: 4, name: 'Jernbane' },
        { id: 5, name: 'Stillas' },
        { id: 6, name: 'Inneklima' },
        { id: 7, name: 'Anlegg' },
        { id: 8, name: 'Bygg' },
        { id: 9, name: 'Kontrakter' },
        { id: 10, name: 'EI' }
        ];

        report.Init = function () {
            report.GetReport();
        };

        report.GetReport = function () {
            report.reportVm.reportId = $routeParams.reportId;
            reportService.GetReport(report.reportVm.reportId, function (response) {
                if (response.success) {
                    report.report = response.result;
                    report.reportVm.name = report.report.reportName;
                    report.reportVm.isstandard = report.report.isStandard;
                    report.reportVm.reportcategoryid = report.report.reportCategoryId;
                    report.reportVm.lastelementid = report.report.lastElementId;
                    report.reportVm.createdDateTime = report.report.createdDateTime;
                    report.reportVm.models.Zones.Header = angular.fromJson(report.report.headerTemplateJson);
                    report.reportVm.models.Zones.Details = angular.fromJson(report.report.detailTemplateJson);
                }
                else {
                    console.log('error Project loading');
                }
            });
        };

        report.UpdateReport = function () {
            var message;
            if (report.reportVm.models.Zones.Header == null || report.reportVm.models.Zones.Header.length == 0) {
                message = '<strong> Please add Fields to Header...! </strong> ';
                flash.create('danger', message, 'custom-class');
            }
            else if (report.reportVm.models.Zones.Details == null || report.reportVm.models.Zones.Details.length == 0) {
                message = '<strong> Please add Fields to Details...! </strong> ';
                flash.create('danger', message, 'custom-class');
            } else {
                reportService.UpdateReport(report.reportVm, function (response) {
                    if (response.success) {
                        message = '<strong> Report Updation Sucess..! </strong> ';
                        $location.path('/Report');
                        flash.create('success', message, 'custom-class');
                    } else {
                        message = '<strong> Report Updation Fail..! </strong> .';
                        flash.create('danger', message, 'custom-class');
                    }
                });
            }
        };

        report.SaveAsNewReport = function () {
            var message;
            report.reportVm.reportId = 0;
            report.reportVm.name = report.reportVm.name + ' - (Copy)';
            report.reportVm.headertemplate = report.reportVm.models.Zones.Header
            report.reportVm.detailtemplate = report.reportVm.models.Zones.Details
            if (report.reportVm.models.Zones.Header == null || report.reportVm.models.Zones.Header.length == 0) {
                message = '<strong> Please add Fields to Header...! </strong> ';
                flash.create('danger', message, 'custom-class');
            }
            else if (report.reportVm.models.Zones.Details == null || report.reportVm.models.Zones.Details.length == 0) {
                message = '<strong> Please add Fields to Details...! </strong> ';
                flash.create('danger', message, 'custom-class');
            }
            else {
                reportService.IsReportNameValid(report.reportVm.name, function (response) {
                    if (response.success && response.result) {
                        reportService.saveForm(report.reportVm, function (response) {
                            if (response.success) {
                                message = '<strong> Report created as new report..! </strong> ';
                                $location.path('/Report');
                                flash.create('success', message, 'custom-class');
                            } else {
                                message = '<strong> Report creation Fail..! </strong> .';
                                flash.create('danger', message, 'custom-class');
                            }
                        });
                    }
                    else {
                        message = '<strong> Report name already exist please use different name..! </strong> .';
                        flash.create('danger', message, 'custom-class');
                    }
                });
            }
        };

        $scope.addNewItem = function (items) {
            items.push({
                name: "",
                value: "false",
                condition: false,
                child: [[]]
            });
        };

        $scope.dropCallback = function (event, index, item) {
            report.reportVm.lastelementid++;
            item.id = report.reportVm.lastelementid;
            return item;
        };

        $scope.logEvent = function (message) {
            console.log(message + 'Last inserted Id' + report.reportVm.lastelementid);
        };

        report.Delete = function () {
            var message;
            reportService.Delete(report.reportVm.reportId, function (response) {
                if (response.success) {
                    message = '<strong> Report Deleted..! </strong> ';
                    $location.path('/Report');
                    flash.create('success', message, 'custom-class');
                } else {
                    message = '<strong> Report Deletion Fail..! </strong> .';
                    flash.create('danger', message, 'custom-class');
                }
            })
        };

        report.Init();

    }
})();
