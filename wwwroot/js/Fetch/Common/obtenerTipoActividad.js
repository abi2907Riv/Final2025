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
    let bodySelect = document.getElementById("TipoActividadId");
    bodySelect.innerHTML = "";
    // let bodySelectFiltro = document.getElementById("filtroActividad");
    // bodySelectFiltro.innerHTML = "";
    

    bodySelect.innerHTML = "<option value='0' hidden>[Seleccione una opción]</option>";
    // bodySelectFiltro.innerHTML = "<option value='0'>[Todas]</option>";

    const activos = data.filter(item => item.eliminado == false);
    activos.forEach(element => {
        let opt = document.createElement("option");
        opt.value = element.tipoActividadID;
        opt.innerHTML = element.nombre;
        bodySelect.appendChild(opt);

        // let optFiltro = document.createElement("option");
        // optFiltro.value = element.tipoActividadID;
        // optFiltro.innerHTML = element.nombre;
        // bodySelectFiltro.appendChild(optFiltro);
   })
}









////////////////////////////////////////////////////
//FUNCION PARA CARGAR DROP CATEGORIAS INICIALMENTE//
///////////////////////////////////////////////////
ObtenerTipoActividadDrop()

