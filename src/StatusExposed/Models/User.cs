using System.ComponentModel.DataAnnotations;

namespace StatusExposed.Models;

public class User
{
    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Permissions { get; init; } = new List<string>();

    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }
}