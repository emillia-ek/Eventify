using Eventify.Models.Accounts;
using Eventify.Services.Auth;
using Eventify.Services.Events;
using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Services.Reservations;
using Eventify.Models.Events;
using Figgle;
using Spectre.Console;
using System.Runtime.CompilerServices;




class Program
{

    private const string UsersFilePath = "Data/users.txt";
    private static AuthService _authService = new AuthService();
    private static User _currentUser;
    private static EventService _eventService = new EventService();
    private static EventConsoleUI _eventConsoleUI = new EventConsoleUI(_eventService);
    private static ReservationService _reservationService = new ReservationService(_eventService);


    static void Main()
    {

        Console.Title = "Eventify - System Zarządzania Wydarzeniami";
        Console.CursorVisible = false;
        Notifier notifier = new Notifier();
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
                    
                    ConsoleHelper.PrintError("Nieprawidłowy wybór. Spróbuj ponownie.");
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
            //AnsiConsole.MarkupLine($"[green]Zalogowano pomyślnie jako {_currentUser.Role}![/]");
            ConsoleHelper.PrintSuccess($"Zalogowano pomyślnie jako {_currentUser.Role}!");
            Console.ReadKey();
            ShowUserDashboard();
        }
        else
        {
            //AnsiConsole.MarkupLine("[red][/]");
            ConsoleHelper.PrintError("Nieprawidłowa nazwa użytkownika lub hasło.");
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

            ConsoleHelper.PrintError("Hasła nie są identyczne!");
            Console.ReadKey();
            return;
        }

        if (_authService.Register(username, password, email))
        {
            
            ConsoleHelper.PrintSuccess("Rejestracja zakończona pomyślnie! Możesz się teraz zalogować.");
            Console.ReadKey();
        }
        else
        {
            
            ConsoleHelper.PrintError("Rejestracja nie powiodła się. Użytkownik może już istnieć.");
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
                AnsiConsole.MarkupLine("[grey]Brak pliku z użytkownikami.[/]");
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
            
            ConsoleHelper.PrintError($"Błąd podczas wczytywania użytkowników: {ex.Message}");
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
            
            ConsoleHelper.PrintError("Nie podano nazwy użytkownika.");
            Console.ReadKey();
            return;
        }

        if (username == _currentUser?.Username)
        {
            
            ConsoleHelper.PrintError("Nie możesz usunąć swojego własnego konta!");
            Console.ReadKey();
            return;
        }

        if (_authService.DeleteUser(username))
        {
            
            ConsoleHelper.PrintSuccess($"Użytkownik {username} został pomyślnie usunięty.");
        }
        else
        {
            
            ConsoleHelper.PrintError("Nie udało się usunąć użytkownika.");
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
            else if (_currentUser.Role == "Manager")
            {
                ShowManagerMenu();
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
                    Console.Clear();
                    _eventConsoleUI.DisplayAllEvents();
                    break;

                case "Zarezerwuj wydarzenie":
                    _eventConsoleUI.DisplayAllEvents(false); // Wyświetl bez czekania na klawisz
                    AnsiConsole.Write("\n[yellow]Podaj ID wydarzenia do rezerwacji:[/] ");
                    var input = Console.ReadLine();

                    if (int.TryParse(input, out var eventId))
                    {
                        _reservationService.Reserve(_currentUser.Username, eventId);
                    }
                    else
                    {
                        ConsoleHelper.PrintError("Nieprawidłowy format ID.");
                    }

                    Console.ReadKey();
                    break;

                case "Moje rezerwacje":
                    _reservationService.ShowUserReservations(_currentUser.Username);
                    Console.ReadKey();
                    break;

                case "Wyloguj się":
                    _currentUser = null;
                    return;

                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }


    static void ShowManagerMenu()
    {
        while (true)
        {
            

            Console.Clear();
            AnsiConsole.Write(new Rule("[red]PANEL MENADŻERA[/]").RuleStyle("grey").Centered());

            

            var menu = new SelectionPrompt<string>()
                .Title("[yellow]Wybierz opcję:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                    "Zarządzaj wydarzeniami",
                    "Przeglądaj rezerwacje",
                    "Generuj raporty",
                    "Wyloguj się"
                });

            var choice = AnsiConsole.Prompt(menu);

            
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
                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }
    static void GenerateReports()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]GENEROWANIE RAPORTÓW[/]").RuleStyle("grey").Centered());

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Wybierz typ raportu:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                    "Raport uczestnictwa w wydarzeniach",
                    "Raport finansowy",
                    "Raport popularności wydarzeń",
                    "Powrót"
                }));

            switch (choice)
            {
                case "Raport uczestnictwa w wydarzeniach":
                    GenerateUserCountReport();
                    break;
                case "Raport finansowy":
                    GenerateFinancialReport();
                    break;
                case "Raport popularności wydarzeń":
                    GeneratePopularityReport();
                    break;
                case "Powrót":
                    return;
            }
            
        }
    }

    static void GenerateUserCountReport()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]RAPORT UCZESTNICTWA[/]").RuleStyle("grey").Centered());

        var reservations = _reservationService.GetAllReservations();

        // Grupowanie po użytkownikach
        var userActivity = reservations
            .GroupBy(r => r.Username)
            .Select(g => new {
                Username = g.Key,
                TotalReservations = g.Count(),
                EventsCount = g.Select(r => r.EventId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalReservations)
            .ToList();

        var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Grey)
        .Title("[yellow]Aktywność użytkowników[/]")
        .AddColumn(new TableColumn("[white]Użytkownik[/]").LeftAligned())
        .AddColumn(new TableColumn("[white]Liczba rezerwacji[/]").Centered())
        .AddColumn(new TableColumn("[white]Różnych wydarzeń[/]").Centered());

        foreach (var user in userActivity)
        {
            table.AddRow(
            user.Username,
            user.TotalReservations.ToString(),
            user.EventsCount.ToString());

        }

        AnsiConsole.Render(table);
        AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
        Console.ReadKey();
    }

    static void GenerateFinancialReport()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]RAPORT FINANSOWY[/]").RuleStyle("grey").Centered());

        var events = _eventService.GetAllEvents();
        var reservations = _reservationService.GetAllReservations();

        decimal totalRevenue = 0;

        var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Grey)
        .Title("[yellow]Przychody z wydarzeń[/]")
        .AddColumn(new TableColumn("[white]Nazwa wydarzenia[/]").LeftAligned())
        .AddColumn(new TableColumn("[white]Przychód[/]").Centered())
        .AddColumn(new TableColumn("[white]Liczba rezerwacji[/]").Centered());

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

        AnsiConsole.Render(table);

        var panel = new Panel($"[green]{totalRevenue.ToString("C")}[/]")
            .Header("[white]Łączny przychód:[/]")
            .BorderColor(Color.Gold1);

        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
        Console.ReadKey();
    }

    static void GeneratePopularityReport()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]RAPORT POPULARNOŚCI[/]").RuleStyle("grey").Centered());

        var events = _eventService.GetAllEvents();
        var reservations = _reservationService.GetAllReservations();

        var eventPopularity = events
            .Select(e => new
            {
                Event = e,
                ReservationsCount = reservations.Count(r => r.EventId == e.Id),
                OccupancyRate = (double)reservations.Count(r => r.EventId == e.Id) / e.MaxParticipants * 100
            })
            .OrderByDescending(x => x.ReservationsCount)
            .ToList();

        var table = new Table()
       .Border(TableBorder.Rounded)
       .BorderColor(Color.Grey)
       .Title("[yellow]Ranking popularności wydarzeń[/]")
       .AddColumn(new TableColumn("[white]Co to jest[/]").Centered())
       .AddColumn(new TableColumn("[white]Miejsce[/]").Centered())
       .AddColumn(new TableColumn("[white]Nazwa wydarzenia[/]").LeftAligned())
       .AddColumn(new TableColumn("[white]Liczba rezerwacji[/]").Centered())
       .AddColumn(new TableColumn("[white]Procent udzialu[/]").Centered());

        for (int i = 0; i < eventPopularity.Count; i++)
        {
            
            string occupancyColor = "";
            switch (eventPopularity[i].OccupancyRate)
            {
                case 90: occupancyColor = "red"; break;
                case 70: occupancyColor = "yellow"; break;
                case 50: occupancyColor = "green"; break;
                default: occupancyColor = "white"; break;

            }

            
            table.AddRow(
                (i + 1).ToString(),
                eventPopularity[i].Event.Location,
                eventPopularity[i].Event.Name,
                eventPopularity[i].ReservationsCount.ToString(),
                $"[{occupancyColor}]{eventPopularity[i].OccupancyRate:0}%[/]"
            );
        }

        AnsiConsole.Render(table);

        var statsPanel = new Panel(
            new Rows(
                new Text($"Łączna liczba wydarzeń: {events.Count}"),
                new Text($"Łączna liczba rezerwacji: {reservations.Count}"),
                new Text($"Średnie obłożenie: {eventPopularity.Average(x => x.OccupancyRate):0}%")
            ))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Grey),
            Header = new PanelHeader("[white]Podsumowanie[/]"),
            Padding = new Padding(2, 1, 2, 1)
        };

        AnsiConsole.Write(statsPanel);
        AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
        Console.ReadKey();
    }

}