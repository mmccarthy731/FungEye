using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FungeyeApp.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace FungeyeApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult UploadImage(string mushroomID)
        {
            ViewBag.MushroomID = mushroomID;
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public ActionResult ViewUploadedImage(HttpPostedFileBase fileUpload, string userDescription, string address, string mushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();
            if (fileUpload == null)
            {
                ViewBag.ErrorMessage = "We were unable to upload your image. Please try again.";
                return View("UploadImage");
            }
            else if (!DAL.ValidateAddress(address))
            {
                ViewBag.ErrorMessage = "Address entered was invalid. Please try again.";
                return View("UploadImage");
            }

            string pictureURL = DAL.UploadImage(fileUpload);

            DAL.AddUserMushroom(userDescription, address, mushroomID, pictureURL, User.Identity.GetUserName(), User.Identity.GetUserId());

            ViewBag.Upload = pictureURL;
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult GetUserInfo(string id)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> userMushrooms = DAL.GetUserMushroomsByUserId(id);
            string result = "";
            for (int i = 0; i < userMushrooms.Count; i++)
            {
                result += $"{{ \"MushroomID\": \"{userMushrooms[i].MushroomID}\", \"lat\": {userMushrooms[i].Latitude}, \"lng\": {userMushrooms[i].Longitude}, \"description\": \"{userMushrooms[i].UserDescription}\", \"address\": \"{userMushrooms[i].Address}\", \"ImageLink\": \"{userMushrooms[i].PictureURL}\", \"email\": \"{userMushrooms[i].Email}\", \"CommonName\": \"{userMushrooms[i].CommonName}\"}},";
            }

            string resul = result.Substring(0, result.Length - 1);
            string json = $"[{resul}]";

            ViewBag.Json = json;
            ViewBag.Key = DAL.GoogleKey;
            ViewBag.User = DAL.GetUser(id);
            ViewBag.UserMushrooms = DAL.GetUserMushroomsByUserId(id);
            ViewBag.CurrentUser = User.Identity.GetUserId();
            return View("User");
        }

        public ActionResult Leaderboards()
        {
            return View("LeaderboardsView");
        }

        //public ContentResult SortLeaderboard(string sortOption)
        //{
        //    FungeyeDAL DAL = new FungeyeDAL();

        //    List<ApplicationUser> users = DAL.GetAllUsers();

        //    if (sortOption == "uniqueMushrooms")
        //    {
        //        users = users.OrderByDescending(x => x.UniqueMushrooms);
        //    }
        //    else if (sortOption == "totalMushrooms")
        //    {
        //        users = users.OrderByDescending(x => x.TotalMushrooms);
        //    }

        //    var list = JsonConvert.SerializeObject(users,
        //        Formatting.None,
        //        new JsonSerializerSettings()
        //        {
        //            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        //        });

        //    return Content(list, "application/json");
        //}

        public ActionResult SortUsersHighestToLowest()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> emails = DAL.GetAllUserMushrooms();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).Reverse().ToList();
            
            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            return View("LeaderboardsView");
        }

        public ActionResult SortUsersLowestToHighest()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> emails = DAL.GetAllUserMushrooms();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            return View("LeaderboardsView");
        }

        public ActionResult ListUsersAtoZ()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> emails = DAL.GetAllUserMushrooms();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailName).ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            return View("LeaderboardsView");
        }

        public ActionResult ListUsersZtoA()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> emails = DAL.GetAllUserMushrooms();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailName).Reverse().ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            return View("LeaderboardsView");
        }
    }
}
