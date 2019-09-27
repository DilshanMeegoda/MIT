(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('companyEditController', companyEditController);

    companyEditController.$inject = ['$location', 'companyService', 'reportService', 'Flash', '$routeParams', '$scope'];

    function companyEditController($location, companyService, reportService, flash, $routeParams, $scope) {
        /* jshint validthis:true */
        var company = this;
        company.title = 'companyEditController';
        company.companyVm = {};
        //Image is not getting delete when modified the image. so need to implement image delete funtioality

        company.reports = [];

        company.myImage = '';
        company.myCroppedImage = '';
        company.logo = '';

        company.Init = function () {
            company.GetCompany();
            company.GetReports();
        };

        company.GetCompany = function () {
            company.companyVm.companyId = $routeParams.companyId;
            companyService.GetCompany(company.companyVm.companyId, function (response) {
                if (response.success) {
                    company.companyVm = response.result;
                    company.DownloadImage();
                }
                else {
                    console.log('error Project loading');
                }
            });
        };

        company.UpdateCompany = function () {
            companyService.UpdateCompany(company.companyVm, function (response) {
                var message;
                if (response.success) {
                    message = '<strong> Company Update Sucess..! </strong> ';
                    $location.path('/Company');
                    flash.create('success', message, 'custom-class');
                } else {
                    message = '<strong> Company Update Fail..! </strong> .';
                    flash.create('danger', message, 'custom-class');
                }
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
                        company.myImage = image.target.result;
                    });
                };
                imageReader.readAsDataURL(file);
            }
        };

        company.UploadImage = function () {
            var blob = company.DataURItoBlob(company.myCroppedImage);
            companyService.UploadFileToUrl(blob, function (response) {
                company.companyVm.imageName = response.fileName;
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

        company.DownloadImage = function () {
            companyService.DownloadImage(company.companyVm.imageName,function(response) {
                if (response.success) {
                    company.logo = response.result;
                } else {
                    console.log('error image loading');
                }
            });
        };

        company.GetReportsByTerm = function ($query) {
            return company.reports.filter(function (report) {
                return report.name.toLowerCase().indexOf($query.toLowerCase()) != -1;
            });
        };

        company.Init();
    }
})();
