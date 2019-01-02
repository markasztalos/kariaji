using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Idea
    {
        public int Id { get; set; }
        
        public int CreatorUserId { get; set; }
        public User CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }

        public string HtmlText { get; set; }



    }
}
