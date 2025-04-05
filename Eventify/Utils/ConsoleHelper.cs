using System;
using System.Globalization;

namespace Eventify.Utils
{
    public static class ConsoleHelper
    {
        public static void DisplayWelcomeMessage(string title = "SYSTEM EVENTIFY")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', title.Length));
            Console.WriteLine(title);
            Console.WriteLine(new string('=', title.Length));
            Console.ResetColor();
        }

        public static string ReadString(string prompt, bool required = true)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
                if (!required) break;
            } while (string.IsNullOrEmpty(input));
            return input;
        }

        public static int ReadInt(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            int result;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out result) || result < minValue || result > maxValue);
            return result;
        }

        public static decimal ReadDecimal(string prompt, decimal minValue = decimal.MinValue)
        {
            decimal result;
            do
            {
                Console.Write(prompt);
            } while (!decimal.TryParse(Console.ReadLine(), NumberStyles.Currency, CultureInfo.CurrentCulture, out result) || result < minValue);
            return result;
        }

        public static DateTime ReadDateTime(string prompt)
        {
            DateTime result;
            do
            {
                Console.Write(prompt);
            } while (!DateTime.TryParse(Console.ReadLine(), out result));
            return result;
        }

        public static bool Confirm(string question)
        {
            Console.Write($"{question} (T/N): ");
            return Console.ReadLine()?.ToUpper() == "T";
        }

        public static void WaitForUser()
        {
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        public static void DisplaySuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void DisplayGoodbyeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nDziękujemy za skorzystanie z systemu Eventify!");
            Console.ResetColor();
        }
    }
}