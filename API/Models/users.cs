using System.ComponentModel.DataAnnotations;


namespace API.Models
{
    public class users
    {
        [Key]
        public int idUser { get; set; }

        [Required, MaxLength(20)]
        public string u_Nom { get; set; }

        [Required, MaxLength(50)]
        public string u_Mail { get; set; }

        [Required, MaxLength(255)]
        public string u_Password { get; set; }

        public int? idgroupe { get; set; }

        public int? u_NbPoint { get; set; }

        public int? u_idEtab { get; set; }

        public int? idStatus { get; set; }

        // Navigation Properties
        public status status { get; set; }
        public etablissement etablissement { get; set; }
        public ICollection<challenge_Done> challenge_Done { get; set; }
        public ICollection<imageChallenge> imageChallenges { get; set; }
    }

    public class UserCreateDTO
    {
        public string u_Nom { get; set; }
        public string u_Mail { get; set; }
        public string u_Password { get; set; }
        public int idgroupe { get; set; }
        public int u_NbPoint { get; set; }
        public int u_idEtab { get; set; }
        public int idStatus { get; set; }

    }
}
