using ASP.NET___CRUD.Services;
using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.Enums;
using EAD_Backend_Application__.NET.Helpers;
using EAD_Backend_Application__.NET.Models;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace EAD_Backend_Application__.NET
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. DATABASE CONFIGURATION
            var connectionString = builder.Configuration.GetConnectionString("EadDatabaseConnection") 
                ?? throw new InvalidOperationException("Connection string 'EadDatabaseConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 2. ADD IDENTITY WITH ROLE MANAGEMENT
            builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; // OPTIONAL: CONFIRM EMAIL BEFORE LOGIN
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // 3. JWT AUTHENTICATION CONFIGURATION
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret is not set in configuration."));
            var expiryInHours = double.TryParse(jwtSettings["ExpiryInHours"], out var hours) ? hours : 2;
            
            builder.Services.Configure<JwtSettings>(jwtSettings);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromHours(expiryInHours)
                };
            });

            // 4. DEPENDENCY INJECTION
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IRankingService, RankingService>();
            builder.Services.AddScoped<TokenService>();

            // 5. CONTROLLER & AUTHORIZATION CONFIGURATION
            builder.Services.AddControllers();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("VendorPolicy", policy => policy.RequireRole("Vendor"));
                options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
            });

            // 6. SWAGGER CONFIGURATION
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "EAD Backend API",
                    Description = "API for managing and handling backend operations, including user authentication, user management, product management, cart management and more."
                });

                c.TagActionsBy(api => new List<string> { api.ActionDescriptor.RouteValues["controller"] + " Controllers" });

                // INCLUDE XML COMMENTS IF THEY EXIST
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // DEFINE THE SECURITY SCHEME FOR JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 6465asdasd6561asd...')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // ADD SECURITY REQUIREMENT FOR THE ENDPOINTS
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

            // 7. ROLE CREATION & SEEDING
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await CreateRoles(services);
            }

            // 8. HTTP REQUEST PIPELINE CONFIGURATION
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();

                // ENABLE SWAGGER IN DEVELOPMENT MODE
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.RoutePrefix = string.Empty; // SET SWAGGER UI TO OPEN AT THE ROOT
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // ENABLE AUTHENTICATION AND AUTHORIZATION MIDDLEWARE
            app.UseAuthentication();  // JWT AUTHENTICATION
            app.UseAuthorization();   // ROLE-BASED AUTHORIZATION

            // MAP CONTROLLERS
            app.MapControllers();

            app.Run();
        }

        // HELPER METHOD: Create roles if they do not exist in the database
        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityResult roleResult;

            // LOOP THROUGH ENUM VALUES AND CHECK IF ROLES EXIST
            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                var roleName = role.ToString();

                // CHECK IF THE ROLE EXISTS
                if (!string.IsNullOrEmpty(roleName))
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        // CREATE THE ROLE IF IT DOES NOT EXIST
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }
    }
}
