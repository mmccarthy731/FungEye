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

            return View("MushroomView");
        }
    }
}