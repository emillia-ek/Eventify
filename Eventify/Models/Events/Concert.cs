using System;

namespace Eventify.Models.Events
{
    public class Concert : Event
    {
        public string Artist { get; set; }

        public Concert(string title, string location, DateTime date, string artist)
            : base(title, location, date)
        {
            Artist = artist;
        }

        public override void Display()
        {
            Console.WriteLine($"🎵 Koncert: {Title} - {Artist} @ {Location}, {Date:d}");
        }
    }
}