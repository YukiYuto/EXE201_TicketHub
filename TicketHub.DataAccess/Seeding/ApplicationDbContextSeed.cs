using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models.Domain;
using TicketHub.Utility.Constants;

namespace TicketHub.DataAccess.Seeding;

public class ApplicationDbContextSeed
{
    public static void SeedAdminAccount(ModelBuilder modelBuilder)
    {
        var staffRoleId = "8fa7c7bb-b4dd-480d-a660-e07a90855d5s";
        var adminRoleId = "8fa7c7bb-daa5-a660-bf02-82301a5eb32a";
        var managerRoledId = "a7782126-d76b-41c9-86d9-f41a026d107d";

        var roles = new List<IdentityRole>
        {
            new()
            {
                Id = staffRoleId,
                ConcurrencyStamp = StaticUserRoles.Staff,
                Name = StaticUserRoles.Staff,
                NormalizedName = StaticUserRoles.Staff
            },
            new()
            {
                Id = adminRoleId,
                ConcurrencyStamp = StaticUserRoles.Admin,
                Name = StaticUserRoles.Admin,
                NormalizedName = StaticUserRoles.Admin
            },
            new()
            {
                Id = managerRoledId,
                ConcurrencyStamp = StaticUserRoles.Manager,
                Name = StaticUserRoles.Manager,
                NormalizedName = StaticUserRoles.Manager
            }
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);

        // Seeding admin user
        var adminUserId = "TicketHub-Admin";
        var hasher = new PasswordHasher<ApplicationUser>();

        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            FullName = "Admin User",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Set appropriate value
            AvatarUrl = "https://example.com/avatar.png", // Set appropriate value
            Country = "Country", // Set appropriate value
            Address = "123 Admin St",
            UserName = "admin@gmail.com",
            NormalizedUserName = "ADMIN@GMAIL.COM",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Admin123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        // Seeding staff user
        var staffUserId = "StaffId";

        var staffUser = new ApplicationUser
        {
            Id = staffUserId,
            FullName = "Staff_1 User",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AvatarUrl = "https://example.com/avatarStaff.png",
            //CCCD = "123456789126",
            Country = "Country",
            Address = "123 Staff St",
            UserName = "staff1@gmail.com",
            NormalizedUserName = "STAFF1@GMAIL.COM",
            Email = "staff1@gmail.com",
            NormalizedEmail = "STAFF1@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Staff123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "0123456789",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };


        var staffUserId2 = "StaffId2";

        var staffUser2 = new ApplicationUser
        {
            Id = staffUserId2,
            FullName = "Staff_2 User",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AvatarUrl = "https://example.com/avatarStaff2.png",
            //CCCD = "123456789124",
            Country = "Country",
            Address = "456 Staff St",
            UserName = "staff2@gmail.com",
            NormalizedUserName = "STAFF2@GMAIL.COM",
            Email = "staff2@gmail.com",
            NormalizedEmail = "STAFF2@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Staff123!"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "0987654321",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var managerUserId = "ManagerId";

        var managerUser = new ApplicationUser
        {
            Id = managerUserId,
            FullName = "Manager User",
            BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Set appropriate value
            AvatarUrl = "https://example.com/avatarManager.png", // Set appropriate value
            Country = "Country",
            Address = "789 Manager St",
            UserName = "manager@gmail.com",
            NormalizedUserName = "MANAGER@GMAIL.COM",
            Email = "manager@gmail.com",
            NormalizedEmail = "MANAGER@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Manager123!"), // Hash password
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "0981234567",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        modelBuilder.Entity<ApplicationUser>().HasData(adminUser, staffUser2, staffUser, managerUser);

        // Assigning the admin role to the admin user
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUserId
        });

        // Assign the Staff role to the staff users  
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = staffUserId,
            RoleId = staffRoleId
        });

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = staffUserId2,
            RoleId = staffRoleId
        });

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = managerUserId,
            RoleId = managerRoledId
        });
    }
}