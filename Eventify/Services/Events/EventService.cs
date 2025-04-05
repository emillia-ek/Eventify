using System.Collections.Generic;
using System;
using System.Linq;
using Eventify.Events;
using Eventify.Models.Events;

public class EventService
{
    public List<Event> Events { get; } = new();

    public void AddEvent(Event e)
    {
        Events.Add(e);
        Console.WriteLine("Wydarzenie dodane.");
    }

    public void RemoveEvent(Guid id)
    {
        var ev = Events.FirstOrDefault(e => e.Id == id);
        if (ev != null)
        {
            Events.Remove(ev);
            EventEvents.RaiseEventRemoved(id);
            Console.WriteLine("Wydarzenie usunięte.");
        }
        else Console.WriteLine("Nie znaleziono wydarzenia.");
    }

    public void ShowAllEvents()
    {
        foreach (var e in Events)
            e.Display();
    }

    public Event? GetEventById(Guid id) =>
        Events.FirstOrDefault(e => e.Id == id);
}
