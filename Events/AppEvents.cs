using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Accounts;

namespace Eventify.Events
{
    public delegate void UserActionEventHandler(object sender, UserActionEventArgs e);

    public class UserActionEventArgs : EventArgs
    {
        public string Username { get; }
        public string ActionType { get; }
        public DateTime ActionTime { get; }

        public UserActionEventArgs(string username, string actionType)
        {
            Username = username;
            ActionType = actionType;
            ActionTime = DateTime.Now;
        }
    }

    public static class AppEvents
    {
        public static event UserActionEventHandler UserLoggedIn;
        public static event UserActionEventHandler UserRegistered;
        public static event UserActionEventHandler UserDeleted;

        public static void OnUserLoggedIn(string username)
        {
            UserLoggedIn?.Invoke(null, new UserActionEventArgs(username, "LOGIN"));
        }

        public static void OnUserRegistered(string username)
        {
            UserRegistered?.Invoke(null, new UserActionEventArgs(username, "REGISTER"));
        }

        public static void OnUserDeleted(string username)
        {
            UserDeleted?.Invoke(null, new UserActionEventArgs(username, "DELETE_USER"));
        }

    }
}
