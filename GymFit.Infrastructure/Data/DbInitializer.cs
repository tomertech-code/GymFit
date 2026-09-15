using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymFit.Domain.Entities;
using GymFit.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymFit.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roleNames = { "Admin", "Trainer", "Member", "Reception" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin User only when an explicit password is supplied through configuration/environment.
            // Never commit a default admin password to source control.
            var configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@gymfit.com";
            var adminPassword = configuration["SeedAdmin:Password"] ?? Environment.GetEnvironmentVariable("GYMFIT_ADMIN_PASSWORD");
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is not null)
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else if (!string.IsNullOrWhiteSpace(adminPassword))
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "GymFit",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Membership Plans
            if (!await context.MembershipPlans.AnyAsync())
            {
                var plans = new List<MembershipPlan>
            {
                new MembershipPlan
                {
                    Name = "Basic",
                    Description = "Perfect for beginners",
                    Price = 29.99m,
                    DurationDays = 30,
                    Type = MembershipType.Basic,
                    Features = "Gym Access,Basic Equipment,Locker",
                    IsActive = true
                },
                new MembershipPlan
                {
                    Name = "Standard",
                    Description = "Most popular choice",
                    Price = 49.99m,
                    DurationDays = 30,
                    Type = MembershipType.Standard,
                    Features = "Gym Access,All Equipment,Locker,Group Classes",
                    IsActive = true
                },
                new MembershipPlan
                {
                    Name = "Premium",
                    Description = "Complete fitness experience",
                    Price = 79.99m,
                    DurationDays = 30,
                    Type = MembershipType.Premium,
                    Features = "Gym Access,All Equipment,Locker,Group Classes,Personal Training,Nutrition Plan",
                    IsActive = true
                },
                new MembershipPlan
                {
                    Name = "VIP",
                    Description = "Ultimate luxury package",
                    Price = 129.99m,
                    DurationDays = 30,
                    Type = MembershipType.VIP,
                    Features = "All Premium Features,Spa Access,Priority Booking,Guest Passes",
                    IsActive = true
                }
            };

                await context.MembershipPlans.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }
        }
    }

}
