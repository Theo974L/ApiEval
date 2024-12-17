using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class challenge_Done
    {
        [Key]
        public int idUser { get; set; }

        [Key]
        public int idChal { get; set; }

        public bool cd_done { get; set; }

        public int? cd_step { get; set; }

        public int idImgChal { get; set; }

        // Navigation Properties
        public challenge challenge { get; set; }
        public users user { get; set; }
    }
}
