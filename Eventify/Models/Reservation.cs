using System;

public class Reservation
{
    public Guid ReservationId { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public Guid EventId { get; set; }
    public DateTime ReservedAt { get; set; } = DateTime.Now;
}
