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

    [HttpGet(template: "/list/all")]
    public async Task<IActionResult> ListAllUsersAction()
    {
        var list = await _usersContext.Users.AsNoTracking().ToListAsync();

        return Ok();
    }

    [HttpGet(template: "/list/all/{role?}")]
    public async Task<IActionResult> ListUsersAction([FromRoute] int? id, [FromRoute] string role = "customer")
    {
        if (role == null)
        {
            return RedirectToAction(nameof(ListAllUsersAction));
        }
        else
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

    }

    [HttpGet(template: "/list/{id:int}")]
    public async Task<IActionResult> ListUsersIdAction([FromRoute] int id, [FromRoute] string role = "customer")
    {
        var user = await _usersContext.Users.AsNoTracking().FirstAsync(u => u.Id == id);

        if (user != null)
        {
            return Ok(user);
        }
        else
        {
            return NotFound($"An User of Id: {id} was not found");
        }
    }

    [HttpPost(template: "/create/")]
    public async Task<IActionResult> CreateAction([FromBody] UsersModels user)
    {
        if (user != null)
        {
            await _usersContext.Users.AddAsync(user);
            await _usersContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ListUsersIdAction), new { id = user.Id }, user);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpDelete(template: "/delete/{id:int}")]
    public async Task<IActionResult> DeleteAction([FromRoute] int id)
    {
        var user = await _usersContext.Users.FirstAsync(u => u.Id == id);
        _usersContext.Users.Remove(user);
        await _usersContext.SaveChangesAsync();

        return Ok();
    }
}