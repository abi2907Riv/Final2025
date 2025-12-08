//INFORME DE ACTIVIDAD POR PERSONA CON FILTRO POR FECHA DESDE Y FECHA HASTA
/////////////////////DTO///////////////////////////
// public class ActividadDetalleDTO
// {
//     public DateTime Fecha { get; set; }
//     public int DuracionMinutos { get; set; }
//     public decimal CaloriasQuemadasTotales { get; set; } //Esto es lo que debe de calcularse
// }

// public class PersonaActividadesDTO
// {
//     public string Nombre { get; set; }
//     public string Email { get; set; }
//     public List<TipoActividadDTO> TiposActividad { get; set; }
// }
// public class TipoActividadDTO
// {
//     public string NombreTipo { get; set; }
//     public decimal CaloriasPorMinuto { get; set; }
//     public List<ActividadDetalleDTO> Actividades { get; set; }
// }
///////////////////////////JAVASCRIPT////////////////////////////////////
// async function ObtenerInformeActividadXPersona() {
//     let fechaActividad = document.getElementById("filtroFechaDesde").value;
//     //let fechaHasta = document.getElementById("filtroFechaHasta").value;

//     let filtro = {
//         fechaActividad: fechaActividad !== "" ? fechaActividad : null,
//         //FechaHasta: fechaHasta !== "" ? fechaHasta : null
//     };

//     try {
//         const res = await authFetch("Actividades/InformeActividadXPersona", {
//             method: "POST",
//             body: JSON.stringify(filtro),
//         });
//         const data = await res.json();
//         MostrarInformeActividad(data);
//     } catch (error) {
//         console.log("No se puede acceder al servicio", error);
//     }
// }

// function MostrarInformeActividad(data) {
//     const tbody = document.querySelector("#tablaActividadPorPersona tbody");
//     tbody.innerHTML = "";

//     if (!data || data.length === 0) {
//         tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay actividades registradas</td></tr>`;
//         return;
//     }

//     data.forEach(persona => {
//         const filaPersona = document.createElement("tr");
//         filaPersona.innerHTML = `
//             <td colspan="4" class="fw-bold categorias-agrupadas">
//                 ${persona.nombre} || ${persona.email}
//             </td>
//         `;
//         tbody.appendChild(filaPersona);

//         persona.tiposActividad.forEach(tipo => {
//             const filaTipo = document.createElement("tr");
//             filaTipo.innerHTML = `
//                 <td colspan="4" class="fw-bold table-success">
//                     ${tipo.nombreTipo} — ${tipo.caloriasPorMinuto} Calorias por Minuto
//                 </td>
//             `;
//             tbody.appendChild(filaTipo);

//             tipo.actividades.forEach(det => {
//                 const filaDetalle = document.createElement("tr");
//                 filaDetalle.innerHTML = `
//                     <td>${new Date(det.fecha).toLocaleDateString()}</td>
//                     <td>${det.duracionMinutos} min</td>
//                     <td>${det.caloriasQuemadasTotales}</td>
//                 `;
//                 tbody.appendChild(filaDetalle);
//             });
//         });
//     });
// }

// // Ejecutar al cargar la página
// $(document).ready(function () {
//     ObtenerInformeActividadXPersona();

//     const filtrosBuscar = $("#filtroFechaDesde, #filtroFechaHasta");
//     filtrosBuscar.on("change keyup", function () {
//         ObtenerInformeActividadXPersona();
//     });
// });
///////////////////////////////HTML//////////////////////////////////
// <!-- Contenido -->
// <div class="content-wrapper">

//   <!-- Título y botón de filtros -->


//   <!-- Contenedor de filtros -->
//         <div class="row g-3 mb-3">
//             <div class="col-md-4">
//                 <label class="form-label fw-semibold" for="filtroFechaDesde">FECHA</label>
//                 <div class="input-group">
//                     <span class="input-group-text"><i class="bx bx-search"></i></span>
//                     <input type="date" class="form-control" id="filtroFechaDesde" placeholder="Ej: 8.5">
//                 </div>
//             </div>

//             <!-- <div class="col-md-4">
//                 <label class="form-label fw-semibold" for="filtroFechaHasta">FECHA DESDE</label>
//                 <div class="input-group">
//                     <span class="input-group-text"><i class="bx bx-search"></i></span>
//                     <input type="date" class="form-control" id="filtroFechaHasta" placeholder="Ej: 8.5">
//                 </div>
//             </div> -->
//         </div>


//   <!-- Cards -->
//   <div class="container-xxl flex-grow-1 container-p-y">
//     <div class="card">

//       <h5 class="card-header d-flex align-items-center justify-content-between fw-bold">
//         <span class="d-flex align-items-center">
//           <i class="bx bx-dumbbell fs-4 me-2"></i> Actividades por Persona
//         </span>
//       </h5>

//       <!-- Tabla -->
//       <div class="table-responsive text-nowrap mt-3">
//         <table class="table" id="tablaActividadPorPersona">
//           <thead class="table-light">
//             <tr>
//               <th>Fecha</th>
//               <th>Duración</th>
//               <th>Total Calorias Quemadas</th>
//             </tr>
//           </thead>
//           <tbody></tbody>
//         </table>
//       </div>
//     </div>
//   </div>

// </div>

// <script src="../js/Fetch/InformeActividadesPorPersona.js"></script>
