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


