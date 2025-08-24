

$(document).ready(function () {

    hideSpinner();

    loadLessonData();


    $('.toggle-table').on('click', function () {
        const $icon = $(this);
        const $tbody = $('#pdfTableBody');
        $tbody.toggle(); 
        if ($icon.hasClass('bi-chevron-up')) {
            $icon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
        } else {
            $icon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
        }
    });

});
function loadLessonData() {
 
    const id = $("#trainingcategory_Id").val();
    const url = sitePath + `api/Trainings/GetCategoryWithPosts?id=${id}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            $("#Post-Heading").text(resp.Data.title);
            postDeatil(resp);

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

function postDeatil(resp) {

    console.log(resp);
    descriptionQuill = new Quill('#post_detail_description_editor', {
        theme: 'snow'
    });

    $("#trainingcategory_Id").val(resp.Data.id);
    let Post = resp?.Data?.posts?.[0] || null;

    console.log(Post);
    if (Post != null) {
        $("#Id").val(Post.id);
        $("#post-detail-title-input").val(Post.title);
        $(`input[name="post_detail_status_radio"][value="${Post.status}"]`).prop('checked', true);
        descriptionQuill.root.innerHTML = Post.description;    
    }
    if (Array.isArray(resp.Data.FileMediaUplaod) && resp.Data.FileMediaUplaod.length > 0) {
        let pdfFilesArray = [];
        resp.Data.FileMediaUplaod.forEach(files => {
            if (files.FileType == "LessonThumbnail") {
                $('#LessonThumbnailId').val(files.FileId);
                $('#Preview-image-Thumbnail').attr('src', files.FilePath);
            }
            if (files.FileType == "LessonVideo") {
                $("#UploadVideoAudioSection").addClass("d-none")
                $("#VideoPlayer").removeClass("d-none")
                $('#LessonVideoId').val(files.FileId);
                url = sitePath + `api/video/${files.FileName}`;
                var videoType = 'video/mp4';
                $('#videoSource').attr('src', url);
                $('#videoSource').attr('type', videoType);
                $('#videoPlayer')[0].load();
            } 
            if (files.FileType == "VideoThumbnail") {
                $('#videoPlayer').attr('poster', files.FilePath)[0].load();
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
///Image Upload
const dropArea = document.getElementById('dropArea');
const PostimageThumbnail = document.getElementById('Post-image-Thumbnail');
const imagePreview = document.getElementById('imagePreview');
const previewContainer = document.getElementById('previewContainer');
const removeImageBtn = document.getElementById('removeImageBtn');
const uploadBtn = document.getElementById('uploadBtn');

// Click on drop area triggers file input
dropArea.addEventListener('click', () => PostimageThumbnail.click());

// Keyboard accessibility: Enter or Space triggers file input
dropArea.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        PostimageThumbnail.click();
    }
});

// Handle file input change
PostimageThumbnail.addEventListener('change', e => {
    const file = e.target.files[0];
    handleFile(file);
});

// Drag & Drop handlers
dropArea.addEventListener('dragover', e => {
    e.preventDefault();
    dropArea.classList.add('dragover');
});

dropArea.addEventListener('dragleave', e => {
    e.preventDefault();
    dropArea.classList.remove('dragover');
});

dropArea.addEventListener('drop', e => {
    e.preventDefault();
    dropArea.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handleFile(file);
});

function handleFile(file) {
    if (!file) return;

    // Validate file type (image only)
    if (!file.type.startsWith('image/')) {
        alert('Only image files are allowed!');
        resetPreview();
        return;
    }

    // Preview the image
    const reader = new FileReader();
    reader.onload = e => {
        imagePreview.src = e.target.result;
        previewContainer.style.display = 'block';
        uploadBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

removeImageBtn.addEventListener('click', () => {
    resetPreview();
    PostimageThumbnail.value = '';
    uploadBtn.disabled = true;
});

function resetPreview() {
    previewContainer.style.display = 'none';
    imagePreview.src = '';
}

uploadBtn.addEventListener('click', () => {
    const file = PostimageThumbnail.files[0];
    if (!file) return alert('No file selected!');
    const formData = new FormData();

    formData.append('Id', $('#LessonThumbnailId').val() || 0);
    formData.append('ReferenceId', $('#trainingcategory_Id').val() || 0);
    formData.append('ModuleName', "2");
    formData.append('FolderName', "images/PostDetails");
    formData.append('Type', "LessonThumbnail");
    const imageFile = $('#Post-image-Thumbnail')[0].files[0];
    if (imageFile) {
        formData.append('File', imageFile);
        let ThumbnailPreviewId = "Preview-image-Thumbnail";
        let LessonThumbnailId = "LessonThumbnailId";
        fileUpload(formData, ThumbnailPreviewId, LessonThumbnailId);

    }
    const modal = bootstrap.Modal.getInstance(document.getElementById('imageUploadModal'));
    modal.hide();

    // Reset after upload
    resetPreview();
    PostimageThumbnail.value = '';
    uploadBtn.disabled = true;
});


function fileUpload(formData, PreviewId, Id) {
    var url = sitePath + 'api/Trainings/UploadMedia';
    const token = localStorage.getItem('jwtToken');
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'Authorization': `Bearer ${token}`
        },
        beforeSend: function () {
            Ajax.enqueueSpinner();
        },
        success: function (res) {

            if (res && res.Data != null) {
                console.log(res);
                $('#' + PreviewId).attr('src', res.Data.FilePath);
                $('#' + Id).val(res.Data.Id);
                $('#LessonThumbnailReplaceBtn').removeClass('d-none');
                $('#LessonThumbnailRemoveBtn').removeClass('d-none');
                $('#LessonThumbnailUploadBtn').addClass('d-none');
            }
           

            swal.fire({
                text: "File added successfully.",
                icon: "success",
                buttonsstyling: false,
                confirmbuttontext: "ok",
                allowescapekey: false,
                allowoutsideclick: false,
                customclass: {
                    confirmbutton: "btn btn-primary",
                },
                didopen: function () {
                    hideSpinner();
                }
            }).then(function () {
                let catId = $('#trainingcategory_Id').val();
                window.location = `/Training/Post_Details/${catId}`;
            });
        },
        error: function (err) {
            showerrormodal("error occurred. please contact your system administrator.");
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });
}


/////////////////

//upload video  .....

const videodropArea = document.getElementById('videoDropArea');
const videofileInput = document.getElementById('PostVideoThumbnail');
const videoPreview = document.getElementById('videoPreview');
const videoPreviewContainer = document.getElementById('videoPreviewContainer');
const VideoUploadBtn = document.getElementById('VideoUploadBtn');
const removeVideoBtn = document.getElementById('removeVideoBtn');

// Click on drop area opens file selector
videodropArea.addEventListener('click', () => {
    videofileInput.click();
});

// Handle file input change
videofileInput.addEventListener('change', () => {
    const file = videofileInput.files[0];
    handleFileVideo(file);
});

// Drag over
videodropArea.addEventListener('dragover', (e) => {
    e.preventDefault();
    videodropArea.classList.add('border-primary');
});

// Drag leave
videodropArea.addEventListener('dragleave', (e) => {
    e.preventDefault();
    dropArea.classList.remove('border-primary');
});

// Drop
videodropArea.addEventListener('drop', (e) => {
    e.preventDefault();
    videodropArea.classList.remove('border-primary');

    const files = e.dataTransfer.files;
    if (files.length > 0) {
        const file = files[0];
        fileInput.files = files;  // Update the file input with dropped file(s)
        handleFileVideo(file);
    }
});

// Handle file validation & preview
function handleFileVideo(file) {
    if (!file) {
        resetPreview();
        return;
    }

    if (!file.type.startsWith('video/')) {
        alert('Please upload a valid video file.');
        resetPreview();
        return;
    }

    const videoURL = URL.createObjectURL(file);
    videoPreview.src = videoURL;
    videoPreviewContainer.style.display = 'block';
    VideoUploadBtn.disabled = false;
}

// Remove preview & reset
removeVideoBtn.addEventListener('click', () => {
    resetPreview();
});

function resetPreview() {
    videofileInput.value = '';
    videoPreview.src = '';
    videoPreviewContainer.style.display = 'none';
    VideoUploadBtn.disabled = true;
}






$('#VideoUploadBtn').on('click', async function () {
    debugger;
    const file = $('#PostVideoThumbnail')[0].files[0];

   

    if (!file) {
        alert('Please select a video file to upload.');
        return;
    }
    if (!file.type.startsWith('video/')) {
        alert('Only video files are allowed.');
        return;
    }

    const formData = new FormData();



    formData.append('File', file);
    formData.append('Id', $('#LessonVideoId').val() || 0);
    formData.append('ReferenceId', $('#trainingcategory_Id').val() || 0);
    formData.append('ModuleName', "2");
    formData.append('FolderName', "videos/Lessons");
    formData.append('Type', "LessonVideo");


    var url = sitePath + 'api/Trainings/UploadMedia';
    const token = localStorage.getItem('jwtToken');
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'Authorization': `Bearer ${token}`
        },
        beforeSend: function () {
            Ajax.enqueueSpinner();
        },
        success: function (res) {
            swal.fire({
                text: "File added successfully.",
                icon: "success",
                buttonsstyling: false,
                confirmbuttontext: "ok",
                allowescapekey: false,
                allowoutsideclick: false,
                customclass: {
                    confirmbutton: "btn btn-primary",
                },
                didopen: function () {
                    hideSpinner();
                }
            }).then(function () {
                let catId =$('#trainingcategory_Id').val();
                window.location = `/Training/Post_Details/${catId}`;

            });
        },
        error: function (err) {

            console.log(err);
            //showerrormodal("error occurred. please contact your system administrator.");
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });
});


//Pdf uploader
const pdfDropArea = document.getElementById('pdfDropArea');
const pdfInput = document.getElementById('PdfFile');
const confirmPdfUploadBtn = document.getElementById('confirmPdfUploadBtn');
const pdfFileName = document.getElementById('pdfFileName');
const pdfPreviewFrame = document.getElementById('pdfPreviewFrame');
const pdfRemoveBtn = document.getElementById('PdfRemoveBtn');

// Click trigger
pdfDropArea.addEventListener('click', () => pdfInput.click());
pdfDropArea.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        pdfInput.click();
    }
});

// Drag events
pdfDropArea.addEventListener('dragover', e => {
    e.preventDefault();
    pdfDropArea.classList.add('dragover');
});
pdfDropArea.addEventListener('dragleave', e => {
    e.preventDefault();
    pdfDropArea.classList.remove('dragover');
});
pdfDropArea.addEventListener('drop', e => {
    e.preventDefault();
    pdfDropArea.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handlePdfFile(file);
});
pdfInput.addEventListener('change', e => {
    const file = e.target.files[0];
    handlePdfFile(file);
});

function handlePdfFile(file) {
    if (!file || file.type !== 'application/pdf') {
        Swal.fire('Invalid File', 'Please select a PDF file.', 'warning');
        resetPdfPreview();
        return;
    }

    pdfFileName.innerText = file.name;
    confirmPdfUploadBtn.disabled = false;
}

function resetPdfPreview() {
    pdfFileName.innerText = '';
    pdfPreviewFrame.src = '';
    pdfPreviewFrame.hidden = true;
    confirmPdfUploadBtn.disabled = true;
}

// Upload to server
confirmPdfUploadBtn.addEventListener('click', () => {
    const file = pdfInput.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('Id', $('#PdfDocumentId').val() || 0);
    formData.append('ReferenceId', $('#trainingcategory_Id').val() || 0);
    formData.append('ModuleName', "2");
    formData.append('FolderName', "pdfs/PostDetails");
    formData.append('Type', "LessonPdf");
    formData.append('File', file);
    formData.append('Description', $('#PdfDescription').val());
    formData.append('Title', $('#PdfTitle').val());

    const token = localStorage.getItem('jwtToken');
    const url = sitePath + 'api/Trainings/UploadMedia';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'Authorization': `Bearer ${token}`
        },
        beforeSend: function () {
            Ajax.enqueueSpinner();
        },
        success: function (res) {
            if (res && res.Data) {
                //renderPdfTable(res.Data);
             
            }

            Swal.fire('Success', 'PDF uploaded successfully.', 'success');
            let catId = $('#trainingcategory_Id').val();
            window.location = `/Training/Post_Details/${catId}`;
        },
        error: function () {
            showerrormodal('Upload failed. Please try again.');
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });

    const modal = bootstrap.Modal.getInstance(document.getElementById('pdfUploadModal'));
    modal.hide();
    resetPdfPreview();
    pdfInput.value = '';
});




// Pdf table rows
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
            <tr data-index="${index}">
                <td>${pdf.FileTitle}</td>
                <td>${pdf.FileDescription || 'N/A'}</td>
                <td>
                    <a href="${pdf.FilePath}" target="_blank" class="btn btn-sm btn-outline-secondary me-1" title="Preview">
                        <i class="bi bi-eye text-primary"></i>
                    </a>
                    <button class="btn btn-sm btn-outline-secondary remove-pdf-btn" title="Remove">
                        <i class="bi bi-trash text-primary"></i>
                    </button>
                </td>
            </tr>
        `;
        $tableBody.append(row);
    });

    $('#pdfTable').show();
}


// Video Thumbnail uploader
const videoThumbnailDropArea = document.getElementById('videoThumbnailDropArea');
const videoThumbnailInput = document.getElementById('VideoThumbnailFile');
const confirmVideoThumbnailUploadBtn = document.getElementById('SubmitVideoThumbnailBtn');
const videoThumbnailPreview = document.getElementById('videoThumbnailPreview');
const videoThumbnailPreviewContainer = document.getElementById('videoThumbnailPreviewContainer');
const removeVideoThumbnailBtn = document.getElementById('removeVideoThumbnailBtn');

// Click to trigger file selection
videoThumbnailDropArea.addEventListener('click', () => videoThumbnailInput.click());

// Keyboard support
videoThumbnailDropArea.addEventListener('keydown', e => {
    if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        videoThumbnailInput.click();
    }
});

// Drag events
videoThumbnailDropArea.addEventListener('dragover', e => {
    e.preventDefault();
    videoThumbnailDropArea.classList.add('dragover');
});
videoThumbnailDropArea.addEventListener('dragleave', e => {
    e.preventDefault();
    videoThumbnailDropArea.classList.remove('dragover');
});
videoThumbnailDropArea.addEventListener('drop', e => {
    e.preventDefault();
    videoThumbnailDropArea.classList.remove('dragover');
    const file = e.dataTransfer.files[0];
    handleVideoThumbnailFile(file);
});

// Input change
videoThumbnailInput.addEventListener('change', e => {
    const file = e.target.files[0];
    handleVideoThumbnailFile(file);
});

// Handle file preview
function handleVideoThumbnailFile(file) {
    if (!file || !file.type.startsWith('image/')) {
        Swal.fire('Invalid File', 'Please select a valid image file.', 'warning');
        resetVideoThumbnailPreview();
        return;
    }

    const reader = new FileReader();
    reader.onload = e => {
        videoThumbnailPreview.src = e.target.result;
        videoThumbnailPreviewContainer.style.display = 'block';
        confirmVideoThumbnailUploadBtn.disabled = false;
    };
    reader.readAsDataURL(file);
}

function resetVideoThumbnailPreview() {
    videoThumbnailPreview.src = '';
    videoThumbnailPreviewContainer.style.display = 'none';
    confirmVideoThumbnailUploadBtn.disabled = true;
}

// Upload to server
confirmVideoThumbnailUploadBtn.addEventListener('click', () => {
    const file = videoThumbnailInput.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('Id', $('#VideoThumbnailId').val() || 0);
    formData.append('ReferenceId', $('#trainingcategory_Id').val() || 0);
    formData.append('ModuleName', "2");
    formData.append('FolderName', "images/VideoThumbnails");
    formData.append('Type', "VideoThumbnail");
    formData.append('File', file);

    const token = localStorage.getItem('jwtToken');
    const url = sitePath + 'api/Trainings/UploadMedia';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'Authorization': `Bearer ${token}`
        },
        beforeSend: function () {
            Ajax.enqueueSpinner();
        },
        success: function (res) {
            if (res && res.Data) {
                $('#VideoThumbnailId').val(res.Data.Id);
                $('#videoThumbnailPreview_Img').attr('src', res.Data.FilePath);
                Swal.fire('Success', 'Thumbnail uploaded successfully.', 'success');
                let catId = $('#trainingcategory_Id').val();
                window.location = `/Training/Post_Details/${catId}`;
            } else {
                Swal.fire('Error', 'Unexpected server response.', 'error');
            }
        },
        error: function () {
            showerrormodal('Upload failed. Please try again.');
        },
        complete: function () {
            Ajax.dequeueSpinner();
        }
    });

    const modal = bootstrap.Modal.getInstance(document.getElementById('videoThumbnailUploadModal'));
    modal.hide();
    resetVideoThumbnailPreview();
    videoThumbnailInput.value = '';
});

// Remove button handler
removeVideoThumbnailBtn.addEventListener('click', () => {
    $('#VideoThumbnailId').val('');
    resetVideoThumbnailPreview();
    videoThumbnailInput.value = '';
});
