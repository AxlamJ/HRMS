
$(document).on('click', '#btn-create-department', function () {
    $("#add-new-departments-modal").modal('show');
});

$(document).on('click', '.btn-new-category', function () {
    $("#add-new-sub-category-modal").modal('show');
});



$(document).on('click', '.btn-edit-department', function () {

    var $self = $(this);

    var deptid = $self.data('id');
    var deptname = $self.data('deptname');
    $(document).find('#add-update-dept').data('deptid', deptid);
    $(document).find("#dept-name").val(deptname);

    $("#add-new-departments-modal").modal('show');
});

$(document).on('click', '.btn-edit-subcategory', function () {

    var $self = $(this);

    var deptid = $self.data('deptid');
    var deptname = $self.data('deptname');
    var subcategoryId = $self.data('id');
    var categoryname = $self.data('categoryname');
    $(document).find('#btn-add-new-sub-category').data('id', subcategoryId);
    $(document).find("#department").val(deptid).trigger("change");
    $(document).find("#sub-category-name").val(categoryname);

    $("#add-new-sub-category-modal").modal('show');
});

// These are the constraints used to validate the form
var constraints = {
    "dept-name": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    }
};

$("#add-update-dept").on("click", function () {
    var form = document.querySelector("#add-update-dept-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var department = {};

        department.departmentName = $("#dept-name").val();
        var DepartmentId = $(document).find('#add-update-dept').data('deptid') || null;

        if (DepartmentId != null) {
            department.departmentId = DepartmentId;
        }
        //site.countryId

        var url = sitePath + 'api/DepartmentAPI/UpsertDepartment';

        Ajax.post(url, department, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Department saved successfully.",
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
                    window.location.href = "/Home/ManageDepartments";
                    if (result.isConfirmed) { }
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

        //showSuccess();
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

// These are the constraints used to validate the form
var subcategoryconstraints = {
    "department": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    },
    "sub-category-name": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    }
};

$("#btn-add-new-sub-category").on("click", function () {
    var form = document.querySelector("#sub-category-form");
    var errors = validate(form, subcategoryconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var subcategory = {};

        subcategory.departmentId = $("#department").val();
        subcategory.departmentName = $("#department :Selected").text();
        subcategory.subCategoryName = $("#sub-category-name").val();
        var SubCategoryId = $(document).find('#btn-add-new-sub-category').data('id') || null;

        if (SubCategoryId != null) {
            subcategory.Id = SubCategoryId;
        }
        //site.countryId

        var url = sitePath + 'api/DepartmentAPI/UpsertDepartmentSubCategory';

        Ajax.post(url, subcategory, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Department Sub-Category saved successfully.",
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
                    window.location.href = "/Home/ManageDepartments";
                    if (result.isConfirmed) { }
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

        //showSuccess();
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
