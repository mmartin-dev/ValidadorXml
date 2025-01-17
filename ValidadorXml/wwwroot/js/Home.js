﻿
function enviarXML() {
    var formData = new FormData();
    var totalFiles = document.getElementById('inputSubirArchivo').files.length;
    var esValido = true;
    if (totalFiles == 0) {
        esValido = false;
    }

    if (esValido == true) {
        var divTablaArchivos = document.getElementById('divTablaArchivos');

        for (var i = 0; i < totalFiles; i++) {
            formData.append("archivos", document.getElementById("inputSubirArchivo").files[i]);
        }

        $.ajax({
            type: "POST",
            url: "/Home/ValidarXMLCargar",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            beforeSend: function () {
                $("#tablaArchivos > tbody").html("");
                $("#tablaArchivos").append('<tr><td colspan="2" class="fila-loader">Validando Facturas...</td></tr>');
            },
            success: function (response) {
                var respuesta = response;
                console.log(respuesta);

                var codigoHtml = "";
                codigoHtml +=
                    `<table class="table table-bordered" id="tablaArchivos">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Nombre</th>
                                    <th>Serie</th>
                                    <th>Folio</th>
                                    <th>Emisor</th>
                                    <th>Receptor</th>
                                    <th>Total</th>
                                    <th>Código Estatus</th>
                                    <th>Estado</th>
                                </tr>
                            </thead>
                            <tbody>`;
                for (var i = 0; i < respuesta.length; i++) {
                    var file = document.getElementById('inputSubirArchivo').files[i];

                    codigoHtml += `<tr>
                            <td>${i + 1}</td>
                            <td>${file.name}</td>
                            <td>${(respuesta[i].serie == null || respuesta[i].serie == 'null') ? '' : respuesta[i].serie}</td>
                            <td>${respuesta[i].folio}</td>
                            <td>${respuesta[i].rfc_emisor}</td>
                            <td>${respuesta[i].rfc_receptor}</td>
                            <td>${respuesta[i].total}</td>
                            <td>${respuesta[i].codigo_estatus}</td>
                            <td id="valido-${i + 1}">${respuesta[i].estado}</td>
                        </tr>`;
                }
                codigoHtml += `</tbody></table>`;
                divTablaArchivos.innerHTML = codigoHtml;

                for (var i = 0; i < respuesta.length; i++) {
                    if (respuesta[i].estado.toString().toLowerCase().trim() == 'vigente') {
                        document.getElementById(`valido-${i + 1}`).classList.add("estatus-vigente");
                    } else if (respuesta[i].estado.toString().toLowerCase().trim() == 'cancelado') {
                        document.getElementById(`valido-${i + 1}`).classList.add("estatus-cancelado");
                    }
                }
                document.getElementById('btnComprobar').style.display = 'none';

            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
                alert("Ocurrió un error al verificar los CFDI(s): " + jqXHR);
            }
        });
    }
}
