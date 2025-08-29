using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using StarLine.Core.Common;
using StarLine.Core.Session;
using StarLine.Infrastructure;
using StarLine.Web.IdentityServices;

namespace StarLine.Web
{
    public static class RegisterServices
    {
        public static void RegisterService(this IServiceCollection services)
        {
            Configure(services, RepositoryRegister.GetTypes());
            ConfigureScoped(services);
        }

        private static void Configure(IServiceCollection services, Dictionary<Type, Type> types)
        {
            foreach (var type in types)
                services.AddTransient(type.Key, type.Value);
        }

        private static void ConfigureScoped(this IServiceCollection services)
        {
            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomClaimsPrincipalFactory>();
            services.AddScoped<IUserSession, UserSession>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<EmailService>();
            services.AddTransient<StarliteEmailService>();
        }
    }
}
