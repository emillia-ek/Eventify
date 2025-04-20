using System;

namespace Eventify.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int EventId { get; set; }
        public DateTime ReservedAt { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledAt { get; set; }

        public void Display()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"ID Wydarzenia: {EventId}");
            Console.WriteLine($"Użytkownik: {Username}");
            Console.WriteLine($"Data rezerwacji: {ReservedAt}");
            Console.WriteLine($"Status: {(IsCancelled ? "ANULOWANA" : "AKTYWNA")}");
            if (IsCancelled)
                Console.WriteLine($"Data anulowania: {CancelledAt}");
            Console.WriteLine(new string('-', 20));
        }
    }
}