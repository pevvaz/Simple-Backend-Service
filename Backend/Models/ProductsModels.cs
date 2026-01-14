using System.ComponentModel.DataAnnotations;

public class ProductsModels
{
    public enum ProductTypeEnum
    {
        Food,
        Electronic,
        Toy,
        Mobile,
        Art
    }

    [Required] public int Id { get; set; }
    [Required] public required ProductTypeEnum Type { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required float Price { get; set; }
}

public class CreateProductModel
{
    public required string Type { get; set; }
    public required string Name { get; set; }
    public required int Price { get; set; }
}

public class UpdateProductModel
{
    public required int Id { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
}