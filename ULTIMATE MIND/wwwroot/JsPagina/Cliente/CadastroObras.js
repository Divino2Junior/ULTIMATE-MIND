var IDObra;
var marker;
var map;
// Variáveis globais para armazenar as coordenadas
var latitudeSelecionada;
var longitudeSelecionada;
$(document).ready(function () {


    // Chama a função para carregar a API do Google Maps quando o modal é aberto
    $('#modalCadastroObra').on('shown.bs.modal', function () {
        loadGoogleMaps();
    });

    $('#selectStatusObra').select2({
        dropdownParent: $('#modalCadastroObra')
    });

    $('#selectCliente').select2({
        dropdownParent: $('#modalCadastroObra'),
        width: 'resolve',
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

    Post("Cliente/BuscarObras", montarTela, Erro);
});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tableObra')) {
        $('#tableObra').DataTable().destroy();
    }

    var tableObra = $('#tableObra').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tableObra.clear();

        IDObra = retorno.idobra

        $.each(retorno, function (index, item) {
            tableObra.row.add([
                item.nomeCliente,
                item.status,
                item.endereco,
                '<button class="btn btn-outline-info" onclick="editarObra(' + item.idobra + ')">Editar</button>'
            ]);
        });

        tableObra.paging = false;
        tableObra.lengthChange = false;
        tableObra.searching = true;
        tableObra.ordering = false;
        tableObra.info = true;
        ocultarLoading();
    }
}

function editarObra(id) {

    Post("Cliente/BuscarInfoObra/" + id, montarModalObra);
}

function montarModalObra(retorno) {

    if (retorno != null && retorno != undefined) {

        IDObra = retorno.idobra;
        $('#txtNomeObra').val(retorno.nomeObra);
        $('#txtLatitudeObra').val(retorno.latitude);
        $('#txtLongitudeObra').val(retorno.longitude);
        $('#txtEnderecoObra').val(retorno.endereco);

        var option = new Option(retorno.nomeStatus, retorno.status, true, true);
        $('#selectStatusObra').append(option).trigger('change');

        var option = new Option(retorno.nomeCliente, retorno.idcliente, true, true);
        $('#selectCliente').append(option).trigger('change');
    }

    $("#modalCadastroObra").modal('show');
}

function NovaObra() {

    idUsuario = 0;
    limparModal();
    $("#modalCadastroObra").modal('show');
}

function salvarObra() {

    if (isEmptyOrNull($("#selectCliente").val())) {
        Alerta("Informe o Cliente");
        return;
    }
    if (isEmptyOrNull($("#selectStatusObra").val())) {
        Alerta("Informe o Status");
        return;
    }
    if (isEmptyOrNull($("#txtLongitudeObra").val())) {
        Alerta("Informe a Longitude");
        return;
    }
    if (isEmptyOrNull($("#txtLatitudeObra").val())) {
        Alerta("Informe a Latitude");
        return;
    }

    if (isEmptyOrNull($("#txtEnderecoObra").val())) {
        Alerta("Informe o Endereço");
        return;
    }

    var formData = new FormData();

    formData.append('Idobra', IDObra);
    formData.append('NomeObra', $('#txtNomeObra').val());
    formData.append('Endereco', $('#txtEnderecoObra').val());
    formData.append('Idcliente', $('#selectCliente').val());
    formData.append('Status', $('#selectStatusObra').val());
    formData.append('Latitude', $('#txtLatitudeObra').val());
    formData.append('Longitude', $('#txtLongitudeObra').val());

    // Faça a requisição AJAX para o servidor
    $.ajax({
        url: '/Cliente/SalvarObra',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#modalCadastroObra").modal('hide');
            limparModal();
            $.alert("Obra cadastrada com sucesso!");

            Post("Cliente/BuscarObras", montarTela, Erro);
        },
        error: function (xhr, status, error) {
            $.alert("Erro: " + error);
        }
    });

}

function limparModal() {
    $('#txtNomeObra').val("");
    $('#txtEnderecoObra').val("");
    $('#txtLatitudeObra').val("");
    $('#txtLongitudeObra').val("");
    $("#selectStatusObra").val(null).trigger('change');
    $("#selectCliente").val(null).trigger('change');
}