const localStorageKey = "__auth_provider_token__";
import { API_SERVER } from "config/constant";

const API_URL = process.env.REACT_APP_API_URL || API_SERVER;

async function getToken() {
  return window.localStorage.getItem(localStorageKey);
}

function handleUserResponse({ username, token }) {
  const user = { username, token };
  window.localStorage.setItem(localStorageKey, token);
  return user;
}

async function login({ email, password }) {
  return client("login", { email, password }).then(handleUserResponse);
}

async function register({ email, password }) {
  return client("register", { email, password }).then(handleUserResponse);
}

function logout() {
  window.localStorage.removeItem(localStorageKey);
}

async function refreshToken() {
  return client("refresh-token", {
    token: null,
  }).then(handleUserResponse);
}

let base = "account";
async function client(endpoint, data) {
  const config = {
    method: "POST",
    body: JSON.stringify(data),
    headers: { "Content-Type": "application/json" },
    credentials: "include",
  };

  return window.fetch(`${API_URL}/${base}/${endpoint}`, config).then(async (response) => {
    const data = await response.json();
    if (response.ok) {
      return data;
    } else {
      return Promise.reject(data);
    }
  });
}

export { getToken, login, register, logout, refreshToken, localStorageKey };
