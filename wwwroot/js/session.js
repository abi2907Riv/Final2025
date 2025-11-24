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

async function CerrarSesion() {
  //FUNCION DE LEER TOKEN DEL DISPOSITIVO
  //const getToken = () => localStorage.getItem("token");
  const token = getToken();
  const email = localStorage.getItem("email"); 

  if (!token || !email) {
    localStorage.removeItem("token");
    localStorage.removeItem("email");
    window.location.href = "../views/sesionUsuario.html";
    return;
  }
  try {
    const res = await fetch(BASE_API_URL + `auth/logout`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ email }),
    });

    if (res.ok) {
      //alert("Sesión cerrada correctamente");
    } else {
      alert("Error al cerrar sesión: " + (await res.text()));
    }
  } catch (error) {
    console.error("Error en logout:", error);
  }

  // Limpiar token y redirigir
  localStorage.removeItem("token");
  localStorage.removeItem("email");

  window.location.href = "../views/sesionUsuario.html";
}
