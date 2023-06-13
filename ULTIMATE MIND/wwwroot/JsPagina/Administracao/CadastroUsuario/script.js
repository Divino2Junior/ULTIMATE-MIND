
$(document).ready(function() {
  //Manipulação do datatable
  $('#table-colab').DataTable({
    columnDefs: [
      { targets: 3, orderable: false }
    ],

    rowReorder: {
      selector: 'td:nth-child(2)'
  },


    // "scrollX": true,
    "responsive": true,
    // "scrollY": "50vh",
    "scrollCollapse": true,
    // "paging": false,
  });

});

$(document).ready(function () {

  $('#m-cpf').mask('999.999.999-99');

  $("#m-telefone").mask("(99) 9999-9999"); 

  $('#select-funcao').select2({
    dropdownParent: $('#exampleModal')
  });

  const emailInput = $('#m-email');

  // Aplica a máscara do email
  emailInput.mask('A', {
    translation: {
      A: { pattern: /[\w@\-.+]/, recursive: true }
    }
  });

  // Validação de email ao perder o foco
  emailInput.on('blur', function() {
    const email = $(this).val();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    if (!emailRegex.test(email)) {
      alert('Email inválido');
      $(this).val('');
    }
  });

});  

