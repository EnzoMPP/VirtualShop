namespace VirtualShopMinimalAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DataCompra { get; set; } 
    }
}