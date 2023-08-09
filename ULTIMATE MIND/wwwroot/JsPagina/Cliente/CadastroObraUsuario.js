var IDRelacionamentoObraUsuario;
var imgUsuario;
$(document).ready(function () {

    $('#m-imagem-input-relacionamento-obra-usuario').on('change', function (e) {
        var file = e.target.files[0];
        var reader = new FileReader();
        imgUsuario = null;
        reader.onload = function () {
            imgUsuario = reader.result;
            $('#m-imagem-preview-relacionamento-obra-usuario').attr('src', reader.result);
        }

        reader.readAsDataURL(file);
    });

    $('#selectObra').select2({
        dropdownParent: $('#modalRelacionarObraUsuario'),
        width: 'resolve',
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

    $('#selectUsuario').select2({
        dropdownParent: $('#modalRelacionarObraUsuario'),
        width: 'resolve',
        ajax: {
            url: urlSite + 'DepartamentoPessoal/BuscarColaboradores',
            processResults: function (data) {
                var dados = [];
                $.each(data, function (index, item) {
                    var array = {
                        id: item.idusuario,
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

    Post("Cliente/BuscarRelacionamentoObraUsuario", montarTelaRelacionamentoObraUsuario, Erro);
});

function montarTelaRelacionamentoObraUsuario(retorno) {

    if ($.fn.dataTable.isDataTable('#tableRelacionamentoObraUsuario')) {
        $('#tableRelacionamentoObraUsuario').DataTable().destroy();
    }

    var tableRelacionamentoObraUsuario = $('#tableRelacionamentoObraUsuario').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {
        tableRelacionamentoObraUsuario.clear();

        $.each(retorno, function (index, item) {
            tableRelacionamentoObraUsuario.row.add([
                item.nomeObra,
                item.nomeUsuario,
                item.status,
                '<button class="btn btn-outline-info" onclick="editarRelacionamentoObraUsuario(' + item.idobraUsuario + ')">Editar</button>'
            ]);
        });

        tableRelacionamentoObraUsuario.paging = false;
        tableRelacionamentoObraUsuario.lengthChange = false;
        tableRelacionamentoObraUsuario.searching = true;
        tableRelacionamentoObraUsuario.ordering = false;
        tableRelacionamentoObraUsuario.info = true;
        ocultarLoading();
    }
}

function editarRelacionamentoObraUsuario(id) {
    IDRelacionamentoObraUsuario = id;
    Post("Cliente/BuscarInfoRelacionamentoObraUsuario/" + id, montarModalRelacionarObraUsuario);
}

function montarModalRelacionarObraUsuario(retorno) {

    var option = new Option(retorno.nomeObra, retorno.idObra, true, true);
    $('#selectObra').append(option).trigger('change');

    var optionUsuario = new Option(retorno.nomeUsuario, retorno.idUsuario, true, true);
    $('#selectUsuario').append(optionUsuario).trigger('change');

    if (retorno.imageUrl) {
        var fotoAtualizada = retorno.imageUrl + '?t=' + new Date().getTime();
        $('#m-imagem-preview-relacionamento-obra-usuario').attr('src', fotoAtualizada);
    } else {
        // Se a foto não existe, exibir o ícone padrão
        $('#m-imagem-preview-relacionamento-obra-usuario').attr('src', '/images/qr-code.png');
    }

    $("#modalRelacionarObraUsuario").modal('show');
}

function novoRelacionamentoObraUsuario() {
    IDRelacionamentoObraUsuario = 0;
    limparModalRelacionarObraUsuario();

    $("#modalRelacionarObraUsuario").modal('show');
}

function salvarRelacionamentoObraUsuario() {

    if (isEmptyOrNull($("#selectObra*").val())) {
        Alerta("Informe uma Obra");
        return;
    }
    if (isEmptyOrNull($("#selectUsuario").val())) {
        Alerta("Informe um Usuário");
        return;
    }

    var formData = new FormData();

    formData.append('IdRelacionamentoObraUsuario', IDRelacionamentoObraUsuario);
    formData.append('IdObra', $('#selectObra').val());
    formData.append('IdUsuario', $('#selectUsuario').val());

    // Verifique se uma imagem foi selecionada
    var imagemInput = document.getElementById('m-imagem-input-relacionamento-obra-usuario');
    if (imagemInput.files.length > 0) {
        formData.append('Foto', imagemInput.files[0]);
    }

    $.ajax({
        url: '/Cliente/SalvarRelacionamentoObraUsuario',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#modalRelacionarObraUsuario").modal('hide');
            limparModalRelacionarObraUsuario();
            $.alert("Relacionamento obra-usuário salvo com sucesso!");
            Post("Cliente/BuscarRelacionamentoObraUsuario", montarTelaRelacionamentoObraUsuario, Erro);
        },
        error: function (xhr, status, error) {
            $.alert("Erro: " + error);
        }
    });
}
function limparModalRelacionarObraUsuario() {
    $('#selectObra').val(null).trigger('change');
    $('#selectUsuario').val(null).trigger('change');
    $('#m-imagem-input-relacionamento-obra-usuario').val('');
    $('#m-imagem-preview-relacionamento-obra-usuario').attr('src', '/images/qr-code.png');
}
