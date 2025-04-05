using Eventify.Models.Accounts;
using System;

public class RegularUser : User
{
    public RegularUser(string username) : base(username, "User") { }

    public override void DisplayMenu()
    {
        Console.WriteLine("User Menu:\n1. Przeglądaj wydarzenia\n2. Zarezerwuj miejsce\n3. Wyloguj");
    }
}