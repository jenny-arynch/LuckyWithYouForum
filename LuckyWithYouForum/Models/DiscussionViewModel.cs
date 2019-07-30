using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Models
{
    public class DiscussionViewModel
    {
        public string date { get; set; }
        public string postid { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string usermail { get; set; }


        public DiscussionViewModel() { }

        public DiscussionViewModel(UserDiscussionViewModel data)
        {
            date = getDateNow();
            postid = data.postid;
            text = data.text;
            time = getTimeNow();
            usermail = data.usermail;
            
        }
        public string getDateNow()
        {
            DateTime datenow = DateTime.Now;
            return datenow.ToString("yyyy:MM:dd");
        }

        public string getTimeNow()
        {
            DateTime datenow = DateTime.Now;
            return datenow.ToString("HH:mm:ss");
        }
    }
}