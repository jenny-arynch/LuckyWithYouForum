using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Models
{
    public class UserViewModel
    {
        public string countofposts { get; set; }
        public string data_display { get; set; }
        public string name { get; set; }
        public string newmessages { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        public string score { get; set; }
        public string typeOfCancer { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string usermail { get; set; }


        public UserViewModel()
        {

        }

    }
}