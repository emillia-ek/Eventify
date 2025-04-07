using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public class Concert : Event
    {
        public string Artist { get; set; }
        public string MusicGenre { get; set; }

        public Concert(int id, string name, DateTime startDate, DateTime endDate,
                     string location, string description, int maxParticipants,
                     decimal price, string artist, string musicGenre)
            : base(id, name, startDate, endDate, location, description, maxParticipants, price)
        {
            Artist = artist;
            MusicGenre = musicGenre;
        }

        public override void DisplayEventDetails()
        {
            Console.WriteLine($"KONCERT: {Name}");
            Console.WriteLine($"Wykonawca: {Artist}");
            Console.WriteLine($"Gatunek muzyczny: {MusicGenre}");
            Console.WriteLine($"Data: {StartDate.ToShortDateString()} {StartDate.ToShortTimeString()} - {EndDate.ToShortTimeString()}");
            Console.WriteLine($"Miejsce: {Location}");
            Console.WriteLine($"Cena: {Price:C}");
            Console.WriteLine($"Liczba miejsc: {MaxParticipants}");
            Console.WriteLine($"Opis: {Description}");
        }
    }
}
