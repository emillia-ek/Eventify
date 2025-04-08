using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public int EventId { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.Now;

        public void Display()
        {
            Console.WriteLine($"[REZ] {Username} zarezerwował wydarzenie ID: {EventId} dnia {ReservedAt}");
        }
    }
}
