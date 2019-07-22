
function AjaxGraph(graphId, ajaxUrl, canvasElementId) {
    this.graphId = graphId;
    this.ajaxUrl = ajaxUrl;
    this.canvasElement = document.getElementById(canvasElementId);
}

AjaxGraph.prototype.createChart = function (response) {

    var chartParams = {
        type: 'line',
        data: {
            labels: response.labels,
            datasets: response.datasets
        },
        options: {
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    type: 'linear',
                    ticks: {
                        display: true,
                        beginAtZero: true,
                        suggestedMax: response.suggestedMaxY // 100%
                    }
                }]
            },
            spanGaps: true
        }
    };

    var ctx = this.canvasElement.getContext('2d');

    this.chart = new Chart(ctx, chartParams);
}

AjaxGraph.prototype.queryAndUpdateChart = function () {

    var t = this;

    this.queryGraphData(function (response) {

        t.updateChartFromResponse(response);
    })
}

AjaxGraph.prototype.updateChartFromResponse = function (response) {
    this.chart.data.labels = response.labels;
    this.chart.data.datasets = response.datasets;

    this.chart.update(0);
}

