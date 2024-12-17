using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class status
    {
        [Key]
        public int idStatus { get; set; }

        [Required, MaxLength(15)]
        public string s_Nom { get; set; }

        // Navigation Properties
        public ICollection<users> Users { get; set; }
        public ICollection<challenge> Challenges { get; set; }
    }
}
