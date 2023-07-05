
$(document).ready(function () {
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

