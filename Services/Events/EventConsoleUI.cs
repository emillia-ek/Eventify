using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eventify.Models.Events;
using Eventify.Utils;
using Spectre.Console;

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
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Eventify").Centered().Color(Color.Pink1));
                AnsiConsole.Write(new Rule("[orchid1]ZARZĄDZANIE WYDARZENIAMI[/]").Centered().RuleStyle("deeppink4"));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[Pink1]Wybierz opcję:[/]")
                        .PageSize(5)
                        .AddChoices(new[] {
                            "Dodaj nowe wydarzenie",
                            "Wyświetl wszystkie wydarzenia",
                            "Edytuj wydarzenie",
                            "Usuń wydarzenie",
                            "Powrót"
                        }));

                switch (choice)
                {
                    case "Dodaj nowe wydarzenie":
                        AddNewEvent();

                        break;
                    case "Wyświetl wszystkie wydarzenia":
                        DisplayAllEvents();
                        break;
                    case "Edytuj wydarzenie":
                        EditEvent();
                        break;
                    case "Usuń wydarzenie":
                        DeleteEvent();
                        break;
                    case "Powrót":
                        return;
                }
            }
        }
        private DateTime GetValidDate(string prompt, string color)
        {
            DateTime date;
            while (true)
            {
                string input = AnsiConsole.Ask<string>($"[{color}]{prompt}[/]");

                if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date; 
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Nieprawidłowy format daty. Proszę spróbować ponownie.[/]");
                }
            }
        }
        private void AddNewEvent()
        {
            ConsoleHelper.PrintHeader("DODAWANIE NOWEGO WYDARZENIA");
            var typeChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[orchid1]Wybierz typ wydarzenia:[/]")
                    .AddChoices("Koncert", "Konferencja"));

            try
            {
                string name = AnsiConsole.Ask<string>("[hotpink2]Nazwa wydarzenia:[/]");
                DateTime startDate = GetValidDate("Data rozpoczęcia (dd.MM.yyyy HH:mm):", "hotpink2");

                DateTime endDate = GetValidDate("Data zakończenia (dd.MM.yyyy HH:mm):", "hotpink2");

                if (endDate <= startDate)
                    throw new ArgumentException("Data zakończenia musi być późniejsza niż data rozpoczęcia.");

                string location = AnsiConsole.Ask<string>("[hotpink2]Miejsce:[/]");
                string description = AnsiConsole.Ask<string>("[hotpink2]Opis:[/]");
                int maxParticipants = AnsiConsole.Ask<int>("[hotpink2]Maksymalna liczba uczestników:[/]");
                decimal price = AnsiConsole.Ask<decimal>("[hotpink2]Cena:[/]");

                if (typeChoice == "Koncert")
                {
                    string artist = AnsiConsole.Ask<string>("[hotpink2]Wykonawca:[/]");
                    string musicGenre = AnsiConsole.Ask<string>("[hotpink2]Gatunek muzyczny:[/]");

                    var concert = new Concert(0, name, startDate, endDate, location, description, maxParticipants, price, artist, musicGenre);
                    _eventService.AddEvent(concert);
                }
                else
                {
                    string theme = AnsiConsole.Ask<string>("[hotpink2]Temat konferencji:[/]");

                    AnsiConsole.MarkupLine("[pink1]Prelegenci (wprowadź jednego na linię, zakończ pustą linią):[/]");
                    var speakers = new List<string>();
                    while (true)
                    {
                        string speaker = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(speaker)) break;
                        speakers.Add(speaker);
                    }

                    if (speakers.Count == 0)
                        throw new ArgumentException("Wymagany jest co najmniej jeden prelegent.");

                    var conference = new Conference(0, name, startDate, endDate, location, description, maxParticipants, price, theme, speakers.ToArray());
                    _eventService.AddEvent(conference);
                }

                AnsiConsole.MarkupLine("[bold Pink1]Wydarzenie zostało dodane pomyślnie![/]");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd: {ex.Message}");
            }

            AnsiConsole.MarkupLine("[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
            Console.ReadKey();
        }

        public void DisplayAllEvents(bool waitForUserInput = true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Eventify").Centered().Color(Color.Pink1));
            AnsiConsole.Write(new Rule("[orchid1]LISTA WYDARZEŃ[/]").Centered().RuleStyle("deeppink4"));

            var events = _eventService.GetAllEvents();

            if (events == null || !events.Any())
            {
                AnsiConsole.MarkupLine("[grey]Brak dostępnych wydarzeń.[/]");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Square)
                    .BorderColor(Color.MediumVioletRed)
                    .AddColumn("[deeppink1]ID[/]")
                    .AddColumn("[hotpink2]Nazwa[/]")
                    .AddColumn("[orchid1]Typ[/]")
                    .AddColumn("[plum1]Data[/]")
                    .AddColumn("[pink1]Miejsce[/]")
                    .AddColumn("[violet]Cena[/]");

                foreach (var ev in events)
                {
                    table.AddRow(
                        ev.Id.ToString(),
                        ev.Name,
                        ev is Concert ? "[orchid1]Koncert[/]" : "[plum1]Konferencja[/]",
                        ev.StartDate.ToString("dd.MM.yyyy"),
                        ev.Location,
                        $"{ev.Price} zł"
                    );
                }

                AnsiConsole.Write(table);
            }

            if (waitForUserInput)
            {
                AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
                Console.ReadKey();
            }
        }

        private void EditEvent()
        {
            ConsoleHelper.PrintHeader("EDYCJA WYDARZENIA");
            DisplayAllEvents(false);

            int id = AnsiConsole.Ask<int>("[orchid1]\nPodaj ID wydarzenia do edycji:[/]");
            var existingEvent = _eventService.GetEventById(id);

            if (existingEvent == null)
            {
                ConsoleHelper.PrintError("Nie znaleziono wydarzenia o podanym ID.");
                Console.ReadKey();
                return;
            }

            try
            {
                ConsoleHelper.PrintHeader("AKTUALNE DANE WYDARZENIA");
                existingEvent.DisplayEventDetails();

                Console.Write("Nowa nazwa (pozostaw puste aby nie zmieniać):");
                string name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    existingEvent.Name = name;
                }

                Console.Write("Nowa data rozpoczęcia (dd.MM.yyyy HH:mm, puste = brak zmiany):");
                string startInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(startInput))
                {
                    if (DateTime.TryParseExact(startInput, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newStart))
                    {
                        existingEvent.StartDate = newStart;
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy format daty. Proszę spróbować ponownie.");
                    }
                }


                _eventService.UpdateEvent(existingEvent);
                AnsiConsole.MarkupLine("[bold Pink1] Wydarzenie zostało zaktualizowane![/]");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd: {ex.Message}");
            }

            Console.ReadKey();
        }

        private void DeleteEvent()
        {
            ConsoleHelper.PrintHeader("USUWANIE WYDARZENIA");
            DisplayAllEvents(false);

            int id = AnsiConsole.Ask<int>("[orchid1]\nPodaj ID wydarzenia do usunięcia:[/]");

            if (_eventService.DeleteEvent(id))
            {
                AnsiConsole.MarkupLine("[bold Pink1] Wydarzenie zostało usunięte![/]");
            }
            else
            {
                ConsoleHelper.PrintError("Nie udało się usunąć wydarzenia.");
            }

            Console.ReadKey();
        }
    }
}