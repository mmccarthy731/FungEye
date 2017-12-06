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


        public ActionResult ViewUploadedImage(HttpPostedFileBase fileUpload, string name, string location, string MushroomID)
        {
            FungeyeDBEntities1 ORM = new FungeyeDBEntities1();
            UserMushroom newItem = new UserMushroom();

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

                newItem.UserID = User.Identity.GetUserId();
                newItem.MushroomID = MushroomID;
                newItem.PictureURL = jsonData["secure_url"].ToString();
                newItem.Address = location;
                newItem.Name = name;

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
    }
}