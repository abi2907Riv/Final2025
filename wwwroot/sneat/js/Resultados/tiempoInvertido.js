async function ObtenerTiempoInvertido() {
  const res = await authFetch("Actividades/TiempoInvertido", {
    method: "POST"
  });
  const data = await res.json();
  MostrarTiempo(data);
}

function MostrarTiempo(data) {
  const tbody = document.querySelector("#tablaTiempoInvertido tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="4" class="text-center text-muted">
          No hay actividades registradas
        </td>
      </tr>`;
    return;
  }

  data.forEach((persona) => {
    // Fila principal de la persona
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
      <td colspan="4" class="fw-bold categorias-agrupadas">
        ${persona.nombre} || ${persona.email}
      </td>`;
    tbody.appendChild(filaPersona);

    // Ahora sus tipos de actividad
    persona.tiposActividad.forEach((tipo) => {
      const filaTipo = document.createElement("tr");
      filaTipo.innerHTML = `
        <td class="table-success" colspan="4">
          <strong>${tipo.nombreTipo}</strong> — 
          Promedio tiempo invertido: 
          <strong>${tipo.promedioTiempo} min.</strong>
        </td>`;
      tbody.appendChild(filaTipo);
    });
  });
}
ObtenerTiempoInvertido();