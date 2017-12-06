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

        public ActionResult Contact()
        {
            

            return View();
        }

        public ActionResult UploadImage(HttpPostedFileBase fileUpload, string imageName)
        {

            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            if (fileUpload != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileUpload.FileName, fileUpload.InputStream),
                    PublicId = imageName
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                JObject jsonData = (JObject)uploadResult.JsonObj;

                ViewBag.Upload = jsonData["secure_url"];
            } else
            {
                ViewBag.Upload = "FAIL";
            }

            return View();
        }

        
    }
}