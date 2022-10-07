using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations._100
{
    [Migration(7)]
    public class _007_CreateTableRecipeCategory : AutoReversingMigration
    { 
        public override void Up()
        {
            Create.Table(Tables.RecipeCategory)
           .WithColumn("Id").AsInt32().PrimaryKey().Identity()
           .WithColumn("RecipeId").AsInt32().ForeignKey(Tables.Recipe, "Id")
          .WithColumn("CategoryId").AsInt32().ForeignKey(Tables.Category, "Id")
          .WithColumn("IsActive").AsBoolean().WithDefaultValue(true);
        }
    }
}
