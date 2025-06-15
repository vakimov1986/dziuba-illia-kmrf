using CurConvApp.Models;

namespace CurConvApp.Services
{
    public class UserSessionManager
    {
        public static UserSessionManager Instance { get; } = new UserSessionManager();

        public DbUser? CurrentUser { get; set; }
    }

}
