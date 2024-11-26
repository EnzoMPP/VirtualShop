import { useEffect, useState } from "react";
import userService from "../services/userService";

const Perfil = () => {
  const [perfil, setPerfil] = useState(null);
  const [produtosComprados, setProdutosComprados] = useState([]);
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchPerfil = async () => {
      try {
        const data = await userService.getProfile(token);
        setPerfil(data);
      } catch (error) {
        alert("Erro ao obter perfil.");
        console.error(error);
      }
    };

    const fetchProdutosComprados = async () => {
      try {
        const data = await userService.getPurchasedProducts(token);
        setProdutosComprados(data);
      } catch (error) {
        alert("Erro ao obter produtos comprados.");
        console.error(error);
      }
    };

    if (token) {
      fetchPerfil();
      fetchProdutosComprados();
    }
  }, [token]);

  if (!perfil) {
    return (
      <div className="container">
        <p>Carregando perfil...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="card">
        <div className="card-header">
          <h1>Perfil do Usuário</h1>
        </div>
        <div className="card-body">
          <p>
            <strong>Nome:</strong> {perfil.nomeUsuario}
          </p>
          <p>
            <strong>Email:</strong> {perfil.email}
          </p>
        </div>
      </div>

      <h2 className="mt-4">Produtos Comprados</h2>
      {produtosComprados.length > 0 ? (
        <div className="row">
          {produtosComprados.map((produto) => (
            <div key={produto.id} className="col-md-4 mb-4">
              <div className="card h-100">
                <img
                  src={produto.imageUrl}
                  className="card-img-top"
                  alt={produto.nome}
                />
                <div className="card-body">
                  <h5 className="card-title">{produto.nome}</h5>
                  <p className="card-text">Preço: R${produto.preco}</p>
                  <p className="card-text">Data da Compra: {produto.dataCompra}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p>Você ainda não comprou nenhum produto.</p>
      )}
    </div>
  );
};

export default Perfil;