let AssessmentPassingGrade = 0;
let totalQuestions=0
$(document).ready(function () {
    LoadAssessmet();
    hideSpinner();
});

function LoadAssessmet() {
    // Example: assuming training 
    const id = $('#Id').val();
    const url = sitePath + `api/Trainings/GetAssessmentsByCategory?id=${id}`;

    Ajax.post(url, null, function (resp) {
        if (resp.StatusCode == 200) {
            console.log(resp.Data);
            if (Array.isArray(resp.Data) && resp.Data.length > 0) {
                const Assessment = resp.Data[0];
                $('#AssessmentPassingGrade').val(Assessment.AssessmentPassingGrade);
                $('#AssessmentId').val(Assessment.AssessmentId);
                  
                loadAssessmentQuestions(Assessment);

            }
        }
        else {
            console.log("sdasdasd");
            addQuestion();
            hideSpinner();
        }
    });
}


function loadAssessmentQuestions(assessment) {

    AssessmentPassingGrade = assessment.AssessmentPassingGrade;
    $('#countQuestion').empty();
    $('#Quiz-title').empty();
    $('#countQuestion').append(`Total Question: <span>${assessment.Questions.length || 0}</sapn>`);
    $('#Quiz-title').append(`Title: <span>${assessment.AssessmentTitle}</sapn>`);

    const container = $('#Quiz-container');
    container.empty(); // Clear existing content

    assessment.Questions.forEach((question, qIndex) => {
        const inputType = question.QuestionType === 'multiple-choice' ? 'checkbox' : 'radio';
        const questionId = question.QuestionId;
        totalQuestions++;

        let questionHtml = `
            <div class="mb-4 p-3 border border-primary rounded">
                <h2 id="question-text" class="mb-3 fw-bold">${question.QuestionTitle}</h2>
        `;

        question.Answers.forEach((answer, aIndex) => {
            const optionLetter = String.fromCharCode(65 + aIndex); // A, B, C, ...
            questionHtml += `
                <label class="option" for="q${questionId}opt${optionLetter}">
                    <input type="${inputType}"
                           id="q${questionId}opt${optionLetter}"
                           name="question_${questionId}"
                            data-is-correct="${answer.IsCorrect}"
                           value="${answer.AnswerId}" />
                          
                    <span class="option-label">${answer.AnswerTitle}</span>
                </label>
            `;
        });

        questionHtml += `</div>`;
        container.append(questionHtml);
    });

    // Optionally render image if exists
    if (assessment.FileMediaUplaod && assessment.FileMediaUplaod.length > 0) {
        const img = assessment.FileMediaUplaod[0];
        const imageHtml = `
     <div class="text-center mt-4 mb-4">
  <img src="${img.FilePath}" alt="Assessment Image" 
       style="width: 150px; height: 150px; object-fit: cover; border-radius: 50%; box-shadow: 0 0 8px rgba(0,0,0,0.15);" />
</div>

        `;
        container.prepend(imageHtml);
    }
}





$('#submit-quiz-btn').on('click', function (e) {
    e.preventDefault();
    quizId = $('#AssessmentId').val();
    Swal.fire({
        text: "Are you sure you want to Submit Quiz?",
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
            submitQuiz();
        } else if (result.dismiss === Swal.DismissReason.cancel) {
        }
    });


});






function submitQuiz() {
    const attempt = {
        QuizID: $("#AssessmentId").val(),  // Or dynamic quiz IDAssessmentId
        Score: 0,   // Will calculate later if needed
        PassScore: $("#AssessmentPassingGrade").val(), // Or get from AssessmentPassingGrade
        Passed: false,
        UserQuestionAnswers: []
    };

    let correctAnswers = 0;

    //// Loop through all questions
    //$('[name^="question_"]').each(function () {
    //    const questionId = $(this).attr('name').split('_')[1];
    //    if (!attempt.UserQuestionAnswers.some(a => a.QuestionID == questionId)) {
    //        totalQuestions++;
    //    }
    //});

    // For each selected answer
    $('[name^="question_"]:checked').each(function () {
        const questionId = $(this).attr('name').split('_')[1];
        const answerId = parseInt($(this).val());
        const isCorrect = $(this).data('is-correct') === true || $(this).data('is-correct') === "true";

        if (isCorrect) {
            correctAnswers++;
        }

        attempt.UserQuestionAnswers.push({
            QuestionID: parseInt(questionId),
            SelectedAnswerID: answerId,
            IsCorrect: isCorrect
        });
    });

    // Calculate score
    attempt.Score = ((correctAnswers / totalQuestions) * 100).toFixed(1);
    attempt.Passed = parseFloat(attempt.Score) >= parseFloat(attempt.PassScore);

    const totalAnswered = attempt.UserQuestionAnswers.length;
    const totalCorrect = attempt.UserQuestionAnswers.filter(q => q.IsCorrect).length;
    const totalWrong = totalAnswered - totalCorrect;
    const percent = parseFloat(attempt.Score);
    const result = attempt.Passed;

    // Format message
    const resultMessage = `
    <strong>Status:</strong> ${result ? '<span style="color:green">Pass</span>' : '<span style="color:red">Failed</span>'}<br>
    <strong>Score:</strong> ${percent}%<br>
    <strong>Correct Answers:</strong> ${totalCorrect}<br>
    <strong>Wrong Answers:</strong> ${totalWrong}
`;
    var url = sitePath + 'api/LessonQuizAttempt/UpsertUserQuizAttempt';

    Ajax.post(url, attempt, function (response) {
        console.log(response);
        if (response.StatusCode === 200) {

            $('input[type="checkbox"], input[type="radio"]').prop('disabled', true);

            // Loop through each answer input
            $('input[name^="question_"]').each(function () {
                const isCorrect = $(this).data('is-correct') === true || $(this).data('is-correct') === "true";
                const isChecked = $(this).is(':checked');
                const parentLabel = $(this).closest('label');

                if (isCorrect) {
                    parentLabel.addClass('correct-answer');
                }

                if (!isCorrect && isChecked) {
                    parentLabel.addClass('wrong-answer');
                }
            });
            //Swal.fire({
            //    title: 'Quiz Results',
            //    html: resultMessage,
            //    icon: result ? 'success' : 'error',
            //    confirmButtonText: 'OK'
            //});
        } else {
            showErrorModal("Error occurred. Please contact your system administrator.");
        }
    });
}







//$(document).on('click', '#submit-quiz-btn', function () {
//    let totalQuestions = $('.mb-4').length;
//    let totalScore = 0;

//    $('.mb-4').each(function () {
//        const $questionBlock = $(this);
//        const questionName = $questionBlock.find('input').first().attr('name');
//        const selectedInputs = $(`input[name="${questionName}"]:checked`);
//        const allCorrectInputs = $(`input[name="${questionName}"][data-is-correct="true"]`);
//        const correctCount = allCorrectInputs.length;

//        if (correctCount === 1) {
//            // Single choice question: full 1 mark if correct selected
//            if (selectedInputs.length === 1 && (selectedInputs.data('is-correct') === true || selectedInputs.data('is-correct') === "true")) {
//                totalScore += 1;
//            }
//        } else if (correctCount > 1) {
//            // Multi-choice question: partial credit
//            let partialScore = 0;

//            // Count how many selected are correct
//            selectedInputs.each(function () {
//                if ($(this).data('is-correct') === true || $(this).data('is-correct') === "true") {
//                    partialScore += 1 / correctCount; // divide question mark by correct options count
//                }
//            });

//            // Optional: if you want to penalize for wrong selected answers, subtract here
//            // For example:
//            // selectedInputs.each(function () {
//            //   if (!$(this).data('is-correct')) partialScore -= 1 / correctCount;
//            // });
//            // partialScore = Math.max(0, partialScore);

//            totalScore += partialScore;
//        }
//    });

//    // Round total score to 2 decimals
//    totalScore = Math.round(totalScore * 100) / 100;

//    const scorePercent = Math.round((totalScore / totalQuestions) * 100);

//    Swal.fire({
//        title: 'Quiz Result',
//        html: `
//            <div>Score: <strong>${totalScore.toFixed(2)}</strong> / <strong>${totalQuestions}</strong></div>
//            <div>Percentage: <strong>${scorePercent}%</strong></div>
//        `,
//        icon: scorePercent >= AssessmentPassingGrade ? 'success' : 'error',
//        confirmButtonText: 'OK',
//        buttonsStyling: false,
//        customClass: {
//            confirmButton: 'btn btn-primary'
//        }
//    });
//});

