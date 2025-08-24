HRMSUtil.onDOMContentLoaded(function () {
    $("#date-from").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date().fp_incr(-60),
    });

    $("#date-to").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date().fp_incr(60),
    });
    PopulateEmployees(function () {
        RenderData();
    });

    hideSpinner()
});


function PopulateEmployees(cb) {

    var ddEmployees = document.querySelector('#employee');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        $(option).data('id', item.Id);
        ddEmployees.appendChild(option);

    });

    if (cb) {
        cb();
    }

}

var RenderData = function () {
    $("#tbl_searchleaves").DataTable({
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
            { title: 'Employee Name', data: "EmployeeName", className: 'all nowrap' },
            { title: 'Emplooye Code', data: 'EmployeeCode', className: 'nowrap' },
            { title: 'Date From', data: 'StartDate', className: 'all nowrap' },
            { title: 'Date To', data: 'EndDate', className: 'all nowrap' },
            { title: 'Total Days', data: 'TotalDays', className: 'nowrap' },
            { title: 'Leave Type', data: 'LeaveTypeName', className: 'all nowrap' },
            { title: 'Leave Status', data: 'LeaveStatusName', className: 'nowrap' },
            { title: 'Approved/Rejected By', data: 'ApprovedBy', className: 'nowrap' },
            { title: 'Approved/Rejected Date', data: 'ApprovedDate', className: 'nowrap' },
            { title: 'Comments', data: 'Comments', className: 'nowrap' },
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
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;

                    return FormatDate(row["StartDate"]);

                },
            },
            {
                "targets": 4,
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;

                    return FormatDate(row["EndDate"]);

                },
            },
            {
                "targets": 7,
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;
                    var LeaveStatus = row["LeaveStatusId"];
                    if (LeaveStatus == 1) {
                        return '<span class="badge py-3 px-4 fs-7 badge-light-warning">' + row["LeaveStatusName"] + '</span>'
                    }
                    else if (LeaveStatus == 2) {
                        return '<span class="badge py-3 px-4 fs-7 badge-light-success">' + row["LeaveStatusName"] + '</span>'
                    }
                    else if (LeaveStatus == 3) {
                        return '<span class="badge py-3 px-4 fs-7 badge-light-danger">' + row["LeaveStatusName"] + '</span>'
                    }
                },
            },
            {
                "targets": 9,
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;

                    return FormatDate(row["ApprovedDate"]);

                },
            },
            {
                targets: -1,
                orderable: false,
                render: function (data, type, row) {
                    var btns = '';
                    var LeaveStatus = row["LeaveStatusId"];
                    if (LeaveStatus == 1) {

                        btns += '<span type="button" '
                            + 'class="btn btn-sm btn-success btn-icon me-1 mt-1 btn-approveleave" onclick="ApproveLeave(this)" '
                            + 'data-id="' + row["LeaveId"] + '"'
                            + 'data-employeecode="' + row["EmployeeCode"] + '"'
                            + 'data-employeename="' + row["EmployeeName"] + '"'
                            + 'data-leavetype="' + row["LeaveTypeName"] + '"'
                            + 'data-totaldays="' + row["TotalDays"] + '"'
                            + 'data-startdate="' + row["StartDate"] + '"'
                            + 'data-enddate="' + row["EndDate"] + '"'
                            + 'title="Approve Leave"> <i class="fa fa-square-check"></i> </span>';

                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 mt-1 btn-rejectleave" onclick="RejectLeave(this)" '
                            + 'data-id="' + row["LeaveId"] + '"'
                            + 'data-employeecode="' + row["EmployeeCode"] + '"'
                            + 'data-employeename="' + row["EmployeeName"] + '"'
                            + 'data-leavetype="' + row["LeaveTypeName"] + '"'
                            + 'data-totaldays="' + row["TotalDays"] + '"'
                            + 'data-startdate="' + row["StartDate"] + '"'
                            + 'data-enddate="' + row["EndDate"] + '"'
                            + 'title="Reject Leave"><i class="fa fa-square-xmark"></i></span>';

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

        "sAjaxSource": sitePath + "api/LeavesAPI/SearchLeaves", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {
            //var element = document.querySelector("#tbl_leaves")
            //var blockUI = new KTBlockUI(element, {
            //    animate: true,
            //    overlayClass: "bg-body",
            //});

            //blockUI.block();
            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getSearchLeavesFilters()
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

function getSearchLeavesFilters() {
    var filter = {};
    filter.EmployeeCode = ($("#employee").val() != '' && $("#employee").val() != null && $("#employee").val() != 0) ? [parseInt($("#employee").val())] : null;
    filter.StartDate = $("#date-from").val();
    filter.EndDate = $("#date-to").val();
    filter.leaveTypeId = $("#leavetype").val() != '' ? parseInt($("#leavetype").val()) : null;
    filter.leaveStatusId = $("#leavesatatus").val() != '' ? parseInt($("#leavesatatus").val()) : null;

    if (filter.EmployeeCode == null || filter.EmployeeCode.length == 0) {

        var select = document.getElementById("employee");
        filter.EmployeeCode = [];
        for (var i = 0; i < select.options.length; i++) {
            if (select.options[i].value != null && select.options[i].value != '') {
                filter.EmployeeCode.push(parseInt(select.options[i].value));

            }
        }

    }

    return filter;
}



$('#btnSearch').click(function () {
    filterAttendance();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterAttendance() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderData();
    hideSpinner();
}

$('#btn-Export').click(function () {
    showSpinner();
    var url = sitePath + "api/AttendanceAPI/ExportAttendance";
    var filters = getSearchLeavesFilters();
    Ajax.post(url, filters, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            hideSpinner();
            window.open(resp.DownloadUrl, '_blank');

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

// These are the constraints used to validate the form
var approveleaveconstraints = {
    "approve-comments": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    }
};

// These are the constraints used to validate the form
var rejectleaveconstraints = {
    "reject-comments": {
        // Site Name is required
        presence: {
            message: "is required."
        }
    }
};
function ApproveLeave(event) {

    var $self = $(event);
    $('#approve_leave_modal').modal('show');
    var leaveid = $self.data('id')
    var employeecode = $self.data('employeecode')
    var employeename = $self.data('employeename')
    var leavetype = $self.data('leavetype')
    var totaldays = $self.data('totaldays')
    var startdate = $self.data('startdate')
    var enddate = $self.data('enddate')

    $("#txtleaveid").val(leaveid);
    $("#txtemployeecode").val(employeecode);
    $("#txtemployeename").val(employeename);
    $("#txtleavetype").val(leavetype);
    $("#txtdatefrom").val(FormatDate(startdate));
    $("#txtdateto").val(FormatDate(enddate));
    $("#txttotaldays").val(totaldays);

}
function RejectLeave(event) {

    var $self = $(event);
    $('#reject_leave_modal').modal('show');
    var leaveid = $self.data('id')
    var employeecode = $self.data('employeecode')
    var employeename = $self.data('employeename')
    var leavetype = $self.data('leavetype')
    var totaldays = $self.data('totaldays')
    var startdate = $self.data('startdate')
    var enddate = $self.data('enddate')

    $("#txtrejectleaveid").val(leaveid);
    $("#txtrejectemployeecode").val(employeecode);
    $("#txtrejectemployeename").val(employeename);
    $("#txtrejectleavetype").val(leavetype);
    $("#txtrejectdatefrom").val(FormatDate(startdate));
    $("#txtrejectdateto").val(FormatDate(enddate));
    $("#txtrejecttotaldays").val(totaldays);

}


$(document).on("click", ".btn-approve", function () {

    var ApproveLeaves = {};
    ApproveLeaves.leaveId = $("#txtleaveid").val();
    ApproveLeaves.employeeCode = ($("#txtemployeecode").val() != '' && $("#txtemployeecode").val() != null && $("#txtemployeecode").val() != 0) ? parseInt($("#txtemployeecode").val()) : null;
    ApproveLeaves.leaveStatusId = 2;
    ApproveLeaves.leaveStatusName = "Approved";
    ApproveLeaves.comments = $("#approve-comments").val();
    var form = document.querySelector("#approve-leave-form");
    var errors = validate(form, approveleaveconstraints);
    showErrors(form, errors || {});

    if (!errors) {

        Swal.fire({
            text: "Are you sure you want to approve?",
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

                Ajax.post(sitePath + "api/LeavesAPI/AprroveRejectLeave", ApproveLeaves, function (resp) {
                    if (resp.StatusCode == 200) {

                        Swal.fire({
                            text: "Leave Approved successfully.",
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
                                window.location.href = "/Home/ApproveLeaves";
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


$(document).on("click", ".btn-reject", function () {

    var RejectLeaves = {};
    RejectLeaves.leaveId = $("#txtrejectleaveid").val();
    RejectLeaves.employeeCode = ($("#txtrejectemployeecode").val() != '' && $("#txtrejectemployeecode").val() != null && $("#txtrejectemployeecode").val() != 0) ? parseInt($("#txtrejectemployeecode").val()) : null;
    RejectLeaves.leaveStatusId = 3;
    RejectLeaves.leaveStatusName = "Rejected";
    RejectLeaves.comments = $("#reject-comments").val();
    var form = document.querySelector("#reject-leave-form");
    var errors = validate(form, rejectleaveconstraints);
    showErrors(form, errors || {});

    if (!errors) {

        Swal.fire({
            text: "Are you sure you want to reject?",
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

                Ajax.post(sitePath + "api/LeavesAPI/AprroveRejectLeave", RejectLeaves, function (resp) {
                    if (resp.StatusCode == 200) {

                        Swal.fire({
                            text: "Leave Rejected successfully.",
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
                                window.location.href = "/Home/ApproveLeaves";
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




function cancelSearch() {
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    //clearSearchState();
    window.location = nURL;
}
function hideOverlay() {

    var element = document.querySelector("#tbl_searchleaves")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_searchleaves")
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

    var element = document.querySelector("#tbl_searchleaves")
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
