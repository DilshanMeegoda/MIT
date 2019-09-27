(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('companyCreateController', companyController);

    companyController.$inject = ['$location', 'companyService', 'reportService', 'Flash', '$scope'];

    function companyController($location, companyService, reportService, flash, $scope) {
        /* jshint validthis:true */
        var company = this;

        company.title = 'companyCreateController';

        company.companyVm = {};
        company.reports = [];

        company.companyVm.myImage = '';
        company.companyVm.myCroppedImage = '';

        company.Init = function () {
            company.GetReports();
        };

        company.CreateCompany = function () {
            company.UploadImage(function () {
                companyService.CreateCompany(company.companyVm, function (response) {
                    var message;
                    if (response.success) {
                        message = '<strong> Company Creation Sucess..! </strong> ';
                        $location.path('/Company');
                        flash.create('success', message, 'custom-class');
                    } else {
                        message = '<strong> Company Creation Fail..! </strong> .';
                        flash.create('danger', message, 'custom-class');
                    }
                });
            });
        };

        company.GetReports = function () {
            reportService.GetReportListToDropDown(function (response) {
                if (response.success) {
                    company.reports = response.result;
                } else {
                    console.log('error report loading');
                }
            });
        };

        company.CropImage = function (event) {
            var file = event.target.files[0];
            if (file) {
                // ng-img-crop
                var imageReader = new FileReader();
                imageReader.onload = function (image) {
                    $scope.$apply(function () {
                        company.companyVm.myImage = image.target.result;
                    });
                };
                imageReader.readAsDataURL(file);
            }
        };

        company.UploadImage = function (callback) {
            var blob = company.DataURItoBlob(company.companyVm.myCroppedImage);
            companyService.UploadFileToUrl(blob, function (response) {
                company.companyVm.imageName = response.fileName;
                callback(response);
            });
        };

        company.DataURItoBlob = function (dataUri) {
            var binary = atob(dataUri.split(',')[1]);
            var mimeString = dataUri.split(',')[0].split(':')[1].split(';')[0];
            var array = [];
            for (var i = 0; i < binary.length; i++) {
                array.push(binary.charCodeAt(i));
            }
            return new Blob([new Uint8Array(array)], { type: mimeString });
        };

        company.GetReportsByTerm = function ($query) {
            return company.reports.filter(function (report) {
                return report.name.toLowerCase().indexOf($query.toLowerCase()) != -1;
            });
        };

        company.Init();
    }
})();
