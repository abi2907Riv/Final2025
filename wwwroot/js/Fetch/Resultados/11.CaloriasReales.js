// function Filtros() {
//     const filtros = document.getElementById("filtrosContainer");
//     filtros.style.display = filtros.style.display === "none" ? "block" : "none";
// }
// $(document).ready(function () {
//     ObtenerEstadisticasActividades();
//     $(" #filtroFechaDesde, #filtroFechaHasta")
//         .on("change keyup", function () {
//             ObtenerEstadisticasActividades();
//         });
// });
async function ObtenerEstadisticasActividades() {
    // let fechaDesde = document.getElementById("filtroFechaDesde").value || null;
    // let fechaHasta = document.getElementById("filtroFechaHasta").value || null;

    // let filtros = {
    //     FechaDesdeFiltro: fechaDesde,
    //     FechaHastaFiltro: fechaHasta
    // };

    const res = await authFetch("Estadisticas/CaloriasQuemadasReales", {
        method: "POST",
        //body: JSON.stringify(filtros)
    });

    const data = await res.json();
    MostrarEstadisticasActividades(data);
}
function MostrarEstadisticasActividades(data) {
    const tbody = document.querySelector("#tablaActividadPorPersona tbody");
    tbody.innerHTML = "";

    if (!data || data.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="4" class="text-center text-muted">No hay información para mostrar</td></tr>
        `;
        return;
    }

    data.forEach(persona => {
        // Fila de la persona
        const filaPersona = document.createElement("tr");
        filaPersona.classList.add("table-primary");
        filaPersona.innerHTML = `
            <td colspan="4" class="fw-bold">
                ${persona.nombre} — <span class="text-success">${persona.peso}kg</span>
            </td>
        `;
        tbody.appendChild(filaPersona);

        persona.tiposActividades.forEach(tipo => {
            tipo.actividades.forEach(act => {
                const fila = document.createElement("tr");
                fila.innerHTML = `
                    <td>${new Date(act.fecha).toLocaleDateString()}</td>
                    <td>${act.duracionMinutos}min</td>
                    <td>${(act.caloriasReales / (persona.peso / 70)).toFixed(2)}</td>
                    <td>${act.caloriasReales.toFixed(2)}</td>
                `;
                tbody.appendChild(fila);
            });

            const filaTipoTotal = document.createElement("tr");
            filaTipoTotal.classList.add("table-success", "fw-bold");
            filaTipoTotal.innerHTML = `
                <td><b>Total ${tipo.nombreTipo}</b></td>
                <td>${tipo.minutosTotales.toFixed(2)}</td>
                <td>-</td>
                <td>${tipo.caloriasRealesTotales.toFixed(2)}</td>
            `;
            tbody.appendChild(filaTipoTotal);
        });

        const filaPersonaTotal = document.createElement("tr");
        filaPersonaTotal.classList.add("table-warning", "fw-bold");
        filaPersonaTotal.innerHTML = `
            <td><b>Total Persona</b></td>
            <td>${persona.minutosTotales.toFixed(2)}</td>
            <td>-</td>
            <td>${persona.caloriasRealesTotales.toFixed(2)}</td>
        `;
        tbody.appendChild(filaPersonaTotal);
    });
}



ObtenerEstadisticasActividades();