using BankApi.Data;
using BankApi.Repositories.Interfaces;
using BankApi.Services.Interfaces;
using BankApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using BankApi.Repositories;
using BankApi.Entities;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes("secret-key-123-456-789-123-213ftyufuyigyfgutiygyuhyifutxcfgvjhb");

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// ✅ Configure Authentication & JWT Bearer Token Validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// ✅ Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy => policy.RequireClaim("RoleId", "3"));
    options.AddPolicy("BankManagerOnly", policy => policy.RequireClaim("RoleId", "2"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireClaim("RoleId", "1"));

    // 🔹 Combine policies: Allow either Customer or SuperAdmin
    options.AddPolicy("AllUsers", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("RoleId", "3") || context.User.HasClaim("RoleId", "1") || context.User.HasClaim("RoleId", "2")
        )
    );
    options.AddPolicy("SuperAdminOrBankManager", policy =>
        policy.RequireAssertion(context =>
             context.User.HasClaim("RoleId", "1") || context.User.HasClaim("RoleId", "2")
        )
    );
});


// ✅ Register Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IBankingRepository, BankingRepository>();
builder.Services.AddScoped<IBankManagerRepository, BankManagerRepository>();
builder.Services.AddScoped<IBankManagerService, BankManagerService>();
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>(); // ✅ Fix: Explicitly register IAdminService

// ✅ Configure Database Context
builder.Services.AddDbContext<BankDb1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Configure JSON Serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ✅ Configure Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

    // 🔹 Enable JWT Authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token."
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

var app = builder.Build();

// ✅ Ensure SuperAdmin Exists When App Starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BankDb1Context>();
    EnsureSuperAdminExists(context);
}

// ✅ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

/// ✅ Method to Ensure SuperAdmin Exists
static void EnsureSuperAdminExists(BankDb1Context context)
{
    try
    {
        context.Database.Migrate(); // Ensure database is up-to-date

        var superAdminRole = context.RoleMaster.FirstOrDefault(r => r.RoleName == "SuperAdmin");
        if (superAdminRole == null)
        {
            superAdminRole = new RoleMaster { RoleName = "SuperAdmin" };
            context.RoleMaster.Add(superAdminRole);
            context.SaveChanges();
        }

        var superAdmin = context.Users.FirstOrDefault(u => u.RoleId == superAdminRole.RoleId);
        if (superAdmin == null)
        {
            var newSuperAdmin = new Users
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "superadmin@bankapi.com",


                PasswordHash = HashPassword("Admin@123"), // Securely hash password
                MobileNo = "9999999999",  // Dummy number, change as needed
                Address = "Admin HQ",   
                DateOfBirth = new DateTime(1980, 1, 1), // Change as required
                RoleId = superAdminRole.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
}

context.Users.Add(newSuperAdmin);
            context.SaveChanges();
            Console.WriteLine("✅ SuperAdmin user created successfully.");
        }
        else
        {
            Console.WriteLine("✅ SuperAdmin already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error creating SuperAdmin: {ex.Message}");
    }
}
