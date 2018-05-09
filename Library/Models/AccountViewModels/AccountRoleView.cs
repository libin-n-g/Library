using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Library.Models.AccountViewModels
{
    public class AccountRoleView
    {
        public ApplicationUser User { get; set; } 

        [Display(Name ="Roles")]
        public String Roles { get; set; }
    }
}   
