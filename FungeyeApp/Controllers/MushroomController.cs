using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using FungeyeApp.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

namespace FungeyeApp.Controllers
{
    public class MushroomController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult IdentifyMushrooms(string Id)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.CapChars = DAL.GetAllMushrooms().Select(x => x.CapChar).Distinct().ToList();
            ViewBag.CapColors = DAL.GetAllMushrooms().Select(x => x.CapColor).Distinct().ToList();
            ViewBag.Stems = DAL.GetAllMushrooms().Select(x => x.Stem).Distinct().ToList();
            ViewBag.Edibility = DAL.GetAllMushrooms().Select(x => x.Edibility).Distinct().ToList();

            ViewBag.User = DAL.GetUser(Id);
            ViewBag.Mushrooms = DAL.GetAllMushrooms();
            return View();  
        }

        [HttpPost]
        public ContentResult FilterResults(string capChar, string capColor, string stem, string edibility)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<Mushroom> results = DAL.GetAllMushrooms();

            if (!string.IsNullOrEmpty(capChar)&&capChar != "null")
            {
                results = results.Where(x => x.CapChar.ToLower() == capChar.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(capColor) && capColor != "null")
            {
                results = results.Where(x => x.CapColor.ToLower() == capColor.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(stem) && stem != "null")
            {
                results = results.Where(x => x.Stem.ToLower() == stem.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(edibility) && edibility != "null")
            {
                results = results.Where(x => x.Edibility.ToLower() == edibility.ToLower()).ToList();
            }

            var list = JsonConvert.SerializeObject(results,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

            return Content(list, "application/json");
        }
        
        public ActionResult ListSpecificMushroom(string mushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.Mushroom = DAL.GetMushroomById(mushroomID);

            List<UserMushroom> UserMushrooms = DAL.GetUserMushroomsByMushroomId(mushroomID);

            if (UserMushrooms.Count > 0)
            {
                string result = "";
                for (int i = 0; i < UserMushrooms.Count; i++)
                {
                    result += $"{{ \"title\": \"{UserMushrooms[i].MushroomID}\", \"lat\": {UserMushrooms[i].Latitude}, \"lng\": {UserMushrooms[i].Longitude}, \"description\": \"{UserMushrooms[i].UserDescription}\", \"address\": \"{UserMushrooms[i].Address}\", \"ImageLink\": \"{UserMushrooms[i].PictureURL}\", \"email\": \"{UserMushrooms[i].Email}\", \"UserID\": \"{UserMushrooms[i].UserID}\"}},";
                }

                string resul = result.Substring(0, result.Length - 1);
                string json = $"[{resul}]";

                ViewBag.Json = json;
                ViewBag.UserMushrooms = UserMushrooms;
                ViewBag.CurrentUser = User.Identity.GetUserId();
            }

            ViewBag.Key = DAL.GoogleKey;

            return View("MushroomView");
        }

        [Authorize]
        public ActionResult AddMushroomToDB()
        {
            return View("AddMushroom");
        }

        [Authorize]
        public ActionResult AddMushroom(HttpPostedFileBase fileUpload, string address, string userDescription, string species, string commonName, string capChar, string capColor, string stem, string stemColor, string hymenium, string hymeniumColor, string sporeColor, string ecology, string substrate, string growthPattern)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            if (fileUpload == null)
            {
                ViewBag.ErrorMessage = "We were unable to upload your image. Please try again.";
                return View("AddMushroom");
            }

            string pictureURL = DAL.UploadImage(fileUpload);

            string mushroomID = DAL.AddMushroom(species, commonName, capChar, capColor, stem, stemColor, hymenium, hymeniumColor, sporeColor, ecology, substrate, growthPattern, pictureURL, User.Identity.GetUserId());

            DAL.AddUserMushroom(userDescription, address, mushroomID, pictureURL, User.Identity.GetUserName(), User.Identity.GetUserId());

            return RedirectToAction("IdentifyMushrooms");
        }

        public ActionResult UserMushroomMapView()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> userMushrooms = DAL.GetAllUserMushrooms();
            string result = "";
            for (int i = 0; i < userMushrooms.Count; i++)
            {
                result += $"{{ \"title\": \"{userMushrooms[i].MushroomID}\", \"lat\": {userMushrooms[i].Latitude}, \"lng\": {userMushrooms[i].Longitude}, \"description\": \"{userMushrooms[i].UserDescription}\", \"address\": \"{userMushrooms[i].Address}\", \"ImageLink\": \"{userMushrooms[i].PictureURL}\"}},";
            }

            string resul = result.Substring(0, result.Length - 1);
            string json = $"[{resul}]";

            ViewBag.Json = json;

            return View();
        }

        [Authorize]
        public ActionResult DeleteUserMushroom(string pictureURL)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            DAL.DeleteUserMushroom(pictureURL, User.Identity.GetUserId());

            return RedirectToAction("IdentifyMushrooms");
        }

        [Authorize]
        public ActionResult UpdateMushroom(string mushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.Mushroom = DAL.GetMushroomById(mushroomID);

            return View("UpdateMushroomForm");
        }

        [Authorize]
        public ActionResult SaveUpdatedMushroom(Mushroom updatedMushroom)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            DAL.UpdateMushroom(updatedMushroom);

            return RedirectToAction("IdentifyMushrooms");
        }
    }
}