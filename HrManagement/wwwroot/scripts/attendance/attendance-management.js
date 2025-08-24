HRMSUtil.onDOMContentLoaded(function () {
    $("#date-from").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date().fp_incr(-30),
    });

    $("#date-to").flatpickr({
        dateFormat: "d/m/Y",
        defaultDate: new Date(),
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
            ddEmployees.appendChild(option);
        }
        else {
            var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
            ddEmployees.appendChild(option);
        }
    });

    if (cb) {
        cb();
    }

}

var RenderData = function () {
    $("#tbl_manageattendance").DataTable({
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
            { title: 'Person Name', data: null, className: 'all nowrap' },
            { title: 'Person ID', data: 'EmployeeCode', className: 'nowrap' },
            { title: 'Date', data: 'AttendanceDate', className: 'all nowrap' },
            { title: 'Timesheet', data: 'TimeSheet', className: 'all nowrap' },
            { title: 'Clock In', data: 'ClockIn', className: 'all nowrap' },
            { title: 'Clock Out', data: 'ClockOut', className: 'nowrap' },
            { title: 'Clock Time(h)', data: 'ClockTime', className: 'nowrap' },
            { title: 'Total Break Time(h)', data: 'TotalBreakTime', className: 'nowrap' },
            { title: 'Total Work Time(h)', data: 'TotalWorkTime', className: 'nowrap' },
            { title: 'Total Overtime Time(h)', data: 'TotalOverTime', className: 'nowrap' },
            { title: 'Total Time(h)', data: 'TotalTime', className: 'nowrap' },
            { title: 'Static Rule Mode', data: 'StaticRuleMode', className: 'nowrap' },
            { title: 'Abnormal Situation', data: 'AbnormalSituation', className: 'nowrap' },
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                "targets": 1,
                //className: 'text-center',
                "render": function (data, type, row) {
                    var fullName = row["FirstName"] + " " + row["LastName"] || null;

                    return fullName;

                },
            },
            {
                "targets": 3,
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;

                    return FormatDate(row["AttendanceDate"]);

                },
            }
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

        "sAjaxSource": sitePath + "api/AttendanceAPI/GetAttendance", //Manage report template search
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {
            //var element = document.querySelector("#tbl_attendance")
            //var blockUI = new KTBlockUI(element, {
            //    animate: true,
            //    overlayClass: "bg-body",
            //});

            //blockUI.block();
            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getAttendanceFilters()
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


function getAttendanceFilters() {
    var filter = {};
    filter.EmployeeCode = ($("#employee").val() != null && $("#employee").val() != 0 && $("#employee").val() != '') ? parseInt($("#employee").val()) : null;
    filter.DateFrom = $("#date-from").val();
    filter.DateTo = $("#date-to").val();

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
    var filters = getAttendanceFilters();
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

    var element = document.querySelector("#tbl_manageattendance")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_manageattendance")
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

    var element = document.querySelector("#tbl_manageattendance")
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
