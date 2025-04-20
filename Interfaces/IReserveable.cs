using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Eventify.Interfaces
{
    public interface IReservable
    {
        //void Reserve(string username, int eventId);
        void ShowUserReservations(string username);
    }
}
