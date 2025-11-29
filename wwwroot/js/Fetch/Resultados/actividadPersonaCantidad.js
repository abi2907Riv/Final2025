async function ObtenerTipoActividadDrop() {
    const res = await authFetch('TiposActividades', {
        method: 'GET'
    })
    .then(response => response.json())
    .then(data => {
        MostrarTipoActividadDrop(data)
    })
    .catch(error => console.log('No se puede acceder al servicio', error));
}
function MostrarTipoActividadDrop(data) {
    let bodySelectFiltro = document.getElementById("filtroActividad");
    bodySelectFiltro.innerHTML = "";
    bodySelectFiltro.innerHTML = "<option value='0'>[Todas]</option>";

    const activos = data.filter(item => item.eliminado == false);
    activos.forEach(element => {
        let optFiltro = document.createElement("option");
        optFiltro.value = element.tipoActividadID;
        optFiltro.innerHTML = element.nombre;
        bodySelectFiltro.appendChild(optFiltro);
   })
}
ObtenerTipoActividadDrop();


$(document).ready(function () {
  ObtenerInformeActividadXPersona();
  const filtrosBuscar = $(
    "#personaBuscar, #filtroActividad, #filtroFechaDesde, #filtroFechaHasta"
  );
  filtrosBuscar.on("change keyup", function () {
    ObtenerInformeActividadXPersona();
  });
});

async function ObtenerInformeActividadXPersona() {
    let personaId = parseInt(document.getElementById("personaBuscar").value) || 0
    let tipoActividadId = parseInt(document.getElementById("filtroActividad").value) || 0
    let fechaDesde = document.getElementById("filtroFechaDesde").value
    let fechaHasta = document.getElementById("filtroFechaHasta").value

    let filtro = {
        PersonaID: parseInt(personaId) || 0,
        TipoActividadID: tipoActividadId,
        FechaDesde: fechaDesde,
        FechaHasta: fechaHasta
    }

  const res = await authFetch("Estadisticas/InformeActividadXPersonaCantidad", {
    method: "POST",
    body: JSON.stringify(filtro)
  });
  const data = await res.json();
  MostrarInformeActividad(data);
}

function MostrarInformeActividad(data) {
  const tbody = document.querySelector("#tablaActividadPorPersona tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
    return;
  }

  if (!Array.isArray(data)) {
    console.error("Error del backend:", data);
    return;
}

  data.forEach((persona) => {
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
      <td colspan="4" class="fw-bold categorias-agrupadas">
        ${persona.nombre} || ${persona.email}
      </td>
    `;
    tbody.appendChild(filaPersona);

    persona.tiposActividad.forEach((tipo) => {
      const filaTipo = document.createElement("tr");
      filaTipo.innerHTML = `
        <td colspan="4" class="fw-bold table-success">
            ${tipo.nombreTipo} — ${tipo.caloriasPorMinuto} kcal/min
        </td>
      `;
      tbody.appendChild(filaTipo);

      tipo.actividades.forEach((det) => {
        let minutos = det.duracionMinutos;
        let duracionFormateada = minutos >= 60
          ? `${Math.floor(minutos / 60)}h ${minutos % 60}min`
          : `${minutos}min`;

        const filaDetalle = document.createElement("tr");
        filaDetalle.innerHTML = `
          <td>${new Date(det.fecha).toLocaleDateString()}</td>
          <td>${duracionFormateada}</td>
          <td>${det.caloriasQuemadas} (kcal)</td>
        `;
        tbody.appendChild(filaDetalle);
      });
    });

    const filaTotales = document.createElement("tr");
    filaTotales.classList.add("table-warning", "fw-bold");

    filaTotales.innerHTML = `
      <td>Totales:</td>
      <td>${persona.totalMinutos} min</td>
      <td>${persona.totalCalorias} kcal</td>
    `;

    tbody.appendChild(filaTotales);
  });
}


ObtenerInformeActividadXPersona();