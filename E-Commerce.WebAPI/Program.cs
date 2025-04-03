using E_Commerce.Application.Helpers;
using E_Commerce.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    options.AddPolicy("AdminRole", op => op.RequireClaim("Admin", "Admin"))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("CustomerRole", op => op.RequireClaim("Customer", "Customer"))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("TraderRole", op => op.RequireClaim("Trader", "Trader"))
    );

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ShippingManRole", op => op.RequireClaim("ShippingMan", "ShippingMan"))
    );

#endregion

#region Swagger Config

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region JWT Config

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
