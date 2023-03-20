using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
  public class Item
  {
    [Required(ErrorMessage = "The item's description can't be empty!")]
    public string Description { get; set; }
    public int ItemId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "You must add your item to a category. Have you created a category yet?")]
    public int CategoryId { get; set;}
    public DateTime DueDate { get; set; }
    public Boolean Completed { get; set; } = false;  
    public Category Category { get; set; }
    public ApplicationUser User { get; set; }
    public List<ItemTag> JoinEntities { get; }
  }
}
