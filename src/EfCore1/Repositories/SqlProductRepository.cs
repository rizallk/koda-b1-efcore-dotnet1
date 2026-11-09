using System.Data;
using EfCore1.Models;
using Microsoft.Data.SqlClient;

namespace EfCore1.Repositories;

public sealed class SqlProductRepository(SqlConnection connection) : IProductRepository
{
  public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT ProductCode, Name, Price, Stock, Category FROM dbo.Product ORDER BY ProductCode DESC;
    """;

    var products = new List<Product>();

    if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

    using var command = new SqlCommand(sql, connection);
    using var reader = await command.ExecuteReaderAsync(cancellationToken);

    while (await reader.ReadAsync(cancellationToken))
    {
      products.Add(ReadProduct(reader));
    }

    return products;
  }

  public async Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT ProductCode, Name, Price, Stock, Category FROM dbo.Product WHERE ProductCode = @productCode;
    """;

    if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, connection);

    AddProductCode(command, productCode);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return ReadProduct(reader);
  }

  public async Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken)
  {
    const string sql = """
      INSERT INTO dbo.Product(ProductCode, Name, Price, Stock, Category) VALUES (@productCode, @name, @price, @stock, @category);
    """;

    if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, connection);

    AddProductParameter(command, product);

    try
    {
      var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

      return affectedRows == 1;
    }
    catch (SqlException e) when (e.Number is 2601 or 2627)
    {
      return false;
    }
  }

  public async Task<bool> TryDeleteAsync(string productCode, CancellationToken cancellationToken)
  {
    const string sql = """
      DELETE FROM dbo.Product WHERE ProductCode = @productCode;
    """;

    if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, connection);

    AddProductCode(command, productCode);

    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  public async Task<bool> TryUpdateAsync(Product product, CancellationToken cancellationToken)
  {
    const string sql = """
      UPDATE dbo.Product SET
        Name = @name,
        Price = @price,
        Stock = @stock,
        Category = @category
      WHERE ProductCode = @productCode;
    """;

    if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, connection);

    AddProductParameter(command, product);

    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  // ============= METHOD

  private static void AddProductParameter(SqlCommand command, Product product)
  {
    AddProductCode(command, product.ProductCode);

    command.Parameters.Add("@name", SqlDbType.NVarChar, 120).Value = product.Name;

    command.Parameters.Add("@stock", SqlDbType.Int).Value = product.Stock;

    var price = command.Parameters.Add("@price", SqlDbType.Decimal);

    command.Parameters.Add("@category", SqlDbType.NVarChar, 80).Value = product.Category;

    price.Precision = 10;
    price.Scale = 2;
    price.Value = product.Price;
  }

  private static void AddProductCode(SqlCommand command, string productCode)
  {
    command.Parameters.Add("@productCode", SqlDbType.NVarChar, 30).Value = productCode.Trim().ToUpperInvariant();
  }

  private static Product ReadProduct(SqlDataReader reader)
  {
    return new Product(
      reader.GetString(0),
      reader.GetString(1),
      reader.GetDecimal(2),
      reader.GetInt32(3),
      reader.GetString(4)
    );
  }
}