using App.Mvc.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Mvc.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return; // DB has been seeded
            }

            //Roles
            var SuperAdminRole = new RoleEntity { Name = "SuperAdmin" };
            var AdminRole = new RoleEntity { Name = "Admin" };
            var ModeratorRole = new RoleEntity { Name = "Moderator" };
            var UserRole = new RoleEntity { Name = "User" };
            var roles = new[] { SuperAdminRole, AdminRole, ModeratorRole, UserRole };
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();

            //Users
           
            var users = new[]
            {

                new UserEntity
                {
                UserName = "NexusOwner",
                Email = "owner@nexusportal.com",
                Password = "SuperSecretPassword123!", 
                RoleId = SuperAdminRole.Id,
                IsApproved = true, 
                IsRejected = false
                 },
                new UserEntity
                {
                    UserName = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    Password = "password123",
                    IsApproved = true,
                    RoleId = ModeratorRole.Id,
                },
                new UserEntity
                {
                    UserName = "Bob Smith",
                    Email = "bob.smith@example.com",
                    Password = "password456",
                    IsApproved = true,
                    RoleId = AdminRole.Id,

                },
                new UserEntity {
                    UserName = "Charlie Brown",
                    Email = "charlie.brown@example.com",
                    Password = "password789",
                    RoleId = UserRole.Id,
                }

            };
            context.Users.AddRange(users); // Add more entities as needed
            await context.SaveChangesAsync();
        }
    }
}
