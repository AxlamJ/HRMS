var surveyId = getParameterByName("SurveyId") || null;
var recurringDate = getParameterByName("RecurringDate") || null;
var IsSubmitted = getParameterByName("IsSubmitted") || null;
let isSubmitted = IsSubmitted == "true" ? true : false;

HRMSUtil.onDOMContentLoaded(function () {
    if (surveyId) {
        loadSurveyDetails(surveyId, employeeCode);
    }
    unspin();
});

function loadSurveyDetails(surveyId, employeeCode) {
    var url = sitePath + `api/SurveyAPI/GetSurveyDetails?surveyId=${surveyId}&employeeCode=${parseInt(employeeCode)}&recurringDate=${recurringDate}`;

    Ajax.get(url, function (resp) {
        //console.log(resp)

        if (resp.StatusCode == 200) {
            $("#surveyTitle").text(resp.Survey.Name);
            $("#surveyDescription").text(resp.Survey.Description);
            renderSurveyQuestions(resp.Questions, resp.Responses);
            toggleFormState();
        }
        else if (resp.StatusCode == 404) {
            Swal.fire({
                text: "Survey not found.",
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

function renderSurveyQuestions(questions, responses) {
    let container = $("#surveyQuestionsContainer");
    container.empty();

    questions.forEach((q, index) => {
        let response = responses.find(r => r.QuestionId === q.Id) || {};
        let inputField = "";

        if (q.QuestionType === "Single Select") {
            // Sort options based on OptionText and the Question's SortOrder
            if (q.SortOrder === "Ascending") {
                q.Options.sort((a, b) => a.OptionText.localeCompare(b.OptionText));
            } else if (q.SortOrder === "Descending") {
                q.Options.sort((a, b) => b.OptionText.localeCompare(a.OptionText));
            }

            inputField = `
            <select class="form-select question-input" data-question-id="${q.Id}" ${isSubmitted ? "disabled" : ""}>
                ${q.Options.map(opt => `
                    <option value="${opt.Id}" ${response.OptionId == opt.Id ? "selected" : ""}>
                        ${opt.OptionText}
                    </option>
                `).join("")}
            </select>`;
        }
        else if (q.QuestionType === "Multi Select") {
            inputField = `<div class="checkbox-container">`;

            // Sort options based on OptionText and the Question's SortOrder
            if (q.SortOrder === "Ascending") {
                q.Options.sort((a, b) => a.OptionText.localeCompare(b.OptionText));
            } else if (q.SortOrder === "Descending") {
                q.Options.sort((a, b) => b.OptionText.localeCompare(a.OptionText));
            }

            // Get all selected options for this question
            let selectedOptions = responses
                .filter(r => r.QuestionId === q.Id) // Find all responses for this question
                .map(r => r.AnswerText); // Extract AnswerText (which contains the selected OptionId)

            inputField += q.Options.map(opt => `
                <div class="form-check" style="margin-bottom: 10px;">
                    <input type="checkbox" class="form-check-input question-input"
                        data-question-id="${q.Id}" value="${opt.Id}" 
                        ${selectedOptions.includes(opt.Id.toString()) ? "checked" : ""} 
                        ${isSubmitted ? "disabled" : ""}>
                    <label class="form-check-label">${opt.OptionText}</label>
                </div>`).join("");

            inputField += `</div>`; // Close divs properly
        }
        else if (q.QuestionType === "Single Text Box") {
            let textValue = response.AnswerText || ""; // Get the saved value or empty
            inputField = `
            <textarea class="form-control question-input" data-question-id="${q.Id}" 
            ${isSubmitted ? "disabled" : ""}>${textValue}</textarea>`;
        }
        //else if (q.QuestionType === "Multi Text Box") {
        //    let textValues = response.AnswerText ? response.AnswerText.split(";") : []; // Get saved values
        //    inputField = q.Options.map((opt, i) => `
        //<div class="mb-2">
        //    <label class="fw-bold">${opt.OptionText}</label>
        //    <textarea class="form-control question-input" 
        //        data-question-id="${q.Id}" 
        //        data-option-id="${opt.Id}" 
        //        ${isSubmitted ? "disabled" : ""}>${textValues[i] || ""}</textarea>
        //        </div>
        //    `).join("");
        //}
        else if (q.QuestionType === "Multi Text Box") {
            // Get all responses for this question where OptionId is null
            let textResponses = responses
                .filter(r => r.QuestionId === q.Id) // Find all responses for this question
                .map(r => r.AnswerText); // Extract only AnswerText values

            inputField = q.Options.map((opt, i) => `
                <div class="mb-2">
                    <label class="fw-bold">${opt.OptionText}</label>
                    <textarea class="form-control question-input" 
                        data-question-id="${q.Id}" 
                        data-option-id="${opt.Id}" 
                        ${isSubmitted ? "disabled" : ""}>${textResponses[i] || ""}</textarea>
                </div>
            `).join("");
        }
        else if (q.QuestionType === "Slider") {
            let sliderValue = response.AnswerText || q.MinValue;

            inputField = `
            <div class="d-flex flex-column align-items-center">  <!-- Centering the content -->
                ${q.Label ? `<label class="fw-bold mb-2 text-center">${q.Label}</label>` : ''}  <!-- Centered Label -->
                <span class="fw-bold text-primary slider-value text-center" id="sliderValue_${q.Id}">${sliderValue}</span>  <!-- Centered Slider Value -->
                <input type="range" min="${q.MinValue}" max="${q.MaxValue}" step="1" class="form-range question-input"
                    data-question-id="${q.Id}" value="${sliderValue}" ${isSubmitted ? "disabled" : ""} 
                    oninput="updateSliderValue(${q.Id}, this.value)">
                <div class="d-flex justify-content-between w-100 mt-2">
                    <span>${q.MinValue}</span>  <!-- Show Start Label -->
                    <span>${q.MaxValue}</span>  <!-- Show End Label -->
                </div>
            </div>`;
        }
        else if (q.QuestionType === "Rating") {
            inputField = `<div class="rating-container d-flex flex-column align-items-start gap-2">`;

            if (q.Label) {
                inputField += `<label class="fw-bold">${q.Label}</label>`;
            }

            // Hidden input to store the selected rating value
            inputField += `<input type="hidden" class="question-input rating-value" data-question-id="${q.Id}" value="${response.AnswerText || ''}">`;

            inputField += `<div class="d-flex flex-wrap justify-content-center gap-2">`;

            for (let i = 1; i <= q.Scale; i++) {
                let isSelected =  i <= response.AnswerText ? "selected" : "";
                let isDisabled = isSubmitted ? "pointer-events: none; opacity: 0.6;" : ""; // Disable selection when viewing

                let iconHTML = q.Shape === "Star"
                    ? `<span class="rating-icon star-icon ${isSelected}" data-question-id="${q.Id}" data-value="${i}" style="${isDisabled}">★</span>`
                    : q.Shape === "Smiley"
                        ? `<span class="rating-icon smiley-icon ${isSelected}" data-question-id="${q.Id}" data-value="${i}" style="${isDisabled}">😊</span>`
                        : q.Shape === "Heart"
                            ? `<span class="rating-icon heart-icon ${isSelected}" data-question-id="${q.Id}" data-value="${i}" style="${isDisabled}">❤</span>`
                            : `<span class="rating-icon thumb-icon ${isSelected}" data-question-id="${q.Id}" data-value="${i}" style="${isDisabled}">
                        <svg width="30" height="30" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path d="M2 10H4V21H2V10ZM6 10H13L12.22 13.17L14.55 15.49C14.92 15.86 15.09 16.34 15.04 16.83L14.14 21.58C14.06 22.05 13.66 22.4 13.17 22.4H6V10Z"/>
                        </svg>
                        </span>`;

                inputField += `
                <div class="rating-item text-center">
                    ${iconHTML}
                    <div class="rating-label">${i}</div>
                </div>`;
            }

            inputField += `</div></div>`;
        }

        container.append(`
            <div class="card mb-3">
                <div class="card-body">
                    <h5 class="card-title fw-bold">${index + 1}. ${q.QuestionText} ${q.IsRequired ? '<span class="text-danger">*</span>' : ''}</h5>
                    ${q.Description ? `<p class="text-muted">${q.Description}</p>` : ''}  <!-- Add this line for description -->
                    <div class="question-input-container">${inputField}</div>
                </div>
            </div>
        `);
    });
}

$(document).on("click", ".rating-icon", function () {
    if (isSubmitted) return; // Prevent selection in view mode

    let questionId = $(this).data("question-id");
    let selectedValue = $(this).data("value");

    // Reset and apply selected class
    $(`.rating-icon[data-question-id="${questionId}"]`).removeClass("selected");
    $(`.rating-icon[data-question-id="${questionId}"]`).each(function () {
        if ($(this).data("value") <= selectedValue) {
            $(this).addClass("selected");
        }
    });

    // Update hidden input value
    $(`.rating-value[data-question-id="${questionId}"]`).val(selectedValue);

});

function updateSliderValue(questionId, value) {
    $(`#sliderValue_${questionId}`).text(value); // Update displayed value in real-time
}

function toggleFormState() {
    if (isSubmitted) {
        $("#submitSurvey").hide(); // Hide submit button in view mode
        $(".question-input").prop("disabled", true);
        $(".rating-icon").off("click"); // Prevent rating selection
    } else {
        $("#submitSurvey").show();
    }
}

$("#submitSurvey").on("click", function () {
    let errors = false;

    $("#surveyQuestionsContainer").find(".card").each(function () {
        let questionContainer = $(this);
        let questionTitle = questionContainer.find(".card-title").html(); // Get the question title

        // **Check if this question is required**
        let isRequired = questionTitle.includes('<span class="text-danger">*</span>');

        //console.log("Question Required:", isRequired, "Title:", questionTitle); // Debugging

        // **Validate only the input fields inside this card**
        questionContainer.find(".question-input").each(function () {
            let inputField = $(this);
            let inputType = inputField.attr("type");
            let inputValue = inputField.val() ? inputField.val().trim() : "";

            //console.log("Field:", inputField, "Required:", isRequired, "Value:", inputValue); // Debugging

            // Remove previous error messages
            inputField.closest(".question-input-container").find(".validation-error").remove();

            if (isRequired) {
                if (inputField.is("select") || inputField.is("textarea") || inputType === "text") {
                    if (inputValue === "") {
                        errors = true;
                        showValidationError(inputField, "This field is required.");
                    }
                }
            }
        });

        // **Validate Multi-Select Checkboxes inside this question only**
        let checkBoxContainer = questionContainer.find(".checkbox-container");
        if (checkBoxContainer.length > 0) {
            let checkedOptions = checkBoxContainer.find("input[type='checkbox']:checked").length;

            //console.log("Checkbox Required:", isRequired, "Checked:", checkedOptions); // Debugging

            if (isRequired && checkedOptions === 0) {
                errors = true;
                showValidationError(checkBoxContainer, "Please select at least one option.");
            }
        }

        // **Validate Rating inside this question only**
        let ratingContainer = questionContainer.find(".rating-container");
        if (ratingContainer.length > 0) {
            let selectedRating = ratingContainer.find(".rating-icon.selected").length;

            //console.log("Rating Required:", isRequired, "Selected:", selectedRating); // Debugging

            ratingContainer.find(".validation-error").remove();

            if (isRequired && selectedRating === 0) {
                errors = true;
                ratingContainer.append(`<p class="validation-error text-danger mt-2">Please select a rating.</p>`);
            }
        }

        // **Validate Slider inside this question only**
        questionContainer.find("input[type='range']").each(function () {
            let slider = $(this);
            let sliderValue = slider.val();
            let minValue = slider.attr("min");

            //console.log("Slider Required:", isRequired, "Value:", sliderValue); // Debugging

            if (isRequired && (sliderValue === "" || sliderValue < minValue)) {
                errors = true;
                showValidationError(slider, "Please adjust the slider.");
            }
        });
    });

    // **Stop submission if errors exist**
    if (errors) {
        let firstErrorField = $(".validation-error:first").closest(".card");
        Swal.fire({
            text: "Please complete all mandatory fields.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok",
            allowEscapeKey: false,
            allowOutsideClick: false,
            customClass: {
                confirmButton: "btn btn-primary",
            }
        }).then(() => {
            if (firstErrorField.length > 0) {
                $("html, body").animate({
                    scrollTop: firstErrorField.offset().top - 50
                }, 500);
            }
        });
        return;
    }

    // **Proceed with Submission if No Errors**
    let responses = [];

    $(".question-input").each(function () {
        let questionId = $(this).data("question-id");
        let answerText = $(this).val();
        let optionId = null;

        if ($(this).is("select")) {
            optionId = $(this).val();
        } else if ($(this).is(":checkbox") && $(this).is(":checked")) {
            optionId = $(this).val();
        }

        responses.push({
            SurveyId: surveyId,
            EmployeeCode: parseInt(employeeCode),
            QuestionId: questionId,
            AnswerText: answerText,
            OptionId: optionId,
            ResponseDate: recurringDate
        });
    });

    if (Window.JustConsole) { console.log(responses); return; }

    var url = sitePath + 'api/SurveyAPI/SubmitResponses';

    Ajax.post(url, responses, function (response) {
        if (response.StatusCode == 200) {
            Swal.fire({
                text: "Survey submitted successfully.",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                allowEscapeKey: false,
                allowOutsideClick: false,
                customClass: {
                    confirmButton: "btn btn-primary",
                }
            }).then(function (result) {
                if (result.isConfirmed) {
                    window.location.href = "/Survey/MySurveys";
                }
            });
        }
        else {
            Swal.fire({
                text: "Error occurred. Please contact your system administrator.",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                allowEscapeKey: false,
                allowOutsideClick: false,
                customClass: {
                    confirmButton: "btn btn-primary",
                }
            });
        }
    });
});

/**
 * Function to show validation error messages
 */
function showValidationError(field, message) {
    let errorMessage = `<p class="validation-error text-danger mt-2">${message}</p>`;

    if (field.hasClass("checkbox-container")) {
        field.append(errorMessage);
    } else {
        let container = field.closest(".question-input-container");
        if (container.length > 0) {
            container.append(errorMessage);
        } else {
            field.after(errorMessage); // Fallback if container is missing
        }
    }
}