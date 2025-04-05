using Eventify.Models.Accounts;

public static class RBACService
{
    public static User CreateUserByRole(string username, string role)
    {
        return role switch
        {
            "Admin" => new Admin(username),
            "Manager" => new Manager(username),
            _ => new RegularUser(username),
        };
    }
}
