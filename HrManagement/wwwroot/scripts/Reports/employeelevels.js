
HRMSUtil.onDOMContentLoaded(function () {

    getEmployeeLevelsStats();
    hideSpinner()
});


function getEmployeeLevelsStats() {

    var url = sitePath + "api/StatsAPI/GetEmployeeLevelStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var EmployeeLevelstats = resp.EmployeeLevelStats;
            console.log(EmployeeLevelstats)

            ShowStats(EmployeeLevelstats);
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


function ShowStats(EmployeeLevelstats) {

    var chartData = EmployeeLevelstats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var levelsChartDom = document.getElementById('level-stats');
    var levelChart = echarts.init(levelsChartDom, null, { renderer: 'svg' });
    var levelsoption = {
        title: {
            text: 'Employee Levels',
            left: 'left'
        },
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        legend: {
            orient: 'vertical',   // vertical legend items
            right: 0,            // stick to the right edge
            top: 'center'         // vertically centered
        },
        series: [
            {

                type: 'pie',
                radius: '80%',
                data: chartData ,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },
                label: {
                    formatter: '{b}: {c}%'
                }
            }
        ]
    };

    levelsoption && levelChart.setOption(levelsoption);


    var totalEmployees = EmployeeLevelstats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var levelRow = document.getElementById("level-wise-stats");

    for (var i = 0; i < EmployeeLevelstats.length; i++) {

        var templateHtml = $("#level-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#levelname").text(EmployeeLevelstats[i].name);
        $template.find("#level-total").text(EmployeeLevelstats[i].totalcount);
        $template.find("#level-percent").text(EmployeeLevelstats[i].value+"%");

        // Append to container
        $(levelRow).append($template);
    }
}