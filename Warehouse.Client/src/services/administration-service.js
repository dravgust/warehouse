import axios from "../api";

export const getUsers = async ({ queryKey }) => {
  const [_key, page] = queryKey;
  const res = await axios.get(`users?page=${page}&take=9`);
  return res?.data;
};
