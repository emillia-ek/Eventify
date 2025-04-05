using System;
using System.IO;

namespace Eventify.Utils
{
    public static class FileHelper
    {
        public static void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
        }

        public static void InitializeSampleData()
        {
            // Przykładowa inicjalizacja danych
            if (!File.Exists("Data/users.txt"))
            {
                File.WriteAllText("Data/users.txt",
                    "admin|hashed_admin_password|Admin\n" +
                    "manager|hashed_manager_password|Manager\n" +
                    "user|hashed_user_password|User");
            }
        }
    }
}