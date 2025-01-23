
using App.Core;
using App.Core.Apps.Movies.Command;
using App.Core.Profiles;
using App.Core.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateBootstrapLogger();

            builder.Host.UseSerilog();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(configuration);
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining(typeof(MovieValidator));
            builder.Services.AddControllers();

            var MyAllowSpecificOrigin = "_myAllowsSpecificOrigin";

            builder.Services.AddCors(options => {
                options.AddPolicy(name: MyAllowSpecificOrigin,
                       policy => {
                           policy.WithOrigins("http://localhost:4200", "http://localhost:60743/", "http://localhost:62991/", "http://localhost:56159/")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                       });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
               options =>
               {
                   options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                   {
                       Title = "Auth Demo",
                       Version = "v1"
                   });

                   options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                   {
                       In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                       Description = "Please enter a token",
                       Name = "Authorization",
                       Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                       BearerFormat = "JWT",
                       Scheme = "bearer"
                   });

                   options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                    []
                    }
               });

               }
           );

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(MyAllowSpecificOrigin);


            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
