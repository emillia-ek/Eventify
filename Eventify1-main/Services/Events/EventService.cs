using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Models.Events;
using System.IO;
using Newtonsoft.Json; //POBIERZ NUGET DO SERIALIZACJI NA POCZATKU


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
            // posortowane na podstawie daty
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

                    // Deserializacja JSON do listy wydarzeń
                    _events = JsonConvert.DeserializeObject<List<Event>>(json) ?? new List<Event>();

                    // Jeśli lista jest pusta lub null, przypisujemy pustą listę
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wczytywania wydarzeń: {ex.Message}");
                    _events = new List<Event>(); // Jeśli wystąpi błąd, ustawiamy pustą listę
                }
            }
            else
            {
                Console.WriteLine("Plik z wydarzeniami nie istnieje.");
                _events = new List<Event>(); // Jeśli plik nie istnieje, również przypisujemy pustą listę
            }
        }


        private void SaveEvents() //zapisywanie do pliku tekstowego
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