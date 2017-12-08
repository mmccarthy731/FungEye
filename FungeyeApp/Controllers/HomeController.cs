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
        string APIKey = ConfigurationManager.AppSettings.Get("APIKey");
        string APISecret = ConfigurationManager.AppSettings.Get("APISecret");
        string ServerName = "fungeye";
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
            UserMushroom newItem = new UserMushroom();

            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            //var address = input;
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);
            double latitude = point.Latitude;
            double longitude = point.Longitude;

            if (fileUpload != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileUpload.FileName, fileUpload.InputStream)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                JObject jsonData = (JObject)uploadResult.JsonObj;


                newItem.UserID = User.Identity.GetUserId();
                newItem.MushroomID = mushroomid;
                newItem.PictureURL = jsonData["secure_url"].ToString();
                newItem.Address = address;
                newItem.UserDescription = name;
                newItem.Latitude = latitude.ToString();
                newItem.Longitude = longitude.ToString();

                ORM.UserMushrooms.Add(newItem);
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

        public ActionResult Contact2(string input)
        {

            var address = input;
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);
            var latitude = point.Latitude;
            var longitude = point.Longitude;

            ViewBag.Lat = latitude;
            ViewBag.Long = longitude;
            ViewBag.Input = input;

            return View("UploadImage");
        }
    }
}