namespace Server.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string? Description { get; set; }
     public virtual ICollection<Cart>? Carts { get; set; }

    //public string ImageUrl { get; set; }//

}
