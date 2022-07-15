using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusExposed.Models;

public class User
{
    public User(string email)
    {
        Email = email;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public string Email { get; set; }
    public string? SessionToken { get; set; }
    public bool IsVerified { get; set; }
    public DateTime LastLoginDate { get; set; }

    public List<Permission> Permissions { get; init; } = new List<Permission>();

    public bool HasPermission(Permission permission)
    {
        return Permissions.Contains(permission);
    }
}