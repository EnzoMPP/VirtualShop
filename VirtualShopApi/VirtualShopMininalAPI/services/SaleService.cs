using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VirtualShopMinimalAPI.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _db;

        public SaleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> RegisterSale(SaleRequest saleRequest)
        {
            if (_db.Sales == null || _db.Products == null)
            {
                return Results.NotFound("Sales or Products collection is null.");
            }

            if (saleRequest.ProductIds.Length != saleRequest.Quantidades.Length)
            {
                return Results.BadRequest("Product IDs and quantities must have the same length.");
            }

            var sale = new Sale
            {
                DataVenda = DateTime.Now,
                UserId = saleRequest.Sale.UserId,
                SaleProducts = new List<SaleProduct>()
            };

            for (int i = 0; i < saleRequest.ProductIds.Length; i++)
            {
                var productId = saleRequest.ProductIds[i];
                var quantidade = saleRequest.Quantidades[i];

                var product = await _db.Products.FindAsync(productId);
                if (product == null)
                {
                    return Results.NotFound($"Product with ID {productId} not found.");
                }

                sale.SaleProducts.Add(new SaleProduct { ProductId = productId, Quantidade = quantidade });
            }

            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();
            return Results.Ok(sale);
        }
    }
}