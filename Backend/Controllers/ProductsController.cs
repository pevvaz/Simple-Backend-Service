using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductsContext _productsContext;

    public ProductsController(IMemoryCache memory, ILogger<ProductsController> logger, ProductsContext productsContext)
    {
        _memory = memory;
        _logger = logger;
        _productsContext = productsContext;
    }

    [HttpGet(template: "list/all/{type?}")]
    public async Task<IActionResult> ListProductsAction([FromRoute] string? type)
    {
        if (String.IsNullOrEmpty(type))
        {
            _logger.LogInformation("#### No product Type informed ####");

            var list = await _productsContext.Products.AsNoTracking().ToListAsync();

            return Ok(list);
        }
        else
        {
            if (Enum.TryParse(type, true, out ProductsModels.ProductTypeEnum parseType))
            {
                var list = await _productsContext.Products.AsNoTracking().Where(p => p.Type == parseType).ToListAsync();

                return Ok(list);
            }
            else
            {
                _logger.LogInformation("#### Error on parse ####");

                return BadRequest($"The following product Type doesn't exist: {type}");
            }
        }
    }

    [HttpGet(template: "list/{id:int}")]
    public async Task<IActionResult> ListProductsIdAction([FromRoute] int id)
    {
        try
        {
            var product = await _productsContext.Products.AsNoTracking().FirstAsync(p => p.Id == id);

            return Ok(product);
        }
        catch
        {
            return NotFound($"A Product of Id: {id} was not found");
        }
    }

    [HttpPost(template: "create/")]
    public async Task<IActionResult> CreateProductAction([FromBody] CreateProductModel product)
    {
        if (product != null && Enum.TryParse(product.Type, true, out ProductsModels.ProductTypeEnum parseType))
        {
            ProductsModels newProduct = new ProductsModels
            {
                Type = parseType,
                Name = product.Name,
                Price = product.Price,
            };
            await _productsContext.Products.AddAsync(newProduct);
            await _productsContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ListProductsIdAction), new { id = newProduct.Id }, newProduct);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPut(template: "update/")]
    public async Task<IActionResult> UpdateProductAction([FromBody] UpdateProductModel updatedProduct)
    {
        try
        {
            var product = await _productsContext.Products.FirstAsync(p => p.Id == updatedProduct.Id);

            if (!String.IsNullOrEmpty(updatedProduct.Name))
            {

            }
            if (!String.IsNullOrEmpty(updatedProduct.Name))
            {

            }
            if (!String.IsNullOrEmpty(updatedProduct.Name))
            {

            }

            await _productsContext.SaveChangesAsync();

            return NoContent();
        }
        catch
        {
            return NotFound($"A Product of Id: {updatedProduct.Id} was not found");
        }
    }

    [HttpDelete(template: "delete/{id:int}")]
    public async Task<IActionResult> DeleteProductAction([FromRoute] int id)
    {
        try
        {
            var product = await _productsContext.Products.FirstAsync(p => p.Id == id);
            _productsContext.Products.Remove(product);
            await _productsContext.SaveChangesAsync();
        }
        catch
        {
            return NotFound($"A Product of Id: {id} was not found");
        }

        return NoContent();
    }
}