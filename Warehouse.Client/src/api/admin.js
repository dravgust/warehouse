import axios from "./index";

export const getProviders = () => axios.get(`providers`);
export const saveUser = (user) => axios.post(`users/set`, user);
export const saveProvider = (user) => axios.post(`providers/set`, user);
export const deleteProvider = (item) => axios.post(`providers/delete`, item);
export const deleteUser = (item) => axios.post(`users/delete`, item);
export const getRoles = () => axios.get(`security/roles`);
export const getObjects = () => axios.get(`security/objects`);
export const savePermissions = (permissions) =>
  axios.post(`security/permissions/save`, permissions);

export const getUsers = ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  return axios.get(`users?page=${page}&size=9&searchTerm=${searchTerm}`);
};
export const getUserRoles = ({ queryKey }) => {
  const [_key, user] = queryKey;
  return axios.get(`security/user-roles/${user.id}`);
};
export const getPermissions = ({ queryKey }) => {
  const [_key, roleId] = queryKey;
  return axios.get(`security/permissions/${roleId}`);
};
