const BASE_API_URL = "http://localhost:5291/api/";

var API_URL = BASE_API_URL;


function getToken() {
    return localStorage.getItem("token");
}

function getRefreshToken() {
    return localStorage.getItem("refreshToken");
}

function getEmail() {
    return localStorage.getItem("email");
}

function getNombre() {
    return localStorage.getItem("nombreCompleto");
}

function getRol() {
  return localStorage.getItem("rol");
}

function saveTokens(token, refreshToken) {
  localStorage.setItem("token", token);
  localStorage.setItem("refreshToken", refreshToken);
}

function refreshToken() {
  return fetch(API_URL + "auth/refresh-token", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      email: getEmail(),
      refreshToken: getRefreshToken()
    })
  })
  .then(function(response) {
    if (!response.ok) {
      throw new Error("Error al renovar el token");
    }
    return response.json();
  })
  .then(function(data) {
    saveTokens(data.token, data.refreshToken);
    return data.token;
  });
}


function authFetch(url, options = {}, retry = true) {
  options.headers = options.headers || {};
  options.headers["Authorization"] = "Bearer " + getToken();

  // Solo poner Content-Type si NO es FormData
  if (!(options.body instanceof FormData)) {
    options.headers["Content-Type"] = "application/json";
  }

  return fetch(API_URL  + url, options).then((response) => {
    if (response.status === 401 && retry) {
      // Token expirado, intentamos renovarlo
      return refreshToken()
        .then((newToken) => {
          options.headers["Authorization"] = "Bearer " + newToken;
          return fetch(API_URL  + url, options);
        })
        .catch((err) => {
          console.error("No se pudo renovar el token:", err);
          localStorage.clear();
          window.location.href = "login.html";
          return response; // devolvemos el 401 original
        });
    }
    return response;
  });
}



