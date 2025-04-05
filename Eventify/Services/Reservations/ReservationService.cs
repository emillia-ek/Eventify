using System.Collections.Generic;
using System;
using Eventify.Models;
using Eventify.Events;
using System.Linq;

public class ReservationService
{
    public List<Reservation> Reservations { get; } = new();

    public void Reserve(string username, Guid eventId)
    {
        var res = new Reservation
        {
            Username = username,
            EventId = eventId
        };
        Reservations.Add(res);
        EventEvents.RaiseReservationCreated(username, eventId);

        Console.WriteLine("Zarezerwowano miejsce!");
    }

    public void ShowReservations(string username)
    {
        var userReservations = Reservations.Where(r => r.Username == username);
        foreach (var r in userReservations)
        {
            Console.WriteLine($"Rezerwacja ID: {r.ReservationId}, Wydarzenie ID: {r.EventId}");
        }
    }
}
