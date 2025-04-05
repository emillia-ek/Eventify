using System;

namespace Eventify.Models.Events
{
    public class Conference : Event
    {
        public string Topic { get; set; }

        public Conference(string title, string location, DateTime date, string topic)
            : base(title, location, date)
        {
            Topic = topic;
        }

        public override void Display()
        {
            Console.WriteLine($"📢 Konferencja: {Title} - {Topic} @ {Location}, {Date:d}");
        }
    }
}