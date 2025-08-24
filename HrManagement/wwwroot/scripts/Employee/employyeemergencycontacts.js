var ContactsList = null;


$(document).on("click", ".btn-emergency-contact-new", function () {
    $('#modal-emergency-contact-details').modal('show');
});



// These are the constraints used to validate the form
var emergencyconatcdetailsconstraints = {
    "first-name": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    },
    "last-name": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "relationship": {
        // Site Time Zone is required
        presence: { message: "is required." }
    },
    "mobile-phone": {
        // Site Time Zone is required
        presence: { message: "is required." }
    }

};



$("#btn-update-emergency-contact-details").on("click", function () {
    var form = document.querySelector("#emergency-contact-details-form");
    var errors = validate(form, emergencyconatcdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var contact = {};

        //employee.Id = EmployeeData.Id;
        contact.EmployeeCode = parseInt(EmployeeData.EmployeeCode);
        contact.firstName = $(form).find("#first-name").val();
        contact.lastName = $(form).find("#last-name").val();
        contact.relationShip = $(form).find("#relationship").val();
        contact.OfficePhone = $(form).find("#office-phone").val();
        contact.MobilePhone = $(form).find("#mobile-phone").val();

        if (Window.JustConsole) { console.log(contact); return; }
        var url = sitePath + 'api/EmployeeAPI/UpsertEmergencyContact';

        Ajax.post(url, contact, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Employee Emergency contact details saved successfully.",
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


$(document).on("click", "#nav-emergency-contact-tab", function () {

    var url = sitePath + "api/EmployeeAPI/GetEmployeeEmergencyConatcts?EmployeeCode=" + parseInt(EmployeeData.EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            ContactsList = resp.ContactsList;
            console.log(resp.ConatctsList);


            const tableContainer = document.getElementById("tableContainer");
            tableContainer.innerHTML = ""; // Clear any existing table

            if (ContactsList.length === 0) {
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
                { label: "First Name", key: "FirstName" },
                { label: "Last Name", key: "LastName" },
                { label: "RelationShip", key: "RelationShip" },
                { label: "Office Phone", key: "OfficePhone" },
                { label: "Mobile Phone", key: "MobilePhone" }
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

            ContactsList.forEach(function (employee, index) {
                const row = document.createElement("tr");

                headers.forEach(function (header) {
                    const td = document.createElement("td");
                    const value = employee[header.key];

                    td.textContent = value || "";
                    row.appendChild(td);
                });

                // Add Delete button with Font Awesome icon
                const actionTd = document.createElement("td");
                const deleteBtn = document.createElement("button");
                deleteBtn.className = "btn btn-danger btn-sm btn-icon btn-delete-emergency-conatct";
                deleteBtn.setAttribute("data-id", employee.Id);
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


$(document).on("click", ".btn-delete-emergency-conatct", function () {

    var $self = $(this);
    var contactId = $self.data("id");


    Ajax.post(sitePath + "api/EmployeeAPI/DeleteEmergencyConatctById?Id=" + contactId, null, function (resp) {
        if (resp.StatusCode == 200) {

            Swal.fire({
                text: "Contact deleted successfully.",
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
});
