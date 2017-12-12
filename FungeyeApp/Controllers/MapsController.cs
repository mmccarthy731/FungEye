﻿using System.Collections.Generic;
using System.Web.Mvc;
using FungeyeApp.Models;


namespace FungeyeApp.Controllers
{
    public class MapsController : Controller
    {
        public string GoogleKey = ConfigurationManager.AppSettings.Get("GoogleKey");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Maps()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> userMushrooms = DAL.GetAllUserMushrooms();
            string result = "";
            for (int i = 0; i < userMushrooms.Count; i++)
            {
                result += $"{{ \"title\": \"{userMushrooms[i].MushroomID}\", \"lat\": {userMushrooms[i].Latitude}, \"lng\": {userMushrooms[i].Longitude}, \"description\": \"{userMushrooms[i].UserDescription}\", \"address\": \"{userMushrooms[i].Address}\", \"ImageLink\": \"{userMushrooms[i].PictureURL}\", \"email\": \"{userMushrooms[i].Email}\", \"UserID\": \"{userMushrooms[i].UserID}\"}},";
            }

            string resul = result.Substring(0, result.Length - 1);
            string json = $"[{resul}]";

            ViewBag.json = json;
            ViewBag.Key = GoogleKey;
            return View();
        }
    }
}