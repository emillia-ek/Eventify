
using Eventify.Models.Accounts;
using Eventify.Services;
using Eventify.Events;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var authService = new AuthService();
        var eventService = new EventService();
        var reservationService = new ReservationService();
        var logger = new LoggerService();

        // Podpięcie zdarzeń
        UserEvents.OnUserLoggedIn += username => logger.Log($"[LOGIN] {username} zalogował się.");
        UserEvents.OnUserRegistered += username => logger.Log($"[REGISTER] Zarejestrowano: {username}");
        EventEvents.OnReservationCreated += (username, eventId) => logger.Log($"[RESERVATION] {username} → {eventId}");
        EventEvents.OnEventRemoved += eventId => logger.Log($"[DELETE] Usunięto wydarzenie {eventId}");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Eventify ====");
            Console.WriteLine("1. Zaloguj");
            Console.WriteLine("2. Zarejestruj");
            Console.WriteLine("0. Wyjdź");

            var choice = Console.ReadLine();
            if (choice == "0") break;

            if (choice == "1")
            {
                Console.Write("Login: ");
                string login = Console.ReadLine();
                Console.Write("Hasło: ");
                string password = Console.ReadLine();

                var user = authService.Login(login, password);
                if (user == null) continue;

                bool loggedIn = true;
                while (loggedIn)
                {
                    Console.Clear();
                    user.DisplayMenu();
                    string option = Console.ReadLine();

                    switch (user)
                    {
                        case Admin admin:
                            if (option == "1")
                            {
                                var ev = DemoCreateEvent();
                                eventService.AddEvent(ev);
                            }
                            else if (option == "2")
                            {
                                Console.Write("ID wydarzenia do usunięcia: ");
                                var id = Guid.Parse(Console.ReadLine());
                                eventService.RemoveEvent(id);
                            }
                            else if (option == "3")
                            {
                                Console.WriteLine(File.ReadAllText("Data/logs.txt"));
                                Console.ReadKey();
                            }
                            else loggedIn = false;
                            break;

                        case Manager manager:
                            if (option == "1")
                            {
                                var ev = DemoCreateEvent();
                                eventService.AddEvent(ev);
                            }
                            else if (option == "2")
                            {
                                eventService.ShowAllEvents();
                                Console.ReadKey();
                            }
                            else loggedIn = false;
                            break;

                        case RegularUser regular:
                            if (option == "1")
                            {
                                eventService.ShowAllEvents();
                                Console.ReadKey();
                            }
                            else if (option == "2")
                            {
                                Console.Write("ID wydarzenia do rezerwacji: ");
                                var id = Guid.Parse(Console.ReadLine());
                                reservationService.Reserve(user.Username, id);
                            }
                            else loggedIn = false;
                            break;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.Write("Nowy login: ");
                string newUser = Console.ReadLine();
                Console.Write("Hasło: ");
                string pass = Console.ReadLine();
                Console.Write("Rola (User/Manager): ");
                string role = Console.ReadLine();
                authService.Register(newUser, pass, role);
            }
        }
    }

    static Eventify.Models.Events.Event DemoCreateEvent()
    {
        Console.Write("Tytuł: ");
        var title = Console.ReadLine();
        Console.Write("Lokalizacja: ");
        var loc = Console.ReadLine();
        Console.Write("Data (rrrr-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var date))
        {
            Console.WriteLine("Nieprawidłowy format daty!");
            return null;
        }

        Console.Write("Typ (concert/festival/conference): ");
        var type = Console.ReadLine()?.ToLower();

        return type switch
        {
            "concert" => new Eventify.Models.Events.Concert(title, loc, date, "Demo Artist"),
            "festival" => new Eventify.Models.Events.Festival(title, loc, date, 3),
            "conference" => new Eventify.Models.Events.Conference(title, loc, date, "Demo Topic"),
            _ => throw new ArgumentException("Nieobsługiwany typ wydarzenia.")
        };
    }

}

