using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey(nameof(Idea))]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }

        [ForeignKey(nameof(ReserverUser))]
        public int ReserverUserId { get; set; }
        public User ReserverUser { get; set; }

        public DateTime ReservationTime { get; set; }

        public bool CanJoin { get; set; }

        [InverseProperty(nameof(ReservationJoin.Reservation))]
        public ICollection<ReservationJoin> ReservationJoins { get; set; }
    }

    public class ReservationJoin
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
