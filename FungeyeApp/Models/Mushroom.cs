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
    using System;
    using System.Collections.Generic;

    public partial class Mushroom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mushroom()
        {
            this.UserMushrooms = new HashSet<UserMushroom>();
        }

        public string Species { get; set; }
        public string CommonName { get; set; }
        public string CapChar { get; set; }
        public string NextCapChar { get; set; }
        public string CapColor { get; set; }
        public string Stem { get; set; }
        public string StemColor { get; set; }
        public string Hymenium { get; set; }
        public string Attachment { get; set; }
        public string HymeniumColor { get; set; }
        public string SporeColor { get; set; }
        public string Annulus { get; set; }
        public string Ecology { get; set; }
        public string NewEcology { get; set; }
        public string Substrate { get; set; }
        public string GrowthPattern { get; set; }
        public string NewGrowthPattern { get; set; }
        public string MushroomID { get; set; }
        public string Edibility { get; set; }
        public string Description { get; set; }
        public string PictureURL { get; set; }

        public Mushroom(string Species, string CommonName, string CapChar, string NextCapChar, string CapColor, string Stem, string StemColor, string Hymenium, string Attachment, string HymeniumColor, string SporeColor, string Annulus, string Ecology, string NewEcology, string Substrate, string GrowthPattern, string NewGrowthPattern, string MushroomID, string Edibility, string Description, string PictureURL)
        {
            this.Species = Species;
            this.CommonName = CommonName;
            this.CapChar = CapChar;
            this.NextCapChar = NextCapChar;
            this.CapColor = CapColor;
            this.Stem = Stem;
            this.StemColor = StemColor;
            this.Hymenium = Hymenium;
            this.Attachment = Attachment;
            this.HymeniumColor = HymeniumColor;
            this.SporeColor = SporeColor;
            this.Annulus = Annulus;
            this.Ecology = Ecology;
            this.NewEcology = NewEcology;
            this.Substrate = Substrate;
            this.GrowthPattern = GrowthPattern;
            this.NewGrowthPattern = NewGrowthPattern;
            this.MushroomID = MushroomID;
            this.Edibility = Edibility;
            this.Description = Description;
            this.PictureURL = PictureURL;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMushroom> UserMushrooms { get; set; }
    }
}
