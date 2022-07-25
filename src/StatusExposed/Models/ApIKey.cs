using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusExposed.Models;

public class ApiKey
{
    public ApiKey(string key)
    {
        Key = key;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Key { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is ApiKey apiKey)
        {
            return Key == apiKey.Key;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}