using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class question
    {
        [Key]
        public int idQuestion { get; set; }

        public int idChal { get; set; }

        [Required, MaxLength(100)]
        public string q_question { get; set; }

        [Required, MaxLength(100)]
        public string q_reponse { get; set; }

        public int q_point { get; set; }

        // Navigation Property
        public challenge challenge { get; set; }
    }
}
