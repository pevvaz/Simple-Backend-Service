using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly ILogger<UsersController> _logger;
    private readonly UsersContext _usersContext;

    public UsersController(IMemoryCache memory, ILogger<UsersController> logger, UsersContext usersContext)
    {
        _memory = memory;
        _logger = logger;
        _usersContext = usersContext;
    }

    [HttpGet(template: "list/all/{role?}")]
    public async Task<IActionResult> ListUsersAction([FromRoute] string? role = null)
    {
        if (String.IsNullOrEmpty(role))
        {
            _logger.LogInformation("#### No user Role informed ####");

            var list = await _usersContext.Users.AsNoTracking().ToListAsync();

            return Ok(list);
        }
        else
        {
            if (Enum.TryParse(role, true, out UsersModels.RolesEnum parseRole))
            {
                var list = await _usersContext.Users.AsNoTracking().Where(u => u.Role == parseRole).ToListAsync();

                return Ok(list);
            }
            else
            {
                _logger.LogInformation("#### Error on parse ####");

                return BadRequest($"The following Role doesn't exist: {role}");
            }
        }
    }

    [HttpGet(template: "list/{id:int}")]
    public async Task<IActionResult> ListUsersIdAction([FromRoute] int id)
    {
        try
        {
            var user = await _usersContext.Users.AsNoTracking().FirstAsync(u => u.Id == id);

            return Ok(user);
        }
        catch
        {
            return NotFound($"An User of Id: {id} was not found");
        }
    }

    [HttpPost(template: "create/")]
    public async Task<IActionResult> CreateUserAction([FromBody] CreateUserModel user)
    {
        if (user != null && Enum.TryParse(user.Role, true, out UsersModels.RolesEnum parseRole))
        {
            UsersModels newUser = new UsersModels
            {
                Role = parseRole,
                Name = user.Name,
                Password = user.Password,
                Email = user.Email,
            };
            await _usersContext.Users.AddAsync(newUser);
            await _usersContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ListUsersIdAction), new { id = newUser.Id }, newUser);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpDelete(template: "delete/{id:int}")]
    public async Task<IActionResult> DeleteUserAction([FromRoute] int id)
    {
        try
        {
            var user = await _usersContext.Users.FirstAsync(u => u.Id == id);
            _usersContext.Users.Remove(user);
            await _usersContext.SaveChangesAsync();
        }
        catch
        {
            return NotFound($"An User of Id: {id} was not found");
        }

        return NoContent();
    }
}