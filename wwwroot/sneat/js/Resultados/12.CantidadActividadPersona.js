$(document).ready(function () {
  ObtenerCantidadActividadesPersona();
  const filtrosBuscar = $("#fechaDesde, #fechaHasta");
  filtrosBuscar.on("change keyup", function () {
    ObtenerCantidadActividadesPersona();
  });
});


function Filtros() {
  const filtros = document.getElementById("filtrosContainer");
  filtros.style.display = filtros.style.display === "none" ? "block" : "none";
}

async function ObtenerCantidadActividadesPersona() {
    let fechaDesde = document.getElementById("fechaDesde").value;
    let fechaHasta = document.getElementById("fechaHasta").value;

    let filtros = {
        FechaDesde: fechaDesde,
        FechaHasta: fechaHasta
    }
  const res = await authFetch(
    "Estadisticas/InformeCantidadActividadPersona",
    {
      method: "POST",
      body: JSON.stringify(filtros)
    }
  )
   const data = await res.json();
    MostrarCantidadActividadesPersona(data);
}


function MostrarCantidadActividadesPersona(data) {
  const tbody = document.querySelector("#tablaCantidadActividadesPorPersona tbody");
  tbody.innerHTML = "";
  if (data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted">No hay actividades</td></tr>`;
    return;
  }

  data.forEach((persona) => {
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
            <td class='text-bold table-primary'>${persona.nombre} ${persona.email}</td>
            <td class='text-bold table-primary'>${persona.totalActividades}</td>
            <td class='text-bold table-primary'>${persona.duracionTotal}min</td>
            <td class='text-bold table-primary'>${persona.totalCalorias}kcal</td>
            <td class='text-bold table-primary'>${new Date(persona.fechaUltimaActividad).toLocaleDateString()}</td>
    `;
    tbody.appendChild(filaPersona);
    persona.tiposActividad.forEach((tiposActividad) => {
      const filaTipos = document.createElement("tr");
      filaTipos.innerHTML = `
        <td class='font-bold ps-4'>${tiposActividad.nombreTipo}</td>
        <td class='font-bold'>${tiposActividad.totalActividades}</td>
        <td class='font-bold'>${tiposActividad.duracionTotal}min</td>
        <td class='font-bold'>${tiposActividad.totalCalorias}kcal</td>
        <td class='font-bold'>${new Date(tiposActividad.fechaUltimaActividad).toLocaleDateString()}</td>
      `;
      tbody.appendChild(filaTipos);
    });
  });
}


ObtenerCantidadActividadesPersona();

