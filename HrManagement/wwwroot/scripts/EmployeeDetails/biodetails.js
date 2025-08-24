var _EmployeeBio = null;


$(document).on("click", ".btn-edit-bio", function () {

    var form = document.querySelector("#bio-details-form");

    if (_EmployeeBio != null) {

        $(form).find("#about").text(_EmployeeBio.About);
        $(form).find("#hobbies").text(_EmployeeBio.Hobbies);
        $(form).find("#favorite-books").text(_EmployeeBio.FavoriteBooks);
        $(form).find("#music-preference").text(_EmployeeBio.MusicPreference);
        $(form).find("#sports").text(_EmployeeBio.Sports);
    }
    $('#modal-bio-details').modal('show');

});




$("#btn-update-bio-details").on("click", function () {
    var form = document.querySelector("#bio-details-form");

    var Bio = {};

    if (_EmployeeBio != null) {
        Bio.Id = _EmployeeBio.Id;
    }
    Bio.EmployeeCode = _EmployeeData.EmployeeCode;
    Bio.about = $(form).find("#about").val();
    Bio.hobbies = $(form).find("#hobbies").val();
    Bio.favoriteBooks = $(form).find("#favorite-books").val();
    Bio.musicPreference = $(form).find("#music-preference").val();
    Bio.sports = $(form).find("#sports").val();

    if (Window.JustConsole) { console.log(Bio); return; }
    var url = sitePath + 'api/EmployeeAPI/UpsertEmployeeBio';

    Ajax.post(url, Bio, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            Swal.fire({
                text: "Employee Bio updated successfully.",
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

});



function LoadBio() {
    var url = sitePath + "api/EmployeeAPI/GetEmployeeBioByEmpCode?EmployeeCode=" + _EmployeeCode;
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            _EmployeeBio = resp.EmployeeBio;
            if (_EmployeeBio != null) {
                $("#user-about").text(_EmployeeBio.About);
                $("#user-hobbies").text(_EmployeeBio.Hobbies);
                $("#user-fav-books").text(_EmployeeBio.FavoriteBooks);
                $("#user-music-pref").text(_EmployeeBio.MusicPreference);
                $("#user-sports").text(_EmployeeBio.Sports);
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


