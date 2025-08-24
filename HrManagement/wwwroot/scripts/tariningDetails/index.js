

$(document).ready(function () {

    document.querySelectorAll('.dropdown-toggle').forEach(function (dropdownToggleEl) {
        new bootstrap.Dropdown(dropdownToggleEl);
    });

    $('#imageInput').on('change', function (event) {
        event.preventDefault();
        const file = event.target.files[0];
        if (!file) return;

        const allowedExtensions = ['jpg', 'jpeg', 'png', 'gif'];
        const ext = file.name.split('.').pop().toLowerCase();

        if (!allowedExtensions.includes(ext)) {
            alert("Only JPG, JPEG, PNG, and GIF files are allowed.");
            resetImage();
            return;
        }
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#imageThumbnail').attr('src', e.target.result);
        };
        reader.readAsDataURL(file);
    });

    $('#removeImageBtn').on('click', function () {
        resetImage();
    });
    function resetImage() {
        $('#imageInput').val('');
        $('#imageThumbnail').attr('src', placeholderImage);
    }
    hideSpinner();
    if (IsPermissionAloow()) {

        loadTrainingData();
    }
    else {
        loadTrainingDataEmployee();
    }
});


function loadTrainingDataEmployee() {

    const trainingId = $("#detail_trainingId").val();
    const url = sitePath + `api/Trainings/GetTrainingDataEmployee?trainingId=${trainingId}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {

            $("#lbl-heading").text(resp.Data.title);
            trainingDetail(resp);
            trainingOutline(resp);
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

function loadTrainingData() {

    const trainingId = $("#detail_trainingId").val();
    const url = sitePath + `api/Trainings/GetTrainingWithDetails?trainingId=${trainingId}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {

            $("#lbl-heading").text(resp.Data.title);
            trainingDetail(resp);
            trainingOutline(resp);
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

function trainingDetail(resp) {

    if (Array.isArray(resp.Data.FileMediaUplaod) && resp.Data.FileMediaUplaod.length > 0) {
        resp.Data.FileMediaUplaod.forEach(files => {
            if (files.FileType == "TrainingThumbnail") {

                $('#Image-Product-Thumbnail').attr('src', files.FilePath);
                $('#ProductThumbnailId').val(files.FileId);
            }
            if (files.FileType == "ProductLogoImage") {
                $('#Image-ProductLogoImage').attr('src', files.FilePath);
                $('#ProductLogoImageId').val(files.FileId);
            }
            if (files.FileType == "ProductHeadshot") {
                $('#Image-ProductHeadshot').attr('src', files.FilePath);
                $('#ProductHeadshotId').val(files.FileId);
            }
        });
    }



    const details = resp.Data.details && resp.Data.details.length > 0 ? resp.Data.details[0] : null;
    if (details) {

        if (details.title && details.title.trim() !== "") {

            $("#txtTitle").val(details.title);
            $("#lbl-heading").text(details.title);
        }
        else {
            $("#txtTitle").val(resp.Data.title);
        }


        $("#txtTitle").val(details.title);
        $("#txtDescription").val(details.description);
        $("#txtInstructorHeading").val(details.instructorHeading);
        $("#txtInstructorName").val(details.instructorName);
        $("#txtInstructorTitle").val(details.instructorTitle);
        $("#txtInstructorBio").val(details.instructorBio);
        $("#detail_trainingId").val(resp.Data.trainingId);
        $("#TrainingsDetailId").val(details.trainingsDetailId);
    }




}

function trainingOutline(resp) {
    console.log(resp);

    if (resp.Data && Array.isArray(resp.Data.structures) && resp.Data.structures.length > 0) {
        $('#training-outline-container').empty();


        

        const statusMap = {
            1: { label: 'Published', icon: 'bi-check-circle-fill', color: 'success' },
            2: { label: 'Draft', icon: 'bi-clipboard', color: 'primary' },
            3: { label: 'Locked', icon: 'bi-lock-fill', color: 'danger' },
            4: { label: 'Drip', icon: 'bi-clock-fill', color: 'secondary' }
        };

        const getEditUrl = (item) => {
            switch (item.type) {
                case 1: return `/Training/Post_Details/${item.id}`;
                case 2:
                case 3: return `/Training/Tarining_Assesment/${item.id}`;
                case 4: return `/Training/Training_Feedback/${item.id}`;
                default: return "#";
            }
        };

        resp.Data.structures.forEach(function (structure) {
            const structureId = structure.trainingStructureId;
            const title = structure.title || 'Untitled Structure';
            const statusId = structure.status || 2;
            const statusData = statusMap[statusId];
            let catQuizLessonbtn = '';

            const structureButtons = getRoleBasedStructureButtons(structureId, statusMap, statusData)


            const categoriesHtml = (structure.categories || []).map(cat => {
                const catStatus = statusMap[cat.status] || statusMap[2];
                const catEditUrl = getEditUrl(cat);
                let catbtns = '';
                if (cat.type != '2') {
                    catQuizLessonbtn = getRoleBasedQuizLessonButtons(cat, catStatus, catEditUrl);
                }
                else {
                    catbtns = getRoleBasedCatButtons(cat, catStatus, structureId);
                }
                // Render subcategories
                const subCategoriesHtml = (cat.TrainingSubCategories || []).map(sub => {
                    const subStatus = statusMap[sub.status] || statusMap[2];
                    const subEditUrl = getEditUrl(sub);
                    let subCatbtns = getRoleBasedSubCatButtons(sub, subStatus, subEditUrl);

                    return `
    <div class="list-group-item ps-5 d-flex justify-content-between align-items-center bg-light">
        <div class="ms-4"><i class="bi bi-chevron-right me-2 text-muted"></i> ${sub.title}</div>    
        ${subCatbtns}
        
    </div>
    `;
                }).join('');

                if (cat.type == "2") {
                    return `
    <div class="list-group-item d-flex justify-content-between align-items-center">
        <div><i class="bi bi-file-earmark-text me-2"></i> ${cat.title}</div>
        <div class="d-flex align-items-center gap-3">
           ${catbtns}   
        </div>
    </div>
               ${subCategoriesHtml}`;
                }
                else {
                    return `
    <div class="list-group-item d-flex justify-content-between align-items-center">
        <div><i class="bi bi-file-earmark-text me-2"></i>${cat.title}</div>
        
        ${catQuizLessonbtn}
        
      
    </div>
    ${subCategoriesHtml}
    `;
                }

            }).join('') || `<div class="text-muted p-3">No categories available.</div>`;

            const cardHtml = `
    <div class="card mb-3">
        <div class="card-header bg-info text-white d-flex align-items-center">
            <h6 class="mb-0 text-white"><i class="bi bi-folder-fill me-2 text-white"></i> ${title}</h6>
           
              ${structureButtons}
        </div>
        <div id="collapse-${structureId}" class="collapse show">
            <div id="training_category_list_${structureId}" class="list-group list-group-flush">
                ${categoriesHtml}
            </div>
        </div>
    </div>
    `;

            $('#training-outline-container').append(cardHtml);
        });
    } else {
        $('#training-outline-container').html('<p class="text-muted">No structures found.</p>');
    }
}

function tarining_category_list(resp, Id) {

    if (resp.Data && resp.Data.structures.structures && resp.Data.structures.structures.length > 0) {
        $('#training-outline-container').empty();

        const statusMap = {
            1: { label: 'Published', icon: 'bi-check-circle-fill', color: 'success' },
            2: { label: 'Draft', icon: 'bi-clipboard', color: 'primary' },
            3: { label: 'Locked', icon: 'bi-lock-fill', color: 'danger' },
            4: { label: 'Drip', icon: 'bi-clock-fill', color: 'secondary' }
        };

        resp.Data.structures.structures.forEach(function (item) {
            const structureId = item.trainingStructureId;
            const title = item.title || 'Untitled Structure';
            const statusId = item.status || 2; // Default to 2 = Draft
            const statusData = statusMap[statusId] || statusMap[2]; // Fallback to Draft
            console.log('ID:', structureId, 'Status:', statusData.label);

            const cardHtml = `
    <div id="collapse-${structureId}" class="collapse show">
        <div id="training_category_list" class="list-group list-group-flush">
            <div class="list-group-item d-flex justify-content-between align-items-center">
                <div><i class="bi bi-file-earmark-text me-2 text-secondary"></i> ${title}</div>
                <div class="d-flex align-items-center gap-3">
                    <div class="action-icons d-flex align-items-center gap-2">
                        <a href="#" onclick="openLessonQuiz(${cat.type})"><i class="bi bi-eye mx-2"></i></a>
                        <a href="#"><i class="bi bi-pencil mx-2"></i></a>
                    </div>
                    <div class="dropdown status-dropdown">
                        <button class="btn btn-sm dropdown-toggle btn-outline-${statusData.color}" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                            <i class="bi ${statusData.icon} me-1"></i>${statusData.label}</button>
                        <ul class="dropdown-menu" style="">
                            <li><a class="dropdown-item" href="#" data-status="Published" data-icon="bi-check-circle-fill" data-color="success"><i class="bi bi-check-circle-fill me-2 text-success"></i>Published</a></li>
                            <li><a class="dropdown-item" href="#" data-status="Draft" data-icon="bi-clipboard" data-color="primary"><i class="bi bi-clipboard me-2 text-primary"></i>Draft</a></li>
                            <li><a class="dropdown-item" href="#" data-status="Locked" data-icon="bi-lock-fill" data-color="danger"><i class="bi bi-lock-fill me-2 text-danger"></i>Locked</a></li>
                            <li><a class="dropdown-item" href="#" data-status="Drip" data-icon="bi-clock-fill" data-color="secondary"><i class="bi bi-clock-fill me-2 text-secondary"></i>Drip</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;
            $(`#training_category_list_${Id}`).append(cardHtml);
        });
    } else {
        $('#training-outline-container').html('<p class="text-muted">No structures found.</p>');
    }
}

function TrainingStructurCategoryModal(title, type, structureId, subCategoryId = null) {
    console.log(title + type)
    $("#AddTrainingStrutureLesson").text(title);
    $("#trainingtyep_Id").val(type);
    $("#training_structureId").val(structureId);
    $("#subCategoryId").val(subCategoryId);
    const modalEl = document.getElementById('AddLessonModel');
    const modal = new bootstrap.Modal(modalEl); // Create instance
    modal.show();
}

function openLessonQuiz(type, lessonId = null, title = null) {
    if (type == '1') {
        const Id = $("#detail_trainingId").val();
        //window.open(`/TrainingSession/Lesson/${Id}`, '_blank');
        window.open(`/TrainingSession/Lesson?id=${Id}&lessonId=${lessonId}`, '_blank');
    }
    if (type == '3') {
        const Id = $("#detail_trainingId").val();
        //window.open(`/TrainingSession/Assessment/${Id}`, '_blank');
        window.open(`/TrainingSession/Assessment?id=${Id}&lessonId=${lessonId}&title=${title}`, '_blank');
    }
}

function UpdateItemStatus(Id, Status) {

    Swal.fire({
        text: "Are you sure you want to Update Status?",
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
            var url = sitePath + `api/Trainings/UpsertCategoryStatus?Id=${Id}&Status=${Status}`;

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

function UpdatestructureStatus(Id, Status) {

    Swal.fire({
        text: "Are you sure you want to Update Status?",
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
            var url = sitePath + `api/Trainings/UpsertStructureStatus?Id=${Id}&Status=${Status}`;

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

function getRoleBasedSubCatButtons(sub, subStatus, subEditUrl) {

    console.log(sub);

    let btn = '';
    if (IsPermissionAloow()) {
        btn = `<div class="d-flex align-items-center gap-3">
        <a onclick="openLessonQuiz(${sub.type}, ${sub.id}, '${sub.title}')" class="btn btn-sm btn-primary">
            <i class="bi bi-eye p-0" style="color: white !important;"></i>
        </a>

        <a class="btn btn-sm btn-primary" href="${subEditUrl}">
            <i class="bi bi-pencil p-0" style="color: white !important;"></i>
        </a>
        <div class="dropdown status-dropdown">
            <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi ${subStatus.icon} text-${subStatus.color} me-1"></i>${subStatus.label}</button>
            <ul class="dropdown-menu" style="">
                <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${sub.id},'1')" data-status="Published" data-icon="bi-check-circle-fill" data-color="success"><i class="bi bi-check-circle-fill me-2 text-success"></i>Published</a></li>
                <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${sub.id},'2')" data-status="Draft" data-icon="bi-clipboard" data-color="primary"><i class="bi bi-clipboard me-2 text-primary"></i>Draft</a></li>
                <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${sub.id},'3')" data-status="Locked" data-icon="bi-lock-fill" data-color="danger"><i class="bi bi-lock-fill me-2 text-danger"></i>Locked</a></li>
                <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${sub.id},'4')" data-status="Drip" data-icon="bi-clock-fill" data-color="secondary"><i class="bi bi-clock-fill me-2 text-secondary"></i>Drip</a></li>
            </ul>
        </div>
    </div>`;
    }
    else {
        btn = `<div class="d-flex align-items-center gap-3">
        <a onclick="openLessonQuiz(${sub.type}, ${sub.id}, '${sub.title}')" class="btn btn-sm btn-primary">
            <i class="bi bi-eye p-0" style="color: white !important;"></i>
        </a></div>`;
    }
    return btn;
}

function getRoleBasedCatButtons(cat, catStatus, structureId) {

    console.log(cat);

    let btn = '';
    if (IsPermissionAloow()) {
        btn = `<div class="dropdown me-2">
                <button class="btn btn-sm btn-primary dropdown-toggle" data-bs-toggle="dropdown">
                    <i class="bi bi-plus-circle me-1"></i>
                </button>
                <ul class="dropdown-menu">
                    <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Lesson','1','${structureId}')">Add Lesson</a></li>
                     <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Assessment','3','${structureId}')">Add Assessment</a></li>
                </ul>
            </div>
            <div class="dropdown status-dropdown">
                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi ${catStatus.icon} text-${catStatus.color} me-1"></i>${catStatus.label}</button>
                <ul class="dropdown-menu" style="">
                    <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'1')" data-status="Published" data-icon="bi-check-circle-fill" data-color="success"><i class="bi bi-check-circle-fill me-2 text-success"></i>Published</a></li>
                    <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'2')" data-status="Draft" data-icon="bi-clipboard" data-color="primary"><i class="bi bi-clipboard me-2 text-primary"></i>Draft</a></li>
                    <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'3')" data-status="Locked" data-icon="bi-lock-fill" data-color="danger"><i class="bi bi-lock-fill me-2 text-danger"></i>Locked</a></li>
                    <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'4')" data-status="Drip" data-icon="bi-clock-fill" data-color="secondary"><i class="bi bi-clock-fill me-2 text-secondary"></i>Drip</a></li>
                </ul>
            </div>`;
    }
    return btn;
}

function getRoleBasedQuizLessonButtons(cat, catStatus, catEditUrl) {

    console.log(cat);

    let btn = '';
    if (IsPermissionAloow()) {
        btn = `
     <div class="d-flex align-items-center gap-3">
     <a class="btn btn-sm btn-primary" onclick="openLessonQuiz(${cat.type}, ${cat.id}, '${cat.title}')">
         <i class="bi bi-eye p-0" style="color: white !important;"></i>
     </a>

     <a class="btn btn-sm btn-primary" href="${catEditUrl}">
         <i class="bi bi-pencil p-0" style="color: white !important;"></i>
     </a>
     <div class="dropdown status-dropdown">
         <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
             <i class="bi ${catStatus.icon} text-${catStatus.color} me-1"></i>${catStatus.label}</button>
         <ul class="dropdown-menu" style="">
             <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'1')" data-status="Published" data-icon="bi-check-circle-fill" data-color="success"><i class="bi bi-check-circle-fill me-2 text-success"></i>Published</a></li>
             <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'2')" data-status="Draft" data-icon="bi-clipboard" data-color="primary"><i class="bi bi-clipboard me-2 text-primary"></i>Draft</a></li>
             <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'3')" data-status="Locked" data-icon="bi-lock-fill" data-color="danger"><i class="bi bi-lock-fill me-2 text-danger"></i>Locked</a></li>
             <li><a class="dropdown-item" href="#" onclick="UpdateItemStatus(${cat.id},'4')" data-status="Drip" data-icon="bi-clock-fill" data-color="secondary"><i class="bi bi-clock-fill me-2 text-secondary"></i>Drip</a></li>
         </ul>
     </div>
 </div>
    `;
    }
    else {
    
            btn = `<div class="d-flex align-items-center gap-3">
     <a class="btn btn-sm btn-primary" onclick="openLessonQuiz(${cat.type}, ${cat.id}, '${cat.title}')">
         <i class="bi bi-eye p-0" style="color: white !important;"></i>
     </a> </div>`;
    
    }
    return btn;

}



function getRoleBasedStructureButtons(structureId, statusMap, statusData) {


    console.log(statusMap);

    let btn = '';
    if (IsPermissionAloow()) {
        btn = ` <div class="ms-auto d-flex align-items-center gap-2">
     <a href="/Training/Edit_Outline/${structureId}" class="btn btn-sm btn-light border">
         <i class="bi bi-pencil-square p-0"></i>
     </a>
     <div class="dropdown">
         <button class="btn btn-sm btn-light border dropdown-toggle" type="button" data-bs-toggle="dropdown">
             <i class="bi bi-plus-circle p-0"></i>
         </button>
         <ul class="dropdown-menu">
             <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Lesson', '1', ${structureId})">Add Lesson</a></li>
             <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Subcategory', '2', ${structureId})">Add Subcategory</a></li>
             <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Assessment', '3', ${structureId})">Add Assessment</a></li>
             <li><a class="dropdown-item" href="#" onclick="TrainingStructurCategoryModal('Add Credential', '4', ${structureId})">Add Credential</a></li>
         </ul>
     </div>
     <div class="dropdown status-dropdown">
         <button class="btn btn-sm btn-light border dropdown-toggle" type="button" data-bs-toggle="dropdown">
             <i class="bi ${statusData.icon} text-${statusData.color} me-1"></i>
         </button>
         <ul class="dropdown-menu">
             ${Object.entries(statusMap).map(([key, val]) => `
                             <li>
                                 <a class="dropdown-item" href="#" onclick="UpdatestructureStatus(${structureId}, ${key})" data-status="${key}">
                                     <i class="bi ${val.icon} me-2 text-${val.color}"></i>${val.label}
                                 </a>
                             </li>
                         `).join('')}
         </ul>
     </div>
     <i class="bi bi-chevron-down ms-3 text-white" data-bs-toggle="collapse" data-bs-target="#collapse-${structureId}" style="cursor: pointer;"></i>
 </div>
    `;
    }
    return btn;

}


function IsPermissionAloow() {
    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 || UserRoleName.toLowerCase().indexOf('admin') > -1) {
        return true;
    }
    return false;
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

function cloneTraining(Id) {
    Swal.fire({
        text: "Are you sure you want to Clone Training?",
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


            var url = sitePath + `api/Trainings/CloneFullTraining?trainingId=${Id}`;
            Ajax.post(url, null, function (response) {
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: "Training Clone Successfully.",
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