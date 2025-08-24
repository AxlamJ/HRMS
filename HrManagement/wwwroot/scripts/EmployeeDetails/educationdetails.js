var EducationDetails = null;


$(document).on("click", ".btn-education-detail-new", function () {
    $('#modal-education-details').modal('show');
});



// These are the constraints used to validate the form
var educationdetailsconstraints = {
    "college-university": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "degree": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "major": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "graduation-date": {
        // Site Time Zone is required
        presence: { message: "is required." }
    }

};



$("#btn-update-education-details").on("click", function () {
    var form = document.querySelector("#education-details-form");
    var errors = validate(form, educationdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var education = {};

        //employee.Id = EmployeeData.Id;
        education.EmployeeCode = parseInt(_EmployeeData.EmployeeCode);
        education.collegeUniversity = $(form).find("#college-university").val();
        education.degree = $(form).find("#degree").val();
        education.major = $(form).find("#major").val();
        education.yearGraduated = $(form).find("#graduation-date").val();

        if (Window.JustConsole) { console.log(education); return; }
        var url = sitePath + 'api/EmployeeAPI/UpsertEducationDetails';

        Ajax.post(url, education, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Employee Education details saved successfully.",
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


$(document).on("click", "#nav-education-tab", function () {

    var url = sitePath + "api/EmployeeAPI/GetEmployeeEducationDetails?EmployeeCode=" + parseInt(_EmployeeData.EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            EducationDetails = resp.EducationDetails;
            console.log(resp.ConatctsList);


            const tableContainer = document.getElementById("education-details-table");
            tableContainer.innerHTML = ""; // Clear any existing table

            if (EducationDetails.length === 0) {
                tableContainer.innerHTML = "<div class='mt-3 text-center mb-0 font-18 text-primary'><i class='fa fa-info-circle mr-1'></i> We couldn't find any records.</div>";
                return;
            }

            // Create table element
            const table = document.createElement("table");
            table.className = "table table-bordered table-responsive table-striped";
            // Create table head
            const thead = document.createElement("thead");
            const headerRow = document.createElement("tr");

            const headers = [
                { label: "College / University", key: "CollegeUniversity" },
                { label: "Degree", key: "Degree" },
                { label: "Major", key: "Major" },
                { label: "Graduation Year", key: "YearGraduated" },
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

            EducationDetails.forEach(function (education, index) {
                const row = document.createElement("tr");

                headers.forEach(function (header) {
                    const td = document.createElement("td");
                    const value = education[header.key];

                    td.textContent = value || "";
                    row.appendChild(td);
                });

                // Add Delete button with Font Awesome icon
                const actionTd = document.createElement("td");
                const deleteBtn = document.createElement("button");
                deleteBtn.className = "btn btn-danger btn-sm btn-icon btn-delete-education-details";
                deleteBtn.setAttribute("data-id", education.Id);
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


$(document).on("click", ".btn-delete-education-details", function () {

    var $self = $(this);
    var educationId = $self.data("id");


    Ajax.post(sitePath + "api/EmployeeAPI/DeleteEducationDetailById?Id=" + educationId, null, function (resp) {
        if (resp.StatusCode == 200) {

            Swal.fire({
                text: "Education detail deleted successfully.",
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
