using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using GoogleMaps.LocationServices;

using FungeyeApp.Models;
using System.IO;
using Microsoft.AspNet.Identity;

namespace FungeyeApp.Controllers
{
    public class MushroomController : Controller
    {
        public string APIKey = ConfigurationManager.AppSettings.Get("APIKey");
        public string APISecret = ConfigurationManager.AppSettings.Get("APISecret");
        public string ServerName = "fungeye";
        // GET: Mushroom
        public ActionResult Index()
        {
            return View();
        }
        

        public ActionResult IdentifyMushrooms()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            ViewBag.CapChars = ORM.Mushrooms.Select(x => x.CapChar).Distinct().ToList();
            ViewBag.CapColors = ORM.Mushrooms.Select(x => x.CapColor).Distinct().ToList();
            ViewBag.Stems = ORM.Mushrooms.Select(x => x.Stem).Distinct().ToList();
            ViewBag.MushroomList = ORM.Mushrooms.ToList();

            return View();
        }

        public ActionResult FilterResults(string CapChar, string CapColor, string Stem)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();
            List<Mushroom> results = ORM.Mushrooms.ToList();
            ViewBag.CapChars = ORM.Mushrooms.Select(x => x.CapChar).Distinct().ToList();
            ViewBag.CapColors = ORM.Mushrooms.Select(x => x.CapColor).Distinct().ToList();
            ViewBag.Stems = ORM.Mushrooms.Select(x => x.Stem).Distinct().ToList();

            if (CapChar != "null")
            {
                results = ORM.Mushrooms.Where(x => x.CapChar == CapChar).ToList();
            }

            if (CapColor != "null")
            {
                results = results.Where(x => x.CapColor == CapColor).ToList();
            }

            if (Stem != "null")
            {
                results = results.Where(x => x.Stem == Stem).ToList();
            }

            ViewBag.MushroomList = results;

            return View("IdentifyMushrooms");
        }

        public ActionResult ListSpecificMushroom(string MushroomID)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();


            ViewBag.userMushrooms = ORM.UserMushrooms.Where(x => x.MushroomID == MushroomID);

            ViewBag.Mushroom = ORM.Mushrooms.Find(MushroomID);

            List<UserMushroom> LocationList = ORM.UserMushrooms.Where(x => x.MushroomID == MushroomID).ToList();

            if (LocationList == null)
            {
                return View("MushroomView");
            }

            string result = "";
            for (int i = 0; i < LocationList.Count; i++)
            {
                result += $"{{ \"title\": \"{LocationList[i].MushroomID}\", \"lat\": {LocationList[i].Latitude}, \"lng\": {LocationList[i].Longitude}, \"description\": \"{LocationList[i].UserDescription}\", \"address\": \"{LocationList[i].Address}\", \"ImageLink\": \"{LocationList[i].PictureURL}\"}},";
                string resul = result.Substring(0, result.Length - 1);
                string json = $"[{resul}]";

                ViewBag.json = json;
            }
            return View("MushroomView");
        }

        public ActionResult AddMushroomToDB()
        {
            return View("AddMushroom");
        }

        public ActionResult AddMushroom(HttpPostedFileBase fileUpload, string Address, string UserDescription, string Species, string CommonName, string CapChar, string CapColor, string Stem, string StemColor, string Hymenium, string HymeniumColor, string SporeColor, string Ecology, string Substrate, string GrowthPattern)
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

                var point = locationService.GetLatLongFromAddress(Address);

                string mushID = (ORM.Mushrooms.ToList().Count() + 1).ToString();

                if (mushID.Length < 3)
                {
                    mushID = "0" + mushID;
                }

                string email = UserORM.Users.Find(User.Identity.GetUserId()).Email;

                ORM.Mushrooms.Add(new Mushroom(Species, CommonName, CapChar, null, CapColor, Stem, StemColor, Hymenium, null, HymeniumColor, SporeColor, null, Ecology, null, Substrate, GrowthPattern, null, mushID, "inedible", null, jsonData["secure_url"].ToString()));

                ORM.SaveChanges();

                ORM.UserMushrooms.Add(new UserMushroom(jsonData["secure_url"].ToString(), Address, User.Identity.GetUserId(), mushID, UserDescription, point.Latitude.ToString(), point.Longitude.ToString(), email, CommonName));

                ORM.SaveChanges();
            }

            return RedirectToAction("IdentifyMushrooms");
        }
        public ActionResult UserMushroomMapView()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> LocationList = ORM.UserMushrooms.ToList();
            string result = "";
            for (int i = 0; i < LocationList.Count; i++)
            {
                result += $"{{ \"title\": \"{LocationList[i].MushroomID}\", \"lat\": {LocationList[i].Latitude}, \"lng\": {LocationList[i].Longitude}, \"description\": \"{LocationList[i].UserDescription}\", \"address\": \"{LocationList[i].Address}\", \"ImageLink\": \"{LocationList[i].PictureURL}\"}},";
            }

            string resul = result.Substring(0, result.Length - 1);
            string json = $"[{resul}]";

            ViewBag.json = json;

            return View();
        }
    }
}