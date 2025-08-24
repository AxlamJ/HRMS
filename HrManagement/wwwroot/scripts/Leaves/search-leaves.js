HRMSUtil.onDOMContentLoaded(function () {
    $("#date-from").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date().fp_incr(-365),
    });

    $("#date-to").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date().fp_incr(60),
    });
    PopulateEmployees(function () {
        RenderData();
    });

    //if (hasRole(4) && globalRoles.length == 1) {
    //    $('#employee').attr("disabled", "disabled")
    //}

    hideSpinner()
});


function PopulateEmployees(cb) {

    var ddEmployees = document.querySelector('#employee');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        if (item.EmployeeCode == EmployeeCode) {

            var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, true, true);
            $(option).data('id', item.Id);
            ddEmployees.appendChild(option);
        }
        else {
            var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
            $(option).data('id', item.Id);
            ddEmployees.appendChild(option);
        }
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

                    return FormatDateTime(row["ApprovedDate"]);

                },
            },
            //,
            //{
            //    "targets": 5,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var pair = row["paired"] || null;

            //        if (pair.length > 0) {
            //            return pair[0].clock_in0
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 6,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var pair = row["paired"] || null;

            //        if (pair.length > 0) {
            //            return pair[0].clock_out0
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 7,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var pair = row["paired"] || null;

            //        if (pair.length > 0) {
            //            return pair[0].period_hrs0
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 8,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var payload = row["payload"] || null;

            //        if (payload != null && payload != {} && payload.hasOwnProperty('paid_break')) {
            //            return payload.paid_break
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 9,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var payload = row["payload"] || null;

            //        if (payload != null && payload != {} && payload.hasOwnProperty('paid_work_deduct_break')) {
            //            return payload.paid_work_deduct_break || ""
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 10,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var payload = row["payload"] || null;

            //        if (payload != null && payload != {} && payload.hasOwnProperty('overtime_hrs')) {
            //            return payload.overtime_hrs || ""
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //},
            //{
            //    "targets": 11,
            //    className: 'text-center',
            //    "render": function (data, type, row) {
            //        var payload = row["payload"] || null;

            //        if (payload != null && payload != {} && payload.hasOwnProperty('paid_work')) {
            //            return payload.paid_work || ""
            //        }
            //        else {
            //            return "";
            //        }
            //    },
            //}
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
