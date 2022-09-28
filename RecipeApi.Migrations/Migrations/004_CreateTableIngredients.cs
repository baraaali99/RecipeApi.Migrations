using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(4)]
    public class _004_CreateIngredientsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Ingredient")
           .WithColumn("Id").AsGuid().PrimaryKey()
           .WithColumn("Name").AsString().NotNullable()
           .WithColumn("RecipeId").AsGuid();

            Create.ForeignKey()
            .FromTable("Ingredient").ForeignColumn("RecipeId")
            .ToTable("Recipe").PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
       
    }
}
