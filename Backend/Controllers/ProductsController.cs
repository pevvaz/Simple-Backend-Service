using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

public class ProductsController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly ILogger<UsersController> _logger;
    private readonly UsersContext _usersContext;

    public ProductsController(IMemoryCache memory, ILogger<UsersController> logger, UsersContext usersContext)
    {
        _memory = memory;
        _logger = logger;
        _usersContext = usersContext;
    }

    // [HttpGet(template: "")]
}