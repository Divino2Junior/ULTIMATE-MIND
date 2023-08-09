
$(document).ready(function () {

    Post('Home/ConsultarBtnTrocarEmpresa', ConsultarBtnTrocarEmpresaResult, Erro);
    Post('Home/BuscarInfoHome', montarTela, Erro);
});

function montarTela(colaborador) {

    if (colaborador.foto) {
        var fotoAtualizada = colaborador.foto + '?t=' + new Date().getTime();
        $('#fotoUsuario').attr('src', fotoAtualizada);
    } else {
        // Se a foto não existe, exibir o ícone padrão
        $('#fotoUsuario').attr('src', '/icons/homem-usuario.png');
    }
    $(".matricula").text("Matrícula: " + colaborador.matricula);
    $(".nome").text(colaborador.nome);
    $(".cpf").text("CPF: " + colaborador.cpf);
    $(".status").text("Status: " + colaborador.status);
    $(".email").text("Email: " + colaborador.email);
    $(".funcao").text("Função: " + colaborador.funcao);
}


function abrirModalTrocarEmpresa() {
    $("#modalContratante").modal('show');
}

function salvarEmpresa() {

    var id = $('#contratoSelect').val();
    Post("Login/SalvarEmpresaLogin/" + id, salvarLoginResult);
}

function abrirModalTrocarSenha() {
    $("#modalAlterarSenha").modal('show');
}

function ConsultarBtnTrocarEmpresaResult(retorno) {
    if (retorno) {
        $("#btnTrocarEmpresa").removeClass('d-none');
    }
    else {
        $("#btnTrocarEmpresa").addClass('d-none');
    }
}

function salvarNovaSenha() {
    // Limpar mensagens de erro
    $(".erro-senha-atual").text("");
    $(".erro-nova-senha").text("");
    $(".erro-confirmar-senha").text("");

    // Obter os valores dos campos de senha
    var senhaAtual = $("#senhaAtual").val();
    var novaSenha = $("#novaSenha").val();
    var confirmarSenha = $("#confirmarSenha").val();

    // Verificar se a nova senha e a confirmação de senha são iguais
    if (novaSenha !== confirmarSenha) {
        $(".erro-nova-senha").text("Nova senha e Confirmação de senha não são iguais. Por favor, verifique.");
        return;
    }

    // Verificar se a senha atual é igual à nova senha
    if (senhaAtual === novaSenha) {
        $(".erro-nova-senha").text("A nova senha não pode ser igual à senha atual. Por favor, escolha uma senha diferente.");
        return;
    }

    $.ajax({
        url: "/Home/AlterarSenha",
        method: "POST",
        data: {
            senhaAtual: senhaAtual,
            novaSenha: novaSenha
        },
        success: function (data) {
            if (data.success) {
                $("#senhaAtual").val("");
                $("#novaSenha").val("");
                $("#confirmarSenha").val("");
                $.alert("Senha alterada com sucesso!");
                $("#modalAlterarSenha").modal("hide");
            } else {
                $.alert("Erro: " + data.message);
            }
        },
        error: function (error) {
            $.alert("Erro: " + error);
        }
    });
}

function salvarLoginResult() {
    $.alert("Troca de empresa realizada sucesso!");
    $("#modalContratante").modal("hide");
}