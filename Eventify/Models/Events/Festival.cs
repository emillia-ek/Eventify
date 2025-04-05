using System;
namespace Eventify.Models.Events
{
    public class Festival : Event
    {
        public int DurationDays { get; set; }

        public Festival(string title, string location, DateTime date, int duration)
            : base(title, location, date)
        {
            DurationDays = duration;
        }

        public override void Display()
        {
            Console.WriteLine($"🎉 Festiwal: {Title}, {DurationDays} dni @ {Location}, {Date:d}");
        }
    }
}