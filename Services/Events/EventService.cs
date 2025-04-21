using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;
using System.IO;
using System.Text.Json;
using Eventify.Utils;
using System.Security.Cryptography.X509Certificates;

namespace Eventify.Services.Events
{
    class Notifier
    {
        public delegate void NotifyWhenDeletedHandler(string message);
        public event NotifyWhenDeletedHandler OnDeletion;

        public void SendNotification(string message)
        {
            OnDeletion?.Invoke(message);
        }
    }
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

            // Jeśli newEvent.Id jest już ustawione (np. podczas edycji), sprawdź, czy nie koliduje
            if (newEvent.Id != 0 && _events.Any(e => e.Id == newEvent.Id))
            {
                throw new InvalidOperationException($"Wydarzenie o ID {newEvent.Id} już istnieje.");
            }

            if (newEvent.Id == 0)
            {
                newEvent.Id = _nextId++;
            }

            _events.Add(newEvent);
            SaveEvents();
        }

        public List<Event> GetAllEvents()
        {
            return _events?
            .GroupBy(e => e.Id).Select(g => g.First())
            .OrderBy(e => e.StartDate)
            .ToList() ?? new List<Event>();
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
                                
                                ConsoleHelper.PrintError($"Nieznany typ wydarzenia: {eventType}");
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
                    ConsoleHelper.PrintError("Błąd podczas wczytywania wydarzeń: " + ex.Message);
                }
            }
        }


        private void SaveEvents()
        {
            try
            {
                var uniqueEvents = _events.GroupBy(e => e.Id).Select(g => g.First()).ToList();
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(uniqueEvents, options);
                File.WriteAllText(EventsFilePath, json);
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Błąd zapisu wydarzeń: {ex.Message}");
            }
        }
    }
}
