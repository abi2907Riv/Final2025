////////////////////////////////////////////
////FUNCION PARA OBTENER LAS ACTIVIDADES/////
////////////////////////////////////////////
async function ObtenerActividad() {
  const res = await authFetch("Actividades", {
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => MostrarActividad(data), LimpiarFormulario())
    .catch((error) => console.log("No se puede acceder al servicio", error));
}


////////////////////////////////////////////
////FUNCION PARA MOSTRAR LAS ACTIVIDADES/////
////////////////////////////////////////////
function MostrarActividad(data) {
  if (window.innerWidth <= 880) {
    MostrarActividadMobile(data);
  } else {
    MostrarActividadDesktop(data);
  }
}
function MostrarActividadDesktop(data) {
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

function MostrarActividadMobile(data) {
  $("#actividadesMobileContainer").empty();

  if (data.length === 0) {
    $("#actividadesMobileContainer").append(
      "<div class='text-center text-muted'>No hay actividades para mostrar</div>"
    );
    return;
  }

  $.each(data, function(index, item) {

    let duracion = "0min";
    if (item.duracionMinutos) {
      const [h, m] = item.duracionMinutos.split(':');
      duracion = h && h !== "00" ? `${parseInt(h)}h ${parseInt(m)}min` : `${parseInt(m)}min`;
    }

    const editarHTML = `
      <button class='btn' style='background:none; border:none; color:#007bff; font-size:18px;' 
        onclick="MostrarModalEditar('${item.actividadID}', '${item.personaID}', '${item.tipoActividadID}', '${item.fecha}', '${item.duracionMinutos}', '${item.observaciones}')">
        <i class='bi bi-pencil-square'></i>
      </button>`;

    const eliminarHTML = `
      <button class='btn' style='background:none; border:none; color:#dc3545; font-size:18px;' 
        onclick="EliminarActividad(${item.actividadID})">
        <i class='bx bx-trash'></i>
      </button>`;

    $("#actividadesMobileContainer").append(`
      <div class='actividad-card' style='
        border:1px solid #ddd; border-radius:10px;
        padding:14px; margin-bottom:12px; background:#fff;
        box-shadow:0 2px 4px rgba(0,0,0,0.05);'>

        <div class='d-flex justify-content-between align-items-center mb-1'>
          <h6 class='fw-bold mb-0' style='color:#333;'>${item.tipoActividad.nombre}</h6>
          <div class='d-flex align-items-center gap-2'>
            ${editarHTML}
            ${eliminarHTML}
          </div>
        </div>

        <div class='text-muted' style='font-size:14px;'>
          <div>Fecha: ${item.fechaString}</div>
          <div>Duración: ${duracion}</div>
        </div>

      </div>
    `);
  });
}


// ////////////////////////////////////////////
// ////FUNCION PARA VALIDAR FORMULARIO/////
// ////////////////////////////////////////////
function ValidarFormulario() {
  //const selectPersona = document.getElementById("PersonaId");
  //const errorSelectPersona = document.getElementById("errorPersonaId");
  //const persona = selectPersona.value  

  const selectTipoActividad = document.getElementById("TipoActividadId");
  const errorSelectTipoActividad = document.getElementById("errorTipoActividadId");
  const tipoActividad = selectTipoActividad.value

  const inputFecha = document.getElementById("Fecha");
  const errorFecha = document.getElementById("errorFecha");
  const fecha = inputFecha.value

  const inputDuracionMinutos = document.getElementById("DuracionMinutos");
  const errorDuracionMinutos = document.getElementById("ErrorDuracionMinutos");
  const duracionMinutos = inputDuracionMinutos.value

  const inputObservacion = document.getElementById("Observaciones");
  const errorObservacion = document.getElementById("ErrorObservaciones");
  const observacion = inputObservacion.value
  

  //Limpiar errores anteriores
  //errorSelectPersona.textContent = "";
  //selectPersona.classList.remove("is-invalid", "is-valid");
  errorSelectTipoActividad.textContent = "";
  selectTipoActividad.classList.remove("is-invalid", "is-valid");
  errorFecha.textContent = "";
  inputFecha.classList.remove("is-invalid", "is-valid");
  errorDuracionMinutos.textContent = "";
  inputDuracionMinutos.classList.remove("is-invalid", "is-valid");
  errorObservacion.textContent = "";
  inputObservacion.classList.remove("is-invalid", "is-valid");

  let valido = true
  //Validar campo nombre
  //if (persona === "0") {
    //selectPersona.classList.add("is-invalid");
   // errorSelectPersona.textContent = "Seleccione una persona";
   // valido = false
 // }
  if (tipoActividad === "0"){
    selectTipoActividad.classList.add("is-invalid");
    errorSelectTipoActividad.textContent = "Seleccione una persona";
    valido = false
  }
  if (!fecha){
      inputFecha.classList.add("is-invalid");
      errorFecha.textContent = "Seleccione una fecha";
      valido = false;
  } else {
      const fechaIngresada = new Date(fecha);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0); // Ignorar horas
      if (fechaIngresada > hoy) {
          inputFecha.classList.add("is-invalid");
          errorFecha.textContent = "La fecha no puede ser futura";
          valido = false;
      }
  }
  if (!duracionMinutos){
    inputDuracionMinutos.classList.add("is-invalid");
    errorDuracionMinutos.textContent = "Campo requerido";
    valido = false
  }
  if (!observacion){
    inputObservacion.classList.add("is-invalid");
    errorObservacion.textContent = "Campo requerido";
    valido = false
  }
  return valido;
}
////////////////////////////////////////////
//FUNCION PARA BUSCAR LA CATEGORIA POR SU ID//
//////VACIO: CREAR -- CONTRARIO: EIDTAR//////
////////////////////////////////////////////
function BuscarActividadId() {
  let idActividad = parseInt(document.getElementById("ActividadId").value);
  //let idPersona = parseInt(document.getElementById("PersonaId").value);
  let idTipoActividad = parseInt(document.getElementById("TipoActividadId").value)
  let fecha = document.getElementById("Fecha").value
  let duracionMinutos = parseInt(document.getElementById("DuracionMinutos").value)
  let observaciones = document.getElementById("Observaciones").value.trim();
  if (!idActividad || idActividad === 0) {
    CrearActividad();
  } else {
    EditarActividad(idActividad, idTipoActividad, fecha, duracionMinutos, observaciones);
  }
}

////////////////////////////////////////////
////FUNCION PARA CREAR UNA ACTIVIDAD/////
////////////////////////////////////////////
async function CrearActividad() {
  if (!ValidarFormulario()) {
    return;}

  const duracionInput = document.getElementById("DuracionMinutos").value;
  const duracionTimeSpan = duracionInput + ":00"; 

  let actividad = {
    // personaID: parseInt(document.getElementById("PersonaId").value),
    tipoActividadID : parseInt(document.getElementById("TipoActividadId").value),
    fecha: document.getElementById("Fecha").value,
    duracionMinutos: duracionTimeSpan,
    observaciones: document.getElementById("Observaciones").value.trim(),
  };
  console.log("Objeto actividad a enviar:", actividad);
  const res = await authFetch("Actividades", {
    method: "POST",
    body: JSON.stringify(actividad),
  })
    .then((response) => response.json())
    .then((data) => {
    //   if (data.mensaje) {
    //     ValidarExistenciaTipoActividad(data.mensaje);
    //   } else {
        Swal.fire({
          toast: true,
          position: "bottom-start",
          icon: "success",
          text: "Actividad Creada",
          showConfirmButton: false,
          timer: 3000,
          timerProgressBar: true,
        });
        document
          .querySelector('[data-bs-target="#collapseActividades"]')
          .click();
        ObtenerActividad();
        LimpiarFormulario()
      //}
    })

    .catch((error) =>
      console.log("Hubo un error al crear la categoria", error)
    );
}


// ////////////////////////////////////////////
// ////FUNCION PARA LIMPIAR FORMULARIO/////
// ////////////////////////////////////////////
function LimpiarFormulario() {
  document.getElementById("ActividadId").value = "";
//   const selectPersonaId = document.getElementById("PersonaId");
//   selectPersonaId.value = "";
  const selectTipoActividadId = document.getElementById("TipoActividadId");
  selectTipoActividadId.value = ""
  const inputFecha = document.getElementById("Fecha");
  inputFecha.value = ""
  const inputDuracionMinutos = document.getElementById("DuracionMinutos");
  inputDuracionMinutos.value = ""
  const inputObservaciones = document.getElementById("Observaciones");
  inputObservaciones.value = ""

  //Limpiar las validaciones
  //selectPersonaId.classList.remove("is-invalid", "is-valid");
  selectTipoActividadId.classList.remove("is-invalid", "is-valid");
  inputFecha.classList.remove("is-invalid", "is-valid");
  inputDuracionMinutos.classList.remove("is-invalid", "is-valid");
  inputObservaciones.classList.remove("is-invalid", "is-valid");

  //Limpiar el mensaje de error
  //const selectPersonaIdError = document.getElementById("errorPersonaId");
  //selectPersonaIdError.textContent = "";
  const selectTipoActividadIdError = document.getElementById("errorTipoActividadId");
  selectTipoActividadIdError.textContent = "";
  const inputFechaError = document.getElementById("errorFecha");
  inputFechaError.textContent = "";
  const inputDuracionMinutosError = document.getElementById("ErrorDuracionMinutos");
  inputDuracionMinutosError.textContent = "";
  const inputObservacionesError = document.getElementById("ErrorObservaciones");
  inputObservacionesError.textContent = "";
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR MODAL EDICION/////
////////////////////////////////////////////
async function MostrarModalEditar(actividadID) {
  const res = await authFetch(`Actividades/${actividadID}`);
  const actividades = await res.json();

  document.getElementById("ActividadId").value = actividades.actividadID;
  //document.getElementById("PersonaId").value = actividades.personaID;
  document.getElementById("TipoActividadId").value = actividades.tipoActividadID;

  const fecha = new Date(actividades.fecha);
  const fechaFormateada = fecha.toISOString().split('T')[0];

  document.getElementById("Fecha").value = fechaFormateada;
  document.getElementById("DuracionMinutos").value = actividades.duracionMinutos.substring(0, 5);;
  document.getElementById("Observaciones").value = actividades.observaciones;


  document.querySelector('[data-bs-target="#collapseActividades"]').click();
}


////////////////////////////////////////////
//////FUNCION PARA EDITAR LA CATEGORIA//////
////////////////////////////////////////////
async function EditarActividad() {
  if (!ValidarFormulario()) {
    return;
  }

  const idActividad = parseInt(document.getElementById("ActividadId").value);
  const idTipoActividad = parseInt(document.getElementById("TipoActividadId").value);
  const fecha = document.getElementById("Fecha").value;
  const duracionInput = document.getElementById("DuracionMinutos").value; // HH:MM
  const observaciones = document.getElementById("Observaciones").value.trim();

  const duracion = duracionInput + ":00";

  const actividad = {
    actividadID: idActividad,  
    tipoActividadID: idTipoActividad,
    fecha: fecha,
    duracionMinutos: duracion,
    observaciones: observaciones
  };

  console.log(actividad)

  try {
    const res = await authFetch(`Actividades/${idActividad}`, {
      method: "PUT",
      body: JSON.stringify(actividad),
    });

    if (res.ok) {
      Swal.fire({
        toast: true,
        position: "bottom-start",
        icon: "success",
        text: "Actividad editada correctamente",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
      });

      document.querySelector('[data-bs-target="#collapseActividades"]').click();
      ObtenerActividad();
      LimpiarFormulario();
    } else {
      if (res?.mensaje) {
        ValidarExistenciaTipoActividad(res.mensaje);
      } else {
        console.error("Error desconocido al editar tipo de actividad", res);
      }
    }

  } catch (error) {
    console.log("No se pudo editar la categoría", error);
  }
}




////////////////////////////////////////////
////FUNCION PARA ELIMAR TIPO ACTIVIDAD/////
////////////////////////////////////////////
function EliminarActividad(actividadID) {
  Swal.fire({
    title: "¿Desea eliminar esta Actividad?", // sin <strong>
    html: `
    <div style="text-align: center; font-size: 0.9rem; color: #6b7280; font-weight: 400;">
      <p>Esta actividad será eliminado de forma definitiva.</p>
      <p>Esta acción no se puede deshacer.</p>
    </div>
  `,
    showCancelButton: true,
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar",
    focusCancel: true,
    customClass: {
      popup: "swal2-border-radius",
      title: "swal2-title-small",
      confirmButton: "swal2-btn-eliminar",
      cancelButton: "swal2-btn-cancelar",
    },
    background: "#fff",
    color: "#22223b",
    buttonsStyling: false,
  }).then((result) => {
    if (result.isConfirmed) {
      EliminarSiActividad(actividadID);
    } else if (result.dismiss === Swal.DismissReason.cancel) {
      Swal.fire({
        title: "Acción Cancelada",
        text: "Permanece registrado.",
        toast: true,
        position: "bottom-end",
        showConfirmButton: false,
        timer: 2200,
        timerProgressBar: true,
        background: "#fef8f4",
        color: "#5f4339",
        icon: "info",
        iconColor: "#ff914d",
        customClass: {
          popup: "swal2-toast-status",
          title: "swal2-toast-title",
          content: "swal2-toast-content",
        },
      });
    }
  });
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
// FUNCION PARA ELIMINAR SI TIPO ACTIVIDAD///////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////
async function EliminarSiActividad(actividadID) {
  try {
    const res = await authFetch(`Actividades/${actividadID}`, {
      method: "DELETE",
    });

    if (!res.ok) throw new Error("No se pudo eliminar la actividad");

    Swal.fire({
      title: "¡Actividad Eliminada!",
      toast: true,
      position: "bottom-end",
      showConfirmButton: false,
      timer: 2200,
      timerProgressBar: true,
      background: "#f4fff7",
      color: "#1c3d26",
      icon: "success",
      iconColor: "#28a746d8",
      customClass: {
        popup: "swal2-toast-small",
        title: "swal2-toast-small-title",
        icon: "swal2-toast-small-icon",
      },
    });

    ObtenerActividad();
  } catch (error) {}
}
ObtenerActividad()







// $(document).ready(function () {
//   ObtenerTickets();
//   const filtrosBuscar = $(
//     "#CategoriaIDBuscar, #fechaDesde, #fechaHasta, #estadoBuscar, #prioridadBuscar"
//   );
//   filtrosBuscar.on("change", function () {
//     ObtenerTickets();
//   });
// });



// async function ObtenerActividad() {
//   let tipoActividadID = parseInt(document.getElementById("filtroActividad").value);
//   let fechaActividad = document.getElementById("filtroFecha").value;
//   let duracionMinutos = document.getElementById("filtroDuracion").value;


//   let filtro = {
//     tipoActividadID: tipoActividadID ? parseInt(tipoActividadID) : null,
//     fechaActividad: fechaActividad ? new Date(fechaActividad) : null,
//     duracionMinutos: duracionMinutos ? parseInt(duracionMinutos) : null
    
//   };
//   const res = await authFetch("Actividades/Filtrar", {
//     method: "POST",
//     body: JSON.stringify(filtro),
//   })
//     .then((response) => response.json())
//     .then((data) => MostrarActividad(data), LimpiarFormulario())
//     .catch((error) => console.log("No se puede acceder al servicio", error));
// }