(function () {
    'use strict';

    angular
        .module('angularjsFormBuilderApp')
        .service('reportService', reportService);

    reportService.$inject = ['$http', 'app.config'];

    function reportService($http, config) {

        var self = this;
        self.url = config.basePath + "Report/";
           
        return {

            fields: [
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "textfield",
                            typeAlies: "Text Field",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                               { id: 4, code: "isdescription", type: "checkbox", name: "Is Description", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "textfieldint",
                            typeAlies: "Text Field (Number)",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "slider",
                            typeAlies: "Slider",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typeyesno",
                            type: "yesno",
                            typeAlies: "Yes/No",
                            info: "",
                            filledby: "",
                            value: "",
                            accepted: [[]],
                            rejected: [[]],
                            options: [
                                 { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                                 { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                                 { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                                 { id: 4, code: "conditional", type: "checkbox", name: "Conditional", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "multilinetextfield",
                            typeAlies: "Multiline Text",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                               { id: 4, code: "isdescription", type: "checkbox", name: "Is Description", value: "false" }

                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "date",
                            typeAlies: "Date",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                                 { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                                 { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                                 { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                                 { id: 4, code: "autogenarated", type: "checkbox", name: "Auto Generated", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "time",
                            typeAlies: "Time",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        //{
                        //    id: 0,
                        //    title: "",
                        //    templateType: "typedefault",
                        //    type: "camera",
                        //    typeAlies: "Camera",
                        //    info: "",
                        //    filledby: "",
                        //    value: "",
                        //    values: [],
                        //    options: [
                        //       { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                        //       { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                        //       { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                        //    ],
                        //    sortOrder: 0
                        //},
                        //{
                        //    id: 0,
                        //    title: "",
                        //    templateType: "typedefault",
                        //    type: "gps",
                        //    typeAlies: "GPS",
                        //    info: "",
                        //    filledby: "",
                        //    value: "",
                        //    values: [],
                        //    options: [
                        //       { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                        //       { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                        //       { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                        //    ],
                        //    sortOrder: 0
                        //},
                        //{
                        //    id: 0,
                        //    title: "",
                        //    templateType: "typedefault",
                        //    type: "signature",
                        //    typeAlies: "Signature",
                        //    info: "",
                        //    filledby: "",
                        //    value: "",
                        //    options: [
                        //       { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                        //       { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                        //       { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                        //    ],
                        //    sortOrder: 0
                        //},
                        {
                            id: 0,
                            title: "",
                            templateType: "typecheckbox",
                            type: "checkbox",
                            typeAlies: "Check Box",
                            info: "",
                            filledby: "",
                            isMultiSelect: true,
                            isVertical: false,
                            values: [],
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                               { id: 4, code: "ismultiselect", type: "checkbox", name: "Multi Select", value: "false" },
                               { id: 5, code: "isvertical", type: "checkbox", name: "Vertical", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedropdown",
                            type: "dropdown",
                            typeAlies: "Drop Down",
                            info: "",
                            filledby: "",
                            isMultiSelect: false,
                            values: [],
                            addNew: false,
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" },
                               { id: 4, code: "ismultiselect", type: "checkbox", name: "Multi Select", value: "false" },
                               { id: 5, code: "addnew", type: "checkbox", name: "Add New", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "updown",
                            typeAlies: "Up Down Arrow",
                            info: "",
                            filledby: "",
                            value: "",
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                            ],
                            sortOrder: 0
                        },
                        {
                            id: 0,
                            title: "",
                            templateType: "typedefault",
                            type: "mainandsubfield",
                            typeAlies: "Main and Sub Field",
                            info: "",
                            filledby: "",
                            values: [{ id: 1, name: "main", value: "" }, { id: 2, name: "subfield", value: "" }],
                            options: [
                               { id: 1, code: "quater", type: "checkbox", name: "Quater", value: "false" },
                               { id: 2, code: "half", type: "checkbox", name: "Half", value: "false" },
                               { id: 3, code: "required", type: "checkbox", name: "Required", value: "false" }
                            ],
                            sortOrder: 0
                        }
            ],

            GetReport: function (reportId, callback) {
                var response = {};
                response.success = false;
                $http({
                    cache: false,
                    url: self.url + 'GetReport/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    params: { reportId: reportId }
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            saveForm: function (data, callback) {
                var responseresult = { success: false };
                var reportTemplate = {
                    ReportName: data.name,
                    IsStandard: data.isstandard,
                    ReportCategoryId: data.reportcategoryid,
                    HeaderTemplate: data.headertemplate,
                    DetailTemplate: data.detailtemplate,
                    LastElementId: data.lastelementid
                };

                $http({
                    cache: false,
                    url: self.url + 'PostReport/',
                    method: "POST",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    data: reportTemplate
                }).success(function (response) {
                    if (response != null) {
                        responseresult = { success: true };
                    }
                    callback(responseresult);
                }).error(function () {
                    callback(responseresult);
                });
            },

            UpdateReport: function (data, callback) {
                var responseresult = { success: false };
                var reportTemplate = {
                    ReportId: data.reportId,
                    ReportName: data.name,
                    IsStandard: data.isstandard,
                    ReportCategoryId: data.reportcategoryid,
                    HeaderTemplate: data.models.Zones.Header,
                    DetailTemplate: data.models.Zones.Details,
                    CreatedDateTime: data.createdDateTime,
                    LastElementId: data.lastelementid
                };
                $http({
                    cache: false,
                    url: self.url + 'UpdateReport/',
                    method: "POST",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    data: reportTemplate
                }).success(function (response) {
                    if (response != null) {
                        responseresult = { success: true };
                    }
                    callback(responseresult);
                }).error(function () {
                    callback(responseresult);
                });
            },

            GetReportList: function (callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'GetAllReports/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            GetReportListToDropDown: function (callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'GetActiveReportsToDropDown/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            GetReportListToDropDownByCompany: function (companyId, callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'GetActiveReportsToDropDown/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    params: { companyId: companyId }
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            IsReportNameValid: function (reportName, callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'IsReportNameValid/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    params: { reportName: reportName }
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            Delete: function (reportId, callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'Delete/',
                    method: "POST",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    data: reportId
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },

            GetReportListByTerm: function (term, callback) {
                var response = {};
                response.success = false;
                response.result = [];
                $http({
                    cache: false,
                    url: self.url + 'GetReportsByTerm/',
                    method: "GET",
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    params: { searchterm: term }
                }).success(function (data) {
                    if (data != null) {
                        response.success = true;
                        response.result = data;
                    } else {
                        response.success = false;
                    }
                    callback(response);
                }).error(function () {
                    console.log('error in service');
                    callback(response);
                });
            },
        };
    };
})();
