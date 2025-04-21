using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.NotificationSystem
{
    public class ConcertDeletedEventArgs:EventArgs
    {
        public int EventId { get; set; }
        public ConcertDeletedEventArgs(int EventId)
        {
            this.EventId = EventId;
        }
    }
}
