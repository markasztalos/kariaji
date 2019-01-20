using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kariaji.WebApi.DAL
{
    public class Supervision
    {
        [Key]
        public int SupervisionId { get; set; }

        [ForeignKey(nameof(ManagerUser))]
        public int ManagerId { get; set; }
        public User ManagerUser { get; set; }
        [ForeignKey(nameof(ManagedUser))]
        public int ManagedId { get; set; }
        public User ManagedUser { get; set; }
    }
}