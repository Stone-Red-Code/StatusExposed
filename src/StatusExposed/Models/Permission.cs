using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusExposed.Models;

public class Permission
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public Permission(string name)
    {
        if (name.Contains(' '))
        {
            throw new ArgumentException("Permissions are not allowed to have spaces in them!");
        }

        Name = name;
    }

    public static implicit operator Permission(string permissionName)
    {
        return new Permission(permissionName);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Permission permission)
        {
            return Name == permission.Name;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}