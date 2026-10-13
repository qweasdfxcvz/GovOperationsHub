using GovOperationsHub.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. إنشاء الأدوار
            string[] roles = {
            AppRoles.SystemAdmin,
            AppRoles.ContractOfficer,
            AppRoles.ServicesOfficer,
            AppRoles.NetworkEngineer,
            AppRoles.MinistryRepresentative
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. إنشاء حساب مدير النظام الافتراضي
            string adminEmail = "admin@gov.sy";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // كلمة المرور الافتراضية: Admin@123
                var result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, AppRoles.SystemAdmin);
                }
            }
        }
    }
}
