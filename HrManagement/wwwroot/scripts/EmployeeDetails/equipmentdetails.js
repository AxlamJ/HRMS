var EquipmnentsList = null;


$(document).on("click", ".btn-add-equipment", function () {
    $('#modal-equipment-details').modal('show');
});


// These are the constraints used to validate the form
var equipmentdetailsconstraints = {
    "item-requested": {
        // Site Country is required
        presence: {
            message: "is required."
        }
    },
    "request-note": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    }

};


$("#btn-add-equipment-details").on("click", function () {
    var form = document.querySelector("#equipment-details-form");
    var errors = validate(form, equipmentdetailsconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var equipment = {};

        equipment.EmployeeCode = parseInt(_EmployeeData.EmployeeCode);
        equipment.equipmentName = $(form).find("#item-requested").val();
        equipment.notes = $(form).find("#request-note").val();
        equipment.status = 1;

        if (Window.JustConsole) { console.log(equipment); return; }
        var url = sitePath + 'api/EmployeeAPI/UpsertEquipments';

        Ajax.post(url, equipment, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Equipment request submitted successfully.",
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



$(document).on("click", "#nav-equipment-tab", function () {

    var url = sitePath + "api/EmployeeAPI/GetEquipmentByEmpCode?EmployeeCode=" + parseInt( _EmployeeData.EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            EquipmnentsList = resp.Equipments;
            console.log(resp.ConatctsList);

            var ApprovedEquipments = EquipmnentsList.filter(function (item) { if (item.status == 1) { return item } });
            var PendingRejectedEquipments = EquipmnentsList.filter(function (item) { if (item.status == 2 || item.status == 3) { return item } });

            const tableContainer = document.getElementById("RequestedContainer");
            tableContainer.innerHTML = ""; // Clear any existing table

            const tableContainerApproved = document.getElementById("ApprovedContainer");
            tableContainerApproved.innerHTML = ""; // Clear any existing table

            if (PendingRejectedEquipments.length === 0) {
                tableContainer.innerHTML = "<div class='mt-3 text-center mb-0 font-18 text-primary'><i class='fa fa-info-circle mr-1'></i> We couldn't find any records.</div>";
            }

            if (ApprovedEquipments.length === 0) {
                tableContainerApproved.innerHTML = "<div class='mt-3 text-center mb-0 font-18 text-primary'><i class='fa fa-info-circle mr-1'></i> We couldn't find any records.</div>";
            }

            if (PendingRejectedEquipments.length > 0) {
                // Create table element
                const table = document.createElement("table");
                table.className = "table table-bordered table-responsive table-striped";
                // Create table head
                const thead = document.createElement("thead");
                const headerRow = document.createElement("tr");

                const headers = [
                    { label: "Item", key: "equipmentName" },
                    { label: "Notes", key: "notes" },
                    { label: "Requested Date", key: "createdDate" },
                    { label: "Status", key: "status" },
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

                PendingRejectedEquipments.forEach(function (equipment, index) {
                    const row = document.createElement("tr");

                    headers.forEach(function (header) {
                        const td = document.createElement("td");
                        var value = equipment[header.key];
                        if (header.key == "createdDate") {
                            value = FormatDateTime(value);
                            td.textContent = value || "";
                        }
                        else if (header.key == "status") {
                            if (value == 1) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-light-success">Approved</span>')
                            }
                            else if (value == 2) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-secondary">Pending</span>')
                            }
                            else if (value == 3) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-light-danger">Rejected</span>')
                            }

                        }
                        else {
                            td.textContent = value || "";
                        }
                        row.appendChild(td);
                    });

                    var statusId = equipment["status"];

                    // Add Delete button with Font Awesome icon
                    const actionTd = document.createElement("td");

                    if (statusId == 2) {
                        const deleteBtn = document.createElement("button");
                        deleteBtn.className = "btn btn-danger btn-sm btn-icon btn-delete-equipment";
                        deleteBtn.setAttribute("data-id", equipment.id);
                        deleteBtn.innerHTML = '<i class="fa fa-trash"></i>';

                        actionTd.appendChild(deleteBtn);
                    }
                    row.appendChild(actionTd);


                    tbody.appendChild(row);

                });

                table.appendChild(tbody);
                tableContainer.appendChild(table);

            }

            if (ApprovedEquipments.length > 0) {
                // Create table element
                const table = document.createElement("table");
                table.className = "table table-bordered table-responsive table-striped";
                // Create table head
                const thead = document.createElement("thead");
                const headerRow = document.createElement("tr");

                const headers = [
                    { label: "Item", key: "equipmentName" },
                    { label: "Notes", key: "notes" },
                    { label: "Requested Date", key: "createdDate" },
                    { label: "Status", key: "status" },
                ];
                // Dynamically generate headers based on keys
                headers.forEach(function (header) {
                    const th = document.createElement("th");
                    th.textContent = header.label;
                    headerRow.appendChild(th);
                });

                thead.appendChild(headerRow);
                table.appendChild(thead);

                // Create table body
                const tbody = document.createElement("tbody");

                ApprovedEquipments.forEach(function (equipment, index) {
                    const row = document.createElement("tr");

                    headers.forEach(function (header) {
                        const td = document.createElement("td");
                        var value = equipment[header.key];
                        if (header.key == "createdDate") {
                            value = FormatDateTime(value);
                            td.textContent = value || "";
                        }
                        else if (header.key == "status") {
                            if (value == 1) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-light-success">Approved</span>')
                            }
                            else if (value == 2) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-secondary">Pending</span>')
                            }
                            else if (value == 3) {
                                $(td).append('<span class="badge py-3 px-4 fs-7 badge-light-danger">Rejected</span>')
                            }

                        }
                        else {
                            td.textContent = value || "";
                        }
                        row.appendChild(td);
                    });

                    tbody.appendChild(row);
                });

                table.appendChild(tbody);
                tableContainerApproved.appendChild(table);

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


$(document).on("click", ".btn-delete-equipment", function () {

    var $self = $(this);
    var equipmentId = $self.data("id");


    Ajax.post(sitePath + "api/EmployeeAPI/DeleteEquipmentById?Id=" + equipmentId, null, function (resp) {
        if (resp.StatusCode == 200) {

            Swal.fire({
                text: "Equipment deleted successfully.",
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
