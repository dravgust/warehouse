import axios from "./index";

//Beacons
export const getRegisteredBeacons = () => axios.get(`beacons/registered`);
export const getBeaconMetadata = () => axios.get(`products/item-metadata`);
export const setBeacon = (item) => axios.post(`beacons/set`, item);
export const deleteBeacon = (item) => axios.post(`beacons/delete`, item);
export const getBeacons = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`beacons?searchTerm=${searchTerm}&page=${page}&size=6`);
};
//alerts
export const saveAlert = (item) => axios.post(`alerts/set`, item);
export const deleteAlert = (item) => axios.post(`alerts/delete`, item);
export const getAlerts = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`alerts?searchTerm=${searchTerm}&page=${page}&size=6`);
};
//Products
export const getProductMetadata = () => axios.get(`products/metadata`);
export const setProduct = (item) => axios.post(`products/set`, item);
export const deleteProduct = (item) => axios.post(`products/delete`, item);
export const getProducts = ({ queryKey }) => {
  const [_key, page, searchTerm, size] = queryKey;
  return axios.get(`products?searchTerm=${searchTerm}&page=${page}&size=${size || "3"}`);
};
//Events
export const getEvents = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`events?page=${page}&size=${3}&searchTerm=${searchTerm}`);
};
//Notifications
export const getNotifications = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`notifications?page=${page}&size=${3}&searchTerm=${searchTerm}`);
};
//Sites
export const getSiteById = (id) => axios.get(`sites/${id}`);
export const getRegisteredGw = () => axios.get(`sites/gw-registered`);
export const setSiteGw = (item) => axios.post(`sites/set-gateway`, item);
export const deleteSiteGw = (item) =>
  axios.get(`sites/${item.siteId}/delete-gw/${item.macAddress}`);
export const setSite = (item) => axios.post(`sites/set`, item);
export const getSites = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`sites?page=${page}&size=10&searchTerm=${searchTerm}`);
};
//Dashboard
export const getAssetsInfo = () => axios.get(`dashboard/products`);
export const getSitesInfo = () => axios.get(`dashboard/sites`);
//export const bootstrap = () => await axios.get("account/bootstrap");
export const getBeaconTelemetry = ({ queryKey }) => {
  const [_key, id] = queryKey;
  return axios.get(`dashboard/beacon/${id}?t=${new Date().getTime()}`);
};
export const getBeaconPosition = ({ queryKey }) => {
  const [_key, site, beacon] = queryKey;
  return axios.get(
    `dashboard/beacon/position/${beacon.macAddress}?siteId=${site.id}&t=${new Date().getTime()}`
  );
};
export const getAssets = ({ queryKey }) => {
  const [_key, page, selectedSite, searchTerm] = queryKey;
  const siteId = selectedSite ? selectedSite.id : "";
  return axios.get(
    `dashboard/beacons?page=${page}&size=8&siteId=${siteId}&searchTerm=${searchTerm}`
  );
};
export const getBeaconTelemetryCharts = ({ queryKey }) => {
  const [_key, id] = queryKey;
  return axios.get(`dashboard/beacon/charts/${id}?t=${new Date().getTime()}`);
};
