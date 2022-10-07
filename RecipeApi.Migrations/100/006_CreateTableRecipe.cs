using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(6)]
    public class _006_CreateRecipeTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Tables.Recipe)
           .WithColumn("Id").AsInt32().Identity().PrimaryKey()
           .WithColumn("Title").AsString().NotNullable()
           .WithColumn("IngredientId").AsInt32().ForeignKey(Tables.Ingredient, "Id")
           .WithColumn("InstructionId").AsInt32().ForeignKey(Tables.Instruction, "Id")
           .WithColumn("IsActive").AsBoolean().WithDefaultValue(true);

        }
    }
}
