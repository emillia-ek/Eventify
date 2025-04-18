using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public class Admin : User
    {
        public Admin(string username, string passwordHash, string email)
            : base(username, passwordHash, "Admin", email) { }

        public override void DisplayDashboard()
        {
            ConsoleHelper.PrintHeader("PANEL ADMINISTRATORA");
            Console.WriteLine("1. Zarządzaj użytkownikami");
            Console.WriteLine("2. Zarządzaj wydarzeniami");
            Console.WriteLine("3. Wyloguj się");
        }
    }
}
