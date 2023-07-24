
function criarAssinaturaEletronica() {
    Post("Signatarios/CriarSignatarios", montarTela, Erro);
}


function montarTela() {
    $.alerta("Assinaturas Cadastradas com sucesso!!");
}