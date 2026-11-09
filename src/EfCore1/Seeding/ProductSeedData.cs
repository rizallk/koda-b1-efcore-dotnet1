using EfCore1.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCore1.Seeding;

public static class ProductSeedData
{
  private static IEnumerable<Product> InitialProducts()
  {
    yield return new Product("P001", "Keyboard Mekanik", 35000m, 20, "Elektronik");
    yield return new Product("P002", "Mouse", 20000m, 21, "Mouse");
    yield return new Product("P003", "Monitor", 15000m, 22, "Screen");
    yield return new Product("P004", "CPU", 16000m, 23, "Screen1");
    yield return new Product("P005", "Laptop", 17000m, 24, "Screen2");
    yield return new Product("P006", "Handphone", 18000m, 25, "Screen3");
    yield return new Product("P007", "Camera", 19000m, 26, "Screen4");
    yield return new Product("P008", "Processor", 20000m, 27, "Screen5");
    yield return new Product("P009", "product1", 21000m, 28, "Screen6");
    yield return new Product("P010", "product2", 22000m, 29, "Screen7");
  }

  public static void Seed(DbContext context)
  {
    foreach (var product in InitialProducts())
    {
      bool exists = context.Set<Product>()
          .Any(p => p.ProductCode == product.ProductCode);

      if (!exists)
      {
        context.Set<Product>().Add(product);
      }
    }
    context.SaveChanges();
  }

  public static async Task SeedAsync(DbContext context, CancellationToken ct)
  {
    foreach (var product in InitialProducts())
    {
      bool exists = await context.Set<Product>().AnyAsync(p => p.ProductCode == product.ProductCode, ct);

      if (!exists)
      {
        context.Set<Product>().Add(product);
      }
    }
    await context.SaveChangesAsync(ct);
  }
}