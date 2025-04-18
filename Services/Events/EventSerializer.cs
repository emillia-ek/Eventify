using Eventify.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Services.Events
{
    public static class EventSerializer
    {
        public static string Serialize(Event @event)
        {
            if (@event is Concert)
            {
                var concert = (Concert)@event;
                return string.Format("Concert|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                    concert.Id,
                    concert.Name,
                    concert.StartDate.ToString("o"),
                    concert.EndDate.ToString("o"),
                    concert.Location,
                    concert.Description,
                    concert.MaxParticipants,
                    concert.Price,
                    concert.Artist,
                    concert.MusicGenre);
            }
            else if (@event is Conference)
            {
                var conference = (Conference)@event;
                return string.Format("Conference|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                    conference.Id,
                    conference.Name,
                    conference.StartDate.ToString("o"),
                    conference.EndDate.ToString("o"),
                    conference.Location,
                    conference.Description,
                    conference.MaxParticipants,
                    conference.Price,
                    conference.Theme,
                    string.Join(",", conference.Speakers));
            }
            return null;
        }

        public static Event Deserialize(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 10) return null;

            try
            {
                if (parts[0] == "Concert")
                {
                    return new Concert(
                        int.Parse(parts[1]),
                        parts[2],
                        DateTime.Parse(parts[3]),
                        DateTime.Parse(parts[4]),
                        parts[5],
                        parts[6],
                        int.Parse(parts[7]),
                        decimal.Parse(parts[8]),
                        parts[9],
                        parts[10]);
                }
                else if (parts[0] == "Conference")
                {
                    return new Conference(
                        int.Parse(parts[1]),
                        parts[2],
                        DateTime.Parse(parts[3]),
                        DateTime.Parse(parts[4]),
                        parts[5],
                        parts[6],
                        int.Parse(parts[7]),
                        decimal.Parse(parts[8]),
                        parts[9],
                        parts[10].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
