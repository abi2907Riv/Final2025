async function ObtenerInformeActividadXPersona() {
  const res = await authFetch("Actividades/LigerasIntensas", {
    method: "POST",
  });
  const data = await res.json();
  MostrarInformeActividad(data);
}

function MostrarInformeActividad(data) {
  const tbody = document.querySelector("#tablaLigerasIntensivas tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
    return;
  }

    data.forEach((persona) => {
        const fila = document.createElement("tr");
        fila.innerHTML = `
        <td>${persona.nombre}</td>
        <td>${persona.totalActividades}</td>
        <td>${persona.ligeras}</td>
        <td>${persona.intensas}</td>
        <td>${persona.porcentajeIntensas}%</td>
        <td>${new Date(persona.fechaUltimaActividad).toLocaleDateString()}</td>
        <td>${new Date(persona.fechaUltimaActividadIntensiva).toLocaleDateString()}</td>
        `;
        tbody.appendChild(fila);
    });
}


ObtenerInformeActividadXPersona();