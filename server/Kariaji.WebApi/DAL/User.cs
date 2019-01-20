using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class User
    {
        [InverseProperty(nameof(Supervision.ManagedUser))]
        public ICollection<Supervision> ManagerSupervisions { get; set; }
        [InverseProperty(nameof(Supervision.ManagerUser))]
        public ICollection<Supervision> ManagedSupervisions { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsManaged { get; set; }

        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsDeleted { get; set; }


        [InverseProperty(nameof(Membership.User))]
        public ICollection<Membership> Memberships { get; set; }

        [InverseProperty(nameof(IdeaUser.User))]
        public ICollection<IdeaUser> Ideas { get; set; }

        [InverseProperty(nameof(IdeaComment.User))]
        public ICollection<IdeaComment> Comments { get; set; }

        [InverseProperty(nameof(Reservation.ReserverUser))]
        public ICollection<Reservation> Reservations { get; set; }

    }
}
