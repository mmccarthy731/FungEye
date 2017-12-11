using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;
using FungeyeApp.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using GoogleMaps.LocationServices;


using Newtonsoft.Json;

namespace FungeyeApp.Controllers
{
   
    public class HomeController : Controller
    {
        public string APIKey = ConfigurationManager.AppSettings.Get("APIKey");
        public string APISecret = ConfigurationManager.AppSettings.Get("APISecret");
        public string ServerName = "fungeye";
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


        public ActionResult ViewUploadedImage(HttpPostedFileBase fileUpload, string name, string address, string mushroomid)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();
            ApplicationDbContext UserORM = new ApplicationDbContext();

            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            //var address = input;
            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(address);
            //double latitude = point.Latitude;
            //double longitude = point.Longitude;

            if (fileUpload != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileUpload.FileName, fileUpload.InputStream)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                JObject jsonData = (JObject)uploadResult.JsonObj;

                var locationService = new GoogleLocationService();

                var point = locationService.GetLatLongFromAddress(address);

                string email = UserORM.Users.Find(User.Identity.GetUserId()).Email;

                string commonName = ORM.Mushrooms.Find(mushroomid).CommonName;

                
                ORM.UserMushrooms.Add(new UserMushroom(jsonData["secure_url"].ToString(), address, User.Identity.GetUserId(), mushroomid, name, point.Latitude.ToString(), point.Longitude.ToString(), email, commonName));

                ORM.SaveChanges();

                ViewBag.Upload = jsonData["secure_url"];

            }
            else
            {
                ViewBag.Upload = "FAIL";
            }

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
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            ApplicationDbContext UserORM = new ApplicationDbContext();

            ApplicationUser User = UserORM.Users.Find(Id);
            List<UserMushroom> List = ORM.UserMushrooms.Where(x => x.UserID == Id).ToList();

            ViewBag.User = User;
            ViewBag.UserMushrooms = List;
            return View("User");
        }

        public ActionResult ListUsersHighestToLowest()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> emails = ORM.UserMushrooms.ToList();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).Reverse().ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            ViewBag.test = groups;


            return View("LeaderboardsView");
        }

        public ActionResult ListUsersLowestToHighest()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> emails = ORM.UserMushrooms.ToList();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            ViewBag.test = groups;


            return View("LeaderboardsView");
        }

        public ActionResult ListUsersAtoZ()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> emails = ORM.UserMushrooms.ToList();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailName).ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            ViewBag.test = groups;


            return View("LeaderboardsView");
        }

        public ActionResult ListUsersZtoA()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> emails = ORM.UserMushrooms.ToList();

            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailName).Reverse().ToList();

            ViewBag.UserList = groups.Select(x => x.EmailName).ToList();
            ViewBag.UserCount = groups.Select(x => x.EmailCount).ToList();

            ViewBag.test = groups;


            return View("LeaderboardsView");
        }

    }
}