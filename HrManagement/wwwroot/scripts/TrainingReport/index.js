
function getRegisteredEmployeesByTraining(flatRows) {
    const result = {};

    flatRows.forEach(r => {
        const key = `${r.Training} (ID: ${r.TrainingId})`;
        if (!result[key]) result[key] = new Set();
        result[key].add(r.EmployeeId); // EmployeeId is unique per employee
    });

    return Object.entries(result).map(([trainingLabel, empSet]) => ({
        Training: trainingLabel,
        RegisteredEmployees: empSet.size
    }));
}


function isEmployeeEligibleForTraining(emp, training) {
    // Parse Employees array from training, map to numbers
    const empCodes = parseJsonArraySafe(training.Employees).map(Number);

    if (empCodes.length > 0) {
        // Check if employee's EmployeeCode matches any in training employees list
        return empCodes.includes(emp.EmployeeCode);
    }

    // Existing Department and Site filtering logic remains
    const deptIds = parseJsonArraySafe(training.Departments).map(String);
    const empDeptIds = parseJsonArraySafe(emp.DepartmentName).map(d => String(d.id));
    if (deptIds.length > 0 && !empDeptIds.some(d => deptIds.includes(d))) {
        return false;
    }

    const siteIds = parseJsonArraySafe(training.Sites).map(String);
    const empSiteIds = parseJsonArraySafe(emp.SiteName).map(s => String(s.id));
    if (siteIds.length > 0 && !empSiteIds.some(s => siteIds.includes(s))) {
        return false;
    }

    return true; // open if no restrictions
}


/* ==========================
   JSON Flattening
   ========================== */
function getFlattenedReport(json) {
    const safeArray = arr => Array.isArray(arr) ? arr : [];
    const flatRows = [];
    const coursesProgressArray = safeArray(json.CoursesProgress);

    safeArray(json.Trainings).forEach(t => {
        console.log('Training:', t.TrainingTitle);

        const trainingTitle = t?.TrainingTitle || "";
        const semesters = safeArray(json.Semester).filter(s => String(s?.TrainingId) === String(t?.TrainingId));
        console.log(semesters);

        const courses = safeArray(json.Courses).filter(c => semesters.some(s => s.SemesterId == c.SemesterId));
        console.log(courses);
        if (courses.length === 0) {
            console.warn(`No courses found for training ${trainingTitle}`);
        }

        safeArray(json.Employees).forEach(emp => {
            const eligible = isEmployeeEligibleForTraining(emp, t);
            console.log(`Employee ${emp.FirstName} ${emp.LastName} eligible for ${trainingTitle}:`, eligible);

            if (!eligible) return;

            courses.forEach(c => {
                // Find progress entry matching employee code and course id
                const progress = coursesProgressArray.find(progress =>
                    progress.EmployeeId === emp.EmployeeCode && progress.CourseId === c.CourseId
                );

                flatRows.push({
                    EmployeeId: emp.EmployeeCode || null,    // EmployeeCode as unique Id
                    EmployeeName: (emp?.FirstName || "") + " " + (emp?.LastName || ""),
                    Department: parseJsonArraySafe(emp.DepartmentName).map(d => d.name).join(", ") || "",
                    SubDept: parseJsonArraySafe(emp.DepartmentSubCategoryName).map(s => s.name).join(", ") || "",
                    Site: parseJsonArraySafe(emp.SiteName).map(s => s.name).join(", ") || "",
                    TrainingId: t?.TrainingId || null, // <--- Add this
                    Training: trainingTitle,
                    Course: c?.CourseTitle || "",
                    CourseType: c?.CourseType,
                    IsCompleted: progress ? progress.IsCompleted : false,
                    QuizScore: progress && progress.QuizScore ? progress.QuizScore : '-',
                    PassFail: progress && progress.PassFail ? progress.PassFail : '-'
                });
            });
        });
    });

    console.log('Generated flatRows:', flatRows.length);
    return flatRows;
}


function parseJsonArraySafe(str) {
    if (!str) return [];
    try {
        return JSON.parse(str);
    } catch {
        return [];
    }
}

function getEmployeeDepartments(emp) {
    return parseJsonArraySafe(emp.DepartmentName).map(d => d.name.trim());
}

function getEmployeeSubDepts(emp) {
    return parseJsonArraySafe(emp.DepartmentSubCategoryName).map(d => d.name.trim());
}

function getEmployeeSites(emp) {
    return parseJsonArraySafe(emp.SiteName).map(d => d.name.trim());
}
/* ==========================
   Filters & Rendering
   ========================== */
function populateFilters(rows, jsonData) {
    const uniqueValues = (arr) => [...new Set(arr.map(s => s.trim()))].filter(Boolean);

    // Departments from JSON + employee rows
    const allDepartments = [
        ...jsonData.Departments.map(d => d.DepartmentName),
        ...rows.flatMap(r => r.Department.split(',').map(s => s.trim()))
    ];

    const allSubDepts = rows.flatMap(r => r.SubDept.split(',').map(s => s.trim()));
    const allSites = rows.flatMap(r => r.Site.split(',').map(s => s.trim()));
    const allTrainings = uniqueValues(
        rows.map(r => `${r.Training} (ID: ${r.TrainingId})`)
    ).sort();



    // Employees: include all from JSON
    const allEmployees = [
        ...jsonData.Employees.map(e => `${e.FirstName} ${e.LastName}`),
        ...rows.map(r => r.EmployeeName)
    ];

    const fillSelect = (selId, arr) => {
        const sel = $(selId).empty().append('<option value="">All</option>');
        uniqueValues(arr).forEach(v => sel.append(`<option value="${v}">${v}</option>`));
    };

    fillSelect('#filterDepartment', allDepartments);
    fillSelect('#filterSubDept', allSubDepts);
    fillSelect('#filterSite', allSites);
    fillSelect('#filterTraining', allTrainings);
    fillSelect('#filterEmployee', allEmployees);
}




function renderFlatTable(rows) {
    const $tb = $('#flatTable tbody').empty();
    rows.forEach(r => {

        // ===== Course Type Icon =====
        let courseIcon = '';
        if (r.CourseType === '1') courseIcon = '<i class="bi bi-camera-video-fill text-primary"></i>'; // Video
        else if (r.CourseType === '2') courseIcon = '<i class="bi bi-journal-text text-info"></i>'; // Lesson
        else if (r.CourseType === '3') courseIcon = '<i class="bi bi-clipboard-check-fill text-warning"></i>'; // Quiz
        else courseIcon = '<i class="bi bi-question-circle-fill text-secondary"></i>';

        // ===== Completion / Quiz Pass-Fail Icon =====
        let statusIcon = '';
        if (r.CourseType === '1' || r.CourseType === '2') {
            statusIcon = r.IsCompleted ? '<i class="bi bi-check-circle-fill text-success"></i>' : '<i class="bi bi-x-circle-fill text-danger"></i>';
        } else if (r.CourseType === '3') {
            if (r.PassFail === 'Pass') statusIcon = '<i class="bi bi-check-circle-fill text-success"></i>';
            else if (r.PassFail === 'Fail') statusIcon = '<i class="bi bi-x-circle-fill text-danger"></i>';
            else statusIcon = '<i class="bi bi-question-circle-fill text-secondary"></i>';
        }

        $tb.append(`<tr>
        <td>${escapeHtml(r.EmployeeName)}</td>
        <td>${escapeHtml(r.Department)}</td>
        <td>${escapeHtml(r.SubDept)}</td>
        <td>${escapeHtml(r.Site)}</td>
        <td>${escapeHtml(r.Training)}</td>
        <td>${courseIcon} ${escapeHtml(r.Course)}</td>
        <td class="text-center">${statusIcon}</td>
        <td class="text-center">${r.QuizScore}</td>
        <td class="text-center">${r.PassFail}</td>
    </tr>`);
    });
}




function escapeHtml(text) {
    return $('<div>').text(text).html();
}

/* ==========================
   Charts
   ========================== */
let chartCompletion, chartPassFail, chartDept, chartSubDept, chartSite, chartTraining;

function renderCharts(rows) {
    const colors = ['#0d6efd', '#198754', '#dc3545', '#ffc107', '#6f42c1', '#fd7e14', '#20c997', '#0dcaf0', '#6610f2', '#e83e8c'];

    // Training Completion % by Employee
    const empLabels = [...new Set(rows.map(r => r.EmployeeName))];
    const empData = empLabels.map(emp => {
        const empRows = rows.filter(r => r.EmployeeName === emp);
        const completed = empRows.filter(r => r.IsCompleted).length;
        return empRows.length > 0 ? (completed / empRows.length) * 100 : 0;
    });

    if (chartCompletion) chartCompletion.destroy();
    chartCompletion = new Chart(document.getElementById('chartCompletion'), {
        type: 'bar',
        data: {
            labels: empLabels,
            datasets: [{
                label: 'Completion %',
                data: empData,
                backgroundColor: colors,
                barThickness: 20,        // fixed thickness
                maxBarThickness: 30      // maximum thickness
            }]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const employee = context.label;
                            const empRows = rows.filter(r => r.EmployeeName === employee);

                            const trainings = [...new Set(empRows.map(r => r.Training))].join(", ");
                            const value = context.raw.toFixed(2) + '%';

                            return [
                                `Completion: ${value}`,
                                `Trainings: ${trainings}`
                            ];
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100,
                    title: {
                        display: true,
                        text: 'Completion %'
                    }
                }
            }
        }
    });


    // Pass/Fail per Training
    const trainLabels = [...new Set(rows.map(r => r.Training))];
    const passData = trainLabels.map(t => rows.filter(r => r.Training === t && r.PassFail === 'Pass').length);
    const failData = trainLabels.map(t => rows.filter(r => r.Training === t && r.PassFail === 'Fail').length);
    if (chartPassFail) chartPassFail.destroy();
    chartPassFail = new Chart(document.getElementById('chartPassFail'), {
        type: 'bar',
        data: {
            labels: trainLabels,
            datasets: [
                { label: 'Pass', data: passData, backgroundColor: '#198754', barThickness: 15, maxBarThickness: 25 },
                { label: 'Fail', data: failData, backgroundColor: '#dc3545', barThickness: 15, maxBarThickness: 25 }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { y: { beginAtZero: true } }
        }
    });

    // Registered Employees by Department (unique EmployeeIds)
    const deptLabels = [...new Set(rows.map(r => r.Department))];
    const deptData = deptLabels.map(d => {
        const uniqueEmps = new Set(rows.filter(r => r.Department === d).map(r => r.EmployeeId));
        return uniqueEmps.size;
    });
    if (chartDept) chartDept.destroy();
    chartDept = new Chart(document.getElementById('chartDept'), {
        type: 'pie',
        data: {
            labels: deptLabels,
            datasets: [{ data: deptData, backgroundColor: colors }]
        }
    });

    // SubDepartment (unique EmployeeIds)
    const subLabels = [...new Set(rows.map(r => r.SubDept))];
    const subData = subLabels.map(d => {
        const uniqueEmps = new Set(rows.filter(r => r.SubDept === d).map(r => r.EmployeeId));
        return uniqueEmps.size;
    });
    if (chartSubDept) chartSubDept.destroy();
    chartSubDept = new Chart(document.getElementById('chartSubDept'), {
        type: 'pie',
        data: {
            labels: subLabels,
            datasets: [{ data: subData, backgroundColor: colors }]
        }
    });

    // Sites (unique EmployeeIds)
    const siteLabels = [...new Set(rows.map(r => r.Site))];
    const siteData = siteLabels.map(d => {
        const uniqueEmps = new Set(rows.filter(r => r.Site === d).map(r => r.EmployeeId));
        return uniqueEmps.size;
    });
    if (chartSite) chartSite.destroy();
    chartSite = new Chart(document.getElementById('chartSite'), {
        type: 'pie',
        data: {
            labels: siteLabels,
            datasets: [{ data: siteData, backgroundColor: colors }]
        }
    });



    // Trainings
    // Trainings - unique employees per training
    const trainingLabels = [...new Set(rows.map(r => `${r.Training} (ID: ${r.TrainingId})`))];
    const trainingData = trainingLabels.map(trainingLabel => {
        const matchingRows = rows.filter(r => `${r.Training} (ID: ${r.TrainingId})` === trainingLabel);
        const uniqueEmps = new Set(matchingRows.map(r => r.EmployeeId));
        return uniqueEmps.size;
    });
    if (chartTraining) chartTraining.destroy();
    chartTraining = new Chart(document.getElementById('chartTraining'), {
        type: 'pie',
        data: {
            labels: trainingLabels,
            datasets: [{
                data: trainingData,
                backgroundColor: colors
            }]
        }
    });
}

/* ==========================
   Employee Registration Summary
   ========================== */
/* ==========================
   Employee Registration Summary (Unique Employees)
   ========================== */
/* ==========================
   Employee Registration Summary (Unique Employees + List)
   ========================== */
function renderRegistrationSummary(rows) {
    const summary = {};

    rows.forEach(r => {
        const trainingLabel = `${r.Training} (ID: ${r.TrainingId})`;

        if (!summary[trainingLabel]) summary[trainingLabel] = {};
        if (!summary[trainingLabel][r.Department]) summary[trainingLabel][r.Department] = {};
        if (!summary[trainingLabel][r.Department][r.SubDept]) summary[trainingLabel][r.Department][r.SubDept] = {};
        if (!summary[trainingLabel][r.Department][r.SubDept][r.Site])
            summary[trainingLabel][r.Department][r.SubDept][r.Site] = new Map();

        summary[trainingLabel][r.Department][r.SubDept][r.Site].set(r.EmployeeId, {
            id: r.EmployeeId,
            code: r.EmployeeCode,
            name: r.EmployeeName
        });

        });
   

    const $accordion = $('#trainingSummaryAccordion').empty();
    let idx = 0;

    for (const training in summary) {
        const itemId = `collapseTraining${idx++}`;
        const headerId = `heading${idx}`;

        let totalEmps = new Set(); // training-level unique employees
        let contentHtml = '';

        for (const dept in summary[training]) {
            contentHtml += `<h6 class="mt-2">Department: ${dept}</h6>`;
            for (const sub in summary[training][dept]) {
                contentHtml += `<p class="ms-3 fw-semibold">SubDept: ${sub}</p>`;
                for (const site in summary[training][dept][sub]) {
                    const empMap = summary[training][dept][sub][site];
                    const employees = Array.from(empMap.values());
                    employees.forEach(e => totalEmps.add(e.id));

                    // employee list under this site
                    const empList = employees.map(e =>
                        `<li>${e.name}</li>`).join("");

                    contentHtml += `
        <p class="ms-5">Site: ${site} → <strong>${employees.length}</strong> employees</p>
        <ul class="ms-7 small text-muted">${empList}</ul>
        `;
                }
            }
        }

        $accordion.append(`
        <div class="accordion-item">
            <h2 class="accordion-header" id="${headerId}">
                <button class="accordion-button collapsed" type="button"
                    data-bs-toggle="collapse" data-bs-target="#${itemId}"
                    aria-expanded="false" aria-controls="${itemId}">
                    🎓 Training: ${training} → <strong>(${totalEmps.size})</strong>  employees
                </button>
            </h2>
            <div id="${itemId}" class="accordion-collapse collapse"
                aria-labelledby="${headerId}" data-bs-parent="#trainingSummaryAccordion">
                <div class="accordion-body">
                    ${contentHtml}
                </div>
            </div>
        </div>
        `);
    }
}

function applyFilters(rows) {
    const dep = $('#filterDepartment').val();
    const sub = $('#filterSubDept').val();
    const site = $('#filterSite').val();
    const training = $('#filterTraining').val();

    const emp = $('#filterEmployee').val();

    return rows.filter(r => {
        const matchDep = dep ? r.Department.includes(dep) : true;
        const matchSub = sub ? r.SubDept.includes(sub) : true;
        const matchSite = site ? r.Site.includes(site) : true;
        const matchTraining = training ? `${r.Training} (ID: ${r.TrainingId})` === training : true;

        const matchEmp = emp ? r.EmployeeName.includes(emp) : true;
        return matchDep && matchSub && matchSite && matchTraining && matchEmp;
    });
}


/* ==========================
   Initialization
   ========================== */
$(document).ready(function () {
    const jsonData = DropDownsData;
    console.log(jsonData);
    hideSpinner();
    const flatRows = getFlattenedReport(jsonData);
   
    const chartData = getRegisteredEmployeesByTraining(flatRows);
    console.log(flatRows);
    console.log(jsonData);
    populateFilters(flatRows, jsonData);
    renderFlatTable(flatRows);
    renderCharts(flatRows);
    renderRegistrationSummary(flatRows);

    $('#filterDepartment, #filterSubDept, #filterSite, #filterTraining, #filterEmployee').on('change', function () {
        const filteredRows = applyFilters(flatRows);
        renderFlatTable(filteredRows);
        renderCharts(filteredRows);
        renderRegistrationSummary(filteredRows);
    });


});