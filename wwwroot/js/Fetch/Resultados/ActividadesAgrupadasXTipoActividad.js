async function ObtenerInformeActividadXPersona() {

  const res = await authFetch("Estadisticas/ActividadesAgrupadasXTipoActividad", {
    method: "POST",
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

  data.forEach((tipoActividad) => {
    const filaTipoActividad = document.createElement("tr");
    filaTipoActividad.innerHTML = `
      <td colspan="4" class="fw-bold categorias-agrupadas">
        ${tipoActividad.nombreTipo}
      </td>
    `;
    tbody.appendChild(filaTipoActividad);

    const filaTotales = document.createElement("tr");
    filaTotales.classList.add("table-success", "fw-bold");

    filaTotales.innerHTML = `
      <td>${tipoActividad.cantidadActividades}</td>
      <td>${tipoActividad.totalMinutos} min</td>
      <td>${tipoActividad.totalCalorias} kcal</td>
    `;

    tbody.appendChild(filaTotales);
  });
}


ObtenerInformeActividadXPersona();