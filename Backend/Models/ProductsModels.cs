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
    required public string Type { get; set; }
    required public string Name { get; set; }
    required public int Price { get; set; }
}