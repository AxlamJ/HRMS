
HRMSUtil.onDOMContentLoaded(function () {


    $("#datefrom").flatpickr({
        dateFormat: "m/Y",
        defaultDate: new Date(new Date().getFullYear() - 1, new Date().getMonth() - 1, 1)
    });

    $("#dateto").flatpickr({
        dateFormat: "m/Y",
        defaultDate: new Date(),
    });
    GetHeadCountStats();
    hideSpinner()
});

$(document).on('click', '#btnSearch', function () {
    GetHeadCountStats();
})

function GetHeadCountStats() {


    var startmonth = $("#datefrom").val().split("/")[0];
    var startyear = $("#datefrom").val().split("/")[1];
    var endmonth = $("#dateto").val().split("/")[0];
    var endyear = $("#dateto").val().split("/")[1];

    var url = sitePath + "api/StatsAPI/GetHeadCountStats?StartMonth=" + startmonth
        + "&StartYear=" + startyear
        + "&StartYear=" + startyear
        + "&EndMonth=" + endmonth
        + "&EndYear=" + endyear;
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var HeadCountStats = resp.HeadCountStats;

            ShowStats(HeadCountStats);
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


function ShowStats(HeadCountStats) {



    var headCountChartDom = document.getElementById('headcount_stats');
    var headCountChart = echarts.init(headCountChartDom);
    var headCountoption;
    // Pre-calculate change from previous month
    const changes = HeadCountStats.yAxisSeries.map((val, i) => {
        if (i === 0) return 0;
        return val - HeadCountStats.yAxisSeries[i - 1];
    });
    headCountoption = {
        tooltip: {
            trigger: 'axis',
            formatter: function (params) {
                const i = params[0].dataIndex;
                const month = HeadCountStats.xAxis[i];
                const count = HeadCountStats.yAxisSeries[i];
                const change = changes[i];
                const changeText = change == 0 ? ` <i class="fa fa-arrow-right" style="color:#f6c000">&nbsp;</i>${change}` : (change > 0 ? `  <i class="fa fa-arrow-up" style="color:green">&nbsp;</i>+${change}` : `  <i class="fa fa-arrow-down" style="color:red">&nbsp;</i>${change}`);
                return `
        <b>${month}</b><br/>
        <span class="d-flex justify-content-center"><b>${count}&nbsp;&nbsp;</b>
        <b style="color:${change == 0 ? '#f6c000' : (change >= 0 ? 'green' : 'red')}"> ${changeText}</b></span>employees
      `;
            }
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: HeadCountStats.xAxis,
            axisLabel: {
                rotate: 45,
                interval: 0 // Ensures all labels are shown
            }
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                data: HeadCountStats.yAxisSeries,
                type: 'line',
                symbolSize: 12, // Increase the size of dots here
                symbol: 'circle',     // Makes dots solid circles (default)
                itemStyle: {
                    color: '#3b82f6',   // Solid dot color
                    borderColor: '#3b82f6',
                    borderWidth: 0      // No white border (makes it solid)
                },
                areaStyle: {
                    opacity: 0.3
                },
                smooth: true,
                emphasis: {
                    focus: 'series',
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    },
                    lineStyle: {
                        width: 4
                    },
                    label: {
                        show: true,
                        formatter: function (params) {
                            return `${params.data}`;
                        },
                        fontWeight: 'bold',
                        position: 'top'
                    }
                }
            }
        ]
    };

    headCountoption && headCountChart.setOption(headCountoption);


    var sum = HeadCountStats.yAxisSeries.reduce(function (acc, val) {
        return acc + val;
    }, 0);

    var average = Math.ceil(sum / HeadCountStats.yAxisSeries.length);

    var min = Math.min(...HeadCountStats.yAxisSeries);
    var max = Math.max(...HeadCountStats.yAxisSeries);
    var difference = max - min;

    $("#avg-employee").text(average)
    $("#new-employee").text(difference)

    //var totalEmployees = DepartmentStats.reduce(function (sum, item) {
    //    return sum + item.totalcount;
    //}, 0);

    //$("#current-date").text(FormatDate(new Date()))
    //var departmentRow = document.getElementById("department-wise-stats");

    //for (var i = 0; i < DepartmentStats.length; i++) {

    //    var templateHtml = $("#department-template").html(); // get template content
    //    var $template = $(templateHtml);               // convert to jQuery element

    //    // Fill in data
    //    $template.find("#departmentname").text(DepartmentStats[i].name);
    //    $template.find("#department-total").text(DepartmentStats[i].totalcount);
    //    $template.find("#department-percent").text(DepartmentStats[i].value + "%");

    //    // Append to container
    //    $(departmentRow).append($template);
    //}
}