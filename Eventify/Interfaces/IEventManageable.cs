using System;
using Eventify.Models.Events;


public interface IEventManageable
{
    void AddEvent(Event e);
    void RemoveEvent(Guid eventId);
}
