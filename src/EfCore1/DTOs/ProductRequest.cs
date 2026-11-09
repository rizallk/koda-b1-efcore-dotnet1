using System.ComponentModel.DataAnnotations;
using EfCore1.Models;

namespace EfCore1.DTOs;

public sealed record ProductRequest(
  [Required, StringLength(30)]
  string ProductCode,

  [Required]
  string Name,

  [Required]
  [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
  decimal Price,

  [Required]
  [Range(0, int.MaxValue)]
  int Stock,

  [Required, StringLength(120)]
  string Category
);