$(document).ready(function () {

    $(".toggle-input").click(function () {
        var target = $(this).data("target");
        $(target).toggle();
    });

    $(".btn-primary").click(function () {
        var target = $(this).data("target");
        $(target).modal("show");
    });
    Post("DepartamentoPessoal/ConsultarPontoDia", montarTela);
    // Atualizar o cronômetro a cada minuto (ou o intervalo desejado)
    setInterval(function () {
        calcularJornada();
    }, 60000);

});

function montarTela(dados) {
    tag = "";
    tag += "Data: " + dados.dataAtual;

    $("#divDataDia").html(tag);

    if (dados.inicioDia) {
        $("#btnInicioDia").addClass("d-none");
        $("#entrada1").removeClass("d-none");
        $("#entrada1").val(dados.inicioDia);
    }

    if (dados.inicioAlmoco) {
        $("#btnInicioAlmoco").addClass("d-none");
        $("#saida1").removeClass("d-none");
        $("#saida1").val(dados.inicioAlmoco);
    }

    if (dados.fimAlmoco) {
        $("#btnFimAlmoco").addClass("d-none");
        $("#entrada2").removeClass("d-none");
        $("#entrada2").val(dados.fimAlmoco);
    }
    if (dados.fimDia) {
        $("#btnFimDia").addClass("d-none");
        $("#saida2").removeClass("d-none");
        $("#saida2").val(dados.fimDia);
    }

    calcularJornada();
}

function calcularJornada() {
    // Obter os valores das marcações de início e fim do dia e início do almoço
    var inicioDia = $("#entrada1").val();
    var fimDia = $("#saida2").val();
    var inicioAlmoco = $("#saida1").val();

    // Verificar se as marcações foram preenchidas corretamente
    if (inicioDia) {

        var inicioDiaDate = new Date("2000-01-01T" + inicioDia);
        var horaInicioCronometro = new Date();
        horaInicioCronometro.setHours(inicioDiaDate.getHours());
        horaInicioCronometro.setMinutes(inicioDiaDate.getMinutes());

        var jornadaTotalMs = 0;

        if (fimDia && inicioAlmoco) {
            // Converter os valores para objetos Date para realizar o cálculo

            var fimDiaDate = new Date("2000-01-01T" + fimDia);
            var inicioAlmocoDate = new Date("2000-01-01T" + inicioAlmoco);
            // Calcular o tempo da jornada da manhã (do início do dia ao início do almoço)
            var jornadaManhaMs = inicioAlmocoDate - inicioDiaDate;

            // Calcular o tempo da jornada da tarde (do fim do almoço ao fim do dia)
            var jornadaTardeMs = fimDiaDate - inicioAlmocoDate;

            // Calcular o tempo total da jornada (soma da jornada da manhã e da tarde)
            jornadaTotalMs = jornadaManhaMs + jornadaTardeMs;

            // Atualizar o valor no campo de input para exibir a jornada total
            $("#cronometroJornada").val(formatarTempo(jornadaTotalMs));
        }
        else {
            var horaAtual = new Date();
            var tempoDecorridoMs = horaAtual - horaInicioCronometro;
            var jornadaAtualMs = jornadaTotalMs + tempoDecorridoMs;

            $("#cronometroJornada").val(formatarTempo(jornadaAtualMs));
        }

    }
}

function pad(number, length) {
    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }
    return str;
}

function formatarTempo(tempoEmMilissegundos) {
    var horas = Math.floor(tempoEmMilissegundos / (1000 * 60 * 60));
    var minutos = Math.floor((tempoEmMilissegundos % (1000 * 60 * 60)) / (1000 * 60));
    var segundos = Math.floor((tempoEmMilissegundos % (1000 * 60)) / 1000);

    return pad(horas, 2) + ":" + pad(minutos, 2);
}


function inicioDia() {
    if ("geolocation" in navigator) {
        // Solicitar permissão para acessar a localização do usuário
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            var obj = {};
            obj.TipoMarcacao = 1;
            obj.Latitude = lat;
            obj.Longitude = lng;

            PostDados("DepartamentoPessoal/SalvarMarcacaoPonto", { dados: JSON.stringify(obj) }, salvarMarcacaoPontoResult);
        }, function (error) {
            Alerta("Erro ao obter localização: " + error.message);
        });
    } else {
        Alerta("Geolocalização não suportada pelo navegador.");
    }
}


function inicioAlmoco() {
    if ("geolocation" in navigator) {
        // Solicitar permissão para acessar a localização do usuário
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            var obj = {};
            obj.TipoMarcacao = 2;
            obj.Latitude = lat;
            obj.Longitude = lng;

            PostDados("DepartamentoPessoal/SalvarMarcacaoPonto", { dados: JSON.stringify(obj) }, salvarMarcacaoPontoResult);
        }, function (error) {
            Alerta("Erro ao obter localização: " + error.message);
        });
    } else {
        Alerta("Geolocalização não suportada pelo navegador.");
    }
}

function fimAlmoco() {
    if ("geolocation" in navigator) {
        // Solicitar permissão para acessar a localização do usuário
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            var obj = {};
            obj.TipoMarcacao = 3;
            obj.Latitude = lat;
            obj.Longitude = lng;

            PostDados("DepartamentoPessoal/SalvarMarcacaoPonto", { dados: JSON.stringify(obj) }, salvarMarcacaoPontoResult);
        }, function (error) {
            Alerta("Erro ao obter localização: " + error.message);
        });
    } else {
        Alerta("Geolocalização não suportada pelo navegador.");
    }
}

function fimDia() {
    if ("geolocation" in navigator) {
        // Solicitar permissão para acessar a localização do usuário
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            var obj = {};
            obj.TipoMarcacao = 4;
            obj.Latitude = lat;
            obj.Longitude = lng;

            PostDados("DepartamentoPessoal/SalvarMarcacaoPonto", { dados: JSON.stringify(obj) }, salvarMarcacaoPontoResult);
        }, function (error) {
            Alerta("Erro ao obter localização: " + error.message);
        });
    } else {
        Alerta("Geolocalização não suportada pelo navegador.");
    }
}

function salvarMarcacaoPontoResult(dados) {

    if (dados.inicioDia) {
        $.alert("Marcação do Ponto: Inicio do dia, realizado com sucesso!");
        $("#btnInicioDia").addClass("d-none");
        $("#entrada1").removeClass("d-none");
        $("#entrada1").val(dados.inicioDia);
    }

    if (dados.inicioAlmoco) {
        $.alert("Marcação do Ponto: Inicio do Almoço, realizado com sucesso!");
        $("#btnInicioAlmoco").addClass("d-none");
        $("#saida1").removeClass("d-none");
        $("#saida1").val(dados.inicioAlmoco);
    }

    if (dados.fimAlmoco) {
        $.alert("Marcação do Ponto: Fim do Almoço, realizado com sucesso!");
        $("#btnFimAlmoco").addClass("d-none");
        $("#entrada2").removeClass("d-none");
        $("#entrada2").val(dados.fimAlmoco);
    }
    if (dados.fimDia) {
        $.alert("Marcação do Ponto: Fim do dia, realizado com sucesso!");
        $("#btnFimDia").addClass("d-none");
        $("#saida2").removeClass("d-none");
        $("#saida2").val(dados.fimDia);
    }
}

function abrirModalHistorico() {
    Post("DepartamentoPessoal/ConsultarHistoricoPonto", montarLinhasTabela);
}

function montarLinhasTabela(dados) {
    var tbody = $("#folhaDePontoBody");

    // Limpar a tabela antes de inserir os novos dados (caso ela já tenha algum conteúdo)
    tbody.empty();

    // Iterar sobre os dados e criar as linhas da tabela
    for (var i = 0; i < dados.length; i++) {
        var dia = dados[i].dataAtual;
        var inicioDia = dados[i].inicioDia;
        var inicioAlmoco = dados[i].inicioAlmoco;
        var fimAlmoco = dados[i].fimAlmoco;
        var fimDia = dados[i].fimDia;
        var jornada = dados[i].jornada;

        var tr = $("<tr>");
        tr.append("<td>" + dia + "</td>");
        tr.append("<td><input type='time' class='form-control' value='" + inicioDia + "' disabled></td>");
        tr.append("<td><input type='time' class='form-control' value='" + inicioAlmoco + "' disabled></td>");
        tr.append("<td><input type='time' class='form-control' value='" + fimAlmoco + "' disabled></td>");
        tr.append("<td><input type='time' class='form-control' value='" + fimDia + "' disabled></td>");
        tr.append("<td><input type='time' class='form-control' value='" + jornada + "' disabled></td>");

        // Adicionar a linha à tabela
        tbody.append(tr);
    }

    // Abrir a modal após montar as linhas
    $("#folhaDePontoModal").modal("show");
}
