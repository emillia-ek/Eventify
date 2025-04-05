using Eventify.Models.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eventify.Services.Events
{
    public class EventService
    {
        private List<Event> _events = new List<Event>();
        private readonly string _eventsFilePath = "Data/events.json";
        private readonly JsonSerializerOptions _jsonOptions;


        public EventService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
            LoadEvents();
        }



        public IEnumerable<Event> GetAllEvents() => new List<Event>(_events);

        public void AddEvent(Event newEvent)
        {
            _events.Add(newEvent);
            SaveEvents();
        }

        public void UpdateEvent(Event updatedEvent)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent != null)
            {
                _events.Remove(existingEvent);
                _events.Add(updatedEvent);
                SaveEvents();
            }
        }

        public void DeleteEvent(Guid eventId)
        {
            _events.RemoveAll(e => e.Id == eventId);
            SaveEvents();
        }

        public Event GetEventById(Guid id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        private void LoadEvents()
        {
            try
            {
                if (File.Exists(_eventsFilePath))
                {
                    string json = File.ReadAllText(_eventsFilePath);
                    _events = JsonSerializer.Deserialize<List<Event>>(json, _jsonOptions) ?? new List<Event>();
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogException(ex);
                _events = new List<Event>();
            }
        }

        private void SaveEvents()
        {
            try
            {
                string json = JsonSerializer.Serialize(_events, _jsonOptions);
                File.WriteAllText(_eventsFilePath, json);
            }
            catch (Exception ex)
            {
                LoggerService.LogException(ex);
            }
        }
    }
}