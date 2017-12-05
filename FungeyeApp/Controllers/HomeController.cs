using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Account account = new Account(ServerName, APIKey, APISecret); //hide API info
            Cloudinary cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(@"http://www.dole.com/~/media/Mastheads/All%20Fruit%20Final.jpg")//set string to input variable of user uploaded image (works with both URLs and file paths)

            };
            var uploadResult = cloudinary.Upload(uploadParams);


            return View();
        }

        
    }
}