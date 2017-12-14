using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FungeyeApp.Models;
using GoogleMaps.LocationServices;
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
   public class UniqueMushroom
    {
        public string Email { get; set; }
        public int Count { get; set; }
    }
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

        public bool ValidateAddress(string address)
        {
            GoogleLocationService locationService = new GoogleLocationService();

            MapPoint point = locationService.GetLatLongFromAddress(address);

            return point != null;
        }

        public void AddUserMushroom(string userDescription, string address, string mushroomID, string pictureURL, string email, string userID)
        {
            GoogleLocationService locationService = new GoogleLocationService();

            MapPoint point = locationService.GetLatLongFromAddress(address);

            string commonName = ORM.Mushrooms.Find(mushroomID).CommonName;

            ORM.UserMushrooms.Add(new UserMushroom(pictureURL, address, userID, mushroomID, userDescription, point.Latitude.ToString(), point.Longitude.ToString(), email, commonName));

            ORM.SaveChanges();

            //UserORM.Users.Find(userID).TotalMushrooms += UserORM.Users.Find(userID).TotalMushrooms;

            //if (ORM.UserMushrooms.Where(x => x.UserID == userID).Select(x => x.MushroomID).ToList().Contains(mushroomID))
            //{
            //    UserORM.Users.Find(userID).UniqueMushrooms += UserORM.Users.Find(userID).UniqueMushrooms;
            //}

            UserORM.SaveChanges();
        }

        public string AddMushroom(string species, string commonName, string capChar, string capColor, string stem, string stemColor, string hymenium, string hymeniumColor, string sporeColor, string ecology, string substrate, string growthPattern, string pictureURL, string userID)
        {
            string mushID = Convert.ToString((int.Parse(ORM.Mushrooms.ToList().Last().MushroomID)) + 1);

            while (mushID.Length < 3)
            {
                mushID = "0" + mushID;
            }

            ORM.Mushrooms.Add(new Mushroom(species, commonName, capChar, null, capColor, stem, stemColor, hymenium, null, hymeniumColor, sporeColor, null, ecology, null, substrate, growthPattern, null, mushID, "inedible", null, pictureURL));

            ORM.SaveChanges();

            //UserORM.Users.Find(userID).UniqueMushrooms += UserORM.Users.Find(userID).UniqueMushrooms;

            UserORM.SaveChanges();

            return mushID;
        }

        public List<UniqueMushroom> GetUniqueMushroomCount()
    {


            List<UniqueMushroom> qres = (from so in ORM.UserMushrooms
                   group so by so.Email into TotaledOrders
                   select new UniqueMushroom
                   {
                       Email = TotaledOrders.Key,
                       Count = TotaledOrders.Select(s => s.MushroomID).Distinct().Count()

                   }).ToList();

           

            return qres;

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

        public List<ApplicationUser> GetAllUsers()
        {
            return UserORM.Users.ToList();
        }

        public void DeleteUserMushroom(string pictureURL, string userID)
        {
            ORM.UserMushrooms.Remove(ORM.UserMushrooms.Find(pictureURL));

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