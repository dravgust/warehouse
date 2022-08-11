import axios from "../api";

export const getBeacons = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`beacons?searchTerm=${searchTerm}&page=${page}&size=6`);
  return res?.data;
};

export const getAlerts = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`alerts?searchTerm=${searchTerm}&page=${page}&size=6`);
  return res?.data;
};

export const saveAlert = async (item) => {
  const res = await axios.post(`alerts/set`, item);
  return res?.data;
};
export const deleteAlert = async (item) => {
  const res = await axios.post(`alerts/delete`, item);
  return res?.data;
};

//Products
export const getProducts = async ({ queryKey }) => {
  const [_key, page, searchTerm, size] = queryKey;
  const res = await axios.get(`products?searchTerm=${searchTerm}&page=${page}&size=${size || "3"}`);
  return res?.data;
};

export const getProductMetadata = async () => {
  const res = await axios.get(`products/metadata`);
  return res?.data;
};

export const getBeaconMetadata = async () => {
  const res = await axios.get(`products/item-metadata`);
  return res?.data;
};

export const setProduct = async (item) => {
  const res = await axios.post(`products/set`, item);
  return res?.data;
};

export const deleteProduct = async (item) => {
  const res = await axios.post(`products/delete`, item);
  return res?.data;
};

export const getEvents = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(
    `events?page=${page}&size=${searchTerm ? "3" : "10"}&searchTerm=${searchTerm}`
  );
  return res?.data;
};

export const getNotifications = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(
    `notifications?page=${page}&size=${searchTerm ? "3" : "10"}&searchTerm=${searchTerm}`
  );
  return res?.data;
};

export const getSites = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`sites?page=${page}&size=10&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getBeaconTelemetry = async ({ queryKey }) => {
  const [_key, id] = queryKey;
  const res = await axios.get(`dashboard/beacon/${id}`);
  return res?.data;
};

export const getAssets = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`dashboard/beacons?page=${page}&size=6&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getBeaconTelemetryCharts = async ({ queryKey }) => {
  const [_key, id] = queryKey;
  const res = await axios.get(`dashboard/beacon/charts/${id}`);
  return res?.data;
};

export const getAssetsInfo = async () => {
  const res = await axios.get(`dashboard/products`);
  return res?.data;
};

export const getSitesInfo = async () => {
  const res = await axios.get(`dashboard/sites`);
  return res?.data;
};

export const getRegisteredBeacons = async () => {
  const res = await axios.get(`beacons/registered`);
  return res?.data;
};
export const getRegisteredGw = async () => {
  const res = await axios.get(`sites/gw-registered`);
  return res?.data;
};

export const setSiteGw = async (item) => {
  const res = await axios.post(`sites/set-gateway`, item);
  return res?.data;
};

export const setSite = async (item) => {
  const res = await axios.post(`sites/set`, item);
  return res?.data;
};

export const setBeacon = async (item) => {
  const res = await axios.post(`beacons/set`, item);
  return res?.data;
};

export const deleteBeacon = async (item) => {
  const res = await axios.post(`beacons/delete`, item);
  return res?.data;
};
