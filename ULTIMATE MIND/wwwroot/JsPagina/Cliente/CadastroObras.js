var IDObra;
var marker;
var map;
// Variáveis globais para armazenar as coordenadas
var latitudeSelecionada;
var longitudeSelecionada;
$(document).ready(function () {


    // Chama a função para carregar a API do Google Maps quando o modal é aberto
    $('#modalCadastroCliente').on('shown.bs.modal', function () {
        loadGoogleMaps();
    });

    $('#m-imagem-input').on('change', function (e) {
        var file = e.target.files[0];
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#m-imagem-preview').attr('src', e.target.result);
        }

        reader.readAsDataURL(file);
    });

    $('#selectStatus').select2({
        dropdownParent: $('#modalCadastroCliente')
    });

    Post("Cliente/BuscarObras", montarTela, Erro);
});

// Função para inicializar o mapa
function initMap() {
    // Coordenadas padrão (pode ser ajustado para uma localização inicial específica)
    var defaultLatLng = { lat: -23.550520, lng: -46.633308 };

    // Cria um mapa centrado nas coordenadas padrão
    var map = new google.maps.Map(document.getElementById("map"), {
        center: defaultLatLng,
        zoom: 12
    });

    // Adiciona um marcador arrastável
    var marker = new google.maps.Marker({
        position: defaultLatLng,
        map: map,
        draggable: true
    });

    map.addListener('click', function (event) {
        var latLng = event.latLng;
        var confirmar = confirm("Deseja salvar a localização selecionada?");
        if (confirmar) {
            document.getElementById('txtLatitude').value = latLng.lat();
            document.getElementById('txtLongitude').value = latLng.lng();
        }
    });
}

// Função para carregar a API do Google Maps
function loadGoogleMaps() {
    var script = document.createElement("script");
    script.src = "https://maps.googleapis.com/maps/api/js?key=AIzaSyC85r-6Rnm-5MmB_BJ_KbayCbLRBEEylbg&callback=initMap";
    script.defer = true;
    script.async = true;
    document.head.appendChild(script);
}


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

        $('#txtLatitude').val(retorno.latitude);
        $('#txtLongitude').val(retorno.longitude);
        $('#txtEndereco').val(retorno.endereco);

        var option = new Option(retorno.nomeStatus, retorno.status, true, true);
        $('#selectStatus').append(option).trigger('change');

        var option = new Option(retorno.nomeCliente, retorno.idcliente, true, true);
        $('#selectCliente').append(option).trigger('change');

        if (retorno.urlFoto) {
            var fotoAtualizada = retorno.urlFoto + '?t=' + new Date().getTime();
            $('#m-imagem-preview').attr('src', fotoAtualizada);
        } else {
            // Se a foto não existe, exibir o ícone padrão
            $('#m-imagem-preview').attr('src', '/icons/homem-usuario.png');
        }
    }

    $("#modalCadastroObra").modal('show');
}

function novoCliente() {

    idUsuario = 0;
    limparModal();
    $("#modalCadastroObra").modal('show');
}

function salvarCliente() {

    if (isEmptyOrNull($("#selectCliente").val())) {
        Alerta("Informe o Cliente");
        return;
    }
    if (isEmptyOrNull($("#selectStatus").val())) {
        Alerta("Informe o Status");
        return;
    }
    if (isEmptyOrNull($("#txtLongitude").val())) {
        Alerta("Informe a Longitude");
        return;
    }
    if (isEmptyOrNull($("#txtLatitude").val())) {
        Alerta("Informe a Latitude");
        return;
    }

    if (isEmptyOrNull($("#txtEndereco").val())) {
        Alerta("Informe o Endereço");
        return;
    }

    var formData = new FormData();

    formData.append('idcliente', idCliente);
    formData.append('NomeObra', $('#txtNomeObra').val());
    formData.append('Endereco', $('#txtEndereco').val());
    formData.append('Status', $('#selectStatus').val());
    formData.append('Latitude', $('#txtLatitudeO').val());
    formData.append('Longitude', $('#txtLongitude').val());

    // Verifique se uma imagem foi selecionada
    var imagemInput = document.getElementById('m-imagem-input-obra');
    if (imagemInput.files.length > 0) {
        formData.append('ImagemObra', imagemInput.files[0]);
    }

    // Faça a requisição AJAX para o servidor
    $.ajax({
        url: '/Obra/SalvarObra',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#modalCadastroObra").modal('hide');
            limparModal();
            $.alert("Obra cadastrada com sucesso!");

            // Chame a função para atualizar a tabela de obras
            montarTela();
        },
        error: function (xhr, status, error) {
            $.alert("Erro: " + error);
        }
    });

}

function limparModal() {
    $('#txtNomeCliente').val("");
    $('#txtCpf').val("");
    $('#txtCnpj').val("");
    $('#txtTelefone').val("");
    $('#m-email').val("");
    $('#txtLatitude').val("");
    $('#txtLongitude').val("");
    $('#txtEndereco').val("");
    $("#selectStatus").val(null).trigger('change');
    $('#m-imagem-preview').attr('src', '/icons/homem-usuario.png');
}

function consultarEndereco() {
    var endereco = document.getElementById('txtEndereco').value;
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ address: endereco }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            if (results.length > 0) {
                var location = results[0].geometry.location;
                map.setCenter(location);
                map.setZoom(15);
                var marker = new google.maps.Marker({
                    position: location,
                    map: map
                });
                var confirmar = confirm("Deseja salvar a localização encontrada?");
                if (confirmar) {
                    document.getElementById('txtLatitude').value = location.lat();
                    document.getElementById('txtLongitude').value = location.lng();
                }
            } else {
                alert("Endereço não encontrado.");
            }
        } else {
            alert("Ocorreu um erro ao consultar o endereço.");
        }
    });

}