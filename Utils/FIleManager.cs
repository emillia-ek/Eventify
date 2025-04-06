using Eventify.Models.Accounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Utils
{
    public class FileManager
    {
        private string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.txt");

        public List<User> LoadUsers()
        {
            var users = new List<User>();
            try
            {
                foreach (var line in File.ReadLines(_filePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        var username = parts[0].Trim();
                        var passwordHash = parts[1].Trim();
                        var role = parts[2].Trim();
                        users.Add(new RegularUser(username, passwordHash, role));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
            return users;
        }
    }

}
