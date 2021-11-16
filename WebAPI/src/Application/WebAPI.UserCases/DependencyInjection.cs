﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WebAPI.UserCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserCases(this IServiceCollection services) =>
            services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}