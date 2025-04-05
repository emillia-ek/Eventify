using Eventify.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventify.Services.Events
{
    public class EventSearch
    {
        private readonly EventService _eventService;

        public EventSearch(EventService eventService)
        {
            _eventService = eventService;
        }

        public IEnumerable<Event> SearchByName(string name)
        {
            return _eventService.GetAllEvents()
                .Where(e => e.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public IEnumerable<Event> SearchByDateRange(DateTime start, DateTime end)
        {
            return _eventService.GetAllEvents()
                .Where(e => e.StartDate >= start && e.EndDate <= end);
        }

        public IEnumerable<Event> SearchByLocation(string location)
        {
            return _eventService.GetAllEvents()
                .Where(e => e.Location.IndexOf(location, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}