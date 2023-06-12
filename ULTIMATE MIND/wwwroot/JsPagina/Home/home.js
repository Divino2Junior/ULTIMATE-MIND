
$(document).ready(function () {
    Post('Home/BuscarInfoHome', montarTela, Erro);
});

function montarTela(colaborador) {

    // Preencha as informações do colaborador na tela
    $("#fotoUsuario").attr("src", colaborador.foto);
    $(".matricula").text("Matrícula: " + colaborador.matricula);
    $(".nome").text(colaborador.nome);
    $(".cpf").text("CPF: " + colaborador.cpf);
    $(".status").text("Status: " + colaborador.status);
    $(".email").text("Email: " + colaborador.email);
    $(".funcao").text("Função: " + colaborador.funcao);
}