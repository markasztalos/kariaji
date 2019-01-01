﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsDeleted { get; set; }


        public ICollection<Membership> Memberships { get; set; }
        
    }
}
