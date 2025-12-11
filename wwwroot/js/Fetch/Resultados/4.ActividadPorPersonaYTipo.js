async function ObtenerInformeActividadXPersonaYTipo() {
  const res = await authFetch("Actividades/InformeActividadXPersonaYTipoAcividad", {
    method: "POST",
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

  data.forEach((persona) => {

    // --- FILA DE PERSONA ---
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
      <td colspan="4" class="fw-bold categorias-agrupadas">
        ${persona.nombre} || ${persona.email}
      </td>
    `;
    tbody.appendChild(filaPersona);

    // --- TIPOS DE ACTIVIDAD ---
    persona.tiposActividad.forEach((tipo) => {

const filaTipo = document.createElement("tr");
filaTipo.innerHTML = `
  <td class="fw-bold table-success">${tipo.nombreTipo}</td>
  <td class="table-success">${tipo.totalMinutos} min</td>
  <td class="table-success">${tipo.totalCalorias} kcal</td>
  <td class="table-success">${tipo.cantidadActividades}</td>
`;
tbody.appendChild(filaTipo);

    });

  });
}


ObtenerInformeActividadXPersonaYTipo();
