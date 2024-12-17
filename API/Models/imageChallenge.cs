using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class imageChallenge
    {
        [Key]
        public int id_ic { get; set; }

        [Required]
        public string ic_image { get; set; }

        [MaxLength(200)]
        public string ic_desc { get; set; }

        public int idUser_catch { get; set; }

        // Navigation Property
        public users userCatch { get; set; }
    }
}
