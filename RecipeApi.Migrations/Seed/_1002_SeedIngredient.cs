using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Seed
{
    [Migration(1002)]
    public class _1002_SeedIngredient : Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
            var ingredients = new List<string>
             {
                "8 pieces Beef Spareribs, cut across bone, 1 Cup American Garden BBQ Sauce Original, American Garden Meat Tenderizer, chopped, 2 Tbsp. Olive Oil, Brown Sugar, Salt-to-taste, Pepper-to-taste",

                "Mac,Milk, Chedder Cheese",

                 "2 cups vanilla Ice cream, 1 cup whole milk, 1 teaspoon vanilla"
            };

            foreach(var ingredient in ingredients)
            {
                Insert.IntoTable(Tables.Ingredient).Row(new { Text = ingredient });
            }

        }
    }
}

