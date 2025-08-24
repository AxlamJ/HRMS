var EmployeeId = getParameterByName("EmployeeId") || null;
var EmployeeCode = getParameterByName("EmployeeCode") || null;
var EmployeeData = null;
HRMSUtil.onDOMContentLoaded(function () {


    InitializeDatePickers.init();
    registerEventListners.init();
    PopulateDropDowns(function () {
        if (EmployeeId) {
            loadData()
        }
    });


    unspin();
});

var InitializeDatePickers = function () {
    return {
        init: function () {

            $("#dob").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#hiring-date").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#probation-from").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#probation-to").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });
        }
    }
}();

var registerEventListners = function () {

    return {
        init: function () {
            $(document).on("click", '.btn-upload-photo', function () {
                $(this).siblings('.profile-photo').click();
            });


            $(document).on('change', '.profile-photo', handleImageUpload);
            //$(document).on("click", ".btn-upload-photo", onbtnUploadClick);

            $(document).on("click", '.btn-del-image', function () {
                var image = $(this).closest('.photo-ctrl').find('.user-photo');
                image[0].src = '/Images/no-image.svg';
                hideImageEditButtons(image);
            });
        }
    };

}();



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

    if (cb) {
        cb();
    }


}



function handleImageUpload(event) {
    var imageFile = event.target.files[0];
    if (imageFile.type.indexOf("image") < 0) {
        ShowError("Only image files are allowed");
        event.target.value = '';
        return;
    }
    var img = event.target.parentElement.parentElement.parentElement.querySelector('.user-photo');
    $(img).data('is-new', true)
    var options = {
        maxSizeMB: 1,
        maxWidthOrHeight: 600
    };
    imageCompression(imageFile, options)
        .then(function (compressedFile) {//compression, Promise resolved.
            var imageName = "img_" + Date.now() + "." + compressedFile.type.split("/").pop();
            imageCompression.getDataUrlFromFile(compressedFile)
                .then(function (dataURLCompressed) {//convert from blob to base64, Promise resolved.
                    //saveImageToAzureBlob(dataURLCompressed, imageName, img);
                    //saveImageToAzureBlob(dataURLCompressed, imageName, img);

                    UploadImage(dataURLCompressed, imageName, img)
                    //showImage(dataURLCompressed, img);

                })
        })
        .catch(function (error) {//compression, Promise rejected.
            //ShowError(error.message);
            console.log(error, "104");
        });
    $(event.target).val("");
}

function UploadImage(dataURL, imageName, imgElement) {

    var url = sitePath + 'api/EmployeeAPI/UploadImage';
    var Image = { base64: dataURL, fileName: imageName };

    Ajax.post(url, Image, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            showImage(response.imageUrl, imgElement);

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

function showImage(dataURL, imgElement) {
    var loader = getImageLoader();
    $(imgElement).closest('.photo-ctrl').prepend($(loader).fadeIn());

    imgElement.src = dataURL;
    imgElement.onload = function () {
        removeImageLoader(loader);
        imgElement.onload = null;
        showImageEditButtons(imgElement);
    };
}

function showImageEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-btns').show();
    $(img).closest('.photo-ctrl').find('.btn-del-image').show();
    $(img).closest('.photo-ctrl').find('.image-select-buttons').hide();
}

function hideImageEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-btns').hide();
    $(img).closest('.photo-ctrl').find('.image-select-buttons').show();
}

function getImageLoader() {

    var loaderContainer = document.createElement('div');
    var loader = document.createElement('div');
    var loaderChild = document.createElement('div');

    loaderContainer.className = 'loader-container';
    loader.className = 'loader';
    loaderChild.className = 'loader-child';

    loader.appendChild(loaderChild);
    loaderContainer.appendChild(loader);

    return loaderContainer;

}

function removeImageLoader(loader) {
    $(loader).remove();
}

function loadData() {
    var url = sitePath + "api/EmployeeAPI/GetEmployeeById?Id=" + EmployeeId;
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            EmployeeData = resp.Employee;
            $("#user-photo").attr("src", EmployeeData.ProfilePhotoUrl);
            $("#employee-code").val(EmployeeData.EmployeeCode);
            $("#firstname").val(EmployeeData.FirstName);
            $("#lastname").val(EmployeeData.LastName);
            $("#hiring-date").flatpickr({
                dateFormat: "d/m/Y",
            }).setDate(FormatDate(EmployeeData.HiringDate));
            $("#dob").flatpickr({
                dateFormat: "d/m/Y",
            }).setDate(FormatDate(EmployeeData.DOB));
            $("#gender").val(EmployeeData.Gender).trigger("change");

            $("#email").val(EmployeeData.Email);
            $("#phone").val(EmployeeData.PhoneNumber);
            $("#city").val(EmployeeData.City);
            $("#postal-code").val(EmployeeData.PostalCode);
            $("#department").val(EmployeeData.DepartmentId).trigger("change");
            $("#site").val(EmployeeData.SiteId).trigger("change");
            $("#address").val(EmployeeData.Address);
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

// These are the constraints used to validate the form
var constraints = {
    "employee-code": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "firstname": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "lastname": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "hiring-date": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "dob": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "gender": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "email": {
        // Site Time Zone is required
        presence: { message: "is required." },
        email: true
    },
    "phone": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "city": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "postal-code": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "department": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "site": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "address": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "employee-status": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "marital-status": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "alternative-email": {
        email: true
    }
};


$("#add-update-employee").on("click", function () {
    var form = document.querySelector("#add-update-employee-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var employee = {};

        employee.profilePhotoUrl = $("#user-photo").attr('src') == "/Images/no-image.svg" ? "" : $("#user-photo").attr('src');
        employee.employeeCode = ($("#employee-code").val() != null && $("#employee-code").val() != 0 && $("#employee-code").val() != "") ? parseInt($("#employee-code").val()) : null;
        employee.firstName = $("#firstname").val();
        employee.lastName = $("#lastname").val();
        employee.joiningDate = $("#joining-date").val();
        employee.dob = $("#dob").val();
        employee.gender = $("#gender").val();
        employee.email = $("#email").val();
        employee.phoneNumber = $("#phone").val();
        employee.city = $("#city").val();
        employee.postalCode = $("#postal-code").val();
        employee.departmentId = parseInt($("#department").val());
        employee.departmentName = $("#department :Selected").text();
        employee.siteId = parseInt($("#site").val());
        employee.siteName = $("#site :Selected").text();
        employee.address = $("#address").val();
        employee.positionId = $("#position").val() != "" ? parseInt($("#position").val()) : null;
        employee.positionName = $("#position").val() != "" ? $("#position :Selected").text() : null;
        employee.maritalStatus = $("#marital-status").val();
        employee.managerId = $("#manager").val() != "" ? $("#manager").val() : null;
        employee.managerName = $("#manager").val() != "" ? $("#manager :Selected").text() : null;
        employee.alternativeEmail = $("#alternative-email").val();
        employee.probationDateStart = $("#probation-from").val();
        employee.probationDateEnd = $("#probation-to").val();
        employee.employmentStatusId = $("#employment-status").val() != "" ? parseInt($("#employment-status").val()) : null;
        employee.employmentStatus = $("#employment-status").val() != "" ? $("#employment-status :Selected").text() : null;
        employee.employmentLevelId = $("#employment-level").val() != "" ? parseInt($("#employment-level").val()) : null;
        employee.employmentLevel = $("#employment-level").val() != "" ? $("#employment-level :Selected").text() : null;
        employee.status = parseInt($("#employee-status").val());

        if (EmployeeId != null) {
            employee.Id = EmployeeData.Id;
        }

        if (Window.JustConsole) { console.log(employee); return; }
        var url = sitePath + 'api/EmployeeAPI/UpsertEmployee';

        Ajax.post(url, employee, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Employee added successfully.",
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