using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Eventify.Models;
using Eventify.Interfaces;
using Eventify.Events;

namespace Eventify.Services.Reservations
{
    public class ReservationService : IReservable
    {
        private const string ReservationsFilePath = "Data/reservations.json";
        private List<Reservation> _reservations = new List<Reservation>();

        public ReservationService()
        {
            _reservations = new List<Reservation>();
            LoadReservations();
        }

        public void Reserve(string username, int eventId)
        {
            var existing = _reservations.FirstOrDefault(r => r.Username == username && r.EventId == eventId);
            if (existing != null)
            {
                Console.WriteLine("Masz już rezerwację na to wydarzenie!");
                return;
            }

            var reservation = new Reservation
            {
                Username = username,
                EventId = eventId,
                ReservedAt = DateTime.Now 
            };

            _reservations.Add(reservation);
            ReservationEvents.RaiseReservationCreated(username, eventId);
            SaveReservations(); 
            Console.WriteLine("Rezerwacja zakończona sukcesem!");
        }

        public void ShowUserReservations(string username)
        {
            var userRes = _reservations.Where(r => r.Username == username).ToList();
            if (!userRes.Any())
            {
                Console.WriteLine("Brak rezerwacji.");
                return;
            }

            Console.WriteLine($"\nTwoje rezerwacje:");
            foreach (var res in userRes)
            {
                res.Display();
            }
        }

        //rezerwacje z pliku
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
                    Console.WriteLine("Błąd podczas wczytywania rezerwacji: " + ex.Message);
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
                Console.WriteLine($"Błąd zapisu rezerwacji: {ex.Message}");
            }
        }
    }
}
