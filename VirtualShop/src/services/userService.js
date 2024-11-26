import axios from "axios";

const baseURL = "http://localhost:5000/api/User";

const userService = {
  register: async (userData) => {
    const response = await axios.post(`${baseURL}/register`, userData);
    return response.data;
  },
  login: async (loginData) => {
    const response = await axios.post(`${baseURL}/login`, loginData);
    return response.data;
  },
  getProfile: async (token) => {
    const response = await axios.get(`${baseURL}/profile`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    const profile = response.data;
    return {
      id: profile.id,
      nomeUsuario: profile.nomeUsuario,
      email: profile.email,
      cpf: profile.cpf,
      isAdmin: profile.isAdmin,
    };
  },
  getPurchasedProducts: async (id, token) => {
    const response = await axios.get(`${baseURL}/${id}/purchased-products`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  },
  addAdmin: async (adminData, token) => {
    const response = await axios.post(`http://localhost:5000/api/Admin`, adminData, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });
    return response.data;
  },
  
};

export default userService;