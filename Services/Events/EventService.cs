using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;
using System.IO;

namespace Eventify.Services.Events
{
    public class EventService
    {
        private const string EventsFilePath = "Data/events.json";
        private List<Event> _events;
        private int _nextId = 1;

        public EventService()
        {
            _events = new List<Event>();
            LoadEvents();
        }

        public void AddEvent(Event newEvent)
        {
            newEvent.Id = _nextId++;
            _events.Add(newEvent);
            SaveEvents();
        }

        public List<Event> GetAllEvents()
        {
            return _events.OrderBy(e => e.StartDate).ToList();
        }

        public Event GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public bool DeleteEvent(int id)
        {
            var eventToRemove = _events.FirstOrDefault(e => e.Id == id);
            if (eventToRemove != null)
            {
                _events.Remove(eventToRemove);
                SaveEvents();
                return true;
            }
            return false;
        }

        private void LoadEvents()
        {
            if (File.Exists(EventsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(EventsFilePath);
                    // Tutaj należy dodać deserializację JSON do listy wydarzeń
                    // W uproszczonej wersji możemy zostawić pustą implementację
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wczytywania wydarzeń: {ex.Message}");
                }
            }
        }

        private void SaveEvents()
        {
            try
            {
                // Tutaj należy dodać serializację listy wydarzeń do JSON
                // W uproszczonej wersji możemy zostawić pustą implementację
                File.WriteAllText(EventsFilePath, "[]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania wydarzeń: {ex.Message}");
            }
        }
    }
}