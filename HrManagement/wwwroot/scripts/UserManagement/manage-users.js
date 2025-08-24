HRMSUtil.onDOMContentLoaded(function () {
    hideSpinner();

    //PopulateDropDowns(function () {

        RenderData();
    //});
});


function PopulateDropDowns(cb) {

    PopulateDepartments(function () {
        PopulateSites(function () {
            if (cb) {
                cb();
            }
        })
    });

}



function PopulateDepartments(cb) {
    var url = sitePath + "api/MasterDataAPI/GetAllDepartments";
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            var ddDepartments = document.querySelector('#department');
            $(ddDepartments).empty();
            var lstDepartments = (resp.Departments || []);
            var option = new Option();
            ddDepartments.appendChild(option);
            lstDepartments.forEach(function (item) {

                var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
                ddDepartments.appendChild(option);
            });

            if (cb) {
                cb();
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
}


function PopulateSites(cb) {
    var url = sitePath + "api/MasterDataAPI/GetAllSites";
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            var ddSites = document.querySelector('#site');
            $(ddSites).empty();
            var lstSites = (resp.Sites || []);
            var option = new Option();
            ddSites.appendChild(option);
            lstSites.forEach(function (item) {

                var option = new Option(item.SiteName, item.Id, false, false);
                ddSites.appendChild(option);
            });

            if (cb) {
                cb();
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
}



var RenderData = function () {
    $("#tbl_manageusers").DataTable({
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
            { title: 'Employee Code', data: 'EmployeeCode', className: 'all nowrap' },
            { title: 'User Name', data: 'UserName', className: 'all nowrap' },
            { title: 'Name', data: null, className: 'all nowrap' },
            { title: 'Email', data: 'Email', className: 'all nowrap' },
            { title: 'Phone Number', data: 'PhoneNumber', className: 'all nowrap' },
            { title: 'Action', data: null, className: 'all nowrap' },
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                "targets": 1,
                orderable: false
            },
            {
                "targets": 2,
                orderable: false
            },
            {
                "targets": 3,
                orderable: false,
                "render": function (data, type, row) {
                    return row["FirstName"] + " " + row["LastName"];
                },
            },
            {
                "targets": 4,
                orderable: false,
            },
            {
                "targets": 5,
                orderable: false,
            },
            {
                targets: -1,
                orderable: false,
                render: function (data, type, row) {
                    var btns = '';

                    btns += '<a type="button" '
                        + 'href="' + sitePath + 'UserManagement/AddUpdateUser?UserId=' + row["UserId"]+ '" '
                        + 'class="btn btn-sm btn-primary btn-icon me-1" '
                        + 'title="Edit User"> <i class="fa fa-pencil-square"></i> </a >';

                    btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 btndelete" '
                        + 'data-id = "' + row["UserId"] + '"'
                        + 'title="Delete User"><i class="fa fa-trash"></i></span>';


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

        "sAjaxSource": sitePath + "api/UserManagementAPI/GetUsers", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getEmployeesFilters()
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


function getEmployeesFilters() {
    var filter = {};
    filter.firstName = $("#firstname").val();
    filter.lastName = $("#lastname").val();
    //filter.departmentId = parseInt($("#department").val());
    //filter.siteId = parseInt($("#site").val());

    return filter;
}

$(document).on("click", ".btndelete", function () {

    var $self = $(this);
    var userId = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to delete this User?",
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

            Ajax.post(sitePath + "api/UserManagementAPI/DeleteUserById?Id=" + userId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "User deleted successfully.",
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
                            window.location.href = "/UserManagement/ManageUsers";
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


$('#btnSearch').click(function () {
    filterUsers();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterUsers() {
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

    var element = document.querySelector("#tbl_manageusers")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_manageusers")
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

    var element = document.querySelector("#tbl_manageusers")
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
