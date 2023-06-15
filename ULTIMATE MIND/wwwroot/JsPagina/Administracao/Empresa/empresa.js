var idEmpresa;

$(document).ready(function () {

    Post("Administracao/ConsultarEmpresas", montarTela, Erro);
});

function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tableEmpresa')) {
        $('#tableEmpresa').DataTable().destroy();
    }

    var tabelaEmpresa = $('#tableEmpresa').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tabelaEmpresa.clear();

        $.each(retorno, function (index, item) {
            tabelaEmpresa.row.add([
                item.idempresa,
                item.apelido,
                item.razaoSocial,
                item.nomeFantasia,
                item.statusNome
            ]);
        });

        tabelaEmpresa.paging = false;
        tabelaEmpresa.lengthChange = false;
        tabelaEmpresa.searching = true;
        tabelaEmpresa.ordering = false;
        tabelaEmpresa.info = true;
        ocultarLoading();
    }
}