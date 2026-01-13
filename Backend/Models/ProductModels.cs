using System.ComponentModel.DataAnnotations;

public class ProductModel
{
    [Required] public int Id { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public int Price { get; set; }
}

public class CreateProductModel
{
    required public string Name { get; set; }
    required public int Price { get; set; }
}