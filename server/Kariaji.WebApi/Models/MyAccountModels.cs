using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Models
{
    public class UpdateAccountModel
    {
        [Required]
        public string DisplayName { get; set; }
    }

   

    public class UpdatePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
