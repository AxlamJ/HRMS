var _EmployeeId = getParameterByName("EmployeeId") || null;
var _EmployeeCode = getParameterByName("EmployeeCode") || null;
var _EmployeeData = null;
HRMSUtil.onDOMContentLoaded(function () {
    hideSpinner();
    registerEventListners.init();
    InitializeDatePickers.init();
    PopulateDropDowns(function () {
        loadData(function () {
            LoadLeaves(function () {
                LoadUpcomingOcassions(function () {
                    LoadBio();
                })
            });
        });
    });

    [].slice.call(document.querySelectorAll('.tabs')).forEach(function (el) {
        new CBPFWTabs(el);
    });
});



function loadData(cb) {
    var url = sitePath + "api/EmployeeAPI/GetEmployeeByEmpCode?EmployeeCode=" + parseInt(_EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            _EmployeeData = resp.Employee;
            $(".btn-basic-details").data("id", _EmployeeData.Id);
            $(".btn-basic-details").data("employeecode", _EmployeeData.EmployeeCode);
            $(".btn-employment-details").data("id", _EmployeeData.Id);
            $(".btn-employment-details").data("employeecode", _EmployeeData.EmployeeCode);
            $("#profile-picture").attr("src", _EmployeeData.ProfilePhotoUrl);
            $("#user-fullname").text(_EmployeeData.FirstName + " " + _EmployeeData.LastName);
            $("#user-dob").text(FormatDate(_EmployeeData.DOB) + " (" + calculateAge(FormatDate(_EmployeeData.DOB)) + " years old)");
            $("#user-email").text(_EmployeeData.Email);
            $("#user-position").text(_EmployeeData.PositionName);
            $("#user-employmentstatus").text(_EmployeeData.EmploymentStatus);

            try {
                var Departments = JSON.parse(_EmployeeData.DepartmentName);

                var DepartmentNames = Departments.map(function (item) {
                    return item.name;
                }).join(", ");

            }
            catch {

            }

            $("#user-department").text(DepartmentNames);


            var Sites = JSON.parse(_EmployeeData.SiteName);

            var siteNames = Sites.map(function (item) {
                return item.name;
            }).join(", ");

            $("#user-site").text(siteNames);
            $("#user-hire-date").text(FormatDate(_EmployeeData.HiringDate));
            $("#user-direct-manager").text(_EmployeeData.ManagerName);
            $("#user-level").text(_EmployeeData.EmploymentLevel);
            $("#date-of-acceptance").text(FormatDate(_EmployeeData.AcceptanceDate));

            $("#user-alter-email").text(_EmployeeData.AlternativeEmail);
            $("#user-office-phone").text(_EmployeeData.PhoneNumber);
            $("#user-mobile").text(_EmployeeData.PhoneNumber);
            $("#user-country").text(_EmployeeData.Country);
            $("#user-address").text(_EmployeeData.Address);
            $("#user-city").text(_EmployeeData.City);
            $("#user-zip-code").text(_EmployeeData.PostalCode);

            if (_EmployeeData.Status == 2) {
                var notifybutton = document.getElementById("btn-notify-user");

                notifybutton.classList.toggle('d-none')
            }

            if (cb) {
                cb()
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

function LoadLeaves(cb) {
    var url = sitePath + "api/LeavesAPI/GetLeavesByEmpCode?EmployeeCode=" + _EmployeeCode;
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var Leaves = resp.Leaves;
            var container = document.getElementById("leaves-data");
            container.innerHTML = ""; // Clear existing content


            if (Leaves.length === 0) {
                // Append "no records" message
                container.innerHTML = `
                    <div class="row mt-5">
                        <div class="mt-3 text-center mb-0 font-18 text-primary">
                            <i class="fa fa-info-circle mr-1"></i> We couldn't find any records.
                        </div>
                    </div>`;
            } else {
                // Create table and populate rows
                var tableHtml = `
                    <div style="overflow-x: auto;">
                        <table class="table table-bordered table-striped table-hover" style="min-width: 600px; width: 100%;">
                            <thead class="thead-dark">
                                <tr>
                                    <th>Leave Type</th>
                                    <th>Start Date</th>
                                    <th>End Date</th>
                                    <th>Total Days</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody>`;

                Leaves.forEach(function (leave) {
                    tableHtml += `
                        <tr>
                            <td>${leave.LeaveTypeName}</td>
                            <td>${FormatDate(leave.StartDate)}</td>
                            <td>${FormatDate(leave.EndDate)}</td>
                            <td>${leave.TotalDays}</td>
        <td>${formatLeaveStatus(leave.LeaveStatusId, leave.LeaveStatusName)}</td>
                        </tr>`;
                });

                tableHtml += `</tbody></table></div>`;
                container.innerHTML = tableHtml;
            }

            if (cb) {
                cb()
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

function LoadUpcomingOcassions(cb) {
    var url = sitePath + "api/StatsAPI/GetBirthDayList";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var BirthDayList = resp.BirthDayList;

            var container = document.getElementById("upcoming-occasions");
            container.innerHTML = ""; // Clear existing content

            if (!BirthDayList || BirthDayList.length === 0) {
                container.innerHTML = `
            <div class="row mt-5">
                <div class="mt-3 text-center mb-0 font-18 text-primary">
                    <i class="fa fa-info-circle mr-1"></i> We couldn't find any records.
                </div>
            </div>`;
                return;
            }

            var tableHtml = `
                    <div style="overflow-x: auto;">
                        <table class="table table-bordered table-striped table-hover" style="min-width: 600px; width: 100%;">
                            <thead class="thead-dark">
                                <tr>
                                    <th>Employee Name</th>
                                    <th>Event Date</th>
                                    <th>Event Type</th>
                                </tr>
                            </thead>
                            <tbody>`;

            BirthDayList.forEach(function (item) {
                tableHtml += `
                        <tr>
                            <td>${item.EmployeeName}</td>
                            <td>${FormatDate(item.EventDate)}</td>
                            <td><span class="badge py-3 px-4 fs-7 badge-light-info">${item.EventType}</span></td>
                        </tr>`;
            });

            tableHtml += `</tbody></table></div>`;
            container.innerHTML = tableHtml;

            if (cb) {
                cb()
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

function formatLeaveStatus(statusId, statusName) {
    let badgeClass = "badge-light-secondary"; // default

    if (statusId == 1) badgeClass = "badge-light-warning";
    else if (statusId == 2) badgeClass = "badge-light-success";
    else if (statusId == 3) badgeClass = "badge-light-danger";

    return `<span class="badge py-3 px-4 fs-7 ${badgeClass}">${statusName}</span>`;
}

var InitializeDatePickers = function () {
    return {
        init: function () {

            $("#dob").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#on-boarding-date").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#hiring-date").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#probation-from").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#probation-to").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });
            $("#acceptance-date").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            flatpickr("#graduation-date", {
                dateFormat: "d/m/Y",
            });

            $("#issue-date").flatpickr({
                dateFormat: "d/m/Y",
            });

            $("#expiration-date").flatpickr({
                dateFormat: "d/m/Y",
            });

            $("#registration-date").flatpickr({
                dateFormat: "d/m/Y",
            });
        }
    }
}();


$(document).on('click', '#btn-notify-user', function () {

});