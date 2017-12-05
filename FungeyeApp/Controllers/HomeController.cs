using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;

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

        public ActionResult About()
        {
            

            return View();
        }

        public ActionResult Contact(string fileName, string imageName)
        {

            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName),//set string to input variable of user uploaded image (works with both URLs and file paths)
                PublicId = imageName
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            JObject jsonData = (JObject)uploadResult.JsonObj;

            ViewBag.Upload = jsonData["secure_url"];


            return View();
        }

        public ActionResult UploadImage()
        {

            return View();
        }

        
    }
}