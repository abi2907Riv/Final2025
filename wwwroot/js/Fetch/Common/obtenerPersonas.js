//////////////////////////////////////////////////
//FUNCION PARA OBTENER LAS PERSONAS EN EL DROP//
/////////////////////////////////////////////////
async function ObtenerPersonasDrop() {
    const res = await authFetch('Personas', {
        method: 'GET'
    })
    .then(response => response.json())
    .then(data => {
        MostrarPersonasDrop(data)
    })
    .catch(error => console.log('No se puede acceder al servicio', error));
}
function MostrarPersonasDrop(data) {
    let bodySelect = document.getElementById("PersonaId");
    bodySelect.innerHTML = "";
    // let bodySelectFiltro = document.getElementById("CategoriaIDBuscar");
    // bodySelectFiltro.innerHTML = "";
    

    bodySelect.innerHTML = "<option value='0' hidden>[Seleccione una opción]</option>";
    // bodySelectFiltro.innerHTML = "<option value='0'>[Todas]</option>";

    data.forEach(element => {
        let opt = document.createElement("option");
        opt.value = element.personaID;
        opt.innerHTML = element.nombre;
        bodySelect.appendChild(opt);

        // let optFiltro = document.createElement("option");
        // optFiltro.value = element.id;
        // optFiltro.innerHTML = element.nombre;
        // bodySelectFiltro.appendChild(optFiltro);
   })
}
////////////////////////////////////////////////////
//FUNCION PARA CARGAR DROP CATEGORIAS INICIALMENTE//
///////////////////////////////////////////////////
ObtenerPersonasDrop()

