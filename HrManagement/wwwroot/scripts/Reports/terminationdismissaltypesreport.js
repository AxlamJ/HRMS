
HRMSUtil.onDOMContentLoaded(function () {

    getTerminationTypesStats();
    hideSpinner()
});


function getTerminationTypesStats() {

    var url = sitePath + "api/StatsAPI/GetTerminationDismissalTypeStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var TerminationTypeStats = resp.TerminationTypeStats;
            console.log(TerminationTypeStats)

            ShowStats(TerminationTypeStats);
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


function ShowStats(TerminationTypeStats) {

    var chartData = TerminationTypeStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var typesChartDom = document.getElementById('type-stats');
    var resonsChart = echarts.init(typesChartDom, null, { renderer: 'svg' });
    var typesoption = {
        title: {
            text: 'Termination/ Dismissal Types',
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

    typesoption && resonsChart.setOption(typesoption);


    var totalEmployees = TerminationTypeStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var resonsRow = document.getElementById("type-wise-stats");

    for (var i = 0; i < TerminationTypeStats.length; i++) {

        var templateHtml = $("#type-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#typename").text(TerminationTypeStats[i].name);
        $template.find("#type-total").text(TerminationTypeStats[i].totalcount);
        $template.find("#type-percent").text(TerminationTypeStats[i].value+"%");

        // Append to container
        $(resonsRow).append($template);
    }
}