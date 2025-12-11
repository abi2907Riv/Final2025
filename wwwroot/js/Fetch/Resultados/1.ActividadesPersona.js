function Filtros() {
  const filtros = document.getElementById("filtrosContainer");
  filtros.style.display = filtros.style.display === "none" ? "block" : "none";
}


//////////////////////////////////////////////////
//FUNCION PARA OBTENER LOS TIPOS DE ACTIVIDAD EN EL DROP//
/////////////////////////////////////////////////
async function ObtenerTipoActividadDrop() {
    const res = await authFetch('TiposActividades', {
        method: 'GET'
    })
    .then(response => response.json())
    .then(data => {
        MostrarTipoActividadDrop(data)
    })
    .catch(error => console.log('No se puede acceder al servicio', error));
}
function MostrarTipoActividadDrop(data) {
    let bodySelectFiltro = document.getElementById("filtroActividad");
    bodySelectFiltro.innerHTML = "";
    bodySelectFiltro.innerHTML = "<option value='0'>[Todas]</option>";
    const activos = data.filter(item => item.eliminado == false);
    activos.forEach(element => {
        let optFiltro = document.createElement("option");
        optFiltro.value = element.tipoActividadID;
        optFiltro.innerHTML = element.nombre;
        bodySelectFiltro.appendChild(optFiltro);
   })
}

ObtenerTipoActividadDrop()



$(document).ready(function () {
  ObtenerInformeActividadXPersona();
  const filtrosBuscar = $(
    "#personaBuscar, #filtroActividad, #filtroFechaDesde, #filtroFechaHasta, #filtroFecha, #filtroDuracion, #filtroCalorias"
  );
  filtrosBuscar.on("change keyup", function () {
    ObtenerInformeActividadXPersona();
  });
});

async function ObtenerInformeActividadXPersona() {
  let personaIdBuscar = document.getElementById("personaBuscar").value;
  let tipoIdBuscar = document.getElementById("filtroActividad").value;
  let fechaDesde = document.getElementById("filtroFechaDesde").value;
  let fechaHasta = document.getElementById("filtroFechaHasta").value;
  //let fechaActividad = document.getElementById("filtroFecha").value;
  //let duracionMinutos = document.getElementById("filtroDuracion").value;
  //let duracionTimeSpan = null;
  //let calorias = document.getElementById("filtroCalorias").value;

  // if (duracionMinutos) {
  //       duracionTimeSpan = duracionMinutos + ":00"; 
  //   }
    let filtros = {
        PersonaID: parseInt(personaIdBuscar) || 0,
        TipoActividadID: parseInt(tipoIdBuscar) || 0,
        FechaDesde: fechaDesde || null,
        FechaHasta: fechaHasta || null,
        //FechaActividad: fechaActividad || null,
       // DuracionMinutos: duracionTimeSpan,
        //CaloriasTotales: calorias !== "" ? parseInt(calorias) : null

  };
  const res = await authFetch("Actividades/InformeActividadXPersona", {
    method: "POST",
    body: JSON.stringify(filtros)
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
                    ${tipo.nombreTipo} — ${tipo.caloriasPorMinuto} kcal/min
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
                    <td>${det.caloriasQuemadas} (kcal)</td>
                `;
        tbody.appendChild(filaDetalle);
      });
    });
  });
}

ObtenerInformeActividadXPersona();