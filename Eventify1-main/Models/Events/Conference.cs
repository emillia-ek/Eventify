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
            Console.WriteLine($"KONFERENCJA: {Name}");
            Console.WriteLine($"Temat: {Theme}");
            Console.WriteLine("Prelegenci:");
            foreach (var speaker in Speakers)
            {
                Console.WriteLine($"- {speaker}");
            }
            Console.WriteLine($"Data: {StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}");
            Console.WriteLine($"Miejsce: {Location}");
            Console.WriteLine($"Cena: {Price:C}");
            Console.WriteLine($"Liczba miejsc: {MaxParticipants}");
            Console.WriteLine($"Opis: {Description}");
        }
    }
}