using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;
using System.IO;
using System.Text.Json;

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
            var toRemove = _events.FirstOrDefault(e => e.Id == id);
            if (toRemove != null)
            {
                _events.Remove(toRemove);
                SaveEvents();
                return true;
            }
            return false;
        }

        private void LoadEvents()
        {
            if (!File.Exists(EventsFilePath)) return;

            try
            {
                string json = File.ReadAllText(EventsFilePath);
                List<JsonElement> rawEvents = JsonSerializer.Deserialize<List<JsonElement>>(json);

                foreach (JsonElement item in rawEvents)
                {
                    string type = item.GetProperty("EventType").GetString();
                    Event ev = null;

                    switch (type)
                    {
                        case "Concert":
                            ev = JsonSerializer.Deserialize<Concert>(item.GetRawText());
                            break;
                        case "Conference":
                            ev = JsonSerializer.Deserialize<Conference>(item.GetRawText());
                            break;
                        default:
                            Console.WriteLine("Nieznany typ wydarzenia: " + type);
                            break;
                    }

                    if (ev != null)
                    {
                        _events.Add(ev);
                    }
                }

                if (_events.Count > 0)
                {
                    _nextId = _events.Max(e => e.Id) + 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas wczytywania wydarzeń: " + ex.Message);
            }
        }


        private void SaveEvents()
        {
            try
            {
                // Odczytanie istniejących danych, jeśli plik już istnieje
                List<Event> allEvents = new List<Event>();
                if (File.Exists(EventsFilePath))
                {
                    string existingJson = File.ReadAllText(EventsFilePath);
                    allEvents = JsonSerializer.Deserialize<List<Event>>(existingJson) ?? new List<Event>();
                }
                allEvents.AddRange(_events);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(allEvents, options);
                File.WriteAllText(EventsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu wydarzeń: {ex.Message}");
            }
        }

    }
}
