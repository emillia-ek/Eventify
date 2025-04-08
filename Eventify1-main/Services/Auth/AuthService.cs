using Eventify.Models.Accounts;
using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.IO;

namespace Eventify.Services.Auth
{
    public class AuthService
    {
        private const string UsersFilePath = "Data/users.txt";
        private readonly PasswordHasher _passwordHasher;
        

        public AuthService()
        {
            _passwordHasher = new PasswordHasher();
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            // Jeśli plik users.txt nie istnieje, utwórz go z domyślnym adminem
            if (!File.Exists(UsersFilePath))
            {
                var defaultAdmin = "admin|5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8|Admin|admin@example.com";
                File.WriteAllText(UsersFilePath, defaultAdmin);
            }
        }
        public bool DeleteUser(string usernameToDelete)
        {
            try
            {
                if (!File.Exists(UsersFilePath))
                {
                    ConsoleHelper.PrintError("Błąd systemu: brak pliku z użytkownikami.");
                    return false;
                }

                var lines = File.ReadAllLines(UsersFilePath).ToList();
                var userToRemove = lines.FirstOrDefault(line => line.Split('|')[0] == usernameToDelete);

                if (userToRemove == null)
                {
                    ConsoleHelper.PrintError("Użytkownik o podanej nazwie nie istnieje.");
                    return false;
                }

                lines.Remove(userToRemove);
                File.WriteAllLines(UsersFilePath, lines);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd podczas usuwania użytkownika: {ex.Message}");
                return false;
            }
        }

        public User Login(string username, string password)
        {
            try
            {
                if (!File.Exists(UsersFilePath))
                {
                    ConsoleHelper.PrintError("Błąd systemu: brak pliku z użytkownikami.");
                    return null;
                }

                var lines = File.ReadAllLines(UsersFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4 && parts[0] == username)
                    {
                        if (_passwordHasher.VerifyPassword(password, parts[1]))
                        {
                            return CreateUserInstance(parts[0], parts[1], parts[2], parts[3]);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd podczas logowania: {ex.Message}");
                return null;
            }
        }

        public bool Register(string username, string password, string email, string role = "User")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
                {
                    ConsoleHelper.PrintError("Wszystkie pola są wymagane.");
                    return false;
                }

                if (IsUsernameTaken(username))
                {
                    ConsoleHelper.PrintError("Nazwa użytkownika jest już zajęta.");
                    return false;
                }

                var passwordHash = _passwordHasher.HashPassword(password);
                var userRecord = $"{username}|{passwordHash}|{role}|{email}";

                File.AppendAllText(UsersFilePath, userRecord + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd podczas rejestracji: {ex.Message}");
                return false;
            }
        }

        private bool IsUsernameTaken(string username)
        {
            if (!File.Exists(UsersFilePath)) return false;

            var lines = File.ReadAllLines(UsersFilePath);
            return lines.Any(line => line.Split('|')[0] == username);
        }

        private User CreateUserInstance(string username, string passwordHash, string role, string email)
        {
            switch (role)
            {
                case "Admin":
                    return new Admin(username, passwordHash, email);
                case "Manager":
                    return new Manager(username, passwordHash, email);
                default:
                    return new RegularUser(username, passwordHash, email);
            }
        }
    }
}
