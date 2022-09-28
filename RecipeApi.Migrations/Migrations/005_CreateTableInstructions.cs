using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(5)]
    public class _005_CreateTableInstructions : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Instruction")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Text").AsString().NotNullable()
            .WithColumn("RecipeId").AsGuid();

            Create.ForeignKey()
            .FromTable("Instruction").ForeignColumn("RecipeId")
            .ToTable("Recipe").PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
