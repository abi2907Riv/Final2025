async function ObtenerInformeActividadXPersonaYTipo() {
  const res = await authFetch("Estadisticas/DuracionPromedioActividadPorPersona", {
    method: "POST",
  });
  const data = await res.json();
  MostrarInformeActividad(data);
}

function MostrarInformeActividad(data) {
  const tbody = document.querySelector("#tablaActividadPorPersona tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="3" class="text-center text-muted">No hay actividades registradas</td></tr>`;
    return;
  }

  data.forEach((persona) => {
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
      <td colspan="3" class="fw-bold categorias-agrupadas">
        ${persona.nombre} || ${persona.email}
      </td>
    `;
    tbody.appendChild(filaPersona);

    const filaDatos = document.createElement("tr");
    filaDatos.innerHTML = `
      <td class="table-success">${persona.promedioMinutos}min </td>
      <td class="table-success">${persona.promedioCalorias} kcal</td>
    `;
    tbody.appendChild(filaDatos);
  });
}






ObtenerInformeActividadXPersonaYTipo();
