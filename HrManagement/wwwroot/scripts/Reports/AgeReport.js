
HRMSUtil.onDOMContentLoaded(function () {

    getAgeStats();
    hideSpinner()
});


function getAgeStats() {

    var url = sitePath + "api/StatsAPI/GetAgeStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var AgeStats = resp.AgeStats;

            ShowStats(AgeStats);
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


function ShowStats(AgeStats) {

    var chartData = AgeStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var siteChartDom = document.getElementById('age-stats');
    var siteChart = echarts.init(siteChartDom, null, { renderer: 'svg' });
    var siteoption = {
        title: {
            text: 'Employee Age',
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


    var totalEmployees = AgeStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var ageRow = document.getElementById("age-wise-stats");

    for (var i = 0; i < AgeStats.length; i++) {

        var templateHtml = $("#age-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#agename").text(AgeStats[i].name);
        $template.find("#age-total").text(AgeStats[i].totalcount);
        $template.find("#age-percent").text(AgeStats[i].value+"%");

        // Append to container
        $(ageRow).append($template);
    }
}