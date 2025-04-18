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
        ShowMainMenu();
    }

    static void ShowMainMenu()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("WITAMY W SYSTEMIE EVENTIFY");
            Console.WriteLine("1. Zaloguj się");
            Console.WriteLine("2. Zarejestruj się");
            Console.WriteLine("3. Wyjdź");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    Register();
                    break;
                case "3":
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
        ConsoleHelper.PrintHeader("LOGOWANIE");
        Console.Write("Nazwa użytkownika: ");
        var username = Console.ReadLine() ?? "";
        Console.Write("Hasło: ");
        var password = ConsoleHelper.ReadPassword();

        _currentUser = _authService.Login(username, password);

        if (_currentUser != null)
        {
            ConsoleHelper.PrintSuccess($"Zalogowano pomyślnie jako {_currentUser.Role}!");
            Console.ReadKey();
            ShowUserDashboard();
        }
        else
        {
            ConsoleHelper.PrintError("Nieprawidłowa nazwa użytkownika lub hasło.");
            Console.ReadKey();
        }
    }

    static void Register()
    {
        ConsoleHelper.PrintHeader("REJESTRACJA");
        Console.Write("Nazwa użytkownika: ");
        var username = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        var email = Console.ReadLine() ?? "";
        Console.Write("Hasło: ");
        var password = ConsoleHelper.ReadPassword();
        Console.Write("Potwierdź hasło: ");
        var confirmPassword = ConsoleHelper.ReadPassword();

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
    }
    static void ShowAdminMenu()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("PANEL ADMINISTRATORA");
            Console.WriteLine("1. Zarządzaj użytkownikami");
            Console.WriteLine("2. Zarządzaj wydarzeniami");
            Console.WriteLine("3. Wyloguj się");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowUserManagementMenu();
                    break;
                case "2":
                    _eventConsoleUI.ShowEventManagementMenu();
                    break;
                case "3":
                    return;
                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ShowUserManagementMenu()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("ZARZĄDZANIE UŻYTKOWNIKAMI");
            Console.WriteLine("1. Wyświetl wszystkich użytkowników");
            Console.WriteLine("2. Usuń użytkownika");
            Console.WriteLine("3. Powrót");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    DisplayAllUsers();
                    break;
                case "2":
                    DeleteUser();
                    break;
                case "3":
                    return;
                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void DisplayAllUsers()
    {
        ConsoleHelper.PrintHeader("LISTA UŻYTKOWNIKÓW");

        try
        {
            if (!File.Exists(UsersFilePath))
            {
                ConsoleHelper.PrintError("Brak pliku z użytkownikami.");
                return;
            }

            var lines = File.ReadAllLines(UsersFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                Console.WriteLine($"Nazwa: {parts[0]}, Rola: {parts[2]}, Email: {parts[3]}");
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Błąd podczas wczytywania użytkowników: {ex.Message}");
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        ConsoleHelper.PrintHeader("USUWANIE UŻYTKOWNIKA");
        DisplayAllUsers();

        Console.Write("\nPodaj nazwę użytkownika do usunięcia: ");
        var username = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username))
        {
            ConsoleHelper.PrintError("Nie podano nazwy użytkownika.");
            return;
        }

        if (username == _currentUser?.Username)
        {
            ConsoleHelper.PrintError("Nie możesz usunąć swojego własnego konta!");
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
            ConsoleHelper.PrintHeader("PANEL UŻYTKOWNIKA");
            Console.WriteLine("1. Przeglądaj wydarzenia");
            Console.WriteLine("2. Zarezerwuj wydarzenie");
            Console.WriteLine("3. Moje rezerwacje");
            Console.WriteLine("4. Wyloguj się");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    _eventConsoleUI.DisplayAllEvents();
                    break;

                case "2":
                    _eventConsoleUI.DisplayAllEvents(false); // Wyświetl bez czekania na klawisz
                    Console.Write("\nPodaj ID wydarzenia do rezerwacji: ");
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

                case "3":
                    _reservationService.ShowUserReservations(_currentUser.Username);
                    Console.ReadKey();
                    break;

                case "4":
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
            ConsoleHelper.PrintHeader("PANEL MENADŻERA");
            Console.WriteLine("1. Zarządzaj wydarzeniami");
            Console.WriteLine("2. Przeglądaj rezerwacje");
            Console.WriteLine("3. Generuj raporty");
            Console.WriteLine("4. Wyloguj się");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    _eventConsoleUI.ShowEventManagementMenu();
                    break;
                case "2":
                    _reservationService.ShowAllReservations();
                    Console.ReadKey();
                    break;
                case "3":
                    GenerateReports();
                    break;
                case "4":
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
            ConsoleHelper.PrintHeader("GENEROWANIE RAPORTÓW");
            Console.WriteLine("1. Raport uczestnictwa w wydarzeniach");
            Console.WriteLine("2. Raport finansowy");
            Console.WriteLine("3. Raport popularności wydarzeń");
            Console.WriteLine("4. Powrót");
            Console.Write("\nWybierz opcję: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    GenerateUserCountReport();
                    break;
                case "2":
                    GenerateFinancialReport();
                    break;
                case "3":
                    GeneratePopularityReport();
                    break;
                case "4":
                    return;
                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                    break;
            }
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }
    }

    static void GenerateUserCountReport()
    {
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

        Console.WriteLine($"{"Użytkownik",-20} {"Liczba rezerwacji",-18} {"Różnych wydarzeń",-18}");
        Console.WriteLine(new string('=', 56));

        foreach (var user in userActivity)
        {
            Console.WriteLine($"{user.Username,-20} {user.TotalReservations,-18} {user.EventsCount,-18}");
        }
    }

    static void GenerateFinancialReport()
    {
        ConsoleHelper.PrintHeader("RAPORT FINANSOWY");

        var events = _eventService.GetAllEvents();
        var reservations = _reservationService.GetAllReservations();

        decimal totalRevenue = 0;
        Console.WriteLine($"{"Nazwa wydarzenia",-40} {"Przychód",-15} {"Liczba rezerwacji",-20}");
        Console.WriteLine(new string('=', 75));

        foreach (var eventItem in events)
        {
            var eventReservations = reservations.Where(r => r.EventId == eventItem.Id).ToList();
            var eventRevenue = eventReservations.Count * eventItem.Price;
            totalRevenue += eventRevenue;

            Console.WriteLine($"{eventItem.Name,-40} {eventRevenue.ToString("C"),-15} {eventReservations.Count,-20}");
        }

        Console.WriteLine($"\nŁączny przychód: {totalRevenue.ToString("C")}");
    }

    static void GeneratePopularityReport()
    {
        ConsoleHelper.PrintHeader("RAPORT POPULARNOŚCI WYDARZEŃ");

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

        Console.WriteLine($"{"Miejsce",-10} {"Nazwa wydarzenia",-40} {"Liczba rezerwacji",-20}");
        Console.WriteLine(new string('=', 70));

        for (int i = 0; i < eventPopularity.Count; i++)
        {
            Console.WriteLine($"{i + 1,-10} {eventPopularity[i].Event.Name,-40} {eventPopularity[i].ReservationsCount,-20}");
        }
    }

}