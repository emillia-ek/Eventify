using Eventify.Models.Accounts;
using Eventify.Services.Auth;
using Eventify.Services.Events;
using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eventify.Services.Reservations;
using Eventify.Models.Events;
using Figgle;
using Spectre.Console;


class Program
{

    private const string UsersFilePath = "Data/users.txt";
    private static AuthService _authService = new AuthService();
    private static User _currentUser;
    private static EventService _eventService = new EventService();
    private static EventConsoleUI _eventConsoleUI = new EventConsoleUI(_eventService);
    private static ReservationService _reservationService = new ReservationService();


    static void Main()
    {
        Console.Title = "Eventify - System Zarządzania Wydarzeniami";
        Console.CursorVisible = false;
        ShowMainMenu();
    }

    static void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]EVENTIFY[/]").RuleStyle("grey").Centered());

            AnsiConsole.Write(
                new Panel(FiggleFonts.Standard.Render("EVENTIFY")).Border(BoxBorder.None)); //aligny nie dzialaja

            AnsiConsole.MarkupLine("[grey]System zarządzania wydarzeniami[/]\n");

            
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var menu = new SelectionPrompt<string>()
                .Title("[red]Wybierz opcję:[/]")
                .PageSize(3)
                .AddChoices(new[] {
                    "Zaloguj się",
                    "Zarejestruj się",
                    "Wyjdź"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Zaloguj się":
                    Login();
                    break;
                case "Zarejestruj się":
                    Register();
                    break;
                case "Wyjdź":
                    AnsiConsole.MarkupLine("[grey]Do zobaczenia![/]");
                    Environment.Exit(0);
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Nieprawidłowy wybór. Spróbuj ponownie.[/]");
                    break;
            }

        }

    }

    static void Login()
    {

        Console.Clear();
        AnsiConsole.Write(new Rule("[red]LOGOWANIE[/]").RuleStyle("grey").Centered());

        var username = AnsiConsole.Ask<string>("[yellow]Nazwa użytkownika:[/] ");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Hasło:[/] ")
                .PromptStyle("red")
                .Secret());

        _currentUser = _authService.Login(username, password);

        if (_currentUser != null)
        {
            AnsiConsole.MarkupLine($"[green]Zalogowano pomyślnie jako {_currentUser.Role}![/]");
            Console.ReadKey();
            ShowUserDashboard();
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Nieprawidłowa nazwa użytkownika lub hasło.[/]");
            Console.ReadKey();
        }

       
    }

    static void Register()
    {

        Console.Clear();
        AnsiConsole.Write(new Rule("[red]REJESTRACJA[/]").RuleStyle("grey").Centered());

        var username = AnsiConsole.Ask<string>("[yellow]Nazwa użytkownika:[/] ");
        var email = AnsiConsole.Ask<string>("[yellow]Email:[/] ");

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Hasło:[/] ")
                .PromptStyle("red")
                .Secret());

        var confirmPassword = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Potwierdź hasło:[/] ")
                .PromptStyle("red")
                .Secret());



        if (password != confirmPassword)
        {
            AnsiConsole.MarkupLine("[red]Hasła nie są identyczne![/]");
            Console.ReadKey();
            return;
        }

        if (_authService.Register(username, password, email))
        {
            AnsiConsole.MarkupLine("[green]Rejestracja zakończona pomyślnie! Możesz się teraz zalogować.[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Rejestracja nie powiodła się. Użytkownik może już istnieć.[/]");
            Console.ReadKey();
        }


      
    }
    static void ShowAdminMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]PANEL ADMINISTRATORA[/]").RuleStyle("grey").Centered());

            var menu = new SelectionPrompt<string>()
                .Title("[yellow]Wybierz opcję:[/]")
                .PageSize(4)
                .AddChoices(new[] {
                    "Zarządzaj użytkownikami",
                    "Zarządzaj wydarzeniami",
                    "Wyloguj się"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Zarządzaj użytkownikami":
                    ShowUserManagementMenu();
                    break;
                case "Zarządzaj wydarzeniami":
                    _eventConsoleUI.ShowEventManagementMenu();
                    break;
                case "Wyloguj się":
                    _currentUser = null;
                    return;
            }

           
        }
    }

    static void ShowUserManagementMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]ZARZĄDZANIE UŻYTKOWNIKAMI[/]").RuleStyle("grey").Centered());

            var menu = new SelectionPrompt<string>()
                .Title("[yellow]Wybierz opcję:[/]")
                .PageSize(4)
                .AddChoices(new[] {
                    "Wyświetl wszystkich użytkowników",
                    "Usuń użytkownika",
                    "Powrót"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Wyświetl wszystkich użytkowników":
                    DisplayAllUsers();
                    break;
                case "Usuń użytkownika":
                    DeleteUser();
                    break;
                case "Powrót":
                    return;
            }
        }
    }

    static void DisplayAllUsers()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]LISTA UŻYTKOWNIKÓW[/]").RuleStyle("grey").Centered());

        try
        {
            if (!File.Exists(UsersFilePath))
            {
                AnsiConsole.MarkupLine("[red]Brak pliku z użytkownikami.[/]");
                Console.ReadKey();
                return;
            }

            var users = File.ReadAllLines(UsersFilePath)
                .Select(line => line.Split('|'))
                .Select(parts => new { Username = parts[0], Role = parts[2], Email = parts[3] });

            var table = new Table();
            table.AddColumn("Nazwa użytkownika");
            table.AddColumn("Rola");
            table.AddColumn("Email");

            foreach (var user in users)
            {
                table.AddRow(user.Username, user.Role, user.Email);
            }

            AnsiConsole.Render(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Błąd podczas wczytywania użytkowników: {ex.Message}[/]"); ;
        }

        AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        DisplayAllUsers();

        var username = AnsiConsole.Ask<string>("\n[yellow]Podaj nazwę użytkownika do usunięcia:[/] ");

        if (string.IsNullOrWhiteSpace(username))
        {
            AnsiConsole.MarkupLine("[red]Nie podano nazwy użytkownika.[/]");
            Console.ReadKey();
            return;
        }

        if (username == _currentUser?.Username)
        {
            AnsiConsole.MarkupLine("[red]Nie możesz usunąć swojego własnego konta![/]");
            Console.ReadKey();
            return;
        }

        if (_authService.DeleteUser(username))
        {
            AnsiConsole.MarkupLine($"[green]Użytkownik {username} został pomyślnie usunięty.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Nie udało się usunąć użytkownika.[/]");
        }

        Console.ReadKey();
    }

    static void ShowUserDashboard()
    {
        if (_currentUser == null) return;

        while (true)
        {
            if (_currentUser.Role == "Admin")
            {
                ShowAdminMenu();
                return;
            }

            Console.Clear();
            AnsiConsole.Write(new Rule($"[red]PANEL UŻYTKOWNIKA: {_currentUser.Username}[/]").RuleStyle("grey").Centered());

            var menu = new SelectionPrompt<string>()
                .Title("[yellow]Wybierz opcję:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                    "Przeglądaj wydarzenia",
                    "Zarezerwuj wydarzenie",
                    "Moje rezerwacje",
                    "Wyloguj się"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Przeglądaj wydarzenia":
                    _eventConsoleUI.DisplayAllEvents();
                    break;
                case "Zarezerwuj wydarzenie":
                    _eventConsoleUI.DisplayAllEvents();
                    var eventId = AnsiConsole.Ask<int>("\n[yellow]Podaj ID wydarzenia do rezerwacji:[/] ");
                    _reservationService.Reserve(_currentUser.Username, eventId);
                    Console.ReadKey();
                    break;
                case "Moje rezerwacje":
                    _reservationService.ShowUserReservations(_currentUser.Username);
                    Console.ReadKey();
                    break;
                case "Wyloguj się":
                    _currentUser = null;
                    return;
            }
        }
    }

}