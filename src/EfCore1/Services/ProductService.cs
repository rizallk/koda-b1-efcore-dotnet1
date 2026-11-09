using EfCore1.DTOs;
using EfCore1.Models;
using EfCore1.Repositories;

namespace EfCore1.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
  private readonly IProductRepository _repository = repository;

  public async Task<ProductResponse?> CreateAsync(ProductRequest req, CancellationToken cancellationToken)
  {
    var product = new Product(req.ProductCode, req.Name, req.Price, req.Stock, req.Category);

    var created = await _repository.TryAddAsync(product, cancellationToken);

    return created ? ProductResponse.FromModel(product) : null;
  }

  public Task<bool> DeleteAsync(string productCode, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(productCode) || productCode.Trim().Length > 30)
    {
      return Task.FromResult(false);
    }
    return _repository.TryDeleteAsync(productCode, cancellationToken);
  }

  public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
  {
    var products = await _repository.GetAllAsync(cancellationToken);

    return products.Select(ProductResponse.FromModel).ToArray();
  }

  public async Task<ProductResponse?> GetByCodeAsync(string productCode, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(productCode) || productCode.Trim().Length > 30)
    {
      return null;
    }

    var product = await _repository.GetByCodeAsync(productCode, cancellationToken);

    return product is null ? null : ProductResponse.FromModel(product);
  }

  public async Task<ProductResponse?> UpdateAsync(string productCode, UpdateProductRequest req, CancellationToken cancellationToken)
  {
    var product = new Product(productCode, req.Name, req.Price, req.Stock, req.Category);

    var updated = await _repository.TryUpdateAsync(product, cancellationToken);

    return updated ? ProductResponse.FromModel(product) : null;
  }
}