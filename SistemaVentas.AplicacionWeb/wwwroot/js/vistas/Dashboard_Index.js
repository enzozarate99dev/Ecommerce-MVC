$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    fetch("/Dashboard/ObtenerResumen")
        .then(res => {
            $("div.container-fluid").LoadingOverlay("hide");

            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {

            const d = resJson.objeto


            if (resJson.estado) {
                //datos cards

                $("#totalVenta").text(d.totalVentas)
                $("#totalIngresos").text(d.totalIngresos)
                $("#totalProductos").text(d.totalProductos)
                $("#totalCategorias").text(d.totalCategorias)
            }   

                //textos y valores del grafico de barras
                let barchart_labels;
                let barchart_data;

                if (d.ventasUltimaSemana.length > 0) {
                    barchart_labels = d.ventasUltimaSemana.map((it) => { return it.fecha })
                    barchart_data = d.ventasUltimaSemana.map((it) => { return it.total })
                } else {
                    barchart_labels = ["Sin resultados"]
                    barchart_data = [0]
                }

                //textos y valores del grafico de torta
                let pierchart_labels;
                let piechart_data;

                if (d.productosTopUltimaSemana.length > 0) {
                    pierchart_labels = d.productosTopUltimaSemana.map((it) => { return it.producto })
                    piechart_data = d.productosTopUltimaSemana.map((it) => { return it.cantidad })
                } else {
                    pierchart_labels = ["Sin resultados"]
                    piechart_data = [0]
                }

                // Barras
                let controlVenta = document.getElementById("chartVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

            // Torta
            let controlProducto = document.getElementById("chartProductos");
            let myPieChart = new Chart(controlProducto, {
                type: 'doughnut',
                data: {
                    labels: pierchart_labels,
                    datasets: [{
                        data: piechart_data,
                        backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                        hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: true
                    },
                    cutoutPercentage: 80,
                },
            });

        

        })
})
