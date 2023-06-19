var idAtendimento;
var isDisabled = false;

$(document).ready(function () {

    Post("Cliente/BuscarAtendimentos", montarTela, Erro);

    $('#selectCliente').select2({
        ajax: {
            url: urlSite + 'Cliente/BuscarSelectCliente',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.idcliente,
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

    if (retorno != null && retorno != undefined) {

        tabelaAtendimentos.clear();

        $.each(retorno, function (index, item) {

            if (item.status === "Pendente") {
                idAtendimento = item.idatendimento;
                isDisabled = true;
            }

            tabelaAtendimentos.row.add([
                item.idcliente + " - " + item.nome,
                item.data,
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

    if (isDisabled) {
        $("#selectCliente").addClass("disabled");
        $("#btnIniciarAtendimento").addClass("disabled");
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

    if (retorno) {
        // Atualiza o QR Code no modal
        $('#m-imagem-preview').attr('src', retorno.urlFoto);

        // Exibe o modal
        $('#modalQRCode').modal('show');

        // Se a foto não existe, exibir o ícone padrão
        $.alert("Atendimento Iniciado com Sucesso");

        idAtendimento = retorno.idAtendimento;
    } else {
        // Se a foto não existe, exibir o ícone padrão
        $.alert("Atendimento Iniciado com Sucesso");
    }
    // Desabilitar o select e o botão de iniciar
    $("#selectCliente").addClass("disabled");
    $("#btnIniciarAtendimento").addClass("disabled");
    limparTela();

    Post("Cliente/BuscarAtendimentos", montarTela, Erro);

}


function finalizarAtendimento() {

    // Obtém a geolocalização do navegador
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // Extrai a latitude e longitude da posição atual
            var latitude = position.coords.latitude;
            var longitude = position.coords.longitude;

            var id = idAtendimento;
            if (isEmptyOrNull($("#txtObservacao").val())) {
                Alerta("É necessário inserir uma observação!");
                return;
            }

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