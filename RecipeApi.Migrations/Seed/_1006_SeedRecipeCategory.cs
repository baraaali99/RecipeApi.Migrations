using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApi.Migrations.Seed
{
    [Migration(1006)]
    public class _1006_SeedRecipeCategory : Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
            var list = RecipeCategory.Recipes();
            foreach(var recipecategory in list)
            {
                Insert.IntoTable(Tables.RecipeCategory).Row(new
                {
                    RecipeId = recipecategory.RecipeId,
                    CategoryId = recipecategory.CategoryId
                }) ;
            }
        }
    }
}
