var LeaveId = null;

HRMSUtil.onDOMContentLoaded(function () {

    $("#datefrom").flatpickr({
        dateFormat: "d/m/Y",
        //defaultDate: new Date().fp_incr(-30),


    });

    $("#dateto").flatpickr({
        dateFormat: "d/m/Y",
        //defaultDate: new Date().fp_incr(-30),


    });
    $("#employeename").val(FullName);
    PopulateDropDowns(function () {

        PopulateAssignedBalanceLeaves();
    });

    hideSpinner();

});


function PopulateDropDowns(cb) {

    PopulateLeaveTypes(function () {
        //PopulateSites(function () {
        if (cb) {
            cb();
        }
        //})
    });

}


function PopulateLeaveTypes(cb) {
    var url = sitePath + "api/LeavesAPI/GetLeaveTypes";
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            var ddLeaveTpes = document.querySelector('#leavetype');
            $(ddLeaveTpes).empty();
            var lstLeaveTypes = (resp.LeaveTypes || []);
            var option = new Option();
            ddLeaveTpes.appendChild(option);
            lstLeaveTypes.forEach(function (item) {

                var option = new Option(item.LeaveTypeName, item.LeaveTypeId, false, false);
                $(option).data("leavetypeshortname", item.LeaveTypeShortName);
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


function PopulateAssignedBalanceLeaves() {
    var tbody = document.getElementById("tbl-leaves-count-body");
    tbody.innerHTML = ""; // Clear existing rows

    for (var i = 0; i < LeavesData.LeavesPolicy.length; i++) {
        var policy = LeavesData.LeavesPolicy[i];
        var availed = 0;

        for (var j = 0; j < LeavesData.EmployeeLeaves.length; j++) {
            if (LeavesData.EmployeeLeaves[j].LeaveTypeId === policy.LeaveTypeId) {
                availed = LeavesData.EmployeeLeaves[j].AvailedLeaves;
                break;
            }
        }

        var balance = policy.PolicyDays - availed;

        var tr = document.createElement("tr");
        tr.innerHTML =
            "<td>" + policy.LeaveTypeName.replace(" Leave", "") + "</td>" +
            "<td>" + policy.PolicyDays + "</td>" +
            "<td>" + availed + "</td>" +
            "<td>" + balance + "</td>";

        tbody.appendChild(tr);
    }
}

// These are the constraints used to validate the form
var constraints = {
    "employeename": {
        presence: {
            message: " is required."
        }
    },
    "leavetype": {
        presence: {
            message: " is required."
        }
    },
    "datefrom": {
        presence: { message: " is required." }
    },
    "dateto": {
        presence: { message: " is required." }
    },
    "reason": {
        presence: { message: " is required." }
    }
};



$("#btn-apply-leave").on("click", function () {
    var form = document.querySelector("#apply-leave-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var leave = {};

        leave.employeeName = $("#employeename").val();
        leave.leaveTypeId = $("#leavetype").val();
        leave.leaveTypeName = $("#leavetype :Selected").text();
        leave.leaveTypeShortName = $("#leavetype :Selected").data("leavetypeshortname");
        leave.startDate = $("#datefrom").val();
        leave.endDate = $("#dateto").val();
        leave.leaveReason = $("#reason").val();

        var startDate = parseDate($("#datefrom").val());
        var endDate = parseDate($("#dateto").val());

        // Calculate requested leave days
        var requestedDays = Math.floor((endDate - startDate) / (1000 * 60 * 60 * 24)) + 1;

        // Get assigned and availed leave info
        var policy = LeavesData.LeavesPolicy.find(function (p) {
            return p.LeaveTypeId === parseInt(leave.leaveTypeId);
        });

        var empLeave = LeavesData.EmployeeLeaves.find(function (l) {
            return l.LeaveTypeId === parseInt(leave.leaveTypeId);
        });

        var assigned = policy ? policy.PolicyDays : 0;
        var availed = empLeave ? empLeave.AvailedLeaves : 0;
        var balance = assigned - availed;

        // Prevent if exceeding balance
        if (requestedDays > balance) {
            Swal.fire({
                text: "You have already availed " + availed + " Leaves. You cannot apply for more than " + balance + " day(s) for this leave type.",
                icon: "warning",
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn btn-primary"
                }
            });
            return; // stop form submission
        }
        else {
            console.log(leave);
            var url = sitePath + 'api/LeavesAPI/UpsertLeave';

            Ajax.post(url, leave, function (response) { // Ensure you use the correct data (exportRequestBody)
                //hideSpinner();

                if (response.StatusCode == 200) {
                    Swal.fire({
                        text: "Leave applied successfully.",
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

                            window.location.href = "/Home/SearchLeaves";
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

function parseDate(str) {
    var parts = str.split('/');
    // parts[0] = day, parts[1] = month, parts[2] = year
    return new Date(parts[2], parts[1] - 1, parts[0]);
}