
function importarContraCheque() {
    Post("Administracao/ImportarContraChequeArquivo", montarTela, Erro);
}


function montarTela() {
    $.alert("Contras Cheques Cadastrados com Sucesso!!");

}