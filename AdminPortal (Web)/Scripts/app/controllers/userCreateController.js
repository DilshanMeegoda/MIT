(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('userCreateController', userCreateController);

    userCreateController.$inject = ['$location', 'userService', 'Flash', '$rootScope', '$scope'];

    function userCreateController($location, userService, flash, $rootScope, $scope) {
        /* jshint validthis:true */
        var user = this;
        user.title = 'userCreateController';
        user.userVm = {};

        user.userVm.myImage = '';
        user.userVm.myCroppedImage = '';

        user.roles = [
          { id: 1, name: 'Super Admin' },
          { id: 2, name: 'Admin' },
          { id: 3, name: 'Normal' },
          { id: 4, name: 'CompanyAdmin' }
        ];

        user.userVm.companyId = $rootScope.companyId;
        user.userVm.roleId = 3;
        user.CreateUser = function () {
            user.UploadImage(function () {
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
                userService.CreateUser(user.userVm, function (response) {
                    var message;
                    if (response.success) {
                        message = '<strong> User Creation Sucess..! </strong> ';
                        $location.path('/User');
                        flash.create('success', message, 'custom-class');
                    } else {
                        message = '<strong> User Creation Fail..! </strong> .';
                        flash.create('danger', message, 'custom-class');
                    }
                });
            });
        };

        user.CropImage = function (event) {
            var file = event.target.files[0];
            if (file) {
                // ng-img-crop
                var imageReader = new FileReader();
                imageReader.onload = function (image) {
                    $scope.$apply(function () {
                        user.userVm.myImage = image.target.result;
                    });
                };
                imageReader.readAsDataURL(file);
            }
        };

        user.UploadImage = function (callback) {
            var blob = user.DataURItoBlob(user.userVm.myCroppedImage);
            userService.UploadFileToUrl(blob, function (response) {
                user.userVm.imageName = response.fileName;
                callback(response);
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
    }
})();
