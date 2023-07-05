var urlSite;

$(document).ready(function () {

    $('#txtcpf').mask('000.000.000-00');
    $("#txtcpf").on("input", function () {
        ocultarMensagemErro();
    });

    $("#frmLogin").on("submit", function (event) {
        event.preventDefault();
        PostForm(this, loginSucesso)
    });
});

function loginSucesso(data) {
    if (data <= 1)
        window.location = urlSite + "Home/"
    else {
        $("#modalContratante").modal('show');
    }
}

function validarEntrada() {
    if (!isCPFValido($("#txtcpf").val())) {
        var errorMessage = "CPF Inválido!";
        $("#error-message").text(errorMessage).show();
        return false;
    }
    return true;
}

function ocultarMensagemErro() {
    $("#error-message").hide();
}

function salvarEmpresa() {

    var id = $('#contratoSelect').val();
    Post("Login/SalvarEmpresaLogin/" + id, salvarLoginResult);
}

function salvarLoginResult(){
    window.location = urlSite + "Home/"
}