using Eventify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eventify.Services.Reservations
{
    public class ReservationService
    {
        private List<Reservation> _reservations = new List<Reservation>();
        private readonly string _reservationsFilePath = "Data/reservations.json";
        private readonly JsonSerializerOptions _jsonOptions;

        public ReservationService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            LoadReservations();
        }

        public void MakeReservation(Reservation reservation)
        {
            _reservations.Add(reservation);
            SaveReservations();
        }

        public void CancelReservation(Guid reservationId)
        {
            _reservations.RemoveAll(r => r.Id == reservationId);
            SaveReservations();
        }

        public IEnumerable<Reservation> GetUserReservations(Guid userId)
        {
            return _reservations.Where(r => r.UserId == userId);
        }

        private void LoadReservations()
        {
            try
            {
                if (File.Exists(_reservationsFilePath))
                {
                    string json = File.ReadAllText(_reservationsFilePath);
                    _reservations = JsonSerializer.Deserialize<List<Reservation>>(json, _jsonOptions) ?? new List<Reservation>();
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogException(ex);
                _reservations = new List<Reservation>();
            }
        }
        private void SaveReservations()
        {
            try
            {
                string json = JsonSerializer.Serialize(_reservations, _jsonOptions);
                File.WriteAllText(_reservationsFilePath, json);
            }
            catch (Exception ex)
            {
                LoggerService.LogException(ex);
            }
        }
    }
}