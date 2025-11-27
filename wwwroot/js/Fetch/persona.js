function Filtros() {
  const filtros = document.getElementById("filtrosContainer");
  filtros.style.display = filtros.style.display === "none" ? "block" : "none";
}

////////////////////////////////////////////
////FUNCION PARA OBTENER LAS PERSONAS/////
////////////////////////////////////////////
async function ObtenerPersonas() {
  const res = await authFetch("Personas", {
    method: "GET",
  })
    .then((response) => response.json())
    .then((data) => MostrarPersonas(data), LimpiarFormulario())
    .catch((error) => console.log("No se puede acceder al servicio", error));
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR LAS PERSONAS/////
////////////////////////////////////////////
function MostrarPersonas(data) {
  if (window.innerWidth <= 880) {
    MostrarPersonasMobile(data);
  } else {
    MostrarPersonasDesktop(data);
  }
}
function MostrarPersonasDesktop(data) {
  $("#todasLasPersonas").empty();

  if (data.length === 0) {
    $("#todasLasPersonas").append(
      "<tr><td colspan='5' class='text-center text-muted'>No hay personas para mostrar</td></tr>"
    );
    return;
  }

  $.each(data, function (index, item) {
    const fechaHoy = new Date();
    const fechaNacimiento = new Date(item.fechaNacimiento);
    let edad = fechaHoy.getFullYear() - fechaNacimiento.getFullYear();
    const mes = fechaHoy.getMonth() - fechaNacimiento.getMonth();
    if (
      mes < 0 ||
      (mes === 0 && fechaHoy.getDate() < fechaNacimiento.getDate())
    ) {
      edad--;
    }

    const botonesAcciones = `
            <td class='text-end'>
                <button class='btn btn-primary btn-editar me-2' 
                        style='background: none; border: none; color: #0d6efd; outline: none; font-size: 18px;' 
                        onclick="MostrarModalEditar(${
                          item.personaID
                        }, '${item.nombre.replace(/'/g, "\\'")}', '${
      item.fechaNacimiento
    }', '${item.email}', ${item.peso})">
                    <i class='bi bi-pencil-square'></i>
                </button>
                <!--<button class='btn btn-danger btn-eliminar' 
                        style='background: none; border: none; color:#dc3545; font-size: 18px;' 
                        onclick="EliminarPersona(${item.personaID})">
                    <i class='bx bx-trash'></i>
                </button>-->
            </td>
        `;

    $("#todasLasPersonas").append(`
            <tr>
                <td>${item.nombre}</td>
                <!-- <td>${item.email}</td> -->
                <td>${edad}</td>
                <td>${item.peso}</td>
                ${botonesAcciones}
            </tr>
        `);
  });
}

function MostrarPersonasMobile(data) {
  $("#personasMobileContainer").empty();

  if (data.length === 0) {
    $("#personasMobileContainer").append(
      "<div class='text-center text-muted'>No hay personas para mostrar</div>"
    );
    return;
  }

  $.each(data, function(index, item) {
    const fechaHoy = new Date();
    const fechaNacimiento = new Date(item.fechaNacimiento);
    let edad = fechaHoy.getFullYear() - fechaNacimiento.getFullYear();
    const mes = fechaHoy.getMonth() - fechaNacimiento.getMonth();
    if (mes < 0 || (mes === 0 && fechaHoy.getDate() < fechaNacimiento.getDate())) {
      edad--;
    }

    const editarHTML = `<button class='btn' style='background:none; border:none; color:#007bff; font-size:18px;' 
      onclick="MostrarModalEditar(${item.personaID}, '${item.nombre.replace(/'/g, "\\'")}', '${item.fechaNacimiento}', '${item.email}', ${item.peso})">
      <i class='bi bi-pencil-square'></i></button>`;

    $("#personasMobileContainer").append(`
      <div class='persona-card' style='
        border:1px solid #ddd; border-radius:10px;
        padding:14px; margin-bottom:12px; background:#fff;
        box-shadow:0 2px 4px rgba(0,0,0,0.05);'>

        <div class='d-flex justify-content-between align-items-center mb-1'>
          <h6 class='fw-bold mb-0' style='color:#333; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;'>${item.nombre}</h6>
          <div class='d-flex align-items-center gap-2'>
            ${editarHTML}
          </div>
        </div>

        <div class='text-muted' style='font-size:14px;'>
          <div>Edad: ${edad} años</div>
          <div>Peso: ${item.peso} kg</div>
        </div>
      </div>
    `);
  });
}


// ////////////////////////////////////////////
// ////FUNCION PARA VALIDAR FORMULARIO/////
// ////////////////////////////////////////////
function ValidarFormulario() {
  const inpuNombre = document.getElementById("nombrePersona");
  const errorNombre = document.getElementById("errorNombrePersona");
  const nombre = inpuNombre.value.trim();

  const inputEmail = document.getElementById("EmailPersona");
  const errorEmail = document.getElementById("errorEmailPersona");
  const email = inputEmail.value.trim();

  const inputFecha = document.getElementById("FechaNacimiento");
  const errorrFecha = document.getElementById("errorFechaNacimiento");
  const fecha = inputFecha.value.trim();

  const inputPeso = document.getElementById("PesoPersona");
  const errorPeso = document.getElementById("ErrorPesoPersona");
  const peso = inputPeso.value

  //Limpiar errores anteriores
  errorNombre.textContent = "";
  inpuNombre.classList.remove("is-invalid", "is-valid");

  errorEmail.textContent = "";
  inputEmail.classList.remove("is-invalid", "is-valid");

  errorrFecha.textContent = "";
  inputFecha.classList.remove("is-invalid", "is-valid");

  errorPeso.textContent = ""
  inputPeso.classList.remove("is-invalid", "is-valid");

  let valido = true
  //Validar campo nombre
  if (!nombre) {
    inpuNombre.classList.add("is-invalid");
    errorNombre.textContent = "Campo requerido";
    valido = false
  }
  if (!email){
    inputEmail.classList.add("is-invalid");
    errorEmail.textContent = "Campo requerido";
    valido = false
  }
    if (!fecha){
      inputFecha.classList.add("is-invalid");
      errorrFecha.textContent = "Seleccione una fecha";
      valido = false;
  } else {
      const fechaIngresada = new Date(fecha);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0); // Ignorar horas
      if (fechaIngresada > hoy) {
          inputFecha.classList.add("is-invalid");
          errorrFecha.textContent = "La fecha no puede ser futura";
          valido = false;
      }
  }
//   if (!fecha) {
//       inputFecha.classList.add("is-invalid");
//       errorrFecha.textContent = "Seleccione una fecha";
//       valido = false;
//   } else {
//     const fechaIngresada = new Date(fecha + "T00:00:00"); 
//     const hoy = new Date();
//     hoy.setHours(0, 0, 0, 0);
//     if (fechaIngresada > hoy) {
//         inputFecha.classList.add("is-invalid");
//         errorrFecha.textContent = "La fecha no puede ser futura";
//         valido = false;
//     } else {
//         let edad = hoy.getFullYear() - fechaIngresada.getFullYear();
//         const mesActual = hoy.getMonth();
//         const mesNacimiento = fechaIngresada.getMonth();
//         if (
//             mesNacimiento > mesActual ||
//             (mesNacimiento === mesActual && fechaIngresada.getDate() > hoy.getDate())
//         ) {
//             edad--;
//         }
//         if (edad < 18) {
//             inputFecha.classList.add("is-invalid");
//             errorrFecha.textContent = "Debe tener al menos 18 años";
//             valido = false;
//         }
//         if (edad > 100) {
//             inputFecha.classList.add("is-invalid");
//             errorrFecha.textContent = "La edad no puede ser mayor a 100 años";
//             valido = false;
//         }
//     }
// }
  // if (!fecha){
  //   inputFecha.classList.add("is-invalid");
  //   errorrFecha.textContent = "Campo requerido";
  //   valido = false
  // }
  if (!peso){
    inputPeso.classList.add("is-invalid");
    errorPeso.textContent = "Campo requerido";
    valido = false
  }
  return valido;
}

////////////////////////////////////////////
//FUNCION PARA BUSCAR LA PERSONA POR SU ID//
//////VACIO: CREAR -- CONTRARIO: EIDTAR/////
////////////////////////////////////////////
function BuscarPersonaId() {
  // if (!ValidarFormulario()) {
  //   return;
  // }
  let id = parseInt(document.getElementById("PersonaId").value);
  let nombre = document.getElementById("nombrePersona").value.trim();
  let email = document.getElementById("EmailPersona").value.trim();
  let fechaNacimiento = document.getElementById("FechaNacimiento").value.trim();
  let peso = document.getElementById("PesoPersona").value.trim();
  if (!id || id === 0) {
    CrearPersona();
  } else {
    EditarPersona(id, nombre, fechaNacimiento, email, peso);
  }
}

// ////////////////////////////////////////////
// ////FUNCION PARA LIMPIAR FORMULARIO/////
// ////////////////////////////////////////////
function LimpiarFormulario() {
  document.getElementById("PersonaId").value = "";
  const inputNombre = document.getElementById("nombrePersona");
  inputNombre.value = "";

  const inputEmail = document.getElementById("EmailPersona");
  inputEmail.value = ""

  const inputFecha = document.getElementById("FechaNacimiento");
  inputFecha.value = "";

  const inputPeso = document.getElementById("PesoPersona");
  inputPeso.value = "";

  //Limpiar las validaciones
  inputNombre.classList.remove("is-invalid", "is-valid");
  inputEmail.classList.remove("is-invalid", "is-valid");
  inputFecha.classList.remove("is-invalid", "is-valid");
  inputPeso.classList.remove("is-invalid", "is-valid");

  //Limpiar el mensaje de error
  const inputErrorNombre = document.getElementById("errorNombrePersona");
  inputErrorNombre.textContent = "";
  const inputErrorEmail = document.getElementById("errorEmailPersona");
  inputErrorEmail.textContent = "";
  const inputErrorFecha = document.getElementById("errorFechaNacimiento");
  inputErrorFecha.textContent = "";
  const inputErrorPeso = document.getElementById("ErrorPesoPersona");
  inputErrorPeso.textContent = "";
}

// ////////////////////////////////////////////
// ////FUNCION PARA VALIDAR EXISTENCIA/////
// ////////////////////////////////////////////
function ValidarExistenciaPersona(mensaje) {
  const inputEmail = document.getElementById("EmailPersona");
  const errorEmail = document.getElementById("errorEmailPersona");

  errorEmail.textContent = mensaje;
  inputEmail.classList.add("is-invalid");
}
////////////////////////////////////////////
////FUNCION PARA CREAR UNA PERSONA/////
////////////////////////////////////////////
async function CrearPersona() {
    if (!ValidarFormulario()) {
      return;}

  let email = document.getElementById("EmailPersona").value.trim();

  let persona = {
    nombre: document.getElementById("nombrePersona").value.trim(),
    fechaNacimiento: document.getElementById("FechaNacimiento").value,
    peso: parseFloat(document.getElementById("PesoPersona").value),
  };
  const res = await authFetch("Personas", {
    method: "POST",
    body: JSON.stringify({ email, persona }),
  })
    .then((response) => response.json())
    .then((data) => {
        if (data.mensaje) {
          ValidarExistenciaPersona(data.mensaje);
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
      document.querySelector('[data-bs-target="#collapsePersonas"]').click();
      ObtenerPersonas();
      LimpiarFormulario();
     }
    })

    .catch((error) =>
      console.log("Hubo un error al crear la categoria", error)
    );
}

////////////////////////////////////////////
////FUNCION PARA MOSTRAR MODAL EDICION/////
////////////////////////////////////////////
async function MostrarModalEditar(personaID) {
  const res = await authFetch(`Personas/${personaID}`);
  const personas = await res.json();

  document.getElementById("PersonaId").value = personas.personaID;
  document.getElementById("nombrePersona").value = personas.nombre;
  document.getElementById("EmailPersona").value = personas.email;

  const fecha = new Date(personas.fechaNacimiento);
  const fechaFormateada = fecha.toISOString().split('T')[0];
  
  document.getElementById("FechaNacimiento").value = fechaFormateada;
  document.getElementById("PesoPersona").value = personas.peso;

  document.getElementById("emailInput").style.display = "none";

  document.querySelector('[data-bs-target="#collapsePersonas"]').click();
}

////////////////////////////////////////////
//////FUNCION PARA EDITAR LA CATEGORIA//////
////////////////////////////////////////////
async function EditarPersona(personaID) {
  if (!ValidarFormulario()) {
    return;
  }

    let persona = {
        personaID: personaID,
        nombre: document.getElementById("nombrePersona").value.trim(),
        fechaNacimiento: document.getElementById("FechaNacimiento").value,
        peso: parseInt(document.getElementById("PesoPersona").value),
    };
  console.log("Datos enviados:", JSON.stringify(persona));

  try {
    const res = await authFetch(`Personas/${personaID}`, {
      method: "PUT",
      body: JSON.stringify(persona),
    });

    if (res.ok) {
      Swal.fire({
        toast: true,
        position: "bottom-start",
        icon: "success",
        text: "Persona editada correctamente",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
      });

      document.querySelector('[data-bs-target="#collapsePersonas"]').click();
      ObtenerPersonas();
      LimpiarFormulario();
    } else {
      if (res?.mensaje) {
        ValidarExistenciaPersona(res.mensaje);
      } else {
        console.error("Error desconocido al editar tipo de actividad", res);
      }
    }

  } catch (error) {
    console.log("No se pudo editar la categoría", error);
  }
}

ObtenerPersonas();




//FILTROS 
// $(document).ready(function () {
//   ObtenerPersonas();
//   const filtrosBuscar = $(
//     "#filtroNombre, #filtroFecha, #filtroPeso, #filtroEdad, #filtroFechaDesde, #filtroFechaHasta"
//   );
//   filtrosBuscar.on("change keyup", function () {
//     ObtenerPersonas();
//   });
// });

// async function ObtenerPersonas() {
//   let nombre = document.getElementById("filtroNombre").value;
//   let fecha = document.getElementById("filtroFecha").value;
//   let peso = document.getElementById("filtroPeso").value;
//   let edad = document.getElementById("filtroEdad").value;
//   let fechaDesde = document.getElementById("filtroFechaDesde").value;
//   let fechaHasta = document.getElementById("filtroFechaHasta").value;

//   let filtro = {
//     nombre: nombre !== "" ? nombre : null,
//     fechaNacimiento: fecha !== "" ? fecha : null,
//     peso: peso !== "" ? parseInt(peso) : null,
//     edad: edad !== ""  ? parseInt(edad) : null,
//     FechaNacimientoDesde: fechaDesde !== " " ? fechaDesde : null,
//     FechaNacimientoHasta: fechaHasta !== " " ? fechaHasta : null,
//   };

//   const res = await authFetch("Personas/Filtrar", {
//     method: "POST",
//     body: JSON.stringify(filtro),
//   })
//     .then((response) => response.json())
//     .then((data) => {
//       MostrarPersonas(data);
//       LimpiarFormulario();
//     })
//     .catch((error) => console.log("No se puede acceder al servicio", error));
// }
