﻿using CloudinaryDotNet.Actions;
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
using System.Data.Entity;

namespace FungeyeApp.Controllers
{
    public class MushroomController : Controller
    {
        public string APIKey = ConfigurationManager.AppSettings.Get("APIKey");
        public string APISecret = ConfigurationManager.AppSettings.Get("APISecret");
        public string ServerName = "fungeye";

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

        [Authorize]
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

                string mushID = Convert.ToString((int.Parse(ORM.Mushrooms.ToList().Last().MushroomID)) + 1);

                while (mushID.Length < 3)
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

        [Authorize]
        public ActionResult DeleteMushroom(string MushroomID)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            ORM.UserMushrooms.RemoveRange(ORM.UserMushrooms.Where(x => x.MushroomID == MushroomID));
            ORM.Mushrooms.Remove(ORM.Mushrooms.Find(MushroomID));
            ORM.SaveChanges();

            return RedirectToAction("IdentifyMushrooms");
        }

        [Authorize]
        public ActionResult UpdateMushroom(string MushroomID)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();
            ViewBag.Mushroom = ORM.Mushrooms.Find(MushroomID);
            return View("UpdateMushroomForm");
        }

        [Authorize]
        public ActionResult SaveUpdatedMushroom(Mushroom updatedMushroom)
        {
            FungeyeDBEntities ORM = new FungeyeDBEntities();

            Mushroom toBeUpdated = ORM.Mushrooms.Find(updatedMushroom.MushroomID);
            foreach (UserMushroom userMush in ORM.UserMushrooms.Where(x => x.CommonName == toBeUpdated.CommonName).ToList())
            {
                userMush.CommonName = updatedMushroom.CommonName;
            }
            toBeUpdated = updatedMushroom;
            ORM.Entry(toBeUpdated).State = EntityState.Modified;
            ORM.SaveChanges();
            
            return RedirectToAction("IdentifyMushrooms");
        }
    }
}