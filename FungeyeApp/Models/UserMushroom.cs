//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FungeyeApp.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class UserMushroom
    {
        public string PictureURL { get; set; }
        public string Address { get; set; }
        public string UserID { get; set; }
        public string MushroomID { get; set; }
        public string UserDescription { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Email { get; set; }
        public string CommonName { get; set; }

        public UserMushroom(string PictureURL, string Address, string UserID, string MushroomID, string UserDescription, string Latitude, string Longitude, string Email, string CommonName)
        {
            this.PictureURL = PictureURL;
            this.Address = Address;
            this.UserID = UserID;
            this.MushroomID = MushroomID;
            this.UserDescription = UserDescription;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Email = Email;
            this.CommonName = CommonName;
        }

        public UserMushroom()
        {

        }

        [JsonIgnore]
        public virtual Mushroom Mushroom { get; set; }
    }
}