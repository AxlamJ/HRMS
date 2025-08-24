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
    var url = sitePath + "api/EmployeeAPI/GetEmployeeByEmpCode?EmployeeCode=" + parseInt(EmployeeCode);
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            EmployeeData = resp.Employee;
            $(".btn-basic-details").data("id", EmployeeData.Id);
            $(".btn-basic-details").data("employeecode", EmployeeData.EmployeeCode);
            $(".btn-employment-details").data("id", EmployeeData.Id);
            $(".btn-employment-details").data("employeecode", EmployeeData.EmployeeCode);
            $(".btn-edit-contact-details").data("id", EmployeeData.Id);
            $(".btn-edit-contact-details").data("employeecode", EmployeeData.EmployeeCode);
            $("#profile-picture").attr("src", EmployeeData.ProfilePhotoUrl);
            $("#user-fullname").text(EmployeeData.FirstName + " " + EmployeeData.LastName);
            $("#user-dob").text(FormatDate(EmployeeData.DOB) + " (" + calculateAge(FormatDate(EmployeeData.DOB)) + " years old)");
            $("#user-email").text(EmployeeData.Email);
            $("#user-position").text(EmployeeData.PositionName);
            $("#user-employmentstatus").text(EmployeeData.EmploymentStatus);

            try {
                var Departments = JSON.parse(EmployeeData.DepartmentName);

                var DepartmentNames = Departments.map(function (item) {
                    return item.name;
                }).join(", ");

                $("#user-department").text(DepartmentNames);

                var Sites = JSON.parse(EmployeeData.SiteName);

                var siteNames = Sites.map(function (item) {
                    return item.name;
                }).join(", ");

            }
            catch {

            }

            $("#user-site").text(siteNames);
            $("#user-hire-date").text(FormatDate(EmployeeData.HiringDate));
            $("#user-direct-manager").text(EmployeeData.ManagerName);
            $("#user-level").text(EmployeeData.EmploymentLevel);
            $("#date-of-acceptance").text(FormatDate(EmployeeData.AcceptanceDate));

            $("#user-alter-email").text(EmployeeData.AlternativeEmail);
            $("#user-office-phone").text(EmployeeData.PhoneNumber);
            $("#user-mobile").text(EmployeeData.PhoneNumber);
            $("#user-country").text(EmployeeData.Country);
            $("#user-address").text(EmployeeData.Address);
            $("#user-city").text(EmployeeData.City);
            $("#user-zip-code").text(EmployeeData.PostalCode);
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
    var url = sitePath + "api/LeavesAPI/GetLeavesByEmpCode?EmployeeCode=" + EmployeeCode;
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

            $("#graduation-date").flatpickr({
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