////////////////////////////////////////////
////FUNCION PARA OBTENER LAS CATEGORIAS/////
////////////////////////////////////////////
async function ObtenerTipoActividad() {
  const res = await authFetch("TipoActividad", {
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => MostrarTipoActividad(data), LimpiarFormulario())
    .catch((error) => console.log("No se puede acceder al servicio", error));
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR LOS TIPOS DE ACTIVIDAD/////
////////////////////////////////////////////

function MostrarTipoActividad(data) {
  $("#todosLosTiposActiidades").empty();
  const rol = getRol();

  if (data.length === 0) {
    $("#todosLosTiposActiidades").append(
      "<tr><td colspan='5' class='text-center text-muted'>No hay tipos de actividad para mostrar</td></tr>"
    );
  }

  $.each(data, function (index, item) {
    var botonesAcciones =
      "<td class='text-end'>" +
      "<button class='btn btn-primary btn-editar me-5' style='background: none; border: none; color: #0d6efd; outline: none; font-size: 18px;' " +
      "onclick='MostrarModalEditar(" +
      item.tipoActividadID +
      ', "' +
      item.nombre +
      '" ,"' +
      item.caloriasPorMinuto +
      "\")'>" +
      "<i class='bi bi-pencil-square'></i>" +
      "</button>" +
      "<button class='btn btn-danger btn-eliminar me-5' style='background: none; border: none; color:#dc3545; font-size: 18px;' " +
      "onclick='EliminarTipoActividad(" +
      item.tipoActividadID +
      ")'>" +
      "<i class='bx bx-trash'></i>" +
      "</button>";

    botonesAcciones += "</td>";
    $("#todosLosTiposActiidades").append(
      "<tr>" +
        "<td>" +
        item.nombre +
        "</td>" +
        "<td>" +
        item.caloriasPorMinuto +
        "</td>" +
        botonesAcciones +
        "</tr>"
    );
  });
}

// ////////////////////////////////////////////
// ////FUNCION PARA VALIDAR FORMULARIO/////
// ////////////////////////////////////////////
function ValidarFormulario() {
  const inpuNombre = document.getElementById("nombreTipoActividad");
  const errorNombre = document.getElementById("errorTipoActividad");
  const nombre = inpuNombre.value.trim();
  const inputCalorias = document.getElementById("CaloriasPorMinutos");
  const errorCalorias = document.getElementById("errorCaloriasPorMinutos");
  const calorias = inputCalorias.value.trim();

  //Limpiar errores anteriores
  errorNombre.textContent = "";
  inpuNombre.classList.remove("is-invalid", "is-valid");
  errorCalorias.textContent = "";
  inputCalorias.classList.remove("is-invalid", "is-valid");

  let valido = true
  //Validar campo nombre
  if (!nombre) {
    inpuNombre.classList.add("is-invalid");
    errorNombre.textContent = "Campo requerido";
    valido = false
  }
  if (!calorias){
    inputCalorias.classList.add("is-invalid");
    errorCalorias.textContent = "Campo requerido";
    valido = false
  }
  return valido;
}

////////////////////////////////////////////
//FUNCION PARA BUSCAR LA CATEGORIA POR SU ID//
//////VACIO: CREAR -- CONTRARIO: EIDTAR//////
////////////////////////////////////////////
function BuscarTipoActividadId() {
    if (!ValidarFormulario()) {
      return;
    }
  let id = parseInt(document.getElementById("TipoActividadId").value);
  let nombre = document.getElementById("nombreTipoActividad").value.trim();
  let categoriaPorMinuto = document
    .getElementById("CaloriasPorMinutos")
    .value.trim();
  if (!id || id === 0) {
    CrearTipoActividad();
  } else {
    EditarTipoActividad(id, nombre, categoriaPorMinuto);
  }
}



// ////////////////////////////////////////////
// ////FUNCION PARA LIMPIAR FORMULARIO/////
// ////////////////////////////////////////////
function LimpiarFormulario() {
  document.getElementById("TipoActividadId").value = "";
  const inputNombre = document.getElementById("nombreTipoActividad");
  inputNombre.value = "";
  const inputCalorias = document.getElementById("CaloriasPorMinutos");
  inputCalorias.value = ""

  //Limpiar las validaciones
  inputNombre.classList.remove("is-invalid", "is-valid");
  inputCalorias.classList.remove("is-invalid", "is-valid");

  //Limpiar el mensaje de error
  const inputErrorNombre = document.getElementById("errorTipoActividad");
  inputErrorNombre.textContent = "";
  const inputErrorCalorias = document.getElementById("errorCaloriasPorMinutos");
  inputErrorCalorias.textContent = "";
}

// ////////////////////////////////////////////
// ////FUNCION PARA VALIDAR EXISTENCIA/////
// ////////////////////////////////////////////
function ValidarExistenciaTipoActividad(mensaje) {
  const inputNombreTipo = document.getElementById("nombreTipoActividad");
  const errorNombreTipo = document.getElementById("errorTipoActividad");

  errorNombreTipo.textContent = mensaje;
  inputNombreTipo.classList.add("is-invalid");
}

////////////////////////////////////////////
////FUNCION PARA CREAR UNA CATEGORIAS/////
////////////////////////////////////////////
async function CrearTipoActividad() {
  if (!ValidarFormulario()) {
    return;}
  let tipoActividad = {
    nombre: document.getElementById("nombreTipoActividad").value.trim(),
    caloriasPorMinuto: parseFloat(
      document.getElementById("CaloriasPorMinutos").value
    ),
  };
  const res = await authFetch("TipoActividad", {
    method: "POST",
    body: JSON.stringify(tipoActividad),
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.mensaje) {
        ValidarExistenciaTipoActividad(data.mensaje);
      } else {
        Swal.fire({
          toast: true,
          position: "bottom-start",
          icon: "success",
          text: "Tipo Actividad Creada",
          showConfirmButton: false,
          timer: 3000,
          timerProgressBar: true,
        });
        document
          .querySelector('[data-bs-target="#collapseTipoActividad"]')
          .click();
        ObtenerTipoActividad();
        LimpiarFormulario()
      }
    })

    .catch((error) =>
      console.log("Hubo un error al crear la categoria", error)
    );
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR MODAL EDICION/////
////////////////////////////////////////////
async function MostrarModalEditar(tipoActividadID) {
  const res = await authFetch(`TipoActividad/${tipoActividadID}`);
  const tipoActividad = await res.json();

  document.getElementById("TipoActividadId").value =
    tipoActividad.tipoActividadID;
  document.getElementById("nombreTipoActividad").value = tipoActividad.nombre;
  document.getElementById("CaloriasPorMinutos").value =
    tipoActividad.caloriasPorMinuto;

  document.querySelector('[data-bs-target="#collapseTipoActividad"]').click();
}

////////////////////////////////////////////
//////FUNCION PARA EDITAR LA CATEGORIA//////
////////////////////////////////////////////
async function EditarTipoActividad(tipoActividadID, nombre, caloriasPorMinutos) {
  if (!ValidarFormulario()) {
    return;
  }

  const tipoActividad = {
    tipoActividadID: tipoActividadID,
    nombre: nombre.trim(),
    caloriasPorMinuto: parseFloat(caloriasPorMinutos)
  };

  try {
    const res = await authFetch(`TipoActividad/${tipoActividadID}`, {
      method: "PUT",
      body: JSON.stringify(tipoActividad),
    });

    if (res.ok) {
      Swal.fire({
        toast: true,
        position: "bottom-start",
        icon: "success",
        text: "Tipo de actividad editada correctamente",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
      });

      document.querySelector('[data-bs-target="#collapseTipoActividad"]').click();
      ObtenerTipoActividad();
      LimpiarFormulario();
    } else {
      if (response?.mensaje) {
        ValidarExistenciaTipoActividad(response.mensaje);
      } else {
        console.error("Error desconocido al editar tipo de actividad", response);
      }
    }

  } catch (error) {
    console.log("No se pudo editar la categoría", error);
  }
}




////////////////////////////////////////////
////FUNCION PARA ELIMAR TIPO ACTIVIDAD/////
////////////////////////////////////////////
function EliminarTipoActividad(tipoActividadID) {
  Swal.fire({
    title: "¿Desea eliminar este tipo de actividad?", // sin <strong>
    html: `
    <div style="text-align: center; font-size: 0.9rem; color: #6b7280; font-weight: 400;">
      <p>Este tipo de actividad será eliminado de forma definitiva.</p>
      <p>Esta acción no se puede deshacer.</p>
    </div>
  `,
    showCancelButton: true,
    confirmButtonText: "Sí, eliminar",
    cancelButtonText: "Cancelar",
    focusCancel: true,
    customClass: {
      popup: "swal2-border-radius",
      title: "swal2-title-small", // <--- clase para achicar título
      confirmButton: "swal2-btn-eliminar",
      cancelButton: "swal2-btn-cancelar",
    },
    background: "#fff",
    color: "#22223b",
    buttonsStyling: false,
  }).then((result) => {
    if (result.isConfirmed) {
      EliminarSiTipoActividad(tipoActividadID);
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
async function EliminarSiTipoActividad(tipoActividadID) {
  try {
    const res = await authFetch(`TipoActividad/${tipoActividadID}`, {
      method: "DELETE",
    });

    if (!res.ok) throw new Error("No se pudo eliminar el criterio");

    Swal.fire({
      title: "¡Tipo Actividad Eliminado!",
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

    ObtenerTipoActividad();
  } catch (error) {}
}

////////////////////////////////////////////
//FUNCION PARA CARGAR CATEGORIAS INICIALMENTE//
////////////////////////////////////////////
ObtenerTipoActividad();





// async function EditarTipoActividad(tipoActividadID, nombre,caloriasPorMinutos) {
//   // if (!ValidarFormulario()) {
//   //   return;
//   // }
//   let tipoActividad = {
//     tipoActividadID: tipoActividadID,
//     nombre: nombre.trim(),
//     caloriasPorMinutos: parseFloat(caloriasPorMinutos)
//   };
//   const res = await authFetch(`TipoActividad/${tipoActividadID}`, {
//     method: "PUT",
//     body: JSON.stringify(tipoActividad),
//   })
//     // .then((response) => response.json())
//     .then((response) => {
//       // if (response.mensaje) {
//       //   ValidarExistenciaCategoria(response.mensaje);
//       // } else {
//       //   LimpiarFormulario();
//         Swal.fire({
//           toast: true,
//           position: "bottom-start",
//           icon: "success",
//           text: "Tipo Actividad Editada",
//           showConfirmButton: false,
//           timer: 3000,
//           timerProgressBar: true,
//         });
//         document.querySelector('[data-bs-target="#collapseTipoActividad"]').click();
//         ObtenerTipoActividad();
//         // LimpiarFormulario();
//       // }
//     })
//     .catch((error) => console.log("No se pudo editar la categoria", error));
// }




// async function EditarTipoActividad(tipoActividadID, nombre,caloriasPorMinutos) {
//   if (!ValidarFormulario()) {
//     return;
//   }
//   let tipoActividad = {
//     tipoActividadID: tipoActividadID,
//     nombre: nombre.trim(),
//     caloriasPorMinutos: parseFloat(caloriasPorMinutos)
//   };
//   const res = await authFetch(`TipoActividad/${tipoActividadID}`, {
//     method: "PUT",
//     body: JSON.stringify(tipoActividad),
//   })
//     .then((response) => response.json())
//     .then((response) => {
//       if (response.mensaje) {
//         ValidarExistenciaCategoria(response.mensaje);
//       } else {
//         Swal.fire({
//           toast: true,
//           position: "bottom-start",
//           icon: "success",
//           text: "Tipo Actividad Editada",
//           showConfirmButton: false,
//           timer: 3000,
//           timerProgressBar: true,
//         });
//         document.querySelector('[data-bs-target="#collapseTipoActividad"]').click();
//         ObtenerTipoActividad();
//         LimpiarFormulario();
//       }
//     })
//     .catch((error) => console.log("No se pudo editar la categoria", error));
// }