namespace EfCore1.Files;

public interface IProductImageStorage
{
  Task<StoredProductImage> SaveAsync(string productCode, IFormFile file, CancellationToken ct);
}