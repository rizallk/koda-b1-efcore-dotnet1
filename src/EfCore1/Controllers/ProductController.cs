using EfCore1.DTOs;
using EfCore1.Models;
using EfCore1.Services;
using Microsoft.AspNetCore.Mvc;

namespace EfCore1.Controllers;

[ApiController]
[Route("api/v1/products")]

public sealed class ProductController(IProductService service) : ControllerBase
{
  private readonly IProductService _service = service;

  [HttpGet]
  [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken cancellationToken)
  {
    var products = await _service.GetAllAsync(cancellationToken);
    return Ok(products);
  }

  [HttpGet("{productCode}")]
  [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<IReadOnlyList<Product>>> GetByCode([FromRoute] string productCode, CancellationToken cancellationToken)
  {
    var product = await _service.GetByCodeAsync(productCode, cancellationToken);

    return product is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found") : Ok(product);
  }

  [HttpPost]
  [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] ProductRequest req, CancellationToken cancellationToken)
  {
    try
    {
      var created = await _service.CreateAsync(req, cancellationToken);

      if (created is null)
      {
        return Problem(statusCode: StatusCodes.Status409Conflict, title: "Product code already exists");
      }

      return CreatedAtAction(actionName: nameof(GetByCode), routeValues: new { productCode = created.ProductCode }, value: created);
    }
    catch (ArgumentException e)
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid product data!", detail: e.Message);
    }

  }

  [HttpPut("{productCode}")]
  [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] string productCode, [FromBody] UpdateProductRequest req, CancellationToken cancellationToken)
  {
    try
    {
      var updated = await _service.UpdateAsync(productCode, req, cancellationToken);

      return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: $"No product was found for code {productCode}.") : Ok(updated);
    }
    catch (ArgumentException e)
    {
      return Problem(
             statusCode: StatusCodes.Status400BadRequest,
             title: "Invalid product data!",
             detail: e.Message
           );
    }
  }

  [HttpDelete("{productCode}")]
  [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete([FromRoute] string productCode, CancellationToken cancellationToken)
  {
    var deleted = await _service.DeleteAsync(productCode, cancellationToken);

    return deleted ? NoContent() : Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: $"No product was found for code {productCode}.");
  }
}