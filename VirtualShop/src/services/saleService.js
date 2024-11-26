import axios from "axios";

const baseURL = "http://localhost:5000/api/Sale"; 

const saleService = {
  registerSale: async (saleRequest, token) => {
    try {
      const response = await axios.post(baseURL, saleRequest, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default saleService;