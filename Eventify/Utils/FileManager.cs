using Eventify.Models.Accounts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eventify.Utils
{
    public static class FileManager
    {
        /*public static List<User> ReadUsersFromFile(string filePath)
        {
            var users = new List<User>();

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                return users;
            }

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(';');
                if (parts.Length == 3)
                {
                    users.Add(new User
                    {
                        Username = parts[0],
                        PasswordHash = parts[1],
                        Role = parts[2]
                    });
                }
            }

            return users;*/
        }
    }
