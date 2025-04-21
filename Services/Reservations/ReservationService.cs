using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Eventify.Models;
using Eventify.Interfaces;
using Eventify.Events;
using Eventify.Models.Events;
using Eventify.Services.Events;
using Eventify.Utils;
using Spectre.Console;

namespace Eventify.Services.Reservations
{
    public class ReservationService : IReservable
    {
        private const string ReservationsFilePath = "Data/reservations.json";
        private const string EventsFilePath = "Data/events.json"; // Ścieżka pliku z wydarzeniami
        private List<Reservation> _reservations = new List<Reservation>();
        private List<Event> _events = new List<Event>();
        private EventService _eventService;

        public ReservationService(EventService eventService) // Wstrzykujemy zależność EventService
        {
            _reservations = new List<Reservation>();
            _eventService = eventService; // Inicjalizacja EventService
            LoadReservations();
        }

        public void Reserve(string username, int eventId)
        {

            var ev = _eventService.GetEventById(eventId); // Pobieramy wydarzenie z EventService
            if (ev == null)
            {
                
                ConsoleHelper.PrintError($"Nie znaleziono wydarzenia o ID {eventId}. Rezerwacja niemożliwa.");
                return;
            }

            // Sprawdzenie, czy użytkownik już ma rezerwację na to wydarzenie
            var existing = _reservations.FirstOrDefault(r => r.Username == username && r.EventId == eventId);
            if (existing != null)
            {
                
                ConsoleHelper.PrintInfo("Masz już rezerwację na to wydarzenie!");
                return;
            }

            // Tworzenie rezerwacji
            var reservation = new Reservation
            {
                Username = username,
                EventId = eventId,
                ReservedAt = DateTime.Now
            };

            _reservations.Add(reservation);
            ReservationEvents.RaiseReservationCreated(username, eventId);
            SaveReservations();
            
            ConsoleHelper.PrintSuccess("Rezerwacja zakończona sukcesem!");
        }

        public void ShowUserReservations(string username)
        {
            var userRes = _reservations.Where(r => r.Username == username).ToList();
            if (!userRes.Any())
            {
                
                ConsoleHelper.PrintInfo("Nie masz jeszcze rezerwacji.");
                return;
            }

            Console.Clear();
            AnsiConsole.Write(new Rule("[red]TWOJE REZERWACJE[/]").RuleStyle("grey").Centered());
            foreach (var res in userRes)
            {
                res.Display();
            }
        }
        public List<Reservation> GetAllReservations()
        {
            return new List<Reservation>(_reservations);
        }
        public List<Reservation> GetReservationsForEvent(int eventId)
        {
            return _reservations.Where(r => r.EventId == eventId).ToList();
        }
        public void ShowAllReservations()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[red]WSZYTSKIE REZERWACJE[/]").RuleStyle("grey").Centered());

            if (_reservations.Count == 0)
            {
                
                AnsiConsole.MarkupLine("[grey]Brak rezerwacji w systemie.[/]");
                return;
            }

            

            foreach (var reservation in _reservations)
            {
                var eventItem = _eventService.GetEventById(reservation.EventId);

                var panel = new Panel(
                    new Rows(
                        new Text($"ID Rezerwacji: {reservation.Id}"),
                        new Text($"ID wydarzenia: {reservation.EventId}"),
                        new Text($"Użytkownik: {reservation.Username}"),
                        new Text($"Data rezerwacji: {reservation.ReservedAt}")
                    //AnsiConsole.MarkupLine("[yellow]Anulowano usuwanie wydarzenia.[/]");
                    ))
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[blue]{reservation.Username}[/]"),
                    Padding = new Padding(1, 1, 1, 1)
                };

                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine();
            }
        }

        // Ładowanie rezerwacji z pliku
        private void LoadReservations()
        {
            if (File.Exists(ReservationsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ReservationsFilePath);
                    _reservations = JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Błąd podczas wczytywania rezerwacji: {ex.Message}");
                    
                }
            }
        }

        private void SaveReservations()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_reservations, options);
                File.WriteAllText(ReservationsFilePath, json);
            }
            catch (Exception ex)
            {
                
                ConsoleHelper.PrintError($"Błąd zapisu rezerwacji: {ex.Message}");
            }
        }
    }

}

