
// Still use old class style for IE11 desktop users

function InputGraph(graphId, ajaxUrl, canvasElementId, medianInputElementId, dispersionInputElementId, skewInputElementId) {

    AjaxGraph.call(this, graphId, ajaxUrl, canvasElementId);

    this.medianInput = document.getElementById(medianInputElementId);
    this.dispersionInput = document.getElementById(dispersionInputElementId);
    this.skewInput = document.getElementById(skewInputElementId);
}

InputGraph.prototype = Object.create(AjaxGraph.prototype);

InputGraph.prototype.initGraph = function (onSuccess) {

    var chart;

    var t = this;

    var updateGraphFunction = function (event) {
        t.queryAndUpdateChart();

        t.listeners.forEach(function (listener) {
            listener(t.getGraphSelection());
        })
    }

    this.medianInput.onchange = updateGraphFunction;
    this.dispersionInput.oninput = updateGraphFunction;
    this.skewInput.oninput = updateGraphFunction;

    this.listeners = [];

    var t = this;

    this.queryGraphData(function (response) {

        t.createChart(response);

        onSuccess();
    });
}

InputGraph.prototype.queryGraphData = function (onSuccess) {

    axios.get(this.ajaxUrl
        + '?graphId=' + this.graphId
        + '&median=' + this.medianInput.value
        + '&dispersion=' + this.dispersionInput.value
        + '&skew=' + this.skewInput.value

    ).then(function (response) {
        onSuccess(response.data);
    })
}

InputGraph.prototype.getGraphSelection = function () {

    return {
        median: this.medianInput.value,
        dispersion: this.dispersionInput.value,
        skew: this.skewInput.value
    };

}

InputGraph.prototype.addGraphUpdateListener = function (listener) {
    this.listeners.push(listener);
}
