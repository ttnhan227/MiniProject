using System.ComponentModel.DataAnnotations.Schema;

namespace Client.Models;

public class Cart
{
    public string UserName { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    
    [ForeignKey("UserName")]
    public virtual AccountModel? AccountModel { get; set; }
   
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}