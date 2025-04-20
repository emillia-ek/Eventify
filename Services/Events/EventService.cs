using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Eventify.Models;
using Eventify.Models.Events;

namespace Eventify.Services.Events
{
    public class EventService
    {
        private const string EventsFilePath = "Data/events.json";
        private List<Event> _events;
        private int _nextId;

        public EventService()
        {
            _events = new List<Event>();
            LoadEvents();
        }

        public void AddEvent(Event newEvent)
        {
            if (newEvent == null)
                throw new ArgumentNullException(nameof(newEvent));

            if (newEvent.Id != 0 && _events.Any(e => e.Id == newEvent.Id))
                throw new InvalidOperationException($"Wydarzenie o ID {newEvent.Id} już istnieje.");

            if (newEvent.StartDate >= newEvent.EndDate)
                throw new ArgumentException("Data rozpoczęcia musi być wcześniejsza niż data zakończenia.");

            if (newEvent.MaxParticipants <= 0)
                throw new ArgumentException("Maksymalna liczba uczestników musi być większa od zera.");

            if (newEvent.Price < 0)
                throw new ArgumentException("Cena nie może być ujemna.");

            if (newEvent.Id == 0)
                newEvent.Id = _nextId++;

            _events.Add(newEvent);
            SaveEvents();
        }

        public List<Event> GetAllEvents()
        {
            return _events?
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .OrderBy(e => e.StartDate)
                .ToList() ?? new List<Event>();
        }

        public Event GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public bool UpdateEvent(Event updatedEvent)
        {
            var existing = GetEventById(updatedEvent.Id);
            if (existing == null)
                return false;

            _events.Remove(existing);
            _events.Add(updatedEvent);
            SaveEvents();
            return true;
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
        public void LoadEvents()
        {
            if (File.Exists(EventsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(EventsFilePath);
                    List<JsonElement> rawEvents = JsonSerializer.Deserialize<List<JsonElement>>(json); // Deserialize to JsonElement list

                    _events.Clear();

                    foreach (var item in rawEvents)
                    {
                        string eventType = item.GetProperty("EventType").GetString();
                        Console.WriteLine("ok");
                        Event ev = null;
                        switch (eventType)
                        {
                            case "Concert":
                                ev = JsonSerializer.Deserialize<Concert>(item.GetRawText());
                                break;
                            case "Conference":
                                ev = JsonSerializer.Deserialize<Conference>(item.GetRawText());
                                break;
                            default:
                                Console.WriteLine($"Nieznany typ wydarzenia: {eventType}");
                                break;
                        }

                        if (ev != null)
                        {
                            _events.Add(ev);
                        }
                    }

                    // After loading events, set _nextId to the highest Id + 1
                    if (_events.Count > 0)
                    {
                        _nextId = _events.Max(e => e.Id) + 1;
                    }
                    else
                    {
                        _nextId = 1; // Start from 1 if no events are loaded
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd podczas wczytywania wydarzeń: " + ex.Message);
                    Console.ReadKey();
                }
            }
        }


        /*private void LoadEvents()
        {
            if (File.Exists(EventsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(EventsFilePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _events = JsonSerializer.Deserialize<List<Event>>(json, options) ?? new List<Event>();

                    _nextId = _events.Count > 0 ? _events.Max(e => e.Id) + 1 : 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wczytywania wydarzeń: {ex.Message}");
                    _events = new List<Event>();
                    _nextId = 1;
                }
            }
            else
            {
                _events = new List<Event>();
                _nextId = 1;
            }
        }*/


        private void SaveEvents()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null
                };
                string json = JsonSerializer.Serialize(_events, options);
                File.WriteAllText(EventsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu wydarzeń: {ex.Message}");
            }
        }
    }
}