using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public abstract class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public abstract string EventType { get; }

        protected Event(int id, string name, DateTime startDate, DateTime endDate,
                      string location, string description, int maxParticipants, decimal price)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            Description = description;
            MaxParticipants = maxParticipants;
            Price = price;
            
        }

        public abstract void DisplayEventDetails();
    }
}
