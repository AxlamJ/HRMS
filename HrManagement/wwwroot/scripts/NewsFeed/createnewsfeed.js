var NewsId = getParameterByName("Id") || null;
var NewsData = null;

HRMSUtil.onDOMContentLoaded(function () {


    //InitializeDatePickers.init();
    registerEventListners.init();
    PopulateDropDowns(function () {
        if (NewsId) {
            loadData();
        }
    });

    $('#content').summernote({
        height: 200,
        minHeight: 200,
        maxHeight: 200,
        toolbar: [
            // [groupName, [list of button]]
            ['insert', ['link', 'hr']],
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']]
        ]
    });
    //Setting by default font size of summernote editable area to 14
    $('.note-editable').css('font-size', '14px');

    unspin();
});

function loadData() {
    var url = sitePath + "api/NewsFeedAPI/GetNewsbyId?Id=" + NewsId;
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            NewsData = resp.News;

            var photoUrl = (NewsData.ImageUrl == "" || NewsData.ImageUrl == null) ? "/Images/no-image-1.svg" : NewsData.ImageUrl;
            var youtubeUrl = (NewsData.YoutubeUrl == "" || NewsData.YoutubeUrl == null) ? "" : NewsData.YoutubeUrl;

            $("#news-photo").attr('src', photoUrl);
            $("#title").val(NewsData.NewsTitle);
            $("#content").summernote("code", NewsData.Content);
            $("#visible-to").val(NewsData.VisibleTo).trigger('change');
            $("#youtube-url").val(NewsData.YoutubeUrl);
            if (NewsData.VisibleTo == "departments") {
                var depts = JSON.parse(NewsData.Departments);
                $("#departments").val(depts).trigger('change');

                if (NewsData.DepartmentsSubCategories != '' && NewsData.DepartmentsSubCategories != null) {
                    var DepartmentsSubCategories = JSON.parse(NewsData.DepartmentsSubCategories);
                    $("#department-sub-category").val(DepartmentsSubCategories).trigger('change');

                }
            }
            else if (NewsData.VisibleTo == "sites") {
                var sites = JSON.parse(NewsData.Sites);

                $("#sites").val(sites).trigger('change');

            }
            else if (NewsData.VisibleTo == "employees") {
                var employees = JSON.parse(NewsData.Employees);

                $("#sites").val(employees).trigger('change');

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
function PopulateDropDowns(cb) {


    var ddDepartments = document.querySelector('#departments');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });


    var ddSites = document.querySelector('#sites');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });


    var ddEmployees = document.querySelector('#employees');
    $(ddEmployees).empty();
    var lstEmployees = (DropDownsData.Employees || []);
    var option = new Option();
    ddEmployees.appendChild(option);
    //var option = new Option("All", -1, false, false);
    //ddEmployees.appendChild(option);
    lstEmployees.forEach(function (item) {

        var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
        ddEmployees.appendChild(option);
    });


    //var ddDepartmentSubCategory = document.querySelector('#department-sub-category');
    //$(ddDepartmentSubCategory).empty();

    //var lstDepartmentSubCategories = (DropDownsData.DepartmentSubCategories || []);
    //var option = new Option(); // You may want to add a label like "--Select--"
    //ddDepartmentSubCategory.appendChild(option);

    //lstDepartmentSubCategories.forEach(function (item) {
    //    var option = new Option(item.SubCategoryName, item.Id, false, false);
    //    ddDepartmentSubCategory.appendChild(option);
    //});

    if (cb) {
        cb();
    }


}



$(document).on('change', '#departments', function () {
    var selectedDepartments = $("#departments").val(); // This is an array if multiple selected

    if (selectedDepartments && selectedDepartments.length > 0) {
        var deptCategories = DropDownsData?.DepartmentSubCategories?.filter(function (item) {
            return selectedDepartments.includes(item.DepartmentId.toString()); // convert if needed
        });

        var ddDepartmentSubCategory = document.querySelector('#department-sub-category');
        $(ddDepartmentSubCategory).empty();

        var lstDepartmentSubCategories = (deptCategories || []);
        var option = new Option(); // You may want to add a label like "--Select--"
        ddDepartmentSubCategory.appendChild(option);

        lstDepartmentSubCategories.forEach(function (item) {
            var option = new Option(item.SubCategoryName, item.Id, false, false);
            ddDepartmentSubCategory.appendChild(option);
        });
    }
});

var registerEventListners = function () {

    return {
        init: function () {
            $(document).on("click", '.btn-upload-photo', function () {
                $(this).siblings('.news-feed-photo').click();
            });


            $(document).on('change', '.news-feed-photo', handleImageUpload);
            //$(document).on("click", ".btn-upload-photo", onbtnUploadClick);

            $(document).on("click", '.btn-del-image', function () {
                var image = $(this).closest('.photo-ctrl').find('.news-photo');
                image[0].src = '/Images/no-image.svg';
                hideImageEditButtons(image);
            });

            $(document).on('change', '#visible-to', function () {
                var $self = $(this);
                var selectedOption = $(this).val();

                if (selectedOption == "departments") {
                    $("#departments-dd").removeClass('d-none');
                    $("#department-sub-category-dd").removeClass('d-none');
                    //$("#department-sub-category-dd").select2();
                    $("#employees-dd").addClass('d-none');
                    $("#sites-dd").addClass('d-none');

                    constraints.departments = { presence: { message: "is required.", allowEmpty: false } };
                    constraints["department-sub-category"] = { presence: { message: "is required.", allowEmpty: false } };
                }
                else if (selectedOption == "sites") {
                    $("#sites-dd").removeClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#department-sub-category-dd").addClass('d-none');
                    $("#employees-dd").addClass('d-none');

                    constraints.sites = { presence: { message: "is required.", allowEmpty: false } };
                }
                else if (selectedOption == "employees") {
                    $("#employees-dd").removeClass('d-none');
                    $("#sites-dd").addClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#department-sub-category-dd").addClass('d-none');

                    constraints.employees = { presence: { message: "is required.", allowEmpty: false } };
                }
                else {

                    delete constraints.departments;
                    delete constraints.sites;
                    delete constraints.employees;

                    $("#sites-dd").addClass('d-none');
                    $("#departments-dd").addClass('d-none');
                    $("#employees-dd").addClass('d-none');
                }
            })
        }
    };

}();


// These are the constraints used to validate the form
var constraints = {
    "title": {
        presence: {
            message: "is required."
        }
    },
    "content": {
        presence: {
            message: "is required."
        }
    },
    "visible-to": {
        presence: {
            message: "is required."
        }
    }
};




$("#add-news-feed").on("click", function () {
    var form = document.querySelector("#add-news-form");
    var errors = validate(form, constraints);
    showErrors(form, errors || {});
    if (!errors) {
        var news = {};

        news.ImageUrl = $("#news-photo").attr('src') == "/Images/no-image-1.svg" ? "" : $("#news-photo").attr('src');
        news.newsTitle = $("#title").val();
        news.youtubeUrl = $("#youtube-url").val();
        news.content = $("#content").val();
        news.visibleTo = $("#visible-to").val();
        news.departments = ($("#departments").val()).length > 0 ? JSON.stringify($("#departments").val()) : null;
        news.departmentsSubCategories = ($("#department-sub-category").val()).length > 0 ? JSON.stringify($("#department-sub-category").val()) : null;
        news.employees = ($("#employees").val()).length > 0 ? JSON.stringify($("#employees").val()) : null;
        news.sites = ($("#sites").val()).length > 0 ? JSON.stringify($("#sites").val()) : null;

        if (NewsId != null && NewsId != '') {
            news.Id = NewsId;
        }
        if (Window.JustConsole) { console.log(news); return; }
        var url = sitePath + 'api/NewsFeedAPI/UpsertNewsFeed';

        Ajax.post(url, news, function (response) { // Ensure you use the correct data (exportRequestBody)
            //hideSpinner();

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "News created successfully.",
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

                        window.location.href = "/NewsFeed/NewsFeedManagement";
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







function handleImageUpload(event) {
    var imageFile = event.target.files[0];
    if (imageFile.type.indexOf("image") < 0) {
        ShowError("Only image files are allowed");
        event.target.value = '';
        return;
    }
    var img = event.target.parentElement.parentElement.parentElement.querySelector('.news-photo');
    $(img).data('is-new', true)
    var options = {
        maxSizeMB: 1,
        maxWidthOrHeight: 600
    };
    imageCompression(imageFile, options)
        .then(function (compressedFile) {//compression, Promise resolved.
            var imageName = "img_" + Date.now() + "." + compressedFile.type.split("/").pop();
            imageCompression.getDataUrlFromFile(compressedFile)
                .then(function (dataURLCompressed) {//convert from blob to base64, Promise resolved.
                    //saveImageToAzureBlob(dataURLCompressed, imageName, img);
                    //saveImageToAzureBlob(dataURLCompressed, imageName, img);

                    UploadImage(dataURLCompressed, imageName, img)
                    //showImage(dataURLCompressed, img);

                })
        })
        .catch(function (error) {//compression, Promise rejected.
            //ShowError(error.message);
            console.log(error, "104");
        });
    $(event.target).val("");
}

function UploadImage(dataURL, imageName, imgElement) {

    var url = sitePath + 'api/NewsFeedAPI/UploadImage';
    var Image = { base64: dataURL, fileName: imageName };

    Ajax.post(url, Image, function (response) { // Ensure you use the correct data (exportRequestBody)
        //hideSpinner();

        if (response.StatusCode == 200) {
            showImage(response.imageUrl, imgElement);

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

function showImage(dataURL, imgElement) {
    var loader = getImageLoader();
    $(imgElement).closest('.photo-ctrl').prepend($(loader).fadeIn());

    imgElement.src = dataURL;
    imgElement.onload = function () {
        removeImageLoader(loader);
        imgElement.onload = null;
        showImageEditButtons(imgElement);
    };
}

function showImageEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-btns').show();
    $(img).closest('.photo-ctrl').find('.btn-del-image').show();
    $(img).closest('.photo-ctrl').find('.image-select-buttons').hide();
}

function hideImageEditButtons(img) {
    $(img).closest('.photo-ctrl').find('.edit-btns').hide();
    $(img).closest('.photo-ctrl').find('.image-select-buttons').show();
}

function getImageLoader() {

    var loaderContainer = document.createElement('div');
    var loader = document.createElement('div');
    var loaderChild = document.createElement('div');

    loaderContainer.className = 'loader-container';
    loader.className = 'loader';
    loaderChild.className = 'loader-child';

    loader.appendChild(loaderChild);
    loaderContainer.appendChild(loader);

    return loaderContainer;

}

function removeImageLoader(loader) {
    $(loader).remove();
}