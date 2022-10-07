using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Seed
{
    [Migration(1003)]
    public class _1003_SeedIngredients : Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
             var instructions = new List<string>
             {
                "mix in American Gardens BBQ spice seasoning, " +
                "Mix BBQ sauce and meat tenderizer for astonishing taste. Pour over the marinate onto the meat, " +
                "let it soak for hours before grilling, " +
                "grill.",

                "Cook elbow macaroni in the boiling water for 8 minutes, " +
                "Add Cheddar cheese and stir until melted, 2 to 4 minutes, " +
                "add milk to melted cheese, " +
                "Drain macaroni and fold into cheese sauce until coated.",

                "blend ice cream, milk, vanilla, " +
                "extract together in a blender until smooth, " +
                "pour into glasses and server, " +
                "drink"
             };

            foreach (var instruction in instructions)
            {
                Insert.IntoTable(Tables.Instruction).Row(new {Text = instruction});
            }
    }
    }
}
