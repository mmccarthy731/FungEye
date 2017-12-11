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
using System.Data.Entity;
using GoogleMaps.LocationServices;

namespace FungeyeApp.Controllers
{
    public class MapsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Maps()
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            List<UserMushroom> LocationList = ORM.UserMushrooms.ToList();
            string result = "";
            for (int i = 0; i < LocationList.Count; i++)
            {
                result += $"{{ \"title\": \"{LocationList[i].MushroomID}\", \"lat\": {LocationList[i].Latitude}, \"lng\": {LocationList[i].Longitude}, \"description\": \"{LocationList[i].UserDescription}\", \"address\": \"{LocationList[i].Address}\", \"ImageLink\": \"{LocationList[i].PictureURL}\", \"email\": \"{LocationList[i].Email}\", \"UserID\": \"{LocationList[i].UserID}\"}},";
            }

            string resul = result.Substring(0, result.Length - 1);
            string json = $"[{resul}]";

            ViewBag.json = json;

            return View();
        }
    }
}