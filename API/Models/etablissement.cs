using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class etablissement
    {
        [Key]
        public int idetab { get; set; }

        [Required, MaxLength(60)]
        public string e_Nom { get; set; }

        // Navigation Property
        public ICollection<users> users { get; set; }
    }
}
