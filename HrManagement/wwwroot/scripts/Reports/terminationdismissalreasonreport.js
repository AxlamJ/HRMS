
HRMSUtil.onDOMContentLoaded(function () {

    getTerminationReasonsStats();
    hideSpinner()
});


function getTerminationReasonsStats() {

    var url = sitePath + "api/StatsAPI/GetTerminationDismissalReasonStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var TerminationReasonsStats = resp.TerminationStats;
            console.log(TerminationReasonsStats)

            ShowStats(TerminationReasonsStats);
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


function ShowStats(TerminationReasonsStats) {

    var chartData = TerminationReasonsStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var reasonsChartDom = document.getElementById('reason-stats');
    var resonsChart = echarts.init(reasonsChartDom, null, { renderer: 'svg' });
    var reasonsoption = {
        title: {
            text: 'Termination/ Dismissal Reasons',
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

    reasonsoption && resonsChart.setOption(reasonsoption);


    var totalEmployees = TerminationReasonsStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var resonsRow = document.getElementById("reason-wise-stats");

    for (var i = 0; i < TerminationReasonsStats.length; i++) {

        var templateHtml = $("#reason-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#reasonname").text(TerminationReasonsStats[i].name);
        $template.find("#reason-total").text(TerminationReasonsStats[i].totalcount);
        $template.find("#reason-percent").text(TerminationReasonsStats[i].value+"%");

        // Append to container
        $(resonsRow).append($template);
    }
}