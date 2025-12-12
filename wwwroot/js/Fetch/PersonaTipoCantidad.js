$(document).ready(function () {
  ObtenerInformeCantidades();
  const filtrosBuscar = $(
    "#filtroFechaDesde, #filtroFechaHasta"
  );
  filtrosBuscar.on("change keyup", function () {
    ObtenerInformeCantidades();
  });
});

function Filtros() {
  const filtros = document.getElementById("filtrosContainer");
  filtros.style.display = filtros.style.display === "none" ? "block" : "none";
}

async function ObtenerInformeCantidades() {
    let fechaDesde = document.getElementById("filtroFechaDesde").value
    let fechaHasta = document.getElementById("filtroFechaHasta").value
    let filtro = {
        FechaDesde: fechaDesde,
        FechaHasta: fechaHasta
    }
  const res = await authFetch("Actividades/CantidadesPorTipo", {
    method: "POST",
    body: JSON.stringify(filtro)
  });
  const data = await res.json();
  MostrarInformeCantidad(data);
}

function MostrarInformeCantidad(data) {
  const tbody = document.querySelector("#tablaCantidades tbody");
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
        ${persona.nombre}
      </td>
    `;
    tbody.appendChild(filaPersona);

    persona.tiposActividad.forEach((tipo) => {
    const filaTipo = document.createElement("tr");
    filaTipo.innerHTML = `
      <td>${tipo.nombreTipo}</td>
      <td>${tipo.totalMinutos}min</td>
      <td>${tipo.totalActividades} </td>
    `;
    tbody.appendChild(filaTipo);

    })
  });
}


ObtenerInformeCantidades();