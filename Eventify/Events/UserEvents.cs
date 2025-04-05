namespace Eventify.Events
{
    public static class UserEvents
    {
        public delegate void UserLoggedInHandler(string username);
        public static event UserLoggedInHandler? OnUserLoggedIn;

        public delegate void UserRegisteredHandler(string username);
        public static event UserRegisteredHandler? OnUserRegistered;

        public static void RaiseUserLoggedIn(string username) =>
            OnUserLoggedIn?.Invoke(username);

        public static void RaiseUserRegistered(string username) =>
            OnUserRegistered?.Invoke(username);
    }
}
