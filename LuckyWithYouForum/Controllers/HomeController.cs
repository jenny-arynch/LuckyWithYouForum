using LuckyWithYouForum.Database;
using LuckyWithYouForum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LuckyWithYouForum.Controllers
{
    public class HomeController : Controller
    {
        public static string pathFireBase="https://luckywithyou-abd13.firebaseio.com";
        public static string SecretKey = "0AsQwhbFLZdHGmSHMarepHKIVZciKTJ5yaTwtCiK";



        List<PostViewModel> posts;
        List<DiscussionViewModel> discussions;
        List<UserViewModel> users;
        UserViewModel currentUser=new UserViewModel();

        public IFirebaseConfig config;
        public IFirebaseClient client;

        DateTime datenow = DateTime.Now;
        JavaScriptSerializer js = new JavaScriptSerializer();


        public HomeController()
        {


        }

        /***************HELP METHODS*********************/


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

        public void getAllUserPosts(int index)
        {
            fillPostsListFromFireBase();
            fillUsersListFromFireBase();
            users[index].newmessages = "0";
            if (users[index].countofposts == "0" || posts==null)
                return;
            List<PostViewModel> userPost = new List<PostViewModel>();
            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].usermail == users[index].usermail)
                {
                    userPost.Add(posts[i]);
                }
            }
            ViewBag.UserPosts = userPost;
            updateUsersToFireBase();
        }
        public void getPostAndDiscussions(string postid)
        {

            int postIndex = getPost(postid);
            if (postIndex < 0)
                return;
            int userIndex = getUser(posts[postIndex].usermail);
            int count= Int32.Parse(posts[postIndex].countofviews);
            posts[postIndex].countofviews = (++count).ToString();

            if (postIndex >= 0 && userIndex >= 0)
                ViewBag.PostData = new UserPostViewModel(posts[postIndex], users[userIndex]);

            getDiscussion(postid, posts[postIndex].usermail);

            updatePostsToFireBase();//update because countofviews change
        }

        public int getPost(string postid)
        {
            
            int postIndex = -1;
            if (posts.Count>0)
            {
                for (int i = 0; i < posts.Count; i++)
                {
                    if (posts[i].postid == postid)
                    {
                        return i;

                    }
                }
            }
            return postIndex;

        }

        public int getUser(string usermail)
        {
            int userIndex = -1;
            if (users.Count>0 )
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].usermail == usermail)
                    {
                        return i;

                    }
                }
            }
            return userIndex;

        }

        public void getDiscussion(string id, string usermail)
        {
                
            var model = new List<UserDiscussionViewModel>();
            if (discussions.Count>0)
            {
                for (int i = 0; i < discussions.Count; i++)
                {
                    if (discussions[i].postid == id)
                    {
                        model.Add(new UserDiscussionViewModel(discussions[i], users[getUser(discussions[i].usermail)])); /*getUser(usermail)*/
                    }
                }
            }
            ViewBag.Discussions = model;

        }

        public void getMostViewedPostsFromFirebase()
        {
            
            var model = new List<PostViewModel>();
            if (posts.Count > 0)
            {
                var sortedList = posts.OrderBy(x => x.countofviews).ToList();
                int stop = 5;
                if (posts.Count < stop)
                    stop = posts.Count;
                for (int i = 0; i < stop; i++)
                {
                    model.Add(sortedList[i]);

                }
            }
            ViewBag.mostViewed = model;

        }

        public void getLastPostsFromFirebase()
        {

            var model = new List<PostViewModel>();
            if (posts.Count>0)
            {
                int stop = 5;
                if (posts.Count < stop)
                    stop = posts.Count;
                for (int i = 0; i<stop; i++)
                {
                    posts[posts.Count - i - 1].usermail = posts[posts.Count - i - 1].usermail.Substring(0, posts[posts.Count - i - 1].usermail.IndexOf('@'));
                    model.Add(posts[posts.Count-i-1]);

                }
            }
            ViewBag.lastPosts = model;
        }

        public void createNewPost(UserPostViewModel data)
        {
            fillPostsListFromFireBase();
            fillUsersListFromFireBase();

            data.countofposts = (Int32.Parse(data.countofposts) + 1).ToString();
            data.postid = (posts.Count + 1).ToString();
            addNewPostToFirebase(new PostViewModel(data));
            users[getUser(data.usermail)].countofposts = data.countofposts;
            updateUsersToFireBase();
            fillUsersListFromFireBase();

        }

        public void sendUpdateMessage(string postid, string usermail)//add update for all discussion
        {
            fillPostsListFromFireBase();
            fillUsersListFromFireBase();

            if(users!=null && posts != null)
            {
                int postIndex = getPost(postid);
                if(posts[postIndex].usermail!=usermail)
                {
                    int userIndex = getUser(posts[postIndex].usermail);
                    users[userIndex].newmessages = "1";
                    updateUsersToFireBase();

                }

            }

        }

        /***************GET METHODS*********************/
        public void fillPostsListFromFireBase()
        {

            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("posts");
            firebaseDBTeams.NodePath("posts");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Get();
            try
            {
                //dictionary type--> generated key for insered data
                Dictionary<string, PostViewModel> entryDict = JsonConvert.DeserializeObject<Dictionary<string, PostViewModel>>(postResponse.JSONContent.ToString());
                if(entryDict!=null)
                    posts = entryDict.Select(x => x.Value).ToList();
                else
                    posts = new List<PostViewModel>();
            }
            catch (Exception e)
            {
                posts = new List<PostViewModel>();
            }
        }

        public void fillDiscussionsListFromFireBase()
        {

            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("discussions");
            firebaseDBTeams.NodePath("discussions");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Get();
            try
            {
                //dictionary type--> generated key for insered data
                Dictionary<string, DiscussionViewModel> entryDict = JsonConvert.DeserializeObject<Dictionary<string, DiscussionViewModel>>(postResponse.JSONContent.ToString());
                if (entryDict != null)
                    discussions = entryDict.Select(x => x.Value).ToList();
                else
                    discussions = new List<DiscussionViewModel>();
            }
            catch (Exception e)
            {
                discussions = new List<DiscussionViewModel>() ;
            }
        }

        public void fillUsersListFromFireBase()
        {

            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("users");
            firebaseDBTeams.NodePath("users");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Get();
            try
            {
                List<UserViewModel> entryDict = JsonConvert.DeserializeObject<List<UserViewModel>>(postResponse.JSONContent.ToString());
                if (entryDict != null)
                    users = entryDict;
                else
                    users = new List<UserViewModel>();
            }
            catch(Exception e)
            {                
                //dictionary type--> generated key for insered data
                Dictionary<string, UserViewModel> entryDict = JsonConvert.DeserializeObject<Dictionary<string, UserViewModel>>(postResponse.JSONContent.ToString());
                if (entryDict != null)
                    users = entryDict.Select(x => x.Value).ToList();
                else
                    users = new List<UserViewModel>();
            }

        }

        public bool checkUser(UserViewModel user)
        {

            fillUsersListFromFireBase();

            for(int i = 0; i < users.Count; i++)
            {
                if (users[i].password == user.password && users[i].usermail == user.usermail)
                {
                    currentUser = users[i];
                    return true;
                }
            }

            return false;

        }

        public void setCurrentUser(UserViewModel user)
        {
            currentUser = user;
            if (currentUser.usermail == null)
            {
                fillUsersListFromFireBase();
                currentUser = users[0];
            }

            ViewBag.currentUser = currentUser;
        }

        /***************POST METHODS*********************/
        private void addNewPostToFirebase(PostViewModel data)
        {
            fillPostsListFromFireBase();
            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("posts");
            firebaseDBTeams.NodePath("posts");

            string json = js.Serialize(data);

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Post(json);
            posts.Add(data);
            updatePostsToFireBase();
        }

        private void addNewDiscussionToFirebase(UserDiscussionViewModel data)
        {
            //fillDiscussionsListFromFireBase();

            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("discussions");
            firebaseDBTeams.NodePath("discussions");
            DiscussionViewModel dis = new DiscussionViewModel(data);
            

            string json = js.Serialize(dis);

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Post(json);
           //updateDiscussionsToFireBase();

        }

        private void addNewUserToFirebase()
        {

            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("users");
            firebaseDBTeams.NodePath("users");

            for (int i = 1; i <= 4; i++)
            {
                var data = new UserViewModel
                {
                    countofposts="0",
                    data_display ="1",
                    name ="Jenny",
                    newmessages = "0",
                    password ="1234",
                    score="0",
                    typeOfCancer="ALL",
                    usermail="jen@gmail.com"
                   

                };

                string json = js.Serialize(data);

                LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Post(json);
            }

        }

        public void updatePostsToFireBase()        //TODO check case of multi-users connecting to DB

        {
            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("posts");
            firebaseDBTeams.NodePath("posts");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Delete();

            for (int i = 0; i < posts.Count; i++)
            {
                string json = js.Serialize(posts[i]);

                postResponse = firebaseDBTeams.Post(json);
            }
        }

        public void updateDiscussionsToFireBase()
        {
            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("discussions");
            firebaseDBTeams.NodePath("discussions");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Delete();

            for (int i = 0; i < discussions.Count; i++)
            {
                string json = js.Serialize(discussions[i]);

                postResponse = firebaseDBTeams.Post(json);
            }
        }

        public void updateUsersToFireBase()
        {
            FirebaseDB firebaseDB = new FirebaseDB(pathFireBase);
            FirebaseDB firebaseDBTeams = firebaseDB.Node("users");
            firebaseDBTeams.NodePath("users");

            LuckyWithYouForum.Database.FirebaseResponse postResponse = firebaseDBTeams.Delete();

            for (int i = 0; i < users.Count; i++)
            {
                string json = js.Serialize(users[i]);
                postResponse = firebaseDBTeams.Post(json);
            }
        }


        /***************ACTION RESULTS****************/

        [HttpPost]
        
        public ActionResult IndexOptions(UserViewModel user)
        {
     
            if (Request.Form["MainForumPage"] != null)
            {
                
                return RedirectToAction("MainForumPage", user);

            }
            else if (Request.Form["NewPost"] != null)
            {
                return RedirectToAction("NewPost", user);


            }

            return View();

        }


        [HttpGet]
        
        public ActionResult MainForumPage(UserViewModel user)
        {

            fillPostsListFromFireBase();
            ViewBag.allPosts = posts;
            setCurrentUser(user);
            return View();
        }


        [HttpGet]
        
        public ActionResult NewPost(UserViewModel user)
        {

            setCurrentUser(user);
            return View();
        }


        public ActionResult Login()
        {
            currentUser = new UserViewModel();
            currentUser.name = "Guest";
            ViewBag.currentUser = currentUser;
            return View();
        }

        [HttpPost]
        
        public ActionResult Login(UserViewModel userTocheck)
        {
            if (userTocheck.usermail != null)
            {
                userTocheck.usermail = userTocheck.usermail.ToLower();
                if (checkUser(userTocheck))
                    return RedirectToAction("Index", "Home", new { usermail = currentUser.usermail, password=userTocheck.password });

            }
            currentUser = new UserViewModel();
            currentUser.name = "Guest";
            ViewBag.currentUser = currentUser;
            return View();
        }

        //new { usermail=ViewBag.currentUser.usermail, password=ViewBag.currentUser.password }
        
        public ActionResult Index(string usermail, string password)
        {
            if (usermail == null)
            {
                currentUser = new UserViewModel();
                currentUser.name = "Guest";
                ViewBag.currentUser = currentUser;

                return RedirectToAction("Login", "Home");
            }
            fillPostsListFromFireBase();
            getLastPostsFromFirebase();
            getMostViewedPostsFromFirebase();

            checkUser(new UserViewModel() { usermail = usermail, password = password });
            ViewBag.currentUser = currentUser;


            return View();
        }

        
        public ActionResult UserPage(string usermail, string password)
        {


            checkUser(new UserViewModel() { usermail = usermail, password = password });
            getAllUserPosts(getUser(usermail));
            ViewBag.currentUser = currentUser;

            return View();
        }


        [HttpPost]
        
        public ActionResult Index(UserViewModel user)
        {
            if (user.usermail == null) {
                currentUser = new UserViewModel();
                currentUser.name = "Guest";
                ViewBag.currentUser= currentUser;
                return RedirectToAction("Login", "Home");
            }
            fillPostsListFromFireBase();
            getLastPostsFromFirebase();
            getMostViewedPostsFromFirebase();

            checkUser(user);


            return View();
        }

        
        public ActionResult About(string usermail, string password)
        {

            return View();
        }

        //[OutputCache(NoStore = true, Duration = 0)]
        public ActionResult OpenPost(string postid, string currentUserMail, string password)
        {

            fillPostsListFromFireBase();
                fillDiscussionsListFromFireBase();
                fillUsersListFromFireBase();

                getPostAndDiscussions(postid);

                checkUser(new UserViewModel() { usermail = currentUserMail, password = password });
                ViewBag.currentUser = currentUser;

         

            return View();
        }


        [HttpPost]
        
        public ActionResult AddDiscussionAndRefreshPost(UserDiscussionViewModel model)
        {

            addNewDiscussionToFirebase(model);
            sendUpdateMessage(model.postid, model.usermail);
            checkUser(new UserViewModel() { usermail = model.usermail, password = model.password });
            ViewBag.currentUser = currentUser;
            string currentpostid = model.postid;
            ModelState.Clear();
            return RedirectToAction("OpenPost","Home", new {postid = currentpostid, currentUserMail= currentUser.usermail, password= currentUser.password  });
        }

        [HttpPost]
        
        public ActionResult AddPost(UserPostViewModel model)
        {
            createNewPost(model);
            checkUser(new UserViewModel() { usermail = model.usermail, password = model.password });
            ViewBag.currentUser = currentUser;

            return RedirectToAction("OpenPost", "Home", new { postid = model.postid, currentUserMail = currentUser.usermail, password = currentUser.password });

        }


        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }







    }




}



