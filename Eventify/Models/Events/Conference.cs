using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public class Conference : Event
    {
        public List<string> Speakers { get; set; }
        public string Topic { get; set; }

        public Conference(string name, string description, DateTime startDate,
                         DateTime endDate, string location, int capacity,
                         string topic, List<string> speakers)
            : base(name, description, startDate, endDate, location, capacity)
        {
            Topic = topic;
            Speakers = speakers;
        }
    }
}