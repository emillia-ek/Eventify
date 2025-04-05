using Eventify.Models.Accounts;
using Eventify.Utils;
using Eventify.Services;
using Eventify.Events;
using System;
using System.IO;

public class AuthService
{
    public event Action<string>? LoginSuccess;

    public User? Login(string username, string password)
    {
        try
        {
            var lines = File.ReadAllLines("Data/users.txt");
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length != 3) continue;

                if (parts[0] == username && parts[1] == PasswordHasher.Hash(password))
                {
                    LoginSuccess?.Invoke(username);
                    UserEvents.RaiseUserLoggedIn(username);

                    return RBACService.CreateUserByRole(username, parts[2]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd logowania: {ex.Message}");
        }

        Console.WriteLine("Nieprawidłowy login lub hasło.");
        return null;
    }

    public void Register(string username, string password, string role)
    {
        try
        {
            var hashed = PasswordHasher.Hash(password);
            File.AppendAllText("Data/users.txt", $"{username},{hashed},{role}\n");
            UserEvents.RaiseUserRegistered(username);

            Console.WriteLine("Użytkownik zarejestrowany.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
        }
    }
}
