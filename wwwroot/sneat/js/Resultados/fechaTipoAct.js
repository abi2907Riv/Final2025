async function ObtenerTickets() {
  const res = await authFetch("Actividades/FechaTipoActividad", {
    method: "POST"
  })
    const data = await res.json();
  MostrarInformeFechaTipo(data);
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR LOS TICKETS/////
////////////////////////////////////////////
function MostrarInformeFechaTipo(data) {
  const tbody = document.querySelector("#tablaTickets tbody");
  tbody.innerHTML = "";

  if (!data.length) {
    tbody.innerHTML = `<tr><td colspan="3" class="text-center text-muted">No hay tickets</td></tr>`;
    return;
  }

  data.forEach((fecha) => {
    const filaFecha = document.createElement("tr");
    filaFecha.innerHTML = `
            <td class='text-bold categorias-agrupadas' colspan='5'>${new Date(fecha.fecha).toLocaleDateString()}</td>
        `;
    tbody.appendChild(filaFecha);
    fecha.tiposActividad.forEach((tipos) => {
      const filaTipo = document.createElement("tr");
      filaTipo.innerHTML = `
                <td class='text-bold categorias-subagrupadas' colspan='5'>${tipos.nombreTipo}</td>
            `;
      tbody.appendChild(filaTipo);

      tipos.actividades.forEach((act) => {
        const filaActividades = document.createElement("tr");
        filaActividades.innerHTML = `
                    <td class='text-bold'>${act.persona || ""}</td>
                    <td class="text-center text-bold">${act.duracionMinutos}min</td>
                    <td class="text-center text-bold">${
                      act.caloriasQuemadas}kcal</td>

                `;
        tbody.appendChild(filaActividades);
      });
    });
  });
}

ObtenerTickets();