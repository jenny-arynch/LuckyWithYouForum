using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckyWithYouForum.Database
{



    public class s
    {
        public Posts posts { get; set; }
        public Users users { get; set; }
    }

    public class Posts
    {
        public Ldbsgat9Waz69i0y_7 LDbsgaT9WAz69i0y_7 { get; set; }
    }

    public class Ldbsgat9Waz69i0y_7
    {
        public int countofviews { get; set; }
        public string date { get; set; }
        public Discussion[] discussion { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string topic { get; set; }
        public string useremail { get; set; }
    }

    public class Discussion
    {
        public string date { get; set; }
        public string text { get; set; }
        public string time { get; set; }
        public string useremail { get; set; }
    }

    public class Users
    {
        public Ldbsglqy1qrhnjfjog4 LDbsglqY1qrHnjFjoG4 { get; set; }
    }

    public class Ldbsglqy1qrhnjfjog4
    {
        public string countofposts { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string score { get; set; }
        public string typeOfCancer { get; set; }
    }


}