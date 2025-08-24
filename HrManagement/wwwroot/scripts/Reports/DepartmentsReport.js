
HRMSUtil.onDOMContentLoaded(function () {

    GetDepartmentStats();
    hideSpinner()
});


function GetDepartmentStats() {

    var url = sitePath + "api/StatsAPI/GetDepartmentStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var DepartmentStats = resp.DepartmentStats;

            ShowStats(DepartmentStats);
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


function ShowStats(DepartmentStats) {

    var chartData = DepartmentStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var siteChartDom = document.getElementById('department-stats');
    var siteChart = echarts.init(siteChartDom, null, { renderer: 'svg' });
    var siteoption = {
        title: {
            text: 'Departments',
            left: 'left'
        },
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        legend: {
            orient: 'vertical',   // vertical legend items
            right: 0,             // stick to the right edge
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

    siteoption && siteChart.setOption(siteoption);


    var totalEmployees = DepartmentStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var departmentRow = document.getElementById("department-wise-stats");

    for (var i = 0; i < DepartmentStats.length; i++) {

        var templateHtml = $("#department-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#departmentname").text(DepartmentStats[i].name);
        $template.find("#department-total").text(DepartmentStats[i].totalcount);
        $template.find("#department-percent").text(DepartmentStats[i].value+"%");

        // Append to container
        $(departmentRow).append($template);
    }
}