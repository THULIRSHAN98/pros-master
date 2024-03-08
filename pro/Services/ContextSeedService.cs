//using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pro;
using pro.Data;
using pro.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace pro.Services
{
    public class ContextSeedService
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeedService(Context context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Any())
            {
                // applies any pending migration into our database
                await _context.Database.MigrateAsync();
            }

            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.AdminRole });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.UserRole });
            }

            if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult())
            {
                var admin = new User
                {
                    FirstName = "admin",
                    LastName = "jackson",
                    UserName = "admin@example.com", // Admin username
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(admin, "AdminPassword123!"); // Admin password
                await _userManager.AddToRoleAsync(admin, SD.AdminRole);
                await _userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Surname, admin.LastName)
                });

                var user = new User
                {
                    FirstName = "user",
                    LastName = "miller",
                    UserName = "user@example.com",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(user, "UserPassword123!"); // User password
                await _userManager.AddToRoleAsync(user, SD.UserRole);
                await _userManager.AddClaimsAsync(user, new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Surname, user.LastName)
                });
            }
        }
    }
}
