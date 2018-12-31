using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Models
{
    public class GroupInfo
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public DateTime CreationDate {get; set; }
        public List<MembershipInfo> Memberships { get; set; }
    }

    public class MembershipInfo
    {
        public int GroupId { get; set; }
        public string GroupDisplayName { get; set; }
        public int UserId { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
