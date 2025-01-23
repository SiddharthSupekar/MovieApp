using App.Core.Apps.Movies.Command;
using App.Core.Interface;
using App.Core.Intetrface;
using Azure.Core;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddScoped<IMovieDbContext, MovieDbContext>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddDbContext<MovieDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MovieConnectionString"),
                    b=> b.MigrationsAssembly(typeof(MovieDbContext).Assembly.FullName));
            });
            

            return services;
        }
    }
}
