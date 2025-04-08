using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;

namespace Eventify.Services.Events
{
    public class EventConsoleUI
    {
        private readonly EventService _eventService;

        public EventConsoleUI(EventService eventService)
        {
            _eventService = eventService;
        }

        public void ShowEventManagementMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MENU ZARZĄDZANIA WYDARZENIAMI ===");
                Console.WriteLine("1. Dodaj nowe wydarzenie");
                Console.WriteLine("2. Wyświetl wszystkie wydarzenia");
                Console.WriteLine("3. Usuń wydarzenie");
                Console.WriteLine("4. Powrót");
                Console.Write("Wybierz opcję: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddNewEvent();
                        break;
                    case "2":
                        DisplayAllEvents();
                        break;
                    case "3":
                        DeleteEvent();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór. Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddNewEvent()
        {
            Console.Clear();
            Console.WriteLine("=== DODAWANIE NOWEGO WYDARZENIA ===");
            Console.WriteLine("1. Koncert");
            Console.WriteLine("2. Konferencja");
            Console.Write("Wybierz typ wydarzenia: ");

            var typeChoice = Console.ReadLine();
            if (typeChoice != "1" && typeChoice != "2")
            {
                Console.WriteLine("Nieprawidłowy wybór typu wydarzenia.");
                return;
            }

            Console.Write("Nazwa wydarzenia: ");
            string name = Console.ReadLine();

            Console.Write("Data rozpoczęcia (dd.MM.yyyy HH:mm): ");
            DateTime startDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Data zakończenia (dd.MM.yyyy HH:mm): ");
            DateTime endDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Miejsce: ");
            string location = Console.ReadLine();

            Console.Write("Opis: ");
            string description = Console.ReadLine();

            Console.Write("Maksymalna liczba uczestników: ");
            int maxParticipants = int.Parse(Console.ReadLine());

            Console.Write("Cena: ");
            decimal price = decimal.Parse(Console.ReadLine());

            if (typeChoice == "1") // Koncert
            {
                Console.Write("Wykonawca: ");
                string artist = Console.ReadLine();

                Console.Write("Gatunek muzyczny: ");
                string musicGenre = Console.ReadLine();

                var concert = new Concert(0, name, startDate, endDate, location,
                                       description, maxParticipants, price,
                                       artist, musicGenre);
                _eventService.AddEvent(concert);
            }
            else // Konferencja
            {
                Console.Write("Temat konferencji: ");
                string theme = Console.ReadLine();

                Console.WriteLine("Prelegenci (wprowadź jednego prelegenta na linię, zakończ pustą linią):");
                var speakers = new List<string>();
                string speaker;
                do
                {
                    speaker = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(speaker))
                    {
                        speakers.Add(speaker);
                    }
                } while (!string.IsNullOrWhiteSpace(speaker));

                var conference = new Conference(0, name, startDate, endDate, location,
                                             description, maxParticipants, price,
                                             theme, speakers.ToArray());
                _eventService.AddEvent(conference);
            }

            Console.WriteLine("Wydarzenie zostało dodane pomyślnie!");
            Console.ReadKey();
        }

        public void DisplayAllEvents()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA WYDARZEŃ ===\n");

            List<Event> events = _eventService.GetAllEvents();
            if (events == null || events.Count == 0)
            {
                Console.WriteLine("Brak dostępnych wydarzeń.");
            }
            else
            {
                foreach (Event ev in events)
                {
                    Console.WriteLine(new string('-', 50));
                    ev.DisplayEventDetails();
                }
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }


        private void DeleteEvent()
        {
            Console.Clear();
            Console.WriteLine("=== USUWANIE WYDARZENIA ===");

            var events = _eventService.GetAllEvents();
            if (events.Count == 0)
            {
                Console.WriteLine("Brak wydarzeń do usunięcia.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Dostępne wydarzenia:");
            foreach (var ev in events)
            {
                Console.WriteLine($"{ev.Id}. {ev.Name} ({ev.StartDate.ToShortDateString()})");
            }

            Console.Write("\nPodaj ID wydarzenia do usunięcia: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (_eventService.DeleteEvent(id))
                {
                    Console.WriteLine("Wydarzenie zostało usunięte pomyślnie!");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono wydarzenia o podanym ID.");
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy format ID.");
            }

            Console.ReadKey();
        }
    }
}
