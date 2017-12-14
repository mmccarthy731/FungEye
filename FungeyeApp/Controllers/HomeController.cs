using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FungeyeApp.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace FungeyeApp.Controllers
{

    public class Leaderboard
    {
        public string Email { set; get; }
        public int TotalCount { set; get; }
        public int UniqueCount { set; get; }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult UploadImage(string mushroomID)
        {
            ViewBag.MushroomID = mushroomID;
            return View();
        }

        [Authorize]
        public ActionResult UploadUserImage(string id)
        {
            return View();
        }

        public ActionResult AddPictureToUser(HttpPostedFileBase fileUpload)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            DAL.UpdateUserPic(fileUpload, User.Identity.GetUserId());

            return RedirectToAction("GetUserInfo", new { id = User.Identity.GetUserId() });
        }

        [Authorize]
        public ActionResult ViewUploadedImage(HttpPostedFileBase fileUpload, string userDescription, string address, string mushroomID)
        {
            FungeyeDAL DAL = new FungeyeDAL();
            if (fileUpload == null)
            {
                ViewBag.ErrorMessage = "We were unable to upload your image. Please try again.";
                return View("UploadImage");
            }
            else if (!DAL.ValidateAddress(address))
            {
                ViewBag.ErrorMessage = "Address entered was invalid. Please try again.";
                return View("UploadImage");
            }

            string pictureURL = DAL.UploadImage(fileUpload);

            DAL.AddUserMushroom(userDescription, address, mushroomID, pictureURL, User.Identity.GetUserName(), User.Identity.GetUserId());

            ViewBag.Upload = pictureURL;
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult GetUserInfo(string id)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            if (DAL.GetUserMushroomsByUserId(id).Count > 0)
            {
                List<UserMushroom> userMushrooms = DAL.GetUserMushroomsByUserId(id);
                string result = "";
                for (int i = 0; i < userMushrooms.Count; i++)
                {
                    result += $"{{ \"MushroomID\": \"{userMushrooms[i].MushroomID}\", \"lat\": {userMushrooms[i].Latitude}, \"lng\": {userMushrooms[i].Longitude}, \"description\": \"{userMushrooms[i].UserDescription}\", \"address\": \"{userMushrooms[i].Address}\", \"ImageLink\": \"{userMushrooms[i].PictureURL}\", \"email\": \"{userMushrooms[i].Email}\", \"id\": \"{userMushrooms[i].UserID}\", \"CommonName\": \"{userMushrooms[i].CommonName}\"}},";
                }

                string resul = result.Substring(0, result.Length - 1);
                string json = $"[{resul}]";

                ViewBag.Json = json;
            }

            ViewBag.Key = DAL.GoogleKey;
            ViewBag.User = DAL.GetUser(id);
            ViewBag.UserMushrooms = DAL.GetUserMushroomsByUserId(id);
            ViewBag.CurrentUser = User.Identity.GetUserId();
            return View("User");
        }

        public ActionResult ViewLeaderboard()
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> emails = DAL.GetAllUserMushrooms();
            //select Email, count(distinct MushroomID) [UniqueIds], count(mushroomId) [Total] from UserMushroom group by email
            var groups = emails.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).Reverse().ToList();

            var uniqueC = DAL.GetUniqueMushroomCount();

            var lst =
                 (from res in groups
                  join res2 in uniqueC
                  on res.EmailName equals res2.Email
                  select new Leaderboard
                  {
                      Email = res.EmailName,
                      TotalCount = res.EmailCount,
                      UniqueCount = res2.Count
                  }).ToList();

            ViewBag.Leaderboard = lst.OrderByDescending(x => x.TotalCount).ToList();

            return View();
        }

        public ContentResult SortLeaderboard(string sortOption)
        {
            FungeyeDAL DAL = new FungeyeDAL();

            List<UserMushroom> userMushrooms = DAL.GetAllUserMushrooms();

            var groups = userMushrooms.GroupBy(x => x.Email).Select(x => new { EmailName = x.Key, EmailCount = x.Count() }).OrderBy(x => x.EmailCount).Reverse().ToList();

            var uniqueC = DAL.GetUniqueMushroomCount();

            var lst =
                 (from res in groups
                  join res2 in uniqueC
                  on res.EmailName equals res2.Email
                  select new Leaderboard
                  {
                      Email = res.EmailName,
                      TotalCount = res.EmailCount,
                      UniqueCount = res2.Count
                  }).ToList();

            if (sortOption == "totalCount")
            {
                lst = lst.OrderByDescending(x => x.TotalCount).ToList();
            }
            else if (sortOption == "uniqueCount")
            {
                lst = lst.OrderByDescending(x => x.UniqueCount).ToList();
            }
            else
            {
                lst = lst.OrderBy(x => x.Email).ToList();
            }

            var list = JsonConvert.SerializeObject(lst,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return Content(list, "application/json");
        }
    }
}