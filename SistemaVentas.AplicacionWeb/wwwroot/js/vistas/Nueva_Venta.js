
let valorImpuesto = 0;
$(document).ready(function () {

    fetch("/Venta/ListaTipoDocumentoVenta")
        .then(res => {
            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {
            if (resJson.data.length > 0) {

                resJson.data.forEach((item) => {
                    $("#cboTipoDocumentoVenta").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    )
                })
            }
        })

    fetch("/Negocio/Obtener")
        .then(res => {
            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {
            if (resJson.estado) {
                const d = resJson.objeto
                $("#inputGroupSubTotal").text(`Sub total - ${d.simboloMoneda}`)
                $("#inputGroupIVA").text(`IVA (${d.porcentajeImpuesto}%) - ${d.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${d.simboloMoneda}`)

                valorImpuesto = parseFloat(d.porcentajeImpuesto)
                
            }
        })

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((elem) => (
                        {
                            id: elem.idProducto,
                            text: elem.descripcion,
                            marca: elem.marca,
                            categoria: elem.nombreCategoria,
                            urlImagen: elem.urlImagen,
                            precio: parseFloat(elem.precio)
                        }
                    ))

                };
            },
        },
        lenguage: "es",
        placeholder: 'Buscar producto',
        minimumInputLength: 1,
        templateResult: formatRepo,
    });
})

function formatRepo(data) {
    if (data.loading) {
        return data.text;
    }

    var container = $(
        `<table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
                </td>
                <td style="width:60px">
                    <p style="font-weight:bolder;margin:2px">${data.marca}</p>
                    <p style="margin:2px">${data.text}</p>
                </td>
            </tr>
        </table>
        `

    );
       
    return container;
}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let productosParaVenta = [];
$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data

    let productoEncontrado = productosParaVenta.filter(p => p.idProducto == data.id)
    if (productoEncontrado.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change")
        toast.warning("", "El producto ya fue agregado")
        return false;
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese Cantidad"
    },
        function (valor) {
            if (valor === false) return false;

            if (valor === "") {
                toastr.warning("", "Debe ingresar la cantidad")
                return false;
            }
            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ser un numero")
                return false;
            }

            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor) * data.precio).toString()
            }

            productosParaVenta.push(producto)

            mostrarProducto_Precios();
            $("#cboBuscarProducto").val("").trigger("change")
            swal.close()

        }
    )
})

function mostrarProducto_Precios() {

    let Total = 0;
    let iva = 0;
    let subtotal = 0;
    let porcentaje = valorImpuesto / 100;

    $("#tbProducto tbody").html("")

    productosParaVenta.forEach((i) => {

        Total += parseFloat(i.total)

        $("#tbProducto tbody").append(
            $("<td>").append(
                $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                    $("<i>").addClass("fas fa-trash-alt")
                ).data("idProducto", i.idProducto)
            ),
            $("<td>").text(i.descripcionProducto),
            $("<td>").text(i.cantidad),
            $("<td>").text(i.precio),
            $("<td>").text(i.total)
        )


    })

    subtotal = Total / (1 + porcentaje)
    iva = Total - subtotal

    $("#txtSubTotal").val(subtotal.toFixed(2))
    $("#txtIVA").val(iva.toFixed(2))
    $("#txtTotal").val(Total.toFixed(2))

}

$(document).on("click", "button.btn-eliminar", function () {

    const _idprod = $(this).data("idProducto")

    productosParaVenta = productosParaVenta.filter(p => p.idProducto != _idprod);
    
    mostrarProducto_Precios();
})
$("#btnTerminarVenta").click(function () {
    if (productosParaVenta.length < 1) {
        toastr.warning("", "Debe ingresar productos ")
        return;
    }

    const vmDetalleVenta = productosParaVenta;
    const venta = {
        idTipoDocumentoVenta: 1,
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: Number($("#txtSubTotal").val().replace(',', '.')),
        impuestoTotal: Number($("#txtIVA").val().replace(',', '.')),
        Total: Number($("#txtTotal").val().replace(',', '.')),
        detalleVenta: vmDetalleVenta
    }

    // Debugging - Ver datos antes de enviar
    console.log("Datos de venta antes de enviar:", venta);
    console.log("JSON a enviar:", JSON.stringify(venta));

    $("#btnTerminarVenta").LoadingOverlay("show");

    // Asegurarnos de que la URL es correcta
    const url = "/Venta/RegistrarVenta";
    console.log("URL de envío:", url);

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(venta)
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            console.log("Status de la respuesta:", response.status);
            console.log("Headers de la respuesta:", response.headers);

            if (!response.ok) {
                return response.text().then(text => {
                    console.log("Error response body:", text);
                    throw new Error(text);
                });
            }
            return response.json();
        })
        .then(resJson => {
            console.log("Respuesta exitosa:", resJson);
            if (resJson.estado) {
                productosParaVenta = [];
                mostrarProducto_Precios();
                $("#txtDocumentoCliente").val("");
                $("#txtNombreCliente").val("");
                swal("¡Registrado!", `Numero Venta : ${resJson.objeto.numeroVenta}`, "success");
            } else {
                swal("Ocurrió un error", resJson.mensaje || "No se pudo registrar la operación", "error");
            }
        })
        .catch(error => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            console.error("Error completo:", error);
            swal("Error", "No se pudo procesar la venta", "error");
        });
});