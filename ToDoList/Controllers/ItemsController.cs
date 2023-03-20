using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;


namespace ToDoList.Controllers
{
  [Authorize]
  // ^^ This allows access to the ItemsController only if a user is logged in. We'll add this attribute to a controller whenever we want to limit its access to signed-in users only. We can also add the [Authorize] attribute to individual controller actions. When we add [Authorize] to the ItemsController, the entirety of the controller is shielded from unauthorized users. We can negate this by including an [AllowAnonymous] attribute above any specific methods that we want unauthorized users to have access to. For example, we could put [AllowAnonymous] above the Index route, if we want users to be able to see a list of items, but require authorization before they view details.
  public class ItemsController : Controller
  {

    private readonly ToDoListContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public ItemsController(UserManager<ApplicationUser> userManager, ToDoListContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index(/*string sortBy*/)
    {

      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // ^^we locate the unique identifier for the currently-logged-in user and assign it the variable name userId.
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
      List<Item> userItems = _db.Items
                          .Where(entry => entry.User.Id == currentUser.Id)
                          .Include(item => item.Category)
                          .ToList();
      return View(userItems);
      // List<Item> model = null;
      // if (sortBy ==null)
      // {
      //   model = _db.Items.Include(item => item.Category).ToList();
      // } 
      // else if (sortBy.Equals("date"))
      // {
      //   model = _db.Items.OrderBy(item => item.DueDate).Include(item => item.Category).ToList();
      // }
      // return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Item item, int CategoryId)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
        return View(item);
      }
      else
      {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        item.User = currentUser;
        _db.Items.Add(item);
        _db.SaveChanges();
        return RedirectToAction("Index");
      }
    }

    public ActionResult Details(int id)
    {
      Item thisItem = _db.Items
          .Include(item => item.Category)
          .Include(item => item.JoinEntities)
          .ThenInclude(join => join.Tag)          
          .FirstOrDefault(item => item.ItemId == id);
      return View(thisItem);
    }

    public ActionResult Edit(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult Edit(Item item)
    {
      _db.Items.Update(item);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      return View(thisItem);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == id);
      _db.Items.Remove(thisItem);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

        public ActionResult AddTag(int id)
    {
      Item thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Title");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult AddTag(Item item, int tagId)
    {
      #nullable enable
      ItemTag? joinEntity = _db.ItemTags.FirstOrDefault(join => (join.TagId == tagId && join.ItemId == item.ItemId));
      #nullable disable
      if (joinEntity == null && tagId != 0)
      {
        _db.ItemTags.Add(new ItemTag() { TagId = tagId, ItemId = item.ItemId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = item.ItemId });
    }  
    
    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      ItemTag joinEntry = _db.ItemTags.FirstOrDefault(entry => entry.ItemTagId == joinId);
      _db.ItemTags.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost, ActionName("MarkComplete")]
    public ActionResult MarkComplete(Boolean Completed, int Id)
    {
      Item thisItem = _db.Items.FirstOrDefault(item => item.ItemId == Id);
      thisItem.Completed = Completed;
      _db.Items.Update(thisItem);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

  }
}