using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;

namespace Kariaji.WebApi.Models
{
    public static class UserConverterter
    {
        public static UserInfo ToInfo(this User user) => new UserInfo
        {
            DisplayName = string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email,
            Email = user.Email,
            Id = user.Id
        };
    }

    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }

    }

    public class GroupInfo
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
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
