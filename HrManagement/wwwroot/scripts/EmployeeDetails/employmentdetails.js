


function PopulateDropDowns(cb) {

    var ddDepartments = document.querySelector('#department');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });


    var ddSites = document.querySelector('#site');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });

    var ddPositions = document.querySelector('#position');
    $(ddPositions).empty();
    var lstPositions = (DropDownsData.EmployeePosition || []);
    var option = new Option();
    ddPositions.appendChild(option);
    lstPositions.forEach(function (item) {

        var option = new Option(item.PositionName, item.Id, false, false);
        ddPositions.appendChild(option);
    });


    var ddEmployeeStatus = document.querySelector('#employment-status');
    $(ddEmployeeStatus).empty();
    var lstEmployeeStatus = (DropDownsData.EmployeeStatus || []);
    var option = new Option();
    ddEmployeeStatus.appendChild(option);
    lstEmployeeStatus.forEach(function (item) {

        var option = new Option(item.EmployeeStatusName, item.Id, false, false);
        ddEmployeeStatus.appendChild(option);
    });

    var ddEmployeeLevel = document.querySelector('#employment-level');
    $(ddEmployeeLevel).empty();
    var lstEmployeeLevels = (DropDownsData.EmployeeLevels || []);
    var option = new Option();
    ddEmployeeLevel.appendChild(option);
    lstEmployeeLevels.forEach(function (item) {

        var option = new Option(item.EmployeeLevel, item.Id, false, false);
        ddEmployeeLevel.appendChild(option);
    });

    var ddManager = document.querySelector('#manager');
    $(ddManager).empty();
    var lstManagers = (DropDownsData.Managers || []);
    var option = new Option();
    ddManager.appendChild(option);
    lstManagers.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddManager.appendChild(option);
    });

    var ddRole = document.querySelector('#role');
    $(ddRole).empty();
    var lstRoles = (DropDownsData.Roles || []);
    var option = new Option();
    ddRole.appendChild(option);
    lstRoles.forEach(function (item) {
        var option = new Option(item.RoleName, item.RoleId, false, false);
        ddRole.appendChild(option);
    });

    if (cb) {
        cb();
    }


}


$(document).on('change', '#department', function () {
    var selectedDepartments = $("#department").val(); // This is an array if multiple selected

    if (selectedDepartments && selectedDepartments.length > 0) {
        var deptCategories = DropDownsData?.DepartmentSubCategories?.filter(function (item) {
            return selectedDepartments.includes(item.DepartmentId.toString()); // convert if needed
        });

        var ddDepartmentSubCategory = document.querySelector('#department-sub-category');
        $(ddDepartmentSubCategory).empty();

        var lstDepartmentSubCategories = (deptCategories || []);
        var option = new Option(); // You may want to add a label like "--Select--"
        ddDepartmentSubCategory.appendChild(option);

        lstDepartmentSubCategories.forEach(function (item) {
            var option = new Option(item.SubCategoryName, item.Id, false, false);
            ddDepartmentSubCategory.appendChild(option);
        });
    }
});



$(document).on("click", ".btn-employment-details", function () {


    $("#position").val(_EmployeeData.PositionId).trigger("change");
    $("#hiring-date").flatpickr({
        dateFormat: "d/m/Y",
    }).setDate(FormatDate(_EmployeeData.HiringDate));

    var Sites = JSON.parse(_EmployeeData.SiteName);

    var siteIds = Sites.map(function (item) {
        return item.id;
    });

    try {

        var Departments = JSON.parse(_EmployeeData.DepartmentName);

        var departmentIds = Departments.map(function (item) {
            return item.id;
        });
        if (departmentIds.length > 0) {
            $("#department").val(departmentIds).trigger("change");
        }
    }
    catch {

    }

    $("#site").val(siteIds).trigger("change");
    $("#employment-status").val(_EmployeeData.EmploymentStatusId).trigger("change");
    $("#employment-level").val(_EmployeeData.EmploymentLevelId).trigger("change");
    $("#manager").val(_EmployeeData.ManagerId).trigger("change");
    $("#role").val(_EmployeeData.UserRoles).trigger("change");


    try {
        var DepartmentsSubCategory = JSON.parse(_EmployeeData.DepartmentSubCategoryName);

        var departmentSubCategoryIds = DepartmentsSubCategory.map(function (item) {
            return item.id;
        });

        if (departmentSubCategoryIds.length > 0) {
            $("#department-sub-category").val(departmentSubCategoryIds).trigger("change");
        }

    }
    catch {

    }
    //$("#department-sub-category").val(_EmployeeData.DepartmentSubCategoryId).trigger("change");
    if ((_EmployeeData.RoleName?.toLowerCase())?.indexOf("acupuncturist") > -1 || (_EmployeeData.RoleName?.toLowerCase())?.indexOf("acupuncture") > -1 || (_EmployeeData.PositionName?.toLowerCase())?.indexOf("acupuncture") > -1 || (_EmployeeData.PositionName?.toLowerCase())?.indexOf("acupuncturist") > -1) {
        $("#acupuncturist-fields").removeClass('d-none')
    }
    else {
        $("#acupuncturist-fields").addClass('d-none')

    }
    if ((_EmployeeData.RoleName?.toLowerCase())?.indexOf("physiotherapist") > -1 || (_EmployeeData.RoleName?.toLowerCase())?.indexOf("physiotherapy") > -1 || (_EmployeeData.PositionName?.toLowerCase())?.indexOf("physiotherapy") > -1 || (_EmployeeData.PositionName?.toLowerCase())?.indexOf("physiotherapist") > -1) {
        $("#physiotherapist-fields").removeClass('d-none')
    }
    else {
        $("#physiotherapist-fields").addClass('d-none')

    }

    if (_EmployeeData.ProbationDateStart != null && _EmployeeData.ProbationDateStart != '') {
        $("#probation-from").flatpickr({
            dateFormat: "d/m/Y",
        }).setDate(FormatDate(_EmployeeData.ProbationDateStart));
    }
    if (_EmployeeData.ProbationDateEnd != null && _EmployeeData.ProbationDateEnd != '') {
        $("#probation-to").flatpickr({
            dateFormat: "d/m/Y",
        }).setDate(FormatDate(_EmployeeData.ProbationDateEnd));
    }

    if (_EmployeeData.AcceptanceDate != null && _EmployeeData.AcceptanceDate != '') {
        $("#acceptance-date").flatpickr({
            dateFormat: "d/m/Y",
        }).setDate(FormatDate(_EmployeeData.AcceptanceDate));
    }
    if (_EmployeeData.RegistrationDate != null && _EmployeeData.RegistrationDate != '') {
        $("#registration-date").flatpickr({
            dateFormat: "d/m/Y",
        }).setDate(FormatDate(_EmployeeData.RegistrationDate));
    }

    $("#registration-number").val(_EmployeeData.RegistrationNumber);
    if (_EmployeeData.LiabilityInsuranceName != null && _EmployeeData.LiabilityInsuranceName != "") {
        $(document).find("#liability-insurance-name").text(_EmployeeData.LiabilityInsuranceName);
        $(document).find("#liability-insurance-name").data('name', _EmployeeData.LiabilityInsuranceName);
        $(document).find("#liability-insurance-name").data('href', _EmployeeData.LiabilityInsuranceUrl);
        $(document).find(".href-download-liability-insurance").attr('href', _EmployeeData.LiabilityInsuranceUrl);
        $(document).find(".href-download-liability-insurance").attr('download', _EmployeeData.LiabilityInsuranceName);
        $(document).find(".href-download-liability-insurance").toggleClass('d-none');
    }

    if (_EmployeeData.RegistrationNumberName != null && _EmployeeData.RegistrationNumberName != "") {
        $(document).find("#registration-number-name").text(_EmployeeData.RegistrationNumberName);
        $(document).find("#registration-number-name").data('name', _EmployeeData.RegistrationNumberName);
        $(document).find("#registration-number-name").data('href', _EmployeeData.RegistrationNumberUrl);
        $(document).find(".href-download-registration-number").attr('href', _EmployeeData.RegistrationNumberUrl);
        $(document).find(".href-download-registration-number").attr('download', _EmployeeData.RegistrationNumberName);
        $(document).find(".href-download-registration-number").toggleClass('d-none');
    }

    $("#business-name").val(_EmployeeData.BusinessName);
    $("#business-email").val(_EmployeeData.BusinessEmail);
    $("#business-number").val(_EmployeeData.BusinessNumber);

    if (_EmployeeData.BusinessChequeName != null && _EmployeeData.BusinessChequeName != "") {
        $(document).find("#business-cheque-name").text(_EmployeeData.BusinessChequeName);
        $(document).find("#business-cheque-name").data('name', _EmployeeData.BusinessChequeName);
        $(document).find("#business-cheque-name").data('href', _EmployeeData.BusinessChequeUrl);
        $(document).find(".href-download-business-cheque").attr('href', _EmployeeData.BusinessChequeUrl);
        $(document).find(".href-download-business-cheque").attr('download', _EmployeeData.BusinessChequeName);
        $(document).find(".href-download-business-cheque").toggleClass('d-none');
    }


    $("#contractor-business-name").val(_EmployeeData.ContractorBusinessName);
    $("#contractor-business-email").val(_EmployeeData.ContractorBusinessEmail);
    $("#contractor-business-number").val(_EmployeeData.ContractorBusinessNumber);

    if (_EmployeeData.ContractorBusinessChequeName != null && _EmployeeData.ContractorBusinessChequeName != "") {
        $(document).find("#contractor-business-cheque-name").text(_EmployeeData.ContractorBusinessChequeName);
        $(document).find("#contractor-business-cheque-name").data('name', _EmployeeData.ContractorBusinessChequeName);
        $(document).find("#contractor-business-cheque-name").data('href', _EmployeeData.ContractorBusinessChequeUrl);
        $(document).find(".href-download-contractor-business-cheque").attr('href', _EmployeeData.ContractorBusinessChequeUrl);
        $(document).find(".href-download-contractor-business-cheque").attr('download', _EmployeeData.ContractorBusinessChequeName);
        $(document).find(".href-download-contractor-business-cheque").toggleClass('d-none');
    }

    $('#modal-employment-details').modal('show');

});


$(document).on('change', '#position', function () {
    var position = $("#position :Selected").text();
    var role = $("#role :Selected").text();

    if ((position?.toLowerCase())?.indexOf("acupuncture") > -1 || (role?.toLowerCase())?.indexOf("acupuncturist") > -1) {
        $("#acupuncturist-fields").removeClass('d-none')
    } else {
        $("#acupuncturist-fields").addClass('d-none')

    }

    if ((position?.toLowerCase())?.indexOf("physiotherapy") > -1 || (role?.toLowerCase())?.indexOf("physiotherapist") > -1) {
        $("#physiotherapist-fields").removeClass('d-none')
    }
    else {
        $("#physiotherapist-fields").addClass('d-none')

    }
})

$(document).on('change', '#employment-status', function () {
    var position = $("#employment-status :Selected").text();
    var role = $("#role :Selected").text();


    if ((position?.toLowerCase())?.indexOf("contractor") > -1 || (role?.toLowerCase())?.indexOf("contractor") > -1) {
        $("#contractor-fields").removeClass('d-none')
    }
    else {
        $("#contractor-fields").addClass('d-none')
    }

})


// These are the constraints used to validate the form
var employmentdetailsconstraints = {
    "position": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "hiring-date": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "site": {
        // Site Time Zone is required
        presence: { message: "is required.", allowEmpty: false }
    },
    "department": {
        // Site Time Zone is required
        presence: { message: "is required.", allowEmpty: false }
    },
    "employment-status": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "employment-level": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    //"manager": {
    //    // Site Time Zone is required
    //    presence: { message: "is required." },
    //},
    "probation-from": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "probation-to": {
        // Site Time Zone is required
        presence: { message: "is required." }
    }

};



$("#btn-update-employment-details").on("click", function () {
    var form = document.querySelector("#employment-details-form");
    var errors = validate(form, employmentdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var employee = {};

        const selectedSites = [];
        const dropdown = document.getElementById("site");

        for (let option of dropdown.selectedOptions) {
            selectedSites.push({
                id: parseInt(option.value),
                name: option.text
            });
        }



        const selectedDepartments = [];
        const departmentdropdown = document.getElementById("department");

        for (let option of departmentdropdown.selectedOptions) {
            selectedDepartments.push({
                id: parseInt(option.value),
                name: option.text
            });
        }
        employee.Id = _EmployeeData.Id;
        employee.positionId = ($("#position").val() == "" || $("#position").val() == null) ? null : $("#position").val();
        employee.positionName = $("#position :Selected").text();
        employee.hiringDate = $("#hiring-date").val();
        //employee.siteId = ($("#site").val() == "" || $("#site").val() == null) ? null : $("#site").val();
        employee.siteName = selectedSites.length > 0 ? JSON.stringify(selectedSites) : null;
        employee.departmentName = selectedDepartments.length > 0 ? JSON.stringify(selectedDepartments) : null;

        //employee.departmentId = ($("#department").val() == "" || $("#department").val() == null) ? null : $("#department").val();
        //employee.departmentName = $("#department :Selected").text();

        const selectedDepartmentsSubCategory = [];
        const departmentSubCatdropdown = document.getElementById("department-sub-category");

        for (let option of departmentSubCatdropdown.selectedOptions) {
            selectedDepartmentsSubCategory.push({
                id: parseInt(option.value),
                name: option.text
            });
        }

        employee.departmentSubCategoryName = selectedDepartmentsSubCategory.length > 0 ? JSON.stringify(selectedDepartmentsSubCategory) : null;


        //employee.departmentSubCategoryId = ($("#department-sub-category").val() == "" || $("#department-sub-category").val() == null) ? null : $("#department-sub-category").val();
        //employee.departmentSubCategoryName = $("#department-sub-category :Selected").text();
        employee.employmentStatusId = ($("#employment-status").val() == "" || $("#employment-status").val() == null) ? null : $("#employment-status").val();
        employee.employmentStatus = $("#employment-status :Selected").text();
        employee.employmentLevelId = ($("#employment-level").val() == "" || $("#employment-level").val() == null) ? null : $("#employment-level").val();
        employee.employmentLevel = $("#employment-level :Selected").text();
        employee.managerId = ($("#manager").val() == "" || $("#manager").val() == null) ? null : $("#manager").val();
        employee.managerName = $("#manager :Selected").text();
        employee.probationDateStart = $("#probation-from").val();
        employee.probationDateEnd = $("#probation-to").val();
        employee.acceptanceDate = ($("#acceptance-date").val() == "" || $("#acceptance-date").val() == null) ? null : $("#acceptance-date").val();
        employee.registrationDate = ($("#registration-date").val() == "" || $("#registration-date").val() == null) ? null : $("#registration-date").val();
        employee.registrationNumber = $("#registration-number").val();
        employee.liabilityInsuranceName = $("#liability-insurance-name").data('name');
        employee.liabilityInsuranceUrl = $("#liability-insurance-name").data('href');
        employee.registrationNumberName = $("#registration-number-name").data('name');
        employee.registrationNumberUrl = $("#registration-number-name").data('href');
        employee.businessName = $("#business-name").val();
        employee.businessEmail = $("#business-email").val();
        employee.businessNumber = $("#business-number").val();
        employee.businessChequeName = $("#business-cheque-name").data('name');
        employee.businessChequeUrl = $("#business-cheque-name").data('href');
        employee.contractorBusinessName = $("#contractor-business-name").val();
        employee.contractorBusinessEmail = $("#contractor-business-email").val();
        employee.contractorBusinessNumber = $("#contractor-business-number").val();
        employee.contractorBusinessChequeName = $("#contractor-business-cheque-name").data('name');
        employee.contractorBusinessChequeUrl = $("#contractor-business-cheque-name").data('href');

        var rolename = $("#role :Selected").text();
        if (employee.employmentStatus.toLowerCase().indexOf('contractor') > -1 || rolename.toLowerCase().indexOf('contractor') > -1) {
            if (employee.contractorBusinessName == ''
                || employee.contractorBusinessName == null
                || employee.contractorBusinessEmail == ''
                || employee.contractorBusinessEmail == null
                || employee.contractorBusinessNumber == ''
                || employee.contractorBusinessNumber == null
                || employee.contractorBusinessChequeUrl == ''
                || employee.contractorBusinessChequeUrl == null
            ) {
                Swal.fire({
                    text: "Error occured. Please provide contractor details.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();
                    }
                });

                return;
            }
        }

        if (Window.JustConsole) { console.log(employee); return; }
        var url = sitePath + 'api/EmployeeAPI/UpdateEmploymentDetails';

        Ajax.post(url, employee, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Employee details updated successfully.",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();

                    }
                }).then(function (result) {
                    if (result.isConfirmed) {

                        window.location.reload();
                    }
                });
            }
            else {
                Swal.fire({
                    text: "Error occured. Please contact your system administrator.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    didOpen: function () {
                        hideSpinner();
                    }
                });
            }
        });
    }
    else {
        Swal.fire({
            text: "Please complete mandatory fields.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok",
            allowEscapeKey: false,
            allowOutsideClick: false,
            customClass: {
                confirmButton: "btn btn-primary",
            },
            didOpen: function () {
                hideSpinner();
            }
        });
    }
});

