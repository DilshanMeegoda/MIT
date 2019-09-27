(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('userEditController', userEditController);

    userEditController.$inject = ['$location', 'userService', 'Flash', '$rootScope', '$routeParams', '$scope'];

    function userEditController($location, userService, flash, $rootScope, $routeParams, $scope) {
        /* jshint validthis:true */
        var user = this;
        user.title = 'userEditController';
        user.userVm = {};
        //Image is not getting delete when modified the image. so need to implement image delete funtioality

        user.roles = [
          { id: 1, name: 'Super Admin' },
          { id: 2, name: 'Admin' },
          { id: 3, name: 'Normal' },
          { id: 4, name: 'CompanyAdmin' }
        ];


        user.myImage = '';
        user.myCroppedImage = '';
        user.profile = '';

        user.userVm.companyId = $rootScope.companyId;

        user.Init = function () {
            user.GetUser();
        };

        user.GetUser = function () {
            user.userVm.userId = $routeParams.userId;
            userService.GetUser(user.userVm.userId, function (response) {
                if (response.success) {
                    user.userVm = response.result;
                    user.DownloadImage();
                }
                else {
                    console.log('error user loading');
                }
            });
        };

        user.UpdateUser = function () {
            if (user.userVm.roleId == 1) {
                user.userVm.role = 'Super Admin';
            }
            if (user.userVm.roleId == 2) {
                user.userVm.role = 'Admin';
            }
            if (user.userVm.roleId == 3) {
                user.userVm.role = 'Normal';
            }
            if (user.userVm.roleId == 4) {
                user.userVm.role = 'CompanyAdmin';
            }
            userService.UpdateUser(user.userVm, function (response) {
                var message;
                if (response.success) {
                    message = '<strong> User Update Sucess..! </strong> ';
                    $location.path('/User');
                    flash.create('success', message, 'custom-class');
                } else {
                    message = '<strong> User Update Fail..! </strong> .';
                    flash.create('danger', message, 'custom-class');
                }
            });
        };

        user.CropImage = function (event) {
            var file = event.target.files[0];
            if (file) {
                // ng-img-crop
                var imageReader = new FileReader();
                imageReader.onload = function (image) {
                    $scope.$apply(function () {
                        user.myImage = image.target.result;
                    });
                };
                imageReader.readAsDataURL(file);
            }
        };

        user.UploadImage = function () {
            var blob = user.DataURItoBlob(user.myCroppedImage);
            userService.UploadFileToUrl(blob, function (response) {
                user.userVm.imageName = response.fileName;
            });
        };

        user.DataURItoBlob = function (dataUri) {
            var binary = atob(dataUri.split(',')[1]);
            var mimeString = dataUri.split(',')[0].split(':')[1].split(';')[0];
            var array = [];
            for (var i = 0; i < binary.length; i++) {
                array.push(binary.charCodeAt(i));
            }
            return new Blob([new Uint8Array(array)], { type: mimeString });
        };

        user.DownloadImage = function () {
            userService.DownloadImage(user.userVm.imageName, function (response) {
                if (response.success) {
                    user.profile = response.result;
                } else {
                    console.log('error image loading');
                }
            });
        };

        user.Init();
    }
})();
