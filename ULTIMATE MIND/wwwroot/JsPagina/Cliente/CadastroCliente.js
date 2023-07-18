var idCliente;
var marker;
var map;
// Variáveis globais para armazenar as coordenadas
var latitudeSelecionada;
var longitudeSelecionada;
$(document).ready(function () {

    $('#txtCpf').mask('999.999.999-99');

    $("#txtTelefone").mask("(99) 99999-9999");

    $('#txtCnpj').mask('99.999.999/9999-99');

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

    $('#switch-input').on('change', function () {
        var isPdf = $(this).prop('checked');

        if (isPdf) {
            $('#m-imagem-input').attr('accept', 'application/pdf');
        } else {
            $('#m-imagem-input').attr('accept', 'image/*');
        }
    });



    $('#selectStatus').select2({
        dropdownParent: $('#modalCadastroCliente')
    });

    const emailInput = $('#m-email');

    // Aplica a máscara do email
    emailInput.mask('A', {
        translation: {
            A: { pattern: /[\w@\-.+]/, recursive: true }
        }
    });

    // Validação de email ao perder o foco
    emailInput.on('blur', function () {
        const email = $(this).val();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            alert('Email inválido');
            $(this).val('');
        }
    });

    Post("Cliente/BuscarClientes", montarTela, Erro);

});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tableCliente')) {
        $('#tableCliente').DataTable().destroy();
    }

    var tableCliente = $('#tableCliente').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tableCliente.clear();

        $.each(retorno, function (index, item) {
            tableCliente.row.add([
                item.idcliente,
                item.apelido,
                item.cpfouCnpj,
                item.status,
                item.telefone,
                '<button class="btn btn-outline-info" onclick="editarUsuario(' + item.idcliente + ')">Editar</button>'
            ]);
        });

        tableCliente.paging = false;
        tableCliente.lengthChange = false;
        tableCliente.searching = true;
        tableCliente.ordering = false;
        tableCliente.info = true;
        ocultarLoading();
    }
}

function editarUsuario(id) {

    Post("Cliente/BuscarInfoCliente/" + id, montarModalCliente);
}

function montarModalCliente(retorno) {

    if (retorno != null && retorno != undefined) {

        idCliente = retorno.idcliente;

        $('#txtNomeCliente').val(retorno.nome);
        $('#txtCpf').val(retorno.cpf);
        $('#txtCnpj').val(retorno.cpnj);
        $('#txtTelefone').val(retorno.telefone);
        $('#m-email').val(retorno.email);
        $('#txtLatitude').val(retorno.latitude);
        $('#txtLongitude').val(retorno.longitude);
        $('#txtEndereco').val(retorno.endereco);

        var option = new Option(retorno.nomeStatus, retorno.status, true, true);
        $('#selectStatus').append(option).trigger('change');

        if (retorno.urlFoto) {
            var fotoAtualizada = retorno.urlFoto + '?t=' + new Date().getTime();
            $('#m-imagem-preview').attr('src', fotoAtualizada);
        } else {
            // Se a foto não existe, exibir o ícone padrão
            $('#m-imagem-preview').attr('src', '/icons/homem-usuario.png');
        }
    }

    $("#modalCadastroCliente").modal('show');
}

function novoCliente() {

    idUsuario = 0;
    limparModal();
    $("#modalCadastroCliente").modal('show');
}

function salvarCliente() {

    if (isEmptyOrNull($("#txtNomeCliente").val())) {
        Alerta("Informe o nome do Cliente");
        return;
    }
    if (isEmptyOrNull($("#selectStatus").val())) {
        Alerta("Informe o Status");
        return;
    }
    if (isEmptyOrNull($("#txtEndereco").val())) {
        Alerta("Informe o Endereço");
        return;
    }

    var formData = new FormData();

    formData.append('idcliente', idCliente);
    formData.append('Nome', $('#txtNomeCliente').val());
    formData.append('Cpf', $('#txtCpf').val());
    formData.append('Cpnj', $('#txtCnpj').val());
    formData.append('Telefone', $('#txtTelefone').val());
    formData.append('Email', $('#m-email').val());
    formData.append('Latitude', $('#txtLatitude').val());
    formData.append('Longitude', $('#txtLongitude').val());
    formData.append('Endereco', $('#txtEndereco').val());
    formData.append('Status', $('#selectStatus').val());

    // Verifique se uma imagem foi selecionada
    var imagemInput = document.getElementById('m-imagem-input');
    if (imagemInput.files.length > 0) {
        formData.append('Imagem', imagemInput.files[0]);
    }

    // Faça a requisição AJAX para o servidor
    $.ajax({
        url: '/Cliente/SalvarCliente',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#modalCadastroCliente").modal('hide');
            limparModal();
            $.alert("Cliente Cadastrado com Sucesso!");

            Post("Cliente/BuscarClientes", montarTela, Erro);
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