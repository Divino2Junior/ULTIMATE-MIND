var idCliente;

$(document).ready(function () {

    $('#txtCpf').mask('999.999.999-99');

    $("#txtTelefone").mask("(99) 99999-9999");

    $('#txtCnpj').mask('99.999.999/9999-99');


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
                item.nomeCliente,
                item.cpfouCnpj,
                item.statusNome,
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

        $('#txtNomeCliente').val(retorno.nomeCliente);
        $('#txtCpf').val(retorno.cpf);
        $('#txtCnpj').val(retorno.cpnj);
        $('#txtTelefone').val(retorno.telefone);
        $('#m-email').val(retorno.email);
        $('#txtLatitude').val(retorno.latitude);
        $('#txtLongitude').val(retorno.longitude);
        $('#txtEndereco').val(retorno.endereco);

        var option = new Option(retorno.nomeStatus, retorno.status, true, true);
        $('#selectStatus').append(option).trigger('change');
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
    if (isEmptyOrNull($("#txtCnpj").val())) {
        Alerta("Informe o CNPJ");
        return;
    }
    if (isEmptyOrNull($("#txtCpf").val())) {
        Alerta("Informe o CPF");
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

    var objet = {};

    objet.IdUsuario = idUsuario;

    objet.NomeCliente = $('#txtNomeCliente').val();
    objet.Cpf = $('#txtCpf').val();
    objet.Cpnj = $('#txtCnpj').val(j);
    objet.Telefone = $('#txtTelefone').val();
    objet.Email = $('#m-email').val();
    objet.Latitude = $('#txtLatitude').val();
    objet.Longitude = $('#txtLongitude').val();
    objet.Endereco = $('#txtEndereco').val();

    PostDados("Cliente/SalvarCliente", { dados: JSON.stringify(objet) }, SalvarUsuarioResult);
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
}

function SalvarUsuarioResult() {

    $("#modalCadastroCliente").modal('hide');
    limparModal();
    $.alert("Cliente Cadastrado com Sucesso!");

    Post("Cliente/BuscarClientes", montarTela, Erro);
}

