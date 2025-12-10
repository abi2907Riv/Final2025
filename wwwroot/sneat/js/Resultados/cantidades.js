async function ObtenerCantidades() {
  const res = await authFetch("Actividades/Cantidades", {
    method: "POST",
  });
  const data = await res.json();
  MostrarCantidades(data);
}

function MostrarCantidades(data) {
  const tbody = document.querySelector("#tablaCantidad tbody");
  tbody.innerHTML = "";

  if (!data || data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
    return;
  }

  data.forEach((personas) => {
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
            <td colspan="4" class="fw-bold categorias-agrupadas">
                ${personas.nombre} || ${personas.email} 
            </td>
        `;
    tbody.appendChild(filaPersona);

    personas.tiposActividad.forEach((tipo) => {
    const filaTipo = document.createElement("tr");
    filaTipo.innerHTML = `
        <td>${tipo.nombreTipo}</td>
        <td class="text-center">${tipo.totalActividades}</td>
        <td class="text-center">${tipo.totalMayor30}</td>
        <td class="text-center">${tipo.totalMenor30}</td>
    `;
    tbody.appendChild(filaTipo);
    });
  });
}

ObtenerCantidades();