import React, { useState } from "react";
import productService from "../../services/productService";

const ProductListItem = ({ product, onDelete, onEdit }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedName, setEditedName] = useState(product.nome);
  const [editedPrice, setEditedPrice] = useState(product.preco);
  const [editedImage, setEditedImage] = useState(product.imageUrl);

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleSave = async () => {
    const editedProduct = {
      ...product,
      nome: editedName,
      preco: parseFloat(editedPrice),
      imageUrl: editedImage,
    };

    try {
      await productService.updateProduct(product.id, editedProduct);
      setIsEditing(false);
      onEdit(); 
    } catch (error) {
      console.error("Erro ao atualizar produto:", error);
    }
  };

  const handleCancel = () => {
    setIsEditing(false);
    setEditedName(product.nome);
    setEditedPrice(product.preco);
    setEditedImage(product.imageUrl);
  };

  return (
    <li className="list-group-item">
      {isEditing ? (
        <div className="row">
          <div className="col">
            <input
              type="text"
              className="form-control"
              value={editedName}
              onChange={(e) => setEditedName(e.target.value)}
              required
            />
          </div>
          <div className="col">
            <input
              type="number"
              className="form-control"
              value={editedPrice}
              onChange={(e) => setEditedPrice(e.target.value)}
              required
            />
          </div>
          <div className="col">
            <input
              type="text"
              className="form-control"
              value={editedImage}
              onChange={(e) => setEditedImage(e.target.value)}
              required
            />
          </div>
          <div className="col-auto">
            <button className="btn btn-success me-2" onClick={handleSave}>
              Salvar
            </button>
            <button className="btn btn-secondary" onClick={handleCancel}>
              Cancelar
            </button>
          </div>
        </div>
      ) : (
        <div className="d-flex justify-content-between align-items-center">
          <span>
            {product.nome} - R${product.preco}
          </span>
          <div>
            <button className="btn btn-primary me-2" onClick={handleEdit}>
              Editar
            </button>
            <button className="btn btn-danger" onClick={onDelete}>
              Deletar
            </button>
          </div>
        </div>
      )}
    </li>
  );
};

export default ProductListItem;