HRMSUtil.onDOMContentLoaded(function () {
    InitializeDatePickers.init();
    RenderData();
    hideSpinner();
});

var InitializeDatePickers = function () {
    return {
        init: function () {

            //    $("#PublishDate").flatpickr({
            //        dateFormat: "d/m/Y",
            //    });

            //    $("#CompletionDate").flatpickr({
            //        dateFormat: "d/m/Y",
            //    });
        }
    }
}();

var RenderData = function () {
    $("#tbl_Mysurveys").DataTable({
        searching: false,
        ordering: false,
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
            { title: 'Survey  Name', data: 'Name', className: 'all nowrap' },
            { title: 'Site', data: 'Site', className: 'all nowrap' },
            { title: 'Department', data: 'Department', className: 'all nowrap' },
            { title: 'Status', data: 'Status', className: 'all nowrap' },
            { title: 'Is Recurring', data: 'IsRecurring', className: 'all nowrap' },
            { title: 'Recursion', data: 'Recursion', className: 'all nowrap' },
            //{ title: 'Start Date', data: 'PublishDate', className: 'all nowrap' },
            //{ title: 'End Date', data: 'CompletionDate', className: 'all nowrap' },
            { title: 'Created By', data: 'CreatedBy', className: 'all nowrap' },
            { title: 'Created Date', data: 'CreatedDate', className: 'all nowrap' },
            { title: 'Recurring Date', data: 'RecurringDate', className: 'all nowrap' },
            { title: 'Is Submitted', data: 'IsSubmitted', className: 'all nowrap' },
            { title: 'Last Date', data: null, className: 'all nowrap' },
            { title: 'DueDate Passed', data: null, className: 'all nowrap' },
            { title: 'Action', data: null, className: 'all nowrap' },
        ],
        columnDefs: [
            {
                className: "dtr-control",
                orderable: false,
                targets: 0
            },
            {
                targets: -1,
                orderable: false,
                render: function (data, type, row) {
                    var btns = '';
                    var date = FormatDate(row["RecurringDate"]);
                    var lastdate = moment(date, 'DD/MM/YYYY').add(5, 'days').format('DD/MM/YYYY');
                    var currentDate = FormatDate(new Date());

                    if (row["IsSubmitted"] == false) {
                        if (moment(date, 'DD/MM/YYYY') <= moment(currentDate, 'DD/MM/YYYY') && moment(currentDate, 'DD/MM/YYYY') <= moment(lastdate, 'DD/MM/YYYY')) {
                            btns += '<a type="button" '
                                + 'href="' + sitePath + 'Survey/EmployeeSurvey?SurveyId=' + row["Id"] + '&RecurringDate=' + date + '" '
                                + 'class="btn btn-sm btn-primary btn-icon me-1" '
                                + 'title="Edit Survey"> <i class="fa fa-pencil-square"></i> </a >';
                        }

                    }
                    else {
                        btns += '<a type="button" '
                            + 'href="' + sitePath + 'Survey/EmployeeSurvey?SurveyId=' + row["Id"] + '&RecurringDate=' + date + '&IsSubmitted=true" '
                            + 'class="btn btn-sm btn-success btn-icon me-1" '
                            + 'title="View Survey"> <i class="fa fa-eye"></i> </a >';
                    }

                    return btns;

                }
            },
            {
                targets: 5,
                orderable: false,
                render: function (data, type, row) {
                    return data == true ? "Yes" : "No";
                }
            },
            //{
            //    targets: 7,
            //    orderable: false,
            //    render: function (data, type, row) {
            //        return FormatDate(data);
            //    }
            //},
            //{
            //    targets: 8,
            //    orderable: false,
            //    render: function (data, type, row) {
            //        return FormatDate(data);
            //    }
            //},
            {
                targets: 8,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(data);
                }
            },
            {
                targets: 9,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(data);
                }
            },
            {
                targets: 10,
                orderable: false,
                render: function (data, type, row) {
                    return data == true ? "<span class='text-green fw-bolder'>Yes</span>" : "<span class='text-red fw-bolder'>No</span>";
                }
            },
            {
                targets: 11,
                orderable: false,
                render: function (data, type, row) {
                    var date = FormatDate(row["RecurringDate"]);
                    var lastdate = moment(date, 'DD/MM/YYYY').add(7, 'days').format('DD/MM/YYYY');
                    return lastdate;

                }
            },
            {
                targets: 12,
                orderable: false,
                render: function (data, type, row) {
                    var date = FormatDate(row["RecurringDate"]);
                    var lastdate = moment(date, 'DD/MM/YYYY').add(5, 'days').format('DD/MM/YYYY');
                    var currentDate = FormatDate(new Date());
                    if (moment(date, 'DD/MM/YYYY') <= moment(currentDate, 'DD/MM/YYYY') && moment(currentDate, 'DD/MM/YYYY') <= moment(lastdate, 'DD/MM/YYYY')) {
                        return "<span class='text-green fw-bolder'>No</span>";
                    }
                    else {
                        return "<span class='text-red fw-bolder'>Yes</span>";
                    }
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

        "sAjaxSource": sitePath + "api/SurveyAPI/GetEmployeeSurveys",
        "fnServerParams": function (aoData) {
        },
        "fnPreDrawCallback": function () {

            return true;
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            blockTable();
            var queryData = getSurveysFilters()
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

function getSurveysFilters() {
    var filter = {};
    filter.Name = $("#Name").val() || null;
    filter.Status = $("#Status").val() || null;
    //filter.PublishDate = $("#PublishDate").val() || null;
    //filter.CompletionDate = $("#CompletionDate").val() || null;
    filter.PublishDate = null;
    filter.CompletionDate = null;

    return filter;
}

$('#btnSearch').click(function () {
    filterSurveys();
});
$('#btnClear').click(function () {
    cancelSearch();
});

function filterSurveys() {
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

    var element = document.querySelector("#tbl_Mysurveys")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_Mysurveys")
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



    var element = document.querySelector("#tbl_Mysurveys")
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
