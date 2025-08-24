


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

    var $self = $(this);
    var EmployeeCode = $self.data("employeecode");

    var url = sitePath + "api/EmployeeAPI/GetEmployeeByEmpCode?EmployeeCode=" + parseInt(EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            EmployeeData = resp.Employee;

            $("#position").val(EmployeeData.PositionId).trigger("change");
            $("#hiring-date").flatpickr({
                dateFormat: "d/m/Y",
            }).setDate(FormatDate(EmployeeData.HiringDate));


            try {
                var Sites = JSON.parse(EmployeeData.SiteName);

                var siteIds = Sites.map(function (item) {
                    return item.id;
                });
                var Departments = JSON.parse(EmployeeData.DepartmentName);

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
            $("#department").val(departmentIds).trigger("change");
            $("#employment-status").val(EmployeeData.EmploymentStatusId).trigger("change");
            $("#employment-level").val(EmployeeData.EmploymentLevelId).trigger("change");
            $("#manager").val(EmployeeData.ManagerId).trigger("change");
            $("#role").val(EmployeeData.UserRoles).trigger("change");



            try {
                var DepartmentsSubCategory = JSON.parse(EmployeeData.DepartmentSubCategoryName);

                var departmentSubCategoryIds = DepartmentsSubCategory.map(function (item) {
                    return item.id;
                });

                if (departmentSubCategoryIds.length > 0) {
                    $("#department-sub-category").val(departmentSubCategoryIds).trigger("change");
                }

            }
            catch {

            }



            if ((EmployeeData.RoleName?.toLowerCase())?.indexOf("acupuncturist") > -1 || (EmployeeData.RoleName?.toLowerCase())?.indexOf("acupuncture") > -1 || (EmployeeData.PositionName?.toLowerCase())?.indexOf("acupuncture") > -1 || (EmployeeData.PositionName?.toLowerCase())?.indexOf("acupuncturist") > -1) {
                $("#acupuncturist-fields").removeClass('d-none')
            } else {
                $("#acupuncturist-fields").addClass('d-none')
            }

            if ((EmployeeData.RoleName?.toLowerCase())?.indexOf("physiotherapist") > -1 || (EmployeeData.RoleName?.toLowerCase())?.indexOf("physiotherapy") > -1 || (EmployeeData.PositionName?.toLowerCase())?.indexOf("physiotherapy") > -1 || (EmployeeData.PositionName?.toLowerCase())?.indexOf("physiotherapist") > -1) {
                $("#physiotherapist-fields").removeClass('d-none')
            }
            else {
                $("#physiotherapist-fields").addClass('d-none')
            }

            if (EmployeeData.ProbationDateStart != null && EmployeeData.ProbationDateStart != '') {
                $("#probation-from").flatpickr({
                    dateFormat: "d/m/Y",
                }).setDate(FormatDate(EmployeeData.ProbationDateStart));
            }
            if (EmployeeData.ProbationDateEnd != null && EmployeeData.ProbationDateEnd != '') {
                $("#probation-to").flatpickr({
                    dateFormat: "d/m/Y",
                }).setDate(FormatDate(EmployeeData.ProbationDateEnd));
            }

            if (EmployeeData.AcceptanceDate != null && EmployeeData.AcceptanceDate != '') {
                $("#acceptance-date").flatpickr({
                    dateFormat: "d/m/Y",
                }).setDate(FormatDate(EmployeeData.AcceptanceDate));
            }

            if (EmployeeData.RegistrationDate != null && EmployeeData.RegistrationDate != '') {
                $("#registration-date").flatpickr({
                    dateFormat: "d/m/Y",
                }).setDate(FormatDate(EmployeeData.RegistrationDate));
            }

            $("#registration-number").val(EmployeeData.RegistrationNumber);
            if (EmployeeData.LiabilityInsuranceName != null && EmployeeData.LiabilityInsuranceName != "") {
                $(document).find("#liability-insurance-name").text(EmployeeData.LiabilityInsuranceName);
                $(document).find("#liability-insurance-name").data('name', EmployeeData.LiabilityInsuranceName);
                $(document).find("#liability-insurance-name").data('href', EmployeeData.LiabilityInsuranceUrl);
                $(document).find(".href-download-liability-insurance").attr('href', EmployeeData.LiabilityInsuranceUrl);
                $(document).find(".href-download-liability-insurance").attr('download', EmployeeData.LiabilityInsuranceName);
                $(document).find(".href-download-liability-insurance").toggleClass('d-none');
            }

            if (EmployeeData.RegistrationNumberName != null && EmployeeData.RegistrationNumberName != "") {
                $(document).find("#registration-number-name").text(EmployeeData.RegistrationNumberName);
                $(document).find("#registration-number-name").data('name', EmployeeData.RegistrationNumberName);
                $(document).find("#registration-number-name").data('href', EmployeeData.RegistrationNumberUrl);
                $(document).find(".href-download-registration-number").attr('href', EmployeeData.RegistrationNumberUrl);
                $(document).find(".href-download-registration-number").attr('download', EmployeeData.RegistrationNumberName);
                $(document).find(".href-download-registration-number").toggleClass('d-none');
            }


            $("#business-name").val(EmployeeData.BusinessName);
            $("#business-email").val(EmployeeData.BusinessEmail);
            $("#business-number").val(EmployeeData.BusinessNumber);

            if (EmployeeData.BusinessChequeName != null && EmployeeData.BusinessChequeName != "") {
                $(document).find("#business-cheque-name").text(EmployeeData.BusinessChequeName);
                $(document).find("#business-cheque-name").data('name', EmployeeData.BusinessChequeName);
                $(document).find("#business-cheque-name").data('href', EmployeeData.BusinessChequeUrl);
                $(document).find(".href-download-business-cheque").attr('href', EmployeeData.BusinessChequeUrl);
                $(document).find(".href-download-business-cheque").attr('download', EmployeeData.BusinessChequeName);
                $(document).find(".href-download-business-cheque").toggleClass('d-none');
            }

            $("#contractor-business-name").val(EmployeeData.ContractorBusinessName);
            $("#contractor-business-email").val(EmployeeData.ContractorBusinessEmail);
            $("#contractor-business-number").val(EmployeeData.ContractorBusinessNumber);

            if (EmployeeData.ContractorBusinessChequeName != null && EmployeeData.ContractorBusinessChequeName != "") {
                $(document).find("#contractor-business-cheque-name").text(EmployeeData.ContractorBusinessChequeName);
                $(document).find("#contractor-business-cheque-name").data('name', EmployeeData.ContractorBusinessChequeName);
                $(document).find("#contractor-business-cheque-name").data('href', EmployeeData.ContractorBusinessChequeUrl);
                $(document).find(".href-download-contractor-business-cheque").attr('href', EmployeeData.ContractorBusinessChequeUrl);
                $(document).find(".href-download-contractor-business-cheque").attr('download', EmployeeData.ContractorBusinessChequeName);
                $(document).find(".href-download-contractor-business-cheque").toggleClass('d-none');
            }

            $('#modal-employment-details').modal('show');

            //$("#user-fullname").text(EmployeeData.FirstName + " " + EmployeeData.LastName);
            //$("#user-dob").text(FormatDate(EmployeeData.DOB) + " (" + calculateAge(FormatDate(EmployeeData.DOB)) + " years old)");
            //$("#user-email").text(EmployeeData.Email);
            //$("#user-position").text(EmployeeData.PositionName);
            //$("#user-employmentstatus").text(EmployeeData.EmploymentStatus);
            //$("#user-department").text(EmployeeData.DepartmentName);
            //$("#user-site").text(EmployeeData.SiteName);
            //$("#user-hire-date").text(FormatDate(EmployeeData.JoiningDate));
            //$("#user-direct-manager").text(EmployeeData.ManagerName);
            //$("#user-level").text(EmployeeData.EmploymentLevel);

            //$("#user-alter-email").text(EmployeeData.AlternativeEmail);
            //$("#user-office-phone").text(EmployeeData.PhoneNumber);
            //$("#user-extension").text('');
            //$("#user-mobile").text(EmployeeData.PhoneNumber);
            //$("#user-country").text('');
            //$("#user-address").text(EmployeeData.Address);
            //$("#user-city").text(EmployeeData.City);
            //$("#user-state").text('');
            //$("#user-zip-code").text(EmployeeData.PostalCode);
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
        presence: {
            message: "is required.", allowEmpty: false
        }
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
        employee.Id = EmployeeData.Id;
        employee.positionId = ($("#position").val() == "" || $("#position").val() == null) ? null : $("#position").val();
        employee.positionName = $("#position :Selected").text();
        employee.hiringDate = $("#hiring-date").val();
        //employee.siteId = ($("#site").val() == "" || $("#site").val() == null) ? null : $("#site").val();
        employee.siteName = JSON.stringify(selectedSites);


        const selectedDepartments = [];
        const departmentdropdown = document.getElementById("department");

        for (let option of departmentdropdown.selectedOptions) {
            selectedDepartments.push({
                id: parseInt(option.value),
                name: option.text
            });
        }

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

                        window.location.href = "/Employee/MyProfile";
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

