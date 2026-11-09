using EfCore1.Models;

namespace EfCore1.DTOs;

public sealed record ProductResponse(string ProductCode, string Name, decimal Price, int Stock, string Category)
{
  public static ProductResponse FromModel(Product product) => new(product.ProductCode, product.Name, product.Price, product.Stock, product.Category);
}