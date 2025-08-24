
HRMSUtil.onDOMContentLoaded(function () {

    getStats();
    hideSpinner()
});


function getStats() {

    var url = sitePath + "api/StatsAPI/GetDashBoardStats";
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var DashBoardStats = resp.DashBoardStats;
            console.log(DashBoardStats)

            ShowStats(DashBoardStats);
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


function ShowStats(DashBoardStats) {
    var genderChartDom = document.getElementById('gender_stats');
    var genderChart = echarts.init(genderChartDom, null, { renderer: 'svg' });
    var genderoption = {
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        series: [
            {

                type: 'pie',
                radius: ['50%', '70%'],

                //radius: '50%',
                data: DashBoardStats.GenderStats,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    genderoption && genderChart.setOption(genderoption);

    var ageChartDom = document.getElementById('age_stats');
    var ageChart = echarts.init(ageChartDom, null, { renderer: 'svg' });
    var ageoption = {
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        series: [
            {

                type: 'pie',
                radius: ['50%', '70%'],

                //radius: '50%',
                data: DashBoardStats.AgeStats,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    ageoption && ageChart.setOption(ageoption);


    var siteChartDom = document.getElementById('site_stats');
    var siteChart = echarts.init(siteChartDom, null, { renderer: 'svg' });
    var siteoption = {
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        series: [
            {

                type: 'pie',
                radius: ['50%', '70%'],
                //radius: '50%',
                data: DashBoardStats.SiteStats,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    siteoption && siteChart.setOption(siteoption);

    var departmentChartDom = document.getElementById('department_stats');
    var departmentChart = echarts.init(departmentChartDom, null, { renderer: 'svg' });
    var departmentoption = {
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c}%'
        },
        series: [
            {

                type: 'pie',
                radius: ['50%', '70%'],
                //radius: '50%',
                data: DashBoardStats.DepartmentStats,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    departmentoption && departmentChart.setOption(departmentoption);


    var headCountChartDom = document.getElementById('headcount_stats');
    var headCountChart = echarts.init(headCountChartDom);
    var headCountoption;
    // Pre-calculate change from previous month
    const changes = DashBoardStats.HeadCountStats.yAxisSeries.map((val, i) => {
        if (i === 0) return 0;
        return val - DashBoardStats.HeadCountStats.yAxisSeries[i - 1];
    });
    headCountoption = {
        tooltip: {
            trigger: 'axis',
            formatter: function (params) {
                const i = params[0].dataIndex;
                const month = DashBoardStats.HeadCountStats.xAxis[i];
                const count = DashBoardStats.HeadCountStats.yAxisSeries[i];
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
            data: DashBoardStats.HeadCountStats.xAxis,
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
                data: DashBoardStats.HeadCountStats.yAxisSeries,
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





    // Prepare series data
    const months = DashBoardStats.HiredVsLeft.map(x => x.MonthYear);
    const hiredData = DashBoardStats.HiredVsLeft.map(x => x.Hired);
    const leftData = DashBoardStats.HiredVsLeft.map(x => x.Left);

    const hiredvsLeftchartDom = document.getElementById('hiredvsleft_stats');
    const hiredvsLeftChart = echarts.init(hiredvsLeftchartDom);

    const hiredvsLeftoption = {
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ['Hired', 'Left'],
            orient: 'horizontal',
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: {
            type: 'category',
            data: months,
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
                name: 'Hired',
                type: 'bar',
                data: hiredData,
                itemStyle: {
                    color: '#4caf50'
                }
            },
            {
                name: 'Left',
                type: 'bar',
                data: leftData,
                itemStyle: {
                    color: '#f44336'
                }
            }
        ]
    };

    hiredvsLeftoption && hiredvsLeftChart.setOption(hiredvsLeftoption);
}