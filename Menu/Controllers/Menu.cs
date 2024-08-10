using Microsoft.AspNetCore.Mvc;
using Menu.Data;
using Menu.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Menu.Controllers
{
    public class Menu : Controller
    {
        private readonly MenuContext _context;
        public Menu(MenuContext context) 
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var dishes = from d in _context.Dishes
                       select d;
            if(!string.IsNullOrEmpty(searchString))
            {
                dishes = dishes.Where(d => d.Name.Contains(searchString));
                return View(await dishes.ToListAsync());
            }
            return View( await dishes.ToListAsync());
        }

        public async Task<IActionResult> Details (int? id)
        {
            var dish = await _context.Dishes
                .Include(di => di.DishIngredients)
                .ThenInclude(i => i.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dish == null) 
            {
                return NotFound();
            }
            return View(dish);
        }

        // GET: Dishes/Create or Dishes/Edit/5
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int? id)
        {
            if (id == null)
            {
                // Creating a new dish
                ViewData["Ingredients"] = new SelectList(await _context.Ingredients.ToListAsync(), "Id", "Name");
                return View(new Dish { DishIngredients = new List<DishIngredient>() });
            }

            // Editing an existing dish
            var dish = await _context.Dishes
                                     .Include(d => d.DishIngredients)
                                         .ThenInclude(di => di.Ingredient)
                                     .FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null)
            {
                return NotFound();
            }

            ViewData["Ingredients"] = new SelectList(await _context.Ingredients.ToListAsync(), "Id", "Name");
            return View(dish);
        }

        // POST: Dishes/CreateOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Dish dish, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                if (dish.Id == 0)
                {
                    // Create new dish
                    _context.Dishes.Add(dish);
                    await _context.SaveChangesAsync(); // Save to get the generated Dish Id

                    if (selectedIngredients != null)
                    {
                        foreach (var ingredientId in selectedIngredients)
                        {
                            _context.DishIngredients.Add(new DishIngredient
                            {
                                DishId = dish.Id,
                                IngredientId = ingredientId
                            });
                        }
                    }
                }
                else
                {
                    // Update existing dish
                    var existingDish = await _context.Dishes
                        .Include(d => d.DishIngredients)
                        .FirstOrDefaultAsync(d => d.Id == dish.Id);

                    if (existingDish == null)
                    {
                        return NotFound();
                    }

                    // Update the main dish properties
                    _context.Entry(existingDish).CurrentValues.SetValues(dish);

                    // Update ingredients
                    var currentIngredients = existingDish.DishIngredients.Select(di => di.IngredientId).ToList();
                    var selectedIngredientIds = selectedIngredients.ToList();

                    // Remove ingredients not in the selected list
                    var ingredientsToRemove = existingDish.DishIngredients
                        .Where(di => !selectedIngredientIds.Contains(di.IngredientId))
                        .ToList();

                    _context.DishIngredients.RemoveRange(ingredientsToRemove);

                    // Add new ingredients
                    foreach (var ingredientId in selectedIngredientIds.Except(currentIngredients))
                    {
                        _context.DishIngredients.Add(new DishIngredient
                        {
                            DishId = dish.Id,
                            IngredientId = ingredientId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
                ViewData["Ingredients"] = new SelectList(await _context.Ingredients.ToListAsync(), "Id", "Name");
            return View(dish);
        }

        // GET: Dishes/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }
            
            return View(dish);
        }

            // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> CreateUser(User user, int[] selectedRoles)
        {
            if (ModelState.IsValid)
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }
            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
            return View(user);
        }

    }
}
