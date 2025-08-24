HRMSUtil.onDOMContentLoaded(function () {

    $("#startdate").flatpickr({
        dateFormat: "Y-m-d",
        onChange: function (selectedDates, dateStr, instance) {
            $("#enddate").flatpickr({
                minDate: selectedDates[0]
            });
        },
        defaultDate: new Date().fp_incr(-60),
    });

    $("#enddate").flatpickr({
        dateFormat: "Y-m-d",
        defaultDate: new Date().fp_incr(60),
    });

    unspin();
});

var RenderData = function () {
    $("#tbl_manageschedule").DataTable({
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
            //{ title: 'Employee Code', data: 'EmployeeCode', className: 'all nowrap' },
            { title: 'Employee Name', data: "EmployeeName", className: 'all nowrap' },
            { title: 'Site', data: "SiteName", className: 'all nowrap' },
            { title: 'Start Date', data: "StartDate", className: 'all nowrap' },
            { title: 'End Date', data: 'EndDate', className: 'all nowrap' },
            { title: 'Start Time', data: 'StartTime', className: 'all nowrap' },
            { title: 'End Time', data: 'EndTime', className: 'all nowrap' },
            { title: 'On Days', data: 'EndTime', className: 'all textwrap' },
            { title: 'Off Days', data: 'EndTime', className: 'all textwrap' },
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
                "targets": 3,
                "render": function (data, type, row) {
                    return FormatDate(row["StartDate"]);
                },
            },
            {
                "targets": 4,
                "render": function (data, type, row) {
                    return FormatDate(row["EndDate"]);
                },
            },
            {
                "targets": 7,
                "render": function (data, type, row) {
                    const daysMap = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

                    const selectedDays = JSON.parse(row["OnDays"]); // Example input array

                    // Convert numbers to day names using a regular function
                    const weekdays = selectedDays.map(function (day) {
                        return daysMap[day - 1];
                    });

                    // Convert the array to a comma-separated string
                    const weekdaysString = weekdays.join(", ");

                    return weekdaysString;
                },
            },
            {
                "targets": 8,
                "render": function (data, type, row) {
                    const daysMap = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

                    const selectedDays = JSON.parse(row["OffDays"]); // Example input array

                    // Convert numbers to day names using a regular function
                    const weekdays = selectedDays.map(function (day) {
                        return daysMap[day - 1];
                    });

                    // Convert the array to a comma-separated string
                    const weekdaysString = weekdays.join(", ");

                    return weekdaysString;
                },
            },
            {
                targets: 9,
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
                        + 'data-id = "' + row["Id"] + '"'
                        + 'class="btn btn-sm btn-primary btn-icon me-1 mt-1 btnedit" '
                        + 'title="Edit Schedule"> <i class="fa fa-pencil-square"></i> </span>';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {
                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 mt-1 btndelete" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'title="Delete Schedule"><i class="fa fa-trash"></i></span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 mt-1 btnApprovedelete" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'title="Approve Schedule Deletion"><i class="fa fa-trash"></i></span>';
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

        "sAjaxSource": sitePath + "api/ScheduleManagementAPI/GetManageScheduleData", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {
            //var element = document.querySelector("#tbl_schedule")
            //var blockUI = new KTBlockUI(element, {
            //    animate: true,
            //    overlayClass: "bg-body",
            //});

            //blockUI.block();
            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getScheduleFilters()
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


function getScheduleFilters() {
    var filter = {};
    filter.EmployeeCode = ($("#employeefilter").val() != '' && $("#employeefilter").val() != null && $("#employeefilter").val() != 0) ? [parseInt($("#employeefilter").val())] : null;
    filter.StartDate = $("#startdate").val();
    filter.EndDate = $("#enddate").val();



    if (filter.EmployeeCode == null || filter.EmployeeCode.length == 0) {
        var select = document.getElementById("employeefilter");
        filter.EmployeeCode = [];
        for (var i = 0; i < select.options.length; i++) {
            if (select.options[i].value != null && select.options[i].value != '') {
                filter.EmployeeCode.push(parseInt(select.options[i].value));

            }
        }
    }

    return filter;
}

$(document).on("click", ".btndelete", function () {

    var $self = $(this);
    var Id = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to delete this Schedule?",
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

            Ajax.post(sitePath + "api/ScheduleManagementAPI/DeleteScheduleById?Id=" + Id, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Schedule deletion requested successfully.",
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
                            window.location.href = "/ScheduleManagement/Index";
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

$(document).on("click", ".btnApprovedelete", function () {

    var $self = $(this);
    var Id = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to approve deletion of this Schedule?",
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

            Ajax.post(sitePath + "api/ScheduleManagementAPI/ApproveDeleteScheduleById?Id=" + Id, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Schedule deletion approved successfully.",
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
                            window.location.href = "/ScheduleManagement/Index";
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
    filterEmployees();
});
$('#btnClear').click(function () {
    cancelSearch();
});


function filterEmployees() {
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

    var element = document.querySelector("#tbl_manageschedule")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_manageschedule")
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

    var element = document.querySelector("#tbl_manageschedule")
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
