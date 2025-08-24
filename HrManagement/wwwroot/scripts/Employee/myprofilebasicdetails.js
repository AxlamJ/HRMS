
$(document).on("click", ".btn-basic-details", function () {

    var form = document.querySelector("#basic-details-form");
    if (EmployeeData.ProfilePhotoUrl == "" || EmployeeData.ProfilePhotoUrl == null) {
        EmployeeData.ProfilePhotoUrl = "/Images/no-image.svg";
    }
    if (EmployeeData.SignaturePhotoUrl == "" || EmployeeData.SignaturePhotoUrl == null) {
        EmployeeData.SignaturePhotoUrl = "/Images/no-image.svg";
    }
    if (EmployeeData.LicensePhotoUrl == "" || EmployeeData.LicensePhotoUrl == null) {
        EmployeeData.LicensePhotoUrl = "/Images/no-image.svg";
    }
    $(form).find("#user-photo").attr("src", EmployeeData.ProfilePhotoUrl);
    $(form).find("#sign-photo").attr("src", EmployeeData.SignaturePhotoUrl);
    $(form).find("#licen-photo").attr("src", EmployeeData.LicensePhotoUrl);
    $(form).find("#employee-code").val(EmployeeData.EmployeeCode).attr("disabled", "disabled");
    $(form).find("#first-name").val(EmployeeData.FirstName);
    $(form).find("#last-name").val(EmployeeData.LastName);
    $(form).find("#gender").val(EmployeeData.Gender).trigger("change");
    $(form).find("#marital-status").val(EmployeeData.MaritalStatus).trigger("change");
    $(form).find("#dob").flatpickr({
        dateFormat: "d/m/Y",
    }).setDate(FormatDate(EmployeeData.DOB));
    $(form).find("#on-boarding-date").flatpickr({
        dateFormat: "d/m/Y",
    }).setDate(FormatDate(EmployeeData.OnBoardingDate));
    $(form).find("#email").val(EmployeeData.Email);
    $(form).find("#phone").val(EmployeeData.PhoneNumber);
    $(form).find("#country").val(EmployeeData.CountryId).trigger("change");
    $(form).find("#city").val(EmployeeData.City);
    //$(form).find("#time-zone").val(EmployeeData.TimeZone).trigger("change");
    $(form).find("#sponsorship").val(EmployeeData.SponsorShip).trigger("change");
    $(form).find("#work-eligibility").val(EmployeeData.WorkEligibility).trigger("change");
    $(form).find("#immigration-status").val(EmployeeData.ImmigrationStatus);
    $(form).find("#other").val(EmployeeData.Other);
    $(form).find("#SIN-No").val(EmployeeData.SinNo);
    if (EmployeeData.SinDocumentName != null && EmployeeData.SinDocumentName != "") {
        $(form).find("#sin-name").text(EmployeeData.SinDocumentName);
        $(form).find("#sin-name").data('name', EmployeeData.SinDocumentName);
        $(form).find("#sin-name").data('href', EmployeeData.SinDocumentUrl);
        $(form).find(".href-download-sin").attr('href', EmployeeData.SinDocumentUrl);
        $(form).find(".href-download-sin").attr('download', EmployeeData.SinDocumentName);
        $(form).find(".href-download-sin").toggleClass('d-none');
    }
    if (EmployeeData.ChequeDocumentName != null && EmployeeData.ChequeDocumentName != "") {
        $(form).find("#cheque-name").text(EmployeeData.ChequeDocumentName);
        $(form).find("#cheque-name").data('name', EmployeeData.ChequeDocumentName);
        $(form).find("#cheque-name").data('href', EmployeeData.ChequeDocumentUrl);
        $(form).find(".href-download-cheque").attr('href', EmployeeData.ChequeDocumentUrl);
        $(form).find(".href-download-cheque").attr('download', EmployeeData.ChequeDocumentName);
        $(form).find(".href-download-cheque").toggleClass('d-none');
    }
    if (EmployeeData.FederalTaxDocumentName != null && EmployeeData.FederalTaxDocumentName != "") {
        $(form).find("#tax-form-federal-name").text(EmployeeData.FederalTaxDocumentName);
        $(form).find("#tax-form-federal-name").data('name', EmployeeData.FederalTaxDocumentName);
        $(form).find("#tax-form-federal-name").data('href', EmployeeData.FederalTaxDocumentUrl);
        $(form).find(".href-download-tax-form-federal").attr('href', EmployeeData.FederalTaxDocumentUrl);
        $(form).find(".href-download-tax-form-federal").attr('download', EmployeeData.FederalTaxDocumentName);
        $(form).find(".href-download-tax-form-federal").toggleClass('d-none');
    }
    if (EmployeeData.AlbertaTaxDocumentName != null && EmployeeData.AlbertaTaxDocumentName != "") {
        $(form).find("#tax-form-alberta-name").text(EmployeeData.AlbertaTaxDocumentName);
        $(form).find("#tax-form-alberta-name").data('name', EmployeeData.AlbertaTaxDocumentName);
        $(form).find("#tax-form-alberta-name").data('href', EmployeeData.AlbertaTaxDocumentUrl);
        $(form).find(".href-download-tax-form-alberta").attr('href', EmployeeData.AlbertaTaxDocumentUrl);
        $(form).find(".href-download-tax-form-alberta").attr('download', EmployeeData.AlbertaTaxDocumentName);
        $(form).find(".href-download-tax-form-alberta").toggleClass('d-none');
    }
    $('#modal-basic-details').modal('show');

});




// These are the constraints used to validate the form
var basicdetailsconstraints = {
    "employee-code": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "first-name": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "last-name": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "gender": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "marital-status": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "dob": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "on-boarding-date": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "email": {
        // Site Time Zone is required
        presence: { message: "is required." },
        email: true
    },
    "phone": {
        presence: { message: "is required." },
    },
    "country": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "city": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "SIN-No": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    //"time-zone": {
    //    // Site Time Zone is required
    //    presence: { message: "is required." }
    //}

};



$("#btn-update-basic-details").on("click", function () {
    var form = document.querySelector("#basic-details-form");
    var errors = validate(form, basicdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var employee = {};

        employee.Id = EmployeeData.Id;
        employee.profilePhotoUrl = $("#user-photo").attr('src') == "/Images/no-image.svg" ? "" : $("#user-photo").attr('src');
        employee.signaturePhotoUrl = $("#sign-photo").attr('src') == "/Images/no-image.svg" ? "" : $("#sign-photo").attr('src');
        employee.licensePhotoUrl = $("#licen-photo").attr('src') == "/Images/no-image.svg" ? "" : $("#licen-photo").attr('src');
        employee.employeeCode = ($("#employee-code").val() != null && $("#employee-code").val() != 0 && $("#employee-code").val() != '') ? parseInt($("#employee-code").val()) : null;
        employee.firstName = $("#first-name").val();
        employee.lastName = $("#last-name").val();
        employee.gender = $("#gender").val();
        employee.maritalStatus = $("#marital-status").val();
        employee.dob = $("#dob").val();
        employee.onBoardingDate = $("#on-boarding-date").val();
        employee.email = $("#email").val();
        employee.phoneNumber = $("#phone").val();
        //employee.alternativeEmail = $("#alternative-email").val();
        employee.country = $("#country :Selected").text();
        employee.countryId = $("#country").val();
        employee.city = $("#city").val();
        //employee.timeZone = $("#time-zone").val();
        //employee.timeZoneOffset = $("#time-zone :Selected").data("offset");
        //employee.timeZoneName = $("#time-zone :Selected").text();
        employee.Sponsorship = $("#sponsorship").val();
        employee.WorkEligibility = $("#work-eligibility").val();
        employee.ImmigrationStatus = $("#immigration-status").val();
        employee.Other = $("#other").val();
        employee.SinNo = $("#SIN-No").val();

        employee.SinDocumentName = $("#sin-name").data('name');
        employee.SinDocumentUrl = $("#sin-name").data('href');
        employee.ChequeDocumentName = $("#cheque-name").data('name');
        employee.ChequeDocumentUrl = $("#cheque-name").data('href');
        employee.FederalTaxDocumentName = $("#tax-form-federal-name").data('name');
        employee.FederalTaxDocumentUrl = $("#tax-form-federal-name").data('href');
        employee.AlbertaTaxDocumentName = $("#tax-form-alberta-name").data('name');
        employee.AlbertaTaxDocumentUrl = $("#tax-form-alberta-name").data('href');

        if (Window.JustConsole) { console.log(employee); return; }
        var url = sitePath + 'api/EmployeeAPI/UpdateBasicDetails';

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




var registerEventListners = function () {

    return {
        init: function () {
            $(document).on("click", '.btn-upload-photo', function () {
                $(this).siblings('.profile-photo').click();
            });
            $(document).on('change', '.profile-photo', handleImageUpload);
            $(document).on("click", '.btn-del-image', function () {
                var image = $(this).closest('.photo-ctrl').find('.user-photo');
                image[0].src = '/Images/no-image.svg';
                hideImageEditButtons(image);
            });

            $(document).on("click", '.btn-upload-sign', function () {
                $(this).siblings('.signature-photo').click();
            });
            $(document).on('change', '.signature-photo', handleSignUpload);
            $(document).on("click", '.btn-del-sign', function () {
                var image = $(this).closest('.photo-ctrl').find('.sign-photo');
                image[0].src = '/Images/no-image.svg';
                hideSignEditButtons(image);
            });

            $(document).on("click", '.btn-upload-license', function () {
                $(this).siblings('.license-photo').click();
            });
            $(document).on('change', '.license-photo', handleLicenseUpload);
            $(document).on("click", '.btn-del-license', function () {
                var image = $(this).closest('.photo-ctrl').find('.licen-photo');
                image[0].src = '/Images/no-image.svg';
                hideLicenseEditButtons(image);
            });

            $(document).on("click", '.btn-upload-cert', function () {
                $(this).siblings('.certificate-photo').click();
            });
            $(document).on('change', '.certificate-photo', handleCertImageUpload);
            $(document).on("click", '.btn-del-cert-image', function () {
                var image = $(this).closest('.photo-ctrl').find('.cert-photo');
                image[0].src = '/Images/no-image-1.svg';
                hideImageCertEditButtons(image);
            });

            $(document).on("click", '.btn-upload-sin', function () {
                $(this).siblings('.upload-sin').click();
            });
            $(document).on('change', '.upload-sin', handleSINUpload);

            $(document).on("click", '.btn-upload-cheque', function () {
                $(this).siblings('.upload-cheque').click();
            });
            $(document).on('change', '.upload-cheque', handleChequeUpload);

            $(document).on("click", '.btn-upload-tax-form-federal', function () {
                $(this).siblings('.upload-tax-form-federal').click();
            });
            $(document).on('change', '.upload-tax-form-federal', handleTaxFormFederalUpload);

            $(document).on("click", '.btn-upload-tax-form-alberta', function () {
                $(this).siblings('.upload-tax-form-alberta').click();
            });
            $(document).on('change', '.upload-tax-form-alberta', handleTaxFormAlbertaUpload);

            $(document).on("click", '.btn-upload-liability-insurance', function () {
                $(this).siblings('.upload-liability-insurance').click();
            });
            $(document).on('change', '.upload-liability-insurance', handleLiabilityInsuranceUpload);

            $(document).on("click", '.btn-upload-registration-number', function () {
                $(this).siblings('.upload-registration-number').click();
            });
            $(document).on('change', '.upload-registration-number', handleRegistrationNumberUpload);

            $(document).on("click", '.btn-upload-business-cheque', function () {
                $(this).siblings('.upload-business-cheque').click();
            });
            $(document).on('change', '.upload-business-cheque', handleBusinessChequeUpload);

            $(document).on("click", '.btn-upload-contractor-business-cheque', function () {
                $(this).siblings('.upload-contractor-business-cheque').click();
            });
            $(document).on('change', '.upload-contractor-business-cheque', handleContractorBusinessChequeUpload);

            $(document).on("click", '.btn-upload-supporting-document', function () {
                $(this).siblings('.upload-supporting-document').click();
            });
            $(document).on('change', '.upload-supporting-document', handleSupportingDocumentsUpload);


        }
    };

}();


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

function handleSignUpload(event) {
    var imageFile = event.target.files[0];
    if (imageFile.type.indexOf("image") < 0) {
        ShowError("Only image files are allowed");
        event.target.value = '';
        return;
    }
    var img = event.target.parentElement.parentElement.parentElement.querySelector('.sign-photo');
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

                    UploadSign(dataURLCompressed, imageName, img)
                    //showImage(dataURLCompressed, img);

                })
        })
        .catch(function (error) {//compression, Promise rejected.
            //ShowError(error.message);
            console.log(error, "104");
        });
    $(event.target).val("");
}
function handleLicenseUpload(event) {
    var imageFile = event.target.files[0];
    if (imageFile.type.indexOf("image") < 0) {
        ShowError("Only image files are allowed");
        event.target.value = '';
        return;
    }
    var img = event.target.parentElement.parentElement.parentElement.querySelector('.licen-photo');
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

                    UploadLicense(dataURLCompressed, imageName, img)
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

function UploadSign(dataURL, imageName, imgElement) {

    var url = sitePath + 'api/EmployeeAPI/UploadImage';
    var Image = { base64: dataURL, fileName: imageName };

    Ajax.post(url, Image, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            showSign(response.imageUrl, imgElement);

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

function UploadLicense(dataURL, imageName, imgElement) {

    var url = sitePath + 'api/EmployeeAPI/UploadImage';
    var Image = { base64: dataURL, fileName: imageName };

    Ajax.post(url, Image, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            showLicense(response.imageUrl, imgElement);

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


function getBase64(file, callback) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        callback(reader.result, file);
    };
    reader.onerror = function (error) {
        console.log('Error: ', error)
        callback(null);
    };
}

function handleSINUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#sin-name');
    var $downloadbtn = $(document).find('.href-download-sin');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleChequeUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#cheque-name');
    var $downloadbtn = $(document).find('.href-download-cheque');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleTaxFormFederalUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#tax-form-federal-name');
    var $downloadbtn = $(document).find('.href-download-tax-form-federal');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleTaxFormAlbertaUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#tax-form-alberta-name');
    var $downloadbtn = $(document).find('.href-download-tax-form-alberta');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleLiabilityInsuranceUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#liability-insurance-name');
    var $downloadbtn = $(document).find('.href-download-liability-insurance');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleRegistrationNumberUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#registration-number-name');
    var $downloadbtn = $(document).find('.href-download-registration-number');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleBusinessChequeUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#business-cheque-name');
    var $downloadbtn = $(document).find('.href-download-business-cheque');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}

function handleContractorBusinessChequeUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#contractor-business-cheque-name');
    var $downloadbtn = $(document).find('.href-download-contractor-business-cheque');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}
function handleSupportingDocumentsUpload(event) {
    var $self = $(event.target);
    var $cntnr = $(document).find('#supporting-doc-name');
    var $downloadbtn = $(document).find('.href-download-supporting-document');
    var file = event.target.files[0];

    $self.val(''); // Clear file input after reading

    getBase64(file, function (base64, file) {
        if (base64) {
            UploadDocuments(base64, file, $cntnr, $downloadbtn);
        }
        hideSpinner();
    });
}
function UploadDocuments(base64, file, $cntnr, $downloadbtn) {
    var url = sitePath + 'api/EmployeeAPI/UploadDocuments';
    var Documents = { base64: base64, fileName: file.name };

    Ajax.post(url, Documents, function (response) {
        if (response.StatusCode == 200) {

            $cntnr.text(response.fileName)
            $cntnr.data('href', response.fileUrl);
            $cntnr.data('name', response.fileName);
            $downloadbtn.attr('href', response.fileUrl);
            $downloadbtn.attr('download', file.fileName); // optional: force download with filename
            $downloadbtn.toggleClass('d-none');

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

function showSign(dataURL, imgElement) {
    var loader = getImageLoader();
    $(imgElement).closest('.photo-ctrl').prepend($(loader).fadeIn());

    imgElement.src = dataURL;
    imgElement.onload = function () {
        removeImageLoader(loader);
        imgElement.onload = null;
        showSignEditButtons(imgElement);
    };
}

function showSignEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-sign-btns').show();
    $(img).closest('.photo-ctrl').find('.btn-del-sign').show();
    $(img).closest('.photo-ctrl').find('.image-select-sign-buttons').hide();
}

function hideSignEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-sign-btns').hide();
    $(img).closest('.photo-ctrl').find('.image-select-sign-buttons').show();
}

function showLicense(dataURL, imgElement) {
    var loader = getImageLoader();
    $(imgElement).closest('.photo-ctrl').prepend($(loader).fadeIn());

    imgElement.src = dataURL;
    imgElement.onload = function () {
        removeImageLoader(loader);
        imgElement.onload = null;
        showLicenseEditButtons(imgElement);
    };
}

function showLicenseEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-license-btns').show();
    $(img).closest('.photo-ctrl').find('.btn-del-license').show();
    $(img).closest('.photo-ctrl').find('.image-select-license-buttons').hide();
}

function hideLicenseEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-license-btns').hide();
    $(img).closest('.photo-ctrl').find('.image-select-license-buttons').show();
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