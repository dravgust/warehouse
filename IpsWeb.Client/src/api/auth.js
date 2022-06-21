import axios from "./index";

class AuthApi {

  static Login = (data) => {

    const instance = axios.create({
      withCredentials: false,
      headers: {
        'Access-Control-Allow-Origin' : '*',
        'Access-Control-Allow-Methods':'GET,PUT,POST,DELETE,PATCH,OPTIONS',
        }
    });
    
    return instance.post(`${base}/login`, data);
  };

  static Register = (data) => {
    return axios.post(`${base}/register`, data);
  };

  static Logout = (data) => {
    return axios.post(`${base}/logout`, data, { headers: { Authorization: `${data.token}` } });
  };
}

let base = "users";

export default AuthApi;