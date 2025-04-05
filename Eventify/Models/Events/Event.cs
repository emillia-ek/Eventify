using System;

namespace Eventify.Models.Events
{
    public abstract class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        public Event(string title, string location, DateTime date)
        {
            Title = title;
            Location = location;
            Date = date;
        }

        public abstract void Display();
    }
}

