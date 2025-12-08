function Filtros() {
    const filtros = document.getElementById("filtrosContainer");
    filtros.style.display = filtros.style.display === "none" ? "block" : "none";
}
$(document).ready(function () {
    ObtenerEstadisticasActividades();
    $(" #filtroFechaDesde, #filtroFechaHasta")
        .on("change keyup", function () {
            ObtenerEstadisticasActividades();
        });
});
async function ObtenerEstadisticasActividades() {
    let fechaDesde = document.getElementById("filtroFechaDesde").value || null;
    let fechaHasta = document.getElementById("filtroFechaHasta").value || null;

    let filtros = {
        FechaDesdeFiltro: fechaDesde,
        FechaHastaFiltro: fechaHasta
    };

    const res = await authFetch("Estadisticas/EstadisticasActividades", {
        method: "POST",
        body: JSON.stringify(filtros)
    });

    const data = await res.json();
    MostrarEstadisticasActividades(data);
}
function MostrarEstadisticasActividades(data) {
    const tbody = document.querySelector("#tablaActividadPorPersona tbody");
    tbody.innerHTML = "";

    if (!data || data.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="6" class="text-center text-muted">No hay información para mostrar</td></tr>
        `;
        return;
    }

    data.forEach(persona => {
        const filaPersona = document.createElement("tr");
        filaPersona.classList.add("table-primary");
        filaPersona.innerHTML = `
            <td colspan="6" class="fw-bold">
                ${persona.nombre} — 
                <span class="text-success">${persona.pesoInicial}kg → ${persona.pesoFinal.toFixed(2)}kg</span>
                (${persona.variacionPeso.toFixed(2)}kg)
            </td>
        `;
        tbody.appendChild(filaPersona);

        persona.tiposAgrupados.forEach(tipo => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td><b>${tipo.nombreActividad}</b></td>
                <td>${tipo.totalActividades}</td>
                <td>${tipo.tiempoTotalMinutos}</td>
                <td>${tipo.totalCalorias}</td>
                <td>${persona.pesoInicial}</td>
                <td>${tipo.pesoFinal.toFixed(2)} kg</td>
            `;
            tbody.appendChild(fila);
        });
        const filaResumen = document.createElement("tr");
        filaResumen.classList.add("table-warning", "fw-bold");
        filaResumen.innerHTML = `
            <td><b>Total</b></td>
            <td>${persona.totalActividades}</td>
            <td>${persona.tiempoTotalMinutos}</td>
            <td>${persona.totalCalorias}</td>
            <td>${persona.pesoInicial}</td>
            <td>${persona.pesoFinal.toFixed(2)} kg</td>
        `;
        tbody.appendChild(filaResumen);
    });
}


ObtenerEstadisticasActividades();