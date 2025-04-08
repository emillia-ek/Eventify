using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public class Manager : User
    {
        public Manager(string username, string passwordHash, string email)
            : base(username, passwordHash, "Manager", email) { }

        public override void DisplayDashboard()
        {
            ConsoleHelper.PrintHeader("PANEL MENADŻERA");
            Console.WriteLine("1. Zarządzaj wydarzeniami");
            Console.WriteLine("2. Przeglądaj rezerwacje");
            Console.WriteLine("3. Generuj raporty");
            Console.WriteLine("4. Wyloguj się");
        }
    }
}