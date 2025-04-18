using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Events
{
    public class Conference : Event
    {
        public string Theme { get; set; }
        public string[] Speakers { get; set; }
        public override string EventType => "Conference";

        // Bezparametrowy konstruktor potrzebny do deserializacji
        public Conference()
    : base(0, "", DateTime.MinValue, DateTime.MinValue, "", "", 0, 0)
        {
            Theme = string.Empty;
            Speakers = Array.Empty<string>();
        }


        public Conference(int id, string name, DateTime startDate, DateTime endDate,
                         string location, string description, int maxParticipants,
                         decimal price, string theme, string[] speakers)
            : base(id, name, startDate, endDate, location, description, maxParticipants, price)
        {
            Theme = theme;
            Speakers = speakers;
        }

        public override void DisplayEventDetails()
        {
            Console.WriteLine($"Id: {Id}");
            Console.WriteLine($"KONFERENCJA: {Name}");
            Console.WriteLine($"Temat: {Theme ?? "brak danych"}");

            Console.WriteLine("Prelegenci:");
            if (Speakers != null && Speakers.Any())
            {
                foreach (var speaker in Speakers)
                {
                    Console.WriteLine($"- {speaker}");
                }
            }
            else
            {
                Console.WriteLine("Brak prelegentów.");
            }

            Console.WriteLine($"Data: {StartDate:dd.MM.yyyy HH:mm} - {EndDate:HH:mm}");
            Console.WriteLine($"Miejsce: {Location}");
            Console.WriteLine($"Cena: {Price:C}");
            Console.WriteLine($"Liczba miejsc: {MaxParticipants}");
            Console.WriteLine($"Opis: {Description}");
        }
    }

}