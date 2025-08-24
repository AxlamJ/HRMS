
HRMSUtil.onDOMContentLoaded(function () {

    GetBirthdayList();
    hideSpinner()
});


$(document).on('click', '#btnSearch', function () {
    GetBirthdayList();
})
function GetBirthdayList() {

    var firstname = $("#firstname").val() == '' ? '' : $("#firstname").val();
    var lastname = $("#lastname").val() == '' ? '' : $("#lastname").val();
    var month = $("#month").val() == '' ? 0 : parseInt($("#month").val());
    var url = sitePath + "api/StatsAPI/GetBirthDayList?FirstName="+firstname+"&LastName="+lastname+"&Month="+month;
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var BirthdatList = resp.BirthDayList;

            ShowStats(BirthdatList);
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


function ShowStats(BirthdatList) {

    var birthdayRow = document.getElementById("birthdaylist");
    $(birthdayRow).empty();
    for (var i = 0; i < BirthdatList.length; i++) {

        var templateHtml = $("#birthday-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#birth_day").text(BirthdatList[i].EmployeeName);
        $template.find("#name").text(FormatDate( BirthdatList[i].EventDate));

        // Append to container
        $(birthdayRow).append($template);
        $(birthdayRow).append('</br>');
    }
}