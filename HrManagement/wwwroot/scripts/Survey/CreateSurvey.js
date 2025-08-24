var surveyId = getParameterByName("SurveyId") || null;
let questionIndex = 0;

HRMSUtil.onDOMContentLoaded(function () {
    InitializeDatePickers.init();
    registerEventListners.init();
    PopulateDropDowns(function () {
        if (surveyId) {
            loadSurvey();
        }
    });
    unspin();
});

var InitializeDatePickers = function () {
    return {
        init: function () {

            $("#PublishDate").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });

            $("#CompletionDate").flatpickr({
                dateFormat: "d/m/Y",
                //defaultDate: new Date().fp_incr(-30),
            });
        }
    }
}();

var registerEventListners = function () {

    return {
        init: function () {
            $("#SiteId, #DepartmentId").change(function () {
                loadEmployees();
            });

            $("#IsRecurring").change(function () {
                if ($(this).is(":checked")) {
                    $("#RecursionDiv").show();
                } else {
                    $("#RecursionDiv").hide();
                    $("#Recursion").val(""); // Reset the dropdown when unchecked
                }
            });

        }
    };

}();

function PopulateDropDowns(cb) {

    var ddSites = document.querySelector('#SiteId');
    $(ddSites).empty();
    var lstSites = (DropDownsData.Sites || []);
    var option = new Option();
    ddSites.appendChild(option);
    lstSites.forEach(function (item) {

        var option = new Option(item.SiteName, item.Id, false, false);
        ddSites.appendChild(option);
    });

    var ddDepartments = document.querySelector('#DepartmentId');
    $(ddDepartments).empty();
    var lstDepartments = (DropDownsData.Departments || []);
    var option = new Option();
    ddDepartments.appendChild(option);
    lstDepartments.forEach(function (item) {

        var option = new Option(item.DepartmentName, item.DepartmentId, false, false);
        ddDepartments.appendChild(option);
    });

    if (cb) {
        cb();
    }
}

function loadEmployees() {
    var siteId = $("#SiteId").val();
    var departmentId = $("#DepartmentId").val();
    var url = sitePath + "api/EmployeeAPI/GetEmployeeBySiteDepartment?siteId=" + siteId + "&departmentId=" + departmentId;

    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var ddEmployees = document.querySelector('#EmployeeCode');
            $(ddEmployees).empty();
            var lstEmployees = (resp.Employee || []);
            var option = new Option();
            ddEmployees.appendChild(option);
            lstEmployees.forEach(function (item) {

                var option = new Option(item.FirstName + " " + item.LastName, item.EmployeeCode, false, false);
                ddEmployees.appendChild(option);
            });

            // Fire event after employees are loaded
            $(document).trigger("employeesLoaded");
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
    "Name": {
        presence: {
            message: "is required."
        }
    },
    "Status": {
        presence: {
            message: "is required."
        }
    },
    "PublishDate": {
        presence: {
            message: "is required."
        }
    },
    "Question": {
        presence: {
            message: "is required."
        }
    },
    "Weight": {
        presence: {
            message: "is required."
        }
    },
    "Max": {
        presence: {
            message: "is required."
        }
    },
    "Min": {
        presence: {
            message: "is required."
        }
    },
    "CompletionDate": {
        presence: {
            message: "is required."
        }
    }
};

$("#create-survey").on("click", function () {
    var form = document.querySelector("#create-survey-form");

    // Only validate fields that exist in the form
    var existingFields = {};
    for (let key in constraints) {
        if ($(`[name='${key}']`).length > 0) {
            existingFields[key] = constraints[key];
        }
    }

    var errors = validate(form, existingFields);
    //console.log("Validating Fields:", Object.keys(errors || {})); // Debugging Log

    showErrors(form, errors || {});
    if (!errors) {

        let surveyData = {
            Id: surveyId != null ? surveyId : 0,
            Name: $("#Name").val(),
            Description: $("#Description").val() || null,
            Status: $("#Status").val() || null,
            IsRecurring: $("#IsRecurring").is(":checked") || null,
            Recursion: $("#Recursion").val() || null,
            PublishDate: $("#PublishDate").val() || null,
            CompletionDate: $("#CompletionDate").val() || null,
            SiteId: $("#SiteId").val() || null,
            Site: $("#SiteId option:selected").text() || null,
            DepartmentId: $("#DepartmentId").val() || null,
            Department: $("#DepartmentId option:selected").text() || null,
            Employees: [],
            Questions: []
        };

        $("#EmployeeCode option:selected").each(function () {
            surveyData.Employees.push({
                EmployeeCode: ($(this).val() != '' && $(this).val() != null && $(this).val() != 0) ? parseInt($(this).val()) : null,
                EmployeeName: $(this).text()
            });
        });

        $(".question-section").each(function () {
            let $this = $(this);
            let question = {
                QuestionText: $this.find("input[name='Question']").val(),
                Description: $this.find("textarea[name='Description']").val(),
                QuestionType: $this.find(".question-title").text().split("(")[1].replace(")", "").trim(),
                IsRequired: $this.find("input[name='IsRequired']").is(":checked"), // Checkbox
                SortOrder: $this.find("select[name='SortOrder']").val() || null, // Ascending/Descending
                MinValue: $this.find("input[name='Min']").val() || null, // Slider Min Value
                MaxValue: $this.find("input[name='Max']").val() || null, // Slider Max Value
                Scale: $this.find("select[name='Scale']").val() || null, // Rating Scale (1-10)
                Shape: $this.find("select[name='Shape']").val() || null, // Rating Shape
                Label: $this.find("input[name='Label']").val() || null, // Slider or Rating Label
                Weight: $this.find("input[name='Weight']").val() || null, // Rating Weight
                Options: []
            };

            $this.find(".option-text").each(function (index) {
                question.Options.push({
                    OptionText: $(this).val(),
                    SortOrder: index + 1
                });
            });

            surveyData.Questions.push(question);
        });

        if (Window.JustConsole) { console.log(surveyData); return; }
        var url = sitePath + 'api/SurveyAPI/Save';

        Ajax.post(url, surveyData, function (response) {

            if (response.StatusCode == 200) {
                Swal.fire({
                    text: "Survey created succesfully.",
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
                        window.location.href = "/Survey/ManageSurveys";
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

// Load survey
function loadSurvey() {
    var url = sitePath + "api/SurveyAPI/GetSurvey/" + surveyId;
    Ajax.get(url, function (resp) {
        console.log(resp)

        if (resp.StatusCode == 200) {
            Survey = resp.Survey;
            $("#Name").val(Survey.Name);
            $("#Description").val(Survey.Description);
            $("#Status").val(Survey.Status).change();

            if (Survey.IsRecurring) {
                $("#IsRecurring").prop("checked", true);
                $("#Recursion").val(Survey.Recursion).change().parents('#RecursionDiv').show(); // Show the recursion field
            } else {
                $("#IsRecurring").prop("checked", false);
                $("#Recursion").parents('#RecursionDiv').hide(); // Hide recursion field
            }

            $("#PublishDate").flatpickr({
                dateFormat: "d/m/Y",
            }).setDate(FormatDate(Survey.PublishDate));
            $("#CompletionDate").flatpickr({
                dateFormat: "d/m/Y",
            }).setDate(FormatDate(Survey.CompletionDate));

            // Set SiteId and DepartmentId without triggering event
            $("#SiteId").val(Survey.SiteId);
            $("#DepartmentId").val(Survey.DepartmentId);

            // Now trigger change events to ensure dependent logic runs
            $("#SiteId").trigger("change");
            $("#DepartmentId").trigger("change");

            // Ensure employees are set after they are loaded
            $(document).one("employeesLoaded", function () {
                let employeeCodes = Survey.Employees.map(emp => emp.EmployeeCode);
                //$("#EmployeeCode").val(employeeCodes).trigger("change");

                setTimeout(() => {
                    $("#EmployeeCode").val(employeeCodes).trigger("change");
                }, 1000); // Ensures it applies AFTER all async updates
            });

            Survey.Questions.forEach((q, index) => {
                questionIndex++;
                let html = $(generateQuestionHtml(q.QuestionType, questionIndex));
                $("#questionsContainer").append(html);

                showHideBottomAddButton();

                let $newQuestion = $("#questionsContainer").find(`#question_${questionIndex}`);
                $newQuestion.find("input[name='Question']").val(q.QuestionText);
                if (q.IsRequired) {
                    $newQuestion.find("input[name='IsRequired']").prop("checked", true);
                }
                else {
                    $newQuestion.find("input[name='IsRequired']").prop("checked", false);
                }
                $newQuestion.find("textarea[name='Description']").val(q.Description);
                $newQuestion.find("select[name='SortOrder']").val(q.SortOrder).change();
                $newQuestion.find("input[name='Min']").val(q.MinValue);
                $newQuestion.find("input[name='Max']").val(q.MaxValue);
                $newQuestion.find("select[name='Scale']").val(q.Scale);
                $newQuestion.find("select[name='Shape']").val(q.Shape);
                $newQuestion.find("input[name='Label']").val(q.Label);
                $newQuestion.find("input[name='Weight']").val(q.Weight);

                q.Options.forEach(option => {
                    let inputType = q.QuestionType == "Multi Select" ? "checkbox" : q.QuestionType == "Single Select" ? "radio" : q.QuestionType == "Multi Text Box" ? "text" : "";
                    let nameAttr = inputType == "radio" ? `question_${questionIndex}_options` : inputType == "checkbox" ? "Option" : "";

                    if (q.QuestionType == "Single Select" || q.QuestionType == "Multi Select") {
                        let optionHtml = `
                        <div class="form-group" style="margin-bottom: 15px;">
                            <div class="form-group d-flex align-items-center mb-2">
                                <input type="${inputType}" name="${nameAttr}" class="me-2">
                                <input type="text" class="form-control me-2 option-text" value="${option.OptionText}" placeholder="Option">
                                <button type="button" class="btn btn-danger btn-sm remove-option">X</button>
                            </div>
                            <span class="col-sm-5 mt-2 messages"></span>
                        </div>
                        `;

                        $(`#options_${questionIndex}`).append(optionHtml);
                    }
                    else if (q.QuestionType == "Multi Text Box") {

                        //let labelCount = $(`#multi_text_${questionIndex}`).children().length + 1;
                        let labelHtml = `
                        <div class="d-flex align-items-center mb-2" style="margin-bottom: 15px;">
                            <input type="text" class="form-control me-2 option-text" value="${option.OptionText}" placeholder="Enter description to show as a lable in employee survey form.">
                            <button type="button" class="btn btn-danger btn-sm remove-multi-text">X</button>
                        </div>
                        `;

                        $(`#multi_text_${questionIndex}`).append(labelHtml);
                    }

                });
            });
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

// Open modal on Add Question click
$(".addQuestionBtn").click(function () {
    $("#questionType").val("");
    $("#questionTypeModal").modal("show");
});

// Add question on modal selection
$("#addQuestionType").click(function () {
    let type = $("#questionType").val();
    if (!type) {
        alert("Please select a question type.");
        return;
    }

    questionIndex++;
    //let html = generateQuestionHtml(type, questionIndex);
    let html = $(generateQuestionHtml(type, questionIndex)); // Convert to jQuery object
    $("#questionsContainer").append(html);
    showHideBottomAddButton();
    $("#questionTypeModal").modal("hide");
    html[0].scrollIntoView({ behavior: "smooth", block: "start" });
});

// Function to generate question HTML
function generateQuestionHtml(type, index) {

    let commonFields = `
                <div class="question-section card mt-3 p-3" id="question_${index}" data-id="${index}" style="margin-bottom: 30px;">
                    <div class="d-flex justify-content-between" style="margin-top: 10px;">
                        <strong class="question-title">Question #${index} (${type})</strong>
                        <button type="button" class="btn btn-danger btn-sm remove-question" data-id="${index}">Remove</button>
                    </div>
    
                    <div class="d-flex align-items-center">
                        <label class="me-1">Question</label>
                        <span style="color:red">*</span>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control mb-2" id="question_${index}" name="Question" required>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>
                    <div class="form-group form-check form-check-inline" style="margin-bottom: 15px;">
                        <input class="form-check-input" type="checkbox" value="1" name="IsRequired" data-bind="attr:{id:'is_required'+$index()} ,checked: is_required" id="is_required0">
                        <label class="form-check-label" data-bind="attr:{for:'is_required'+$index()}" for="is_required0">This Question Is Required</label>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Description</label>
                        <textarea class="form-control mb-2" id="description_${index}" name="Description"></textarea>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>
                    `;

    let specificFields = "";
    switch (type) {
        case "Single Select":
        case "Multi Select":
            let inputType = type == "Single Select" ? "radio" : "checkbox";
            specificFields = `
                    <label>Sort By</label>
                    <div class="form-group" style="margin-bottom: 15px;">
                        <select class="form-control mb-2" name="SortOrder">
                            <option value="Ascending">Ascending</option>
                            <option value="Descending">Descending</option>
                        </select>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <button type="button" class="btn btn-primary btn-sm add-option" data-input="${inputType}" data-id="${index}" style="margin-bottom: 15px;">Add Option</button>
                    
                    <div class="options-container mt-3" id="options_${index}" style="margin-bottom: 15px;"></div>
                    `;
            break;

        case "Single Text Box":
        case "Multi Text Box":
            specificFields = type == "Multi Text Box"
                ? `<button type="button" class="btn btn-primary btn-sm add-multi-text" data-id="${index}" style="margin-bottom: 15px;">Add Label</button>
                       <div class="multi-text-container mt-3" id="multi_text_${index}" style="margin-bottom: 15px;"></div>`
                : "";
            break;

        case "Slider":
            specificFields = `

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Label</label>
                        <input type="text" class="form-control mb-2" id="label_${index}" name="Label">
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>
                    
                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Min Value</label>
                        <span style="color:red" class="label-staric">*</span>
                        <input type="number" class="form-control mb-2" id="min_${index}" name="Min">
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Max Value</label>
                        <span style="color:red" class="label-staric">*</span>
                        <input type="number" class="form-control mb-2" id="max_${index}" name="Max">
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>
                    `;
            break;

        case "Rating":
            specificFields = `
                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Scale</label>
                        <select class="form-control mb-2" name="Scale">
                            ${Array.from({ length: 10 }, (_, i) => `<option value="${i + 1}">${i + 1}</option>`).join("")}
                        </select>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Shape</label>
                        <select class="form-control mb-2" name="Shape">
                            <option value="Star">Star</option>
                            <option value="Smiley">Smiley</option>
                            <option value="Heart">Heart</option>
                            <option value="Thumb">Thumb</option>
                        </select>
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Rating Label</label>
                        <input type="text" class="form-control mb-2" id="rating_label_${index}" Name="Label">
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>

                    <div class="form-group" style="margin-bottom: 15px;">
                        <label>Weight</label>
                        <span style="color:red" class="label-staric">*</span>
                        <input type="number" class="form-control mb-2" id="weight_${index}" name="Weight">
                        <span class="col-sm-5 mt-2 messages"></span>
                    </div>
                    `;

            break;
    }

    return commonFields + specificFields + `</div>`;
}

// Add option for single/multi select
$(document).on("click", ".add-option", function () {
    let id = $(this).data("id");
    let inputType = $(this).data("input");

    // Assign a unique name for radio buttons within the same question
    let nameAttr = inputType === "radio" ? `question_${id}_options` : "Option";

    let optionHtml = `
                <div class="form-group" style="margin-bottom: 15px;">
                    <div class="form-group d-flex align-items-center mb-2">
                        <input type="${inputType}" name="${nameAttr}" class="me-2">
                        <input type="text" class="form-control me-2 option-text" placeholder="Option">
                        <button type="button" class="btn btn-danger btn-sm remove-option">X</button>
                    </div>
                    <span class="col-sm-5 mt-2 messages"></span>
                </div>
                `;

    $(`#options_${id}`).append(optionHtml);
});

// Add multi-text labels
$(document).on("click", ".add-multi-text", function () {
    let id = $(this).data("id");
    //let labelCount = $(`#multi_text_${id}`).children().length + 1;
    let labelHtml = `
            <div class="d-flex align-items-center mb-2" style="margin-bottom: 15px;">
                <input type="text" class="form-control me-2 option-text" placeholder="Enter description to show as a lable in employee survey form.">
                <button type="button" class="btn btn-danger btn-sm remove-multi-text">X</button>
            </div>
            `;
    $(`#multi_text_${id}`).append(labelHtml);
});

// Remove question and update indexes
$(document).on("click", ".remove-question", function () {
    $(this).closest(".question-section").remove();
    showHideBottomAddButton();
    updateQuestionIndexes(); // Re-index questions after removal
});

// Remove dynamically added fields
$(document).on("click", ".remove-option, .remove-multi-text", function () {
    $(this).parent().remove();
});

// Function to update indexes after removing a question
function updateQuestionIndexes() {
    questionIndex = 0; // Reset counter
    $(".question-section").each(function (index) {
        questionIndex = index + 1;

        let questionType = $(this).find(".question-title").text().split("(")[1].replace(")", "").trim(); // Extract existing type
        questionType = questionType ? `(${questionType})` : ""; // Keep format intact

        $(this).attr("id", "question_" + questionIndex);
        $(this).attr("data-id", questionIndex);
        $(this).find(".question-title").text("Question #" + questionIndex);
        $(this).find(".question-title").text(`Question #${questionIndex} ${questionType}`);
        $(this).find(".remove-question").attr("data-id", questionIndex);
    });
}

function showHideBottomAddButton() {
    if ($("#questionsContainer").children().length > 0) {
        $(".addBottomButton").show();
    }
    else {
        $(".addBottomButton").hide();
    }
}
