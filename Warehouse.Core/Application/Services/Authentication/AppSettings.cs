namespace Warehouse.Core.Application.Services.Authentication
{
    public class AppSettings
    {
        public string Secret { get; set; }

        /// <summary>
        ///  refresh token time to live (in days), inactive tokens are
        /// automatically deleted from the database after this time
        /// </summary>
        public int RefreshTokenTTL { get; set; }
        /// <summary>
        /// refresh token expiration (in days)
        /// </summary>
        public int RefreshTokenExpires { get; set; }
        /// <summary>
        ///  token expiration (in minutes)
        /// </summary>
        public int TokenExpires { get; set; }
    }
}
