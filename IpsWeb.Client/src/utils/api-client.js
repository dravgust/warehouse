import { queryCache } from "react-query";
import * as auth from "auth-provider";
const apiURL = process.env.REACT_APP_API_URL;

async function client(endpoint, { data, token, headers: customHeaders, ...customConfig } = {}) {
  const config = {
    method: data ? "POST" : "GET",
    body: data ? JSON.stringify(data) : undefined,
    headers: {
      Authorization: token ? `Bearer ${token}` : undefined,
      "Content-Type": data ? "application/json" : undefined,
      ...customHeaders,
    },
    credentials: "include",
    ...customConfig,
  };

  return window.fetch(`${apiURL}/${endpoint}`, config).then(async (response) => {
    console.log("api-client:response", response);
    if (response.status === 201) {
      return Promise.resolve();
    }
    if (response.status === 401) {
      /*try {
        await auth.refreshToken();
        window.location.assign(window.location);
        return Promise.reject();
      } catch (err) {
        console.log("api-client:err", err);
      }*/
      //queryCache.clear()
      await auth.logout();
      // refresh the page for them
      window.location.assign(window.location);
      return Promise.reject({ message: "Please re-authenticate." });
    }
    const data = await response.json();
    if (response.ok) {
      return data;
    } else {
      return Promise.reject(data);
    }
  });
}

export { client };
