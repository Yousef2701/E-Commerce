using System.Text;
using E_Commerce.Application.Enums;
using E_Commerce.Application.Helpers;
using E_Commerce.Persistence.Data;
using E_Commerce.Persistence.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


#region Database Config

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region Set Passwort Settings

builder.Services.Configure<IdentityOptions>(o => {
    o.Password.RequiredUniqueChars = 0;
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
});

#endregion

#region Authorization Roles Config

builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminRole", op => op.RequireClaim(UsersRoles.Admin.ToString(), UsersRoles.Admin.ToString()))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("CustomerRole", op => op.RequireClaim(UsersRoles.Customer.ToString(), UsersRoles.Customer.ToString()))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("TraderRole", op => op.RequireClaim(UsersRoles.Trader.ToString(), UsersRoles.Trader.ToString()))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ShippingManRole", op => op.RequireClaim(UsersRoles.ShippingMan.ToString(), UsersRoles.ShippingMan.ToString()))
    );

#endregion

#region Swagger Config

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region JWT Config

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issure"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero

        };
    }
);

#endregion

#region Mapping Services

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
