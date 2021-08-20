using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHub.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarHub.Extensions
{
    public static class DefaultUserSetupExtension
    {
        public static async Task CreateDefaultUser(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(CreateDefaultUser));

            var userOptions = new DefaultUserOptions();
            configuration.GetSection(nameof(DefaultUserOptions)).Bind(userOptions);

            logger.LogInformation("Checking if default user already exists in database");
            var user = await userManager.FindByEmailAsync(userOptions.EmailAddress);

            if (user != null)
            {
                logger.LogInformation("Skipping user initialization because user is already added. {@User}", new { user.Id, user.Email, user.UserName });
                return;
            }

            user = new IdentityUser
            {
                Email = userOptions.EmailAddress,
                UserName = userOptions.EmailAddress,
                LockoutEnabled = false,
                EmailConfirmed = true
            };

            logger.LogInformation("Creating user with default settings {@UserOptions}", userOptions);
            var result = await userManager.CreateAsync(user, userOptions.InitialPassword);


            logger.LogInformation("Result: {@Result}", result);

            if (!result.Succeeded)
            {
                throw new Exception("Application cannot start due to an error while initializing the default user. See logs for details");
            }
        }
    }
}
