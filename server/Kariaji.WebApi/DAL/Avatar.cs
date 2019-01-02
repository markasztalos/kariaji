using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kariaji.WebApi.DAL
{
    public class Avatar
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
    }
}
