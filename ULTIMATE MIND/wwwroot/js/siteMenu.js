$(document).ready(function () {
    obterUsuarioLogado();
});

// Função para construir o menu com base no JSON fornecido
function construirMenu(menuJson) {
    const menuContainer = document.querySelector('#menu-dinamico');

    // Limpa o conteúdo existente no menu
    menuContainer.innerHTML = '';

    // Percorre o JSON e constrói os itens e subitens do menu
    for (let i = 0; i < menuJson.length; i++) {
        const menuItem = construirItem(menuJson[i]);
        menuContainer.appendChild(menuItem);
    }
}

function construirItem(item) {
    const menuItem = document.createElement('a');
    menuItem.textContent = item.tela;
    menuItem.href = item.link || '#'; // Se não houver link, use '#' por padrão

    // Adicionar uma classe específica para o Home
    if (item.tela === 'Home') {
        menuItem.classList.add('nav-link', '/Home/Index');
    } else if (item.tela === 'Sair') {
        menuItem.classList.add('nav-link', '#');
        menuItem.setAttribute('onclick', 'logout();'); // Adicionar o atributo "onclick" para o Sair
    } else if (item.submenu && item.submenu.length > 0) {
        menuItem.classList.add('nav-link', 'dropdown-toggle');
        menuItem.dataset.bsToggle = 'dropdown';
        menuItem.dataset.bsTarget = `#submenu-${item.id}`;

        const submenuList = document.createElement('ul');
        submenuList.classList.add('dropdown-menu');
        submenuList.setAttribute('aria-labelledby', `menu-${item.id}`);

        // Percorre os subitens e constrói o submenu
        for (let j = 0; j < item.submenu.length; j++) {
            const submenuItemElement = document.createElement('li');
            submenuItemElement.innerHTML = `<a class="dropdown-item" href="${item.submenu[j].link || '#'}">${item.submenu[j].tela}</a>`;
            submenuList.appendChild(submenuItemElement);
        }

        const dropdownContainer = document.createElement('li');
        dropdownContainer.classList.add('nav-item', 'dropdown');
        dropdownContainer.appendChild(menuItem);
        dropdownContainer.appendChild(submenuList);

        return dropdownContainer;
    }

    const navItem = document.createElement('li');
    navItem.classList.add('nav-item');
    navItem.appendChild(menuItem);

    return navItem;
}

// Função para fazer a requisição e obter o menu filtrado
function obterMenuFiltrado() {

    Post('Home/GetMenu', construirMenu, Erro);

}

function obterUsuarioLogado() {
    Post('Home/ObterUsuarioLogado', ObterUsuarioLogadoResult, Erro);
}

function ObterUsuarioLogadoResult(resultado) {
    if (resultado <= 0) {
        window.location = urlSite + "Login/"
    }
    else {
        obterMenuFiltrado();
    }
}