namespace VirtualShopMinimalAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public bool IsAdmin { get; set; }
    }
}