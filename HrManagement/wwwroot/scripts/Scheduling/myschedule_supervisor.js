
var calendarEl = $('#calendar');
var PlannedSchedule = null;
var AllEmployees = null;
HRMSUtil.onDOMContentLoaded(function () {
    calendar.render();
    // add class to style the center content of headertoolbar
    $('.fc-toolbar-chunk > div').addClass('fc-center');
    $('.fc-AddSchedule-button').addClass('fc-button-active');


    PopulateEmployees(function () {
        GetResources();
    });

    //if (hasRole(4) && globalRoles.length == 1) {
    //    $('#employee').attr("disabled", "disabled")
    //}
    unspin();

});

function PopulateEmployees(cb) {

    var ddEmployees = document.querySelector('#employee');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    AllEmployees = lstEmployees;
    var option = new Option();
    ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {
        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployees.appendChild(option);
    });


    var ddSites = document.querySelector('#site');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    AllSites = lstSites;
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

$('#btn_search').click(function () {

    calendar.removeAllEvents();

    GetResources();


});
$('#btn_clear').click(function () {

    //clearSearchState();
    var nURL = window.location.href.replace(/([&\?]SaveState=true*$|key=val&|[?&]key=val(?=#))/, '');
    window.onbeforeunload = null;
    window.location = nURL.split("?")[0];

});

// Var Function to Hold FullCalendar 
var calendar = new FullCalendar.Calendar(calendarEl[0], {
    schedulerLicenseKey: 'GPL-My-Project-Is-Open-Source',
    timeZone: 'local',
    firstDay: 1, //set moday as start day of week
    themeSystem: 'bootstrap',
    initialView: 'dayGridMonth',
    resourceOrder: 'position',
    expandRows: true,
    selectable: false,
    height: 'auto',
    editable: false,
    droppable: false,
    eventOverlap: true, // Allow events to overlap in the same column
    dayMaxEventRows: true,
    eventColor: '#428bca',
    eventDisplay: 'auto',
    contentHeight: 800,
    headerToolbar: {
        right: '',
        center: 'prev,backwardButton,title,forwardButton,next', //center top will contain previous next and the title as name of month
        left: 'today resourceTimelineDay dayGridMonth' /*dayGridWeek*/
    },
    buttonIcons: {
        prev: 'chevrons-left',
        next: 'chevrons-right',
        backwardButton: 'chevron-left',
        forwardButton: 'chevron-right'
    },
    buttonText: {
        resourceTimelineDay: 'Day',
        dayGridMonth: 'Month',
        //resourceTimelineWeek: 'Week',
    },
    views: { //Number of views
        //resourceTimelineWeek: {
        //    buttonText: "Week",
        //    type: 'resourceTimeline',
        //    displayEventTime: true,
        //    duration: { weeks: 1 },
        //    slotDuration: { days: 1 },
        //    dayMaxEventRows: 6,
        //    titleFormat: function (date) {

        //        const startDate = moment(date.start);
        //        const endDate = moment(date.end);
        //        if (startDate.month() === endDate.month()) {
        //            return startDate.format("MMM DD") + " - " + endDate.format("DD, YYYY");
        //        } else {
        //            return startDate.format("MMM DD") + " - " + endDate.format("MMM DD, YYYY");
        //        }
        //    },
        //    slotLabelFormat: {
        //        day: 'numeric',
        //        weekday: 'short'
        //    },
        //},
        resourceTimelineDay: {
            duration: { day: 1 },
            slotDuration: '1:00',
            slotLabelInterval: '1:00'
        },
    },
    resourceAreaColumns: [
        {
            field: 'title',
            headerContent: 'Employee'
        }
    ],
    resourceAreaWidth: "250px",
    customButtons: {

        backwardButton: {
            icon: 'chevron-left',
            click: function () {

                calendar.gotoDate(moment(calendar.view.currentEnd).add(- 2, "days").toDate());
                calendar.setOption('firstDay', calendar.view.currentStart.getDay() - 1);
                calendar.removeAllEvents();

                Calendardateschange();
            }
        },
        forwardButton: {
            icon: 'chevron-right',
            click: function () {
                calendar.gotoDate(moment(calendar.view.currentStart).add(1, "days").toDate());
                calendar.setOption('firstDay', calendar.view.currentStart.getDay() + 1);
                calendar.removeAllEvents();

                Calendardateschange();
            }
        },
        prev: {
            click: function () {

                var currentView = calendar.view.type;

                //if (currentView == "resourceTimelineWeek") {

                //    calendar.gotoDate(moment(calendar.view.currentEnd).add(- 8, "days").toDate());
                //    calendar.setOption('firstDay', calendar.view.currentStart.getDay() - 7);
                //}

                if (currentView == "resourceTimelineDay") {

                    calendar.gotoDate(moment(calendar.view.currentEnd).add(- 2, "days").toDate());
                    calendar.setOption('firstDay', calendar.view.currentStart.getDay() - 1);
                }
                else if (currentView == "dayGridMonth") {

                    calendar.gotoDate(moment(calendar.view.currentEnd).add(-1, "days").add(- 1, "months").toDate());
                    calendar.setOption('firstDay', calendar.view.currentStart.getMonth() - 1);
                }
                calendar.removeAllEvents();
                Calendardateschange();
            }
        },
        next: {
            click: function () {

                var currentView = calendar.view.type;

                //if (currentView == "resourceTimelineWeek") {
                //    calendar.gotoDate(moment(calendar.view.currentStart).add(7, "days").toDate());
                //    calendar.setOption('firstDay', calendar.view.currentStart.getDay() + 7);
                //}
                if (currentView == "resourceTimelineDay") {

                    calendar.gotoDate(moment(calendar.view.currentStart).add(1, "days").toDate());
                    calendar.setOption('firstDay', calendar.view.currentStart.getDay() + 1);
                }
                else if (currentView == "dayGridMonth") {

                    calendar.gotoDate(moment(calendar.view.currentEnd).add(-1, "days").add(1, "months").toDate());
                    calendar.setOption('firstDay', calendar.view.currentStart.getMonth() + 1);
                }

                calendar.removeAllEvents();
                Calendardateschange();
            },
        },
    },
    // This function is called after an external event is dropped onto the calendar
    eventReceive: function (info) {

        var event = calendar.getEventById(info.event.id);

        if (event) {
            event.remove();
        }
    },
    eventDidMount: function (info) {

        var eventElement = info.el;

        // Display events in month view (style was changed when time included) same as in week and day, remove dot button from event and change styling same as in week and day events also showing Employee name in title.
        if (info.view.type === "dayGridMonth") {


            const employeename = info.event.extendedProps.EmployeeName;

            // Check if the title already contains the Employee Name
            if (!info.event.title.includes(employeename)) {
                // Append the Employee Name to the event title
                info.event.setProp('title', `${info.event.title}, (${employeename})`);
            }
        }

        //month, weeek and day timeview
        //if (info.view.type === "dayGridMonth") {

        //    var _title = info.el.querySelectorAll('.fc-event-title')[0];
        //    if (_title) {
        //        _title.innerHTML.content = "";

        //        _title.innerHTML = info.event.title;
        //    }
        //} else {
        //    var _title = info.el.querySelectorAll('.fc-event-title')[0];
        //    if (_title) {

        //        _title.innerHTML = info.event.title.replace(/<br\s*\/?>/gi, "");;
        //    }

        //}
        //else {
        //    //listview/agenda
        //    var _list_event_title = info.el.querySelectorAll('.fc-list-event-title')[0];
        //    if (_list_event_title) {
        //        var _a = $('a', _list_event_title)[0];
        //        if (_a) {
        //            _a.innerHTML = info.event.title;
        //        }
        //    }

        //}
        eventElement.addEventListener('contextmenu', function (e) {
            e.preventDefault(); // Prevent the default context menu
        });
    },
    eventMouseEnter: function (info) {

        let scheduleinfo = '';
        let employeeName = '';
        let siteName = '';
        let event_Id = ''
        let event_Start = ''
        let event_End = ''

        employeeName = info?.event?._def?.extendedProps.EmployeeName.trim();
        siteName = info?.event?._def?.extendedProps.SiteName.trim();
        event_Id = info?.event?._def?.publicId.trim();
        event_Start = moment(info?.event?.startStr?.replace('Z', '')).format('DD/MM/YYYY HH:mm');
        let endStr = info?.event?.endStr == '' ? info.event.startStr?.replace('Z', '') : info?.event?.endStr.replace('Z', ''); // Beacause Initially before drag or drop, End date is null
        event_End = moment(endStr)?.format('DD/MM/YYYY HH:mm');

        var eventObject = calendar?.getEventById(event_Id);

        var employee_code = info.event?._def?.resourceIds[0]; // TO Get Employee Name Using Employee Code

        var dataHover =
            ` <div class="tooltip-custom-body">
                 <div class="tooltip-custom-content">
                    <div class="tooltip-row">
                        <span class="tooltip-label">Employee Name:</span>
                        <span class="tooltip-value">${employeeName}</span>
                    </div>
                    <div class="tooltip-row">
                        <span class="tooltip-label">Site:</span>
                        <span class="tooltip-value">${siteName}</span>
                    </div>
                    <div class="tooltip-row">
                        <span class="tooltip-label">Start Date:</span>
                        <span class="tooltip-value">${event_Start}</span>
                    </div>
                    <div class="tooltip-row">
                        <span class="tooltip-label">End Date:</span>
                        <span class="tooltip-value">${event_End}</span>
                    </div>`;


        dataHover += `</div></div>`;



        var popoverOptions = {
            container: '#calendar',
            placement: 'auto',
            trigger: 'hover',
            html: true,
            fallbackPlacement: 'clockwise',
            content: function () {
                return dataHover;
            }
        };

        $(info.el).popover(popoverOptions);

    },
    // Callback function when Calendar view is changed or called.
    viewDidMount: function (info) {

        if (info.view.type === 'dayGridMonth' || info.view.type === 'resourceTimelineDay') {
            $('.fc-backwardButton-button').remove();
            $('.fc-forwardButton-button').remove();
        }
        else {
            $(".fc-backwardButton-button, .fc-forwardButton-button").show();
        }

    },
    datesSet: function (info) {
        if (info.view.type === 'dayGridMonth' || info.view.type === 'resourceTimelineDay') {
            $('.fc-backwardButton-button').remove();
            $('.fc-forwardButton-button').remove();
        }
        else {
            $(".fc-backwardButton-button, .fc-forwardButton-button").show();
        }

    },
    eventOverlap: function (stillEvent, movingEvent) {
        //  Check if the overlapping event is an event and existing event is Background event.
        if ((calendar.view.type != 'resourceTimelineDay')) {
            // Remove the still event
            // stillEvent.remove();
        }
        return true;
    },
});


$(document).on('click', '#calendar .fc-center h2', function (e) {

    var fpId = "tempFlatpicker";
    var $hiddenFp = $('<input type="hidden" id="' + fpId + '">');

    // Initialize Flatpicker on the hidden input element
    $hiddenFp.flatpickr();

    flatpickr($hiddenFp, {
        dateFormat: 'd M Y',
        defaultDate: new Date(),
        locale: {
            firstDayOfWeek: 1 // 0 for Sunday, 1 for Monday, etc.
        },
        onReady: (selectedDate, dateStr, instance) => {

            instance.setDate(calendar.view.currentStart);
        },
        onChange: (selectedDate, dateStr, instance) => {

            var changedDate = new Date(selectedDate);
            calendar.gotoDate(changedDate);
        },
    }).open();

    $('.flatpickr-calendar').css({ left: e.pageX - 60, top: e.pageY + 17 });
});

function Calendardateschange() {


    showSpinner();
    GetPlannedSchedulebyEmployeeId();

    if (calendar.view.type === 'dayGridMonth' || calendar.view.type === 'resourceTimelineDay') {
        $('.fc-backwardButton-button').remove();
        $('.fc-forwardButton-button').remove();
    }
    else {
        $(".fc-backwardButton-button, .fc-forwardButton-button").show();
    }


}


// Function to remove all planned events from calendar
function RemoveAllPlannedEventsfromCalendar() {

    // Find all events which are Planned
    var eventsToRemove = calendar.getEvents().filter(function (event) {
        return event.extendedProps.IsPlannedSchedule === true;
    });

    if (eventsToRemove) {
        eventsToRemove.forEach(function (event) {
            event.remove();
        });
    }
}

function GetResources() {
    var SelectedEmployeeCodes = $('#employee').find('option:selected').val() || null;

    var resources = [];

    if (SelectedEmployeeCodes == null) {
        AllEmployees.forEach(function (item) {
            var resource = {
                id: item.EmployeeCode,
                title: item.FirstName + " " + item.LastName,
            };
            resources.push(resource);
        });

    }
    else {

        var SelectedEmployees = AllEmployees.filter(function (item) {
            return item.EmployeeCode === SelectedEmployeeCodes;
        });

        SelectedEmployees.forEach(function (item) {
            var resource = {
                id: item.EmployeeCode,
                title: item.FirstName + " " + item.LastName,
            };
            resources.push(resource);
        });


    }


    // Set the resources for FullCalendar
    calendar.setOption('resources', resources);
    calendar.render();
    //renderList(resources);
    GetPlannedSchedulebyEmployeeId();
}

function GetPlannedSchedulebyEmployeeId() {
    showSpinner()
    PlannedSchedule = null;
    var EmployeeCode = ($('#employee').find('option:selected').val() != '' && $('#employee').find('option:selected').val() != null && $('#employee').find('option:selected').val() != 0) ? parseInt( $('#employee').find('option:selected').val()) : null;
    var ScheduleFilter = {};
    ScheduleFilter.EmployeeCode = EmployeeCode;
    ScheduleFilter.SiteId = ($('#site').find('option:selected').val() != null && $('#site').find('option:selected').val() != "") ? parseInt($('#site').find('option:selected').val()) : null;

    var fromDate = moment(calendar.view.activeStart).format("yyyy-MM-DDTHH:mm:ss");
    var toDate = moment(calendar.view.activeEnd).format("yyyy-MM-DDTHH:mm:ss");

    var Datefrom = moment(fromDate).format("YYYY-MM-DD");
    var Dateto = moment(toDate).format("YYYY-MM-DD");

    ScheduleFilter.StartDate = Datefrom;
    ScheduleFilter.EndDate = Dateto;


    var url = sitePath + 'api/ScheduleManagementAPI/GetSupervisorsSchedule';

    if (ScheduleFilter != undefined) {

        Ajax.post(url, ScheduleFilter, function (response) {

            if (response.StatusCode == 200 && response.Data.length > 0) {

                PlannedSchedule = response.Data; // Global variable to hold plannedSchedule.

                PopulatePlannedScheduleinCalendar();

            }
            else if (response.StatusCode == 200 && response.Data.length == 0) {

                Swal.fire({
                    text: "Schedule not found for selected employee.",
                    icon: "info",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });

                PopulatePlannedScheduleinCalendar();

            }
            else {
                Swal.fire({
                    text: "Error Occured. Contact with your system administrator.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok",
                    allowOutsideClick: false,
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
                RemoveAllPlannedEventsfromCalendar();
            }
            if (calendar.view.type != 'resourceTimelineDay') {
            }
        });
    }
}

function PopulatePlannedScheduleinCalendar() {
    showSpinner()
    if (PlannedSchedule != null && PlannedSchedule.length > 0)
        PlannedSchedule.forEach(function (item) {
            var PlannedEvents;
            PlannedEvents =
            {
                id: item.RosterId,
                resourceId: item.EmployeeCode,
                resourceName: item.EmployeeName,
                EmployeeName: item.EmployeeName,
                SiteName: item.SiteName,
                title: item.EmployeeName + " | " + item.SiteName + " | " + moment(item.StartDate?.replace('Z', '')).format('HH:mm') + " - " + moment(item.EndDate?.replace('Z', '')).format('HH:mm'),
                start: item.StartDate, // Convert UTC DateTime into Local DateTime.
                end: item.EndDate,
                overlap: true,
                IsPlannedSchedule: true, // true mean already planned scheduled.
                color: '#428bca',
                textColor: 'white',
                display: 'block',
                displayEventTime: false,
                description: "test"

            }

            if (PlannedEvents != undefined) {

                calendar.addEvent(PlannedEvents)
            }
        })

    calendar.render();
    hideSpinner();
}
function EnterFullScreen() {
    const appContent = document.getElementById('kt_app_content_container');
    appContent.classList.toggle('fullscreen');
    document.getElementById("exit-full-screen-row").removeAttribute("style");
    document.getElementById("filters-card-row").style.display = "none";
    document.getElementById("legend-row").style.display = "none";
}

function ExitFullScreen() {
    const appContent = document.getElementById('kt_app_content_container');
    appContent.classList.toggle('fullscreen');
    document.getElementById("exit-full-screen-row").style.display = "none";
    document.getElementById("filters-card-row").removeAttribute("style");
    document.getElementById("legend-row").removeAttribute("style");
}



function printFullCalendar() {
    var calendarDiv = document.getElementById("calendar");

    if (!calendarDiv) {
        alert("Calendar not found!");
        return;
    }

    // Clone the calendar div to avoid modifying the original
    var printContent = calendarDiv.cloneNode(true);

    // Remove buttons from the cloned calendar
    var buttons = printContent.querySelectorAll("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].parentNode.removeChild(buttons[i]);
    }

    // Open a new print window
    var printWindow = window.open("", "", "width=1000,height=700");

    // Copy all stylesheets and inline styles
    var styles = "";
    var styleSheets = document.styleSheets;

    for (var i = 0; i < styleSheets.length; i++) {
        try {
            if (styleSheets[i].href) {
                styles += '<link rel="stylesheet" href="' + styleSheets[i].href + '">';
            } else {
                var cssRules = styleSheets[i].cssRules || styleSheets[i].rules;
                if (cssRules) {
                    styles += "<style>";
                    for (var j = 0; j < cssRules.length; j++) {
                        styles += cssRules[j].cssText;
                    }
                    styles += "</style>";
                }
            }
        } catch (e) {
            console.warn("Could not load style:", styleSheets[i]);
        }
    }

    printWindow.document.write(`
    <html>
      <head>
        <title>Print Calendar</title>
        ${styles} <!-- Include all styles -->
        <style>
          @media print {
            body {
              font-family: Arial, sans-serif;
              padding: 10px;
            }
            .fc-header-toolbar {
              display: none; /* Hide toolbar (buttons like today, prev, next) */
            }
          }
        </style>
      </head>
      <body>
        ${printContent.outerHTML} <!-- Insert cleaned FullCalendar -->
      </body>
    </html>
  `);

    printWindow.document.close();
    printWindow.focus();
    setTimeout(function () {
        printWindow.print();
        printWindow.close();
    }, 1000); // Give time for styles to load
}


function _printCalendarCard() {
    var calendarCard = document.querySelector(".calendar-card");

    if (!calendarCard) {
        alert("Calendar card not found!");
        return;
    }

    // Clone the calendar card to avoid modifying the original
    var printContent = calendarCard.cloneNode(true);

    // Remove buttons from the cloned div
    var buttons = printContent.querySelectorAll("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].parentNode.removeChild(buttons[i]);
    }

    // Create a hidden print-only container
    var printContainer = document.createElement("div");
    printContainer.id = "printContainer";
    printContainer.appendChild(printContent);
    document.body.appendChild(printContainer);

    // Add print styles
    var style = document.createElement("style");
    style.innerHTML = `
    @media print {
      body * {
        visibility: hidden; /* Hide everything */
      }
      #printContainer, #printContainer * {
        visibility: visible; /* Show only the calendar card */
      }
      #printContainer {
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
      }
    }
  `;
    document.head.appendChild(style);

    // Print
    window.print();

    // Cleanup after printing
    document.head.removeChild(style);
    document.body.removeChild(printContainer);
}

function __printCalendarCard() {
    var calendarCard = document.querySelector(".calendar-card");

    if (!calendarCard) {
        alert("Calendar card not found!");
        return;
    }

    // Add print styles dynamically
    var style = document.createElement("style");
    style.innerHTML = `
    @media print {
        div:not(.calendar-card){
            display: none !important;
        }
      .calendar-card button {
        display: none !important; /* Hide buttons inside the calendar card */
      }
    }
  `;
    document.head.appendChild(style);

    // Trigger print (same as Ctrl + P)
    window.print();

    // Remove the print styles after printing
    document.head.removeChild(style);
}
function printCalendarCard() {
    var calendarCard = document.querySelector(".calendar-card");

    if (!calendarCard) {
        alert("Calendar card not found!");
        return;
    }

    // Add print styles dynamically
    var style = document.createElement("style");
    style.innerHTML = `
    @media print {
      @page {
        size: landscape; /* Force landscape mode */
      }

      /* Hide everything initially */
      body * {
        visibility: hidden;
      }

      /* Show only the calendar-card */
      .calendar-card, .calendar-card * {
        visibility: visible;
      }

      /* Move calendar-card to start from the second page */
      .calendar-card {
        position: relative;
        page-break-before: always; /* Push it to the second page */
        width: 100%;
      }

      /* Hide buttons inside the calendar-card */
      .calendar-card button {
        display: none !important;
      }

      /* Hide the first page content */
      body::before {
        content: "";
        display: block;
        height: 100vh; /* Creates a blank first page */
      }
    }
  `;
    document.head.appendChild(style);

    // Trigger print (same as Ctrl + P)
    window.print();

    // Remove the print styles after printing
    document.head.removeChild(style);
}
