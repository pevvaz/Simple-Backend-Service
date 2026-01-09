using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[Controller]
[Route("service/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly UsersContext _usersContext;

    public UsersController(IMemoryCache memory, UsersContext usersContext)
    {
        _memory = memory;
        _usersContext = usersContext;
    }

    [HttpGet(template: "{role?}/list/all")]
    public async Task<IActionResult> ListUsers([FromRoute] int? id, [FromRoute] string role = "customer")
    {
        if (Enum.TryParse(role, true, out UsersModels.Roles parseRole))
        {
            var list = await _usersContext.Users.AsNoTracking().Where(u => u.Role == parseRole).ToListAsync();
            return Ok(list);
        }
        else
        {
            return BadRequest($"The following Role doesn't exist: {role}");
        }
    }
    [HttpGet(template: "{role}/list/{id:int?}")]
    public async Task<IActionResult> ListUsersId([FromRoute] int id, [FromRoute] string role = "customer")
    {
        if (Enum.TryParse(role, true, out UsersModels.Roles parseRole))
        {
            var user = await _usersContext.Users.AsNoTracking().FirstAsync(u => u.Role == parseRole && u.Id == id);

            if (user == null)
            {
                return NotFound($"An User of Id: {id} and Role: {role} doesn't exist.");
            }

            return Ok(user);
        }
        else
        {
            return BadRequest($"The following Role doesn't exist: {role}");
        }
    }
}