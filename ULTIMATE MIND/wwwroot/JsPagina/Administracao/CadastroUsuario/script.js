var idUsuario = 0;

var imgUsuario;

$(document).ready(function () {

    $('#m-cpf').mask('999.999.999-99');

    $("#m-telefone").mask("(99) 99999-9999");

    $('#m-imagem-input').on('change', function (e) {
        var file = e.target.files[0];
        var reader = new FileReader();
        imgUsuario = null;
        reader.onload = function () {
            imgUsuario = reader.result;
            $('#m-imagem-preview').attr('src', reader.result);
        }

        reader.readAsDataURL(file);
    });

    $('#selectFuncao').select2({
        dropdownParent: $('#modalCadastroCliente'),
        width: 'resolve',
        ajax: {
            url: urlSite + 'Administracao/ObterCargos',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.idcargo,
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

    $('#selectStatus').select2({
        dropdownParent: $('#modalCadastroCliente'),
        width: 'resolve',
        ajax: {
            url: urlSite + 'Administracao/ObterStatus',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.id,
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

    $('#selectGrupoPermissao').select2({
        dropdownParent: $('#modalCadastroCliente'),
        width: 'resolve',
        ajax: {
            url: urlSite + 'Administracao/ObterGrupoPermissao',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.idgrupoPermissao,
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

    Post("Administracao/BuscarUsuarios", montarTela, Erro);

});


function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#table-colab')) {
        $('#table-colab').DataTable().destroy();
    }

    var tabelaUsuarios = $('#table-colab').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tabelaUsuarios.clear();

        $.each(retorno, function (index, item) {
            tabelaUsuarios.row.add([
                item.matricula,
                item.nome,
                item.cpf,
                item.cargo,
                '<button class="btn btn-outline-info" onclick="editarUsuario(' + item.idusuario + ')">Editar</button>'
            ]);
        });

        tabelaUsuarios.paging = false;
        tabelaUsuarios.lengthChange = false;
        tabelaUsuarios.searching = true;
        tabelaUsuarios.ordering = false;
        tabelaUsuarios.info = true;
        ocultarLoading();
    }
}

function editarUsuario(id) {

    Post("Administracao/BuscarInfoUsuario/" + id, montarModalUsuario);
}

function montarModalUsuario(retorno) {

    if (retorno != null && retorno != undefined) {

        idUsuario = retorno.idUsuario;

        $('#m-matricula').val(retorno.matricula);
        $('#m-nome').val(retorno.nome);
        $('#m-cpf').val(retorno.cpf);
        $('#m-telefone').val(retorno.telefone);
        $('#m-email').val(retorno.email);
        $('#m-rg').val(retorno.rg);

        $('#m-dataNascimento').val(retorno.dataNascimento);
        $('#m-dataAdmissao').val(retorno.dataAdmissao);
        $('#m-dataDemissao').val(retorno.dataDemissao);
        $('#m-entrada').val(retorno.horaEntrada);
        $('#m-saida').val(retorno.horaSaida);
        $('#m-saidaAlmoco').val(retorno.horaInicioAlmoco);
        $('#m-entradaAlmoco').val(retorno.horaFimAlmoco);

        var option = new Option(retorno.nomeStatus, retorno.status, true, true);
        $('#selectStatus').append(option).trigger('change');

        var optionFuncao = new Option(retorno.nomeCargo, retorno.idCargo, true, true);
        $('#selectFuncao').append(optionFuncao).trigger('change');

        var optionGrupoPermissao = new Option(retorno.nomeGrupoPermissao, retorno.idGrupoPermissao, true, true);
        $("#selectGrupoPermissao").append(optionGrupoPermissao).trigger('change');

        if (retorno.imgUsuario) {
            var fotoAtualizada = retorno.imgUsuario + '?t=' + new Date().getTime();
            $('#m-imagem-preview').attr('src', fotoAtualizada);
        } else {
            // Se a foto não existe, exibir o ícone padrão
            $('#m-imagem-preview').attr('src', '/icons/homem-usuario.png');
        }
    }

    $("#modalCadastroCliente").modal('show');
}

function novoUsuario() {

    idUsuario = 0;
    limparModal();
    $('#m-imagem-preview').attr('src', '/icons/homem-usuario.png');
    $("#modalCadastroCliente").modal('show');
}

function salvarUsuario() {

    if (isEmptyOrNull($("#m-nome").val())) {
        Alerta("Informe o nome do Colaborador");
        return;
    }
    if (isEmptyOrNull($("#m-cpf").val())) {
        Alerta("Informe o Cpf");
        return;
    }
    if (isEmptyOrNull($("#selectStatus").val())) {
        Alerta("Informe o Status");
        return;
    }
    if (isEmptyOrNull($("#selectFuncao").val())) {
        Alerta("Informe a Função");
        return;
    }
    if (isEmptyOrNull($("#selectGrupoPermissao").val())) {
        Alerta("Informe o Grupo de Usuário");
        return;
    }

    var formData = new FormData();

    formData.append('IdUsuario', idUsuario);
    formData.append('Nome', $('#m-nome').val());
    formData.append('Cpf', $('#m-cpf').val());
    formData.append('Telefone', $('#m-telefone').val());
    formData.append('Email', $('#m-email').val());
    formData.append('Rg', $('#m-rg').val());
    formData.append('Status', $('#selectStatus').val());
    formData.append('IdCargo', $('#selectFuncao').val());
    formData.append('DataNascimento', $('#m-dataNascimento').val());
    formData.append('DataAdmissao', $('#m-dataAdmissao').val());
    formData.append('DataDemissao', $('#m-dataDemissao').val());
    formData.append('IdGrupoPermissao', $('#selectGrupoPermissao').val());
    formData.append('HoraEntrada', $('#m-entrada').val());
    formData.append('HoraSaida', $('#m-saida').val());
    formData.append('HoraInicioAlmoco', $('#m-saidaAlmoco').val());
    formData.append('HoraFimAlmoco', $('#m-entradaAlmoco').val());

    // Verifique se uma imagem foi selecionada
    var imagemInput = document.getElementById('m-imagem-input');
    if (imagemInput.files.length > 0) {
        formData.append('Imagem', imagemInput.files[0]);
    }

    // Faça a requisição AJAX para o servidor
    $.ajax({
        url: '/Administracao/SalvarUsuario',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#modalCadastroCliente").modal('hide');
            limparModal();
            $.alert("Usuário Cadastrado com Sucesso!");

            Post("Administracao/BuscarUsuarios", montarTela, Erro);
        },
        error: function (xhr, status, error) {
            $.alert("Erro: " + error);
        }
    });
}

function limparModal() {
    $('#m-matricula').val("");
    $('#m-nome').val("");
    $('#m-cpf').val("");
    $('#m-telefone').val("");
    $('#m-email').val("");
    $('#m-rg').val("");
    $("#selectStatus").val(null).trigger('change');
    $("#selectFuncao").val(null).trigger('change');
    $("#selectGrupoPermissao").val(null).trigger('change');
    $('#m-dataNascimento').val("");
    $('#m-dataAdmissao').val("");
    $('#m-dataDemissao').val("");
    $('#m-entrada').val("");
    $('#m-saida').val("");
    $('#m-saidaAlmoco').val("");
    $('#m-entradaAlmoco').val("");
}

