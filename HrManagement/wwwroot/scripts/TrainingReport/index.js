const reportData = DropDownsData;



console.log(reportData);

function getEmployeeCountByDept(employees, departments) {



    console.log(employees);

    const counts = departments.map(d => {

        const depId = d.DepartmentId;

        const count = employees.filter(emp => {

            try {

                console.log(emp);

                const deps = JSON.parse(emp.DepartmentName);

                return deps.some(dp => dp.id === depId);

            } catch (err) {

                console.error("Error occurred:", err);

                return false;

            }

        }).length;

        return { label: d.DepartmentName, count };

    }).filter(c => c.count > 0);

    return counts;

}

function getEmployeeCountBySite(employees, sites) {

    const counts = sites.map(s => {

        const siteId = s.Id;

        const count = employees.filter(emp => {

            try {

                const sitesEmp = JSON.parse(emp.SiteName);

                return sitesEmp.some(st => st.id === siteId);

            } catch (err) {

                console.error("Error occurred:", err);

                return false;

            }

        }).length;

        return { label: s.SiteName, count };

    }).filter(c => c.count > 0);

    return counts;

}



// 1. Prepare the data

function getTrainingParticipation(trainings, courses, employees, courseProgress) {



    console.log(trainings);

    console.log(courses);

    console.log(employees);

    console.log(courseProgress);



    try {

        return trainings.map(training => {

            let assignedEmpIds = new Set();



            // Parse Employees array if exists

            try {

                if (training.Employees) {



                    console.log(training.Employees);



                    const empIds = JSON.parse(training.Employees);

                    empIds.forEach(id => assignedEmpIds.add(parseInt(id)));



                    console.log(empIds);

                }

            } catch (err) {

                console.error("Error parsing Employees for training:", training.TrainingTitle, err);

            }



            // Parse Departments if exists and assign employees accordingly

            try {

                if (training.Departments) {

                    const deps = JSON.parse(training.Departments);

                    const depIds = deps.map(d => d.DepartmentId);



                    employees.forEach(emp => {

                        try {

                            if (emp.DepartmentName) {

                                const empDeps = JSON.parse(emp.DepartmentName);

                                if (empDeps.some(dep => depIds.includes(dep.DepartmentId))) {

                                    assignedEmpIds.add(emp.Id);

                                }

                            }

                        } catch (err) {

                            console.error("Error parsing Employee.DepartmentName for empId:", emp.Id, err);

                        }

                    });

                }

            } catch (err) {

                console.error("Error parsing Departments for training:", training.TrainingTitle, err);

            }



            // Parse Sites if exists and assign employees accordingly

            try {

                if (training.Sites) {

                    const sites = JSON.parse(training.Sites);

                    const siteIds = sites.map(s => s.Id);



                    employees.forEach(emp => {

                        try {

                            if (emp.SiteName) {

                                const empSites = JSON.parse(emp.SiteName);

                                if (empSites.some(site => siteIds.includes(site.Id))) {

                                    assignedEmpIds.add(emp.Id);

                                }

                            }

                        } catch (err) {

                            console.error("Error parsing Employee.SiteName for empId:", emp.Id, err);

                        }

                    });

                }

            } catch (err) {

                console.error("Error parsing Sites for training:", training.TrainingTitle, err);

            }



            // Filter courses belonging to this training

            let trainingCourses = [];

            try {

                trainingCourses = courses.filter(c => c.TrainingId === training.TrainingId);

            } catch (err) {

                console.error("Error filtering courses for training:", training.TrainingTitle, err);

            }



            // Extract course IDs

            let courseIds = [];

            try {

                courseIds = trainingCourses.map(c => c.CourseId);

            } catch (err) {

                console.error("Error mapping courseIds for training:", training.TrainingTitle, err);

            }



            // Find relevant progress entries matching courses and assigned employees

            let relevantProgress = [];

            try {

                relevantProgress = courseProgress.filter(p =>

                    courseIds.includes(p.CourseId) && assignedEmpIds.has(p.EmployeeId)

                );

            } catch (err) {

                console.error("Error filtering courseProgress for training:", training.TrainingTitle, err);

            }



            // Calculate totals safely

            let totalAssigned = 0, totalCompleted = 0, inProgress = 0;

            try {

                totalAssigned = new Set(relevantProgress.map(p => `${p.EmployeeId}-${p.CourseId}`)).size;

                totalCompleted = relevantProgress.filter(p => p.IsCompleted).length;

                inProgress = totalAssigned - totalCompleted;

            } catch (err) {

                console.error("Error calculating progress counts for training:", training.TrainingTitle, err);

            }



            return {

                label: training.TrainingTitle,

                Assigned: totalAssigned,

                Completed: totalCompleted,

                'In Progress': inProgress

            };

        }).filter(t => t.Assigned > 0);

    } catch (err) {

        console.error("Unexpected error in getTrainingParticipation:", err);

        return [];

    }

}



function getTrainingProgress(courses, employees, coursesProgress) {

    // Calculate % progress for courses linked to selected employees

    const courseIds = courses.map(c => c.CourseId);

    const empIds = employees.map(e => e.EmployeeCode);

    console.log(empIds);



    // Filter progress for these employees and courses



    console.log(coursesProgress);

    const filteredProgress = coursesProgress.filter(p =>

        empIds.includes(p.EmployeeId) && courseIds.includes(p.CourseId)

    );



    if (filteredProgress.length === 0) return 0;



    const completedCount = filteredProgress.filter(p => p.IsCompleted).length;

    return Math.round((completedCount / filteredProgress.length) * 100);

}



function getCourseCompletionDetailed(courses, employees, coursesProgress) {

    const courseLabels = courses.map(c => c.CourseTitle);



    // For each employee, create a dataset showing completion (1 or 0) per course

    const datasets = employees.map((emp, idx) => {

        const data = courses.map(course => {

            const progress = coursesProgress.find(p =>

                p.EmployeeId === emp.EmployeeCode && p.CourseId === course.CourseId

            );

            return progress && progress.IsCompleted ? 1 : 0;

        });



        // Colors - reuse or extend your existing palette

        const colors = [

            '#0d6efd', '#198754', '#ffc107', '#dc3545', '#6f42c1', '#fd7e14', '#20c997', '#6610f2'

        ];



        return {

            label: emp.FirstName + ' ' + emp.LastName,

            data,

            backgroundColor: colors[idx % colors.length],

            borderColor: colors[idx % colors.length],

            borderWidth: 1,

            barThickness: 10  // Fix bar thickness here for this dataset

        };

    });



    return { labels: courseLabels, datasets };

}



let courseCompletionChart = null;

function updateCourseCompletionChartDetailed(data) {

    const ctx = document.getElementById('courseCompletionChart').getContext('2d');

    if (courseCompletionChart) courseCompletionChart.destroy();



    if (!data || !data.datasets.length) {

        courseCompletionChart = new Chart(ctx, {

            type: 'bar',

            data: { labels: [], datasets: [] },

            options: {

                plugins: {

                    legend: { display: false },

                    tooltip: { enabled: false }

                }

            }

        });

        return;

    }



    courseCompletionChart = new Chart(ctx, {

        type: 'bar',

        data,

        options: {

            responsive: true,





            scales: {

                x: { stacked: true },

                y: {

                    beginAtZero: true,

                    max: 1,

                    ticks: {

                        stepSize: 1,

                        callback: val => (val === 1 ? 'Completed' : 'Not Completed')

                    },

                    title: { display: true, text: 'Completion Status' }

                }

            },

            plugins: {

                legend: {

                    display: false,               // Show legend

                    position: 'bottom',          // Position at bottom

                    labels: { boxWidth: 12 }

                },

                tooltip: {

                    enabled: true,

                    mode: 'nearest',            // Show tooltip for nearest data point

                    intersect: true,

                    callbacks: {

                        label: function (ctx) {

                            const label = ctx.dataset.label || 'Unknown Employee';

                            const val = ctx.parsed.y === 1 ? 'Completed' : 'Not Completed';

                            return `${label}: ${val}`;

                        },

                        title: function (ctx) {

                            return ctx[0]?.label || '';  // Course title as tooltip title

                        }

                    }

                },

                datalabels: {

                    anchor: 'end',

                    align: 'top',

                    color: '#333',

                    font: { weight: '600' },

                    formatter: val => (val === 1 ? '✔' : '')

                }

            }

        },

        plugins: [ChartDataLabels]

    });

}



function getQuizReportDetailed(trainings, employees, courses, coursesQuizResult) {

    // Filter quiz courses in selected trainings

    const trainingIds = trainings.map(t => t.TrainingId);

    const employeeIds = employees.map(e => e.EmployeeCode);



    const quizCourses = courses.filter(c =>

        trainingIds.includes(c.TrainingId) && c.CourseType.toLowerCase() === '3'

    );



    if (quizCourses.length === 0) return null;



    const quizLabels = quizCourses.map(qc => qc.CourseTitle);



    // For each employee, get their scores for each quiz (0 if no score)

    const datasets = employees.map((emp, idx) => {

        const data = quizCourses.map(quiz => {

            const result = coursesQuizResult.find(r =>

                r.EmployeeId === emp.EmployeeCode && r.CourseId === quiz.CourseId

            );



            console.log(result);

            return result ? result.SecureScore : 0



        });



        // Generate a color for each employee, simple distinct colors

        const colors = [

            '#dc3545', '#6f42c1', '#fd7e14', '#20c997', '#6610f2', '#0d6efd', '#198754', '#ffc107',

        ];

        return {

            label: emp.FirstName + ' ' + emp.LastName,

            data,

            backgroundColor: colors[idx % colors.length],

            borderColor: colors[idx % colors.length],

            borderWidth: 1,

            barThickness: 10  // Fix bar thickness here for this dataset

        };

    });



    return { labels: quizLabels, datasets };

}

function updateQuizChartDetailed(data) {

    console.log(data);

    const ctx = document.getElementById('quizChart').getContext('2d');

    if (quizChart) quizChart.destroy();



    if (!data) {

        // No quiz data, show empty chart or message

        quizChart = new Chart(ctx, {

            type: 'bar',

            data: { labels: [], datasets: [] },

            options: {

                plugins: {

                    legend: { display: false },

                    tooltip: { enabled: false }

                }

            }

        });

        return;

    }



    quizChart = new Chart(ctx, {

        type: 'bar',

        data,

        options: {

            responsive: true,

            scales: {

                x: { stacked: false, barThickness: 10 },

                y: {

                    beginAtZero: true,

                    max: 100,

                    title: { display: true, text: 'Score (%)' },

                    ticks: { stepSize: 10 }

                }

            },

            plugins: {

                legend: {

                    display: false,

                    position: 'bottom',

                    labels: { boxWidth: 12 }

                },

                tooltip: {

                    enabled: true,

                    mode: 'nearest',

                    intersect: true,

                    callbacks: {

                        label: function (ctx) {

                            const label = ctx.dataset.label || 'Unknown Employee';

                            const value = ctx.parsed.y ?? 0;

                            return `${label}: ${value}`;

                        },

                        title: function (ctx) {

                            return ctx[0]?.label || '';

                        }

                    }

                },

                datalabels: {

                    anchor: 'end',

                    align: 'top',

                    color: '#333',

                    font: { weight: '600' },

                    formatter: val => (val > 0 ? val : '')

                }

            }





        },

        plugins: [ChartDataLabels]

    });

}



// Chart instances to update later

let deptChart, siteChart, trainingChart, progressChart, structureChart, quizChart;

function createDonutChart(ctx, labels, data, colors, centerCount = '', centerLabel = '') {



    console.log(data);



    if (!ctx) return;



    return new Chart(ctx, {

        type: 'doughnut',

        data: {

            labels,

            datasets: [{

                data,

                backgroundColor: colors,

                borderWidth: 0

            }]

        },

        options: {

            cutout: '70%',

            plugins: {

                legend: { position: 'bottom', labels: { boxWidth: 12, padding: 16 } },

                tooltip: { enabled: true },

                title: { display: false },

                datalabels: { display: false }

            }

        },

        plugins: [{

            id: 'centerCountPlugin',

            afterDraw(chart) {

                const { ctx, chartArea: { width, height } } = chart;

                ctx.save();



                // Draw count (larger)

                ctx.font = 'bold 24px Arial';

                ctx.fillStyle = '#000';

                ctx.textAlign = 'center';

                ctx.textBaseline = 'middle';

                ctx.fillText(centerCount, width / 2, height / 2 - 10);



                // Draw label (smaller)

                if (centerLabel) {

                    ctx.font = '14px Arial';

                    ctx.fillStyle = '#666';

                    ctx.fillText(centerLabel, width / 2, height / 2 + 15);

                }



                ctx.restore();

            }

        }]

    });

}

function createBarChart(ctx, labels, data, colors = [], chartTitle = '', barThickness = 25) {

    if (!ctx) return;

    return new Chart(ctx, {

        type: 'bar',

        data: {

            labels,

            datasets: [{

                label: chartTitle || 'Count',

                data,

                backgroundColor: colors.length === data.length ? colors : '#0d6efd',

                borderRadius: 0,           // Remove rounded corners for thin line look

                borderSkipped: false,

                barThickness: barThickness // Set bar width (4px by default, you can pass smaller value)

            }]

        },

        options: {

            indexAxis: 'x',

            responsive: true,

            plugins: {

                title: {

                    display: !!chartTitle,

                    text: chartTitle,

                    font: { size: 18, weight: 'bold' },

                    padding: { bottom: 20 }

                },

                legend: { display: false },

                tooltip: { enabled: true },

                datalabels: {

                    anchor: 'end',  // top of the bar

                    align: 'top',   // label above the bar

                    color: '#333',

                    font: { weight: '600' },

                    formatter: val => val

                }

            },

            scales: {

                x: {

                    beginAtZero: true,

                    grid: { color: '#eee' },

                    ticks: { stepSize: 1 },

                    offset: false,  // disables padding before first tick

                },

                y: {

                    beginAtZero: true,

                    grid: { color: '#eee' },

                    ticks: { stepSize: 1 }

                }

            }

        },

        plugins: [ChartDataLabels]

    });

}

function updateDeptChart(data) {

    const ctx = document.getElementById('deptChart').getContext('2d');

    if (deptChart) deptChart.destroy();



    const labels = data.map(d => d.label);

    const counts = data.map(d => d.count);

    deptChart = createDonutChart(ctx, labels, counts, ['#0d6efd', '#6c757d', '#ffc107', '#198754'], '');



    // Show total in center

    const centerDiv = document.getElementById('deptChartCenter');

    centerDiv.textContent = counts.reduce((a, b) => a + b, 0);

}

function updateSiteChart(data) {

    const ctx = document.getElementById('siteChart').getContext('2d');

    if (siteChart) siteChart.destroy();



    const labels = data.map(d => d.label);

    const counts = data.map(d => d.count);

    const colors = ['#0d6efd', '#198754', '#ffc107', '#dc3545', '#6f42c1', '#fd7e14']; // extend as needed



    siteChart = createDonutChart(ctx, labels, counts, colors, '');



    const centerDiv = document.getElementById('siteChartCenter');

    if (centerDiv) {

        centerDiv.textContent = counts.reduce((a, b) => a + b, 0);

        centerDiv.style.display = 'block'; // or 'none' if you want to hide outside count

    }



}









function updateTrainingChart(trainingData, canvasId) {

    const labels = trainingData.map(t => t.label);

    const assignedCounts = trainingData.map(t => t.Assigned);

    const completedCounts = trainingData.map(t => t.Completed);

    const inProgressCounts = trainingData.map(t => t['In Progress']);



    const ctx = document.getElementById(canvasId).getContext('2d');



    // Destroy previous chart instance if exists to avoid overlap

    if (trainingChart) {

        trainingChart.destroy();

    }



    trainingChart = new Chart(ctx, {

        type: 'bar',

        data: {

            labels: labels,

            datasets: [

                {

                    label: 'Assigned',

                    data: assignedCounts,

                    backgroundColor: 'rgba(54, 162, 235, 0.7)'

                },

                {

                    label: 'Completed',

                    data: completedCounts,

                    backgroundColor: 'rgba(75, 192, 192, 0.7)'

                },

                {

                    label: 'In Progress',

                    data: inProgressCounts,

                    backgroundColor: 'rgba(255, 206, 86, 0.7)'

                }

            ]

        },

        options: {

            responsive: true,

            scales: {

                y: {

                    beginAtZero: true,

                    precision: 0 // to show integer ticks

                }

            }

        }

    });

}



function updateProgressChart(percent) {

    const ctx = document.getElementById('progressChart').getContext('2d');

    if (progressChart) progressChart.destroy();



    const data = [percent, 100 - percent];

    progressChart = createDonutChart(ctx, ['Completed', 'Remaining'], data, ['#198754', '#dee2e6']);

}

function updateStructureChart(data) {

    const ctx = document.getElementById('structureChart').getContext('2d');

    if (structureChart) structureChart.destroy();



    const labels = data.map(d => d.type);

    const percentages = data.map(d => d.percentage);



    // Color palette for course types bars

    const colors = [

        '#198754', // green

        '#0d6efd', // blue

        '#ffc107', // yellow

        '#dc3545', // red

        '#6f42c1', // purple

        '#fd7e14'  // orange

    ];



    structureChart = createBarChart(ctx, labels, percentages, colors, 'Course Completion');

}

function updateQuizChart(avgScore) {

    const ctx = document.getElementById('quizChart').getContext('2d');

    if (quizChart) quizChart.destroy();



    const data = [avgScore, 100 - avgScore];

    quizChart = createDonutChart(ctx, ['Average Score', 'Remaining'], data, ['#ffc107', '#dee2e6']);

}



function initSelect2() {

    // Extract unique values for filters

    const departments = reportData.Departments.map(d => ({ id: d.DepartmentId, text: d.DepartmentName }));

    const subDepts = reportData.DepartmentSubCategories.map(s => ({ id: s.Id, text: s.SubCategoryName }));

    const sites = reportData.Sites.map(s => ({ id: s.Id, text: s.SiteName }));

    const employees = reportData.Employees.map(e => ({ id: e.EmployeeCode, text: e.FirstName + " " + e.LastName }));

    const trainings = reportData.Trainings.map(t => ({ id: t.TrainingId, text: t.TrainingTitle }));



    $('#deptFilter').select2({ data: departments, theme: 'bootstrap-5', placeholder: 'Select Departments' });

    $('#subDeptFilter').select2({ data: subDepts, theme: 'bootstrap-5', placeholder: 'Select Sub-Departments' });

    $('#siteFilter').select2({ data: sites, theme: 'bootstrap-5', placeholder: 'Select Sites' });

    $('#empFilter').select2({ data: employees, theme: 'bootstrap-5', placeholder: 'Select Employees' });

    $('#trainFilter').select2({ data: trainings, theme: 'bootstrap-5', placeholder: 'Select Trainings' });

}



function applyFilters() {

    const selectedDept = $('#deptFilter').val() || [];

    const selectedSubDept = $('#subDeptFilter').val() || [];

    const selectedSites = $('#siteFilter').val() || [];

    const selectedEmp = $('#empFilter').val() || [];

    const selectedTrain = $('#trainFilter').val() || [];



    // Step 1: Filter trainings

    const filteredTrainings = reportData.Trainings.filter(tr => {

        if (selectedTrain.length > 0 && !selectedTrain.includes(tr.TrainingId)) return false;



        console.log(tr);

        try {

            if (tr.Departments) {



                const deps = JSON.parse(tr.Departments).map(d => d.toString());

                if (selectedDept.length > 0 && !deps.some(d => selectedDept.includes(d))) return false;

            }

            if (tr.Sites) {

                const st = JSON.parse(tr.Sites).map(s => s.toString());

                if (selectedSites.length > 0 && !st.some(s => selectedSites.includes(s))) return false;

            }

            if (tr.Employees) {

                const emps = JSON.parse(tr.Employees).map(e => e.toString());

                if (selectedEmp.length > 0 && !emps.some(e => selectedEmp.includes(e))) return false;

            }

            return true;

        } catch (err) {

            console.error("Error occurred:", err);

            return false;

        }



    });



    // Step 2: Filter employees

    let filteredEmployees = reportData.Employees.filter(emp => {









        try {

            const empDeps = JSON.parse(emp.DepartmentName || "[]").map(d => d.id.toString());

            const empSubDeps = JSON.parse(emp.DepartmentSubCategoryName || "[]").map(s => s.id.toString());

            const empSites = JSON.parse(emp.SiteName || "[]").map(s => s.id.toString());



            const deptMatch = selectedDept.length === 0 || empDeps.some(d => selectedDept.includes(d));

            const subDeptMatch = selectedSubDept.length === 0 || empSubDeps.some(s => selectedSubDept.includes(s));

            const siteMatch = selectedSites.length === 0 || empSites.some(st => selectedSites.includes(st));

            const empMatch = selectedEmp.length === 0 || selectedEmp.includes(emp.EmployeeCode.toString());



            console.log(deptMatch);

            console.log(subDeptMatch);

            console.log(siteMatch);

            console.log(empMatch);





            return deptMatch && subDeptMatch && siteMatch && empMatch;

        } catch (err) {

            console.error("Error occurred:", err);

            return false;

        }



    });



    // ✅ Step 3: If no employees are selected directly, infer from trainings

    if (filteredEmployees.length === 0 && selectedTrain.length > 0) {

        console.log(filteredEmployees);



        const empIdsFromTrainings = new Set();



        filteredTrainings.forEach(tr => {

            if (tr.Employees) {

                try {

                    const empList = JSON.parse(tr.Employees).map(e => parseInt(e));

                    empList.forEach(id => empIdsFromTrainings.add(id));

                } catch (err) {

                    console.error("Error occurred:", err);

                    return false;

                }



            }

            if (tr.Departments) {

                console.log(tr.Departments);

                try {

                    const depList = JSON.parse(tr.DepartmentName).map(d => parseInt(d));

                    reportData.Employees.forEach(emp => {

                        try {

                            const empDeps = JSON.parse(emp.Name).map(d => d.Id);

                            if (empDeps.some(d => depList.includes(d))) {

                                empIdsFromTrainings.add(emp.Id);

                            }

                        } catch (err) {

                            console.error("Error occurred:", err);

                            return false;

                        }



                    });

                } catch (err) {

                    console.error("Error occurred:", err);

                    return false;

                }

            }

            if (tr.Sites) {

                try {

                    const siteList = JSON.parse(tr.Sites).map(s => parseInt(s));

                    reportData.Employees.forEach(emp => {

                        try {

                            const empSites = JSON.parse(emp.SiteName).map(s => s.Id);

                            if (empSites.some(id => siteList.includes(id))) {

                                empIdsFromTrainings.add(emp.Id);

                            }

                        } catch (err) {

                            console.error("Error occurred:", err);

                            return false;

                        }



                    });

                } catch (err) {

                    console.error("Error occurred:", err);

                    return false;

                }



            }

        });

        filteredEmployees = reportData.Employees.filter(emp => empIdsFromTrainings.has(emp.Id));

    }

    // Step 4: Filter courses for these trainings

    const filteredCourses = reportData.Courses.filter(c =>

        filteredTrainings.some(t => t.TrainingId === c.TrainingId)

    );

    // Step 5: Update charts

    updateDeptChart(getEmployeeCountByDept(filteredEmployees, reportData.Departments));

    updateSiteChart(getEmployeeCountBySite(filteredEmployees, reportData.Sites));

    //updateTrainingChart(getTrainingParticipation(filteredTrainings, filteredEmployees));

    // Assuming reportData.Trainings and reportData.Employees available

    const trainingData = getTrainingParticipation(filteredTrainings, filteredCourses, filteredEmployees, reportData.CoursesProgress);

    updateTrainingChart(trainingData, 'trainingChart');







    updateProgressChart(getTrainingProgress(filteredCourses, filteredEmployees, reportData.CoursesProgress));



    //updateStructureChart(getCourseCompletion(filteredCourses, filteredEmployees, reportData.CoursesProgress));



    const courseCompletionData = getCourseCompletionDetailed(filteredCourses, filteredEmployees, reportData.CoursesProgress);

    updateCourseCompletionChartDetailed(courseCompletionData);





    const quizData = getQuizReportDetailed(filteredTrainings, filteredEmployees, filteredCourses, reportData.CoursesQuizResult);

    updateQuizChartDetailed(quizData);

}

function clearFilters() {

    $('#deptFilter').val(null).trigger('change');

    $('#subDeptFilter').val(null).trigger('change');

    $('#siteFilter').val(null).trigger('change');

    $('#empFilter').val(null).trigger('change');

    $('#trainFilter').val(null).trigger('change');



    applyFilters();

}



$(document).ready(function () {

    hideSpinner();

    initSelect2();

    applyFilters();



    $('#applyFilter').click(() => applyFilters());

    $('#clearFilter').click(() => clearFilters());

});