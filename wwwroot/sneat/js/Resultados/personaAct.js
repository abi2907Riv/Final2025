async function ObtenerActividadPersona() {
  const res = await authFetch("Actividades/PersonasActividades", {
    method: "POST"
  });
  const data = await res.json();
  MostrarActividadPersona(data);
}

function MostrarActividadPersona(data) {
  const tbody = document.querySelector("#tablaActividadPorPersona tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
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

    persona.actividades.forEach((act) => {

    let minutos = act.duracionMinutos;
    let duracionFormateada = "0min";

    if (minutos >= 60) {
      const h = Math.floor(minutos / 60);
      const m = minutos % 60;
      duracionFormateada = `${h}h ${m}min`;
    } else {
      duracionFormateada = `${minutos}min`;
    }


        const filaDetalle = document.createElement("tr");
        filaDetalle.innerHTML = `
                    <td>${new Date(act.fecha).toLocaleDateString()}</td>
                    <td>${duracionFormateada}</td>
                    <td>${act.caloriasQuemadas} (kcal)</td>
                `;
        tbody.appendChild(filaDetalle);
    });
  });
}

ObtenerActividadPersona();