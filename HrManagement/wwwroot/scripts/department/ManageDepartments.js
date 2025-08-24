HRMSUtil.onDOMContentLoaded(function () {
    hideSpinner();

    LoadDropDowns(function () {
        RenderDepartmentsData(function () {
            RenderDepartmentsSubCategoryData();
        });

    })
});

function LoadDropDowns(cb) {

    var ddDepartments = document.querySelector('#department');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });

    var ddFilterDepartments = document.querySelector('#department-id');
    $(ddFilterDepartments).empty();
    var lstFilterDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddFilterDepartments.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddDepartments.appendChild(option);
    lstFilterDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddFilterDepartments.appendChild(option);
    });


    if (cb) {
        cb();
    }
}

var RenderDepartmentsData = function (cb) {
    $("#tbl_managedepartments").DataTable({
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
            { title: 'Department Name', data: 'DepartmentName', className: 'all nowrap' },
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
                targets: 2,
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
                        + 'data-id="' + row["DepartmentId"] + '" '
                        + 'data-deptname="' + row["DepartmentName"] + '" '
                        + 'class="btn btn-sm btn-primary btn-icon me-1 btn-edit-department" '
                        + 'title="Edit Department"> <i class="fa fa-pencil-square"></i></span>';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {
                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 btndelete" '
                            + 'data-id = "' + row["DepartmentId"] + '"'
                            + 'title="Delete Department"><i class="fa fa-trash"></i></span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 btnApprovedeptdelete" '
                            + 'data-id = "' + row["DepartmentId"] + '"'
                            + 'title="Approve Department Deletion"><i class="fa fa-trash"></i></span>';
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

        "sAjaxSource": sitePath + "api/DepartmentAPI/GetDepartments", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getDepartmentsFilters()
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

    if (cb) {
        cb()
    }
}


function getDepartmentsFilters() {
    var filter = {};
    filter.departmentName = $("#filter-dept-name").val();

    return filter;
}

$(document).on("click", ".btndelete", function () {

    var $self = $(this);
    var siteId = $self.data("id");

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

            Ajax.post(sitePath + "api/DepartmentAPI/DeleteDepartmentById?DepartmentId=" + siteId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Department deletion requested successfully.",
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
                            window.location.href = "/Home/ManageDepartments";
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


$(document).on("click", ".btnApprovedeptdelete", function () {

    var $self = $(this);
    var siteId = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to approve deletion of this Department?",
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

            Ajax.post(sitePath + "api/DepartmentAPI/ApproveDeleteDepartmentById?DepartmentId=" + siteId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Department deleted successfully.",
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
                            window.location.href = "/Home/ManageDepartments";
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
    filterDepartments();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterDepartments() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderDepartmentsData();
    hideSpinner();
}

function cancelSearch() {
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    //clearSearchState();
    window.location = nURL;
}
function hideOverlay() {

    var element = document.querySelector("#tbl_managedepartments")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_managedepartments")
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



    var element = document.querySelector("#tbl_managedepartments")
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

var RenderDepartmentsSubCategoryData = function (cb) {
    $("#tbl_subcategory").DataTable({
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
            { title: 'Department Name', data: 'DepartmentName', className: 'all nowrap' },
            { title: 'Sub-Category Name', data: 'SubCategoryName', className: 'all nowrap' },
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
                targets: 3,
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
                        + 'data-id="' + row["Id"] + '" '
                        + 'data-categoryname="' + row["SubCategoryName"] + '" '
                        + 'data-deptid="' + row["DepartmentId"] + '" '
                        + 'data-deptname="' + row["DepartmentName"] + '" '
                        + 'class="btn btn-sm btn-primary btn-icon me-1 btn-edit-subcategory" '
                        + 'title="Edit Department"> <i class="fa fa-pencil-square"></i></span>';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {
                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 btndelete-category" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'data-deptid="' + row["DepartmentId"] + '" '
                            + 'title="Delete Sub-Category"><i class="fa fa-trash"></i></span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 btnapprovesubcatdelete" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'data-deptid="' + row["DepartmentId"] + '" '
                            + 'title="Approve Sub-Category Deletion"><i class="fa fa-trash"></i></span>';
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

        "sAjaxSource": sitePath + "api/DepartmentAPI/GetDepartmentsSubCategories", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockSubCategoryTable();
            var queryData = getSubCategoriesFilters()
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
            unblockSubCategoryTable();
            hideSubCategoryOverlay();

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

    if (cb) {
        cb()
    }
}


function getSubCategoriesFilters() {
    var filter = {};
    filter.departmentId = ($("#department-id").val() != null && $("#department-id").val() != "") ? parseInt($("#department-id").val()) : null;
    filter.subCategoryName = $("#subcategoryname").val();

    return filter;
}

$(document).on("click", ".btndelete-category", function () {

    var $self = $(this);
    var Id = $self.data("id");
    var DepartmentId = $self.data("deptid");

    Swal.fire({
        text: "Are you sure you want to delete this Department Sub-Category?",
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

            Ajax.post(sitePath + "api/DepartmentAPI/DeleteDepartmentSubCategoryById?Id=" + Id + "&DepartmentId=" + DepartmentId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Department Sub-Category deletion requested successfully.",
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
                            window.location.href = "/Home/ManageDepartments";
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


$(document).on("click", ".btnapprovesubcatdelete", function () {

    var $self = $(this);
    var Id = $self.data("id");
    var DepartmentId = $self.data("deptid");

    Swal.fire({
        text: "Are you sure you want to approve deletion of this Department Sub-Category?",
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

            Ajax.post(sitePath + "api/DepartmentAPI/ApproveDeleteDepartmentSubCategoryById?Id=" + Id + "&DepartmentId=" + DepartmentId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Department Sub-Category deleted successfully.",
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
                            window.location.href = "/Home/ManageDepartments";
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


$('#btnCategoriesSearch').click(function () {
    filterSubCategories();
});
$('#btnClearCategories').click(function () {
    cancelSearch();
});

function filterSubCategories() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderDepartmentsSubCategoryData();
    hideSpinner();
}



function hideSubCategoryOverlay() {

    var element = document.querySelector("#tbl_subcategory")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockSubCategoryTable() {

    var element = document.querySelector("#tbl_subcategory")
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

function unblockSubCategoryTable() {



    var element = document.querySelector("#tbl_subcategory")
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
