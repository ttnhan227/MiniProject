using System.ComponentModel.DataAnnotations;

namespace Client.Models;

public class AccountModel
{
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Fullname { get; set; }
    public float TotalAmount { get; set; }
    
    public virtual ICollection<Cart> Carts { get; set; }
    
}