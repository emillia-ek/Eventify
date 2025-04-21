using Eventify.Models;
using Eventify.Services.Events;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Utils
{
    public static class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', title.Length + 4));
            Console.WriteLine($"  {title}  ");
            Console.WriteLine(new string('=', title.Length + 4));
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void PrintSuccess(string message)
        {
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("");
            var errPanel = new Panel(new Rows(new Text(message))) { Border = BoxBorder.Rounded, Header = new PanelHeader("[green]SUKCES![/]"), Padding = new Padding(1, 1, 1, 1) };
            AnsiConsole.Write(errPanel);
        }

        public static void PrintError(string message)
        {
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("");
            var errPanel = new Panel(new Rows(new Text(message))) { Border = BoxBorder.Rounded, Header = new PanelHeader("[red]UWAGA![/]"), Padding = new Padding(1, 1, 1, 1) };
            AnsiConsole.Write(errPanel);

        }
        public static void PrintInfo(string message)
        {
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("");
            var errPanel = new Panel(new Rows(new Text(message))) { Border = BoxBorder.Rounded, Header = new PanelHeader("[yellow]INFO[/]"), Padding = new Padding(1, 1, 1, 1) };
            AnsiConsole.Write(errPanel);

        }

        public static string ReadPassword()
        {
            var password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password.ToString();
        }
    }
}
