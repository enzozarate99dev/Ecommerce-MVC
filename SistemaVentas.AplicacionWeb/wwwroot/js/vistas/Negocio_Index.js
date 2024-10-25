$(document).ready(function () {

    $(".card-body").LoadingOverlay("show");

    fetch("/Negocio/Obtener")
        .then(res => {
            $(".card-body").LoadingOverlay("hide");

            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {

            console.log("resjson",resJson)

            if (resJson.estado) {
                const d = resJson.objeto

                $("#txtNumeroDocumento").val(d.numeroDocumento)
                $("#txtRazonSocial").val(d.nombre)
                $("#txtCorreo").val(d.correo)
                $("#txtDireccion").val(d.direccion)
                $("#txTelefono").val(d.telefono)
                $("#txtImpuesto").val(d.porcentajeImpuesto)
                $("#txtSimboloMoneda").val(d.simboloMoneda)
                $("#imgLogo").attr("src", d.urlLogo)

            } else {
                swal("Ocurrió un error", resJson.mensaje, "error")

            }
        })
})

$("#btnGuardarCambios").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const msg = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", msg)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val()
    }

    const inputLogo = document.getElementById("txtLogo")

    const formData = new FormData()

    formData.append("logo", inputLogo.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $(".card-body").LoadingOverlay("show");

    fetch("/Negocio/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(res => {
            $(".card-body").LoadingOverlay("hide");

            return res.ok ? res.json() : Promise.reject(res)
        })

        .then(resJson => {

            if (resJson.estado) {
                const d = resJson.objeto

                $("#imgLogo").attr("src", d.urlLogo)

            } else {
                swal("Ocurrió un error", resJson.mensaje, "error")

            }
        })
})