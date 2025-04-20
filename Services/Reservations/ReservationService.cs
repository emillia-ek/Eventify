using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Eventify.Events;
using Eventify.Interfaces;
using Eventify.Models;
using Eventify.Models.Events;
using Eventify.Services.Events;
using Eventify.Utils;

namespace Eventify.Services.Reservations
{
    public class ReservationService : IReservable
    {
        private const string ReservationsFilePath = "Data/reservations.json";
        private List<Reservation> _reservations;
        private readonly EventService _eventService;
        public event EventHandler<Reservation> ReservationCancelled;


        public ReservationService(EventService eventService)
        {
            _eventService = eventService;
            _reservations = new List<Reservation>();
            LoadReservations();
        }

        public bool Reserve(string username, int eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Nazwa użytkownika jest wymagana.");

                var ev = _eventService.GetEventById(eventId);
                if (ev == null)
                    throw new ArgumentException($"Nie znaleziono wydarzenia o ID {eventId}.");

                if (ev.StartDate <= DateTime.Now)
                    throw new InvalidOperationException("Nie można zarezerwować wydarzenia, które już się odbyło.");

                if (_reservations.Count(r => r.EventId == eventId) >= ev.MaxParticipants)
                    throw new InvalidOperationException("Brak wolnych miejsc na to wydarzenie.");

                if (_reservations.Any(r => r.Username == username && r.EventId == eventId))
                    throw new InvalidOperationException("Masz już rezerwację na to wydarzenie!");

                var reservation = new Reservation
                {
                    Id = _reservations.Count > 0 ? _reservations.Max(r => r.Id) + 1 : 1,
                    Username = username,
                    EventId = eventId,
                    ReservedAt = DateTime.Now,
                    IsCancelled = false
                };

                _reservations.Add(reservation);
                SaveReservations();

                // WYWOŁANIE EVENTU
                ReservationEvents.RaiseReservationCreated(username, eventId);

                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd rezerwacji: {ex.Message}");
                return false;
            }
        }



        public bool CancelReservation(int reservationId, string username)
        {
            try
            {
                var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId && r.Username == username);
                if (reservation == null)
                    throw new ArgumentException("Nie znaleziono rezerwacji.");

                var ev = _eventService.GetEventById(reservation.EventId);
                if (ev == null)
                    throw new ArgumentException("Wydarzenie nie istnieje.");

                if (ev.StartDate <= DateTime.Now)
                    throw new InvalidOperationException("Nie można anulować rezerwacji wydarzenia, które już się odbyło.");

                if (ev.StartDate <= DateTime.Now.AddHours(24))
                    throw new InvalidOperationException("Nie można anulować rezerwacji na mniej niż 24 godziny przed wydarzeniem.");

                _reservations.Remove(reservation); 
                SaveReservations();
                ReservationCancelled?.Invoke(this, reservation);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd anulowania rezerwacji: {ex.Message}");
                return false;
            }
        }



        public void ShowUserReservations(string username)
        {
            var userReservations = _reservations
                .Where(r => r.Username == username)
                .OrderByDescending(r => r.ReservedAt)
                .ToList();

            ConsoleHelper.PrintHeader("TWOJE REZERWACJE");

            if (!userReservations.Any())
            {
                Console.WriteLine("Brak rezerwacji.");
                return;
            }

            foreach (var res in userReservations)
            {
                
               
                var ev = _eventService.GetEventById(res.EventId);
                Console.WriteLine($"ID Rezerwacji: {res.Id}");
                Console.WriteLine($"Wydarzenie: {ev?.Name ?? "Nieznane"} (ID: {res.EventId})");
                Console.WriteLine($"Data: {ev?.StartDate.ToShortDateString() ?? "Nieznana"}");
                Console.WriteLine($"Status: {(res.IsCancelled ? "ANULOWANA" : "AKTYWNA")}");
                Console.WriteLine($"Data rezerwacji: {res.ReservedAt}");
                Console.WriteLine(new string('-', 40));
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
            ConsoleHelper.PrintHeader("WSZYSTKIE REZERWACJE");

            if (!_reservations.Any())
            {
                Console.WriteLine("Brak rezerwacji w systemie.");
                return;
            }

            foreach (var res in _reservations.OrderByDescending(r => r.ReservedAt))
            {
                var ev = _eventService.GetEventById(res.EventId);
                Console.WriteLine($"ID: {res.Id}");
                Console.WriteLine($"Wydarzenie: {ev?.Name ?? "Nieznane"} (ID: {res.EventId})");
                Console.WriteLine($"Użytkownik: {res.Username}");
                Console.WriteLine($"Status: {(res.IsCancelled ? "ANULOWANA" : "AKTYWNA")}");
                Console.WriteLine($"Data rezerwacji: {res.ReservedAt}");
                Console.WriteLine(new string('-', 40));
            }
        }

        //pobieranie rezerwacji z pliku json
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
                    Console.WriteLine($"Błąd podczas wczytywania rezerwacji: {ex.Message}");
                    _reservations = new List<Reservation>();
                }
            }
        }
        //usuwanie rezerwacji, w przypadku gdy event zostanie usuniety
        public void RemoveReservationsForEvent(int eventId)
        {
            var reservationsToRemove = _reservations.Where(r => r.EventId == eventId).ToList();
            foreach (var reservation in reservationsToRemove)
            {
                _reservations.Remove(reservation);
            }
            SaveReservations();
        }

        //zapisywanie do pliku rezerwacji
        private void SaveReservations()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_reservations, options);
                File.WriteAllText(ReservationsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu rezerwacji: {ex.Message}");
            }
        }
    }
}