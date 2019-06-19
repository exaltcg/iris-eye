using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IrisEye.Web.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IrisEye.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost =  CreateWebHostBuilder(args).Build();
            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var users = serviceProvider.GetService<UserManager<IdentityUser>>();
                var roles = serviceProvider.GetService<RoleManager<IdentityRole>>();
                try
                {
                    DbInitializer.SeedUsers(users, roles).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            webHost.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://0.0.0.0:5151")
                .UseStartup<Startup>();
    }
}