
import { useEffect, useState } from "react";
import productService from "../services/productService";
import saleService from "../services/saleService"; 
import userService from "../services/userService"; 
import "../styles/Home.css"; 

const Home = () => {
  const [produtos, setProdutos] = useState([]);
  const [perfil, setPerfil] = useState(null);
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchProdutos = async () => {
      try {
        const data = await productService.getAllProducts();
        console.log("Produtos recebidos:", data);
        setProdutos(Array.isArray(data) ? data : []);
      } catch (error) {
        console.error("Erro ao buscar produtos:", error);
        setProdutos([]);
      }
    };

    const fetchPerfil = async () => {
      try {
        const data = await userService.getProfile(token);
        setPerfil(data);
        console.log("Perfil do usuário:", data); 
      } catch (error) {
        console.error("Erro ao buscar perfil:", error);
        setPerfil(null);
      }
    };

    if (token) {
      fetchProdutos();
      fetchPerfil();
    }
  }, [token]);

  const handleComprar = async (produtoId) => {
    if (!perfil) {
      alert("Usuário não autenticado.");
      return;
    }
  
    
    const userId = perfil.id;
    if (!userId) {
      alert("ID de usuário inválido.");
      return;
    }
  
    console.log("UserId para venda:", userId); 
  
    const saleRequest = {
      Sale: {
        DataVenda: new Date(),
        UserId: userId, 
      },
      ProductIds: [produtoId],
      Quantidades: [1],
    };
  
    try {
      const venda = await saleService.registerSale(saleRequest, token);
      alert("Compra realizada com sucesso!");
      console.log("Venda registrada:", venda);
    } catch (error) {
      console.error("Erro ao registrar venda:", error);
      if (error.response && error.response.data) {
        alert(`Erro: ${error.response.data}`);
      } else {
        alert("Erro ao realizar a compra.");
      }
    }
  };

  return (
    <div className="container mt-4">
      <h1>Home</h1>
      <p>Bem-vindo ao VirtualShop!</p>
      <div className="row">
        {produtos.length > 0 ? (
          produtos.map((produto) => (
            <div key={produto.id} className="col-md-4 mb-4">
              <div className="card h-100">
                <img src={produto.imageUrl} className="card-img-top" alt={produto.nome} />
                <div className="card-body d-flex flex-column">
                  <h5 className="card-title">{produto.nome}</h5>
                  <p className="card-text">Preço: R${produto.preco}</p>
                  <button
                    className="btn btn-primary mt-auto"
                    onClick={() => handleComprar(produto.id)}
                  >
                    Comprar
                  </button>
                </div>
              </div>
            </div>
          ))
        ) : (
          <p>Nenhum produto encontrado.</p>
        )}
      </div>
    </div>
  );
};

export default Home;