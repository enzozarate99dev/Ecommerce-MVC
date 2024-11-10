﻿let tablaData
$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Reporte/ReporteVentas?fechaInicio=01/01/2001&fechaFin=01/01/2001',
            "type": "GET",
            "datatype": "json",

        },
        "columns": [

            { "data": "fechaRegistro" },
            { "data": "nroVenta" },
            { "data": "tipoDocumento" },
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "subtotalVenta" },
            { "data": "impuestoTotalVenta" },
            { "data": "totalVenta" },
            { "data": "producto" },
            { "data": "cantidad" },
            { "data": "precio" },
            { "data": "total" },
            
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Ventas',
                
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

$("#btnBuscar").click(function () {

    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
        toastr.warning("", "Debe ingresar fecha de inicio y fin")
        return;
    }
    let fInicio = $("#txtFechaInicio").val().trim()
    let fFin = $("#txtFechaFin").val().trim()

    let nuevaUrl = ` /Reporte/ReporteVentas?fechaInicio=${fInicio}&fechaFin=${fFin}`

    tablaData.ajax.url(nuevaUrl).load()

})