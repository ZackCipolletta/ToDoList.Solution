using System.Collections.Generic;

namespace ToDoList.Models
{
  public class Item
  {
    public string Description { get; set; }
    public int ItemId { get; set; }
    public int CategoryId { get; set;}
    public DateTime DueDate { get; set; }
    public Boolean Completed { get; set; } = false;  
    public Category Category { get; set; }
    public List<ItemTag> JoinEntities { get; }
  }
}
