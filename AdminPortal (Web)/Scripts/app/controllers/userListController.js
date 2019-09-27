(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .controller('userListController', userListController);

    userListController.$inject = ['$location', 'userService', 'Flash', '$rootScope'];

    function userListController($location, userService, flash, $rootScope) {
        /* jshint validthis:true */
        var user = this;
        user.title = 'userListController';

        user.users = [];
        user.userVm = {};

        user.Init = function () {
            user.GetUsers();
            user.InitializeGrid();
        };

        user.InitializeGrid = function () {
            user.gridOptions = {
                data: 'user.users',
                enableColumnResize: true,
                columnDefs: [
                    {
                        field: 'userId',
                        displayName: 'User Id',
                        width: 150,
                    },
                    {
                        field: 'displayName',
                        displayName: 'Display Name',
                        width: 200,
                    },
                    {
                        field: 'userName',
                        displayName: 'User Name',
                        width: 200,
                    },
                    {
                        field: 'phone',
                        displayName: 'Phone',
                        width: 100,
                    },
                     {
                         field: 'role',
                         displayName: 'Role',
                         width: 100,
                     },
                    {
                        field: 'companyName',
                        displayName: 'Company',
                        width: 100,
                    },
                    {
                        field: 'isActive',
                        displayName: 'Active',
                        width: 80,
                    },
                    {
                        field: 'Action',
                        displayName: 'Action',
                        width: 100,
                        cellTemplate: '<button id="editBtn" type="button" class="btn btn-xs btn-info"  ng-click="grid.appScope.user.EditUser(row.entity.userId)" >Edit</button>'
                    }
                ]
            };
        };

        user.GetUsers = function () {
            user.userVm.companyId = $rootScope.companyId;
            userService.GetUserList(user.userVm.companyId, function (response) {
                if (response.success) {
                    user.users = response.result;
                }
                else {
                    console.log('error user loading');
                }
            });
        };

        user.CreateNewUser = function () {
            $location.path('/User/Create');
        };

        user.EditUser = function (userId) {
            $location.path('/User/Edit/' + userId);
        };

        user.Init();

    }
})();
