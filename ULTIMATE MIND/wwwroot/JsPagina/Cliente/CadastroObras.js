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

    $('#m-imagem-input-obra').on('change', function (e) {
        var file = e.target.files[0];
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#m-imagem-preview-obra').attr('src', e.target.result);
        }

        reader.readAsDataURL(file);
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
            document.getElementById('txtLatitudeObra').value = latLng.lat();
            document.getElementById('txtLongitudeObra').value = latLng.lng();
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

        if (retorno.urlFoto) {
            var fotoAtualizada = retorno.urlFoto + '?t=' + new Date().getTime();
            $('#m-imagem-preview-obra').attr('src', fotoAtualizada);
        } else {
            // Se a foto não existe, exibir o ícone padrão
            $('#m-imagem-preview-obra').attr('src', '/images/qr-code.png');
        }
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

    // Verifique se uma imagem foi selecionada
    var imagemInput = document.getElementById('m-imagem-input-obra');
    if (imagemInput.files.length > 0) {
        formData.append('ImagemObra', imagemInput.files[0]);
    }

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
    $('#m-imagem-preview-obra').attr('src', '/images/qr-code.png');
}

//function consultarEnderecoObra() {
//    var endereco = document.getElementById('txtEnderecoObra').value;
//    var geocoder = new google.maps.Geocoder();
//    geocoder.geocode({ address: endereco }, function (results, status) {
//        if (status === google.maps.GeocoderStatus.OK) {
//            if (results.length > 0) {
//                var location = results[0].geometry.location;
//                map.setCenter(location);
//                map.setZoom(15);
//                var marker = new google.maps.Marker({
//                    position: location,
//                    map: map
//                });
//                var confirmar = confirm("Deseja salvar a localização encontrada?");
//                if (confirmar) {
//                    document.getElementById('txtLatitudeO').value = location.lat();
//                    document.getElementById('txtLongitude').value = location.lng();
//                }
//            } else {
//                alert("Endereço não encontrado.");
//            }
//        } else {
//            alert("Ocorreu um erro ao consultar o endereço.");
//        }
//    });

//}