using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class challenge
    {
        [Key]
        public int idChal { get; set; }

        public int c_Status { get; set; }

        [Required, MaxLength(50)]
        public string c_libelle { get; set; }

        [Required, MaxLength(50)]
        public string c_Desc { get; set; }

        public int c_NbPoint { get; set; }

        public int c_step { get; set; }

        // Navigation Property
        public status status { get; set; }
        public ICollection<challenge_Done> challenge_Done { get; set; }
        public ICollection<question> questions { get; set; }
    }

    public class ChallengeCreateDTO
    {
        public int c_Status { get; set; }
        public string c_libelle { get; set; }
        public string c_Desc { get; set; }
        public int c_NbPoint { get; set; }
        public int c_step { get; set; }
    }
}
