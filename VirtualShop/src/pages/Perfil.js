import { useEffect, useState } from "react";
import userService from "../services/userService";
import AdminModal from "../components/layout/AdminModal";

const Perfil = () => {
  const [perfil, setPerfil] = useState(null);
  const [produtosComprados, setProdutosComprados] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [adminData, setAdminData] = useState({ nomeUsuario: "", email: "", cpf: "", senha: "" });
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchPerfil = async () => {
      try {
        const data = await userService.getProfile(token);
        setPerfil(data);
        console.log("Perfil do usuário:", data);

        const comprasData = await userService.getPurchasedProducts(data.id, token);
        console.log("Produtos Comprados (raw):", comprasData);

        const produtos = comprasData.$values || [];
        setProdutosComprados(produtos);
        console.log("Produtos Comprados (processed):", produtos);
      } catch (error) {
        console.error("Erro ao buscar perfil ou produtos comprados:", error);
        setPerfil(null);
        setProdutosComprados([]);
      }
    };

    if (token) {
      fetchPerfil();
    }
  }, [token]);

  const handleShowModal = () => setShowModal(true);
  const handleCloseModal = () => setShowModal(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setAdminData({ ...adminData, [name]: value });
  };

  const handleAddAdmin = async () => {
    try {
      await userService.addAdmin(adminData, token);
      alert("Administrador cadastrado com sucesso!");
      handleCloseModal();
    } catch (error) {
      console.error("Erro ao cadastrar administrador:", error);
      alert("Erro ao cadastrar administrador.");
    }
  };

  if (!perfil) {
    return (
      <div className="container">
        <p>Carregando perfil...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h1>Perfil</h1>
      <div className="card mb-4">
        <div className="card-body">
          <p><strong>Nome de Usuário:</strong> {perfil.nomeUsuario}</p>
          <p><strong>Email:</strong> {perfil.email}</p>
          <p><strong>CPF:</strong> {perfil.cpf}</p>
          <p><strong>Status:</strong> {perfil.isAdmin ? "Administrador" : "Usuário"}</p>
          {perfil.isAdmin && (
            <button className="btn btn-primary" onClick={handleShowModal}>
              Adicionar Administrador
            </button>
          )}
        </div>
      </div>

      <h2>Produtos Comprados</h2>
      {produtosComprados.length > 0 ? (
        <ul className="list-group">
          {produtosComprados.map((compra) => (
            <li key={compra.id} className="list-group-item">
              <strong>Produto:</strong> {compra.nome} <br />
              <strong>Preço:</strong> R${compra.preco} <br />
              <strong>Data da Compra:</strong> {new Date(compra.dataCompra).toLocaleDateString('pt-BR')} <br />
              <img src={compra.imageUrl} alt={compra.nome} style={{ width: '100px', height: '100px' }} />
            </li>
          ))}
        </ul>
      ) : (
        <p>Nenhuma compra registrada.</p>
      )}

      {/* Modal de Cadastro de Administrador */}
      {perfil.isAdmin && (
        <AdminModal
          showModal={showModal}
          handleCloseModal={handleCloseModal}
          handleInputChange={handleInputChange}
          handleAddAdmin={handleAddAdmin}
          adminData={adminData}
        />
      )}
    </div>
  );
};

export default Perfil;