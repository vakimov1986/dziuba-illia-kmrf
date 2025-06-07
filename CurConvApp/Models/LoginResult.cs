namespace CurConvApp.Models
{
    public record LoginResult
    {
        public bool IsSuccess { get; init; }

        public bool IsFailed => !IsSuccess;

        public string? Error { get; init; }

        public User? User { get; init; }

        internal static LoginResult Success(User user) => new LoginResult()
        {
            IsSuccess = true,
            Error = null,
            User = user
        };

        internal static LoginResult Failed(string error) => new LoginResult()
        {
            IsSuccess = false,
            Error = error,
            User = null
        };
    }
}
