
HRMSUtil.onDOMContentLoaded(function () {


    $("#datefrom").flatpickr({
        dateFormat: "m/Y",
        defaultDate: new Date(new Date().getFullYear() - 1, new Date().getMonth() - 1, 1)
    });

    $("#dateto").flatpickr({
        dateFormat: "m/Y",
        defaultDate: new Date(),
    });
    GetHiredVsLeftStats();
    hideSpinner()
});

$(document).on('click', '#btnSearch', function () {
    GetHiredVsLeftStats();
})

function GetHiredVsLeftStats() {


    var startmonth = $("#datefrom").val().split("/")[0];
    var startyear = $("#datefrom").val().split("/")[1];
    var endmonth = $("#dateto").val().split("/")[0];
    var endyear = $("#dateto").val().split("/")[1];

    var url = sitePath + "api/StatsAPI/GetHiredVsLeftStats?StartMonth=" + startmonth
        + "&StartYear=" + startyear
        + "&StartYear=" + startyear
        + "&EndMonth=" + endmonth
        + "&EndYear=" + endyear;
    Ajax.get(url, function (resp) {
        if (resp.StatusCode == 200) {
            var HiredVsLeftStats = resp.HiredVsLeftStats;

            ShowStats(HiredVsLeftStats);
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


function ShowStats(HiredVsLeftStats) {



    const months = HiredVsLeftStats.map(x => x.MonthYear);
    const hiredData = HiredVsLeftStats.map(x => x.Hired);
    const leftData = HiredVsLeftStats.map(x => x.Left);

    const hiredvsLeftchartDom = document.getElementById('hired_left_stats');
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