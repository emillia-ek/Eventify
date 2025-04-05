using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public abstract class Event
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }

        protected Event(string name, string description, DateTime startDate,
                       DateTime endDate, string location, int capacity)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            Capacity = capacity;
            IsActive = true;
        }
    }
}