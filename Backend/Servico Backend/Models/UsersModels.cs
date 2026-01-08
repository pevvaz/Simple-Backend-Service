using System.ComponentModel.DataAnnotations;

public class UsersModels
{
    public enum Roles
    {
        Admin,
        Customer
    }

    [Required] public int Id { get; set; }
    [Required] public Roles Role { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public string? Password { get; set; }
    [Required] public string? Email { get; set; }
}
