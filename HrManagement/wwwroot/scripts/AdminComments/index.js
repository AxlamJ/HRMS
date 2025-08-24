
// Map status codes to labels
const statusMap = { 1: "Enable", 2: "Enable", 3: "Hidden", 4: "Lock" };

// Dropdown options
const dropdownOptions = [
    { value: "Hidden", text: "Hidden" },
    { value: "Lock", text: "Lock" },
    { value: "Enable", text: "Enable" }
];

function renderStructure(data) {
    const $container = $("#structureList");
    $container.empty();

    data.structures.forEach(struct => {
        struct.categories.forEach(cat => {
            $container.append(createItem(cat, false)); // Category

            cat.TrainingSubCategories?.forEach(sub => {
                $container.append(createItem(sub, true)); // Subcategory
            });
        });
    });
}

function createItem(item, isSub = false) {
    const id = item.id;
    const title = item.title?.trim() || "Untitled";
    const statusVal = statusMap[item.status] || "Enable";

    let dropdown = `<select class="form-select form-select-sm status-dropdown" data-id="${id}">`;
    dropdownOptions.forEach(opt => {
        const sel = opt.value === statusVal ? "selected" : "";
        dropdown += `<option value="${opt.value}" ${sel}>${opt.text}</option>`;
    });
    dropdown += "</select>";

    return $(`
        <div class="structure-item d-flex justify-content-between align-items-center mb-2">
            <div class="item-title ${isSub ? 'ms-4 text-muted' : ''}">
                ${isSub ? '<span class="text-muted small">↳</span> ' : ''}
                ${title}
            </div>
            <div class="right-controls d-flex align-items-center gap-2">
                <button type="button" class="btn btn-primary" onclick="openChatModal(${id})">
                    <i class="bi bi-chat-dots"></i>
                </button> 
            </div>
        </div>
    `);
}

$(document).ready(function () {

    console.log(".................................")
    //renderStructure(responseData);

    let Id = $('#ddTrainings option:not([value=""]):first').val();
    loadTrainingDataComments(Id);

    //$(document).on('change', '.status-dropdown', function () {
    //    const id = $(this).data('id');
    //    const val = $(this).val();
    //    console.log(`Status changed: ID=${id}, New=${val}`);
    //    const url = `api/Trainings/UpsertCategoryStatus?Id=${id}&Status=${val}`;
    //    console.log("API:", url);
    //});

    //$(document).on('click', '.comments-label', function () {
    //    const id = $(this).data('id');
    //    alert(`Comments clicked for ID ${id}`);
    //});
});

function loadTrainingDataComments(Id) {


    const url = sitePath + `api/Trainings/GetTrainingWithDetails?trainingId=${Id}`;
    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            renderStructure(resp.Data);
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

// Open the modal and load comments
function openChatModal(categoryId) {
    currentCategoryId = categoryId;
    const modal = new bootstrap.Modal(document.getElementById('chatModal'));
    modal.show();
    adminloadComments(categoryId);
}

// Load comments
function adminloadComments(id) {

    if (id === "" || id === null || id === undefined) {

        return null;
    }



$('#commentsId').val(id);
hideSpinner();

const url = sitePath + `api/Chat/loadUserComments?Id=${id}`;

Ajax.post(url, null, function (resp) {
    const container = $('#chatMessages');
    container.empty();

    if (resp.StatusCode === 200 && Array.isArray(resp.Data)) {
        resp.Data.forEach(comment => {
            const profileImage = '/profile-photos/img_1740304073560.png';
            const userName = comment.UserName || `User ${comment.UserId}`;
            const createdAt = new Date(comment.CreatedAt).toLocaleString();

            const messageHtml = `
                   <div class="d-flex mb-3">
  <img src="${profileImage}" alt="A" class="rounded-circle me-3" width="48" height="48" />
  <div style="width: 100%;">
    <div class="bg-white border rounded shadow-sm p-3">
      <div class="fw-bold mb-2 text-primary">${comment.CreatedBy}</div>
     <div class="text-dark" style="max-width: 500px; word-wrap: break-word;">
  ${comment.CommentText}
</div>
    </div>
    <small class="text-muted mt-1 d-block">${new Date(comment.CreatedAt).toLocaleString()}</small>
  </div>
</div>
`;

            container.append(messageHtml);
        });

        container.scrollTop(container[0].scrollHeight);
    } else {
        showErrorAlert("Error occurred. Please contact your system administrator.");
    }
});
}

// Send comment
function sendComment() {
    hideSpinner();

    if ($('#chatInput').val() == null || $('#chatInput').val() == '') {
        return null;
    }

    let id = $('#commentsId').val();
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
            adminloadComments(id)
        } else {
            showErrorModal("Error occurred. Please contact your system administrator.");
        }
    });
}


$('#ddTrainings').on('change', function () {
    var Id = $(this).val();
    var selectedText = $("#ddTrainings option:selected").text();
    loadTrainingDataComments(Id);
});