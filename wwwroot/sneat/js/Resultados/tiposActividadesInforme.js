async function ObtenerInformeActividadXTipo() {
  const res = await authFetch("Actividades/TipoActividadActividades", {
    method: "POST",
  });
  const data = await res.json();
  MostrarInformeActividadTipo(data);
}

function MostrarInformeActividadTipo(data) {
  const tbody = document.querySelector("#tablaActividadPorTipo tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
    return;
  }

  data.forEach((tipoActividad) => {
    const filaTipo = document.createElement("tr");
    filaTipo.innerHTML = `
            <td colspan="4" class="fw-bold categorias-agrupadas">
                ${tipoActividad.nombreTipo} || ${tipoActividad.caloriasPorMinuto}  kcal/min
            </td>
        `;
    tbody.appendChild(filaTipo);

    tipoActividad.actividades.forEach((det) => {
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
                    <td>${det.caloriasQuemadas} (kcal)</td>
                `;
        tbody.appendChild(filaDetalle);
    });
  });
}

ObtenerInformeActividadXTipo();