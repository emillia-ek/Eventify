using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Events
{
    internal class NotificationMessages
    {
        public static string GetDeletionNotification(string username, int eventId)
        {
            return $"[green]Uzytkowniku {username}, wydarzenie {eventId} zostalo odwolane. Przepraszamy[/]";
        }
    }
}
