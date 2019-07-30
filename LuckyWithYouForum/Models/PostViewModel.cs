using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Models
{

    public class PostViewModel
    {
        public string countofviews { get; set; }
        public string date { get; set; }
        public string postid { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string topic { get; set; }
        public string usermail { get; set; }
        public PostViewModel()
        {

        }

        public PostViewModel(UserPostViewModel data) {
            countofviews = "0";
            date = getDateNow();
            postid = data.postid;
            subject = data.subject;
            text = data.text;
            time = getTimeNow();
            topic = data.topic;
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