using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Models
{
    public class UpdateMyAccountModel
    {
        [Required]
        public string DisplayName { get; set; }
    }

   

    public class UpdatePasswordModel
    {
        public string Password { get; set; }
    }
}
