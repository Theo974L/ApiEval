using System.Collections.Generic;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cmp;
using Microsoft.EntityFrameworkCore.Design;


namespace API.ApiDbContexts
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<challenge> Challenges { get; set; }
        public DbSet<challengeHistory> challenge_history { get; set; }

        public DbSet<challenge_Done> Challenge_Done { get; set; }
        public DbSet<etablissement> Etablissements { get; set; }
        public DbSet<imageChallenge> ImageChallenges { get; set; }
        public DbSet<question> Questions { get; set; }
        public DbSet<status> Status { get; set; }
        public DbSet<users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER ...

            modelBuilder.Entity<users>()
                .HasOne(u => u.etablissement) // Un User a un Etablissement
                .WithMany(e => e.users) // Un Etablissement a plusieurs Users
                .HasForeignKey(u => u.u_idEtab) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<users>()
                .HasOne(u => u.status) // Un User a un Status
                .WithMany(s => s.Users) // Un Status a plusieurs Users
                .HasForeignKey(u => u.idgroupe) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);


            //------------------------------------------------

            modelBuilder.Entity<challenge>()
                .HasOne(c => c.status) // Un Challenge a un Status
                .WithMany(s => s.Challenges) // Un Status peut avoir plusieurs Challenges
                .HasForeignKey(c => c.c_Status) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<challenge_Done>()
                .HasKey(cd => new { cd.idUser, cd.idChal }); // Clé composite

                modelBuilder.Entity<challenge_Done>()
                    .HasOne(cd => cd.challenge) // Un ChallengeDone a un Challenge
                    .WithMany(c => c.challenge_Done) // Un Challenge peut avoir plusieurs ChallengeDone
                    .HasForeignKey(cd => cd.idChal) // Clé étrangère
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<challenge_Done>()
                    .HasOne(cd => cd.user) // Un ChallengeDone a un User
                    .WithMany(u => u.challenge_Done) // Un User peut avoir plusieurs ChallengeDone
                    .HasForeignKey(cd => cd.idUser) // Clé étrangère
                    .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<imageChallenge>()
                .HasOne(ic => ic.userCatch) // Un ImageChallenge a un User qui l'a récupéré
                .WithMany(u => u.imageChallenges) // Un User peut avoir plusieurs ImageChallenges
                .HasForeignKey(ic => ic.idUser_catch) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<etablissement>()
                .HasMany(e => e.users) // Un Etablissement a plusieurs Users
                .WithOne(u => u.etablissement) // Un User a un Etablissement
                .HasForeignKey(u => u.u_idEtab) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<question>()
                .HasOne(q => q.challenge) // Une Question a un Challenge
                .WithMany(c => c.questions) // Un Challenge peut avoir plusieurs Questions
                .HasForeignKey(q => q.idChal) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<status>()
                .HasMany(s => s.Users) // Un Status a plusieurs Users
                .WithOne(u => u.status) // Un User a un Status
                .HasForeignKey(u => u.idgroupe) // Clé étrangère
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------------------

            modelBuilder.Entity<challengeHistory>()
                .HasOne(ch => ch.challenge) // challengeHistory a un Challenge
                .WithMany()                 // Challenge n'a pas besoin d'une navigation inverse
                .HasForeignKey(ch => ch.idChallenge) // Utilise idChallenge comme clé étrangère
                .OnDelete(DeleteBehavior.Restrict); // Restriction en cas de suppression

            modelBuilder.Entity<challengeHistory>()
                .HasOne(ch => ch.user) // challengeHistory a un Challenge
                .WithMany()                 // Challenge n'a pas besoin d'une navigation inverse
                .HasForeignKey(ch => ch.idUser) // Utilise idChallenge comme clé étrangère
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}


/**
 * commande pour init la bdd
 * 
 * dotnet ef migrations add InitialCreate
 * dotnet ef database update
 * 
 * 
 * 
 * 
 */
