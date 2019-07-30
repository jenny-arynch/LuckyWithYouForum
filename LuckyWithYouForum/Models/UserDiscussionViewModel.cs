using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Models
{
    public class UserDiscussionViewModel 
    {
        public string countofposts { get; set; }
        public string name { get; set; }
        public string usermail { get; set; }
        public string date { get; set; }
        public string postid { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string password { get; set; }

        public UserDiscussionViewModel() { }
        public UserDiscussionViewModel(DiscussionViewModel dis, UserViewModel user)
        {
            countofposts = user.countofposts;
            name = user.name;
            usermail = user.usermail;
            date = dis.date;
            postid = dis.postid;
            text = dis.text;
            time = dis.time;
            password = user.password;
        }
    }
}