$(document).on("click", ".btn-add-cert", function () {
    $('#modal-cert-details').modal('show');

});



// These are the constraints used to validate the form
var certdetailsconstraints = {
    "license-certificate-name": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "issuing-organization": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "credential-id": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "url": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "issue-date": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "expiration-date": {
        // Site Time Zone is required
        presence: { message: "is required." }
    }

};



$("#btn-add-cert").on("click", function () {
    var form = document.querySelector("#cert-details-form");
    var errors = validate(form, certdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var certificate = {};

        //employee.Id = EmployeeData.Id;
        certificate.EmployeeCode = parseInt(EmployeeData.EmployeeCode);
        certificate.Name = $(form).find("#license-certificate-name").val();
        certificate.IssuingOrganization = $(form).find("#issuing-organization").val();
        certificate.CredentialId = $(form).find("#credential-id").val();
        certificate.URL = $(form).find("#url").val();
        certificate.IssueDate = $(form).find("#issue-date").val();
        certificate.ExpirationDate = $(form).find("#expiration-date").val();

        if (Window.JustConsole) { console.log(certificate); return; }
        var url = sitePath + 'api/EmployeeAPI/UpdateCertDetails';

        Ajax.post(url, certificate, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Certificate/License details submitted successfully.",
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





function handleCertImageUpload(event) {
    var imageFile = event.target.files[0];
    if (imageFile.type.indexOf("image") < 0) {
        ShowError("Only image files are allowed");
        event.target.value = '';
        return;
    }
    var img = event.target.parentElement.parentElement.parentElement.querySelector('.cert-photo');
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

                    UploadCertImage(dataURLCompressed, imageName, img)
                    //showImage(dataURLCompressed, img);

                })
        })
        .catch(function (error) {//compression, Promise rejected.
            //ShowError(error.message);
            console.log(error, "104");
        });
    $(event.target).val("");
}

function UploadCertImage(dataURL, imageName, imgElement) {

    var url = sitePath + 'api/EmployeeAPI/UploadCertImage';
    var Image = { base64: dataURL, fileName: imageName };

    Ajax.post(url, Image, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            showCertImage(response.imageUrl, imgElement);

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

function showCertImage(dataURL, imgElement) {
    var loader = getImageLoader();
    $(imgElement).closest('.photo-ctrl').prepend($(loader).fadeIn());

    imgElement.src = dataURL;
    imgElement.onload = function () {
        removeImageLoader(loader);
        imgElement.onload = null;
        showImageCertEditButtons(imgElement);
    };
}

function showImageCertEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-cert-btns').show();
    $(img).closest('.photo-ctrl').find('.btn-del-cert-image').show();
    $(img).closest('.photo-ctrl').find('.image-cert-select-buttons').hide();
}

function hideImageCertEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-cert-btns').hide();
    $(img).closest('.photo-ctrl').find('.image-cert-select-buttons').show();
}



$(document).on("click", "#nav-license-tab", function () {

    var url = sitePath + "api/EmployeeAPI/GetCertByEmpCode?EmployeeCode=" + parseInt(EmployeeData.EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            CertList = resp.CertLicenses;

            const tableContainer = document.getElementById("CertLicenseList");
            tableContainer.innerHTML = ""; // Clear any existing table

            if (CertList.length === 0) {
                tableContainer.innerHTML = "<div class='mt-3 text-center mb-0 font-18 text-primary'><i class='fa fa-info-circle mr-1'></i> We couldn't find any records.</div>";
            }

            // Create table element
            const table = document.createElement("table");
            table.className = "table table-bordered table-responsive table-striped";
            // Create table head
            const thead = document.createElement("thead");
            const headerRow = document.createElement("tr");

            const headers = [
                { label: "Name", key: "Name" },
                { label: "Organization", key: "IssuingOrganization" },
                { label: "Issue Date", key: "IssueDate" },
                { label: "Expiration Date", key: "ExpirationDate" },
            ];
            // Dynamically generate headers based on keys
            headers.forEach(function (header) {
                const th = document.createElement("th");
                th.textContent = header.label;
                headerRow.appendChild(th);
            });

            // Add "Action" header
            const actionTh = document.createElement("th");
            actionTh.textContent = "Action";
            headerRow.appendChild(actionTh);

            thead.appendChild(headerRow);
            table.appendChild(thead);

            // Create table body
            const tbody = document.createElement("tbody");

            CertList.forEach(function (cert, index) {
                const row = document.createElement("tr");

                headers.forEach(function (header) {
                    const td = document.createElement("td");
                    var value = cert[header.key];
                    if (header.key == "IssueDate" || header.key == "ExpirationDate") {
                        value = FormatDateTime(value);
                        td.textContent = value || "";
                    }
                    else {
                        td.textContent = value || "";
                    }
                    row.appendChild(td);
                });

                // Add Delete button with Font Awesome icon
                const actionTd = document.createElement("td");

                const deleteBtn = document.createElement("button");
                deleteBtn.className = "btn btn-danger btn-sm btn-icon btn-delete-cert";
                deleteBtn.setAttribute("data-id", cert.id);
                deleteBtn.innerHTML = '<i class="fa fa-trash"></i>';

                actionTd.appendChild(deleteBtn);
                row.appendChild(actionTd);


                tbody.appendChild(row);

            });

            table.appendChild(tbody);
            tableContainer.appendChild(table);

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

