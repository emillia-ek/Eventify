using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;
using Eventify.Utils;
using Spectre.Console;
using Eventify.NotificationSystem;

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
                AnsiConsole.Write(new Rule("[red]ZARZĄDZANIE WYDARZENIAMI[/]").RuleStyle("grey").Centered());

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Wybierz opcję:[/]")
                        .PageSize(5)
                        .AddChoices(new[] {
                            "Dodaj nowe wydarzenie",
                            "Wyświetl wszystkie wydarzenia",
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
                    case "Usuń wydarzenie":
                        DeleteEvent();
                        break;
                    case "Powrót":
                        return;
                }
            }
        }

        private void AddNewEvent()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]DODAWANIE NOWEGO WYDARZENIA[/]").RuleStyle("grey").Centered());

            var eventType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Wybierz typ wydarzenia:[/]")
                    .AddChoices("Koncert", "Konferencja"));

            var name = AnsiConsole.Ask<string>("[yellow]Nazwa wydarzenia:[/] ");

            var startDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("[yellow]Data rozpoczęcia (dd.MM.yyyy HH:mm):[/] ")
                    .ValidationErrorMessage("[red]Nieprawidłowy format daty[/]")
                    .DefaultValue(DateTime.Now.AddDays(1))
                    .PromptStyle("green"));

            var endDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("[yellow]Data zakończenia (dd.MM.yyyy HH:mm):[/] ")
                    .ValidationErrorMessage("[red]Nieprawidłowy format daty[/]")
                    .DefaultValue(startDate.AddHours(2))
                    .Validate(date =>
                    {
                        return date <= startDate
                            ? ValidationResult.Error("[red]Data zakończenia musi być późniejsza niż data rozpoczęcia[/]")
                            : ValidationResult.Success();
                    })
                    .PromptStyle("green"));

            var location = AnsiConsole.Ask<string>("[yellow]Miejsce:[/] ");
            var description = AnsiConsole.Ask<string>("[yellow]Opis:[/] ");

            var maxParticipants = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Maksymalna liczba uczestników:[/] ")
                    .ValidationErrorMessage("[red]Wprowadź poprawną liczbę[/]")
                    .DefaultValue(100)
                    .PromptStyle("green"));

            var price = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[yellow]Cena:[/] ")
                    .ValidationErrorMessage("[red]Wprowadź poprawną cenę[/]")
                    .DefaultValue(0)
                    .PromptStyle("green"));

            if (eventType == "Koncert")
            {
                var artist = AnsiConsole.Ask<string>("[yellow]Wykonawca:[/] ");
                var musicGenre = AnsiConsole.Ask<string>("[yellow]Gatunek muzyczny:[/] ");

                var concert = new Concert(0, name, startDate, endDate, location,
                                       description, maxParticipants, price,
                                       artist, musicGenre);
                _eventService.AddEvent(concert);
            }
            else // Konferencja
            {
                var theme = AnsiConsole.Ask<string>("[yellow]Temat konferencji:[/] ");

                AnsiConsole.MarkupLine("[yellow]Prelegenci (wprowadź jednego prelegenta na linię, zakończ pustą linią):[/]");
                var speakers = new List<string>();

                while (true)
                {
                    // Używamy TextPrompt z możliwością pustego wprowadzenia
                    var speaker = AnsiConsole.Prompt(
                        new TextPrompt<string>("[grey]Prelegent (Enter aby zakończyć):[/]")
                            .AllowEmpty());

                    if (string.IsNullOrWhiteSpace(speaker))
                    {
                        break;
                    }

                    speakers.Add(speaker);
                    AnsiConsole.MarkupLine($"[grey]Dodano prelegenta: {speaker}[/]");
                }

                AnsiConsole.MarkupLine("[green]Zakończono dodawanie prelegentów.[/]");

                var conference = new Conference(0, name, startDate, endDate, location,
                                             description, maxParticipants, price,
                                             theme, speakers.ToArray());
                _eventService.AddEvent(conference);
            }

            
            ConsoleHelper.PrintSuccess("Wydarzenie zostało dodane pomyślnie!");
            Console.ReadKey();
        }

        public void DisplayAllEvents(bool waitForUserInput = true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]LISTA WYDARZEŃ[/]").RuleStyle("grey").Centered());

            var events = _eventService.GetAllEvents();
            if (events == null || events.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Brak dostępnych wydarzeń.[/]");
            }
            else
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Grey)
                    .Title("[yellow]Dostępne wydarzenia[/]")
                    .AddColumn(new TableColumn("[white]ID[/]").Centered())
                    .AddColumn(new TableColumn("[white]Nazwa[/]"))
                    .AddColumn(new TableColumn("[white]Data[/]").Centered())
                    .AddColumn(new TableColumn("[white]Miejsce[/]"))
                    .AddColumn(new TableColumn("[white]Typ[/]").Centered());

                foreach (var ev in events)
                {
                    var type = ev is Concert ? "\u266b Koncert" : "\u058e Konferencja";//zamienic na unicode ->  u266b -nutka; u266f
                    table.AddRow(
                        ev.Id.ToString(),
                        ev.Name,
                        ev.StartDate.ToString("g", CultureInfo.CurrentCulture),
                        ev.Location,
                        type);
                }

                AnsiConsole.Render(table); //to dziala i bym nie zmieniala tbh
            }

            AnsiConsole.MarkupLine("\n[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
            Console.ReadKey();
        }

        public event EventHandler<ConcertDeletedEventArgs> ConcertDeleted;

        //private List<string> concerts = new List<string>();

        

        private void DeleteEvent()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]USUWANIE WYDARZENIA[/]").RuleStyle("grey").Centered());

            var events = _eventService.GetAllEvents();
            if (events.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Brak wydarzeń do usunięcia.[/]");
                Console.ReadKey();
                return;
            }

            var eventChoices = events.Select(e =>
                new { Id = e.Id, Display = $"{e.Name} ({e.StartDate:dd.MM.yyyy HH:mm})" })
                .ToList();

            var selectedEvent = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Wybierz wydarzenie do usunięcia:[/]")
                    .PageSize(10)
                    .AddChoices(eventChoices.Select(ec => ec.Display)));

            var eventId = eventChoices.First(ec => ec.Display == selectedEvent).Id;

            if (AnsiConsole.Confirm($"Czy na pewno chcesz usunąć wydarzenie [red]'{selectedEvent}'[/]?"))
            {
                if (_eventService.DeleteEvent(eventId))
                {
                    ConcertDeleted?.Invoke(this, new ConcertDeletedEventArgs(eventId));
                    ConsoleHelper.PrintSuccess("Wydarzenie zostało usunięte pomyślnie!");
                   // EventDeletionEvents.RaiseEventDeleted(eventId, selectedEvent.Name, affectedUsers);
                }
                else
                {
                    ConsoleHelper.PrintError("Nie udało się usunąć wydarzenia.");
                }
            }
            else
            {
                
                ConsoleHelper.PrintInfo("Anulowano usuwanie wydarzenia.");
            }



             Console.ReadKey();
        }
    }

    //zaczete zdarzenie - na razie nic nie robi
    public static class EventDeletionEvents
    {
        // Delegat dla zdarzenia usuwania wydarzenia
        public delegate void EventDeletedHandler(DeletedEventInfo deletedEventInfo);

        // Zdarzenie wywoływane przy usuwaniu wydarzenia
        public static event EventDeletedHandler OnEventDeleted;

        // Metoda do wywoływania zdarzenia
        public static void RaiseEventDeleted(int eventId, string eventName, List<string> affectedUsers)
        {
            OnEventDeleted?.Invoke(new DeletedEventInfo(eventId, eventName, affectedUsers));
        }
    }

    // Klasa przechowująca informacje o usuniętym wydarzeniu
    public class DeletedEventInfo
    {
        public int EventId { get; }
        public string EventName { get; }
        public List<string> AffectedUsers { get; }
        public DateTime DeletionTime { get; }

        public DeletedEventInfo(int eventId, string eventName, List<string> affectedUsers)
        {
            EventId = eventId;
            EventName = eventName;
            AffectedUsers = affectedUsers;
            DeletionTime = DateTime.Now;
        }
    }
}
