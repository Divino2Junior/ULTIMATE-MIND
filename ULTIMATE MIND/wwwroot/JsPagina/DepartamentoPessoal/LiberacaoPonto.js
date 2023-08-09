$(document).ready(function () {

    $('#selectUsuario').select2({
        dropdownParent: $('#modalLiberacaoPonto'),
        width: 'resolve',
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

    Post("DepartamentoPessoal/ConsultarLiberacaoPonto", montarTela, Erro);
});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tabelaLiberacaoPonto')) {
        $('#tabelaLiberacaoPonto').DataTable().destroy();
    }

    var tabelaLiberacaoPonto = $('#tabelaLiberacaoPonto').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tabelaLiberacaoPonto.clear();
        $.each(retorno, function (index, item) {
            tabelaLiberacaoPonto.row.add([
                item.nomeColaborador,
                item.nomeGestor,
                item.data,
                item.hora,
                item.observacao,
                "<img class='imgAcao' onclick='excluirLiberacaoPonto(" + item.idhistoricoLiberacaoPonto + ")' src='../../images/delete-24.png'></img>"
            ]);
        });

        tabelaLiberacaoPonto.paging = false;
        tabelaLiberacaoPonto.lengthChange = false;
        tabelaLiberacaoPonto.searching = true;
        tabelaLiberacaoPonto.ordering = false;
        tabelaLiberacaoPonto.info = true;
    }

    ocultarLoading();
}

function excluirLiberacaoPonto(id) {
    $.confirm({
        title: "Atenção",
        content: "Deseja realmente excluir esta Liberação de Ponto?",
        type: 'warning',
        typeAnimated: true,
        buttons: {
            yes: {
                text: 'Sim',
                isHidden: false,
                action: function () {
                    Post("DepartamentoPessoal/ExcluirLiberacaoPonto/" + id, ExcluirLiberacaoPontoResult, Erro);
                }
            },
            no: {
                text: 'Não',
                isHidden: false,
                action: function () {

                }
            }
        }
    });
}

function ExcluirLiberacaoPontoResult() {
    $.alert("Liberação de Ponto excluido com sucesso!");
    Post("DepartamentoPessoal/ConsultarLiberacaoPonto", montarTela, Erro);
}

function AbrirModalNovaLiberacaoPonto() {
    $("#modalLiberacaoPonto").modal("show");
}
function AdicionarLiberacaoPonto() {

    if (isEmptyOrNull($("#selectUsuario").val())) {
        Alerta("É necessário selecionar o colaborador para continuar!");
        return;
    }

    if ($("#txtObservacao").val() == "") {
        Alerta("É necessário preencher o campo de observação!");
        return;
    }

    var usuario = $("#selectUsuario").val();
    var Observacao = $("#txtObservacao").val();

    var formData = new FormData();
    formData.append('usuario', usuario);
    formData.append('observacao', Observacao);

    // Faça a solicitação AJAX
    $.ajax({
        url: '/DepartamentoPessoal/SalvarLiberacaoPonto',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            // Limpar os campos do formulário
            $.alert("Liberação de Ponto Salvo com sucesso!");
            $("#selectUsuario").val(null).trigger('change');
            $('#txtObservacao').val('');
            $("#modalLiberacaoPonto").modal("hide");
            Post("DepartamentoPessoal/ConsultarLiberacaoPonto", montarTela, Erro);
        },
        error: function (xhr, status, error) {
            $.alert("Erro: " + error);
        }
    });
}