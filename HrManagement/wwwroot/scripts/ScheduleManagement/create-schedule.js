var ScheduleId = getParameterByName("Id") || null;
var EmployeeCode = getParameterByName("EmployeeCode") || null;
var ScheduleData = null;
HRMSUtil.onDOMContentLoaded(function () {

    $("#date-start").flatpickr({
        dateFormat: "Y-m-d",
        onChange: function (selectedDates, dateStr, instance) {
            $("#date-end").flatpickr({
                minDate: selectedDates[0]
            });
        },
        //defaultDate: new Date().fp_incr(-30),
    });

    $("#date-end").flatpickr({
        dateFormat: "Y-m-d",
        //defaultDate: new Date().fp_incr(-30),
    });

    $("#start-time").flatpickr({
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true,
        onChange: function (selectedDate, selectedTime, instance) {
            $("#end-time").flatpickr({
                enableTime: true,
                noCalendar: true,
                dateFormat: "H:i",
                time_24hr: true,
                minTime: selectedTime
            });
        },
        //defaultDate: new Date().fp_incr(-30),
    });

    $("#end-time").flatpickr({
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true
        //defaultDate: new Date().fp_incr(-30),
    });

    PopulateDropDowns(function () {
        RenderData();
    });


    unspin();
});

function PopulateDropDowns(cb) {

    PopulateEmployees(function () {
        cb();
    });

}

function PopulateEmployees(cb) {

    var ddEmployees = document.querySelector('#employee');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployees.appendChild(option);
    });

    var ddEmployeesFilter = document.querySelector('#employeefilter');
    $(ddEmployeesFilter).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployeesFilter.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployeesFilter.appendChild(option);
    });


    var ddSites = document.querySelector('#site');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });



    if (cb) {
        cb()
    }

}


// These are the constraints used to validate the form
var constraints = {
    "employee": {
        presence: {
            message: "is required."
        }
    },
    "site": {
        presence: {
            message: "is required."
        }
    },
    "date-start": {
        presence: {
            message: "is required."
        }
    },
    "date-end": {
        presence: { message: "is required." }
    },
    "start-time": {
        presence: { message: "is required." }
    },
    "end-time": {
        presence: { message: "is required." }
    },
    "days_on": {
        presence: { message: "is required.", allowEmpty: false }
    }
};


$(document).on("click", ".btn-create-schedule", function () {
    var form = document.querySelector("#schedule-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var schedule = {};

        var action = $(this).data('action');
        var message = "Schedule created successfully.";
        if (action == 'update') {
            message = "Schedule updated successfully."
            schedule.Id = parseInt($(this).data('id'));
        }

        schedule.employeeCode = ($("#employee").val() != '' && $("#employee").val() != null && $("#employee").val() != 0) ? parseInt($("#employee").val()) : null;
        schedule.employeeName = $("#employee :Selected").text();
        schedule.startDate = $("#date-start").val();
        schedule.endDate = $("#date-end").val();
        schedule.startTime = $("#start-time").val();
        schedule.endTime = $("#end-time").val();
        schedule.SiteId = $("#site").val();
        schedule.SiteName = $("#site :Selected").text();
        var onDays = $("#days_on").val();

        let allDays = $("#days_on option").map(function () {
            return $(this).val();
        }).get().filter(function (val) { return val !== "" }); // Get all options

        let offDays = allDays.filter(function (val) { return !onDays.includes(val) }); // Find unselected


        schedule.onDays = JSON.stringify(onDays);
        schedule.offDays = JSON.stringify(offDays);



        //if (EmployeeId != null) {
        //    employee.Id = EmployeeData.Id;
        //}
        var url = sitePath + 'api/ScheduleManagementAPI/UpsertSchedule';

        Ajax.post(url, schedule, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: message,
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




$(document).on("click", ".btnedit", function () {

    var $self = $(this);
    var Id = $self.data("id");

    Ajax.get(sitePath + "api/ScheduleManagementAPI/GetScheduleById?Id=" + Id, function (resp) {
        if (resp.StatusCode == 200) {

            var schedule = resp.Schedule

            $("#employee").val(schedule.EmployeeCode).trigger("change");
            //$("#date-start").val(FormatDate(schedule.StartDate));
            //$("#date-end").val(FormatDate(schedule.EndDate));

            $("#date-start").flatpickr({
                dateFormat: "Y-m-d",
            }).setDate(FormatScheduleDate(schedule.StartDate));

            $("#date-end").flatpickr({
                dateFormat: "Y-m-d",
            }).setDate(FormatScheduleDate(schedule.EndDate));


            $("#start-time").flatpickr({
                enableTime: true,
                noCalendar: true,
                dateFormat: "H:i",
                time_24hr: true,
            }).setDate(schedule.StartTime);


            $("#end-time").flatpickr({
                enableTime: true,
                noCalendar: true,
                dateFormat: "H:i",
                time_24hr: true,
            }).setDate(schedule.EndTime);

            //$("#start-time").val(schedule.StartTime).trigger("change")
            //$("#end-time").val(schedule.EndTime).trigger("change")
            $("#site").val(schedule.SiteId).trigger("change");
            var onDays = JSON.parse(schedule.OnDays);
            $("#days_on").val(onDays).trigger('change');
            $(".btn-create-schedule").data('id', schedule.Id);
            $(".btn-create-schedule").data('action', 'update');
            $('#Add_Schedule_Modal').modal('show');

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


    //if (EmployeeBio != null) {

    //    $(form).find("#about").text(EmployeeBio.About);
    //    $(form).find("#hobbies").text(EmployeeBio.Hobbies);
    //    $(form).find("#favorite-books").text(EmployeeBio.FavoriteBooks);
    //    $(form).find("#music-preference").text(EmployeeBio.MusicPreference);
    //    $(form).find("#sports").text(EmployeeBio.Sports);
    //}

});