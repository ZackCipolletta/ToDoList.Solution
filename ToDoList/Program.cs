using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Models;
using Microsoft.AspNetCore.Identity;

namespace ToDoList
{
  class Program
  {
    static void Main(string[] args)
    {
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

      builder.Services.AddControllersWithViews();

      builder.Services.AddDbContext<ToDoListContext>(
        dbContextOptions => dbContextOptions
          .UseMySql(
            builder.Configuration["ConnectionStrings:DefaultConnection"], ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:DefaultConnection"]
          )
        )
      );

      builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
      // ^^ We specify <ApplicationUser, IdentityRole> — these are the two models that we're using to designate the user and the role. Just like IdentityUser, IdentityRole is a built-in class to Identity, and it allows us to use the default configurations for roles. We won't be configuring roles beyond the defaults, so we use the built-in IdentityRole class here.
                  .AddEntityFrameworkStores<ToDoListContext>()
                  .AddDefaultTokenProviders();
                  // ^^ The first method ensures that the Identity user data is saved via EF Core to our database (as represented by the ToDoListContext class). The second method sets up Identity's providers for tokens, which are created during password reset or two factor authentication, for example.

      builder.Services.Configure<IdentityOptions>(options =>
      {
        // Default Password settings.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 0;
        options.Password.RequiredUniqueChars = 0;
      }); // ^^ overrides all our password requirements, which is helpful while we are in development so we can create accounts to experiment without having to type out long complicated passwords again and again.

      WebApplication app = builder.Build(); 

      //   app.UseDeveloperExceptionPage();
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();
      // ^^ The order in which we set up the middleware matters! If these methods are called in the wrong order, you may run into unhandled exceptions or issues logging in with Identity. Fortunately, the Microsoft Docs has a list of how middleware should be ordered.

      app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
      );

      app.Run();
    }
  }
}