$(document).ready(function () {

    Post("DepartamentoPessoal/ConsultarValidacaoContraCheque", montarTela, Erro);

    $('#tabelaContracheques').on('click', '.btn-download', function () {
        var url = $(this).data('url');
        var link = document.createElement('a');
        link.href = url;
        link.download = url.substr(url.lastIndexOf('/') + 1);
        link.target = '_blank';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });
});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tabelaContracheques')) {
        $('#tabelaContracheques').DataTable().destroy();
    }

    var tabelaContracheques = $('#tabelaContracheques').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tabelaContracheques.clear();

        $.each(retorno, function (index, item) {

            var acoes = '';
            if (item.isAssinado === true) {
                // Caso o campo isValidado seja true
                acoes = '<button class="btn btn-primary btn-download" data-url="' + item.urlPdf + '">Visualizar</button>';
            } else {
                // Caso o campo isValidado seja false
                acoes = '<button class="btn btn-success" onclick="validarPdf(' + item.idContraCheque + ');">Validar</button>';
            }

            tabelaContracheques.row.add([
                item.matricula,
                item.nomeColaborador,
                item.mesReferencia,
                item.anoReferencia,
                acoes
            ]);
        });

        tabelaContracheques.paging = false;
        tabelaContracheques.lengthChange = false;
        tabelaContracheques.searching = true;
        tabelaContracheques.ordering = false;
        tabelaContracheques.info = true;
        ocultarLoading();
    }
}

function visualizarPdf(url) {

    window.location.href = url;
}

function validarPdf(id) {

}