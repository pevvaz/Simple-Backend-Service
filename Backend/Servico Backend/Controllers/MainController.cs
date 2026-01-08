using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[Controller]
[Route("service/users")]
public class MainController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly UsersContext _usersContext;

    public MainController(IMemoryCache memory, UsersContext usersContext)
    {
        _memory = memory;
        _usersContext = usersContext;
    }

    [HttpGet(template: "{role}/list/all")]
    [Route(template: "{role}/list/{id}")]
    public async Task<IActionResult> ListUsers([FromRoute] string role, [FromRoute] int? id)
    {
        if (role == "admin")
        {
            if (id == null || id == 0)
            {
                var list = await _usersContext.Users.AsNoTracking().Where(b => b.Role == UsersModels.Roles.Admin).ToListAsync();
                return Ok(list);
            }
            else
            {
                var adminFound = await _usersContext.Users.AsNoTracking().FirstAsync(b => b.Role == UsersModels.Roles.Admin && b.Id == id);
                return Ok(adminFound);
            }
        }
        else if (role == "customer")
        {
            if (id == null || id == 0)
            {
                var list = await _usersContext.Users.AsNoTracking().Where(b => b.Role == UsersModels.Roles.Customer).ToListAsync();
                return Ok(list);
            }
            else
            {
                var customerFound = await _usersContext.Users.AsNoTracking().FirstAsync(b => b.Role == UsersModels.Roles.Customer && b.Id == id);
                return Ok(customerFound);
            }
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet(template: "get/{id}")]
    [HttpPost(template: "create/")]
}