var urlSite;

$("#txtCPF").mask("999.999.999-99");

function loginSucesso(data) {
    if (data <= 1)
        window.location = urlSite + "Home/"
}

$("#frmLogin").on("submit", function (event) {
    event.preventDefault();
    PostForm(this, loginSucesso)
});

function validarEntrada() {
    if (!isCPFValido($("#txtCPF").val())) {
        Alerta("CPF Inválido!");
        return false;
    }
    return true;
}