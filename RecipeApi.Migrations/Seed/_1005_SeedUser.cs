using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Seed
{
    [Migration(1005)]
    public class _1005_SeedUser : Migration
    {
        public override void Down()
        {
            //
        }

        public override void Up()
        {
            Insert.IntoTable(Tables.User).Row(new
            {
                Username = "Baraa Ali",
                Password = "1234567890",
                RefreshToken = "token"
            });
        }
    }
}
