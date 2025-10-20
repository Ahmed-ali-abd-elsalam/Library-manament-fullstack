using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class DataSeedingService
    {
        public static async Task<bool> SeedDataAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] Roles = ["Admin", "Member"];
            foreach (var role in Roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var userManager = serviceProvider.GetRequiredService<UserManager<Member>>();
            if (await userManager.FindByEmailAsync("admin@library.com") == null)
            {
                Member admin = new Member
                {
                    UserName="Admin",
                    Email = "admin@library.com",
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(admin, "adminPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            return true;
        }
    }
}
