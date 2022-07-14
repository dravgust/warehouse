import { queryClient } from "context/app.context";
import * as auth from "auth-provider";
const apiURL = process.env.REACT_APP_API_URL;

async function client(
  endpoint,
  { data, formData, token, headers: customHeaders, ...customConfig } = {}
) {
  const config = {
    method: data || formData ? "POST" : "GET",
    body: data ? JSON.stringify(data) : formData ? formData : undefined,
    headers: {
      Authorization: token ? `Bearer ${token}` : undefined,
      "Content-Type": data ? "application/json" : undefined,
      ...customHeaders,
    },
    credentials: "include",
    ...customConfig,
  };
  if (formData) {
    delete config.headers["Content-Type"];
  }

  return window.fetch(`${apiURL}/${endpoint}`, config).then(async (response) => {
    if (response.status === 201) {
      return Promise.resolve();
    }
    if (response.status === 401) {
      //todo:design refresh-token automatically with client proxy
      try {
        await auth.refreshToken();
        window.location.assign(window.location);
        return Promise.reject();
      } catch (err) {}
      queryClient.clear();
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
