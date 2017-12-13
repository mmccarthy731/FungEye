﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FungeyeApp.Models;
using GoogleMaps.LocationServices;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FungeyeApp.Controllers
{
    public class FungeyeDAL : Controller
    {
        FungeyeDBEntities ORM = new FungeyeDBEntities();
        ApplicationDbContext UserORM = new ApplicationDbContext();
        public string APIKey = ConfigurationManager.AppSettings.Get("CloudinaryKey");
        public string APISecret = ConfigurationManager.AppSettings.Get("CloudinarySecret");
        public string GoogleKey = ConfigurationManager.AppSettings.Get("GoogleKey");
        public string ServerName = "fungeye";

        public string UploadImage(HttpPostedFileBase fileUpload)
        {
            Account account = new Account(ServerName, APIKey, APISecret);
            Cloudinary cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileUpload.FileName, fileUpload.InputStream)
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            JObject jsonData = (JObject)uploadResult.JsonObj;

            return jsonData["secure_url"].ToString();
        }

        public void AddUserMushroom(string userDescription, string address, string mushroomID, string pictureURL, string email, string userID)
        {
            var locationService = new GoogleLocationService();

            var point = locationService.GetLatLongFromAddress(address);

            string commonName = ORM.Mushrooms.Find(mushroomID).CommonName;

            ORM.UserMushrooms.Add(new UserMushroom(pictureURL, address, userID, mushroomID, userDescription, point.Latitude.ToString(), point.Longitude.ToString(), email, commonName));

            ORM.SaveChanges();
        }

        public string AddMushroom(string species, string commonName, string capChar, string capColor, string stem, string stemColor, string hymenium, string hymeniumColor, string sporeColor, string ecology, string substrate, string growthPattern, string pictureURL)
        {
            string mushID = Convert.ToString((int.Parse(ORM.Mushrooms.ToList().Last().MushroomID)) + 1);
            while (mushID.Length < 3)
            {
                mushID = "0" + mushID;
            }

            ORM.Mushrooms.Add(new Mushroom(species, commonName, capChar, null, capColor, stem, stemColor, hymenium, null, hymeniumColor, sporeColor, null, ecology, null, substrate, growthPattern, null, mushID, "inedible", null, pictureURL));
            ORM.SaveChanges();

            return mushID;
        }

        public List<Mushroom> GetAllMushrooms()
        {
            return ORM.Mushrooms.ToList();
        }

        public Mushroom GetMushroomById(string mushroomID)
        {
            return ORM.Mushrooms.Find(mushroomID);
        }

        public List<UserMushroom> GetAllUserMushrooms()
        {
            return ORM.UserMushrooms.ToList();
        }

        public List<UserMushroom> GetUserMushroomsByUserId(string id)
        {
            return ORM.UserMushrooms.Where(x => x.UserID == id).ToList();
        }

        public List<UserMushroom> GetUserMushroomsByMushroomId(string MushroomID)
        {
            return ORM.UserMushrooms.Where(x => x.MushroomID == MushroomID).ToList();
        }

        public ApplicationUser GetUser(string id)
        {
            return UserORM.Users.Find(id);
        }

        public void DeleteMushroom(string mushroomID)
        {
            ORM.UserMushrooms.RemoveRange(ORM.UserMushrooms.Where(x => x.MushroomID == mushroomID));
            ORM.Mushrooms.Remove(ORM.Mushrooms.Find(mushroomID));
            ORM.SaveChanges();
        }

        public void UpdateMushroom(Mushroom updatedMushroom)
        {
            Mushroom toBeUpdated = ORM.Mushrooms.Find(updatedMushroom.MushroomID);

            foreach (UserMushroom userMush in ORM.UserMushrooms.Where(x => x.CommonName == toBeUpdated.CommonName).ToList())
            {
                userMush.CommonName = updatedMushroom.CommonName;
            }

            toBeUpdated.Species = updatedMushroom.Species;
            toBeUpdated.CommonName = updatedMushroom.CommonName;
            toBeUpdated.CapChar = updatedMushroom.CapChar;
            toBeUpdated.NextCapChar = updatedMushroom.NextCapChar;
            toBeUpdated.CapColor = updatedMushroom.CapColor;
            toBeUpdated.Stem = updatedMushroom.Stem;
            toBeUpdated.StemColor = updatedMushroom.StemColor;
            toBeUpdated.Hymenium = updatedMushroom.Hymenium;
            toBeUpdated.Attachment = updatedMushroom.Attachment;
            toBeUpdated.HymeniumColor = updatedMushroom.HymeniumColor;
            toBeUpdated.SporeColor = updatedMushroom.SporeColor;
            toBeUpdated.CapColor = updatedMushroom.CapColor;
            toBeUpdated.Annulus = updatedMushroom.Annulus;
            toBeUpdated.Ecology = updatedMushroom.Ecology;
            toBeUpdated.NewEcology = updatedMushroom.NewEcology;
            toBeUpdated.Substrate = updatedMushroom.Substrate;
            toBeUpdated.GrowthPattern = updatedMushroom.GrowthPattern;
            toBeUpdated.NewGrowthPattern = updatedMushroom.NewGrowthPattern;

            ORM.Entry(toBeUpdated).State = EntityState.Modified;
            ORM.SaveChanges();
        }
    }
}