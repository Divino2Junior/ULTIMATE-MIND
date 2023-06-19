var urlSite;

$(document).ready(function () {

    Post('DepartamentoPessoal/BuscarContraCheque', montarTela, Erro);

    $('#selectUsuario').select2({
        ajax: {
            url: urlSite + 'DepartamentoPessoal/BuscarColaboradores',
            processResults: function (data) {
                bDevolucao = data;
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

    $('#arquivoPdf').change(function (e) {
        var files = $('#arquivoPdf').prop("files");

        var valido = true;
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toUpperCase().indexOf(".PDF") < 0)
                valido = false;
        }

        if (!valido) {
            Erro("Selecione somente arquivos com extenção .PDF");
        }
    });

    // Intercepte o evento de envio do formulário
    $('#novoContraChequeForm').on('submit', function (e) {
        e.preventDefault(); // Impedir o envio padrão do formulário

        if (isEmptyOrNull($("#selectUsuario").val())) {
            Alerta("É necessário selecionar o colaborador para continuar!");
            return;
        }

        if ($("#mesReferencia").val() == "") {
            Alerta("É necessário informar a data de referência!");
            return;
        }

        // Obtenha os valores dos campos do formulário
        var usuario = $('#selectUsuario').val();
        var mesReferencia = $('#mesReferencia').val();
        var arquivoPdf = $('#arquivoPdf')[0].files[0];

        if (arquivoPdf === undefined) {
            Alerta("É necessário selecionar um arquivo!");
            return;
        }

        // Crie um objeto FormData para enviar os dados do formulário
        var formData = new FormData();
        formData.append('usuario', usuario);
        formData.append('mesReferencia', mesReferencia);
        formData.append('arquivoPdf', arquivoPdf);

        // Faça a solicitação AJAX
        $.ajax({
            url: '/DepartamentoPessoal/InserirContraCheque',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                // Limpar os campos do formulário
                $.alert("Contra Cheque Importado com Sucesso!");
                $("#selectUsuario").val(null).trigger('change');
                $('#mesReferencia').val('');
                $('#arquivoPdf').val('');
                Post('DepartamentoPessoal/BuscarContraCheque', montarTela, Erro);
            },
            error: function (xhr, status, error) {
                $.alert("Erro: " + error);
            }
        });
    });
});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#contraChequesTable')) {
        $('#contraChequesTable').DataTable().destroy();
    }

    var contraChequesTable = $('#contraChequesTable').DataTable(
        {
            "autoWidth": false,
            "ordering": false,
            "lengthChange": false,
            "pageLength": 50,
        });

    contraChequesTable.clear();
    mostrarLoading();

    if (retorno != null && retorno != undefined) {


        $.each(retorno, function (index, item) {
            contraChequesTable.row.add([
                item.nomeColaborador,
                item.referencia,
                "<img class='imgAcao' onclick='excluirContraChequeUsuario(" + item.idContraCheque + ")' src='../../images/delete-24.png'></img>"
            ]);
        });

        contraChequesTable.paging = false;
        contraChequesTable.lengthChange = false;
        contraChequesTable.searching = true;
        contraChequesTable.ordering = false;
        contraChequesTable.info = true;
        ocultarLoading();
    }
}

function excluirContraChequeUsuario(id) {

    Post("DepartamentoPessoal/ExcluirContraChequeUsuario/" + id, excluirContraChequeUsuarioResult);
}

function excluirContraChequeUsuarioResult() {

    $.alert("Contra Cheque Excluido com Sucesso!");
    Post('DepartamentoPessoal/BuscarContraCheque', montarTela, Erro);
}
