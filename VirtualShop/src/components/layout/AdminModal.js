import React from "react";

const AdminModal = ({ showModal, handleCloseModal, handleInputChange, handleAddAdmin, adminData }) => {
  return (
    <div className={`modal ${showModal ? 'd-block' : 'd-none'}`} tabIndex="-1" role="dialog">
      <div className="modal-dialog" role="document">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Adicionar Administrador</h5>
            <button type="button" className="close" onClick={handleCloseModal} aria-label="Close" style={{ position: 'absolute', top: '10px', right: '10px' }}>
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="modal-body">
            <div className="form-group">
              <label>Nome de Usuário</label>
              <input
                type="text"
                className="form-control"
                name="nomeUsuario"
                value={adminData.nomeUsuario}
                onChange={handleInputChange}
              />
            </div>
            <div className="form-group">
              <label>Email</label>
              <input
                type="email"
                className="form-control"
                name="email"
                value={adminData.email}
                onChange={handleInputChange}
              />
            </div>
            <div className="form-group">
              <label>CPF</label>
              <input
                type="text"
                className="form-control"
                name="cpf"
                value={adminData.cpf}
                onChange={handleInputChange}
              />
            </div>
            <div className="form-group">
              <label>Senha</label>
              <input
                type="password"
                className="form-control"
                name="senha"
                value={adminData.senha}
                onChange={handleInputChange}
              />
            </div>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" onClick={handleCloseModal}>Cancelar</button>
            <button type="button" className="btn btn-primary" onClick={handleAddAdmin}>Adicionar</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminModal;