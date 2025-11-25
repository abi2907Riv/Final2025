async function ObtenerInformeActividadXPersona() {
  const res = await authFetch("Actividades/InformeActividadXPersona", {
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
                    ${tipo.nombreTipo} — ${tipo.caloriasPorMinuto} Calorias por Minuto
                </td>
            `;
      tbody.appendChild(filaTipo);

      tipo.actividades.forEach((det) => {

    let minutos = det.duracionMinutos;
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
                    <td>${new Date(det.fecha).toLocaleDateString()}</td>
                    <td>${duracionFormateada}</td>
                    <td>${det.caloriasQuemadas}</td>
                `;
        tbody.appendChild(filaDetalle);
      });
    });
  });
}

ObtenerInformeActividadXPersona();