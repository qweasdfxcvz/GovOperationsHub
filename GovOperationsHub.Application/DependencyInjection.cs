using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using GovOperationsHub.Application.Common.Behaviours;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GovOperationsHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            return services;
        }
    }
}
