HRMSUtil.onDOMContentLoaded(function () {
    hideSpinner();
    //registerEventListners.init();
    //InitializeDatePickers.init();
    //PopulateDropDowns(function () {
    //    loadData(function () {
    //        LoadBio()
    //    });
    //});
    RenderRolesData();
    RenderUsersData();
    [].slice.call(document.querySelectorAll('.tabs')).forEach(function (el) {
        new CBPFWTabs(el);
    });
});


var RenderRolesData = function () {
    $("#tbl_manageroles").DataTable({
        searching: false,
        ordering: true,
        paging: true,
        responsive: true,
        processing: true,
        pagingType: 'simple_numbers',
        "language": {
            "info": "Showing _START_ to _END_ of _TOTAL_ entries",
            "infoEmpty": "Showing 0 to 0 of 0 entries",
            "lengthMenu": "_MENU_ records",
            infoEmpty: 'No records to display',
            zeroRecords: 'No records to display',

        },
        "lengthMenu": [[10, 25, 50, 100], [10, 25, 50, 100]],
        responsive: {
            details: {
                type: 'column',
                defaultContent: '',
                target: 0
            },
        },
        //"fixedHeader": {
        //    "header": true,
        //    "headerOffset": 5
        //},
        columns: [
            {
                "class": "control",
                "orderable": false,
                "data": null,
                "defaultContent": ""
            },
            { title: 'Role Title', data: 'RoleName', className: 'all nowrap' },
            { title: 'Description', data: 'Description', className: 'all nowrap' },
            { title: 'Created', data: 'CreatedDate', className: 'all nowrap' },
            { title: 'Is Delete Requested', data: 'IsActive', className: 'all nowrap' },
            { title: 'Action', data: null, className: 'all nowrap' },
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                targets: 1,
                orderable: false,

            },
            {
                targets: 2,
                orderable: false,

            },
            {
                targets: 3,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(row["CreatedDate"])
                }

            },
            {
                targets: 4,
                orderable: false,
                render: function (data, type, row) {
                    var isactive = row['IsActive'] == true ? "No" : "Yes";
                    return isactive
                }
            },
            {
                targets: -1,
                orderable: false,
                render: function (data, type, row) {
                    var btns = '';

                    btns +=
                        '<button class="btn btn-sm btn-primary" onclick="trainingPermissionModalRole(\'' + row["RoleName"].replace(/'/g, "\\'") + '\')">' +
                        'Assign Training</button>';
                    return btns;

                }
            }
        ],
        "iDisplayLength": 10,
        "bSortClasses": false,
        "bPaginate": true,
        "bAutoWidth": false,
        "autoWidth": false,
        "filter": false,
        "bDestroy": true,
        "bDeferRender": true,
        "bServerSide": true,

        "sAjaxSource": sitePath + "api/PermissionsAPI/GetRoles", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getRolesFilters()
            aoData.forEach(function (item) {
                queryData[item.name] = item.value;
            });
            queryData.SortCol = queryData["mDataProp_" + queryData.iSortCol_0];
            Ajax.post(sSource, queryData, function (data) {
                if (data != null && data != "") {
                    //var resp = JSON.parse(data);
                    fnCallback(data);
                } else {
                    fnCallback();
                }

            });
        },
        "fnDrawCallback": function () {
            unblockTable();
            hideOverlay();

        },
        //data: dataSet,
        "dom":
            "<'row mb-2'" +
            "<'col-sm-6 d-flex align-items-center justify-conten-start dt-toolbar'l>" +
            "<'col-sm-6 d-flex align-items-center justify-content-end dt-toolbar'f>" +
            ">" +

            "<'table-responsive'tr>" +

            "<'row'" +
            "<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start'i>" +
            "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>" +
            ">"

    });
}


$(document).on('click', '#nav-users-tab', function () {

    PopulateRoles(function () {
        RenderUsersData();
    });
});

var RenderUsersData = function () {
    var table = $("#tbl_users").DataTable({
        searching: false,
        ordering: true,
        paging: true,
        responsive: true,
        processing: true,
        pagingType: 'simple_numbers',
        language: {
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            infoEmpty: "Showing 0 to 0 of 0 entries",
            lengthMenu: "_MENU_ records",
            infoEmpty: 'No records to display',
            zeroRecords: 'No records to display',
        },
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        responsive: {
            details: {
                type: 'column',
                defaultContent: '',
                target: 0
            },
        },
        columns: [
            {
                class: "control",
                orderable: false,
                data: null,
                defaultContent: ""
            },
            { title: 'Name', data: null, className: 'all nowrap' },
            { title: 'Email', data: 'Email', className: 'all nowrap' },
            { title: 'Roles', data: 'RoleName', className: 'all nowrap' },
            { title: 'Actions', data: null, orderable: false, className: 'text-center' }  // New Actions column
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                targets: 1,
                orderable: false,
                render: function (data, type, row) {
                    return row["FirstName"] + " " + row["LastName"];
                },
            },
            {
                targets: 2,
                orderable: false,
            },
            {
                targets: 4,  // Actions column index
                render: function (data, type, row) {
                    return `<button class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#trainingPermissionModal" data-userid="${row.UserId}">Assign Training</button>`;
                }
            }
        ],
        iDisplayLength: 10,
        bSortClasses: false,
        bPaginate: true,
        bAutoWidth: false,
        autoWidth: false,
        filter: false,
        bDestroy: true,
        bDeferRender: true,
        bServerSide: true,
        sAjaxSource: sitePath + "api/PermissionsAPI/GetAllUsers",
        fnServerData: function (sSource, aoData, fnCallback) {
            blockUsersTable();
            var queryData = {};
            aoData.forEach(function (item) {
                queryData[item.name] = item.value;
            });
            queryData.SortCol = queryData["mDataProp_" + queryData.iSortCol_0];
            Ajax.post(sSource, queryData, function (data) {
                if (data) {
                    fnCallback(data);
                } else {
                    fnCallback();
                }
            });
        },
        fnDrawCallback: function () {
            unblockUsersTable();
            hideOverlayUsersTable();

            // Attach click handler to Assign Training buttons after table draw
            $(".assign-training-btn").off("click").on("click", function () {
                var userId = $(this).data("userid");
                // Your assign training logic here
                alert("Assign Training clicked for user ID: " + userId);
                // For example, open a modal or redirect:
                // openAssignTrainingModal(userId);
            });
        },
        dom:
            "<'row mb-2'" +
            "<'col-sm-6 d-flex align-items-center justify-content-start dt-toolbar'l>" +
            "<'col-sm-6 d-flex align-items-center justify-content-end dt-toolbar'>" +
            ">" +
            "<'table-responsive'tr>" +
            "<'row'" +
            "<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start'i>" +
            "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>" +
            ">",
       
    });
};

function getRolesFilters() {
    var filter = {};
    filter.roleNmae = $("#role-name").val();

    return filter;
}

$(document).on("click", ".btndelete", function () {

    var $self = $(this);
    var roleId = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to delete this Department?",
        icon: "question",
        buttonsStyling: false,
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        showCancelButton: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: "btn btn-secondary"
        }
    }).then(function (result) {
        if (result.isConfirmed) {

            Ajax.post(sitePath + "api/PermissionsAPI/DeleteRoleById?RoleId=" + roleId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Role deletion requested successfully.",
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
    });

});

$(document).on("click", ".btnapprovedelete", function () {

    var $self = $(this);
    var roleId = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to approve deletion request of this Role?",
        icon: "question",
        buttonsStyling: false,
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        showCancelButton: true,
        allowEscapeKey: false,
        allowOutsideClick: false,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: "btn btn-secondary"
        }
    }).then(function (result) {
        if (result.isConfirmed) {

            Ajax.post(sitePath + "api/PermissionsAPI/ApproveDeleteRoleById?RoleId=" + roleId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Role deletion request approved successfully.",
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
    });

});


function PopulateRoles(cb) {

    var ddRoles = document.querySelector('#role-ids');
    $(ddRoles).empty();
    var lstRoles = (DropDownsData.Roles || []);
    var option = new Option();
    ddRoles.appendChild(option);
    lstRoles.forEach(function (item) {

        var option = new Option(item.RoleName, item.RoleId, false, false);
        ddRoles.appendChild(option);
    });

    if (cb) {
        cb();
    }

}

$(document).on('click', '#assign-role-btn', function () {
    var checkboxes = document.getElementsByClassName('selectMultipleJobsCheckBox');
    var checkboxesChecked = [];

    var roleId = $("#role-ids").val() == "" ? null : $("#role-ids").val();
    // loop over them all
    for (var i = 0; i < checkboxes.length; i++) {
        // And stick the checked ones onto an array...
        if (checkboxes[i].checked) {
            checkboxesChecked.push(checkboxes[i].id);
        }
    }

    if (checkboxesChecked.length < 1 || roleId == null) {
        Swal.fire({
            text: "No User/Role selected, Please first select User/Role to change Role.",
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
    else {
        var checkedUserIds = [];
        for (var i = 0; i < checkboxesChecked.length; i++) {
            var cleaningId = checkboxesChecked[i];
            cleaningId = cleaningId.split("_");
            checkedUserIds.push({ UserId: parseInt(cleaningId[1]), RoleId: roleId });
        }
        console.log(checkedUserIds);

        if (Window.JustConsole) { console.log(checkedUserIds); return; }
        var url = sitePath + 'api/PermissionsAPI/UpsertUsersRole';

        Ajax.post(url, checkedUserIds, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Users Roles updated successfully.",
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
});

$('#btnSearch').click(function () {
    filterDepartments();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterDepartments() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderData();
    hideSpinner();
}

function cancelSearch() {
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    //clearSearchState();
    window.location = nURL;
}
function hideOverlay() {

    var element = document.querySelector("#tbl_manageroles")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_manageroles")
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



    var element = document.querySelector("#tbl_manageroles")
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

function hideOverlayUsersTable() {

    var element = document.querySelector("#tbl_users")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockUsersTable() {

    var element = document.querySelector("#tbl_users")
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

function unblockUsersTable() {

    var element = document.querySelector("#tbl_users")
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

function createTreeItem(title, id, itemType, childrenHtml = []) {
    const li = $('<li class="tree-item open"></li>');
    const label = $(`
                <div class="tree-label">
                    <input type="checkbox" id="item-${id}" data-id="${id}" data-type="${itemType}">
                    <label for="item-${id}" class="mb-0">${title}</label>
                </div>
            `);
    li.append(label);

    if (childrenHtml.length > 0) {
        const childrenContainer = $('<ul class="children"></ul>');
        childrenHtml.forEach(child => childrenContainer.append(child));
        li.append(childrenContainer);
    }

    return li;
}

// Function to build the entire tree from JSON
function buildTree(trainingData) {
    const tree = $('#trainingTree');
    tree.empty();

    const structureNodes = [];

    trainingData.structures.forEach(structure => {
        let categoryNodes = [];

        structure.categories.forEach(category => {
            let subCategoryNodes = [];

            (category.TrainingSubCategories || []).forEach(sub => {
                subCategoryNodes.push(
                    createTreeItem(sub.title, sub.id, 3) // Subcategory
                );
            });

            categoryNodes.push(
                createTreeItem(category.title, category.id, 3, subCategoryNodes) // Category
            );
        });

        structureNodes.push(
            createTreeItem(structure.title, structure.trainingStructureId, 2, categoryNodes) // Structure
        );
    });

    // Root training node
    const trainingNode = createTreeItem(
        trainingData.title,
        trainingData.trainingId,
        1,
        structureNodes
    );

    tree.append(trainingNode);
}

// Show the tree when the modal is opened
$('#trainingPermissionModal').on('shown.bs.modal', function (event) {
    const button = $(event.relatedTarget);
    const userId = button.data('userid');

    $('#UserId').val(userId);

    let Id = $('#ddTrainings option:not([value=""]):first').val();
    loadTrainingData(Id);
});



function trainingPermissionModalRole(Name) {
    const myModal = new bootstrap.Modal(document.getElementById('trainingPermissionModalRole'));
    myModal.show();
    $('#RoleName').val(Name);
    var url = sitePath + `api/TrainingPermissions/GetByRoleName?RoleName=${Name}`;
    Ajax.post(url, null, function (response) {
            console.log(response);
            if (response.StatusCode === 200) {

                var container = $('#scrollableContent');
                container.empty(); // clear previous content if any

                response.Data.forEach(function (item) {
                    var isChecked = item.IsAssigned ? 'checked' : '';
                    var permissionIdAttr = item.PermissionId ? 'data-permission-id="' + item.PermissionId + '"' : '';
                    var allowedRoleAttr = item.AllowedRole ? 'data-allowed-role="' + item.AllowedRole + '"' : '';

                    var checkboxHtml = `
      <div class="form-check">
        <input class="form-check-input" type="checkbox" name="selectedTrainings" value="${item.TrainingId}" id="training_${item.TrainingId}" ${permissionIdAttr} ${allowedRoleAttr} ${isChecked}>
        <label class="form-check-label" for="training_${item.TrainingId}">${item.Title}</label>
      </div>
    `;
                    container.append(checkboxHtml);
                });

               
            } else {
                showErrorModal("Error occurred. Please contact your system administrator.");
            }
        });
}




$('#ddTrainings').on('change', function () {
    var Id = $(this).val();
    var selectedText = $("#ddTrainings option:selected").text();
    loadTrainingData(Id);
});


function loadTrainingData(Id) {
    const url = sitePath + `api/Trainings/GetTrainingWithDetails?trainingId=${Id}`;
    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {

            buildTree(resp.Data)
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


function getSelectedPermissions(userId) {
    const selected = [];

    $('#trainingTree input[type="checkbox"]:checked').each(function () {
        selected.push({
            UserId: userId,
            ItemId: $(this).data('id'),
            ItemType: $(this).data('type'),
            IsAssigned: true
        });
    });

    return selected;
}

$('#savePermissions').on('click', function (e) {
    e.preventDefault();
    const userId = $('#UserId').val();
    const permissions = getSelectedPermissions(userId);

    var url = sitePath + 'api/TrainingPermissions/UpsertPermission';
    if (permissions && permissions.length > 0) {
        Ajax.post(url, permissions, function (response) {
            console.log(response);
            if (response.StatusCode === 200) {
                Swal.fire({
                    text: "Permission successfully added.",
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
                }).then(function () {

                });
            } else {
                showErrorModal("Error occurred. Please contact your system administrator.");
            }
        });
    }  
});


$('#submitTrainingSelection').on('click', function () {
    var allItems = [];
    const roleName = $('#RoleName').val();
    $('input[name="selectedTrainings"]').each(function () {
        var trainingId = $(this).val();
        var permissionId = $(this).data('permission-id') || 0;
        var isAssigned = $(this).is(':checked');
        allItems.push({
            ItemId: parseInt(trainingId),
            ItemType:'1',
            PermissionId: permissionId !== null ? parseInt(permissionId) : 0,
            IsAssigned: isAssigned,
            AllowedRole:roleName
        });
    });

    console.log(allItems);

    var url = sitePath + 'api/TrainingPermissions/UpsertPermission';
    Ajax.post(url, allItems, function (response) {
        console.log(response);
        if (response.StatusCode === 200) {
            Swal.fire({
                text: "Permission successfully added.",
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
            }).then(function () {

            });
        } else {
            showErrorModal("Error occurred. Please contact your system administrator.");
        }
    });
    console.log(allItems);
});
