let Id = $("#detail_trainingId").val();

HRMSUtil.onDOMContentLoaded(function () {
    hideSpinner();
});

$("#btn-AssignTrainingModal").on("click", function () {


    var myModal = new bootstrap.Modal(document.getElementById('AssignTrainingModal'));
    myModal.show();
    registerEventListners.init();
    PopulateDropDowns(function () {
        if (Id) {
            loadData();
        }
    });


});



var registerEventListners = function () {

    return {
        init: function () {

            $(document).on('change', '#visible-to', function () {
                var $self = $(this);
                var selectedOption = $(this).val();

                if (selectedOption == "departments") {
                    $("#departments-dd").removeClass('d-none');
                    $("#department-sub-category-dd").removeClass('d-none');
                    //$("#department-sub-category-dd").select2();
                    $("#employees-dd").addClass('d-none');
                    $("#sites-dd").addClass('d-none');

                    constraints.departments = { presence: { message: "is required.", allowEmpty: false } };
                    constraints["department-sub-category"] = { presence: { message: "is required.", allowEmpty: false } };
                }
                else if (selectedOption == "sites") {
                    $("#sites-dd").removeClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#department-sub-category-dd").addClass('d-none');
                    $("#employees-dd").addClass('d-none');

                    constraints.sites = { presence: { message: "is required.", allowEmpty: false } };
                }
                else if (selectedOption == "employees") {
                    $("#employees-dd").removeClass('d-none');
                    $("#sites-dd").addClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#department-sub-category-dd").addClass('d-none');

                    constraints.employees = { presence: { message: "is required.", allowEmpty: false } };
                }
                else {

                    delete constraints.departments;
                    delete constraints.sites;
                    delete constraints.employees;

                    $("#sites-dd").addClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#employees-dd").addClass('d-none');
                }
            })
        }
    };

}();


function loadData() {
    var url = sitePath + "api/TrainingAssignAPI/GetTrainingAssignment?Id=" + Id;
    Ajax.post(url, null, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            data = resp.Data;
            $("#visible-to").val(data.VisibleTo).trigger('change');
            if (data.VisibleTo == "departments") {
                var depts = JSON.parse(data.Departments);
                $("#departments").val(depts).trigger('change');

                if (data.DepartmentsSubCategories != '' && data.DepartmentsSubCategories != null) {
                    var DepartmentsSubCategories = JSON.parse(data.DepartmentsSubCategories);
                    $("#department-sub-category").val(DepartmentsSubCategories).trigger('change');

                }
            }
            else if (data.VisibleTo == "sites") {
                var sites = JSON.parse(data.Sites);

                $("#sites").val(sites).trigger('change');

            }
            else if (data.VisibleTo == "employees") {
                var employees = JSON.parse(data.Employees);

                console.log(employees);

                $("#employees").val(employees).trigger('change');

            }
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

////////////// Populate Dropdown

function PopulateDropDowns(cb) {


    var ddDepartments = document.querySelector('#departments');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });


    var ddSites = document.querySelector('#sites');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });


    var ddEmployees = document.querySelector('#employees');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployees.appendChild(option);
    });


    //var ddDepartmentSubCategory = document.querySelector('#department-sub-category');
    //$(ddDepartmentSubCategory).empty();

    //var lstDepartmentSubCategories = (DropDownsData.DepartmentSubCategories || []);
    //var option = new Option(); // You may want to add a label like "--Select--"
    //ddDepartmentSubCategory.appendChild(option);

    //lstDepartmentSubCategories.forEach(function (item) {
    //    var option = new Option(item.SubCategoryName, item.Id, false, false);
    //    ddDepartmentSubCategory.appendChild(option);
    //});

    if (cb) {
        cb();
    }
}


///department change

$(document).on('change', '#departments', function (e) {
    e.preventDefault();

    var selectedDepartments = $("#departments").val(); // This is an array if multiple selected

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


$("#SubmitAssignForm").on("click", function () {

    var training = {};
    training.visibleTo = $("#visible-to").val();
    training.departments = ($("#departments").val()).length > 0 ? JSON.stringify($("#departments").val()) : null;
    training.departmentsSubCategories = ($("#department-sub-category").val()).length > 0 ? JSON.stringify($("#department-sub-category").val()) : null;
    training.employees = ($("#employees").val()).length > 0 ? JSON.stringify($("#employees").val()) : null;
    training.sites = ($("#sites").val()).length > 0 ? JSON.stringify($("#sites").val()) : null;

    if (Id != null && Id != '') {
        training.assigneId = Id;
    }
    //if (Window.JustConsole) { console.log(news); return; }
    var url = sitePath + 'api/TrainingAssignAPI/AssignTraining';

    Ajax.post(url, training, function (response) {
        if (response.StatusCode == 200) {
            Swal.fire({
                text: "training Assigned successfully.",
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

                    var modalEl = document.getElementById('AssignTrainingModal');
                    var modal = bootstrap.Modal.getInstance(modalEl);
                    modal.hide();

                    //window.location.href = "/NewsFeed/NewsFeedManagement";
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
});