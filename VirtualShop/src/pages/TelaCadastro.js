import { useState } from "react";
import { useNavigate } from "react-router-dom";
import userService from "../services/userService";

const Cadastro = () => {
  const [nome, setNome] = useState("");
  const [cpf, setCpf] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const user = { NomeUsuario: nome, Email: email, Senha: senha, Cpf: cpf };
    try {
      const response = await userService.register(user);
      alert("Usuário cadastrado com sucesso!");
      navigate("/login");
    } catch (error) {
      alert(error.response?.data || "Erro ao cadastrar usuário.");
      console.error(error);
    }
  };

  const handleBackToLogin = () => {
    navigate("/login");
  };

  return (
    <div className="container d-flex justify-content-center align-items-center min-vh-100">
      <div className="row text-center col-12 col-md-6">
        <h1 className="text-center mb-4">Cadastro</h1>
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="nome" className="form-label">
              Nome:
            </label>
            <input
              type="text"
              id="nome"
              name="nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <div className="mb-3">
            <label htmlFor="cpf" className="form-label">
              CPF:
            </label>
            <input
              type="text"
              id="cpf"
              name="cpf"
              value={cpf}
              onChange={(e) => setCpf(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              Email:
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">
              Senha:
            </label>
            <input
              type="password"
              id="senha"
              name="senha"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <button className="btn btn-primary w-100" type="submit">
            Cadastrar
          </button>
        </form>
        <button className="btn btn-secondary w-100 mt-3" onClick={handleBackToLogin}>
          Voltar para Login
        </button>
      </div>
    </div>
  );
};

export default Cadastro;  