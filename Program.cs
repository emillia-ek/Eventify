using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eventify.Models;
using Eventify.Models.Accounts;
using Eventify.Services.Auth;
using Eventify.Services.Events;
using Eventify.Services.Reservations;
using Eventify.Utils;
using Eventify.Events;
using Spectre.Console;

class Program
{
    private static AuthService _authService = new AuthService();
    private static User _currentUser;
    private static EventService _eventService = new EventService();
    private static ReservationService _reservationService = new ReservationService(_eventService);
    private static EventConsoleUI _eventConsoleUI = new EventConsoleUI(_eventService);
    private const string UsersFilePath = "Data/users.txt";
    private const string LogFilePath = "Data/user_events.txt";

    private static void OnReservationCancelled(object sender, Reservation reservation)
    {
        AnsiConsole.MarkupLine($"[bold hotpink2][[INFO]][/] Użytkownik [pink1]{reservation.Username}[/] anulował rezerwację ID: [pink1]{reservation.Id}[/].");
    }

    static void Main()
    {
        Console.Title = "Eventify - System Zarządzania Wydarzeniami";
        InitializeDataDirectory();
        _reservationService.ReservationCancelled += OnReservationCancelled;
        AppEvents.UserLoggedIn += OnUserAction;
        AppEvents.UserRegistered += OnUserAction;
        AppEvents.UserDeleted += OnUserAction;

        ShowMainMenu();
    }

    private static void OnUserAction(object sender, UserActionEventArgs e)
    {
        try
        {
            //AnsiConsole.MarkupLine($"[bold hotpink2][[EVENT]][/] [pink1]{e.ActionType}[/] | Użytkownik: [pink1]{e.Username}[/] | Czas: [pink1]{e.ActionTime}[/]");

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            File.AppendAllText("Data/user_events.txt", $"{e.ActionTime:u} | {e.ActionType} | {e.Username}{Environment.NewLine}");
            File.AppendAllText(LogFilePath, "log");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Błąd podczas zapisywania do pliku:[/] [pink1]{ex.Message}[/]");
        }
    }

    static void ShowMainMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleHelper.PrintHeader("WITAMY W SYSTEMIE EVENTIFY");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Wybierz opcję:[/]")
                    .AddChoices(new[] {
                        "Zaloguj się",
                        "Zarejestruj się",
                        "Wyjdź"
                    }));

            switch (choice)
            {
                case "Zaloguj się":
                    Login();
                    break;
                case "Zarejestruj się":
                    Register();
                    break;
                case "Wyjdź":
                    Environment.Exit(0);
                    break;
            }
        }
    }

    static void InitializeDataDirectory()
    {
        if (!Directory.Exists("Data"))
            Directory.CreateDirectory("Data");
    }

    static void Login()
    {
        AnsiConsole.Clear();
        ConsoleHelper.PrintHeader("LOGOWANIE");

        var username = AnsiConsole.Ask<string>("[pink1]Nazwa użytkownika:[/] ");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[pink1]Hasło:[/] ")
                .Secret());

        _currentUser = _authService.Login(username, password);

        if (_currentUser != null)
        {
            AnsiConsole.MarkupLine($"[bold hotpink2]Zalogowano pomyślnie jako [pink1]{_currentUser.Role}[/]![/]");
            AppEvents.OnUserLoggedIn(_currentUser.Username);
            Console.ReadKey();
            ShowUserDashboard();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Nieprawidłowa nazwa użytkownika lub hasło.[/]");
            Console.ReadKey();
        }
    }

    static void Register()
    {
        AnsiConsole.Clear();
        ConsoleHelper.PrintHeader("REJESTRACJA");

        var username = AnsiConsole.Ask<string>("[pink1]Nazwa użytkownika:[/] ");
        var email = AnsiConsole.Ask<string>("[pink1]Email:[/] ");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[pink1]Hasło:[/] ")
                .Secret());
        var confirmPassword = AnsiConsole.Prompt(
            new TextPrompt<string>("[pink1]Potwierdź hasło:[/] ")
                .Secret());

        if (password != confirmPassword)
        {
            AnsiConsole.MarkupLine("[bold red]Hasła nie są identyczne![/]");
            Console.ReadKey();
            return;
        }

        if (_authService.Register(username, password, email))
        {
            AnsiConsole.MarkupLine("[bold hotpink2]Rejestracja zakończona pomyślnie! Możesz się teraz zalogować.[/]");
            AppEvents.OnUserRegistered(username);
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]Użytkownik o nazwie '{username}' już istnieje![/]");
        }

        Console.ReadKey();
    }


    static void ShowAdminMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleHelper.PrintHeader("PANEL ADMINISTRATORA");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Wybierz opcję:[/]")
                    .AddChoices(new[] {
                        "Zarządzaj użytkownikami",
                        "Zarządzaj wydarzeniami",
                        "Wyloguj się"
                    }));

            switch (choice)
            {
                case "Zarządzaj użytkownikami":
                    ShowUserManagementMenu();
                    break;
                case "Zarządzaj wydarzeniami":
                    _eventConsoleUI.ShowEventManagementMenu();
                    break;
                case "Wyloguj się":
                    return;
            }
        }
    }

    static void ShowUserManagementMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();

            ConsoleHelper.PrintHeader("ZARZĄDZANIE UŻYTKOWNIKAMI");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Wybierz opcję:[/]")
                    .AddChoices(new[] {
                        "Wyświetl wszystkich użytkowników",
                        "Usuń użytkownika",
                        "Powrót"
                    }));

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
        AnsiConsole.Clear();
        ConsoleHelper.PrintHeader("LISTA UŻYTKOWNIKÓW");

        try
        {
            if (!File.Exists(UsersFilePath))
            {
                AnsiConsole.MarkupLine("[bold red]Brak pliku z użytkownikami.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.AddColumn("[pink1]Nazwa[/]");
            table.AddColumn("[pink1]Rola[/]");
            table.AddColumn("[pink1]Email[/]");

            var lines = File.ReadAllLines(UsersFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                table.AddRow(parts[0], parts[2], parts[3]);
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Błąd podczas wczytywania użytkowników:[/] [pink1]{ex.Message}[/]");
        }

        AnsiConsole.MarkupLine("\n[pink1]Naciśnij dowolny klawisz, aby kontynuować...[/]");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        DisplayAllUsers();
        var username = AnsiConsole.Ask<string>("\n[pink1]Podaj nazwę użytkownika do usunięcia:[/] ");

        if (string.IsNullOrWhiteSpace(username))
        {
            AnsiConsole.MarkupLine("[bold red]Nie podano nazwy użytkownika.[/]");
            return;
        }

        if (username == _currentUser?.Username)
        {
            AnsiConsole.MarkupLine("[bold red]Nie możesz usunąć swojego własnego konta![/]");
            return;
        }

        if (_authService.DeleteUser(username))
        {
            AnsiConsole.MarkupLine($"[bold hotpink2]Użytkownik [pink1]{username}[/] został pomyślnie usunięty.[/]");
            AppEvents.OnUserDeleted(username);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Nie udało się usunąć użytkownika.[/]");
        }

        Console.ReadKey();
    }

    static void ShowManagerMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleHelper.PrintHeader("PANEL MENADŻERA");
            var panel = new Panel("[bold hotpink2]PANEL MENADŻERA[/]")
                .BorderColor(Color.HotPink2);
            AnsiConsole.Write(panel);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Wybierz opcję:[/]")
                    .AddChoices(new[] {
                        "Zarządzaj wydarzeniami",
                        "Przeglądaj rezerwacje",
                        "Generuj raporty",
                        "Wyloguj się"
                    }));

            switch (choice)
            {
                case "Zarządzaj wydarzeniami":
                    _eventConsoleUI.ShowEventManagementMenu();
                    break;
                case "Przeglądaj rezerwacje":
                    _reservationService.ShowAllReservations();
                    Console.ReadKey();
                    break;
                case "Generuj raporty":
                    GenerateReports();
                    break;
                case "Wyloguj się":
                    _currentUser = null;
                    return;
            }
        }
    }

    static void GenerateReports()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleHelper.PrintHeader("GENEROWANIE RAPORTÓW");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Wybierz opcję:[/]")
                    .AddChoices(new[] {
                        "Raport uczestnictwa w wydarzeniach",
                        "Raport finansowy",
                        "Raport popularności wydarzeń",
                        "Powrót"
                    }));

            switch (choice)
            {
                case "Raport uczestnictwa w wydarzeniach":

                    ConsoleHelper.PrintHeader("GENEROWANIE RAPORTÓW");
                    GenerateUserCountReport();
                    break;
                case "Raport finansowy":

                    ConsoleHelper.PrintHeader("GENEROWANIE RAPORTÓW");
                    GenerateFinancialReport();
                    break;
                case "Raport popularności wydarzeń":

                    ConsoleHelper.PrintHeader("GENEROWANIE RAPORTÓW");
                    GeneratePopularityReport();
                    break;
                case "Powrót":
                    return;
            }
            AnsiConsole.MarkupLine("\n[pink1]Naciśnij dowolny klawisz, aby kontynuować...[/]");
            Console.ReadKey();
        }
    }

    static void GenerateUserCountReport()
    {
        var reservations = _reservationService.GetAllReservations();

        var userActivity = reservations
            .GroupBy(r => r.Username)
            .Select(g => new {
                Username = g.Key,
                TotalReservations = g.Count(),
                EventsCount = g.Select(r => r.EventId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalReservations)
            .ToList();

        var table = new Table();
        table.AddColumn("[pink1]Użytkownik[/]");
        table.AddColumn("[pink1]Liczba rezerwacji[/]");
        table.AddColumn("[pink1]Różnych wydarzeń[/]");

        foreach (var user in userActivity)
        {
            table.AddRow(user.Username, user.TotalReservations.ToString(), user.EventsCount.ToString());
        }

        AnsiConsole.Write(table);
    }

    static void GenerateFinancialReport()
    {
        var panel = new Panel("[bold hotpink2]RAPORT FINANSOWY[/]")
            .BorderColor(Color.HotPink2);
        AnsiConsole.Write(panel);

        var events = _eventService.GetAllEvents();
        var reservations = _reservationService.GetAllReservations();

        decimal totalRevenue = 0;
        var table = new Table();
        table.AddColumn("[pink1]Nazwa wydarzenia[/]");
        table.AddColumn("[pink1]Przychód[/]");
        table.AddColumn("[pink1]Liczba rezerwacji[/]");

        foreach (var eventItem in events)
        {
            var eventReservations = reservations.Where(r => r.EventId == eventItem.Id).ToList();
            var eventRevenue = eventReservations.Count * eventItem.Price;
            totalRevenue += eventRevenue;

            table.AddRow(
                eventItem.Name,
                eventRevenue.ToString("C"),
                eventReservations.Count.ToString()
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[bold hotpink2]\nŁączny przychód:[/] [pink1]{totalRevenue.ToString("C")}[/]");
    }

    static void GeneratePopularityReport()
    {
        var panel = new Panel("[bold hotpink2]RAPORT POPULARNOŚCI WYDARZEŃ[/]")
            .BorderColor(Color.HotPink2);
        AnsiConsole.Write(panel);

        var events = _eventService.GetAllEvents();
        var reservations = _reservationService.GetAllReservations();

        var eventPopularity = events
            .Select(e => new
            {
                Event = e,
                ReservationsCount = reservations.Count(r => r.EventId == e.Id)
            })
            .OrderByDescending(x => x.ReservationsCount)
            .ToList();

        var table = new Table();
        table.AddColumn("[pink1]Miejsce[/]");
        table.AddColumn("[pink1]Nazwa wydarzenia[/]");
        table.AddColumn("[pink1]Liczba rezerwacji[/]");

        for (int i = 0; i < eventPopularity.Count; i++)
        {
            table.AddRow(
                (i + 1).ToString(),
                eventPopularity[i].Event.Name,
                eventPopularity[i].ReservationsCount.ToString()
            );
        }

        AnsiConsole.Write(table);
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
            else if (_currentUser.Role == "Manager")
            {
                ShowManagerMenu();
                return;
            }
            else if (_currentUser.Role == "User")
            {
                AnsiConsole.Clear();
                ConsoleHelper.PrintHeader("PANEL UŻYTKOWNIKA");
                var panel = new Panel($"[bold hotpink2]Witaj, {_currentUser.Username}![/]")
                    .BorderColor(Color.HotPink2);
                AnsiConsole.Write(panel);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[pink1]Wybierz opcję:[/]")
                        .AddChoices(new[] {
                            "Przeglądaj wydarzenia",
                            "Zarezerwuj wydarzenie",
                            "Moje rezerwacje",
                            "Anuluj rezerwację",
                            "Wyloguj się"
                        }));

                switch (choice)
                {
                    case "Przeglądaj wydarzenia":
                        _eventConsoleUI.DisplayAllEvents();
                        break;
                    case "Zarezerwuj wydarzenie":
                        HandleReservation();
                        break;
                    case "Moje rezerwacje":
                        _reservationService.ShowUserReservations(_currentUser.Username);
                        Console.ReadKey();
                        break;
                    case "Anuluj rezerwację":
                        HandleCancellation();
                        break;
                    case "Wyloguj się":
                        _currentUser = null;
                        return;
                }
            }
        }
    }

    static void HandleReservation()
    {
        _eventConsoleUI.DisplayAllEvents(false);
        var eventId = AnsiConsole.Ask<int>("\n[pink1]Podaj ID wydarzenia do rezerwacji:[/] ");

        if (_reservationService.Reserve(_currentUser.Username, eventId))
            AnsiConsole.MarkupLine("[bold hotpink2]Rezerwacja zakończona sukcesem![/]");
        else
            AnsiConsole.MarkupLine("[bold red]Nie udało się zarezerwować.[/]");

        Console.ReadKey();
    }

    static void HandleCancellation()
    {
        _reservationService.ShowUserReservations(_currentUser.Username);
        var reservationId = AnsiConsole.Ask<int>("\n[pink1]Podaj ID rezerwacji do anulowania:[/] ");

        if (_reservationService.CancelReservation(reservationId, _currentUser.Username))
            AnsiConsole.MarkupLine("[bold hotpink2]Rezerwacja została anulowana![/]");
        else
            AnsiConsole.MarkupLine("[bold red]Nie udało się anulować rezerwacji.[/]");

        Console.ReadKey();
    }
}