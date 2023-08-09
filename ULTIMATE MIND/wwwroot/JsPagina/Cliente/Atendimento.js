var idAtendimento;
var isDisabled = false;
var idCliente;

let selectedClientId, latitude, longitude;

$(document).ready(function () {

    Post("Cliente/BuscarAtendimentos", montarTela, Erro);

    $('#selectCliente').select2({
        ajax: {
            url: urlSite + 'Cliente/BuscarSelectObra',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.idobra,
                        text: item.nome
                    }
                    dados.push(array);
                });

                return {
                    results: dados
                };
            }
        }
    });
});


function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tabelaAtendimentos')) {
        $('#tabelaAtendimentos').DataTable().destroy();
    }

    var tabelaAtendimentos = $('#tabelaAtendimentos').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno.atendimentos != null && retorno.atendimentos != undefined) {

        tabelaAtendimentos.clear();
        $.each(retorno.atendimentos, function (index, item) {
            tabelaAtendimentos.row.add([
                item.nome,
                item.data,
                item.inicioAtendimento,
                item.fimAtendimento,
                item.status
            ]);
        });

        tabelaAtendimentos.paging = false;
        tabelaAtendimentos.lengthChange = false;
        tabelaAtendimentos.searching = true;
        tabelaAtendimentos.ordering = false;
        tabelaAtendimentos.info = true;
        ocultarLoading();
    }

    if (retorno.atendimento != null && retorno.atendimento != undefined) {

        tag = "";
        tag += "Data: " + retorno.atendimento.data;

        $("#divDataDia").html(tag);
        $("#btnIniciarAtendimento").addClass("d-none");
        $("#inputInicioAtendimento").removeClass("d-none");
        $("#inputInicioAtendimento").val(retorno.atendimento.inicioAtendimento);

        var option = new Option(retorno.atendimento.nome, retorno.atendimento.idObra, true, true);
        $('#selectCliente').append(option).trigger('change');
        $("#selectCliente").prop("disabled", false);
    }
    else {
        $("#selectCliente").removeClass("disabled");
        $("#btnIniciarAtendimento").removeClass("disabled");
    }
}

function BuscarQrCode() {
    if (isEmptyOrNull($("#selectCliente").val())) {
        Alerta("É necessário selecionar um cliente para continuar!");
        return;
    }
    // Obtém a geolocalização do navegador
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // Extrai a latitude e longitude da posição atual
            var latitude = position.coords.latitude;
            var longitude = position.coords.longitude;

            var id = $("#selectCliente").val();

            // Passa a latitude e longitude como parâmetros para o servidor
            Post("Cliente/BuscarQrCodeCliente/" + id + "?latitude=" + latitude + "&longitude=" + longitude, retornoInicioAtendimento);
        });
    } else {
        // Navegador não suporta geolocalização
        Alerta("Seu navegador não suporta geolocalização.");
    }

}

function retornoInicioAtendimento(retorno) {

    if (retorno.urlFoto != null && retorno.urlFoto != "" && retorno.urlFoto != undefined) {
        // Atualiza o QR Code no modal
        $('#m-imagem-preview').attr('src', retorno.urlFoto);

        // Desabilita o Select2
        $("#selectCliente").prop("disabled", true);
        $("#btnIniciarAtendimento").addClass("disabled");
        $('#modalQRCode').modal('show');
        idAtendimento = retorno.idAtendimento;
        Post("Cliente/BuscarAtendimentos", montarTela, Erro);
    } else {
        // Chama a função para capturar a foto
        capturarFoto();
    }
}


function finalizarAtendimento() {

    var id = idAtendimento;
    if (isEmptyOrNull($("#txtObservacao").val())) {
        Alerta("É necessário inserir uma observação!");
        return;
    }

    // Obtém a geolocalização do navegador
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // Extrai a latitude e longitude da posição atual
            var latitude = position.coords.latitude;
            var longitude = position.coords.longitude;
            var observacao = $("#txtObservacao").val();
            var observacaoCodificada = encodeURIComponent(observacao);

            // Passa a latitude e longitude como parâmetros para o servidor
            Post("Cliente/FinalizarAtendimento/" + id + "?latitude=" + latitude + "&longitude=" + longitude + "&observacao=" + observacaoCodificada, FinalizarAtendimentoResult);
        });
    } else {
        // Navegador não suporta geolocalização
        Alerta("Seu navegador não suporta geolocalização.");
    }
}

function limparTela() {

    $("#selectCliente").val(null).trigger('change');
    $("#txtObservacao").val("");
}

function FinalizarAtendimentoResult() {

    isDisabled = false;
    limparTela();
    $("#selectCliente").removeClass("disabled");
    $("#btnIniciarAtendimento").removeClass("disabled");
    $.alert("Atendimento Finalizado com Sucesso");
    Post("Cliente/BuscarAtendimentos", montarTela, Erro);
}


// Função para capturar a foto da câmera
function capturarFoto() {
    if (isEmptyOrNull($("#selectCliente").val())) {
        Alerta("É necessário selecionar um cliente para continuar!");
        return;
    }

    selectedClientId = $("#selectCliente").val();

    // Obtém a geolocalização do navegador
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // Extrai a latitude e longitude da posição atual
            latitude = position.coords.latitude;
            longitude = position.coords.longitude;

            // Acessar a câmera e abrir a modal com o vídeo da câmera
            const videoElement = document.getElementById("video");
            const constraints = { video: true };
            navigator.mediaDevices.getUserMedia(constraints)
                .then(function (stream) {
                    videoElement.srcObject = stream;
                })
                .catch(function (error) {
                    console.error("Erro ao acessar a câmera:", error);
                    alert("Erro ao acessar a câmera. Verifique as permissões e tente novamente.");
                });

            // Exibe a modal com o vídeo da câmera
            $("#modalCapturarFoto").modal("show");
        });
    } else {
        // Navegador não suporta geolocalização
        console.error("Seu navegador não suporta geolocalização.");
        alert("Seu navegador não suporta geolocalização.");
    }
}

// Função para tirar a foto da câmera e exibir na tela
function tirarFoto() {
    const videoElement = document.getElementById("video");
    const canvasElement = document.getElementById("canvas");
    const context = canvasElement.getContext("2d");

    // Ajustar o tamanho do canvas para a resolução do vídeo
    canvasElement.width = videoElement.videoWidth;
    canvasElement.height = videoElement.videoHeight;

    // Capturar a foto e exibi-la
    context.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height);
    videoElement.style.display = "none";
    canvasElement.style.display = "block";
}

// Função para enviar a foto e os dados para o servidor
function enviarFoto() {
    const canvasElement = document.getElementById("canvas");
    const dataUrl = canvasElement.toDataURL("image/jpeg");

    // Enviar a foto, ID do cliente, latitude e longitude para o servidor
    Post("/Cliente/EnviarFotoCliente", {
        foto: dataUrl,
        clienteId: selectedClientId,
        latitude: latitude,
        longitude: longitude,
    }, retornoInicioAtendimento);

    // Esconde a modal da câmera
    $("#modalCapturarFoto").modal("hide");
}