using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.Interfaces.Services;
using TodoApp.Services.Implementations;

namespace TodoApp.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}
