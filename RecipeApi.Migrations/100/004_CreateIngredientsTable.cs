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
            Create.Table(Tables.Ingredient)
           .WithColumn("Id").AsInt32().Identity().PrimaryKey()
           .WithColumn("Text").AsString().NotNullable();
        }
       
    }
}
