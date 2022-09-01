import axios from "./index";

export const getUsers = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`users?page=${page}&size=9&searchTerm=${searchTerm}`);
  return res?.data;
};

export const saveUser = async (user) => {
  const res = await axios.post(`users/set`, user);
  return res?.data;
};

export const getRoles = async ({ queryKey }) => {
  const [_key] = queryKey;
  const res = await axios.get(`security/roles`);
  return res?.data;
};

export const getUserRoles = async ({ queryKey }) => {
  const [_key, user] = queryKey;
  const res = await axios.get(`security/user-roles/${user.id}`);
  return res?.data;
};

export const getObjects = async ({ queryKey }) => {
  const [_key] = queryKey;
  const res = await axios.get(`security/objects`);
  return res?.data;
};

export const getPermissions = async ({ queryKey }) => {
  const [_key, roleId] = queryKey;
  const res = await axios.get(`security/permissions/${roleId}`);
  return res?.data;
};

export const savePermissions = async (permissions) => {
  const res = await axios.post(`security/permissions/save`, permissions);
  return res?.data;
};
