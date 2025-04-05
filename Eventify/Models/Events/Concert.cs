using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public class Concert : Event
    {
        public string Artist { get; set; }
        public string Genre { get; set; }

        public Concert(string name, string description, DateTime startDate,
                      DateTime endDate, string location, int capacity,
                      string artist, string genre)
            : base(name, description, startDate, endDate, location, capacity)
        {
            Artist = artist;
            Genre = genre;
        }
    }
}
