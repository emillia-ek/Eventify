using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models
{
    public class Reservation
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime ReservationDate { get; private set; }
        public int NumberOfTickets { get; set; }
        public bool IsConfirmed { get; set; }

        public Reservation(Guid userId, Guid eventId, int numberOfTickets)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            EventId = eventId;
            ReservationDate = DateTime.UtcNow;
            NumberOfTickets = numberOfTickets;
            IsConfirmed = false;
        }
    }
}