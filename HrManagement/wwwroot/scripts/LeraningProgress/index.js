function pct(num, den) {

    return den === 0 ? 0 : Math.round((num / den) * 100);

}



function renderDashboard(structures) {

    const container = document.getElementById('dashboard');

    container.innerHTML = '';



    structures.forEach(struct => {



        if (struct.StructureType != '2') {







            const cats = struct.Categories || [];



            let totalLessons = cats.filter(c => c.CategoryType === "1").length;

            let watchedLessons = 0;

            cats.forEach(c => {

                if (Array.isArray(c.LessonProgress)) {

                    watchedLessons += c.LessonProgress.filter(lp => lp.Watched).length;

                }

            });

            const lessonPct = pct(watchedLessons, totalLessons);

            const quizzes = cats.filter(c => c.CategoryType === "3" && c.Assessment);



            const card = document.createElement('div');

            card.className = 'card';



            card.innerHTML = `

          <h2>${struct.StructureTitle || 'Untitled Structure'}</h2>

          <p>Lessons Watched: ${watchedLessons}/${totalLessons} (${lessonPct}%)</p>

          <div class="progress-bar-bg">

            <div class="progress-bar-fill" style="width: ${lessonPct}%;"></div>

          </div>

          <ul class="quiz-list"></ul>

          <div class="charts"></div>

        `;



            const quizList = card.querySelector('.quiz-list');

            const chartsDiv = card.querySelector('.charts');



            quizzes.forEach((quiz, index) => {

                const assessment = quiz.Assessment;

                let bestScore = 0;

                if (Array.isArray(assessment.Attempts)) {

                    bestScore = Math.max(...assessment.Attempts.map(a => a.QuizScore));

                }



                const passScore = assessment.QuizPassScore || 0;

                const passed = bestScore >= passScore;



                const li = document.createElement('li');

                li.innerHTML = `

            <strong>${assessment.QuizTitle}</strong>

            <span>Best Score: ${bestScore}% (Pass: ${passScore}%)</span>

            <span class="${passed ? 'pass' : 'fail'}">${passed ? 'Pass' : 'Fail'}</span>

          `;

                quizList.appendChild(li);



                const canvas = document.createElement('canvas');

                canvas.className = 'chart-canvas';

                canvas.id = `chart-${struct.TrainingStructureId}-${index}`;

                canvas.height = 200;

                chartsDiv.appendChild(canvas);



                const ctx = canvas.getContext('2d');

                const attempts = assessment.Attempts || [];

                const labels = attempts.map((a, i) => `Attempt ${i + 1}`);

                const scores = attempts.map(a => a.QuizScore);

                const colors = scores.map(s => s >= passScore ? 'green' : 'red');



                new Chart(ctx, {

                    type: 'bar',

                    data: {

                        labels: labels,

                        datasets: [{

                            label: 'Quiz Scores',

                            data: scores,

                            backgroundColor: colors

                        }]

                    },

                    options: {

                        scales: {

                            x: { stacked: false, barThickness: 10 },

                            y: {

                                beginAtZero: true,

                                max: 100,

                                title: { display: true, text: 'Score (%)' },

                                ticks: { stepSize: 10 }

                            }

                        },



                        legend: {

                            display: false,

                            position: 'bottom',

                            labels: { boxWidth: 12 }

                        },

                    }

                });

            });



            container.appendChild(card);

        }

    });



}



$(document).ready(() => {





    let id = $('#Id').val();

    var url = sitePath + `api/UserProgressReport/GetUserLessonReport/?id=${id}`;



    Ajax.post(url, null, function (response) {

        console.log(response);

        if (response.StatusCode === 200) {

            let dataObj = JSON.parse(response.Data);





            dataObj = filtredJson(dataObj);



            ;

            // Get the Structures array from the first Training item safely

            const structures = dataObj.Trainings?.[0]?.Structures || [];





            $("#TrainingTitle").append(`<h4 class="mb-4 mx-3">Training: ${dataObj.Trainings?.[0].TrainingTitle}</h4>`);



            console.log(structures);



            renderDashboard(structures);

        } else {

            showErrorModal("Error occurred. Please contact your system administrator.");

        }

    });

    hideSpinner();

});



function filtredJson(data) {



    let result = {

        Trainings: (data.Trainings || []).map(t => ({

            ...t,

            Structures: (t.Structures || []).map(s => ({



                ...s,

                Categories: (s.Categories || []).map(c => {

                    let newCategories = { ...c };

                    if (c.LessonProgress) {

                        if (Array.isArray(c.LessonProgress)) {

                            newCategories.LessonProgress =

                                c.LessonProgress.filter(l => l.LessonUserID == EmployeeCode)



                        }

                        else {

                            newCategories.LessonProgress = null;

                        }

                    }

                    if (c.Assessment) {

                        let AttemptsList =

                            Array.isArray(c.Assessment.Attempts) ?

                                c.Assessment.Attempts.filter(q => q.QuizUserID == EmployeeCode) : [];

                        newCategories.Assessment = {

                            ...c.Assessment, Attempts: AttemptsList

                        };

                    }

                    else {

                        newCategories.Assessment = null;

                    }

                    return newCategories;

                })

            }))

        }))
    }

    return result

}