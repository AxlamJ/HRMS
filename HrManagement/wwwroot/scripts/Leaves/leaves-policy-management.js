HRMSUtil.onDOMContentLoaded(function () {

    //$("#datefrom").flatpickr({
    //    dateFormat: "d/m/Y",
    //    //defaultDate: new Date().fp_incr(-30),


    //});

    //$("#dateto").flatpickr({
    //    dateFormat: "d/m/Y",
    //    //defaultDate: new Date().fp_incr(-30),


    //});
    //$("#employeename").val(FullName);
    PopulateDropDowns(function () {
        RenderLeaveTypesData(function () {
            RenderLeavesPolicyData();
        });
    });

    hideSpinner();

});


function PopulateDropDowns(cb) {
    var url = sitePath + "api/LeavesAPI/GetLeaveTypes";
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            var ddLeaveTpes = document.querySelector('#leave-type-id');
            $(ddLeaveTpes).empty();
            var lstLeaveTypes = (resp.LeaveTypes || []);
            var option = new Option();
            ddLeaveTpes.appendChild(option);
            lstLeaveTypes.forEach(function (item) {

                var option = new Option(item.LeaveTypeName, item.LeaveTypeId, false, false);
                $(option).data("leavetypeshortname", item.LeaveTypeShortName);
                $(option).data("autoapprove", item.AutoApprove);
                ddLeaveTpes.appendChild(option);
            });


            var ddLeaveTpes = document.querySelector('#policy-type');
            $(ddLeaveTpes).empty();
            var lstLeaveTypes = (resp.LeaveTypes || []);
            var option = new Option();
            ddLeaveTpes.appendChild(option);
            lstLeaveTypes.forEach(function (item) {

                var option = new Option(item.LeaveTypeName, item.LeaveTypeId, false, false);
                $(option).data("leavetypeshortname", item.LeaveTypeShortName);
                $(option).data("autoapprove", item.AutoApprove);
                ddLeaveTpes.appendChild(option);
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

$(document).on('click', '.btn-new-type', function () {

    $("#add-new-leave-type-modal").modal('show');
});

$(document).on('click', '.btn-edit-leave-type', function () {

    var $self = $(this);

    var leavetypeid = $self.data('id');
    var leavetypename = $self.data('typename');
    var autoapprove = $self.data('autoapprove');

    $(document).find('#leave-type-name').val(leavetypename);
    if (autoapprove == true) {
        document.getElementById("approve-automatically").checked = true;
    }
    else {
        document.getElementById("approve-automatically").checked = false;
    }
    $(document).find('#btn-add-new-leave-type').data('leavetypeid', leavetypeid);

    $("#add-new-leave-type-modal").modal('show');
});


// These are the constraints used to validate the form
var constraints = {
    "leave-type-name": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    }
};


$("#btn-add-new-leave-type").on("click", function () {
    var form = document.querySelector("#leave-type-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var leaveType = {};


        var leaveTypeId = $(this).data('leavetypeid') || null;
        leaveType.leaveTypeName = $("#leave-type-name").val();
        leaveType.leaveTypeShortName = leaveType.leaveTypeName
            .split(" ")
            .filter(word => word.length > 0)
            .map(word => word[0])
            .join("");
        var checkBox = document.getElementById("approve-automatically");
        leaveType.autoApprove = checkBox.checked == true ? true : false;

        if (leaveTypeId != null) {
            leaveType.leaveTypeId = leaveTypeId;
        }
        //site.countryId

        var url = sitePath + 'api/LeavesAPI/UpsertLeaveTypes';

        Ajax.post(url, leaveType, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Leave type saved successfully.",
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

        //showSuccess();
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


$(document).on("click", ".btn-delete-leave-type", function () {

    var leaveTypeId = $(this).data('id') || null;

    Swal.fire({
        text: "Are you sure you want to delete this Leave Type?",
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


            var url = sitePath + 'api/LeavesAPI/DeleteLeaveTypeById?leaveTypeId=' + parseInt(leaveTypeId);

            Ajax.post(url, null, function (response) { // Ensure you use the correct data (exportRequestBody)
                //hideSpinner();

                if (response.StatusCode == 200) {
                    Swal.fire({
                        text: "Leave type deletion requested successfully.",
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

    var leaveTypeId = $(this).data('id') || null;

    Swal.fire({
        text: "Are you sure you want to approve deletion of this leave type?",
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
            var url = sitePath + 'api/LeavesAPI/ApproveDeleteLeaveTypeById?leaveTypeId=' + parseInt(leaveTypeId);

            Ajax.post(url, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Leave Type deletion approved successfully.",
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



$(document).on('click', '.btn-new-policy', function () {
    $("#add-new-leave-policy-modal").modal('show');
});


$(document).on('click', '.btn-edit-leave-policy', function () {

    var $self = $(this);

    var policyId = $self.data('id');
    var PolicyName = $self.data('policyname');
    var PolicyTypeId = $self.data('policytypeid');
    var PolicyType = $self.data('policytypename');
    var PolicyDays = $self.data('policydays');
    var PolicyPeriod = $self.data('policyperiod');
    var MovetoNextPeriod = $self.data('movetonextperiod');

    $(document).find('#policy-type').val(PolicyTypeId).trigger('change');
    $(document).find('#policy-name').val(PolicyName);
    $(document).find('#policy-days').val(PolicyDays);
    $(document).find('#policy-period').val(PolicyPeriod).trigger('change');
    if (MovetoNextPeriod == true) {
        $(document).find('#unused-days').val(1).trigger('change');
    }
    else {
        $(document).find('#unused-days').val(0).trigger('change');
    }

    $(document).find('#btn-add-new-leave-policy').data('leavepolicyid', policyId);

    $("#add-new-leave-policy-modal").modal('show');
});



// These are the constraints used to validate the form
var policyconstraints = {
    "policy-type": {
        presence: {
            message: " is required."
        }
    },
    "policy-name": {
        presence: {
            message: " is required."
        }
    },
    "policy-days": {
        presence: {
            message: " is required."
        }
    },
    "policy-period": {
        presence: {
            message: " is required."
        }
    },
    "unused-days": {
        presence: {
            message: " is required."
        }
    }
};


$("#btn-add-new-leave-policy").on("click", function () {
    var form = document.querySelector("#leave-policy-form");
    var errors = validate(form, policyconstraints);
    showErrors(form, errors || {});
    if (!errors) {
        var leavePolicy = {};


        var leavePolicyId = $(this).data('leavepolicyid') || null;

        leavePolicy.policyName = $("#policy-type :Selected").text();
        leavePolicy.policyTypeId = $("#policy-type").val();
        leavePolicy.policyType = $("#policy-name").val();
        leavePolicy.policyDays = $("#policy-days").val();
        leavePolicy.policyPeriod = $("#policy-period").val();
        //leavePolicy.applyDays
        leavePolicy.movetoNextPeriod = $("#unused-days").val() == 1 ? true : false;


        if (leavePolicyId != null) {
            leavePolicy.id = leavePolicyId;
        }
        //site.countryId

        var url = sitePath + 'api/LeavesAPI/UpsertLeavePolicy';

        Ajax.post(url, leavePolicy, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Leave Policy saved successfully.",
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

        //showSuccess();
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




$(document).on("click", ".btn-delete-leave-policy", function () {

    var LeavePolicyId = $(this).data('id') || null;

    Swal.fire({
        text: "Are you sure you want to delete this Leave Policy?",
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


            var url = sitePath + 'api/LeavesAPI/DeleteLeavePolicyById?policyId=' + parseInt(LeavePolicyId);

            Ajax.post(url, null, function (response) { // Ensure you use the correct data (exportRequestBody)
                //hideSpinner();

                if (response.StatusCode == 200) {
                    Swal.fire({
                        text: "Leave Policy deletion requested successfully.",
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




$(document).on("click", ".btnapprovepolicydelete", function () {

    var LeavePolicyId = $(this).data('id') || null;

    Swal.fire({
        text: "Are you sure you want to approve deletion of this leave policy?",
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
            var url = sitePath + 'api/LeavesAPI/ApproveDeleteLeavePolicyById?policyId=' + parseInt(LeavePolicyId);

            Ajax.post(url, null, function (resp) {
                if (resp.StatusCode == 200) {

                    Swal.fire({
                        text: "Leave policy deletion approved successfully.",
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


var RenderLeaveTypesData = function (cb) {
    $("#tbl_leavetypes").DataTable({
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
            { title: 'Leave Type', data: "LeaveTypeName", className: 'all nowrap' },
            { title: 'Auto Approve', data: 'AutoApprove', className: 'all nowrap' },
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
                "targets": 2,
                //className: 'text-center',
                "render": function (data, type, row) {
                    //var fullName = row["FirstName"] + " "+row["LastName"] || null;
                    var autoApprove = (row["AutoApprove"] == null || row["AutoApprove"] == 0) ? "No" : "Yes";
                    return autoApprove;

                },
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
                        + 'class="btn btn-sm btn-primary btn-icon me-1 btn-edit-leave-type" '
                        + 'data-id="' + row["LeaveTypeId"] + '"'
                        + 'data-typename="' + row["LeaveTypeName"] + '"'
                        + 'data-autoapprove="' + row["AutoApprove"] + '"'
                        + 'title="Edit Leave Type"> <i class="fa fa-pencil"></i> </span>';

                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {

                        btns += '<span type="button" '
                            + 'class="btn btn-sm btn-danger btn-icon me-1 btn-delete-leave-type" '
                            + 'data-id="' + row["LeaveTypeId"] + '"'
                            + 'title="Delete Leave Type"> <i class="fa fa-trash"></i> </span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 btnapprovedelete" '
                            + 'data-id = "' + row["LeaveTypeId"] + '"'
                            + 'title="Approve Leave Type Deletion"><i class="fa fa-trash"></i></span>';
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

        "sAjaxSource": sitePath + "api/LeavesAPI/SearchLeaveTypes", //Manage report template search
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
            blockTypeTable();
            var queryData = getSearchLeaveTypesFilters()
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
            unblockTypeTable();
            hideTypeOverlay();

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


var RenderLeavesPolicyData = function () {
    $("#tbl_leavespolicy").DataTable({
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
            { title: 'Policy Name', data: "PolicyName", className: 'all nowrap' },
            { title: 'Policy Type', data: 'PolicyType', className: 'all nowrap' },
            { title: 'Policy Days', data: 'PolicyDays', className: 'all nowrap' },
            { title: 'Policy Period', data: 'PolicyPeriod', className: 'all nowrap' },
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
                targets: 5,
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
                        + 'class="btn btn-sm btn-primary btn-icon me-1 btn-edit-leave-policy" '
                        + 'data-id="' + row["Id"] + '"'
                        + 'data-policyname="' + row["PolicyName"] + '"'
                        + 'data-policytypeid="' + row["PolicyTypeId"] + '"'
                        + 'data-policytypename="' + row["PolicyType"] + '"'
                        + 'data-policydays="' + row["PolicyDays"] + '"'
                        + 'data-policyperiod="' + row["PolicyPeriod"] + '"'
                        + 'data-movetonextperiod="' + row["MovetoNextPeriod"] + '"'
                        + 'title="Edit Leave Type"> <i class="fa fa-pencil"></i> </span>';


                    if (row["IsActive"] && UserRoleName.toLowerCase().indexOf('admin') > -1) {
                        btns += '<span type="button" '
                            + 'class="btn btn-sm btn-danger btn-icon me-1 btn-delete-leave-policy" '
                            + 'data-id="' + row["Id"] + '"'
                            + 'title="Delete Leave Type"> <i class="fa fa-trash"></i> </span>';
                    }


                    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 && (row["IsActive"] == false || row["IsActive"] == 'false') && (row["IsActiveApproved"] == false || row["IsActiveApproved"] == 'false' || row["IsActiveApproved"] == null || row["IsActiveApproved"] == '')) {

                        btns += '<span type="button" class="btn btn-sm btn-warning btn-icon me-1 btnapprovepolicydelete" '
                            + 'data-id = "' + row["Id"] + '"'
                            + 'title="Approve Leave Policy Deletion"><i class="fa fa-trash"></i></span>';
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

        "sAjaxSource": sitePath + "api/LeavesAPI/SearchLeavesPolicy", //Manage report template search
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
            blockPolicyTable();
            var queryData = getSearchLeavePolicyFilters()
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
            unblockPolicyTable();
            hidePolicyOverlay();

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



function getSearchLeaveTypesFilters() {
    var filter = {};
    filter.LeaveTypeName = $("#leavetypename").val() || null;
    filter.AutoApprove = ($("#auto-approve").val() != '' && $("#auto-approve").val() != null) ? ($("#auto-approve").val() == 1 ? true : false) : null;

    return filter;
}
function getSearchLeavePolicyFilters() {
    var filter = {};
    filter.PolicyName = $("#leavepolicyname").val() || null;
    filter.PolicyTypeId = ($("#leave-type-id").val() != '' && $("#leave-type-id").val() != null) ? $("#leave-type-id").val() : null;

    return filter;
}



$('#btnSearchTypes').click(function () {
    filterLeaveTypes();
});

$('#btnSearchPolicy').click(function () {
    filterLeavesPolicy();
});

$('#btnClear').click(function () {
    cancelSearch();
});

$('#btnClearPolicy').click(function () {
    cancelSearch();
});

function filterLeaveTypes() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderLeaveTypesData();
    hideSpinner();
}

function filterLeavesPolicy() {
    showSpinner();
    //savePageState();
    //saveSearchState();
    RenderLeavesPolicyData();
    hideSpinner();
}


function cancelSearch() {
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    //clearSearchState();
    window.location = nURL;
}
function hideTypeOverlay() {

    var element = document.querySelector("#tbl_leavetypes")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockTypeTable() {

    var element = document.querySelector("#tbl_leavetypes")
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

function unblockTypeTable() {

    var element = document.querySelector("#tbl_leavetypes")
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

function hidePolicyOverlay() {

    var element = document.querySelector("#tbl_leavespolicy")
    var blockUI = KTBlockUI.getInstance(element);
    if (blockUI.blocked) {
        setTimeout(() => {
            blockUI.release();
            blockUI.destroy();
        }, 1000);
    }
}

function blockPolicyTable() {

    var element = document.querySelector("#tbl_leavespolicy")
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

function unblockPolicyTable() {

    var element = document.querySelector("#tbl_leavespolicy")
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
