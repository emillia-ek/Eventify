using Eventify.Models.Accounts;
using System;

public class Admin : User
{
    public Admin(string username) : base(username, "Admin") { }

    public override void DisplayMenu()
    {
        Console.WriteLine("Admin Menu:\n1. Dodaj wydarzenie\n2. Usuń wydarzenie\n3. Wyświetl logi\n4. Wyloguj");
    }
}