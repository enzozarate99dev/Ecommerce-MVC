const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    }, 
    busquedaVenta: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }

}

$(document).ready(function () {
    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

})

$("#cboBuscarPor").change(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }

})

$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {

        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha de inicio y fin")
            return;
        }

    } else {

        if ($("#txtNumeroVenta").val().trim() == "" ) {
            toastr.warning("", "Debe ingresar el numero de venta")
            return;
        }
    }

    let nroVenta = $("#txtNumeroVenta").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()

    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?nroVenta=${nroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(res => {
            $(".card-body").find("div.row").LoadingOverlay("hide");

            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {

            $("#tbventa tbody").html("")

            if (resJson.length > 0) {
                resJson.forEach((venta) => {
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("venta", venta)
                            )
                        )
                    )
                })
            }



           
        })
})

$("#tbventa tbody").on("click", ".btn-info", function () {

    let d = $(this).data("venta")

    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtNumVenta").val(d.numeroVenta)
    $("#txtUsuarioRegistro").val(d.usuario)
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtSubTotal").val(d.subTotal)
    $("#txtIVA").val(d.impuestoTotal)
    $("#txtTotal").val(d.total)

    $("#tbProductos tbody").html("");

    d.detalleVenta.forEach((elem) => {
        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(elem.descripcionProducto),
                $("<td>").text(elem.cantidad),
                $("<td>").text(elem.precio),
                $("<td>").text(elem.total)
                
            )
        )
    })

    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?nroVenta=${d.numeroVenta}`)


    $("#modalData").modal("show")

})