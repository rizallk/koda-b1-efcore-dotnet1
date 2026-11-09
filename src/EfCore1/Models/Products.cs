namespace EfCore1
.Models;

public sealed class Product
{
  public string ProductCode { get; private set; }
  public string Name { get; private set; }
  public decimal Price { get; private set; }
  public int Stock { get; private set; }
  public string Category { get; private set; }

  public Product(string productCode, string name, decimal price, int stock, string category)
  {
    if (string.IsNullOrWhiteSpace(productCode))
    {
      throw new ArgumentException("Product code is required", nameof(productCode));
    }
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Product code is required", nameof(name));
    }
    if (price < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
    }
    if (stock < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative");
    }
    if (string.IsNullOrWhiteSpace(category))
    {
      throw new ArgumentException("Category code is required", nameof(category));
    }
    ProductCode = productCode;
    Name = name;
    Price = price;
    Stock = stock;
    Category = category.Trim();
  }

  public void UpdateDetail(string name, decimal price, int stock, string category)
  {
    var validated = new Product(ProductCode, name, price, stock, category);

    Name = validated.Name;
    Price = validated.Price;
    Stock = validated.Stock;
    Category = validated.Category;
  }
}