const quillInstances = {};
var passQuill = null;
var failQuill = null;
$(document).ready(function () {


   
    $('#PassingGrade').on('keydown', function (e) {
        if (
            $.inArray(e.keyCode, [8, 9, 13, 27, 46, 37, 38, 39, 40]) !== -1 ||
            (e.ctrlKey === true &&
                (e.keyCode === 65 || e.keyCode === 67 || e.keyCode === 86 || e.keyCode === 88))
        ) {
            return;
        }
        if (e.keyCode < 48 || e.keyCode > 57) {
            e.preventDefault();
        }
    });
    LoadAssessmet();
    hideSpinner();

    $('#btnPreviewAssessment').on('click', function (e) {
        e.preventDefault();
        quizId = $('#AssessmentId').val();
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
                window.open(`/TrainingSession/Quiz/${quizId}`, '_blank');
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });

      
    });
  
});
document.querySelectorAll('.rich_textbox_editor111').forEach(function (editorDiv) {
    new Quill(editorDiv, {
        theme: 'snow'
    });
});
let questionIndex = 0;
let optionNo = 0;
function createQuillEditor(id) {
    const quill = new Quill(`#${id}`, {
        theme: 'snow'
    });
    quillInstances[id] = quill; // Correct
    return quill;
}

function addQuestion(questionData = null) {
    optionNo = 0;
    const qId = `question_${questionIndex}`;
    // Set question data
    if (questionData != null) {
        const qId = questionIndex;
        const questionType = questionData?.QuestionType?.toLowerCase().includes('multi') ? 'multi' : 'single';

        const questionHTML = `
<div class="card mb-3 question-card" data-question-index="${questionIndex}">
    <div class="card">
      <input type="hidden" class="question-id" value="${questionData?.QuestionId || ''}" />
        <div class="card-header d-flex justify-content-between align-items-center">
            <span class="fw-bold">Question #${questionIndex + 1}</span>
            <div class="btn-group">
                <button type="button" class="btn btn-sm btn-danger delete-question">
                    <i class="bi bi-trash"></i> Delete
                </button>
            </div>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <select class="form-select question-type">
                    <option value="single" ${questionType === 'single' ? 'selected' : ''}>Single Choice</option>
                    <option value="multi" ${questionType === 'multi' ? 'selected' : ''}>Multi Choice</option>
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label fw-bold">Question</label>
                <div class="rich_textbox_editor" id="editor_question_${questionIndex}" style="height: 100px;"></div>
            </div>
            <div class="answers-container"></div>
            <button type="button" class="btn btn-sm btn-primary add-option">
                <i class="bi bi-plus-circle me-1"></i> Add Choice
            </button>
        </div>
    </div>
</div>
`;

        // Append the question block
        $('#questionContainer').append(questionHTML);

        // Initialize the question editor
        createQuillEditor(`editor_question_${questionIndex}`);

        // Set question title if exists
        if (questionData?.QuestionTitle) {
            const editor = quillInstances[`editor_question_${questionIndex}`];
            if (editor) {
                editor.root.innerHTML = questionData.QuestionTitle;
            }
        }

        // Add answers if available
        if (Array.isArray(questionData?.Answers) && questionData.Answers.length > 0) {
            questionData.Answers.forEach(Answer => {
                addOption(questionIndex, Answer);
            });
        } else {
            addOption(questionIndex); // add a blank option if none
        }

        questionIndex++;
    }


    else {
        const questionHTML = `
    <div class="card mb-3 question-card" data-question-index="${questionIndex}">
      <input type="hidden" class="question-id" value="${questionData?.QuestionId || ''}" />
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <span class="fw-bold">Question #${questionIndex + 1}</span>
                <div class="btn-group">
                    <button type="button" class="btn btn-sm btn-danger delete-question">
                        <i class="bi bi-trash"></i> Delete
                    </button>
                </div>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <select class="form-select question-type">
                        <option value="single" selected>Single Choice</option>
                        <option value="multi">Multi Choice</option>
                    </select>
                </div>
                <div class="mb-3">
                    <label class="form-label fw-bold">Question</label>
                    <div class="rich_textbox_editor" id="editor_question_${questionIndex}" style="height: 100px;"></div>
                </div>
                <div class="answers-container"></div>
                <button type="button" class="btn btn-sm btn-primary add-option">
                    <i class="bi bi-plus-circle me-1"></i> Add Choice
                </button>
            </div>
        </div>
    </div>
    `;

        $('#questionContainer').append(questionHTML);
        createQuillEditor(`editor_question_${questionIndex}`);

        // Add first default answer
        addOption(questionIndex);
        questionIndex++;
    }
   
}
function addOption(questionIndex, answerData = null) {
 
    const questionCard = $(`.question-card[data-question-index="${questionIndex}"]`);
    const answersContainer = questionCard.find('.answers-container');
    const optionIndex = answersContainer.children().length;

    const optionId = `editor_option_${questionIndex}_${optionIndex}`;
    const checkboxId = `optionCheck_${questionIndex}_${optionIndex}`;

    if (answerData != null) {
        const isChecked = answerData?.IsCorrect ? 'checked' : '';
        const optionHTML = `
        <div class="mb-3 option-item d-flex flex-column gap-2">
          <input type="hidden" class="answer-id" value="${answerData?.AnswerId || ''}" />
            <label class="form-label fw-bold">Option ${optionIndex + 1}</label>
            <div class="rich_textbox_editor" id="${optionId}" style="height: 100px;"></div>
            <div class="d-flex justify-content-between align-items-center">
                <div class="form-check">
                    <input class="form-check-input is-correct" type="checkbox" id="${checkboxId}" ${isChecked} />
                    <label class="form-check-label fw-bold" for="${checkboxId}">
                        Correct
                    </label>
                </div>
                <button type="button" class="btn btn-danger btn-sm delete-option">
                    <i class="bi bi-trash-fill me-1"></i> Delete
                </button>
            </div>
        </div>
    `;
        answersContainer.append(optionHTML);
        createQuillEditor(optionId);
        if (answerData?.AnswerTitle) {
            const editor = quillInstances[optionId];
            if (editor) {
                editor.root.innerHTML = answerData.AnswerTitle;
            }
        }


    }
    else {
        const optionHTML = `
    <div class="mb-3 option-item d-flex flex-column gap-2">
     <input type="hidden" class="answer-id" value="${answerData?.AnswerId || ''}" />
        <label class="form-label fw-bold">Option ${optionIndex + 1}</label>
        <div class="rich_textbox_editor" id="${optionId}" style="height: 100px;"></div>
        <div class="d-flex justify-content-between align-items-center">
            <div class="form-check">
                <input class="form-check-input is-correct" type="checkbox" id="${checkboxId}" />
                <label class="form-check-label" for="${checkboxId}">
                    Correct
                </label>
            </div>
            <button type="button" class="btn btn-danger btn-sm delete-option">
                <i class="bi bi-trash-fill me-1"></i> Delete
            </button>
        </div>
    </div>
    `;
        answersContainer.append(optionHTML);
        createQuillEditor(optionId);
    } 
}


$(document).on('click', '#addQuestionBtn', function (e) {
    e.preventDefault();
    addQuestion();
});

$(document).on('click', '.add-option', function (e) {
    e.preventDefault();
    const card = $(this).closest('.question-card');
    const qIndex = card.data('question-index');
    addOption(qIndex);
});

$(document).on('click', '.delete-option', function (e) {
    e.preventDefault();
    $(this).closest('.option-item').remove();
});

$(document).on('click', '.delete-question', function (e) {
    e.preventDefault();
    $(this).closest('.question-card').remove();
});
// Only one checkbox allowed if question type is "single"
$(document).on('change', '.is-correct', function () {
    const questionCard = $(this).closest('.question-card');
    const type = questionCard.find('.question-type').val();

    if (type === 'single') {
        questionCard.find('.is-correct').not(this).prop('checked', false);
    }
});

// Optional: reset checkboxes when question type is changed
$(document).on('change', '.question-type', function () {
    const questionCard = $(this).closest('.question-card');
    const checkboxes = questionCard.find('.is-correct');

    if ($(this).val() === 'single') {
        // Allow only one checked
        let checkedFound = false;
        checkboxes.each(function () {
            if (checkedFound) {
                $(this).prop('checked', false);
            } else if ($(this).is(':checked')) {
                checkedFound = true;
            }
        });
    }
});



function assessmetData(Data) {

    if (Array.isArray(Data) && Data.length > 0) {
        const Assessment = Data[0];
    if (!Assessment.AssessmentId || Assessment.AssessmentId === 0) {
        passQuill = new Quill('#PassConfirmationMessage', { theme: 'snow' });
        failQuill = new Quill('#FailConfirmationMessage', { theme: 'snow' });

        addQuestion();
    }

    else {
        
        passQuill = new Quill('#PassConfirmationMessage', { theme: 'snow' });
        failQuill = new Quill('#FailConfirmationMessage', { theme: 'snow' });
        // Safely set form fields using jQuery
        $('#PassingGrade').val(Assessment.AssessmentPassingGrade);
        $('#AssessmentId').val(Assessment.AssessmentId);
        $('#Title').val(Assessment.AssessmentTitle);
        $('#IsPassingGrade').prop('checked', Assessment.AssessmentIsPassingGrade);
        passQuill.root.innerHTML = Assessment.AssessmentPassMessage;
        failQuill.root.innerHTML = Assessment.AssessmentFailMessage;

        if (Assessment.FileMediaUplaod && Assessment.FileMediaUplaod.length > 0) {
            let imageFile = Assessment.FileMediaUplaod[0];
            $('#imageThumbnail').attr('src', imageFile.FilePath);
            $('#ImageId').val(imageFile.FileId);
            console.log(imageFile);
        }
        if (Array.isArray(Assessment.Questions) && Assessment.Questions.length > 0) {
            Assessment.Questions.forEach(Question => {
                addQuestion(Question);
            });
        }
        }
    } else {
        console.error("No assessment data found.");
        // Optionally show fallback UI:
        passQuill = new Quill('#PassConfirmationMessage', { theme: 'snow' });
        failQuill = new Quill('#FailConfirmationMessage', { theme: 'snow' });
        addQuestion();
    }
}

    function LoadAssessmet() {
        // Example: assuming training 
        const id = $('#trainingcategory_Id').val();
        const url = sitePath + `api/Trainings/GetAssessmentsByCategory?id=${id}`;

        Ajax.post(url, null, function (resp) {
            if (resp.StatusCode == 200) {
                console.log(resp.Data);
                assessmetData(resp.Data);
               
            }
            else {
                console.log("sdasdasd");
                addQuestion();
                        hideSpinner();
                   
               
            }

        });
    }



