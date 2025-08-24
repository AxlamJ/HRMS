let currentPage = 1;
const pageSize = 12; // Employees per page


HRMSUtil.onDOMContentLoaded(function () {

    PopulateDropDowns(function () {

        RenderListData(currentPage);
    });
    $('#st-accordion').accordion();
    $("#btn-import").on('click', function () { $("#import-file").trigger('click') });
    $("#termination-date").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date(),
    });
    $("#dob").flatpickr({
        dateFormat: "d/m/Y",
    });
    $("#on-boarding-date").flatpickr({
        dateFormat: "d/m/Y",
    });
    $("#hiring-date").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date(),
    });
    hideSpinner();
});


function PopulateDropDowns(cb) {


    var ddDepartments = document.querySelector('#filter-department');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });

    var ddSites = document.querySelector('#filter-site');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });

    var ddUserDepartments = document.querySelector('#user-department');
    $(ddUserDepartments).empty();
    var lstUserDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddUserDepartments.appendChild(option);
    lstUserDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddUserDepartments.appendChild(option);
    });



    var ddUserSites = document.querySelector('#user-site');
    $(ddUserSites).empty();
    var lstUserSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddUserSites.appendChild(option);
    lstUserSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddUserSites.appendChild(option);
    });


    var ddUserManagers = document.querySelector('#user-manager');
    $(ddUserManagers).empty();
    var lstUserManagers = (DropDownsData.Managers || []);
    var option = new Option();
    ddUserManagers.appendChild(option);
    lstUserManagers.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.Id, false, false);
        ddUserManagers.appendChild(option);
    });


    var ddUserRoles = document.querySelector('#user-role');
    $(ddUserRoles).empty();
    var lstUserRoles = (DropDownsData.Roles || []);
    var option = new Option();
    ddUserRoles.appendChild(option);
    lstUserRoles.forEach(function (item) {

        var option = new Option(item.RoleName, item.RoleId, false, false);
        ddUserRoles.appendChild(option);
    });

    var ddPosition = document.querySelector('#position');
    $(ddPosition).empty();
    var lstPositions = (DropDownsData.EmployeePosition || []);
    var option = new Option();
    ddPosition.appendChild(option);
    lstPositions.forEach(function (item) {

        var option = new Option(item.PositionName, item.Id, false, false);
        ddPosition.appendChild(option);
    });


    var ddEmploymentStatus = document.querySelector('#employment-status-type');
    $(ddEmploymentStatus).empty();
    var lstddEmploymentStatus = (DropDownsData.EmployeeStatus || []);
    var option = new Option();
    ddEmploymentStatus.appendChild(option);
    lstddEmploymentStatus.forEach(function (item) {
        var option = new Option(item.EmployeeStatusName, item.Id, false, false);
        ddEmploymentStatus.appendChild(option);
    });


    var ddEmployeeStatus = document.querySelector('#employment-status');
    $(ddEmployeeStatus).empty();
    var lstddEmployeeStatus = (DropDownsData.EmployeeStatus || []);
    var option = new Option();
    ddEmployeeStatus.appendChild(option);
    lstddEmployeeStatus.forEach(function (item) {
        var option = new Option(item.EmployeeStatusName, item.Id, false, false);
        ddEmployeeStatus.appendChild(option);
    });

    var ddReason = document.querySelector('#reason');
    $(ddReason).empty();
    var lstReasons = (DropDownsData.TerminationDismissalReason || []);
    var option = new Option();
    ddReason.appendChild(option);
    lstReasons.forEach(function (item) {

        var option = new Option(item.ReasonName, item.Id, false, false);
        ddReason.appendChild(option);
    });

    if (cb) {
        cb();
    }

}

$(document).on('change', '#user-department', function () {
    var selectedDepartments = $("#user-department").val(); // This is an array if multiple selected

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

$(document).on('input', '#filter-firstname', delay(function () {
    let text = $("#filter-firstname").val();
    if (text && text != "") {
        RenderListData(currentPage)
    }
}, 500));

$(document).on('input', '#filter-lastname', delay(function () {
    let text = $("#filter-lastname").val();
    if (text && text != "") {
        RenderListData(currentPage)
    }
}, 500));

function delay(callback, ms) {
    var timer = 0;



    return function () {
        clearTimeout(timer);


        timer = setTimeout(function () {
            callback();
        }, ms);
    }
}
function RenderListData(pageNumber) {

    var queryData = getEmployeesFilters()

    queryData.PageNumber = pageNumber - 1;
    queryData.PageSize = pageSize;
    var url = sitePath + 'api/EmployeeAPI/GetEmployeesList';

    Ajax.post(url, queryData, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            $(".active-employees").html(response.ActiveEmployees);
            $(".all-employees").html(response.AllEmployees);
            $(".active-contractor").html(response.ActiveContractorCount);
            $(".all-contractor").html(response.AllContractorCount);
            renderEmployees(response.Employees, function () {
                renderPagination(response.TotalRecords, pageNumber);
            })
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


function renderEmployees(employees, cb) {
    const container = $("#employeelist");
    container.empty();

    employees.forEach(function (emp) {
        var Status = emp.Status == 1 ? "Active" : emp.Status == 2 ? "Pending" : "Terminated";
        var ShowHideTerminatebtn = emp.Status == 1 ? "show" : emp.Status == 2 ? "show" : "d-none";
        var StatusClass = emp.Status == 1 ? "badge-success" : emp.Status == 2 ? "badge-primary" : "badge-danger";

        try {
            var Departments = JSON.parse(emp.DepartmentName);

            var DepartmentNames = Departments.map(function (item) {
                return item.name;
            }).join(", ");

        }
        catch {

        }

        const card = `
                    <div class="col-md-6 col-lg-4 col-sm-12 col-xs-12 mb-4">
                    	<div class="card card-flush mb-10">
							<div class="card-body">
                            	<div class="d-flex align-items-center d-lg-grid">
									<div class="symbol symbol-50px me-5">
										<img src="${emp.ProfilePhotoUrl}" class="" alt="" />
									</div>
									<div class="flex-grow-1">
										<h5 class="card-title">${emp.FirstName} ${emp.LastName}</h5>
										<span class="badge ${StatusClass}">${Status}</span>
									</div>
								</div>
								<div class="fs-6 fw-normal text-gray-700 mb-5 mt-5">
									<span><strong>Employee Code:</strong> ${emp.EmployeeCode || 'N/A'}</span>
                                    <br>
									<span><strong>Position:</strong> ${emp.PositionName || 'N/A'}</span>
									<br>
									<span><strong>Department:</strong> ${DepartmentNames || 'N/A'}</span>
                                    <br>
									<span><strong>Employee Status:</strong> ${emp.EmploymentStatus || 'N/A'}</span>
								</div>
								<div class="fs-6 fw-normal text-black mb-5 mt-5">
									<span><i class="fa fa-mobile-alt fs-4 me-3"></i> ${emp.PhoneNumber || 'N/A'}</span>
									<br>
									<span><i class="fa fa-envelope fs-4 me-2"></i> ${emp.Email}</span>
									<br>
									<span><i class="fa fa-cake fs-4 me-3"></i> ${FormatDate(emp.DOB)}</span>
								</div>
							</div>
							<div class="card-footer pt-0">
								<div class="mb-6">
                                    <div class="row">
                                        <div class="d-flex justify-content-center text-end gap-2 d-lg-grid">
                                            <a  href="${sitePath}Employee/EmployeeDetails?EmployeeId=${emp.Id}&EmployeeCode=${emp.EmployeeCode}" class="btn btn-primary btn-lg" id="btnedit">
                                                <i class="fa fa-pencil fs-4 me-2"></i>
                                                Edit
                                            </a>
                                            <span class="btn btn-danger ${ShowHideTerminatebtn}" id="btnterminate" data-id="${emp.Id}">
                                                <i class="fa fa-user-xmark  fs-4 me-2"></i>
                                                Terminate
                                            </span>
                                        </div>
                                    </div>
                                </div>
							</div>
						</div>
                    </div>`;
        container.append(card);
    });

    if (cb) {
        cb()
    }
}

function renderPagination(totalRecords, currentPage) {
    const totalPages = Math.ceil(totalRecords / pageSize);
    const pagination = $("#pagination");
    pagination.empty();

    const maxVisiblePages = 4;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = startPage + maxVisiblePages - 1;

    if (endPage > totalPages) {
        endPage = totalPages;
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // Previous Button
    pagination.append(`
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${currentPage - 1})"><i class="fa fa-chevron-left fs-4"></i></a>
            </li>
        `);

    // Page Numbers
    for (let i = startPage; i <= endPage; i++) {
        pagination.append(`
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${i})">${i}</a>
                </li>
            `);
    }

    // Next Button
    pagination.append(`
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link btn btn-outline btn-outline-dashed border-black" href="#" onclick="RenderListData(${currentPage + 1})"><i class="fa fa-chevron-right fs-4"></i></a>
            </li>
        `);
}

function getEmployeesFilters() {
    var filter = {};
    filter.firstName = $("#filter-firstname").val();
    filter.lastName = $("#filter-lastname").val();
    filter.departmentId = parseInt($("#filter-department").val());
    filter.siteId = $("#filter-site").val().length > 0 ? $("#filter-site").val() : null;
    filter.positionId = parseInt($("#position").val());
    filter.employmentStatusId = parseInt($("#employment-status").val());
    filter.formerEmployee = parseInt($("#former-employees").val());

    if (filter.siteId == null || filter.siteId.length == 0) {
        var select = document.getElementById("filter-site");
        filter.siteId = [];
        for (var i = 0; i < select.options.length; i++) {
            if (select.options[i].value != null && select.options[i].value != '') {
                filter.siteId.push(parseInt(select.options[i].value));

            }
        }

    }

    return filter;
}

$(document).on("click", "#btnterminate", function () {

    var $self = $(this);
    var employeeId = $self.data("id");

    $("#modal-terminate-details").modal('show');

    $("#modal-terminate-details").find('#btn-submit-terminate-details').data('employee-id', employeeId);
});





// These are the constraints used to validate the form
var terminateconstraints = {
    "termination-date": {
        presence: {
            message: "is required."
        }
    },
    "reason": {
        presence: {
            message: "is required."
        }
    },
    "comment": {
        presence: {
            message: "is required."
        }
    }
};


$(document).on("click", "#btn-submit-terminate-details", function () {

    var $self = $(this);

    var form = document.querySelector("#terminate-details-form");
    var errors = validate(form, terminateconstraints);
    showErrors(form, errors || {});
    if (!errors) {

        var employeeId = $self.data("employeeId");
        var TerminationDismissalDate = $("#termination-date").val();
        var TerminationDismissalReasonId = $("#reason").val();
        var TerminationDismissalReason = $("#reason :Selected").text();
        var TerminationDismissalComment = $("#comment").val();
        var TerminationDismissalType = $('input[name="dismissaltype"]:checked').val();

        var url = sitePath + "api/EmployeeAPI/DeleteEmployeeById?Id=" + employeeId
            + "&TerminationDismissalDate=" + TerminationDismissalDate
            + "&TerminationDismissalReasonId=" + TerminationDismissalReasonId
            + "&TerminationDismissalReason=" + TerminationDismissalReason
            + "&TerminationDismissalComment=" + TerminationDismissalComment
            + "&TerminationDismissalType=" + TerminationDismissalType
        Ajax.post(url, null, function (resp) {
            if (resp.StatusCode == 200) {

                Swal.fire({
                    text: "Employee terminated successfully.",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        window.location.href = "/Employee/ManageEmployees";
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
            }
        });
    }
});


$('#btnSearch').click(function () {
    filterEmployees();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterEmployees() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderListData(currentPage);
    hideSpinner();
}

function cancelSearch() {
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    //clearSearchState();
    window.location.reload()// = nURL;
}
function hideOverlay() {

    var element = document.querySelector("#tbl_manageemployees")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_manageemployees")
    var blockUI = new KTBlockUI(element, {
        animate: true,
        overlayClass: "bg-body",
    });
    try {
        blockUI.block();
    }
    catch (e) {
        alert(e.toString());
    }
}

function unblockTable() {

    var element = document.querySelector("#tbl_manageemployees")
    var blockUI = KTBlockUI.getInstance(element);
    try {
        if (blockUI.blocked) {
            setTimeout(() => {
                blockUI.release();
                blockUI.destroy();
            }, 1000);
        }
    }
    catch (e) {
        alert(e.toString());
    }
}


function ExportActiveEmployees() {

    var queryData = getEmployeesFilters()

    var url = sitePath + 'api/EmployeeAPI/ExportActiveEmployees';

    Ajax.post(url, queryData, function (resp) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (resp.StatusCode == 200) {
            hideSpinner();
            window.open(resp.DownloadUrl, '_blank');

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
function ExportFormerEmployees() {

    var queryData = getEmployeesFilters()

    var url = sitePath + 'api/EmployeeAPI/ExportFormerEmployees';

    Ajax.post(url, queryData, function (resp) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (resp.StatusCode == 200) {
            hideSpinner();
            window.open(resp.DownloadUrl, '_blank');

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


$(document).on("change", "#import-file", function () {
    let fileInput = document.getElementById("import-file");
    let file = fileInput.files[0];

    //var $self = $(this);
    //var file = $self.files[0];

    if (!file) {
        Swal.fire({
            text: "Please select excel file.",
            icon: "warning",
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

    let formData = new FormData();
    formData.append("file", file);

    var url = sitePath + 'api/EmployeeAPI/ImportEmployees';

    const token = localStorage.getItem('jwtToken');

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        headers: {
            'Authorization': `Bearer ${token}`
        },
        contentType: false,  // Must be false for FormData
        processData: false,  // Prevent jQuery from processing the data
        success: function (response) {
            if (response.StatusCode == 200) {

                var message = "Employees Imported successfully";
                if (response.ExistingEmployees != null && response.ExistingEmployees.length > 0) {
                    message += " Following employees already exists in the system. ";

                    for (var i = 0; i < ExistingEmployees.length; i++) {
                        message += "</br>  Name: " + response.ExistingEmployees[i].FirstName + response.ExistingEmployees[i].LastName + "   Email: " + response.ExistingEmployees[i].Email + " </br></br>";
                    }
                }
                Swal.fire({
                    text: message,
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
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }
    });

    //Ajax.post(url, formData, function (resp) { // Ensure you use the correct data (exportRequestBody)
    //    //hideSpinner();

    //    if (resp.StatusCode == 200) {

    //        console.log(resp.ExcelData)

    //    }
    //    else {
    //        Swal.fire({
    //            text: "Error occured. Please contact your system administrator.",
    //            icon: "error",
    //            buttonsStyling: false,
    //            confirmButtonText: "Ok",
    //            allowEscapeKey: false,
    //            allowOutsideClick: false,
    //            customClass: {
    //                confirmButton: "btn btn-primary",
    //            },
    //            didOpen: function () {
    //                hideSpinner();
    //            }
    //        });
    //    }
    //});
});

function AddUserInSystem() {
    // Get the checkbox
    var checkBox = document.getElementById("add-user-in-system");
    // Get the output text
    var rolediv = document.getElementById("role-div");

    // If the checkbox is checked, display the output text
    if (checkBox.checked == true) {
        checkBox.value = 1;
        rolediv.classList.remove('d-none')
    } else {
        checkBox.value = null;
        rolediv.classList.add('d-none')
    }
}

$(document).on("click", "#btn-createuser", function () {

    $("#add-new-employee-modal").modal('show');

});

document.querySelector('.chevron-toggle').addEventListener('click', function () {
    document.querySelector('.st-arrow').classList.toggle('active');
});

$("#add-new-employee-modal").on('hide.bs.modal', function () {
    var form = document.querySelector("#new-employee-form");
    form.reset();
});
// These are the constraints used to validate the form
var addnewemployeeconstraints = {
    //"employee-code": {
    //    presence: {
    //        message: "is required."
    //    }
    //},
    "first-name": {
        presence: {
            message: "is required."
        }
    },
    "last-name": {
        presence: {
            message: "is required."
        }
    },
    "dob": {
        presence: {
            message: "is required."
        }
    },
    "on-boarding-date": {
        presence: {
            message: "is required."
        }
    },
    "phone": {
        presence: {
            message: "is required."
        }
    },
    "email": {
        presence: {
            message: "is required."
        }
    },
    "gender": {
        presence: {
            message: "is required."
        }
    },
    "marital-status": {
        presence: {
            message: "is required."
        }
    },
    "hiring-date": {
        presence: {
            message: "is required."
        }
    },
    "add-user-in-system": {
        presence: {
            allowEmpty: false,
            message: "is required."
        }
    },
    "user-site": {
        presence: {
            allowEmpty: false,
            message: "is required."
        }
    },
    "employment-status-type": {
        presence: {
            allowEmpty: false,
            message: "is required."
        }
    },
    "user-department": {
        presence: {
            allowEmpty: false,
            message: "is required."
        }
    },
    "user-role": {
        presence: {
            message: "is required."
        }
    }
};



$(document).on("click", "#btn-add-new-employee", function () {

    var $self = $(this);

    var form = document.querySelector("#new-employee-form");
    var errors = validate(form, addnewemployeeconstraints);
    showErrors(form, errors || {});
    if (!errors) {

        var checkBox = document.getElementById("add-user-in-system");

        if (checkBox.checked == true) {
            var roleid = $("#user-role").val() || null;

            if (roleid == null || roleid == undefined || roleid == '') {
                Swal.fire({
                    text: "Please select role for user addition in system.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    }
                });
                return;
            }

        }

        var employee = {};



        employee.EmployeeCode = null;
        employee.FirstName = $(form).find("#first-name").val()
        employee.LastName = $(form).find("#last-name").val()
        employee.HiringDate = $(form).find("#hiring-date").val()
        employee.DOB = $(form).find("#dob").val();
        employee.onBoardingDate = $(form).find("#on-boarding-date").val();
        employee.phoneNumber = $(form).find("#phone").val();
        employee.Gender = $(form).find("#gender").val()
        employee.Email = $(form).find("#email").val()
        employee.DepartmentId = ($(form).find("#user-department").val() > 0 && $(form).find("#user-department").val() != '') ? parseInt($(form).find("#user-department").val()) : null;
        //employee.DepartmentName = ($(form).find("#user-department").val() > 0 && $(form).find("#user-department").val() != '') ? $(form).find("#user-department :Selected").text() : '';
        //employee.SiteId = $(form).find("#user-site").val().length > 0 ? $(form).find("#user-site").val() : null;

        const selectedSites = [];
        const dropdown = document.getElementById("user-site");

        for (let option of dropdown.selectedOptions) {
            selectedSites.push({
                id: parseInt(option.value),
                name: option.text
            });
        }

        const selectedDepartments = [];
        const departmentdropdown = document.getElementById("user-department");

        for (let option of departmentdropdown.selectedOptions) {
            selectedDepartments.push({
                id: parseInt(option.value),
                name: option.text
            });
        }


        const selectedDepartmentsSubCategory = [];
        const departmentSubCatdropdown = document.getElementById("department-sub-category");

        for (let option of departmentSubCatdropdown.selectedOptions) {
            selectedDepartmentsSubCategory.push({
                id: parseInt(option.value),
                name: option.text
            });
        }

        employee.departmentSubCategoryName = selectedDepartmentsSubCategory.length > 0 ? JSON.stringify(selectedDepartmentsSubCategory) : null;


        employee.SiteName = selectedSites.length > 0 ? JSON.stringify(selectedSites) : null;
        employee.DepartmentName = selectedDepartments.length > 0 ? JSON.stringify(selectedDepartments) : null;
        employee.MaritalStatus = $(form).find("#marital-status").val();
        employee.ManagerId = ($(form).find("#user-manager").val() > 0 && $(form).find("#user-manager").val() != '') ? parseInt($(form).find("#user-manager").val()) : null;
        employee.ManagerName = ($(form).find("#user-manager").val() > 0 && $(form).find("#user-manager").val() != '') ? $(form).find("#user-manager :Selected").text() : '';
        employee.employmentStatusId = ($("#employment-status-type").val() == "" || $("#employment-status-type").val() == null) ? null : $("#employment-status-type").val();
        employee.employmentStatus = $("#employment-status-type :Selected").text();

        employee.Status = 1

        //For User Creation

        var message = "Employee Added successfully.";
        if (checkBox.checked == true) {
            employee.CreateUser = true;
            employee.RoleId = ($(form).find("#user-role").val() > 0 && $(form).find("#user-role").val() != '') ? parseInt($(form).find("#user-role").val()) : null;
            employee.UserRoles = $(form).find("#user-role").val();
            employee.RoleName = $(form).find("#user-role :Selected").text();
            message = "Employee Added successfully and login creadentials are emailed."
        }
        else {
            employee.Status = 2
        }

        var url = sitePath + "api/EmployeeAPI/AddNewEmployee";
        Ajax.post(url, employee, function (resp) {
            if (resp.StatusCode == 200) {

                Swal.fire({
                    text: message,
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowEscapeKey: false,
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary",
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        window.location.href = "/Employee/ManageEmployees";
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
            }
        });
    }
});

