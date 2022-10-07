using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Seed
{
    [Migration(1004)]
    public class _1004_SeedRecipe : Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
            var recipes = Recipe.Recipes();

            foreach (var recipe in recipes)
            {
                Insert.IntoTable(Tables.Recipe).Row(new
                {
                    Title = recipe.Title,
                    IngredientId = recipe.IngredientId,
                    InstructionId = recipe.InstructionId
                });
            }
        }
    }
}
