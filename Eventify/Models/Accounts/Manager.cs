using Eventify.Models.Accounts;
using System;

public class Manager : User
{
    public Manager(string username) : base(username, "Manager") { }

    public override void DisplayMenu()
    {
        Console.WriteLine("Manager Menu:\n1. Dodaj wydarzenie\n2. Wyświetl wydarzenia\n3. Wyloguj");
    }
}