using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public class RegularUser : User
    {
        public RegularUser(string username, string passwordHash, string email)
            : base(username, passwordHash, "User", email) { }

        public override void DisplayDashboard()
        {
            ConsoleHelper.PrintHeader("PANEL UŻYTKOWNIKA");
            Console.WriteLine("1. Przeglądaj wydarzenia");
            Console.WriteLine("2. Moje rezerwacje");
            Console.WriteLine("3. Zmień hasło");
            Console.WriteLine("4. Wyloguj się");
        }
    }
}
