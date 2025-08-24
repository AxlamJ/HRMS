
HRMSUtil.onDOMContentLoaded(function () {

    getGenderStats();
    hideSpinner()
});


function getGenderStats() {

    var url = sitePath + "api/StatsAPI/GetGenderStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var GenderStats = resp.GenderStats;
            console.log(GenderStats)

            ShowStats(GenderStats);
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


function ShowStats(GenderStats) {

    var chartData = GenderStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var siteChartDom = document.getElementById('gender-stats');
    var siteChart = echarts.init(siteChartDom, null, { renderer: 'svg' });
    var siteoption = {
        title: {
            text: 'Employee Gender',
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


    var totalEmployees = GenderStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var sitesRow = document.getElementById("gender-wise-stats");

    for (var i = 0; i < GenderStats.length; i++) {

        var templateHtml = $("#gender-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#gendername").text(GenderStats[i].name);
        $template.find("#gender-total").text(GenderStats[i].totalcount);
        $template.find("#gender-percent").text(GenderStats[i].value+"%");

        // Append to container
        $(sitesRow).append($template);
    }
}