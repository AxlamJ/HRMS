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

                    btns += '<span type="button" '
                        + 'data-id="' + row["RoleId"] + '" '
                        + 'class="btn btn-sm btn-primary btn-icon me-1 mt-1 btn-edit-role" '
                        + 'title="Edit Role"> <i class="fa fa-pencil-square"></i> </span>';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {
                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 btndelete mt-1" '
                            + 'data-id = "' + row["RoleId"] + '"'
                            + 'title="Delete Role"><i class="fa fa-trash"></i></span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 btnapprovedelete" '
                            + 'data-id = "' + row["RoleId"] + '"'
                            + 'title="Approve Role Deletion"><i class="fa fa-trash"></i></span>';
                    }

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
    $("#tbl_users").DataTable({
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
            {
                "class": "centerCheckbox",
                "orderable": false,
                "data": null,
                "defaultContent": ""
            },
            { title: 'Name', data: null, className: 'all nowrap' },
            { title: 'Email', data: 'Email', className: 'all nowrap' },
            { title: 'Roles', data: 'RoleName', className: 'all nowrap' },
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                targets: [1],
                className: "centerCheckbox",
                orderable: false,
                "render": function (data, type, row) {
                    var checkBox = '<input  id="SingleJobCheckbox_' + row["UserId"] + '" class="form-check-input selectMultipleJobsCheckBox icheck checkboxclass dynamicCheckbox" type="checkbox">';
                    return checkBox;
                },

            },
            {
                targets: 2,
                orderable: false,
                "render": function (data, type, row) {
                    return row["FirstName"] + " " + row["LastName"];
                },

            },
            {
                targets: 3,
                orderable: false,
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

        "sAjaxSource": sitePath + "api/PermissionsAPI/GetAllUsers", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockUsersTable();
            var queryData = {};
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
            unblockUsersTable();
            hideOverlayUsersTable();

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
