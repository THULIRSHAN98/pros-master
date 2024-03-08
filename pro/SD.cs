using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace pro
{
    public static class SD
    {
        // Roles
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public const string AdminUserName = "admin@example.com";
        public const string SuperAdminChangeNotAllowed = "Super Admin change is not allowed!";
        public const int MaximumLoginAttempts = 3;
    }
}