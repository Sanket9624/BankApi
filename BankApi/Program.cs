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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using BankApi.Enums;

namespace BankApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Get JWT key from appsettings.json
            var jwtKey = builder.Configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing in appsettings.json");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            ConfigureCors(builder);
            ConfigureAuth(builder, key);
            RegisterServices(builder);
            ConfigureDatabase(builder);



            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            ConfigureSwagger(builder);

            var app = builder.Build();

            EnsureSuperAdminExists(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        // ✅ CORS Configuration
        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }



        // ✅ Authentication & Authorization Configuration
        private static void ConfigureAuth(WebApplicationBuilder builder, byte[] key)
        {
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

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOnly", policy => policy.RequireClaim("RoleId", "3"));
                options.AddPolicy("BankManagerOnly", policy => policy.RequireClaim("RoleId", "2"));
                options.AddPolicy("SuperAdminOnly", policy => policy.RequireClaim("RoleId", "1"));

                options.AddPolicy("AllUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("RoleId", "3") ||
                        context.User.HasClaim("RoleId", "1") ||
                        context.User.HasClaim("RoleId", "2")
                    )
                );
                options.AddPolicy("SuperAdminOrBankManager", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("RoleId", "1") ||
                        context.User.HasClaim("RoleId", "2")
                    )
                );
            });
        }

        // ✅ Register Services & Repositories
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBankManagerRepository, BankManagerRepository>();
            builder.Services.AddScoped<IBankManagerService, BankManagerService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();  // Add this line
            builder.Services.AddScoped<IPermissionService, PermissionService>(); // Add this line
        }


        // ✅ Configure Database Context
        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<BankDb1Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        // ✅ Swagger Configuration
        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

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
        }

        // ✅ Ensure SuperAdmin Exists When App Starts
        private static async Task EnsureSuperAdminExists(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<BankDb1Context>();
            //var roleRepository = services.GetRequiredService<IRoleRepository>();
            var permissionRepository = services.GetRequiredService<IPermissionRepository>();
            var rolePermissionService = services.GetRequiredService<IPermissionService>(); // Assuming this handles permission assignments

            try
            {
                context.Database.Migrate(); // Ensure the latest migrations are applied

                // ✅ Ensure Permissions Exist
                if (!context.Permissions.Any())
                {
                    Console.WriteLine("✅ Seeding Permissions Data...");
                    List<Permissions> permissions = new List<Permissions>();
                    foreach (PermissionEnum permission in Enum.GetValues(typeof(PermissionEnum)))
                    {
                        permissions.Add(new Permissions
                        {
                            PermissionId = (int)permission,
                            PermissionName = permission.ToString()
                        });
                    }
                    context.Permissions.AddRange(permissions);
                    context.SaveChanges();
                    Console.WriteLine("✅ Permissions seeded successfully.");
                }
                else
                {
                    Console.WriteLine("✅ Permissions already exist.");
                }

                // ✅ Ensure SuperAdmin Role Exists
                var superAdminRole = context.RoleMaster.FirstOrDefault(r => r.RoleName == "SuperAdmin");
                if (superAdminRole == null)
                {
                    superAdminRole = new RoleMaster { RoleName = "SuperAdmin" };
                    context.RoleMaster.Add(superAdminRole);
                    context.SaveChanges();
                }

                // ✅ Ensure SuperAdmin User Exists
                var superAdmin = context.Users.FirstOrDefault(u => u.RoleId == superAdminRole.RoleId);
                if (superAdmin == null)
                {
                    var newSuperAdmin = new Users
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        Email = "superadmin@indigobook.com",
                        PasswordHash = PasswordUtility.HashPassword("Admin@123"),
                        MobileNo = "9999999999",
                        Address = "Admin HQ",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        RoleId = superAdminRole.RoleId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsEmailVerified = true,
                        TwoFactorEnabled = false,
                        RequestStatus = RequestStatus.Approved,
                    };

                    context.Users.Add(newSuperAdmin);
                    context.SaveChanges();
                    Console.WriteLine("✅ SuperAdmin user created successfully.");
                }
                else
                {
                    Console.WriteLine("✅ SuperAdmin already exists.");
                }

                // ✅ Ensure Admin Role Has All Permissions Assigned
                var allPermissions = context.Permissions.Select(p => p.PermissionId).ToList();
                var assignedPermissions = context.RolePermissions
                    .Where(rp => rp.RoleId == superAdminRole.RoleId)
                    .Select(rp => rp.PermissionId)
                    .ToList();

                // Get permissions that are not assigned
                var permissionsToAssign = allPermissions.Except(assignedPermissions).ToList();

                if (permissionsToAssign.Any())
                {
                    await rolePermissionService.AssignPermissionsToRoleAsync(superAdminRole.RoleId, permissionsToAssign);
                    Console.WriteLine($"✅ Assigned missing permissions to SuperAdmin: {permissionsToAssign.Count} permissions.");
                }
                else
                {
                    Console.WriteLine("✅ SuperAdmin already has all permissions assigned.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding data: {ex.Message}");
            }
        }


    }
}
