import axios from "../api";

export const getBeacons = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`sites/beacons?searchTerm=${searchTerm}&page=${page}&size=6`);
  return res?.data;
};

export const getProducts = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`items?searchTerm=${searchTerm}&page=${page}&size=3`);
  return res?.data;
};

export const getEvents = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`events?page=${page}&size=10&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getNotifications = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`notifications?page=${page}&size=10&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getSites = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`sites?page=${page}&size=10&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getBeaconTelemetry = async ({ queryKey }) => {
  const [_key, id] = queryKey;
  const res = await axios.get(`assets/beacon?macAddress=${id}`);
  return res?.data;
};

export const getAssets = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`assets?page=${page}&size=6&searchTerm=${searchTerm}`);
  return res?.data;
};

export const getBeaconTelemetryCharts = async ({ queryKey }) => {
  const [_key, id] = queryKey;
  const res = await axios.get(`assets/beacon-telemetry?macAddress=${id}`);
  return res?.data;
};

export const getAssetsInfo = async () => {
  const res = await axios.get(`assets/info`);
  return res?.data;
};

export const getSitesInfo = async () => {
  const res = await axios.get(`assets/sites`);
  return res?.data;
};

export const getRegisteredBeacons = async () => {
  const res = await axios.get(`sites/beacons-registered`);
  return res?.data;
};
export const getRegisteredGw = async () => {
  const res = await axios.get(`sites/gw-registered`);
  return res?.data;
};

export const getProductMetadata = async () => {
  const res = await axios.get(`items/metadata`);
  return res?.data;
};

export const getBeaconMetadata = async () => {
  const res = await axios.get(`items/item-metadata`);
  return res?.data;
};
