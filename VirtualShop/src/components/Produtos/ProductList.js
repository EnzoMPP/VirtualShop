import React, { useState, useEffect } from "react";
import ProductListItem from "./ProductListItem";
import productService from "../../services/productService";

const ProductList = ({ onEditProduct }) => {
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(true); 
  const [error, setError] = useState(null); 

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const productsData = await productService.getAllProducts();
      console.log("Produtos recebidos:", productsData); 

      if (Array.isArray(productsData)) {
        setProducts(productsData);
      } else {
        console.error("A resposta da API não é um array:", productsData);
        setError("Formato de dados inválido recebido da API.");
      }
    } catch (err) {
      console.error("Erro ao buscar produtos:", err);
      setError("Falha ao carregar produtos.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id) => {
    try {
      await productService.deleteProduct(id);
      fetchProducts();
    } catch (err) {
      console.error("Erro ao deletar produto:", err);
      setError("Falha ao deletar produto.");
    }
  };

  const handleEdit = () => {
    fetchProducts(); 
  };

  if (isLoading) {
    return <div className="container"><p>Carregando produtos...</p></div>;
  }

  if (error) {
    return <div className="container"><p className="text-danger">{error}</p></div>;
  }

  return (
    <div className="container">
      <h2 className="my-4">Lista de Produtos</h2>
      {products.length > 0 ? (
        <ul className="list-group">
          {products.map((product) => (
            <ProductListItem
              key={product.id}
              product={product}
              onDelete={() => handleDelete(product.id)}
              onEdit={handleEdit}
            />
          ))}
        </ul>
      ) : (
        <p>Nenhum produto encontrado.</p>
      )}
    </div>
  );
};

export default ProductList;