var idCargoUsuario = 0;

$(document).ready(function () {

    Post("Administracao/BuscarCargoUsuarios", montarTela, Erro);
});


function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tableCargoUsuario')) {
        $('#tableCargoUsuario').DataTable().destroy();
    }

    var tableCargoUsuario = $('#tableCargoUsuario').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tableCargoUsuario.clear();

        $.each(retorno, function (index, item) {
            tableCargoUsuario.row.add([
                item.idcargo,
                item.nome,
                "<img class='imgAcao' onclick='editarCargoUsuario(" + item.idcargo + ")' src='../../images/editar.png'></img>"

            ]);
        });

        tableCargoUsuario.paging = false;
        tableCargoUsuario.lengthChange = false;
        tableCargoUsuario.searching = true;
        tableCargoUsuario.ordering = false;
        tableCargoUsuario.info = true;
        ocultarLoading();
    }

}

function editarCargoUsuario(id) {

    Post("Administracao/BuscarInfoCargoUsuario/" + id, montarModalCargoUsuario);
}

function novoCargoUsuario() {

    idCargoUsuario = 0;
    limparModal();
    $("#modalCargoUsuario").modal('show');
}

function salvarCargoUsuario() {

    if (isEmptyOrNull($("#nomeCargoUsuario").val())) {
        Alerta("Informe o Nome do Cargo");
        return;
    }

    var obj = {};
    obj.nomeCargoUsuario = $("#nomeCargoUsuario").val();
    obj.IDCargo = idCargoUsuario;
        
    PostDados("Administracao/SalvarCargoUsuario", { dados: JSON.stringify(obj) }, SalvarCargoUsuarioResult);
}

function limparModal() {

    $("#nomeCargoUsuario").val("");
}

function SalvarCargoUsuarioResult() {

    $("#modalCargoUsuario").modal('hide');
    limparModal();
    $.alert("Cargo de Usuário Cadastrado com Sucesso!");

    Post("Administracao/BuscarCargoUsuarios", montarTela, Erro);
}

function montarModalCargoUsuario(retorno) {

    $("#nomeCargoUsuario").val(retorno.nome);
    idCargoUsuario = retorno.idcargo;
    $("#modalCargoUsuario").modal('show');
}

