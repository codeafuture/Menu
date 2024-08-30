using Microsoft.EntityFrameworkCore;
using Menu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Menu.Data
{
    public class MenuContext : IdentityDbContext<IdentityUser>
    {
        public MenuContext( DbContextOptions<MenuContext> options ) : base(options) 
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DishIngredient>().HasKey(di => new
            {
                di.DishId,
                di.IngredientId
            });
            modelBuilder.Entity<DishIngredient>().HasOne(d => d.Dish).WithMany(di => di.DishIngredients).HasForeignKey(d => d.DishId);
            modelBuilder.Entity<DishIngredient>().HasOne(i => i.Ingredient).WithMany(di => di.DishIngredients).HasForeignKey(i => i.IngredientId);


            modelBuilder.Entity<Dish>().HasData(
                new Dish { Id=1, Name= "Margheritta", Price= 7.50, Offer="30% OFF", ImageUrl = "https://cdn.shopify.com/s/files/1/0205/9582/articles/20220211142347-margherita-9920_ba86be55-674e-4f35-8094-2067ab41a671.jpg?crop=center&height=915&v=1644590192&width=1200" }
                );
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id= 1, Name="Tomato Sauce"},
                new Ingredient { Id = 2, Name = "Mozzarella" }
                );
            modelBuilder.Entity<DishIngredient>().HasData(
                new DishIngredient { DishId=1, IngredientId=1},
                new DishIngredient { DishId = 1, IngredientId = 2 }
                );
            modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRole>(
                 u => u.HasOne(ur => ur.Role).WithMany(),
                 r => r.HasOne(ur => ur.User).WithMany(),
                 je =>
                 {
                     je.HasKey(ur => new { ur.UserId, ur.RoleId });
                 });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Dish> Dishes { get; set; }  
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

    }
}
