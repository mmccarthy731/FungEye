using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using FungeyeApp.Models;
using Newtonsoft.Json;

namespace FungeyeApp.Controllers
{
    public class MushroomController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult IdentifyMushrooms()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.CapChars = DAL.GetAllMushrooms().Select(x => x.CapChar).Distinct().ToList();
            ViewBag.CapColors = DAL.GetAllMushrooms().Select(x => x.CapColor).Distinct().ToList();
            ViewBag.Stems = DAL.GetAllMushrooms().Select(x => x.Stem).Distinct().ToList();

            return View();
        }

        [HttpPost]
        public ContentResult FilterResults(string CapChar, string CapColor, string Stem)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<Mushroom> results = DAL.GetAllMushrooms();

            if (!string.IsNullOrEmpty(CapChar)&&CapChar != "null")
            {
                results = results.Where(x => x.CapChar == CapChar).ToList();
            }

            if (!string.IsNullOrEmpty(CapColor) && CapColor != "null")
            {
                results = results.Where(x => x.CapColor == CapColor).ToList();
            }

            if (!string.IsNullOrEmpty(Stem) && Stem != "null")
            {
                results = results.Where(x => x.Stem == Stem).ToList();
            }

            var list = JsonConvert.SerializeObject(results,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

            return Content(list, "application/json");
        }
        
        public ActionResult ListSpecificMushroom(string MushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.Mushroom = DAL.GetMushroomById(MushroomID);

            List<UserMushroom> UserMushrooms = DAL.GetUserMushroomsByMushroomId(MushroomID);

            if (UserMushrooms.Count > 0)
            {
                string result = "";
                for (int i = 0; i < UserMushrooms.Count; i++)
                {
                    result += $"{{ \"title\": \"{UserMushrooms[i].MushroomID}\", \"lat\": {UserMushrooms[i].Latitude}, \"lng\": {UserMushrooms[i].Longitude}, \"description\": \"{UserMushrooms[i].UserDescription}\", \"address\": \"{UserMushrooms[i].Address}\", \"ImageLink\": \"{UserMushrooms[i].PictureURL}\"}},";
                }

                string resul = result.Substring(0, result.Length - 1);
                string json = $"[{resul}]";

                ViewBag.json = json;
                ViewBag.UserMushrooms = UserMushrooms;
            }

            return View("MushroomView");
        }

        [Authorize]
        public ActionResult AddMushroomToDB()
        {
            return View("AddMushroom");
        }

        [Authorize]
        public ActionResult AddMushroom(HttpPostedFileBase fileUpload, string Address, string UserDescription, string Species, string CommonName, string CapChar, string CapColor, string Stem, string StemColor, string Hymenium, string HymeniumColor, string SporeColor, string Ecology, string Substrate, string GrowthPattern)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            if (fileUpload == null)
            {
                ViewBag.ErrorMessage = "We were unable to upload your image. Please try again.";
                return View("AddMushroom");
            }

            string pictureURL = DAL.UploadImage(fileUpload);

            string mushroomID = DAL.AddMushroom(Species, CommonName, CapChar, CapColor, Stem, StemColor, Hymenium, HymeniumColor, SporeColor, Ecology, Substrate, GrowthPattern, pictureURL);

            DAL.AddUserMushroom(UserDescription, Address, mushroomID, pictureURL);

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

            ViewBag.json = json;

            return View();
        }

        [Authorize]
        public ActionResult DeleteMushroom(string MushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            DAL.DeleteMushroom(MushroomID);

            return RedirectToAction("IdentifyMushrooms");
        }

        [Authorize]
        public ActionResult UpdateMushroom(string MushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            ViewBag.Mushroom = DAL.GetMushroomById(MushroomID);

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