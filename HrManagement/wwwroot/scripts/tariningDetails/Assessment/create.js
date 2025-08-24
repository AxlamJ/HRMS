

$(document).ready(function () {
    $('#submitBtn').on('click', function (e) {
        e.preventDefault();
        const questions = [];
        const formData = new FormData();
        formData.append('title', $('#Title').val());
        var isPass = $('#IsPassingGrade').is(':checked');
       
        console.log(isPass);


        formData.append('passingGrade', $('#PassingGrade').val());
        formData.append('isPassingGrade', isPass);
        formData.append('categoryId', $('#trainingcategory_Id').val());
        formData.append('id', $('#AssessmentId').val());
        const imageId = $('#ImageId').val() || 0;
        formData.append('imageId', imageId);
        const passHtml = passQuill.root.innerHTML;
        const failHtml = failQuill.root.innerHTML;

        formData.append('passConfirmationMessage', passHtml);
        formData.append('failConfirmationMessage', failHtml);


        const imageFile = $('#imageInput')[0].files[0];
        if (imageFile) {
            formData.append('imageThumbnail', imageFile);
        }


        $('.question-card').each(function () {
            const questionIndex = $(this).data('question-index');

            const questionType = $(this).find('.question-type').val();
            const questionId = `editor_question_${questionIndex}`;
            const q_QuestionId = $(this).find('.question-id').val()||0;


            const quill = quillInstances[questionId];

            if (!quill) {
                console.warn(`Quill instance not found for: ${questionId}`);
                return;
            }

            const questionText = quill.root.innerHTML;
            const categoryId = $('#trainingcategory_Id').val();

            const answers = [];
            $(this).find('.option-item').each(function (optIndex) {
                const optionId = `editor_option_${questionIndex}_${optIndex}`;
            
                const optionQuill = quillInstances[optionId];

                if (!optionQuill) {
                    console.warn(`Option Quill not found for: ${optionId}`);
                    return;
                }

                const optionText = optionQuill.root.innerHTML;
                const isCorrect = $(this).find('.is-correct').is(':checked');
                const answerId = $(this).find('.answer-id').val()||0;

                answers.push({
                    title: optionText,
                    isCorrect: isCorrect,
                    categoryId: categoryId,
                    assessmentQuestionId: 0,
                    status: "active",
                    type: "text",
                    id: answerId
                });
            });

            questions.push({
                id:q_QuestionId,
                title: questionText,
                categoryId: categoryId,
                status: "active",
                type: questionType === 'multi' ? 'multiple-choice' : 'single-choice',
                answers: answers
            });
        });

        console.log(JSON.stringify(questions));
        debugger
        formData.append('questions', JSON.stringify(questions));



        var url = sitePath + 'api/Trainings/UpsertAssessment';
        debugger;


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
                    text: "Assessment added successfully.",
                    icon: "success",
                    buttonsstyling: false,
                    confirmbuttontext: "ok",
                    allowescapekey: false,
                    allowoutsideclick: false,
                    customclass: {
                        confirmbutton: "btn btn-primary",
                    },
                    didopen: function () {
                       // hideSpinner();
                    }
                }).then(function () {
                    let assessment_Id = $("#trainingcategory_Id").val();
                    window.location = `/Training/Tarining_Assesment/${assessment_Id}`;
                });

               
            },
            error: function (err) {

                showerrormodal("error occurred. please contact your system administrator.");
            },
            complete: function () {
                Ajax.dequeueSpinner();
            }
        });
    });
    $('#imageInput').on('change', function (event) {
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
});
