$(document).ready(function () {
  ObtenerInformeActividadXPersona();
  const filtrosBuscar = $(
    "#personaBuscar, #filtroFechaDesde, #filtroFechaHasta"
  );
  filtrosBuscar.on("change keyup", function () {
    ObtenerInformeActividadXPersona();
  });
});

async function ObtenerInformeActividadXPersona() {
    let personaId = parseInt(document.getElementById("personaBuscar").value) || 0
    let filtro = {
        PersonaID: parseInt(personaId) || 0,
    }

  const res = await authFetch("Estadisticas/InformeTotalAactividadesXPersona", {
    method: "POST",
    body: JSON.stringify(filtro)
  });
  const data = await res.json();
  MostrarInformeActividad(data);
}

function MostrarInformeActividad(data) {
  const tbody = document.querySelector("#tablaTotalActividadesXPersona tbody");
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

    const filaTotales = document.createElement("tr");
    filaTotales.classList.add("table-success", "fw-bold");

    filaTotales.innerHTML = `
      <td>${persona.cantidadActividades}</td>
      <td>${persona.totalMinutos} min</td>
      <td>${persona.totalCalorias} kcal</td>
    `;

    tbody.appendChild(filaTotales);
  });
}


ObtenerInformeActividadXPersona();