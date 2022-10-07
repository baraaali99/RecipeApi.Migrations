using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Seed
{
    [Migration(1001)]
    public class _1001_SeedCategory : FluentMigrator.Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
            var categories = new List<string>
             {
                    "American Food",
                    "Chinese",
                    "Drinks",
                    "Egyptian Food",
                    "Italy",
                    "Pasta Dishes",
                    "Spanish Food"
             };

             foreach (var category in categories)
            {
                Insert.IntoTable(Tables.Category).Row(new {name = category});
            }
        }   
    
    }
}
