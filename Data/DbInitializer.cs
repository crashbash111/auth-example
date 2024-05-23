using Lab4API.Models;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            );
            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            var adminUser = new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") };
            var normalUser = new User { Username = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123") };

            context.Users.AddRange(adminUser, normalUser);
            context.SaveChanges();

            context.UserRoles.AddRange(
                new UserRole { UserId = adminUser.Id, RoleId = context.Roles.Single(r => r.Name == "Admin").Id },
                new UserRole { UserId = normalUser.Id, RoleId = context.Roles.Single(r => r.Name == "User").Id }
            );
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Product 1", Price = 10.0m },
                new Product { Name = "Product 2", Price = 20.0m },
                new Product { Name = "Product 3", Price = 30.0m }
            );
            context.SaveChanges();
        }
    }
}
