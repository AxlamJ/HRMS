
    $(document).ready(function () {
    
        

        if (IsPermissionAloow()) {

            loadSessionData();
        }
        else {
            loadSessionDataEmployee();
        }

        hideSpinner();
    });

function loadSessionDataEmployee() {

    const id = $("#id").val();
    const url = sitePath + `api/TrainingSessions/EmployeeWatchLesson?id=${id}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {

            console.log(resp);
            getTrainingDetail(resp.Data.TrainingId);
            renderTrainingStructure(resp.Data);

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




function loadSessionData() {

    const id = $("#id").val();
    const url = sitePath + `api/TrainingSessions/WatchLesson?id=${id}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {

            console.log(resp);
            getTrainingDetail(resp.Data.TrainingId);
            renderTrainingStructure(resp.Data);

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


function renderTrainingStructure(data) {

    var titleHtml = `<h3>${data.TrainingTitle}</h3>`;
    $('#trainingTitle').append(titleHtml);
    const container = $("#training-container");
    container.empty();

    data.Structures.forEach((structure, index) => {
        const collapseId = `collapse-${index}`;

        const header = `
            <div class="mb-2">
                <button class="cat-btn btn w-100 text-start d-flex justify-content-between align-items-center"
                        type="button"
                        data-target="#${collapseId}"
                        aria-expanded="false"
                        aria-controls="${collapseId}">
                    <div class="d-flex align-items-center gap-2">
                        <i class="bi bi-folder2 text-danger" style="font-size:1.1rem"></i>
                        <span>${structure.StructureTitle}</span>
                    </div>
                    <i class="bi bi-chevron-down"></i>
                </button>


               <div class="collapse" id="${collapseId}">
  <div class="ps-3 pt-3">
    <ul class="list-group">
     ${structure.Categories
            .filter(category => category.CategoryType === "1" || category.CategoryType=='2')
                .map(category => `
  <li class="list-group-item mb-2 rounded shadow-sm border-0 d-flex justify-content-between align-items-center category-item" role="listitem" tabindex="0" onclick="openLessonDetail(${category.CategoryId},${category.CategoryType},'${category.CategoryTitle}')">
    <div class="d-flex align-items-center gap-3">
      ${getCategoryIcon(category.CategoryType)} <!-- Use the function here -->
      <span class="fw-semibold text-truncate category-title" title="${category.CategoryTitle}">${category.CategoryTitle}</span>
    </div>
    ${category.TrainingSubCategories?.length ? `
      <button class="btn btn-sm btn-outline-primary toggle-subcat-btn" aria-label="Toggle subcategories" aria-expanded="false" aria-controls="subcat-${category.CategoryId}">
        <i class="bi bi-chevron-down"></i>
      </button>
    ` : ''}
  </li>
  ${category.TrainingSubCategories?.length ? `
    <ul class="list-group ps-5 mb-3 collapse subcategory-list" id="subcat-${category.CategoryId}" role="group" aria-label="Subcategories">
      ${category.TrainingSubCategories
                        .filter(sub => sub.CategoryType === "1")
                        .map(sub => `
        <li class="list-group-item mb-2 rounded shadow-sm border-0 d-flex justify-content-between align-items-center gap-2 category-item" role="listitem" tabindex="0" onclick="openLessonDetail(${sub.CategoryId},${sub.CategoryType},'${sub.CategoryTitle}')">
          ${getCategoryIcon(sub.CategoryType)}
          <span class="text-muted text-truncate" title="${sub.CategoryTitle}">${sub.CategoryTitle}</span>
        </li>
      `).join('')}
    </ul>
  ` : ''}
`).join('')}

    </ul>
  </div>
</div>


            </div>
        `;

        container.append(header);
    });

    const firstCategory = data.Structures
        .map(s => s.Categories)
        .filter(cats => cats && cats.length > 0)
        .flat()
        .find(cat => cat.CategoryType == "1");
    console.log(firstCategory);


    let LessonId = $("#lessonId").val();
    openLessonDetail(LessonId, "1", null);
  
}

$(document).on('click', '.cat-btn', function () {
    const targetId = $(this).data('target');
    const $target = $(targetId);

    if ($target.hasClass('show')) {
        $target.removeClass('show').slideUp(200);
        $(this).attr('aria-expanded', 'false');
    } else {
        $target.addClass('show').slideDown(200);
        $(this).attr('aria-expanded', 'true');
    }
});

$(document).on('click', '.toggle-subcat-btn', function () {
    const $btn = $(this);
    const targetId = $btn.attr('aria-controls');
    const $target = $('#' + targetId);

    if (!$target.length) return;

    $target.collapse('toggle');

    const expanded = $btn.attr('aria-expanded') === 'true';
    $btn.attr('aria-expanded', !expanded);

    $btn.find('i').toggleClass('bi-chevron-down bi-chevron-up');
});


function openLessonDetail(id, Type, title) {
    if (Type == '1') {
        getLessonVideo(id, title);
      

        $('#divMarkComplete').empty();
        loadComments(id);
        $('#comments-section').empty();
        $('#comments-section').append(` <button  type="button" class="btn btn-primary" onclick="WriteComments(${id})" ><i class="bi bi-send-fill"></i></button>`);

        $('#divMarkComplete').append(`<button class="btn btn-outline-success" onclick="lessonMarkComplet(${id})"><i class="bi bi-check-circle-fill"></i> Mark it Complete</button>`);
           }
    if (Type == '2') {
    }  
    if (Type == '3') {
        Swal.fire({
            text: "Are you sure you want to Attempt Quiz?",
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
                window.open(`/TrainingSession/Quiz/${id}`, '_blank');
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }
}


// Optional: Handle click on .video-item
function playVideo(url) {
    const videoItem = e.target.closest(".video-item");
    if (videoItem) {
        const videoSrc = videoItem.dataset.src;
        const title = videoItem.dataset.title;
        const desc = videoItem.dataset.desc;

        // Do something with this (like open a modal or play video)
        console.log("Video clicked:", title, videoSrc);
    }
}

function getCategoryIcon(type) {

    console.log(type);
    switch (type) {
        case '1': // Video lesson
            return '<i class="bi bi-play-circle-fill text-danger fs-5"></i>';
        case '3': // Quiz/Test
            return '<i class="bi bi-question-circle-fill text-warning fs-5"></i>';
        case '2': // Folder / subcategory container
            return '<i class="bi bi-folder2-open-fill text-primary fs-4"></i>';
        default:
            return '<i class="bi bi-file-earmark-text text-secondary fs-5"></i>';
    }
}

function getLessonVideo(id, videoTitle) {
        const url = sitePath + `api/Trainings/GetCategoryWithPosts?id=${id}`;

        Ajax.post(url, null, function (resp) {
            if (resp.StatusCode == 200) {
                console.log(resp);
                $('#lessonTitle').empty();
                $('#videoDesc').empty();
                $('#lessonTitle').append(`<h3>${resp.Data.title}</h3>`);
                $('#videoDesc').append(`<h3>${videoTitle}</h3>`);
                 playVideo(resp);
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
function playVideo(resp) {

    if (Array.isArray(resp.Data.FileMediaUplaod) && resp.Data.FileMediaUplaod.length > 0) {
        let pdfFilesArray = [];
        resp.Data.FileMediaUplaod.forEach(files => {
            
            if (files.FileType == "LessonVideo") {
                $("#UploadVideoAudioSection").addClass("d-none")
                $("#mainVideo").removeClass("d-none")
                $('#LessonVideoId').val(files.FileId);
                url = sitePath + `api/video/${files.FileName}`;
                var videoType = 'video/mp4';
                $('#videoSource').attr('src', url);
                $('#mainVideo').attr('type', videoType);
                $('#mainVideo')[0].load();
            }
            if (files.FileType == "VideoThumbnail") {
                $('#mainVideo').attr('poster', files.FilePath)[0].load();
                $('#VideoThumbnailId').val(files.FileId);
                $('#videoThumbnailPreview_Img').attr('src', files.FilePath);
            }

            if (files.FileType == "LessonPdf") {
                pdfFilesArray.push(files);
            }
        });
        if (Array.isArray(pdfFilesArray) && pdfFilesArray.length > 0) {
            renderPdfTable(pdfFilesArray);
        }
    }
}

function lessonMarkComplet(id) {


    var lesson = {
        "LessonID": id,
    }




    Swal.fire({
        text: "Are you sure you completed Lesson?",
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
            var url = sitePath + 'api/LessonQuizAttempt/UpsertUserLessonProgress';

            Ajax.post(url, lesson, function (response) {
                console.log(response);
                if (response.StatusCode === 200) {
                    Swal.fire({
                        text: "you have successfully completed watching the lesson.",
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
function getTrainingDetail(id) {
    const url = sitePath + `api/TrainingSessions/GetTrainingDetails?trainingId=${id}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            console.log(resp.Data);

            let detail = resp.Data.details[0];
            $("#TrainingInstructorDetails").empty();
            $("#TrainingInstructorDetails").append(`
                      <div class="col-md-4 d-flex justify-content-center align-items-center p-4">
                          <img id="ProductLogoImage" src="https://via.placeholder.com/120" class="img-fluid rounded-circle border border-3" alt="Instructor Logo" style="width:120px;height:120px; object-fit:cover;">
                      </div>
                      <div class="col-md-8">
                         <div class="card-body py-4 px-3" style="font-size: 18px;">
    <h6 class="card-title text-primary fw-bold mb-2 pb-3" style="font-size: 1.5rem;">
        ${detail.instructorHeading}
    </h6>
    <p class="mb-1 p-2" style="font-size: 1.2rem;">
        <i class="bi bi-person-fill text-muted"></i>
        <strong class="m-1">Name:</strong>
        <span class="m-1">${detail.instructorName}</span>
    </p>
    <p class="mb-1 p-2" style="font-size: 1.2rem;">
        <i class="bi bi-briefcase-fill text-muted me-2"></i>
        <strong>Title:</strong> ${detail.instructorTitle}
    </p>
    <p class="mb-0 p-2" style="font-size: 1.2rem;">
        <i class="bi bi-journal-text text-muted me-2"></i>
        <strong>Bio:</strong> ${detail.instructorBio}.
    </p>
</div>

                      </div>

            `)

            if (resp.Data.FileMediaUplaod && resp.Data.FileMediaUplaod.length > 0) {
                resp.Data.FileMediaUplaod.forEach(item => {
                    if (item.FileType == "ProductLogoImage") {
                        $("#ProductLogoImage").empty();
                        $("#ProductLogoImage").attr('src', item.FilePath);
                    }
                });
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

function WriteComments(id) {
    hideSpinner();
    let CommentsData = {
        //"CommentId": 1,
        "CategoryId": id,
        "CommentText": $('#chatInput').val(),

    },

        url = sitePath + 'api/Chat/WriteComments';
    Ajax.post(url, CommentsData, function (response) {
        console.log(response);
        if (response.StatusCode === 200) {
            $('#chatInput').val('');
            loadComments(id);
        } else {
            showErrorModal("Error occurred. Please contact your system administrator.");
        }
    });
}


function loadComments(id) {
    hideSpinner();
    const url = sitePath + `api/Chat/loadUserComments?Id=${id}`;
    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            console.log(resp);
            const container = $('#chatMessages');
            container.empty();
            resp.Data.forEach(comment => {

                const profileImage = '/profile-photos/img_1740304073560.png';
                const userName = comment.UserName || `User ${comment.UserId}`;
                const createdAt = new Date(comment.CreatedAt).toLocaleString();
                const messageHtml = `
                        <div class="mb-2">
                            <div class="bg-light border rounded p-2">
                                <div class="fw-semibold">${comment.CreatedBy}</div>
                                <div>${comment.CommentText}</div>
                                <div class="text-muted small">${new Date(comment.CreatedAt).toLocaleString()}</div>
                            </div>
                        </div>`;
                container.append(messageHtml);
            });
            container.scrollTop(container[0].scrollHeight);
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

function renderPdfTable(uploadedPdfData) {
    console.log(uploadedPdfData);

    let $tableBody = $('#pdfTableBody');
    $tableBody.empty();

    if (!uploadedPdfData || uploadedPdfData.length === 0) {
        $('#pdfTable').hide();
        return;
    }

    uploadedPdfData.forEach((pdf, index) => {
        const row = `

            
                  <h5 class="card-title">
                   <a href="${pdf.FilePath}" target="_blank" class="text-decoration-none">
                     📄 ${pdf.FileTitle}
                        </a>
                        </h5>
                        <p class="card-text">
                    ${pdf.FileDescription}
                     </p>
                
        `;
        $('#pdf-section').append(row);
    });
}

function IsPermissionAloow() {
    if (UserRoleName.toLowerCase().indexOf('super admin') > -1 || UserRoleName.toLowerCase().indexOf('admin') > -1) {
        return true;
    }
    return false;
}