var SiteId = getParameterByName("Id") || null;
var SiteData = null;
HRMSUtil.onDOMContentLoaded(function () {

    if (SiteId) {
        loadData()
    }

    unspin();
});


function loadData() {
    var url = sitePath + "api/SitesAPI/GetSiteById?Id=" + SiteId;
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            SiteData = resp.Site;
            $("#site-name").val(resp.Site.SiteName);
            $("#site-country").val(resp.Site.CountryId).trigger("change");
            $("#site-timezone").val(resp.Site.TimeZoneId).trigger("change");
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


// These are the constraints used to validate the form
var constraints = {
    "site-name": {
        // Site Name is required
        presence: {
            message: " is required."
        }
    },
    "site-country": {
        // Site Country is required
        presence: {
            message: " is required."
        }
    },
    "site-timezone": {
        // Site Time Zone is required
        presence: { message: " is required." }
    }
};

$("#add-update-site").on("click", function () {
    var form = document.querySelector("#add-update-site-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var site = {};

        site.siteName = $("#site-name").val();
        site.countryId = $("#site-country").val();
        site.countryName = $("#site-country :Selected").text();
        site.timeZoneId = $("#site-timezone").val();
        site.timeZoneName = $("#site-timezone :Selected").text();
        site.timeZoneOffset = $("#site-timezone :Selected").data("offset");

        if (SiteId != null) {
            site.Id = SiteData.Id;
            site.CreatedBy = SiteData.CreatedBy;
            site.CreatedById = SiteData.CreatedById;
            site.CreatedDate = SiteData.CreatedDate;
        }
        //site.countryId

        console.log(site);
        var url = sitePath + 'api/SitesAPI/UpsertSite';

        Ajax.post(url, site, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Site added successfully.",
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
                    window.location.href = "/Home/ManageSites";
                    if (result.isConfirmed) { }
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
