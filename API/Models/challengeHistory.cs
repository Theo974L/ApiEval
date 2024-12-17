using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class challengeHistory
    {

        [Key]
        public int id { get; set; }

        public int idChallenge { get; set; }
        
        public int idUser { get; set; }

        public bool? valide { get; set; }

        public challenge challenge { get; set; }
        public users user { get; set; }


    }

    public class DTOChallengeHistory
    {
        [Key]
        public int id { get; set; }

        public int idChallenge { get; set; }

        public int idUser { get; set; }

        public bool? valide { get; set; }
    }
}
