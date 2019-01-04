using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Models
{
    public class Pageable<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
