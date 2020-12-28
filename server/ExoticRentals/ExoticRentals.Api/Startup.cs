using System;
using System.Text;
using System.Threading.Tasks;
using ExoticRentals.Api.Contexts;
using ExoticRentals.Api.Entities;
using ExoticRentals.Api.Services;
using ExoticRentals.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ExoticRentals.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ExoticRentalDbContext>();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ExoticRentalDbContext>(opt =>
                {
                    opt
                    .UseNpgsql(Configuration.GetConnectionString("ExoticRentalDbContext"),
                    m => m.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                });
            AddAuthentication(services);

            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExoticRentals.Api", Version = "v1" });
            });
            services.AddCors(c => c.AddPolicy("dev", opt =>
            {
                opt.AllowAnyHeader()
                .WithExposedHeaders(AuthSettings.EXPIRED_TOKEN_HEADER) //https://stackoverflow.com/questions/37897523/axios-get-access-to-response-header-fields#answer-55714686
                .AllowAnyMethod()
                .WithOrigins("https://localhost:4001");

            }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExoticRentals.Api v1"));
                app.UseCors("dev");
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.SaveToken = false;
                jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = Configuration["JwtOptions:Issuer"],
                    ValidAudience = Configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtOptions:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = failedContext =>
                    {
                        if (failedContext.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            failedContext.Response.Headers.Add(AuthSettings.EXPIRED_TOKEN_HEADER, "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
