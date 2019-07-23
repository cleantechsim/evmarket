
function ComputeGraph(graphId, ajaxUrl, canvasElementId) {
    AjaxGraph.call(this, graphId, ajaxUrl, canvasElementId);
}

ComputeGraph.prototype = Object.create(AjaxGraph.prototype);

