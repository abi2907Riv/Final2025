$(document).ready(function () {
  ObtenerActividadesPorPersona();
  const filtrosBuscar = $(
    "#personaBuscar"
  );
  filtrosBuscar.on("change keyup", function () {
    ObtenerActividadesPorPersona();
  });
});
async function ObtenerActividadesPorPersona() {
    let personaIdBuscar = document.getElementById("personaBuscar").value;

    let filtros = {
        PersonaID: parseInt(personaIdBuscar) || 0
    }
    const res = await authFetch('Estadisticas/FiltrarPersona', {
        method: 'POST',
        body: JSON.stringify(filtros)
    })
    const data = await res.json();
    MostrarActividadesPersona(data);
}


function MostrarActividadesPersona(data) {
  $("#todasLasActividades").empty();

  if (data.length === 0) {
    $("#todasLasActividades").append(
      "<tr><td colspan='5' class='text-center text-muted'>No hay actividades para mostrar</td></tr>"
    );
  }

  $.each(data, function (index, item) {
    let duracion = "0min";
    if (item.duracionMinutos) {
        const [h, m] = item.duracionMinutos.split(':');
        duracion = h && h !== "00" ? `${parseInt(h)}h ${parseInt(m)}min` : `${parseInt(m)}min`;
    }

    var botonesAcciones =
      "<td class='text-end'>" +
      "<button class='btn btn-primary btn-editar me-5' style='background: none; border: none; color: #0d6efd; outline: none; font-size: 18px;' " +
      "onclick=\"MostrarModalEditar('" +
      item.actividadID + "','" +
      item.personaID + "','" +
      item.tipoActividadID + "','" +
      item.fecha + "','" +
      item.duracionMinutos + "','" +
      item.observaciones +
      "')\">" +
      "<i class='bi bi-pencil-square'></i>" +
      "</button>" +

      "<button class='btn btn-danger btn-eliminar me-5' style='background: none; border: none; color:#dc3545; font-size: 18px;' " +
      "onclick='EliminarActividad(" + item.actividadID + ")'>" +
      "<i class='bx bx-trash'></i>" +
      "</button>" +
      "</td>";

    $("#todasLasActividades").append(
      "<tr>" +
      //"<td>" + item.persona.nombre + "</td>" +
      "<td>" + item.tipoActividad.nombre + "</td>" +
      "<td>" + item.fechaString + "</td>" +
      "<td>" + duracion + "</td>" +
      // "<td>" + item.observaciones + "</td>" +
      botonesAcciones +
      "</tr>"
    );
  });
}

ObtenerActividadesPorPersona();