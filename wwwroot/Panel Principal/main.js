// ===========================================================
// CARGAR VISTAS DINÁMICAMENTE
// ===========================================================
function cargarVista(view) {
  fetch(`/views/${view}.html`)
    .then(res => {
      if (!res.ok) throw new Error(`No se encontró la vista: ${view}`);
      return res.text();
    })
    .then(html => {
      const app = document.getElementById('app');
      app.innerHTML = html;

      // Ejecutar scripts embebidos en la vista
      const tempDiv = document.createElement('div');
      tempDiv.innerHTML = html;
      const scripts = tempDiv.querySelectorAll('script');

      scripts.forEach(script => {
        const nuevoScript = document.createElement('script');
        if (script.src) {
          nuevoScript.src = script.src;
        } else {
          nuevoScript.textContent = script.textContent;
        }
        document.body.appendChild(nuevoScript);
      });
    })
    .catch(err => console.error(err));
}

// ===========================================================
// CAMBIO DE VISTAS SEGÚN HASH
// ===========================================================
function cargarVistaPorHash() {
  const vista = window.location.hash.replace('#', '') || 'home';
  cargarVista(vista);
  actualizarLinkActivo();
}

// ===========================================================
// NAVEGACIÓN Y MENÚ ACTIVO
// ===========================================================
function navigateTo(vista) {
  window.location.hash = vista;
}

function actualizarLinkActivo() {
  const vistaActual = window.location.hash.replace('#', '') || 'home';

  // Remover clase activa de todos los links
  const todosItemsNav = document.querySelectorAll('.menu-item');
  todosItemsNav.forEach(item => item.classList.remove('active'));

  const todosLinks = document.querySelectorAll('a.menu-link[href]');
  todosLinks.forEach(link => {
    const href = link.getAttribute('href');

    if (href.includes(vistaActual)) {
      link.classList.add('active');

      const itemPadre = link.closest('.menu-item');
      if (itemPadre) {
        itemPadre.classList.add('active');
      }
    } else {
      link.classList.remove('active');
    }
  });
}

// ===========================================================
// EVENTOS DE INICIO
// ===========================================================
window.addEventListener('hashchange', cargarVistaPorHash);
window.addEventListener('DOMContentLoaded', () => {
  cargarVistaPorHash();
});

// ===========================================================
// CERRAR COLLAPSE AL HACER CLICK FUERA
// ===========================================================
document.addEventListener('click', function (event) {
  const collapsesAbiertos = document.querySelectorAll('.collapse.show');
  collapsesAbiertos.forEach(collapseElement => {
    if (!collapseElement.contains(event.target)) {
      const collapse = bootstrap.Collapse.getInstance(collapseElement);
      collapse.hide();
    }
  });
});
