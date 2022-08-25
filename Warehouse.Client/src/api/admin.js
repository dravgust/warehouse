import axios from "./index";

export const getUsers = async ({ queryKey }) => {
  const [_key, page] = queryKey;
  const res = await axios.get(`users?page=${page}&take=9`);
  return res?.data;
};

export const getRoles = async ({ queryKey }) => {
  const [_key] = queryKey;
  const res = await axios.get(`security/roles`);
  return res?.data;
};

export const getObjects = async ({ queryKey }) => {
  const [_key] = queryKey;
  const res = await axios.get(`security/objects`);
  return res?.data;
};
