import Axios from "axios";
import { API_SERVER } from "../config/constant";
import * as auth from "services/auth-provider";
import { queryClient } from "../context/app.context";

const API_URL = process.env.REACT_APP_API_URL || API_SERVER;

const axios = Axios.create({
  baseURL: `${API_URL}`,
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
  (response) => Promise.resolve(response?.data),
  (error) => {
    return new Promise(async (resolve, reject) => {
      const originalRequest = error.config;
      if (error.response && error.response.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        try {
          await auth.refreshToken();
          return resolve(axios(originalRequest));
        } catch {}
        queryClient.clear();
        await auth.logout();
        window.location.assign(window.location);
        return reject({ message: "Please re-authenticate." });
      }
      return reject(error);
    });
  }
);

export default axios;
