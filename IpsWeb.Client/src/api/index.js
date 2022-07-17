import Axios from "axios";
import { API_SERVER } from "../config/constant";
import * as auth from "../auth-provider";

const axios = Axios.create({
  baseURL: `${API_SERVER}`,
  headers: { "Content-Type": "application/json" },
});

axios.interceptors.request.use(
  async (config) => {
    const token = await auth.getToken();
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return Promise.resolve(config);
  },
  (error) => Promise.reject(error)
);

axios.interceptors.response.use(
  (response) => Promise.resolve(response),
  (error) => {
    return new Promise(async (resolve) => {
      const originalRequest = error.config;
      if (error.response && error.response.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        try {
          await auth.refreshToken();
          const response = axios(originalRequest);
          return resolve(response);
        } catch (err) {
          console.log("refresh-token-error", err);
        }
      }
      console.log("logout");
      await auth.logout();
      // refresh the page for them
      window.location.assign(window.location);
      return Promise.reject({ message: "Please re-authenticate." });
    });
  }
);

export default axios;
