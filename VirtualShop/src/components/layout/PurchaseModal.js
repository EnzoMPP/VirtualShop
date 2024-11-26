import React, { useState } from "react";

const PurchaseModal = ({ showModal, handleCloseModal, handlePurchase }) => {
  const [quantidade, setQuantidade] = useState(1);
  const [formaPagamento, setFormaPagamento] = useState("Cartão de Crédito");

  const handleQuantidadeChange = (e) => {
    setQuantidade(e.target.value);
  };

  const handleFormaPagamentoChange = (e) => {
    setFormaPagamento(e.target.value);
  };

  const handleConfirmPurchase = () => {
    handlePurchase(quantidade);
    handleCloseModal();
  };

  return (
    <div className={`modal ${showModal ? 'd-block' : 'd-none'}`} tabIndex="-1" role="dialog">
      <div className="modal-dialog" role="document">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Comprar Produto</h5>
            <button type="button" className="close" onClick={handleCloseModal} aria-label="Close" style={{ position: 'absolute', top: '10px', right: '10px' }}>
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="modal-body">
            <div className="form-group">
              <label>Quantidade</label>
              <input
                type="number"
                className="form-control"
                value={quantidade}
                onChange={handleQuantidadeChange}
                min="1"
              />
            </div>
            <div className="form-group">
              <label>Forma de Pagamento</label>
              <select className="form-control" value={formaPagamento} onChange={handleFormaPagamentoChange}>
                <option>Cartão de Crédito</option>
                <option>Cartão de Débito</option>
                <option>Boleto</option>
                <option>Pix</option>
              </select>
            </div>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" onClick={handleCloseModal}>Cancelar</button>
            <button type="button" className="btn btn-primary" onClick={handleConfirmPurchase}>Confirmar</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PurchaseModal;