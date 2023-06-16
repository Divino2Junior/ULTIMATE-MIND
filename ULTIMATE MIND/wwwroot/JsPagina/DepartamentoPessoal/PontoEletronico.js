$(document).ready(function() {
  // Função para criar as linhas da folha de ponto
  function criarLinhasFolhaDePonto() {
    var $tbody = $("#folhaDePontoBody");
    for (var dia = 1; dia <= 30; dia++) {
      var $linha = $("<tr></tr>");
      $linha.append("<td>" + dia + "</td>");
      $linha.append('<td><input type="time" class="form-control"></td>');
      $linha.append('<td><input type="time" class="form-control"></td>');
      $linha.append('<td><input type="time" class="form-control"></td>');
      $linha.append('<td><input type="time" class="form-control"></td>');
      $linha.append('<td><input type="time" class="form-control"></td>');
      $tbody.append($linha);
    }
  }
  
  criarLinhasFolhaDePonto();

  // Ao abrir o modal, criar as linhas da folha de ponto
  $("#folhaDePontoModal").on("shown.bs.modal", function() {
    criarLinhasFolhaDePonto();
  });

  
});