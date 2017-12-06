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

            List<Mushroom> MushroomList = ORM.Mushrooms.ToList();

            List<string> CapChars = ORM.Mushrooms.Select(x => x.CapChar).Distinct().ToList();
            List<string> Stems = ORM.Mushrooms.Select(x => x.Stem).Distinct().ToList();

            ViewBag.CapChars = CapChars;
            ViewBag.Stems = Stems;
            ViewBag.MushroomList = MushroomList;

            return View();
        }

        public ActionResult ListShroomsByCapChar(string CapChar)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<Mushroom> OutputList = new List<Mushroom>();
            ViewBag.CapChars = ORM.Mushrooms.Select(x => x.CapChar == CapChar).ToList();

            OutputList = ORM.Mushrooms.SqlQuery($"select * from Mushrooms where CapChar=@param1",
                new SqlParameter("@param1", CapChar)).ToList();

            ViewBag.MushroomList = OutputList;

            return View("IdentifyMushrooms");
        }
    }
}