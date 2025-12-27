using Flygans_Backend.Data;

// Repositories
using Flygans_Backend.Repositories.Auth;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Repositories.Wishlists;
using Flygans_Backend.Repositories.Carts;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Payments;
using Flygans_Backend.Repositories.Users;
using Flygans_Backend.Repositories.Admin;

// Services
using Flygans_Backend.Services.Auth;
using Flygans_Backend.Services.Products;
using Flygans_Backend.Services.Wishlists;
using Flygans_Backend.Services.Carts;
using Flygans_Backend.Services.Orders;
using Flygans_Backend.Services.Payments;
using Flygans_Backend.Services.Users;
using Flygans_Backend.Services.Cloudinary;
using Flygans_Backend.Services.Admin;

// Middleware
using Flygans_Backend.Middleware;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//
// ======================= CORS =======================
// React (Vite) runs on http://localhost:5173
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//
// ======================= CONTROLLERS =======================
//
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

//
// ======================= SWAGGER + JWT =======================
//
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//
// ======================= DATABASE =======================
//
builder.Services.AddDbContext<FlyganDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//
// ======================= AUTHENTICATION =======================
//
var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

//
// ======================= REPOSITORIES =======================
//
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();

//
// ======================= SERVICES =======================
//
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

//
// ======================= PIPELINE =======================
//
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// ✅ CORS must be after UseRouting
app.UseCors("AllowFrontend");

// ✅ Global Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// ✅ Auth order is IMPORTANT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
