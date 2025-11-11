const BASE_API_URL = "http://localhost:5291/api/";

var API_URL = BASE_API_URL;

//Registrar usuario
document
  .getElementById("registerForm")
  .addEventListener("submit", async (e) => {
    e.preventDefault();
    const data = {
      NombreCompleto: document.getElementById("regNombre").value,
      Email: document.getElementById("regEmail").value,
      Password: document.getElementById("regPassword").value,
    };
    // const apiBase = `${BASE_API_URL}auth`;
    const response = await fetch(BASE_API_URL + "auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    const result = await response.text();
    if (response.ok) {
      alert("¡Registro exitoso!", "success");
      setTimeout(() => (window.location.href = "login.html"), 10000);
    } else {
      alert("Registro fallido", "error");
    }
  });

document.getElementById("loginForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const data = {
    email: document.getElementById("loginEmail").value,
    password: document.getElementById("loginPassword").value,
  };

  try {
    const response = await fetch(BASE_API_URL + "auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      throw new Error("Error en la solicitud");
    }

    const result = await response.json();
    localStorage.setItem("token", result.token);
    localStorage.setItem("refreshToken", result.refreshToken);
    console.log(
      "Tokens guardados en login:",
      result.token,
      result.refreshToken
    );
    localStorage.setItem("email", document.getElementById("loginEmail").value);
    localStorage.setItem("nombreCompleto", result.nombreCompleto);
    localStorage.setItem("rol", result.rol);

    window.location.href = "../index.html";
  } catch (error) {
    document.getElementById("tokenOutput").textContent = "No conecta la API";
  }
});


async function cerrarSesion() {
  //FUNCION DE LEER TOKEN DEL DISPOSITIVO
  //const getToken = () => localStorage.getItem("token");
  const token = getToken();
  const email = localStorage.getItem("email"); // suponiendo que guardaste el email al hacer login

  if (!token || !email) {
    localStorage.removeItem("token");
    localStorage.removeItem("email");
    window.location.href = "../views/login.html";
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

  window.location.href = "../views/login.html";
}