// $(document).ready(function () {
//   ObtenerCantidadTicketsDesarrolladorCategoria();
//   const filtrosBuscar = $("#fechaCierreDesde, #fechaCierreHasta");
//   filtrosBuscar.on("change keyup", function () {
//     ObtenerCantidadTicketsDesarrolladorCategoria();
//   });
// });


// function Filtros() {
//   const filtros = document.getElementById("filtrosContainer");
//   filtros.style.display = filtros.style.display === "none" ? "block" : "none";
// }

async function ObtenerCantidadActividadesPersona() {
    // let fechaCierreDesde = document.getElementById("fechaCierreDesde").value;
    // let fechaCierreHasta = document.getElementById("fechaCierreHasta").value;

    // let filtros = {
    //     FechaCierreDesde: fechaCierreDesde,
    //     FechaCierreHasta: fechaCierreHasta
    // }
  const res = await authFetch(
    "Estadisticas/InformeCantidad",
    {
      method: "POST"
    }
  )
   const data = await res.json();
    MostrarCantidadActividadesPersona(data);
}


function MostrarCantidadActividadesPersona(data) {
  const tbody = document.querySelector("#tablaCantidadActividades tbody");
  tbody.innerHTML = "";
  if (data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted">No hay actividades</td></tr>`;
    return;
  }

  data.forEach((persona) => {
    const filaPersona = document.createElement("tr");
    filaPersona.innerHTML = `
            <td class='text-bold table-primary'>${persona.nombre} ${persona.email}</td>
            <td class='text-bold table-primary'>${persona.totalActividades}</td>
            <td class='text-bold table-primary'>${persona.duracionTotal}min</td>
            <td class='text-bold table-primary'>${persona.totalCalorias}kcal</td>
            <td class='text-bold table-primary'>${new Date(persona.fechaUltimaActividad).toLocaleDateString()}</td>
    `;
    tbody.appendChild(filaPersona);
  });
}


ObtenerCantidadActividadesPersona();

