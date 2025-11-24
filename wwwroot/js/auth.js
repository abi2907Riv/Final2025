const BASE_API_URL = "http://localhost:5291/api/";
var API_URL = BASE_API_URL;


function validarRegistro() {
  const nombre = document.getElementById("regNombre");
  const email = document.getElementById("regEmail");
  const password = document.getElementById("regPassword");
  const fecha = document.getElementById("regFechaNacimiento");
  const peso = document.getElementById("regPeso");

  const errorNombre = document.getElementById("errorNombreRegistrar");
  const errorEmail = document.getElementById("errorEmailRegistrar");
  const errorPassword = document.getElementById("errorContraseñaRegistrar");
  const errorFecha = document.getElementById("errorFechaNacimientoRegistrar");
  const errorPeso = document.getElementById("errorPesoRegistrar");

  [nombre, email, password, fecha, peso].forEach((i) =>
    i.classList.remove("is-invalid")
  );
  [errorNombre, errorEmail, errorPassword, errorFecha, errorPeso].forEach(
    (e) => (e.textContent = "")
  );

  let valido = true;

  if (!nombre.value.trim()) {
    nombre.classList.add("is-invalid");
    errorNombre.textContent = "El nombre es obligatorio.";
    valido = false;
  }

  if (!email.value.trim()) {
    email.classList.add("is-invalid");
    errorEmail.textContent = "El email es obligatorio.";
    valido = false;
  }

  if (!password.value.trim()) {
    password.classList.add("is-invalid");
    errorPassword.textContent = "La contraseña es obligatoria.";
    valido = false;
  }

  if (!fecha.value.trim()) {
    fecha.classList.add("is-invalid");
    errorFecha.textContent = "La fecha de nacimiento es obligatoria.";
    valido = false;
  }

  const pesoValue = parseFloat(peso.value);
  if (isNaN(pesoValue)) {
    peso.classList.add("is-invalid");
    errorPeso.textContent = "Ingrese un peso válido.";
    valido = false;
  }

  return valido;
}

function validarLogin() {
  const email = document.getElementById("loginEmail");
  const password = document.getElementById("loginPassword");

  const errorEmail = document.getElementById("errorLoginEmail");
  const errorPassword = document.getElementById("errorContraseñaLogin");

  email.classList.remove("is-invalid");
  password.classList.remove("is-invalid");

  errorEmail.textContent = "";
  errorPassword.textContent = "";

  let valido = true;

  if (!email.value.trim()) {
    email.classList.add("is-invalid");
    errorEmail.textContent = "Ingrese su email.";
    valido = false;
  }

  if (!password.value.trim()) {
    password.classList.add("is-invalid");
    errorPassword.textContent = "Ingrese su contraseña.";
    valido = false;
  }

  return valido;
}



// ======================= REGISTRO =======================
document
  .getElementById("registerForm")
  .addEventListener("submit", async (e) => {
    e.preventDefault();

    if (!validarRegistro()) return;

    const nombre = document.getElementById("regNombre").value.trim();
    const email = document.getElementById("regEmail").value.trim();
    const password = document.getElementById("regPassword").value;
    const fechaNacimiento = document.getElementById("regFechaNacimiento").value;

    let pesoValue = document.getElementById("regPeso").value.trim();
    if (!pesoValue) {
      const pesoInput = document.getElementById("regPeso");
      pesoInput.classList.add("is-invalid");
      document.getElementById("errorPesoRegistrar").textContent =
        "Ingrese un peso válido.";
      return;
    }
    pesoValue = pesoValue.replace(",", ".");
    pesoValue = parseFloat(pesoValue);
    if (isNaN(pesoValue)) {
      const pesoInput = document.getElementById("regPeso");
      pesoInput.classList.add("is-invalid");
      document.getElementById("errorPesoRegistrar").textContent =
        "Ingrese un peso válido.";
      return;
    }

    const data = {
      NombreCompleto: nombre,
      Email: email,
      Password: password,
      FechaNacimiento: fechaNacimiento,
      Peso: pesoValue,
    };

    const response = await fetch(BASE_API_URL + "auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });

    const result = await response.text();

    if (response.ok) {
      alert("¡Registro exitoso!");
      setTimeout(() => (window.location.href = "./sesionUsuario.html?login=1"), 1500);
    } else {
      const emailInput = document.getElementById("regEmail");
      const errorEmail = document.getElementById("errorEmailRegistrar");
      emailInput.classList.add("is-invalid");
      errorEmail.textContent = result;
    }
  });



// ======================= LOGIN =======================
document.getElementById("loginForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  if (!validarLogin()) return;

  const data = {
    email: document.getElementById("loginEmail").value,
    password: document.getElementById("loginPassword").value,
  };

  try {
    const response = await fetch(BASE_API_URL + "auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error("Credenciales inválidas");
    }

    const result = await response.json();

    localStorage.setItem("token", result.token);
    localStorage.setItem("refreshToken", result.refreshToken);
    localStorage.setItem("email", data.email);
    localStorage.setItem("nombreCompleto", result.nombreCompleto);
    localStorage.setItem("rol", result.rol);

    window.location.href = "../index.html";

  } catch (error) {
    const pass = document.getElementById("loginPassword");
    const errorPass = document.getElementById("errorContraseñaLogin");

    pass.classList.add("is-invalid");
    errorPass.textContent = "Email o contraseña incorrectos";
  }
});
