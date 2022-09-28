using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(1)]
    public class _001_CreateTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Recipe")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("IsActive").AsBoolean().WithDefaultValue(true);

            Create.ForeignKey()
           .FromTable("Recipe").ForeignColumn("Title")
           .ToTable("Recipe").PrimaryColumn("Id")
           .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
