using System;

public interface IReservable
{
    void ReserveEvent(string username, Guid eventId);
}