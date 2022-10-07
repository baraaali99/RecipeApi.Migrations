using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApi.Migrations.Seed;

    public class Seed
    {
    /////
    }
public class RecipeSeedItem
{
    public string Title { get; set; }
    public int CategoryId { get; set; }
    public int IngredientId { get; set; }
    public int InstructionId { get; set; }

    public RecipeSeedItem(string title, int ingredientId, int instructionId)
    {
        Title = title;
        IngredientId = ingredientId;
        InstructionId = instructionId;
    }
}

public class Recipe
{
    public static RecipeSeedItem AmericanSteak = new("American steak", 1, 1);
    public static RecipeSeedItem MacNCheese = new("Mac And Cheese", 2, 2);
    public static RecipeSeedItem VanillaMilkshake = new("Vanilla Milkshake", 3, 3);

    public static List<RecipeSeedItem> Recipes()
    {
        return new List<RecipeSeedItem>
            {
                AmericanSteak,
                MacNCheese,
                VanillaMilkshake ,
            };
    }

}
public class RecipeCategorySeedItem
{
    public int RecipeId { get; set; }
    public int CategoryId { get; set; }

   public RecipeCategorySeedItem(int recipeId, int categoryId)
    {
        RecipeId = recipeId;
        CategoryId = categoryId;
    }
}

public class RecipeCategory
{
    public static RecipeCategorySeedItem AmericanSteak = new(1, 1);
    public static RecipeCategorySeedItem FirstItem = new(2, 5);
    public static RecipeCategorySeedItem SecondItem = new(2, 6);
    public static RecipeCategorySeedItem ThirdItem = new(3, 3);
    public static RecipeCategorySeedItem FourthItem = new(3, 1);

    public static List<RecipeCategorySeedItem> Recipes()
    {
        return new List<RecipeCategorySeedItem>
            {
                AmericanSteak,
                FirstItem,
                SecondItem,
                ThirdItem,
                FourthItem,
            };
    }
}


