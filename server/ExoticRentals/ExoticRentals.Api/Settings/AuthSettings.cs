namespace ExoticRentals.Api.Settings
{
    public enum ApplicationRoles
    {
        Admin,
        Manager,
        Staff,
        Customer
    }

    public static class AuthSettings
    {
        public const string REFRESH_TOKEN_COOKIE = "refresh-token-cookie";
        public const string REFRESH_TOKEN_HEADER = "refresh-token-header";
        public const string EXPIRED_TOKEN_HEADER = "expired-token-header";

    }
}
