using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Services.Reservations
{
    internal class ReservationService
    {

        private readonly string _reservationFilePath = "Data/reservations.txt";




        // Delegat i zdarzenie
        public delegate void ReservationMadeHandler(string username, string eventId);
        public event ReservationMadeHandler ReservationMade;

        // Dodanie rezerwacji i wywołanie zdarzenia
        public void MakeReservation(string username, string eventId)
        {
            // Logika aplikacyjna, np. sprawdzenie, czy już zarezerwowane itd. można tu dodać

            // Wywołanie zdarzenia
            ReservationMade?.Invoke(username, eventId);
        }

        // Metoda obsługująca zapis do pliku
        public void SaveReservationToFile(string username, string eventId)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(_reservationFilePath))
                {
                    sw.WriteLine($"{username}|{eventId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy zapisie rezerwacji: {ex.Message}");
            }
        }








































        public List<string> GetReservationsByUsername(string username)
        {
            var reservations = new List<string>();

            if (!File.Exists(_reservationFilePath))
                return reservations;

            var lines = File.ReadAllLines(_reservationFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 2 && parts[0] == username)
                {
                    reservations.Add(line);
                }
            }

            return reservations;
        }

        public void AddReservation(string username, string eventId)
        {
            var dataRezerwacji = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var newReservation = $"{username}|{eventId}|{dataRezerwacji}";

            using (StreamWriter sw = File.AppendText(_reservationFilePath))
            {
                sw.WriteLine(newReservation);
            }
        }

        public bool HasReservation(string username, string eventId)
        {
            if (!File.Exists(_reservationFilePath))
                return false;

            var lines = File.ReadAllLines(_reservationFilePath);
            return lines.Any(line =>
            {
                var parts = line.Split('|');
                return parts.Length >= 2 && parts[0] == username && parts[1] == eventId;
            });
        }
        public List<string> GetAllReservations()
        {
            if (!File.Exists(_reservationFilePath))
                return new List<string>();

            return File.ReadAllLines(_reservationFilePath).ToList();
        }

        public List<string> GetReservationsForUser(string username)
        {
            var userReservations = new List<string>();

            if (!File.Exists(_reservationFilePath))
                return userReservations;

            var lines = File.ReadAllLines(_reservationFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 2 && parts[0] == username)
                {
                    userReservations.Add(parts[1]); // eventId
                }
            }

            return userReservations;
        }


    }
}
