namespace Eventify.Models.Accounts
{
    public abstract class User
    {
        public string Username { get; set; }
        public string Role { get; set; }

        public User(string username, string role)
        {
            Username = username;
            Role = role;
        }

        public abstract void DisplayMenu();
    }
}
