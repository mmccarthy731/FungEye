using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FungeyeApp.Models;

namespace FungeyeApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult UploadImage(string MushroomID)
        {
            ViewBag.MushroomID = MushroomID;
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

            string pictureURL = DAL.UploadImage(fileUpload);

            DAL.AddUserMushroom(userDescription, address, mushroomID, pictureURL);

            ViewBag.Upload = pictureURL;
            return View();
        }

        public ActionResult Leaderboards()
        {
            return View("LeaderboardsView");
        }

        public ActionResult AboutUs()
        {
            return View();
        }


        public ActionResult GetUserInfo(string Id)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.User = DAL.GetUser(Id);
            ViewBag.UserMushrooms = DAL.GetUserMushroomsByUserId(Id);
            return View("User");
        }

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
