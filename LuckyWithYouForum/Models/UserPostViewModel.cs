using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Models
{
    public class UserPostViewModel
    {
        public string countofposts { get; set; }
        public string name { get; set; }
        public string usermail { get; set; }
        public string countofviews { get; set; }
        public string date { get; set; }
        public string postid { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string topic { get; set; }
        public string password { get; set; }
        public UserPostViewModel(PostViewModel post, UserViewModel user)
        {
            countofposts = user.countofposts;
            name = user.name;
            usermail = user.usermail;
            date = post.date;
            postid = post.postid;
            text = post.text;
            time = post.time;
            topic = post.topic;
            subject = post.subject;
            countofviews = post.countofviews;
        }
        public UserPostViewModel()
        {

        }

    }
}