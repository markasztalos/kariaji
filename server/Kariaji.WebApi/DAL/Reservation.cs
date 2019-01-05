using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Reservation
    {
        public int Id { get; set; }
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }

        public int ReserverUserId { get; set; }
        public User ReserverUser { get; set; }

        public DateTime ReservationTime { get; set; }
        public bool CanJoin { get; set; }

        public ICollection<ReservationJoin> Joins { get; set; }
    }

    public class ReservationJoin
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
