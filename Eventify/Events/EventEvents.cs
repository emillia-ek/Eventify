using System;

namespace Eventify.Events
{
    public static class EventEvents
    {
        public delegate void ReservationCreatedHandler(string username, Guid eventId);
        public static event ReservationCreatedHandler? OnReservationCreated;

        public delegate void EventRemovedHandler(Guid eventId);
        public static event EventRemovedHandler? OnEventRemoved;

        public static void RaiseReservationCreated(string username, Guid eventId) =>
            OnReservationCreated?.Invoke(username, eventId);

        public static void RaiseEventRemoved(Guid eventId) =>
            OnEventRemoved?.Invoke(eventId);
    }
}
