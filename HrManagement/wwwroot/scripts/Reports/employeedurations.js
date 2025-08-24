
HRMSUtil.onDOMContentLoaded(function () {

    getWorkDurationStats();
    hideSpinner()
});


function getWorkDurationStats() {

    var url = sitePath + "api/StatsAPI/GetWorkDurationStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var WorkDurationStats = resp.WorkDurationStats;
            console.log(WorkDurationStats)

            ShowStats(WorkDurationStats);
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


function ShowStats(WorkDurationStats) {

    var chartData = WorkDurationStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var durationsChartDom = document.getElementById('duration-stats');
    var resonsChart = echarts.init(durationsChartDom, null, { renderer: 'svg' });
    var durationsoption = {
        title: {
            text: 'Work Duration',
            left: 'left'
        },
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        legend: {
            orient: 'horizontal',   // vertical legend items
            right: 0,            // stick to the right edge
            left: 0,            // stick to the left edge
            top: 'bottom'         // vertically centered
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

    durationsoption && resonsChart.setOption(durationsoption);


    var totalEmployees = WorkDurationStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var resonsRow = document.getElementById("duration-wise-stats");

    for (var i = 0; i < WorkDurationStats.length; i++) {

        var templateHtml = $("#duration-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#durationname").text(WorkDurationStats[i].name);
        $template.find("#duration-total").text(WorkDurationStats[i].totalcount);
        $template.find("#duration-percent").text(WorkDurationStats[i].value+"%");

        // Append to container
        $(resonsRow).append($template);
    }
}