TestFlow.prototype.getMetrics = function (index) {
    $(_this.modal).find(".modal-title").html($scope.suite.children[index].name + " Metrics");
    $(_this.modal).find(".modal-body").html('<canvas id="myChart" width="400" height="400"></canvas>');
    $(_this.modal).modal('show');

    var ctx = $("#myChart").get(0).getContext("2d");

    var data = [
        {
            value: 25,
            color: "#F7464A",
            highlight: "#FF5A5E",
            label: "Product Failure",
            labelColor: 'Black',
            labelFontSize: '16'
        },
        {
            value: 100,
            color: "#46BFBD",
            highlight: "#5AD3D1",
            label: "Test Success"
        },
        {
            value: 15,
            color: "#FDB45C",
            highlight: "#FFC870",
            label: "Test Failure"
        }
    ];

    var options = {
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
    };

    var chart = new Chart(ctx).Doughnut(data, options);
    //var legend = chart.generateLegend();
    //$(".modal-body").append(legend);
}