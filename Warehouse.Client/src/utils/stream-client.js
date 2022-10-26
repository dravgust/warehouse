import { API_SERVER } from "../config/constant";

async function streamClient(
  endpoint,
  onRead = () => {},
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

  return window.fetch(`${API_SERVER}/${endpoint}`, config).then(async (response) => {
    const reader = response.body?.getReader();
    if (!reader) {
      throw new Error("Failed to read response");
    }
    while (true) {
      const { done, value } = await reader.read();
      if (done) break;
      if (!value) continue;
      let decoder = new TextDecoder("utf8");
      const jsonPart = decoder.decode(value);
      onRead(jsonPart);
    }
    reader.releaseLock();
  });
}

export { streamClient };
