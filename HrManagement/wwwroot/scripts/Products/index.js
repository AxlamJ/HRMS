
$(document).ready(function () {
    hideSpinner();
    PopulateDropDowns();
    curPage = "TrainingDetails";
    loadTrainingData();
    registerEventListners.init();
   
});

function loadTrainingData() {
    const url = sitePath + `api/Trainings/GetTrainingsDetails`;

    Ajax.post(url, null, function (resp) {

        console.log(resp);
        if (Array.isArray(resp.aaData) && resp.aaData.length > 0) {
            filter(resp);
        }
   });
}

function course_detail(TrainingId) {
   
    window.location.href = `/traning/course_detail/${TrainingId}`;
}

function deleteTraining(Id) {
    Swal.fire({
        text: "Are you sure you want to Delete Training?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        buttonsStyling: false,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: "btn btn-secondary"
        },
        allowEscapeKey: false,
        allowOutsideClick: false,
        didOpen: function () {
            hideSpinner();
        }
    }).then((result) => {
        if (result.isConfirmed) {
            var url = sitePath + `api/Trainings/DeleteTraining?trainingId=${Id}`;
            Ajax.post(url, null, function (response) {
                console.log(response);
                if (response.StatusCode === 200) {

                    Swal.fire({
                        text: "Status Updated successfully.",
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
                            location.reload();
                        }
                    }).then(function () {

                    });
                } else {
                    showErrorModal("Error occurred. Please contact your system administrator.");
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
        }
    });
}
function filter(resp) {
    resp.aaData.forEach(data => {
        if (UserRoleName.toLowerCase().indexOf('super admin') > -1 || UserRoleName.toLowerCase().indexOf('admin') > -1) {
                let cardHtml = `<div class="card-advanced col-md-4 col-lg-4 m-4 p-0">
              <div class="card-header m-0 p-0">${data.Title}</div>
              <div class="image-wrapper">
                <img onclick="course_detail(${data.TrainingId})" src="${data.FilePath || 'https://staging.cdn.apisystem.tech/assets/membership/image-placeholder.svg'}"
                  alt="Card Image" style="width: 100%; height: 100%; object-fit: cover;" />

                <div class="overlay-buttons" role="group" aria-label="Card actions">
                  <a class="overlay-buttons-a" href="/traning/course_detail/${data.TrainingId}" data-bs-toggle="tooltip"
                    data-bs-placement="left" title="Edit" aria-label="Edit">
                    <i class="bi bi-pencil me-2 fs-5 text-dark bold-icon"></i>
                  </a>
                  <a onclick="deleteTraining(${data.TrainingId})" class="overlay-buttons-a" data-bs-toggle="tooltip" data-bs-placement="left" title="Delete" aria-label="Delete">
                    <i class="bi bi-trash me-2 fs-5 text-dark bold-icon"></i>
                  </a>
                  <a href="/LearningProgress/Index/${data.TrainingId}" class="overlay-buttons-a text-center py-2 mb-2 fw-bold text-dark" data-bs-toggle="tooltip" data-bs-placement="left" title="Progress" aria-label="Progress">
                  <i class="bi bi-graph-up-arrow me-2 fs-5 text-dark bold-icon"></i>
                  </a>
                </div>
              </div>
              <div class="card-body" style="background:#1f4293;color:white;font-weight:400;">
                <p>
                  ${data.Description || ""}.
                </p>
              </div>
            </div>`

            $('#Tarining-Grid-Card').append(cardHtml);
         }
         
        if (data.VisibleTo == 'all') {
            let cardHtml = `<div class="card-advanced col-md-4 col-lg-4 m-4 p-0">
         <div class="card-header m-0 p-0">${data.Title}</div>
          <div class="image-wrapper">
        <img onclick="course_detail(${data.TrainingId})" src="${data.FilePath || 'https://staging.cdn.apisystem.tech/assets/membership/image-placeholder.svg'}"
         alt="Card Image" style="width: 100%; height: 100%; object-fit: cover;" />

        <div class="overlay-buttons" role="group" aria-label="Card actions">
      <a class="overlay-buttons-a" href="/traning/course_detail/${data.TrainingId}" data-bs-toggle="tooltip"
        data-bs-placement="left" title="Edit" aria-label="Edit">
        <i class="bi bi-eye me-2 fs-5 text-dark bold-icon"></i>
      </a>
    
      <a href="/LearningProgress/Index/${data.TrainingId}" class="overlay-buttons-a text-center py-2 mb-2 fw-bold text-dark" data-bs-toggle="tooltip" data-bs-placement="left" title="Progress" aria-label="Progress">
      <i class="bi bi-graph-up-arrow me-2 fs-5 text-dark bold-icon"></i>
      </a>
    </div>
  </div>
  <div class="card-body" style="background:#1f4293;color:white;font-weight:400;">
    <p>
      ${data.Description || ""}.
    </p>
  </div>
</div>`

            $('#Tarining-Grid-Card').append(cardHtml);

        }
        else if (data.VisibleTo == "departments") {
            var departments = JSON.parse(feed.Departments);

            const departmentids = UserDepartments.map(function (dept) {
                return dept.id;
            });
            var hasMatch = departments.some(function (deptid) {
                return departmentids.includes(parseInt(deptid));
            });
            if (hasMatch) {

                var departmentssubcat = JSON.parse(feed.DepartmentsSubCategories);

                const departmentsubcatids = UserDepartmentSubCategories.map(function (subcat) {
                    return subcat.id;
                });
                var hassubCatMatch = departmentssubcat?.some(function (subcatid) {
                    return departmentsubcatids.includes(parseInt(subcatid));
                });
                if (departmentssubcat == null) {
                    hassubCatMatch = true;
                }

                if (hassubCatMatch) {
                    let cardHtml = `<div class="card-advanced col-md-4 col-lg-4 m-4 p-0">
  <div class="card-header m-0 p-0">${data.Title}</div>
  <div class="image-wrapper">
    <img onclick="course_detail(${data.TrainingId})" src="${data.FilePath || 'https://staging.cdn.apisystem.tech/assets/membership/image-placeholder.svg'}"
      alt="Card Image" style="width: 100%; height: 100%; object-fit: cover;" />

    <div class="overlay-buttons" role="group" aria-label="Card actions">
      <a class="overlay-buttons-a" href="/traning/course_detail/${data.TrainingId}" data-bs-toggle="tooltip"
        data-bs-placement="left" title="Edit" aria-label="Edit">
        <i class="bi bi bi-eye me-2 fs-5 text-dark bold-icon"></i>
      </a>
     
      <a href="/LearningProgress/Index/${data.TrainingId}" class="overlay-buttons-a text-center py-2 mb-2 fw-bold text-dark" data-bs-toggle="tooltip" data-bs-placement="left" title="Progress" aria-label="Progress">
      <i class="bi bi-graph-up-arrow me-2 fs-5 text-dark bold-icon"></i>
      </a>
    </div>
  </div>
  <div class="card-body" style="background:#1f4293;color:white;font-weight:400;">
    <p>
      ${data.Description || ""}.
    </p>
  </div>
</div>`

                    $('#Tarining-Grid-Card').append(cardHtml);

                }
            }
        }
        else if (data.VisibleTo == "sites") {
            var sites = JSON.parse(data.Sites);
            const siteids = UserSites.map(function (site) {
                return site.id;
            });
            var hasMatch = sites.some(function (siteId) {
                return siteids.includes(parseInt(siteId));
            });


            if (hasMatch) {

                let cardHtml = `<div class="card-advanced col-md-4 col-lg-4 m-4 p-0">
  <div class="card-header m-0 p-0">${data.Title}</div>
  <div class="image-wrapper">
    <img onclick="course_detail(${data.TrainingId})" src="${data.FilePath || 'https://staging.cdn.apisystem.tech/assets/membership/image-placeholder.svg'}"
      alt="Card Image" style="width: 100%; height: 100%; object-fit: cover;" />

    <div class="overlay-buttons" role="group" aria-label="Card actions">
      <a class="overlay-buttons-a" href="/traning/course_detail/${data.TrainingId}" data-bs-toggle="tooltip"
        data-bs-placement="left" title="Edit" aria-label="Edit">
        <i class="bi bi bi-eye me-2 fs-5 text-dark bold-icon"></i>
      </a>
     
      <a href="/LearningProgress/Index/${data.TrainingId}" class="overlay-buttons-a text-center py-2 mb-2 fw-bold text-dark" data-bs-toggle="tooltip" data-bs-placement="left" title="Progress" aria-label="Progress">
      <i class="bi bi-graph-up-arrow me-2 fs-5 text-dark bold-icon"></i>
      </a>
    </div>
  </div>
  <div class="card-body" style="background:#1f4293;color:white;font-weight:400;">
    <p>
      ${data.Description || ""}.
    </p>
  </div>
</div>`

                $('#Tarining-Grid-Card').append(cardHtml);
            }
        }
        else if (data.VisibleTo == "employees") {
            var employees = JSON.parse(data.Employees);

            if (employees.indexOf(EmployeeCode) > -1) {
                let cardHtml = `<div class="card-advanced col-md-4 col-lg-4 m-4 p-0">
  <div class="card-header m-0 p-0">${data.Title}</div>
  <div class="image-wrapper">
    <img onclick="course_detail(${data.TrainingId})" src="${data.FilePath || 'https://staging.cdn.apisystem.tech/assets/membership/image-placeholder.svg'}"
      alt="Card Image" style="width: 100%; height: 100%; object-fit: cover;" />

    <div class="overlay-buttons" role="group" aria-label="Card actions">
      <a class="overlay-buttons-a" href="/traning/course_detail/${data.TrainingId}" data-bs-toggle="tooltip"
        data-bs-placement="left" title="Edit" aria-label="Edit">
        <i class="bi bi bi-eye me-2 fs-5 text-dark bold-icon"></i>
      </a>
     
      <a href="/LearningProgress/Index/${data.TrainingId}" class="overlay-buttons-a text-center py-2 mb-2 fw-bold text-dark" data-bs-toggle="tooltip" data-bs-placement="left" title="Progress" aria-label="Progress">
      <i class="bi bi-graph-up-arrow me-2 fs-5 text-dark bold-icon"></i>
      </a>
    </div>
  </div>
  <div class="card-body" style="background:#1f4293;color:white;font-weight:400;">
    <p>
      ${data.Description || ""}.
    </p>
  </div>
</div>`;
                $('#Tarining-Grid-Card').append(cardHtml);
            }
        }
    });
}
function loadData() {
    var url = sitePath + "api/TrainingAssignAPI/GetTrainingAssignment?Id=" + Id;
    Ajax.post(url, null, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            data = resp.Data;
            $("#visible-to").val(data.VisibleTo).trigger('change');
            if (data.VisibleTo == "departments") {
                var depts = JSON.parse(data.Departments);
                $("#departments").val(depts).trigger('change');

                if (data.DepartmentsSubCategories != '' && data.DepartmentsSubCategories != null) {
                    var DepartmentsSubCategories = JSON.parse(data.DepartmentsSubCategories);
                    $("#department-sub-category").val(DepartmentsSubCategories).trigger('change');

                }
            }
            else if (data.VisibleTo == "sites") {
                var sites = JSON.parse(data.Sites);

                $("#sites").val(sites).trigger('change');

            }
            else if (data.VisibleTo == "employees") {
                var employees = JSON.parse(data.Employees);

                console.log(employees);

                $("#employees").val(employees).trigger('change');

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

////////////// Populate Dropdown

function PopulateDropDowns(cb=null) {
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


///department change

$(document).on('change', '#departments', function (e) {
    e.preventDefault();

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