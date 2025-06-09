using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurConvApp.Models;

namespace CurConvApp.Services
{
    public class UserSessionManager
    {
        private static UserSessionManager? _instance; // Marked as nullable to resolve CS8618
        public static UserSessionManager Instance => _instance ??= new UserSessionManager();

        public DbUser? CurrentUser { get; set; }
    }

}
