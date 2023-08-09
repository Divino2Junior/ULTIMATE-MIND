var idGrupoUsuario = 0;

$(document).ready(function () {

    Post("Administracao/BuscarGrupoUsuarios", montarTela, Erro);
    Post("Administracao/ObterMenuXml/", construirArvoreMenu);

});


function montarTela(retorno) {

    if ($.fn.dataTable.isDataTable('#tableGrupoUsuario')) {
        $('#tableGrupoUsuario').DataTable().destroy();
    }

    var tableGrupoUsuario = $('#tableGrupoUsuario').DataTable({
        "responsive": true,
        "scrollCollapse": true,
        "autoWidth": false,
        "ordering": false,
    });

    mostrarLoading();

    if (retorno != null && retorno != undefined) {

        tableGrupoUsuario.clear();

        $.each(retorno, function (index, item) {
            tableGrupoUsuario.row.add([
                item.idgrupoPermissao,
                item.nome,
                "<img class='imgAcao' onclick='editarGrupoUsuario(" + item.idgrupoPermissao + ")' src='../../images/editar.png'></img>"

            ]);
        });

        tableGrupoUsuario.paging = false;
        tableGrupoUsuario.lengthChange = false;
        tableGrupoUsuario.searching = true;
        tableGrupoUsuario.ordering = false;
        tableGrupoUsuario.info = true;
        ocultarLoading();
    }

}

function editarGrupoUsuario(id) {

    Post("Administracao/BuscarInfoGrupoUsuario/" + id, montarModalGrupoUsuario);
}


function construirArvoreMenu(menuCompleto) {
    var htmlMenu = '<ul>';

    for (var i = 0; i < menuCompleto.length; i++) {
        var item = menuCompleto[i];

        htmlMenu += '<li id="' + item.telaId + '">'; // Adicione o ID do item aqui
        htmlMenu += '<a>' + item.tela + '</a>';

        if (item.submenu && item.submenu.length > 0) {
            htmlMenu += '<ul>';
            for (var j = 0; j < item.submenu.length; j++) {
                var subitem = item.submenu[j];
                htmlMenu += '<li id="' + subitem.telaId + '"><a>' + subitem.tela + '</a></li>'; // Adicione o ID do subitem aqui
            }
            htmlMenu += '</ul>';
        }

        htmlMenu += '</li>';
    }

    htmlMenu += '</ul>';

    // Inserir o HTML montado no elemento com ID "tree"
    document.getElementById('tree').innerHTML = htmlMenu;
}


function montarModalGrupoUsuario(retorno) {

    limparModal();
    if ((retorno != null && retorno != undefined) || retorno.lstNomeTela.length > 0) {
        idGrupoUsuario = retorno.idgrupoPermissao;

        $("#nomeGrupoUsuario").val(retorno.nomeGrupoUsuario);

        $('#tree').jstree({
            'checkbox': {
                'keep_selected_style': true
            },
            'plugins': ['checkbox']
        });

        marcarCheckboxes(retorno.lstNomeTela);
    }
    else {

        $('#tree').jstree({
            'checkbox': {
                'keep_selected_style': true
            },
            'plugins': ['checkbox']
        });
    }

    $("#modalGrupoUsuario").modal('show');
}


function marcarCheckboxes(listaValores) {

    if (listaValores != null) {
        $('#tree').jstree(true).get_json('#', { flat: true }).forEach(function (node) {
            var telaEncontrada = listaValores.find(function (tela) {
                return tela === node.id; // Ajuste a propriedade para a comparação correta
            });

            if (telaEncontrada) {
                $('#tree').jstree('check_node', node.id);
            }
        });
    }    
}

function novoGrupoUsuario() {

    idGrupoUsuario = 0;
    limparModal();
    $("#modalGrupoUsuario").modal('show');

    $('#tree').jstree({
        'checkbox': {
            'keep_selected_style': true
        },
        'plugins': ['checkbox']
    });

}

function salvarPermissaoUsuario() {

    var selectedNodes = $('#tree').jstree('get_selected', true);

    // Cria um array para armazenar os IDs das telas selecionadas
    var telaIds = [];

    // Percorre os nós selecionados e obtém seus IDs
    selectedNodes.forEach(function (node) {
        telaIds.push(node.id);
    });

    if (telaIds.length <= 0) {
        Alerta("Selecione pelo menos uma tela para continuar");
        return;
    }

    if (isEmptyOrNull($("#nomeGrupoUsuario").val())) {
        Alerta("Informe o Nome do Grupo");
        return;
    }

    var obj = {};
    obj.ListaTela = telaIds;
    obj.nomeGrupoUsuario = $("#nomeGrupoUsuario").val();
    obj.IDGrupoPermissao = idGrupoUsuario;
        
    PostDados("Administracao/SalvarPermissaoGrupoUsuario", { dados: JSON.stringify(obj) }, SalvarPermissaoUsuarioResult);
}

function limparModal() {

    $("#nomeGrupoUsuario").val("");

    $('#tree').jstree('deselect_all');
}

function SalvarPermissaoUsuarioResult() {

    $("#modalGrupoUsuario").modal('hide');
    limparModal();
    $.alert("Grupo de Usuário Cadastrado com Sucesso!");

    Post("Administracao/BuscarGrupoUsuarios", montarTela, Erro);
}

