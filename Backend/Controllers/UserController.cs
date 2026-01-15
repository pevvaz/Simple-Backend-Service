using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route(template: "[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memory;
    private readonly ILogger<UserController> _logger;
    private readonly UserContext _userContext;

    public UserController(IConfiguration configuration, IMemoryCache memory, ILogger<UserController> logger, UserContext userContext)
    {
        _configuration = configuration;
        _memory = memory;
        _logger = logger;
        _userContext = userContext;
    }

    [HttpPost(template: "signin")]
    public async Task<IActionResult> SignInAction([FromBody] SignInModel data)
    {
        try
        {
            var user = await _userContext.Users.AsNoTracking().FirstAsync(u => (u.Name == data.NameEmail || u.Email == data.NameEmail) && u.Password == data.Password);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _configuration["JwtSettings:Issuer"]!,
                Audience = user.Role.ToString().ToLower(),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, user.Role.ToString().ToLower()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Hash, Encoding.UTF8.GetHashCode().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return Ok(token);
        }
        catch
        {
            return NotFound("Wrong informations, try again");
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet(template: "list/all")]
    public async Task<IActionResult> ListAllAction()
    {
        if (!_memory.TryGetValue("List_All", out List<UserModel>? usersList))
        {
            usersList = await _userContext.Users.AsNoTracking().ToListAsync();

            _memory.Set("List_All", usersList, TimeSpan.FromMinutes(1));
            _logger.LogInformation("### List_All cached ###");
        }

        return Ok(usersList!);
    }

    [Authorize(Roles = "admin")]
    [HttpGet(template: "list/admin")]
    public async Task<IActionResult> ListAdminAction()
    {
        if (!_memory.TryGetValue("List_Admin", out List<UserModel>? list))
        {
            list = await _userContext.Users.AsNoTracking().Where(u => u.Role == UserModel.RolesEnum.Admin).ToListAsync();

            _memory.Set("List_Admin", list);
            _logger.LogInformation("### List_Admin cached ###");
        }

        return Ok(list);
    }

    [Authorize(Roles = "admin, customer")]
    [HttpGet(template: "list/customer")]
    public async Task<IActionResult> ListCustomerAction()
    {
        if (!_memory.TryGetValue("List_Customer", out List<UserModel>? list))
        {
            list = await _userContext.Users.AsNoTracking().Where(u => u.Role == UserModel.RolesEnum.Customer).ToListAsync();

            _memory.Set("List_Customer", list);
            _logger.LogInformation("### List_Customer cached ###");
        }

        return Ok(list);
    }

    [Authorize(Roles = "admin")]
    [HttpGet(template: "list/{id:int}")]
    public async Task<IActionResult> ListIdAction([FromRoute] int id)
    {
        if (!_memory.TryGetValue($"List_Id:{id}", out UserModel? user))
        {
            try
            {
                user = await _userContext.Users.AsNoTracking().FirstAsync(u => u.Id == id);

                _memory.Set($"List_Id:{id}", user, TimeSpan.FromMinutes(1));
                _logger.LogInformation($"### List_Id:{id} cached ###");
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

        if (Enum.TryParse(user.Role, true, out UserModel.RolesEnum parseRole))
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

            _memory.Remove("List_All");
            _memory.Remove("List_Admin");
            _memory.Remove("List_Customer");
            _logger.LogInformation("### Lists removed from cache ###");

            return CreatedAtAction(nameof(ListIdAction), new { id = newUser.Id }, newUser);
        }
        else if (user != null)
        {
            return BadRequest("The User Credentials doesn't fulfill or match the requirements");
        }
        else
        {
            return BadRequest("User object null");
        }
    }

    [Authorize(Roles = "admin")]
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

            _memory.Remove("List_All");
            _memory.Remove("List_Admin");
            _memory.Remove("List_Customer");
            _memory.Remove($"List_Id:{userToBeUpdated.Id}");
            _logger.LogInformation($"### Lists and User:{userToBeUpdated.Id} removed from cache ###");

            return NoContent();
        }
        catch
        {
            return NotFound($"An User of Id: {newValues.Id} was not found");
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete(template: "delete/{id:int}")]
    public async Task<IActionResult> DeleteAction([FromRoute] int id)
    {
        try
        {
            var user = await _userContext.Users.FirstAsync(u => u.Id == id);
            _userContext.Users.Remove(user);
            await _userContext.SaveChangesAsync();

            _memory.Remove("List_All");
            _memory.Remove("List_Admin");
            _memory.Remove("List_Customer");
            _memory.Remove($"List_Id:{user.Id}");
            _logger.LogInformation($"### Lists and User:{user.Id} removed from cache ###");
        }
        catch
        {
            return NotFound($"An User of Id: {id} was not found");
        }

        return NoContent();
    }
}