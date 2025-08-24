$(document).on("click", ".btn-add-supporting-docs", function () {
    $('#modal-supporting-docs').modal('show');

});



var supportingdoc = 0;
$(document).on("click", "#btn-add-supporting-doc", function () {

    var documentName = $("#supporting-document-name").val();

    var fileName = $(document).find('#supporting-doc-name').data('name');
    var fileUrl = $(document).find('#supporting-doc-name').data('href');

    if (documentName == null || documentName.trim() == "" || fileName == "" || fileName == null) {
        Swal.fire({
            text: "Please provide document name as well as document.",
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
    } else {

        var doc = {};

        doc.employeeCode = parseInt(EmployeeData.EmployeeCode);
        doc.documentName = documentName;
        doc.documentFileName = fileName;
        doc.documentUrl = fileUrl;

        if (Window.JustConsole) { console.log(doc); return; }
        var url = sitePath + 'api/EmployeeAPI/UpsertSupportingDocuments';

        Ajax.post(url, doc, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Supporting document added successfully.",
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
                        $('#modal-supporting-docs').modal('hide');
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
});

var SupportingDocs = null;

$(document).on("click", "#nav-supporting-documents-tab", function () {

    var url = sitePath + "api/EmployeeAPI/GetSupportingDocsByEmpCode?EmployeeCode=" + parseInt(EmployeeData.EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            SupportingDocs = resp.SupportingDocs;

            if (SupportingDocs != null && SupportingDocs.length > 0) {
                var $cntnr = $(document).find('#SupportingDocumentsList');
                $cntnr.html('');

                for (var i = 0; i < SupportingDocs.length; i++) {
                    var template = `<div class="d-flex flex-wrap align-items-center">
                            <div id="document_name_sec_${SupportingDocs[i].Id}">
                                <div class="fs-6 fw-bold mb-1">${SupportingDocs[i].DocumentName}</div>
                                <div class="fw-semibold text-gray-600" id="supporting-doc-name_${SupportingDocs[i].Id}" data-href="${SupportingDocs[i].DocumentUrl}" data-name="${SupportingDocs[i].DocumentFileName}">${SupportingDocs[i].DocumentFileName}</div>
                            </div>
                            <div class="ms-auto supporting-document-btns">
                                <a class="btn btn-primary btn-icon href-download-supporting-document_${SupportingDocs[i].Id} mt-10px" href="${SupportingDocs[i].DocumentUrl}" download="${SupportingDocs[i].DocumentFileName}" target="_blank"><i class="fa fa-download"></i></a>
                                <button type="button" class="btn btn-icon btn-danger btn-delete-supporting-document mt-10px" data-id="${SupportingDocs[i].Id}" >
                                    <i class="fa fa-trash"></i>
                                </button>

                            </div>
                        </div>
                        <div class="separator separator-dashed my-6"></div>`
                    $cntnr.append(template);
                }
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
});



$(document).on("click", ".btn-delete-supporting-document", function () {

    var $self = $(this);
    var docId = $self.data("id");


    Ajax.post(sitePath + "api/EmployeeAPI/DeleteSupportingDocumentById?Id=" + docId, null, function (resp) {
        if (resp.StatusCode == 200) {

            Swal.fire({
                text: "Supporting document deleted Successfully.",
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
});
