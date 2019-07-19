
// Still use old class style for IE11 desktop users

function DynamicGraph(graphId, ajaxUrl, canvasElementId, medianInputElementId, dispersionInputElementId, skewInputElementId) {

    this.graphId = graphId;
    this.ajaxUrl = ajaxUrl;
    this.canvasElement = document.getElementById(canvasElementId);
    this.medianInput = document.getElementById(medianInputElementId);
    this.dispersionInput = document.getElementById(dispersionInputElementId);
    this.skewInput = document.getElementById(skewInputElementId);
}

DynamicGraph.prototype.initChart = function () {

    var chart;

    var t = this;

    var updateGraphFunction = function (event) { t.updateGraph(chart); }

    this.medianInput.onchange = updateGraphFunction;
    this.dispersionInput.oninput = updateGraphFunction;
    this.skewInput.oninput = updateGraphFunction;

    var ctx = this.canvasElement.getContext('2d');

    this.queryGraphData(function (response) {

        chart = new Chart(ctx, {
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
        });
    })
}

DynamicGraph.prototype.updateGraph = function (chart) {

    this.queryGraphData(function (response) {
        chart.data.labels = response.labels;
        chart.data.datasets = response.datasets;

        chart.update(0);
    })
}

DynamicGraph.prototype.queryGraphData = function (onSuccess) {

    axios.get(this.ajaxUrl
        + '?graphId=' + this.graphId
        + '&median=' + this.medianInput.value
        + '&dispersion=' + this.dispersionInput.value
        + '&skew=' + this.skewInput.value

    ).then(function (response) {
        onSuccess(response.data);
    })
}

