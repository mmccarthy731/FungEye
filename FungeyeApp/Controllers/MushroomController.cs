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
using System.Data.Entity;

using FungeyeApp.Models;

namespace FungeyeApp.Controllers
{
    public class MushroomController : Controller
    {
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

        public ActionResult DeleteShroom(string MushroomID)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            DbContextTransaction DeleteShroomMoom =
                ORM.Database.BeginTransaction();

            try
            {
                Mushroom Temp = ORM.Mushrooms.Find(MushroomID);

                ORM.Mushrooms.Remove(Temp);

                ORM.SaveChanges();


                DeleteShroomMoom.Commit();
            }

            catch
            {   
                DeleteShroomMoom.Rollback();

            }

            return RedirectToAction("IdentifyMushrooms");
        }
    }
}