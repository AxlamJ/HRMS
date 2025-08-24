HRMSUtil.onDOMContentLoaded(function () {
    InitializeDatePickers.init();
    RenderData();
    hideSpinner();
});

var InitializeDatePickers = function () {
    return {
        init: function () {

            $("#PublishDate").flatpickr({
                dateFormat: "d/m/Y",
            });

            $("#CompletionDate").flatpickr({
                dateFormat: "d/m/Y",
            });
        }
    }
}();

var RenderData = function () {
    $("#tbl_managesurveys").DataTable({
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
            { title: 'Survey  Name', data: 'Name', className: 'all nowrap' },
            { title: 'Site', data: 'Site', className: 'all nowrap' },
            { title: 'Department', data: 'Department', className: 'all nowrap' },
            { title: 'Status', data: 'Status', className: 'all nowrap' },
            { title: 'Is Recurring', data: 'IsRecurring', className: 'all nowrap' },
            { title: 'Recursion', data: 'Recursion', className: 'all nowrap' },
            { title: 'Start Date', data: 'PublishDate', className: 'all nowrap' },
            { title: 'End Date', data: 'CompletionDate', className: 'all nowrap' },
            { title: 'Created By', data: 'CreatedBy', className: 'all nowrap' },
            { title: 'Created Date', data: 'CreatedDate', className: 'all nowrap' },
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

                    btns += '<a type="button" '
                        + 'href="' + sitePath + 'Survey/CreateSurvey?SurveyId=' + row["Id"] + '" '
                        + 'class="btn btn-sm btn-primary btn-icon me-1" '
                        + 'title="Edit Survey"> <i class="fa fa-pencil-square"></i> </a >';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {

                        btns += '<span type="button" class="btn btn-sm btn-danger btn-icon me-1 btndelete" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'title="Delete Survey"><i class="fa fa-trash"></i></span>';
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
            {
                targets: 7,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(data);
                }
            },
            {
                targets: 8,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(data);
                }
            },
            {
                targets: 10,
                orderable: false,
                render: function (data, type, row) {
                    return FormatDate(data);
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

        "sAjaxSource": sitePath + "api/SurveyAPI/GetSurveys",
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
    filter.PublishDate = $("#PublishDate").val() || null;
    filter.CompletionDate = $("#CompletionDate").val() || null;

    return filter;
}

$(document).on("click", ".btndelete", function () {

    var $self = $(this);
    var surveyId = $self.data("id");

    Swal.fire({
        text: "Are you sure you want to delete this Survey?",
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
            Ajax.post(sitePath + "api/SurveyAPI/DeleteSurveyById?Id=" + surveyId, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Survey deleted successfully.",
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
                            window.location.href = "/Survey/ManageSurveys";
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

    var element = document.querySelector("#tbl_managesurveys")
    var blockUI = KTBlockUI.getInstance(element);

    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTable() {

    var element = document.querySelector("#tbl_managesurveys")
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



    var element = document.querySelector("#tbl_managesurveys")
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
