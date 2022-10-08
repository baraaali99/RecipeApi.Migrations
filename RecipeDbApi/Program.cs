using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SD.LLBLGen.Pro.ORMSupportClasses;
using RecipeDbApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Data;
using SD.LLBLGen.Pro.DQE.SqlServer;
using System.Security.Claims;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using SD.LLBLGen.Pro.LinqSupportClasses;
using Shop.Data;
using Shop.Data.FactoryClasses;
using Shop.Data.EntityClasses;
using Shop.Data.DatabaseSpecific;
using Shop.Data.HelperClasses;
using Shop.Data.Linq;
using View.Persistence;
using Npgsql;
using System.Data.SqlClient;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddCors(options =>
{
	options.AddPolicy("Cors Policy",
		policy =>
		{
			policy
				.WithOrigins(builder.Configuration["FrontendOrigin"])
				.AllowAnyHeader()
				.AllowAnyMethod()
				.WithExposedHeaders("IS-TOKEN-EXPIRED")
				.AllowCredentials();
		});
});

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(o =>
	{
		var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
		o.SaveToken = true;
		o.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			ValidAudience = builder.Configuration["JWT:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Key),
			ClockSkew = TimeSpan.Zero
		};
		o.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
				{
					context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
				}
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
	options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("Cors Policy");
app.UseAuthentication();
app.UseAuthorization();

// configure LLBLGen pro
ConfigureLLBLGen(builder.Services, builder.Configuration);
RuntimeConfiguration.AddConnectionString(app.Configuration["Key"],
									app.Configuration["ConnectionString"]);

RuntimeConfiguration.ConfigureDQE<SQLServerDQEConfiguration>(
				c => c.AddDbProviderFactory(typeof(System.Data.SqlClient.SqlClientFactory)));

// methods
string GenerateRefreshToken()
{
	var randomNumber = new byte[32];
	using (var rng = RandomNumberGenerator.Create())
	{
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}

JwtToken? GenerateJWT(User user)
{
	var tokenHandler = new JwtSecurityTokenHandler();
	var tokenKey = Encoding.UTF8.GetBytes(app.Configuration["JWT:Key"]);
	var tokenDescriptor = new SecurityTokenDescriptor
	{
		Subject = new ClaimsIdentity(new Claim[]
		{
				new Claim(ClaimTypes.Name, user.Name)
		}),
		Expires = DateTime.UtcNow.AddMinutes(1),
		SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
	};
	var token = tokenHandler.CreateToken(tokenDescriptor);
	return new JwtToken { Token = tokenHandler.WriteToken(token), RefreshToken = GenerateRefreshToken() };
}

// variables
QueryFactory qf = new();

// user enpoint
app.MapPost("/register", async (HttpContext context, IAntiforgery forgeryService, User user) =>
{
	using (DataAccessAdapter adapter = new())
	{
		var userEntity = new UserEntity
		{
			Username = user.Name,
			Password = user.Password
		};

		var q = qf.User.Where(UserFields.Username == user.Name);

		if (user.Name == String.Empty || user.Password == String.Empty || await adapter.FetchFirstAsync(q) != null)
		{
			return Results.BadRequest();
		}

		PasswordHasher<string> pw = new();
		userEntity.Password = pw.HashPassword(user.Name, user.Password);

		var token = GenerateJWT(user);

		if (token == null)
			return Results.Unauthorized();

		userEntity.RefreshToken = token.RefreshToken;

		var result = await adapter.SaveEntityAsync(userEntity);
		if (result)
		{
			return Results.Ok(token);
		}
		else
		{
			return Results.BadRequest();
		}
	}
});

app.MapPost("/login", async (HttpContext context, IAntiforgery forgeryService, User user) =>
{
	using (DataAccessAdapter adapter = new())
	{
		var userEntity = new UserEntity
		{
			Username = user.Name,
			Password = user.Password
		};

		PasswordHasher<string> pw = new();

		var query = qf.User.Where(UserFields.Username == user.Name);
		userEntity = await adapter.FetchFirstAsync(query);
		if (userEntity != null && pw.VerifyHashedPassword(user.Name, userEntity.Password, user.Password) == PasswordVerificationResult.Success)
		{
			var token = GenerateJWT(user);

			if (token == null)
			{
				return Results.Unauthorized();
			}

			userEntity.RefreshToken = token.RefreshToken;

			await adapter.SaveEntityAsync(userEntity);
			return Results.Ok(token);
		}
		return Results.Unauthorized();
	}
});

app.MapGet("/antiforgery/token", [Authorize] (IAntiforgery forgeryService, HttpContext context) =>
{
	var tokens = forgeryService.GetAndStoreTokens(context);
	context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
			new CookieOptions { Secure = true, HttpOnly = false, SameSite = SameSiteMode.None });
});

app.MapPost("/refresh", async (JwtToken jwt) =>
{
	using (DataAccessAdapter adapter = new())
	{
		var userEntity = new UserEntity
		{
			RefreshToken = jwt.RefreshToken
		};

		if (adapter.FetchEntityUsingUniqueConstraint(userEntity, null))
		{
			var token = GenerateJWT(new User
			{
				Name = userEntity.Username,
				Password = userEntity.Password
			});

			if (token == null)
				return Results.Unauthorized();

			userEntity.RefreshToken = token.RefreshToken;
			await adapter.SaveEntityAsync(userEntity);
			return Results.Ok(token);
		}
	}
	return Results.Unauthorized();
});

// recipe endpoints
app.MapGet("/recipes", [Authorize] async (HttpContext context, IAntiforgery forgeryService) =>
{
	var adapter = new DataAccessAdapter();
	var meta = new LinqMetaData(adapter);
	var list = await meta.Recipe.Where(x => x.IsActive).ProjectToRecipeView().ToListAsync();

	return Results.Ok(list);
});

app.MapGet("/recipes/{id}", [Authorize] async (int id, HttpContext context, IAntiforgery forgeryService) =>
{
    try
    {
		var adapter = new DataAccessAdapter();
		var meta = new LinqMetaData(adapter);
		var recipe = await meta.Recipe.Where(x => x.IsActive && x.Id == id).ProjectToRecipeView().FirstOrDefaultAsync();
		return Results.Ok(recipe);
	}
    catch (Exception)
    {
		return Results.BadRequest();
    }

});

app.MapPost("/recipes", [Authorize] async (RecipeInput recipe, HttpContext context, IAntiforgery forgeryService) =>
{
	////await forgeryService.ValidateRequestAsync(context);

	using (DataAccessAdapter adapter = new())
	{
		try
		{
			await adapter.StartTransactionAsync(IsolationLevel.ReadUncommitted, "Insert Recipe");
			var recipeEntity = new RecipeEntity()
			{
				Title = recipe.Title,
				InstructionId = recipe.InstructionsId,
				IngredientId = recipe.IngredientsId,
				IsNew = true
			};

			await adapter.SaveEntityAsync(recipeEntity, true);

			var collection = new EntityCollection<RecipeCategoryEntity>();
			foreach (var categoryId in recipe.Categories)
            {
				var entity = new RecipeCategoryEntity
				{
					RecipeId = recipeEntity.Id,
					CategoryId = categoryId,
					IsNew = true
                };
				collection.Add(entity);
            }
			await adapter.SaveEntityCollectionAsync(collection);
			// create instruction entities
			
			adapter.Commit();
			return Results.Created($"/recipes/{recipeEntity.Title}", recipe);
		}
		catch (Exception)
		{
			// Rollback if anything goes wrong
			adapter.Rollback();
			return Results.BadRequest();
		}
	}
});

app.MapDelete("/recipes/{id}", [Authorize] async (int id, HttpContext context, IAntiforgery forgeryService) =>
{
    ////await forgeryService.ValidateRequestAsync(context);

    try
    {
		var adapter = new DataAccessAdapter();
		await adapter.StartTransactionAsync(IsolationLevel.ReadUncommitted, "Delete Recipe");
		var recipe = new RecipeEntity()
		{
			Id = id,
			IsNew = false,
			IsActive = false
		};

		await adapter.SaveEntityAsync(recipe, true);
		adapter.Commit(); return Results.Ok();
    }
    catch (Exception)
    {
		return Results.BadRequest();
    }
});

app.MapPut("/recipes/{id}", [Authorize] async (RecipeInput recipeInput, HttpContext context, IAntiforgery forgeryService) =>
{
	////await forgeryService.ValidateRequestAsync(context);

	using (DataAccessAdapter adapter = new())
	{
		adapter.StartTransaction(IsolationLevel.ReadUncommitted, "Put Recipe");
		try
		{
			var meta = new LinqMetaData();
			var categories = new EntityCollection<RecipeCategoryEntity>();
			var categoryList = await meta.RecipeCategory.Where(x => x.IsActive && x.RecipeId == recipeInput.Id).ToListAsync();

			foreach (var categoryId in recipeInput.Categories)
            {
				if(!categoryList.Any(x => x.Id == categoryId))
                {
					var category = new RecipeCategoryEntity()
					{
						CategoryId = categoryId,
						RecipeId = recipeInput.Id,
						IsNew = true
					};
					categories.Add(category);
				}
			}

			await adapter.SaveEntityCollectionAsync(categories);

			var entity = new RecipeEntity()
			{
				Title = recipeInput.Title,
				Id = recipeInput.Id,
				IngredientId = recipeInput.IngredientsId,
				InstructionId = recipeInput.InstructionsId,
				IsNew = false
			};

			await adapter.SaveEntityAsync(entity);

			adapter.Commit(); 
			return Results.Ok(entity);
		}
		catch (Exception)
		{
			// Rollback if anything goes wrong
			adapter.Rollback();
			return Results.BadRequest();
		}
	}
});

// category endpoints
app.MapGet("/categories", [Authorize] async (HttpContext context, IAntiforgery forgeryService) =>
{
	//await forgeryService.ValidateRequestAsync(context);

	var adapter = new DataAccessAdapter();
	var meta = new LinqMetaData(adapter);
	var list = await meta.Category.Where(x => x.IsActive).ProjectToCategoryView().ToListAsync();

	return Results.Ok(list);

});

app.MapPost("/category", [Authorize] async (string category, HttpContext context, IAntiforgery forgeryService) =>
{
	using (DataAccessAdapter adapter = new())
	{
		try
		{
			await adapter.StartTransactionAsync(IsolationLevel.ReadUncommitted, "Insert Category");
			var CategoryEntity = new CategoryEntity()
			{
				Name = category,
				IsNew = true
			};

			await adapter.SaveEntityAsync(CategoryEntity, true);

			adapter.Commit();
			return Results.Created($"/category/{CategoryEntity.Name}", category);
		}
		catch (Exception)
		{
			// Rollback if anything goes wrong
			adapter.Rollback();
			return Results.BadRequest();
		}
	}
});

app.MapDelete("/categories/{categoryId}", [Authorize] async (int categoryId, HttpContext context, IAntiforgery forgeryService) =>
{
	if (categoryId == null)
	{
		return Results.BadRequest();
	}

	try
	{
		var adapter = new DataAccessAdapter();
		await adapter.StartTransactionAsync(IsolationLevel.ReadUncommitted,"Delete Category");
		var category = new CategoryEntity()
		{
			Id = categoryId,
			IsNew = false,
			IsActive = false
		};

		await adapter.SaveEntityAsync(category, true);
		adapter.Commit(); return Results.Ok();
    }
	catch (Exception)
	{
		return Results.Problem();
	}
});

app.MapPut("/categories/{categoryId}", [Authorize] async (int categoryId, string editedCategory, HttpContext context, IAntiforgery forgeryService) =>
{
	//await forgeryService.ValidateRequestAsync(context);
	if (editedCategory == null)
	{
		return Results.BadRequest();
	}
	using (DataAccessAdapter adapter = new())
	{
		await adapter.StartTransactionAsync(IsolationLevel.ReadUncommitted, "Put Category");

		try
		{
			var entity = new CategoryEntity()
			{
				Id = categoryId,
				Name = editedCategory,
				IsNew = false
			};

			return Results.Ok(entity);
		}
		catch (Exception)
		{
			return Results.Problem();
		}
	}
	
});
static void ConfigureLLBLGen(IServiceCollection services, IConfiguration config)
{
	var connectionString = config["ConnectionString"];
	RuntimeConfiguration.AddConnectionString("ConnectionString.SQL Server (SqlClient)", connectionString);
	RuntimeConfiguration.ConfigureDQE<SQLServerDQEConfiguration>(c =>
	{
		c.AddDbProviderFactory(typeof(SqlClientFactory));
		c.SetTraceLevel(TraceLevel.Verbose);
	});

	services.AddScoped<DataAccessAdapter>();
}
app.Run();