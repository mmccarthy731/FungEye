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


        public ActionResult ViewUploadedImage(HttpPostedFileBase fileUpload, string name, string location, string MushroomID)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();
            ApplicationDbContext UserORM = new ApplicationDbContext();

            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            if (fileUpload != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileUpload.FileName, fileUpload.InputStream)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                JObject jsonData = (JObject)uploadResult.JsonObj;

                var locationService = new GoogleLocationService();

                var point = locationService.GetLatLongFromAddress(location);

                string email = UserORM.Users.Find(User.Identity.GetUserId()).Email;

                string commonName = ORM.Mushrooms.Find(MushroomID).CommonName;

                
                ORM.UserMushrooms.Add(new UserMushroom(jsonData["secure_url"].ToString(), location, User.Identity.GetUserId(), MushroomID, name, point.Latitude.ToString(), point.Longitude.ToString(), email, commonName));

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

            ApplicationUser user = UserORM.Users.Find(Id);
            List<UserMushroom> list = ORM.UserMushrooms.Where(x => x.UserID == Id).ToList();

            ViewBag.user = user;
            ViewBag.UserMushrooms = list;
            return View("User");

        }
    }
}