using CrilieTechReviewsBlog.DataManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;

namespace CrilieTechReviewsBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            ILogger<Program> _logger = host.Services.CreateScope().ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var scope = host.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //Make sure database is created
                context.Database.EnsureCreated();

                var adminRole = new IdentityRole("Administrator");

                //If role does not exist, create it 
                if (!context.Roles.Any(r => r.Name == adminRole.Name))
                    roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();

                //Get potential role id and related user-role 
                string roleId = context.Roles.Where(u => u.Name == adminRole.Name).FirstOrDefault().Id;
                var userRole = context.UserRoles.Where(u => u.RoleId == roleId).FirstOrDefault();

                //If there is no user with administrator role, create a default one
                if (userRole == null && !context.Users.Any(u => u.Id == userRole.UserId))
                {
                    var adminUser = new IdentityUser()
                    {
                        UserName = "asd@asd.com",
                        Email = "asd@asd.com"
                    };
                    userManager.CreateAsync(adminUser, "asd").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddNLog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
