function verificarUsuario(){
    const token = getToken();
    const email = getEmail(); // suponiendo que guardaste el email al hacer login
    const nombre = getNombre();
    const rol = getRol();
   
    $("#email-usuario").text(email);
    $("#nombre-usuario").text(nombre);
    $("#rol-usuario").text(rol);

    if (!token) {
        localStorage.removeItem("token");
        localStorage.removeItem("email");
        window.location.href = "../views/login.html";
        return;
    }
}  