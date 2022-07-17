import axios from "../api";

export const getProducts = async ({ queryKey }) => {
  const [_key, page, searchTerm] = queryKey;
  const res = await axios.get(`items?searchTerm=${searchTerm}&page=${page}&size=3`);
  return res?.data;
};
