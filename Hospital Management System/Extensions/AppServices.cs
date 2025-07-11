using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.Errors;
using Hospital_Management_System.Helper;
using Hospital_Management_System.Repository.Contexts;
using Hospital_Management_System.Repository.Repositories;
using Hospital_Management_System.Services.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Hospital_Management_System.Extensions
{
    public static class AppServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Register application services here
            // Example: services.AddScoped<IYourService, YourServiceImplementation>();
            services.AddControllers();
             
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            //Validation Error
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(x => x.Value.Errors.Count() > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();

                    var validationerror = new ApiValidationError()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(validationerror);
                };
            });

            services.AddIdentity<UserApp, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters=new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = services.BuildServiceProvider().GetRequiredService<IConfiguration>()["JWT:Issuer"],
                        ValidAudience = services.BuildServiceProvider().GetRequiredService<IConfiguration>()["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(services.BuildServiceProvider().GetRequiredService<IConfiguration>()["JWT:Key"]))
                    };          

                });
            services.AddScoped<ImageSettings>();

            services.AddScoped(typeof(ITokenServices), typeof(TokenServices));

            return services;
        }
    }
}
