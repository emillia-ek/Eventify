using System;
using System.Collections.Generic;
using System.Linq;
using Eventify.Models.Events;
using Eventify.Utils;

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
                ConsoleHelper.PrintHeader("MENU ZARZĄDZANIA WYDARZENIAMI");
                Console.WriteLine("1. Dodaj nowe wydarzenie");
                Console.WriteLine("2. Wyświetl wszystkie wydarzenia");
                Console.WriteLine("3. Edytuj wydarzenie");
                Console.WriteLine("4. Usuń wydarzenie");
                Console.WriteLine("5. Powrót");
                Console.Write("Wybierz opcję: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        AddNewEvent();
                        break;
                    case "2":
                        DisplayAllEvents();
                        break;
                    case "3":
                        EditEvent();
                        break;
                    case "4":
                        DeleteEvent();
                        break;
                    case "5":
                        return;
                    default:
                        ConsoleHelper.PrintError("Nieprawidłowy wybór.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddNewEvent()
        {
            ConsoleHelper.PrintHeader("DODAWANIE NOWEGO WYDARZENIA");
            Console.WriteLine("1. Koncert");
            Console.WriteLine("2. Konferencja");
            Console.Write("Wybierz typ wydarzenia: ");

            var typeChoice = Console.ReadLine();
            if (typeChoice != "1" && typeChoice != "2")
            {
                ConsoleHelper.PrintError("Nieprawidłowy wybór typu wydarzenia.");
                return;
            }

            try
            {
                Console.Write("Nazwa wydarzenia: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Nazwa wydarzenia jest wymagana.");

                Console.Write("Data rozpoczęcia (dd.MM.yyyy HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                    throw new ArgumentException("Nieprawidłowy format daty.");

                Console.Write("Data zakończenia (dd.MM.yyyy HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                    throw new ArgumentException("Nieprawidłowy format daty.");

                if (endDate <= startDate)
                    throw new ArgumentException("Data zakończenia musi być późniejsza niż data rozpoczęcia.");

                Console.Write("Miejsce: ");
                string location = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(location))
                    throw new ArgumentException("Miejsce jest wymagane.");

                Console.Write("Opis: ");
                string description = Console.ReadLine();

                Console.Write("Maksymalna liczba uczestników: ");
                if (!int.TryParse(Console.ReadLine(), out int maxParticipants) || maxParticipants <= 0)
                    throw new ArgumentException("Nieprawidłowa liczba uczestników.");

                Console.Write("Cena: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                    throw new ArgumentException("Nieprawidłowa cena.");

                if (typeChoice == "1")
                {
                    Console.Write("Wykonawca: ");
                    string artist = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(artist))
                        throw new ArgumentException("Wykonawca jest wymagany.");

                    Console.Write("Gatunek muzyczny: ");
                    string musicGenre = Console.ReadLine();

                    var concert = new Concert(0, name, startDate, endDate, location,
                                           description, maxParticipants, price,
                                           artist, musicGenre);
                    _eventService.AddEvent(concert);
                }
                else
                {
                    Console.Write("Temat konferencji: ");
                    string theme = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(theme))
                        throw new ArgumentException("Temat konferencji jest wymagany.");

                    Console.WriteLine("Prelegenci (wprowadź jednego prelegenta na linię, zakończ pustą linią):");
                    var speakers = new List<string>();
                    string speaker;
                    do
                    {
                        speaker = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(speaker))
                            speakers.Add(speaker);
                    } while (!string.IsNullOrWhiteSpace(speaker));

                    if (speakers.Count == 0)
                        throw new ArgumentException("Wymagany jest co najmniej jeden prelegent.");

                    var conference = new Conference(0, name, startDate, endDate, location,
                                                 description, maxParticipants, price,
                                                 theme, speakers.ToArray());
                    _eventService.AddEvent(conference);
                }

                ConsoleHelper.PrintSuccess("Wydarzenie zostało dodane pomyślnie!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd: {ex.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        public void DisplayAllEvents(bool waitForUserInput = true)
        {
            ConsoleHelper.PrintHeader("LISTA WYDARZEŃ");
            var events = _eventService.GetAllEvents();

            if (events == null || !events.Any())
            {
                Console.WriteLine("Brak dostępnych wydarzeń.");
            }
            else
            {
                foreach (var ev in events)
                {
                    Console.WriteLine(new string('-', 50));
                    ev.DisplayEventDetails();
                    Console.WriteLine();
                }
            }

            if (waitForUserInput)
            {
                Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();
            }
        }

        private void EditEvent()
        {
            ConsoleHelper.PrintHeader("EDYCJA WYDARZENIA");
            DisplayAllEvents(false);

            Console.Write("\nPodaj ID wydarzenia do edycji: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ConsoleHelper.PrintError("Nieprawidłowy format ID.");
                Console.ReadKey();
                return;
            }

            var existingEvent = _eventService.GetEventById(id);
            if (existingEvent == null)
            {
                ConsoleHelper.PrintError("Nie znaleziono wydarzenia o podanym ID.");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine("\nAktualne dane wydarzenia:");
                existingEvent.DisplayEventDetails();

                Console.Write("\nNowa nazwa (pozostaw puste aby nie zmieniać): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                    existingEvent.Name = name;

                Console.Write("Nowa data rozpoczęcia (dd.MM.yyyy HH:mm, pozostaw puste aby nie zmieniać): ");
                string startDateInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(startDateInput))
                {
                    if (DateTime.TryParse(startDateInput, out DateTime startDate))
                        existingEvent.StartDate = startDate;
                    else
                        throw new ArgumentException("Nieprawidłowy format daty.");
                }

                // Analogicznie dla innych pól...

                _eventService.UpdateEvent(existingEvent);
                ConsoleHelper.PrintSuccess("Wydarzenie zostało zaktualizowane!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd podczas edycji: {ex.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private void DeleteEvent()
        {
            ConsoleHelper.PrintHeader("USUWANIE WYDARZENIA");
            DisplayAllEvents(false);

            Console.Write("\nPodaj ID wydarzenia do usunięcia: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ConsoleHelper.PrintError("Nieprawidłowy format ID.");
                Console.ReadKey();
                return;
            }

            if (_eventService.DeleteEvent(id))
            {
                ConsoleHelper.PrintSuccess("Wydarzenie zostało usunięte pomyślnie!");
            }
            else
            {
                ConsoleHelper.PrintError("Nie udało się usunąć wydarzenia.");
            }
            Console.ReadKey();
        }
    }
}