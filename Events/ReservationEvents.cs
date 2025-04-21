using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Events
{
    public static class ReservationEvents
    {
        public delegate void ReservationCreatedEventHandler(string username, int eventId);
        public static event ReservationCreatedEventHandler ReservationCreated;

        

        public static void RaiseReservationCreated(string username, int eventId)
        {
            Console.WriteLine($"[LOG] Rezerwacja utworzona przez {username} dla wydarzenia ID: {eventId}");
            ReservationCreated?.Invoke(username, eventId);
        }

        public delegate void NotifyWhenEventDeleted(string username, int eventId);
        public static event NotifyWhenEventDeleted OnDeletion;

        public static void SendDeletionNotification(string username, int eventId)
        {
            
            var message = NotificationMessages.GetDeletionNotification(username, eventId);
            AnsiConsole.Write(message);
            OnDeletion?.Invoke(username, eventId);
        }
    }
}

