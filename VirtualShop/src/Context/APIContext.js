import axios from "axios";
import { createContext, useContext, useEffect, useState } from "react";

const APIContext = createContext();

const initialUser = {
  data: [],
  auth: "",
};

export const Context = ({ children }) => {
  const [apiData, setApiData] = useState("");
  const [user, setUser] = useState(initialUser);
  const [isSigned, setIsSigned] = useState(false);

  const Login = async (email, password) => {
    try {
      const response = await axios.post(`${baseUrl}auth/sign-in`, {
        email,
        password,
      });
      setUser((prevState) => ({
        ...prevState,
        data: response.data,
        auth: response.headers.authorization,
      }));
      setIsSigned(true);
    } catch (error) {
      console.log(error);
    }

  };

  const Logout = () => {
    setUser([]);
    setIsSigned(false);
    localStorage.clear();
  };

  return <APIContext.Provider value={{}}>{children}</APIContext.Provider>;
};
