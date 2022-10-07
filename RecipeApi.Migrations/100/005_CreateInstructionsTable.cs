using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(5)]
    public class _005_CreateInstructionsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Tables.Instruction)
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("Text").AsString().NotNullable();
        }
    }
}
