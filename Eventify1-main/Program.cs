using Eventify.Models.Accounts;
using Eventify.Services.Auth;
using Eventify.Services.Events;
using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eventify.Services.Reservations;

class Program
{

    private const string UsersFilePath = "Data/users.txt";
    private static AuthService _authService = new AuthService();
    private static User _currentUser;
    private static EventService _eventService = new EventService();
    private static EventConsoleUI _eventConsoleUI = new EventConsoleUI(_eventService);
    private static ReservationService _reservationService = new ReservationService();
    static Program()
    {
        _reservationService.ReservationMade += _reservationService.SaveReservationToFile;
    }




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
                    ConsoleHelper.PrintSuccess("Do zobaczenia!");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.PrintError("Nieprawidłowy wybór. Spróbuj ponownie.");
                    Console.ReadKey();
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

    static void ShowManagerMenu()
    {
        ConsoleHelper.PrintHeader("PANEL MANAGERA");
        Console.WriteLine("1. Zarządzaj użytkownikami");
        Console.WriteLine("2. Wyświetl wydarzenia");
        Console.WriteLine("3. Wyloguj się");
        Console.Write("\nWybierz opcję: ");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ShowUserManagementMenu();
                break;
            case "2":
                DisplayAllEvents();
                break;
            case "3":
                return;
            default:
                ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                Console.ReadKey();
                break;
        }


    }
    static void ShowUserMenu()
    {

        ConsoleHelper.PrintHeader("PANEL UZYTKOWNIKA");
        Console.WriteLine("1. Przeglądaj wydarzenia");
        Console.WriteLine("2. Wyświetl moje rezerwacje");
        Console.WriteLine("3. Wyloguj się");
        Console.Write("\nWybierz opcję: ");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                DisplayAllEvents();//dodac metode
                Console.WriteLine("\nCzy chesz zarezerwować wydarzenie?");
                Console.WriteLine("1. Tak");
                Console.WriteLine("2. Nie, dziękuję. Powrót");
                int c = int.Parse(Console.ReadLine());
                if (c == 1)
                {
                    ReserveEvent();
                    return;
                }
                else
                {
                    return;
                }
                break;
            case "2":
                DisplayUserReservations();
                break;
            case "3":
                return; 
            default:
                ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                Console.ReadKey();
                break;
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

    static void DisplayAllEvents() //powinno zadziałac
    {
        ConsoleHelper.PrintHeader("WSZYSTKIE WYDARZENIA");

        var events = _eventService.GetAllEvents();

        if (events == null || !events.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Brak dostępnych wydarzeń do wyświetlenia.");
            Console.ResetColor();

        }
        else
        {
            foreach (var ev in events)
            {
                Console.WriteLine($"ID: {ev.Id}");
                Console.WriteLine($"Nazwa: {ev.Name}");
                Console.WriteLine($"Data rozpoczecia: {ev.StartDate}");
                Console.WriteLine($"Data zakończenia: {ev.EndDate}");
                Console.WriteLine($"Lokalizacja: {ev.Location}");
                Console.WriteLine($"Opis: {ev.Description}");
                Console.WriteLine($"Maksymalna ilość osób: {ev.MaxParticipants}");
                Console.WriteLine($"Cena: {ev.Price}");
                Console.WriteLine(new string('-', 30));
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void DisplayUserReservations()
    {
        ConsoleHelper.PrintHeader("TWOJE REZERWACJE");

        var reservations = _reservationService.GetReservationsForUser(_currentUser.Username);

        if (reservations.Count == 0)
        {
            Console.WriteLine("Brak zarezerwowanych wydarzeń.");
        }
        else
        {
            foreach (var eventId in reservations)
            {
                Console.WriteLine($"- ID wydarzenia: {eventId}");
                // Można tu dodać _eventService.GetEventById(eventId) jeśli chcesz wyświetlić nazwę wydarzenia
            }
        }

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    static void ReserveEvent()
    {
        Console.Write("Podaj ID wydarzenia, które chcesz zarezerwować: ");
        var eventId = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(eventId))
        {
            ConsoleHelper.PrintError("Nie podano ID wydarzenia.");
            return;
        }

        _reservationService.MakeReservation(_currentUser.Username, eventId);
        ConsoleHelper.PrintSuccess("Rezerwacja została złożona!");
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

    static void ShowUserDashboard() //zaleznie od roli usera
    {
        if (_currentUser == null) return;

        while (true)
        {

            ConsoleHelper.PrintHeader($" {_currentUser.Username} - {_currentUser.Role}");
            _currentUser.DisplayDashboard(); // np. pokazuje opcje zalezne od roli


            if (_currentUser.Role == "Admin")
            {
                ShowAdminMenu();
                return;
            }
            else if (_currentUser.Role == "Manager")
            {
                {
                    ShowManagerMenu();
                    return;
                }
            }
            else // Zwykły użytkownik
            {
                ShowUserMenu();
                return;


                }
            }
        }
    }
