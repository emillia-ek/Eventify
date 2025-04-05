using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public class Festival : Event
    {
        public List<string> Attractions { get; set; }
        public int NumberOfStages { get; set; }

        public Festival(string name, string description, DateTime startDate,
                        DateTime endDate, string location, int capacity,
                        List<string> attractions, int numberOfStages)
            : base(name, description, startDate, endDate, location, capacity)
        {
            Attractions = attractions;
            NumberOfStages = numberOfStages;
        }
    }
}