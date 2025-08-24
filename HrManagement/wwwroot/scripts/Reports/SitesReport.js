
HRMSUtil.onDOMContentLoaded(function () {

    getSiteStats();
    hideSpinner()
});


function getSiteStats() {

    var url = sitePath + "api/StatsAPI/GetSiteStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var SiteStats = resp.SiteStats;
            console.log(SiteStats)

            ShowStats(SiteStats);
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


function ShowStats(SiteStats) {

    var chartData = SiteStats.map(function (item) {
        return {
            name: item.name,
            value: item.value
        };
    });

    var siteChartDom = document.getElementById('site-stats');
    var siteChart = echarts.init(siteChartDom, null, { renderer: 'svg' });
    var siteoption = {
        title: {
            text: 'Sites',
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


    var totalEmployees = SiteStats.reduce(function (sum, item) {
        return sum + item.totalcount;
    }, 0);

    $("#total-employee").text(totalEmployees)
    $("#current-date").text(FormatDate(new Date()))
    var sitesRow = document.getElementById("site-wise-stats");

    for (var i = 0; i < SiteStats.length; i++) {

        var templateHtml = $("#site-template").html(); // get template content
        var $template = $(templateHtml);               // convert to jQuery element

        // Fill in data
        $template.find("#sitename").text(SiteStats[i].name);
        $template.find("#site-total").text(SiteStats[i].totalcount);
        $template.find("#site-percent").text(SiteStats[i].value+"%");

        // Append to container
        $(sitesRow).append($template);
    }
}