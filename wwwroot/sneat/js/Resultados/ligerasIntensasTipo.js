async function ObtenerInformeActividadXPersona() {
  const res = await authFetch("Actividades/LigerasIntensasTipo", {
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
        <td class="table-success">${persona.nombre}</td>
        <td class="table-success">${persona.totalActividades}</td>
        <td class="table-success">${persona.ligeras}</td>
        <td class="table-success">${persona.intensas}</td>
        <td class="table-success">${persona.porcentajeIntensas}%</td>
        <td class="table-success">${new Date(persona.fechaUltimaActividad).toLocaleDateString()}</td>
    `;
    tbody.appendChild(fila);

    // Mostrar los tipos de actividad de esa persona
    persona.tiposActividad?.forEach((tipo) => {
        const filaTipo = document.createElement("tr");
        filaTipo.innerHTML = `
            <td>${tipo.nombreTipo}</td>
            <td>${tipo.totalActividades}</td>
            <td>${tipo.totalMenor30}</td>
            <td>${tipo.totalMayor30}</td>
            <td>${tipo.porcentajeIntensivas}%</td>
            <td>${new Date(tipo.fechaUltimaActividad).toLocaleDateString()}</td>
        `;
        tbody.appendChild(filaTipo);
    });
});

}


ObtenerInformeActividadXPersona();