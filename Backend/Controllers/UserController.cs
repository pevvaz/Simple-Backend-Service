using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
public class UserController : ControllerBase
{
    private readonly IMemoryCache _memory;
    private readonly ILogger<UserController> _logger;
    private readonly UserContext _userContext;

    public UserController(IMemoryCache memory, ILogger<UserController> logger, UserContext userContext)
    {
        _memory = memory;
        _logger = logger;
        _userContext = userContext;
    }

    [HttpGet(template: "list/all")]
    public async Task<IActionResult> ListAllAction()
    {
        if (!_memory.TryGetValue("list_all", out List<UserModel>? usersList))
        {
            usersList = await _userContext.Users.AsNoTracking().ToListAsync();

            _memory.Set("list_all", usersList, TimeSpan.FromMinutes(1));
            _logger.LogInformation("### ListAll cached ###");
        }

        return Ok(usersList!);
    }

    [HttpGet(template: "list/all/{role?}")]
    public async Task<IActionResult> ListRoleAction([FromRoute] string? role = null)
    {
        if (String.IsNullOrEmpty(role))
        {
            return RedirectToAction(nameof(ListAllAction));
        }

        if (Enum.TryParse(role, true, out UserModel.RolesEnum parseRole))
        {
            if (!_memory.TryGetValue(role, out List<UserModel>? usersList))
            {
                usersList = await _userContext.Users.AsNoTracking().Where(u => u.Role == parseRole).ToListAsync();

                _memory.Set($"list_{parseRole}", usersList, TimeSpan.FromMinutes(1));
                _logger.LogInformation($"### List{parseRole} cached ###");
            }

            return Ok(usersList!);
        }
        else
        {
            return BadRequest($"The following Role doesn't exist: {role}");
        }
    }

    [HttpGet(template: "list/{id:int}")]
    public async Task<IActionResult> ListIdAction([FromRoute] int id)
    {
        if (!_memory.TryGetValue($"list_{id}", out UserModel? user))
        {
            try
            {
                user = await _userContext.Users.AsNoTracking().FirstAsync(u => u.Id == id);

                _memory.Set($"list_{id}", user, TimeSpan.FromMinutes(1));
                _logger.LogInformation("### ListId cached ###");
            }
            catch
            {
                return NotFound($"An User of Id: {id} was not found");
            }
        }

        return Ok(user!);
    }

    [HttpPost(template: "create/")]
    public async Task<IActionResult> CreateAction([FromBody] CreateUserModel user)
    {
        if (await _userContext.Users.AsNoTracking().AnyAsync(u => u.Password == user.Password || u.Email == user.Email))
        {
            return BadRequest("There's an already User with the same password or Email");
        }

        if (user != null && Enum.TryParse(user.Role, true, out UserModel.RolesEnum parseRole))
        {
            UserModel newUser = new UserModel
            {
                Role = parseRole,
                Name = user.Name,
                Password = user.Password,
                Email = user.Email,
            };
            await _userContext.Users.AddAsync(newUser);
            await _userContext.SaveChangesAsync();

            _memory.Remove("list_all");
            _memory.Remove($"list_{UserModel.RolesEnum.Admin}");
            _memory.Remove($"list_{UserModel.RolesEnum.Customer}");
            _logger.LogInformation("### Lists removed from cache ###");

            return CreatedAtAction(nameof(ListIdAction), new { id = newUser.Id }, newUser);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateAction([FromBody] UpdateUserModel newValues)
    {
        if (await _userContext.Users.AsNoTracking().AnyAsync(u => u.Id != newValues.Id && (u.Password == newValues.Password || u.Email == newValues.Email)))
        {
            return BadRequest("There's an already User with the same password or Email");
        }

        try
        {
            var userToBeUpdated = await _userContext.Users.FirstAsync(u => u.Id == newValues.Id);

            if (userToBeUpdated == null)
            {
                return NotFound($"An User of Id: {newValues.Id} was not found");
            }

            if (!String.IsNullOrEmpty(newValues.Role))
            {
                if (Enum.TryParse(newValues.Role, true, out UserModel.RolesEnum parseEnum))
                {
                    userToBeUpdated.Role = parseEnum;
                }
                else
                {
                    return BadRequest($"The following Role doesn't exist: {newValues.Role}");
                }
            }
            if (!String.IsNullOrEmpty(newValues.Name))
            {
                userToBeUpdated.Name = newValues.Name;
            }
            if (!String.IsNullOrEmpty(newValues.Password))
            {
                userToBeUpdated.Password = newValues.Password;
            }
            if (!String.IsNullOrEmpty(newValues.Email))
            {
                userToBeUpdated.Email = newValues.Email;
            }

            await _userContext.SaveChangesAsync();

            _memory.Remove("list_all");
            _memory.Remove($"list_{UserModel.RolesEnum.Admin}");
            _memory.Remove($"list_{UserModel.RolesEnum.Customer}");
            _memory.Remove($"list_{userToBeUpdated.Id}");
            _logger.LogInformation($"### Lists and User:{userToBeUpdated.Id} removed from cache ###");

            return NoContent();
        }
        catch
        {
            return NotFound($"An User of Id: {newValues.Id} was not found");
        }
    }

    [HttpDelete(template: "delete/{id:int}")]
    public async Task<IActionResult> DeleteAction([FromRoute] int id)
    {
        try
        {
            var user = await _userContext.Users.FirstAsync(u => u.Id == id);
            _userContext.Users.Remove(user);
            await _userContext.SaveChangesAsync();

            _memory.Remove("list_all");
            _memory.Remove($"list_{UserModel.RolesEnum.Admin}");
            _memory.Remove($"list_{UserModel.RolesEnum.Customer}");
            _memory.Remove($"list_{user.Id}");
            _logger.LogInformation($"### Lists and User:{user.Id} removed from cache ###");
        }
        catch
        {
            return NotFound($"An User of Id: {id} was not found");
        }

        return NoContent();
    }
}