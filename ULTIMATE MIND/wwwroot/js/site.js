var urlSite;


$(document).ready(function () {
    ocultarLoading();
});

function Alerta(pMsg) {
    $.alert({
        title: 'Atenção!',
        content: pMsg,
        autoClose: true,
        closeTime: 1000000,
        type: 'warning'
    });
}

function Sucesso(pMsg) {
    $.alerta(pMsg, {
        title: 'Sucesso',
        autoClose: true,
        type: 'success'
    });
}

function Erro(pMsg) {
    $.confirm({
        columnClass: 'col-md-6 offset-md-3',
        title: 'Erro!',
        content: pMsg,
        type: 'red',
        typeAnimated: true,
        boxWidth: '30%',
        useBootstrap: false,
        columnClass: 'col-md-6 offset-md-3',
        containerFluid: false,
        draggable: false,
        autoClose: true,
        closeTime: 1000000,
        buttons: {
            sim: {
                text: 'OK'
            },
        }
    });
}

function Post(metodo, pSucesso, pErro) {

    var end = urlSite + metodo;

    mostrarLoading();

    $.ajax({
        type: "POST",
        url: end,
        data: "",
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (pErro != null && pErro != undefined)
                pErro(request?.responseJSON?.message);
            else
                Erro(request?.responseJSON?.message);
        }
    });
}

function PostJSON(metodo, pSucesso, pErro) {

    var end = urlSite + metodo;

    mostrarLoading();

    $.ajax({
        type: "POST",
        url: end,
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        data: "",
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (pErro != null && pErro != undefined)
                pErro(request?.responseJSON?.message);
            else
                Erro(request?.responseText);
        }
    });
}


function PostDadosSemLoading(metodo, dados, pSucesso, pErro) {

    var end = urlSite + metodo;
    $.ajax({
        url: end,
        type: 'POST',
        data: dados,
        success: function (sesponseTest) {
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            if (pErro != null && pErro != undefined)
                pErro(request.responseJSON.message);
            else
                Erro(request.responseJSON.message);
        }
    });
}

function PostForm(pForm, pSucesso, pErro) {
    mostrarLoading();
    var url = $(pForm).attr("action");
    var formData = $(pForm).serialize();
    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (pErro != null && pErro != undefined)
                pErro(request?.responseJSON?.message);
            else
                Erro(request?.responseJSON?.message);
        }
    })
}

function PostDados(metodo, dados, pSucesso, pErro) {

    var end = urlSite + metodo;
    mostrarLoading();
    $.ajax({
        url: end,
        type: 'POST',
        data: dados,
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (pErro != null && pErro != undefined)
                pErro(request?.responseJSON?.message);
            else
                Erro(request?.responseJSON?.message);
        }
    });
}

function PostDadosJSON(metodo, dados, pSucesso, pErro) {

    var end = urlSite + metodo;
    mostrarLoading();
    $.ajax({
        url: end,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        data: dados,
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (pErro != null && pErro != undefined)
                pErro(request?.responseJSON?.message);
            else
                pSucesso(request?.statusText);
        }
    });
}

function PostFormUpload(pForm, idUpload, dados, pSucesso, pErro) {
    mostrarLoading();
    var url = $(pForm).attr("action");
    //var formData = $(pForm).serialize();
    var data = new FormData();

    //var fileUpload = $("[id*=" + idUpload+"]").get(0);
    //var fileUpload =
    $('[id*="' + idUpload + '"]').each(function (index, element) {
        var files = element.files;
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }
    });

    if (dados != null || dados != undefined)
        data.append('dados', JSON.stringify(dados))

    $.ajax({
        url: url,
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        success: function (sesponseTest) {
            ocultarLoading();
            pSucesso(sesponseTest);
        },
        error: function (request, message, error) {
            ocultarLoading();
            if (request != null &&
                request != undefined &&
                request.responseJSON != null &&
                request.responseJSON != undefined &&
                request.responseJSON.message != null &&
                request.responseJSON.message != undefined) {
                if (pErro != null && pErro != undefined)
                    pErro(request?.responseJSON?.message);
                else
                    Erro(request?.responseJSON?.message);
            }
            else if (error != null && error != undefined) {
                Erro(error);
            }
        }
    })
}

function isEmptyOrNull(pValue) {
    if (pValue == null || pValue == undefined || pValue == '')
        return true;
    else
        return false;
}

function isCPFValido(cpf) {
    cpf = cpf.replace(/[^\d]+/g, '');
    if (cpf == '') return false;
    // Elimina CPFs invalidos conhecidos
    if (cpf.length != 11 ||
        cpf == "00000000000" ||
        cpf == "11111111111" ||
        cpf == "22222222222" ||
        cpf == "33333333333" ||
        cpf == "44444444444" ||
        cpf == "55555555555" ||
        cpf == "66666666666" ||
        cpf == "77777777777" ||
        cpf == "88888888888" ||
        cpf == "99999999999")
        return false;
    // Valida 1o digito
    add = 0;
    for (i = 0; i < 9; i++)
        add += parseInt(cpf.charAt(i)) * (10 - i);
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(9)))
        return false;
    // Valida 2o digito
    add = 0;
    for (i = 0; i < 10; i++)
        add += parseInt(cpf.charAt(i)) * (11 - i);
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(10)))
        return false;
    return true;
}

function ExportarRelatorioExcel(pNomeArquivo, pTitulo, pLista, pArrayTitulos, pArrayColunas) {


    //gerar relatorio para exportação
    if (pLista == null || pLista.length == 0) {
        Alerta("Não existem dados para serem exportados");
        return;
    }
    var tagExp = "<h3>" + pTitulo + "<h3>";
    tagExp += '<table style="border:0.7px solid">';
    tagExp += '<thead>';
    tagExp += '<tr>';

    for (var i = 0; i < pArrayTitulos.length; i++) {
        tagExp += '<th scope="col" style="border:0.7px solid;font-size:15px;vertical-align: middle;">' + pArrayTitulos[i] + '</th>';
    }

    tagExp += '</tr>';
    tagExp += '</thead>';
    tagExp += '<tbody>';
    for (var i = 0; i < pLista.length; i++) {
        var item = pLista[i];

        tagExp += '<tr>';
        for (var j = 0; j < pArrayColunas.length; j++) {
            tagExp += '<td style="border:0.7px solid;font-size:15px;vertical-align: middle;">' + item[pArrayColunas[j]] + '</td>';
        }

        tagExp += '</tr>';
    }

    tagExp += '</tbody>';
    tagExp += '</table>';
    var divRelatorio = document.createElement('div');
    divRelatorio.id = "div_relatorio_temp_automatico";
    $('body').prepend(divRelatorio);
    document.getElementById(divRelatorio.id).innerHTML = tagExp;

    var data_type = 'data:text/csv;charset=utf-8,%EF%BB%BF';
    var table_div = document.getElementById('div_relatorio_temp_automatico');
    var table_html = table_div.outerHTML.replace(/ /g, '%20');

    var a = document.createElement('a');
    a.href = data_type + ' ' + table_html;
    a.download = pNomeArquivo + '_' + Math.floor((Math.random() * 9999999) + 1000000) + '.xls';
    a.click();

    $('#div_relatorio_temp_automatico').remove();
}

function exportToCsv(arquivoNome, header, linhas, separador) {
    var processarLinhas = function (row) {
        var finalVal = '';
        for (var j = 0; j < row.length; j++) {
            var innerValue = row[j] === null ? '' : row[j].toString();
            if (row[j] instanceof Date) {
                innerValue = row[j].toLocaleString();
            };
            var result = innerValue.replace(/"/g, '""');
            if (result.search(/("|,|\n)/g) >= 0)
                result = '"' + result + '"';
            if (j > 0)
                finalVal += separador;
            finalVal += result;
        }
        return finalVal + '\n';
    };

    var csvFile = '';
    csvFile += processarLinhas(header);

    for (var i = 0; i < linhas.length; i++) {
        csvFile += processarLinhas(linhas[i]);
    }

    var blob = new Blob([csvFile], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(blob, arquivoNome);
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", arquivoNome);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}

function mostrarLoading() {
    $("#loading-overlay").fadeIn();
}

function ocultarLoading() {
    $("#loading-overlay").fadeOut();
}

function logout() {
    Post("Login/Logout", logoutResult);
}

function logoutResult() {
    window.location = "/Login/";
}