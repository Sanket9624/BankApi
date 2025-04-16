using BankApi.Data;
using BankApi.Repositories;
using BankApi.Repositories.Interfaces;
using BankApi.Services;
using BankApi.Services.Interfaces;
using BankApi.Entities;
using BankApi.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace BankApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔐 JWT Key
            var jwtKey = builder.Configuration["JWT:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            // 🔧 Service Registrations
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in Enum.GetValues(typeof(Permissions)))
                {
                    var policyName = permission.ToString();
                    options.AddPolicy(policyName, policy =>
                        policy.Requirements.Add(new PermissionRequirement((Permissions)permission)));
                }

                // Add branch-based policies
                options.AddPolicy("BranchAdminOnly", policy => policy.RequireClaim("RoleId", "1", "2") 
                    .RequireClaim("BranchId"));

                options.AddPolicy("BankAdminOnly", policy => policy.RequireClaim("RoleId", "1") 
                    .RequireClaim("BankId"));
            });

            RegisterCustomServices(builder.Services);
            ConfigureCors(builder.Services);
            ConfigureAuthentication(builder.Services, key);
            ConfigureAuthorization(builder.Services);
            ConfigureDatabase(builder.Services, builder.Configuration);
            ConfigureSwagger(builder.Services);

            // 🧾 JSON Enum Converter
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            var app = builder.Build();

            // 🚀 Initial Setup
            EnsureSuperAdminExists(app);
            EnsurePermissionsInitialized(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ⚙️ Middleware
            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        #region Configuration Methods

        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
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

        private static void ConfigureAuthentication(IServiceCollection services, byte[] key)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOnly", policy => policy.RequireClaim("RoleId", "3"));
                options.AddPolicy("BankManagerOnly", policy => policy.RequireClaim("RoleId", "2"));
                options.AddPolicy("SuperAdminOnly", policy => policy.RequireClaim("RoleId", "1"));

                options.AddPolicy("AllUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("RoleId", "1") ||
                        context.User.HasClaim("RoleId", "2") ||
                        context.User.HasClaim("RoleId", "3")
                    ));

                options.AddPolicy("SuperAdminOrBankManager", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("RoleId", "1") ||
                        context.User.HasClaim("RoleId", "2")
                    ));

                // Policies for branch-level and bank-level authorization
                options.AddPolicy("BranchAdminOnly", policy => policy.RequireClaim("RoleId", "1", "2") // Admin or Bank Manager
                    .RequireClaim("BranchId"));

                options.AddPolicy("BankAdminOnly", policy => policy.RequireClaim("RoleId", "1") // Only Admin
                    .RequireClaim("BankId"));
            });
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BankDb1Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
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
                        new string[] { }
                    }
                });
            });
        }

        private static void RegisterCustomServices(IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IBankManagerRepository, BankManagerRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IBankRepository, BankRepository>();


            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBankManagerService, BankManagerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBranchService, BranchService>();

        }

        #endregion

        #region Initialization Methods

        private static void EnsureSuperAdminExists(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BankDb1Context>();

            try
            {
                context.Database.Migrate();

                // Create default bank if not exists
                var defaultBank = context.Banks.FirstOrDefault(b => b.Name == "Indigo Bank");
                if (defaultBank == null)
                {
                    defaultBank = new Bank { Name = "Indigo Bank" , Address = "Ahmedabad" };
                    context.Banks.Add(defaultBank);
                    context.SaveChanges();
                    Console.WriteLine("🏦 Default bank created.");
                }

                // Create default branch if not exists
                var defaultBranch = context.Branches.FirstOrDefault(b => b.Name == "Main Branch" && b.BankId == defaultBank.BankId);
                if (defaultBranch == null)
                {
                    defaultBranch = new Branch
                    {
                        Name = "Main Branch",
                        Address = "Headquarters",
                        BankId = defaultBank.BankId
                    };
                    context.Branches.Add(defaultBranch);
                    context.SaveChanges();
                    Console.WriteLine("🏢 Default branch created.");
                }

                var superAdminRole = context.RoleMaster.FirstOrDefault(r => r.RoleName == "SuperAdmin")
                    ?? new RoleMaster { RoleName = "SuperAdmin" };

                if (superAdminRole.RoleId == 0)
                {
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
                        BranchId = defaultBranch.BranchId ?? 0,  // Assigning default branch
                       
                    };

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


        private static void EnsurePermissionsInitialized(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BankDb1Context>();

            try
            {
                var superAdminRole = context.RoleMaster.FirstOrDefault(r => r.RoleName == "SuperAdmin");
                if (superAdminRole == null)
                {
                    Console.WriteLine("❌ SuperAdmin role not found.");
                    return;
                }

                var allEnumPermissions = Enum.GetNames(typeof(Permissions));
                var existingPermissions = context.Permissions.Select(p => p.PermissionName).ToList();

                foreach (var perm in allEnumPermissions.Except(existingPermissions))
                {
                    context.Permissions.Add(new Permission { PermissionName = perm });
                    Console.WriteLine($"✅ Added permission: {perm}");
                }

                context.SaveChanges();

                var allPermissions = context.Permissions.ToList();
                var existingRolePermissions = context.RolePermissions
                    .Where(rp => rp.RoleId == superAdminRole.RoleId)
                    .Select(rp => rp.PermissionId)
                    .ToHashSet();

                foreach (var permission in allPermissions)
                {
                    if (!existingRolePermissions.Contains(permission.PermissionId))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = superAdminRole.RoleId,
                            PermissionId = permission.PermissionId
                        });
                        Console.WriteLine($"🔗 Assigned {permission.PermissionName} to SuperAdmin");
                    }
                }

                context.SaveChanges();
                Console.WriteLine("✅ All permissions initialized and assigned to SuperAdmin.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing permissions: {ex.Message}");
            }
        }

        #endregion
    }
}
