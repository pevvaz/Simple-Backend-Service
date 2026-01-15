using System.ComponentModel.DataAnnotations;

public class UserModel
{
    public enum RolesEnum
    {
        Admin,
        Customer
    }

    [Required] public int Id { get; set; }
    [Required] public required RolesEnum Role { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string Email { get; set; }
}

public class CreateUserModel
{
    public required string Role { get; set; }
    public required string Name { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public class UpdateUserModel
{
    public required int Id { get; set; }
    public string? Role { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}

public class SignInModel
{
    public required string NameEmail { get; set; }
    public required string Password { get; set; }
}